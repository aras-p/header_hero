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
                try
                {
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
                catch (Exception e)
                {
                     Errors.Add(String.Format("Exception: \"{0}\" for #include <{1}>", e.Message, s));
                }
                
            }
        }
    }
}
