using System;
using System.IO;
using System.Text.Json;

namespace HeaderHero;

public sealed class AppSettings
{
    public string LastProject { get; set; }

    static readonly Lazy<AppSettings> _instance = new(Load);
    public static AppSettings Instance => _instance.Value;

    static string SettingsPath =>
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "HeaderHero",
            "settings.json"
        );

    static AppSettings Load()
    {
        try
        {
            if (File.Exists(SettingsPath))
            {
                var json = File.ReadAllText(SettingsPath);
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("LastProject", out var prop))
                {
                    return new AppSettings {LastProject = prop.GetString() ?? ""};
                }
            }
        }
        catch
        {
            // ignore errors, return default
        }
        return new AppSettings();
    }

    public void Save()
    {
        try
        {
            var dir = Path.GetDirectoryName(SettingsPath)!;
            if (dir != null && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            using var stream = File.Create(SettingsPath);
            using var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true });
            writer.WriteStartObject();
            writer.WriteString("LastProject", LastProject);
            writer.WriteEndObject();
        }
        catch
        {
            // ignore errors
        }
    }
}
