using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeaderHero.Data
{
    public class SourceFile
    {
        public List<string> LocalIncludes { get; set; }
        public List<string> SystemIncludes { get; set; }
        public List<string> AbsoluteIncludes { get; set; }
        public int Lines { get; set; }
       
        public SourceFile()
        {
            LocalIncludes = new List<string>();
            SystemIncludes = new List<string>();
            AbsoluteIncludes = new List<string>();
            Lines = 0;
        }
    }
}