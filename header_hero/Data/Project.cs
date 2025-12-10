using System;
using System.Collections.Generic;

namespace HeaderHero.Data;

public class Project
{
    public List<string> ScanDirectories { get; set; } = [];
    public List<string> IncludeDirectories { get; set; } = [];
    public string PrecompiledHeader { get; set; } = string.Empty;
    public Dictionary<string, SourceFile> Files { get; } = new();
    public DateTime LastScan { get; set; } = DateTime.Now;

    public void Clean()
    {
        Files.Clear();
    }
}