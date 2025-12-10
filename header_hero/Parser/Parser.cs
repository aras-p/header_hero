using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HeaderHero.Parser;

static class Parser
{
    public class Result
    {
        public List<string> SystemIncludes {get;} = [];
        public List<string> LocalIncludes {get;} = [];
        public int Lines;
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
                if (line.IndexOf("include", i, StringComparison.Ordinal) == i) {
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

    public static Result ParseFile(string fullPath, ConcurrentQueue<string> errors)
    {
        Result res = new Result();
        string[] lines = File.ReadAllLines(fullPath, Encoding.UTF8);
        res.Lines = lines.Length;
        foreach (string line in lines)
        {
            if (line.Contains('#', StringComparison.Ordinal) && line.Contains("include", StringComparison.Ordinal))
            {
                ParseResult r = ParseLine(line, res);
                if (r == ParseResult.Error)
                    errors.Enqueue("Could not parse line: " + line + " in file: " + fullPath);
            }
        }
        return res;
    }
}