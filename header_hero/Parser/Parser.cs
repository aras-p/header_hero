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
                    int qt = line.IndexOf('"', i);

                    if (lt < 0 && qt < 0 || lt > 0 && qt > 0)
                    {
                        errors.Add("Could not parse line: " + line + " in file: " + fi.FullName);
                        continue;
                    }

                    if (qt<0) {
                        int gt = line.IndexOf('>', lt+1);
                        res.SystemIncludes.Add(line.Substring(lt+1,gt-lt-1));
                    } else {
                        int qt2 = line.IndexOf('"', qt+1);
                        res.LocalIncludes.Add(line.Substring(qt+1,qt2-qt-1));
                    }
                }
            }
            return res;
        }
    }
}
