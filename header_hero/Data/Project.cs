using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace HeaderHero.Data;

public class Project
{
    public List<string> ScanDirectories { get; set; } = [];
    public List<string> IncludeDirectories { get; set; } = [];
    public string PrecompiledHeader { get; set; } = string.Empty;
    public Dictionary<string, SourceFile> Files { get; } = new();
    public TimeSpan ScanTime { get; set; }

    public string ToJson()
    {
        using var stream = new MemoryStream();
        using var writer = new Utf8JsonWriter(stream, new JsonWriterOptions {Indented = true} );
        writer.WriteStartObject();
        writer.WriteStartArray("ScanDirectories");
        foreach (var s in ScanDirectories)
            writer.WriteStringValue(s);
        writer.WriteEndArray();
        writer.WriteStartArray("IncludeDirectories");
        foreach (var s in IncludeDirectories)
            writer.WriteStringValue(s);
        writer.WriteEndArray();
        writer.WriteString("PrecompiledHeader", PrecompiledHeader);
        writer.WriteEndObject();
        writer.Flush();
        return Encoding.UTF8.GetString(stream.ToArray());
    }

    public void FromJsonFile(string filePath)
    {
        ScanDirectories = [];
        IncludeDirectories = [];
        PrecompiledHeader = string.Empty;
        Files.Clear();

        try
        {
            var json = File.ReadAllText(filePath);
            using var doc = JsonDocument.Parse(json);
            ScanDirectories = GetStringList("ScanDirectories", doc.RootElement);
            IncludeDirectories = GetStringList("IncludeDirectories", doc.RootElement);
            PrecompiledHeader = GetString("PrecompiledHeader", doc.RootElement) ?? string.Empty;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load JSON from {filePath}: {ex.Message}");
        }
    }

    static string GetString(string key, JsonElement element)
    {
        if (element.TryGetProperty(key, out var prop) && prop.ValueKind == JsonValueKind.String)
        {
            return prop.GetString();
        }
        return null;
    }

    static List<string> GetStringList(string key, JsonElement element)
    {
        List<string> res = [];
        if (element.TryGetProperty(key, out var prop) && prop.ValueKind == JsonValueKind.Array)
        {
            foreach (var o in prop.EnumerateArray())
            {
                if (o.ValueKind == JsonValueKind.String)
                    res.Add(o.GetString());
            }
        }
        return res;
    }
}