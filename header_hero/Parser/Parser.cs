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

        enum States { Start, Hash, Include, AngleBracket, Quote }
        enum ParseResult { Ok, Error }

        static ParseResult ParseLine(string line, Result result)
        {
            int i = 0;
            int path_start = 0;
            States state = States.Start;

            while (true) {
                if (i >= line.Length)
                    return ParseResult.Error;

                char c = line[i];
                ++i;
               
                if (c == ' ' || c == '\t') {
                
                } else if (state == States.Start) {
                    if (c == '#')
                        state = States.Hash;
                    else if (c == '/') {
                        if (i >= line.Length)
                            return ParseResult.Error;
                        if (line[i] == '/')
                            return ParseResult.Ok; // Matched C++ style comment
                    } else
                        return ParseResult.Error;
                } else if (state == States.Hash) {
                    --i;
                    if (line.IndexOf("include", i) == i) {
                        i += 7;
                        state = States.Include;
                    } else
                        return ParseResult.Ok; // Matched preprocessor other than #include
                } else if (state == States.Include) {
                    if (c == '<') {
                        path_start = i;
                        state = States.AngleBracket;
                    } else if (c == '"') {
                        path_start = i;
                         state = States.Quote;
                    } else
                        return ParseResult.Error;
                } else if (state == States.AngleBracket) {
                    if (c == '>') {
                        result.SystemIncludes.Add(line.Substring(path_start, i-path_start-1));
                        return ParseResult.Ok;
                    }
                } else if (state == States.Quote) {
                    if (c == '"') {
                        result.LocalIncludes.Add(line.Substring(path_start, i-path_start-1));
                        return ParseResult.Ok;
                    }
                }
            }
        }

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
                if (line.Contains('#') && line.Contains("include"))
                {
                    ParseResult r = ParseLine(line, res);
                    if (r == ParseResult.Error)
                        errors.Add("Could not parse line: " + line + " in file: " + fi.FullName);
                }
            }
            return res;
        }
    }
}
