using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
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

    ConcurrentDictionary<string, byte> _nextPass;

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
        _nextPass = [];
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
        string[] currentPass = null;

        feedback.Title = "Scanning precompiled header...";

        // scan everything that goes into precompiled header
        _scanning_pch = true;
        if (!string.IsNullOrEmpty(_project.PrecompiledHeader) && FileExists(_project.PrecompiledHeader))
        {
            var inc = Path.GetFullPath(_project.PrecompiledHeader);
            ScanFile(inc);

            currentPass = _nextPass.Keys.ToArray();
            _nextPass = [];
            while (currentPass.Length > 0)
            {
                foreach (string path in currentPass)
                    ScanFile(path);
                currentPass = _nextPass.Keys.ToArray();
                _nextPass = [];
            }
            _queued.Clear();
        }
        _scanning_pch = false;

        feedback.Title = "Scanning directories...";
        foreach (string dir in _project.ScanDirectories)
        {
            ScanDirForSourceFilesRecurse(new DirectoryInfo(dir));
        }

        feedback.Title = "Scanning headers...";
        foreach (string dir in _project.IncludeDirectories)
        {
            ScanDirForHeaders(new DirectoryInfo(dir));
        }

        feedback.Title = "Scanning files...";

        ParallelOptions parOptions = new ParallelOptions { MaxDegreeOfParallelism = Math.Min(Environment.ProcessorCount, 4) };

        currentPass = _nextPass.Keys.ToArray();
        _nextPass = [];
        int processedCounter = 0;
        while (currentPass.Length > 0)
        {
            Parallel.ForEach(currentPass, parOptions, path =>
            {
                int current = Interlocked.Increment(ref processedCounter);
                if ((current & 255) == 0)
                {
                    lock (feedback)
                    {
                        feedback.Item = current;
                        feedback.Count = _queued.Count;
                        feedback.Message = Path.GetFileName(path);
                    }
                }
                ScanFile(path);
            });
            currentPass = _nextPass.Keys.ToArray();
            _nextPass = [];
        }

        _queued.Clear();
        _system_includes.Clear();

        _project.ScanTime = sw.Elapsed;
    }

    void ScanDirForSourceFilesRecurse(DirectoryInfo di)
    {
        FileInfo[] files;
        DirectoryInfo[] subdirs;

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
        {
            if (!subdir.Name.StartsWith('.'))
                ScanDirForSourceFilesRecurse(subdir);
        }
    }

    void ScanDirForHeaders(DirectoryInfo di)
    {
        FileInfo[] files;
        try
        {
            files = di.GetFiles();
        }
        catch
        {
            // ignore exceptions
            return;
        }
        foreach (FileInfo file in files)
        {
            if (SourceFile.IsHeaderExtension(file.Extension))
            {
                _system_includes.TryAdd(file.Name, CanonicalPath(file.FullName));
            }
        }
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
            _nextPass.TryAdd(fullPath, 0);
        }
    }

    bool ContainsPrecompiledPath(string abs)
    {
        return _project.Files.TryGetValue(abs, out var f) && f.Precompiled;
    }

    void ScanFile(string path)
    {
        path = CanonicalPath(path);
        if (_project.Files.ContainsKey(path) && !_scanning_pch)
            return;
        Parser.Result res = Parser.ParseFile(path, Errors);
        var sf = new SourceFile
        {
            Lines = res.Lines,
            LocalIncludes = res.LocalIncludes,
            SystemIncludes = res.SystemIncludes,
            Precompiled = _scanning_pch
        };
        _project.Files.AddOrUpdate(path, sf, (_, __) => sf);

        sf.AbsoluteIncludes.Clear();

        string local_dir = Path.GetDirectoryName(path) ?? string.Empty;
        foreach (string s in sf.LocalIncludes) {
            try
            {
                string inc = Path.GetFullPath(Path.Combine(local_dir, s));
                string abs = CanonicalPath(inc);
                // found a header that's part of PCH during regular scan: ignore it
                if (!_scanning_pch && ContainsPrecompiledPath(abs))
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
                if (_system_includes.TryGetValue(s, out var sysIncPath))
                {
                    // An entry in _system_includes might have been found during the include folders
                    // scan; does not mean that all files under it are actually included into the project yet.
                    // Make sure it is scanned (if already is, this will early out).
                    Enqueue(sysIncPath, sysIncPath);

                    // found a header that's part of PCH during regular scan: ignore it
                    if (!_scanning_pch && ContainsPrecompiledPath(sysIncPath))
                    {
                        continue;
                    }
                    sf.AbsoluteIncludes.Add(sysIncPath);
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
                        if (!_scanning_pch && ContainsPrecompiledPath(abs))
                        {
                            continue;
                        }

                        sf.AbsoluteIncludes.Add(abs);
                        _system_includes.TryAdd(s, abs);
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