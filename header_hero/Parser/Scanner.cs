﻿using System;
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

        public List<string> Errors;
        public HashSet<string> NotFound;

        public Scanner(Data.Project p)
        {
            _project = p;
            _queued = new HashSet<string>();
            _scan_queue = new List<FileInfo>();
            _system_includes = new Dictionary<string, string>();

            Errors = new List<string>();
            NotFound = new HashSet<string>();
        }

        public void Rescan(ProgressFeedback feedback)
        {
            foreach (string dir in _project.ScanDirectories)
            {
                feedback.Message = dir;
                ScanDirectory(new DirectoryInfo(dir));
            }

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
                    feedback.Title = fi.Name;
                    ScanFile(fi);
                }
            }
        }
        
        void ScanDirectory(DirectoryInfo di)
        {
            foreach (FileInfo file in di.EnumerateFiles())
            {
                if (file.Extension == @".cpp" || file.Extension == @".c" || file.Extension == @".cc")
                    ScanFile(file);
            }
            foreach (DirectoryInfo subdir in di.EnumerateDirectories())
                if (!subdir.Name.StartsWith("."))
                    ScanDirectory(subdir);
        }

        string CanonicalPath(FileInfo fi)
        {
            return fi.FullName;
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
            if (_project.Files.ContainsKey(path) && _project.LastScan > fi.LastWriteTime)
                sf = _project.Files[path];
            else
            {
                Parser.Result res = Parser.ParseFile(fi, Errors);
                sf = new Data.SourceFile();
                sf.Lines = res.Lines;
                sf.LocalIncludes = res.LocalIncludes;
                sf.SystemIncludes = res.SystemIncludes;
                _project.Files[path] = sf;
            }

            sf.AbsoluteIncludes.Clear();

            string local_dir = Path.GetDirectoryName(path);
            foreach (string s in sf.LocalIncludes) {
                FileInfo inc = new FileInfo(Path.Combine(local_dir, s));
                string abs = CanonicalPath(inc);
                if (!inc.Exists)
                {
                    if (!sf.SystemIncludes.Contains(s))
                        sf.SystemIncludes.Add(s);
                    continue;
                }
                sf.AbsoluteIncludes.Add(abs);
                Enqueue(inc, abs);
            }

            foreach (string s in sf.SystemIncludes)
            {
                if (_system_includes.ContainsKey(s))
                {
                    string abs = _system_includes[s];
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
                        sf.AbsoluteIncludes.Add(abs);
                        _system_includes[s] = abs;
                        Enqueue(found, abs);
                    }
                    else
                        NotFound.Add(s);
                }
                
            }
        }
    }
}
