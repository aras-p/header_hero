using System;
using System.Collections.Generic;

namespace HeaderHero.Data;

public class Project
{
    public List<string> ScanDirectories { get; set; } = [];
    public List<string> IncludeDirectories { get; set; } = [];
    public string PrecompiledHeader { get; set; } = string.Empty;
    public Dictionary<string, SourceFile> Files { get; } = new();
    public TimeSpan ScanTime { get; set; }

    public Dictionary<string, object> ToDict()
    {
        Dictionary<string, object> dict = new()
        {
            ["ScanDirectories"] = ScanDirectories,
            ["IncludeDirectories"] = IncludeDirectories,
            ["PrecompiledHeader"] = PrecompiledHeader
        };
        return dict;
    }

    public void FromDict(Dictionary<string, object> dict)
    {
        ScanDirectories = [];
        IncludeDirectories = [];
        PrecompiledHeader = string.Empty;
        Files.Clear();

        ScanDirectories = GetStringList("ScanDirectories", dict);
        IncludeDirectories = GetStringList("IncludeDirectories", dict);

        if (dict.TryGetValue("PrecompiledHeader", out var val))
        {
            if (val is string str)
            {
                PrecompiledHeader = str;
            }
        }
    }

    static List<string> GetStringList(string key, Dictionary<string, object> dict)
    {
        List<string> res = [];
        if (!dict.TryGetValue(key, out var val)) return res;
        if (val is not List<object> list) return res;
        foreach (var o in list)
        {
            if (o is string s)
                res.Add(s);
        }
        return res;
    }
}