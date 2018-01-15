using System;
using System.Collections.Generic;

namespace HeaderHero.Data
{
    public class Project
    {
        public List<string> ScanDirectories { get; set; }
        public List<string> IncludeDirectories { get; set; }
        public string PrecompiledHeader { get; set; }
        public Dictionary<string, SourceFile> Files { get; set; }
        public DateTime LastScan { get; set; }

        public Project()
        {
            ScanDirectories = new List<string>();
            IncludeDirectories = new List<string>();
            PrecompiledHeader = string.Empty;
            Files = new Dictionary<string, SourceFile>();
            LastScan = DateTime.Now;
        }

        public void Clean()
        {
            Files.Clear();
        }
    }
}
