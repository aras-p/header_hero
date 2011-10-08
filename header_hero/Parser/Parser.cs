using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HeaderHero.Parser
{
    class Parser
    {
        public class Result
        {
            public List<string> SystemIncludes {get; set;}
            public List<string> LocalIncludes {get; set;}
            public int Lines;

            public Result()
            {
                SystemIncludes = new List<string>();
                LocalIncludes = new List<string>();
            }
        };

        /// <summary>
        /// Simple parser... only looks for #include lines. Does not take #defines or comments into account.
        /// </summary>
        static public Result ParseFile(FileInfo fi, List<string> errors)
        {
            Result res = new Result();
            string[] lines = File.ReadAllLines(fi.FullName, Encoding.UTF8);
            res.Lines = lines.Count();
            foreach (string line in lines)
            {
                if (line.Contains("#include"))
                {
                    int i = line.IndexOf('#');
                    int lt = line.IndexOf('<', i);
                    int gt = line.IndexOf('>', lt + 1);
                    int qt = line.IndexOf('"', i);
                    int qt2 = line.IndexOf('"', qt + 1);
                    
                    if (lt>=0 && gt>=0 && qt<0)
                        res.SystemIncludes.Add(line.Substring(lt+1,gt-lt-1));
                    else if (qt>=0 && qt2>=0 && lt<0)
                        res.LocalIncludes.Add(line.Substring(qt+1,qt2-qt-1));
                    else
                        errors.Add("Could not parse line: " + line + " in file: " + fi.FullName);
                }
            }
            return res;
        }
    }
}
