using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HeaderHero.Parser
{
    public class Scanner
    {
        Data.Project _project;
        HashSet<string> _queued;
        List<FileInfo> _scan_queue;
        Dictionary<string, string> _system_includes;
        bool _scanning_pch = false;
        public bool CaseSensitive { get; set; }

        public List<string> Errors;
        public HashSet<string> NotFound;
        public Dictionary<string, string> NotFoundOrigins;

        public Scanner(Data.Project p)
        {
            _project = p;
            _queued = new HashSet<string>();
            _scan_queue = new List<FileInfo>();
            _system_includes = new Dictionary<string, string>();

            Errors = new List<string>();
            NotFound = new HashSet<string>();
            NotFoundOrigins = new Dictionary<string, string>();

            CaseSensitive = IsCaseSensitive();
        }

        private bool IsCaseSensitive()
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
            feedback.Title = "Scanning precompiled header...";
            foreach (Data.SourceFile sf in _project.Files.Values)
            {
                sf.Touched = false;
                sf.Precompiled = false;
            }

            // scan everything that goes into precompiled header
            _scanning_pch = true;
            if (!string.IsNullOrEmpty(_project.PrecompiledHeader) && File.Exists(_project.PrecompiledHeader))
            {
                var inc = new FileInfo(_project.PrecompiledHeader);
                ScanFile(inc);
                while (_scan_queue.Count > 0)
                {
                    FileInfo[] to_scan = _scan_queue.ToArray();
                    _scan_queue.Clear();
                    foreach (FileInfo fi in to_scan)
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
                FileInfo[] to_scan = _scan_queue.ToArray();
                _scan_queue.Clear();
                foreach (FileInfo fi in to_scan)
                {
                    feedback.Count = dequeued + _scan_queue.Count;
                    feedback.Item++;
                    feedback.Message = fi.Name;
                    ScanFile(fi);
                }
            }
            _queued.Clear();
            _system_includes.Clear();

            foreach (var it in _project.Files.Where(kvp => !kvp.Value.Touched).ToList())
                _project.Files.Remove(it.Key);
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
                Errors.Add(string.Format("Cannot descend into {0}: {1}", di.FullName, e.Message));
                return;
            }

            foreach (FileInfo file in files)
            {
                if (file.Extension == @".cpp" || file.Extension == @".c" || file.Extension == @".cc" || file.Extension == @".cxx")
                    Enqueue(file, CanonicalPath(file));
            }
            foreach (DirectoryInfo subdir in subdirs)
                if (!subdir.Name.StartsWith("."))
                    ScanDirectory(subdir, feedback);
        }

        string CanonicalPath(FileInfo fi)
        {
            if (CaseSensitive)
                return fi.FullName;
            else
                return fi.FullName.ToLowerInvariant();
        }

        void Enqueue(FileInfo inc, string abs)
        {
            if (!_queued.Contains(abs))
            {
                _queued.Add(abs);
                _scan_queue.Add(inc);
            }
        }

        void ScanFile(FileInfo fi)
        {
            string path = CanonicalPath(fi);
            Data.SourceFile sf = null;
            if (_project.Files.ContainsKey(path) && _project.LastScan > fi.LastWriteTime && !_scanning_pch)
            {
                sf = _project.Files[path];
            }
            else
            {
                Parser.Result res = Parser.ParseFile(fi, Errors);
                sf = new Data.SourceFile();
                sf.Lines = res.Lines;
                sf.LocalIncludes = res.LocalIncludes;
                sf.SystemIncludes = res.SystemIncludes;
                sf.Precompiled = _scanning_pch;
                _project.Files[path] = sf;
            }

            sf.Touched = true;
            sf.AbsoluteIncludes.Clear();

            string local_dir = Path.GetDirectoryName(path);
            foreach (string s in sf.LocalIncludes) {
                try
                {
                    FileInfo inc = new FileInfo(Path.Combine(local_dir, s));
                    string abs = CanonicalPath(inc);
                    // found a header that's part of PCH during regular scan: ignore it
                    if (!_scanning_pch && _project.Files.ContainsKey(abs) && _project.Files[abs].Precompiled)
                    {
                        _project.Files[abs].Touched = true;
                        continue;
                    }
                    if (!inc.Exists)
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
                    Errors.Add(String.Format("Exception: \"{0}\" for #include \"{1}\"", e.Message, s));
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
                            _project.Files[abs].Touched = true;
                            continue;
                        }
                        sf.AbsoluteIncludes.Add(abs);
                    }
                    else
                    {
                        FileInfo found = null;

                        foreach (string dir in _project.IncludeDirectories)
                        {
                            found = new FileInfo(Path.Combine(dir, s));
                            if (found.Exists)
                                break;
                            found = null;
                        }

                        if (found != null)
                        {
                            string abs = CanonicalPath(found);
                            // found a header that's part of PCH during regular scan: ignore it
                            if (!_scanning_pch && _project.Files.ContainsKey(abs) && _project.Files[abs].Precompiled)
                            {
                                _project.Files[abs].Touched = true;
                                continue;
                            }

                            sf.AbsoluteIncludes.Add(abs);
                            _system_includes[s] = abs;
                            Enqueue(found, abs);
                        }
                        else
                        {
                            if (NotFound.Add(s))
                            {
                                NotFoundOrigins.Add(s, path);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                     Errors.Add(String.Format("Exception: \"{0}\" for #include <{1}>", e.Message, s));
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
}
