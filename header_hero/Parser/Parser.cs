using System;
using System.Collections.Generic;
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
    }

    static bool WordAt(string s, int index, string word)
    {
        if (index < 0 || index + word.Length > s.Length)
            return false;

        return s.AsSpan(index, word.Length).SequenceEqual(word);
    }

    static bool SkipSpace(string s, ref int i)
    {
        while (i < s.Length && char.IsWhiteSpace(s[i]))
            ++i;
        if (i >= s.Length)
            return false;
        return true;
    }

    static bool ParseLine(string line, Result result)
    {
        int i = 0;
        if (!SkipSpace(line, ref i))
            return true;

        // is a preprocessor macro?
        if (line[i] != '#')
            return true;
        ++i;

        if (!SkipSpace(line, ref i))
            return true;

        // has "include"?
        string keyword = "include";
        if (!WordAt(line, i, keyword))
            return true;
        i += keyword.Length;
        if (i >= line.Length || (!char.IsWhiteSpace(line[i]) && line[i] != '<' && line[i] != '"'))
            return false; // malformed include
        if (!SkipSpace(line, ref i))
            return false; // malformed include

        // angled or quoted?
        bool angled;
        if (line[i] == '<')
            angled = true;
        else if (line[i] == '"')
            angled = false;
        else
            return false; // malformed include or include w/ a define
        ++i;

        // scan the name
        int nameStart = i;
        while (i < line.Length)
        {
            if (angled && line[i] == '>')
            {
                int nameEnd = i - 1;
                if (nameEnd == nameStart)
                    return false; // empty include
                result.SystemIncludes.Add(line.Substring(nameStart, nameEnd-nameStart+1));
                return true;
            }
            if (!angled && line[i] == '"')
            {
                int nameEnd = i - 1;
                if (nameEnd == nameStart)
                    return false; // empty include
                result.LocalIncludes.Add(line.Substring(nameStart, nameEnd-nameStart+1));
                return true;
            }
            ++i;
        }

        return false; // non-terminated include name
    }

    public static Result ParseFile(string fullPath, List<ScanError> errors)
    {
        Result res = new Result();
        string[] lines = File.ReadAllLines(fullPath, Encoding.UTF8);
        res.Lines = lines.Length;
        foreach (string line in lines)
        {
            if (!ParseLine(line, res))
                errors.Add(new ScanError($"Could not parse: {line}", fullPath));
        }
        return res;
    }
}