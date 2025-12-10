using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using HeaderHero.Data;

namespace HeaderHero.Parser;

public class ProgressFeedback
{
    public string Title = "";
    public int Item;
    public int Count;
    public string Message = "";
}

public class Scanner
{
    readonly Project _project;
    ConcurrentDictionary<string, byte> _queued;
    ConcurrentQueue<string> _scan_queue;
    ConcurrentDictionary<string, string> _system_includes;
    ConcurrentDictionary<string, bool> _file_existence;
    bool _scanning_pch;
    bool CaseSensitive { get; }

    public ConcurrentQueue<string> Errors;
    public ConcurrentDictionary<string, byte> NotFound;
    public ConcurrentDictionary<string, string> NotFoundOrigins;

    public Scanner(Project p)
    {
        _project = p;
        CaseSensitive = IsCaseSensitive();
        Clear();
    }

    void Clear()
    {
        _project.Files.Clear();

        _queued = [];
        _scan_queue = [];
        _system_includes = [];
        _file_existence = [];

        Errors = [];
        NotFound = [];
        NotFoundOrigins = [];
    }

    bool IsCaseSensitive()
    {
        foreach (string dir in _project.ScanDirectories)
        {
            DirectoryInfo dl = new DirectoryInfo(dir.ToLowerInvariant());
            DirectoryInfo du = new DirectoryInfo(dir.ToUpperInvariant());
            if (dl.Exists != du.Exists || dl.CreationTime != du.CreationTime || dl.LastAccessTime != du.LastAccessTime)
                return true;
        }
        return false;
    }

    public void Rescan(ProgressFeedback feedback)
    {
        Stopwatch sw = Stopwatch.StartNew();

        Clear();

        feedback.Title = "Scanning precompiled header...";

        // scan everything that goes into precompiled header
        _scanning_pch = true;
        if (!string.IsNullOrEmpty(_project.PrecompiledHeader) && FileExists(_project.PrecompiledHeader))
        {
            var inc = Path.GetFullPath(_project.PrecompiledHeader);
            ScanFile(inc);
            while (!_scan_queue.IsEmpty)
            {
                string[] to_scan = _scan_queue.ToArray();
                _scan_queue.Clear();
                foreach (string fi in to_scan)
                {
                    ScanFile(fi);
                }
            }
            _queued.Clear();
        }
        _scanning_pch = false;

        feedback.Title = "Scanning directories...";
        foreach (string dir in _project.ScanDirectories)
        {
            feedback.Message = dir;
            ScanDirectory(new DirectoryInfo(dir), feedback);
        }

        feedback.Title = "Scanning files...";

        int dequeued = 0;

        while (_scan_queue.Count > 0)
        {
            dequeued += _scan_queue.Count;
            string[] to_scan = _scan_queue.ToArray();
            _scan_queue.Clear();
            foreach (string fi in to_scan)
            {
                feedback.Count = dequeued + _scan_queue.Count;
                feedback.Item++;
                feedback.Message = Path.GetFileName(fi);
                ScanFile(fi);
            }
        }
        _queued.Clear();
        _system_includes.Clear();

        _project.ScanTime = sw.Elapsed;
    }

    void ScanDirectory(DirectoryInfo di, ProgressFeedback feedback)
    {
        FileInfo[] files;
        DirectoryInfo[] subdirs;

        feedback.Message = di.FullName;

        try
        {
            files = di.GetFiles();
            subdirs = di.GetDirectories();
        }
        catch (Exception e)
        {
            Errors.Enqueue($"Cannot descend into {di.FullName}: {e.Message}");
            return;
        }

        foreach (FileInfo file in files)
        {
            if (SourceFile.IsTranslationUnitExtension(file.Extension))
            {
                string fullPath = file.FullName;
                Enqueue(fullPath, CanonicalPath(fullPath));
            }
        }
        foreach (DirectoryInfo subdir in subdirs)
            if (!subdir.Name.StartsWith('.'))
                ScanDirectory(subdir, feedback);
    }

    // On a case-insensitive file system, this returns
    // path that is all lowercase
    string CanonicalPath(string path)
    {
        return CaseSensitive ? path : path.ToLowerInvariant();
    }

    bool FileExists(string path)
    {
        if (_file_existence.TryGetValue(path, out var value))
        {
            return value;
        }

        value = File.Exists(path);
        _file_existence.TryAdd(path, value);
        return value;
    }

    void Enqueue(string fullPath, string abs)
    {
        if (_queued.TryAdd(abs, 0))
        {
            _scan_queue.Enqueue(fullPath);
        }
    }

    void ScanFile(string path)
    {
        path = CanonicalPath(path);
        SourceFile sf;
        if (_project.Files.ContainsKey(path) && !_scanning_pch)
        {
            sf = _project.Files[path];
        }
        else
        {
            Parser.Result res = Parser.ParseFile(path, Errors);
            sf = new SourceFile
            {
                Lines = res.Lines,
                LocalIncludes = res.LocalIncludes,
                SystemIncludes = res.SystemIncludes,
                Precompiled = _scanning_pch
            };
            _project.Files[path] = sf;
        }

        sf.AbsoluteIncludes.Clear();

        string local_dir = Path.GetDirectoryName(path) ?? string.Empty;
        foreach (string s in sf.LocalIncludes) {
            try
            {
                string inc = Path.GetFullPath(Path.Combine(local_dir, s));
                string abs = CanonicalPath(inc);
                // found a header that's part of PCH during regular scan: ignore it
                if (!_scanning_pch && _project.Files.TryGetValue(abs, out SourceFile value) && value.Precompiled)
                {
                    continue;
                }
                if (!FileExists(inc))
                {
                    if (!sf.SystemIncludes.Contains(s))
                        sf.SystemIncludes.Add(s);
                    continue;
                }
                sf.AbsoluteIncludes.Add(abs);
                Enqueue(inc, abs);
            }
            catch (Exception e)
            {
                Errors.Enqueue($"Exception: \"{e.Message}\" for #include \"{s}\"");
            }
        }

        foreach (string s in sf.SystemIncludes)
        {
            try
            {
                if (_system_includes.ContainsKey(s))
                {
                    string abs = _system_includes[s];
                    // found a header that's part of PCH during regular scan: ignore it
                    if (!_scanning_pch && _project.Files.ContainsKey(abs) && _project.Files[abs].Precompiled)
                    {
                        continue;
                    }
                    sf.AbsoluteIncludes.Add(abs);
                }
                else
                {
                    string found = null;

                    foreach (string dir in _project.IncludeDirectories)
                    {
                        found = Path.GetFullPath(Path.Combine(dir, s));
                        if (FileExists(found))
                            break;
                        found = null;
                    }

                    if (found != null)
                    {
                        string abs = CanonicalPath(found);
                        // found a header that's part of PCH during regular scan: ignore it
                        if (!_scanning_pch && _project.Files.ContainsKey(abs) && _project.Files[abs].Precompiled)
                        {
                            continue;
                        }

                        sf.AbsoluteIncludes.Add(abs);
                        _system_includes[s] = abs;
                        Enqueue(found, abs);
                    }
                    else
                    {
                        if (NotFound.TryAdd(s, 0))
                        {
                            NotFoundOrigins.TryAdd(s, path);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Errors.Enqueue($"Exception: \"{e.Message}\" for #include <{s}>");
            }

        }

        // Only treat each include as done once. Since we completely ignore preprocessor, for patterns like
        // this we'd end up having same file in includes list multiple times. Let's assume that all includes use
        // pragma once or include guards and are only actually parsed just once.
        //   #if FOO
        //   #include <bar>
        //   #else
        //   #include <bar>
        //   #endif
        sf.AbsoluteIncludes = sf.AbsoluteIncludes.Distinct().ToList();
    }
}