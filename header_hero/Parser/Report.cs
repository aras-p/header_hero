using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.IO;

namespace HeaderHero.Parser;

public class ReportFile(int count, string path)
{
    public int count { get; set; } = count;
    public string path { get; } = path;
    public string name { get; set; } = Path.GetFileName(path);
}

class Report
{
    readonly Data.Project _project;
    readonly Analytics _analytics;

    public readonly string summary;
    public readonly List<ReportFile> largest;
    public readonly List<ReportFile> hubs;
    public readonly List<ReportFile> precompiled;

    public Report(Data.Project project, Analytics analytics)
    {
        _project = project;
        _analytics = analytics;
        summary = GenerateSummary();
        largest = GenerateLargestContributors();
        hubs = GenerateHeaderHubs();
        precompiled = GeneratePrecompiled();
    }

    List<ReportFile> AppendFileList(IEnumerable<KeyValuePair<string, int>> count)
    {
        return count.Select(kvp => new ReportFile(kvp.Value, kvp.Key)).ToList();
    }

    string GenerateSummary()
    {
        int pch_lines = _project.Files.Where(kvp => kvp.Value.Precompiled).Sum(kvp => kvp.Value.Lines);
        int total_lines = _project.Files.Sum(kvp => kvp.Value.Lines) - pch_lines;
        int total_parsed = _analytics.Items
            .Where (kvp => Data.SourceFile.IsTranslationUnitPath(kvp.Key) && !_project.Files[kvp.Key].Precompiled)
            .Sum(kvp => kvp.Value.TotalIncludeLines + _project.Files[kvp.Key].Lines);
        float factor = total_lines == 0 ? 0.0f : total_parsed / (float)total_lines;

        const int valueWidth = 13;
        var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
        nfi.NumberGroupSeparator = " ";

        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"Files             {_project.Files.Count.ToString("N0",nfi),valueWidth} (scanned in {_project.ScanTime.TotalSeconds:0.0} sec)");
        sb.AppendLine($"Total Lines       {total_lines.ToString("N0",nfi),valueWidth}");
        sb.AppendLine($"Total Precompiled {pch_lines.ToString("N0",nfi),valueWidth}");
        sb.AppendLine($"Total Parsed      {total_parsed.ToString("N0",nfi),valueWidth}");
        sb.AppendLine($"Blowup Factor     {factor.ToString("0.00",nfi),valueWidth}");
        return sb.ToString();
    }

    List<ReportFile> GenerateLargestContributors()
    {
        var most = _analytics.Items
            .ToDictionary(kvp => kvp.Key, kvp => _project.Files[kvp.Key].Lines *
                                                 kvp.Value.TranslationUnitsIncludedBy.Count)
            .Where(kvp => !_project.Files[kvp.Key].Precompiled)
            .Where(kvp => kvp.Value > 0)
            .OrderByDescending(kvp => kvp.Value);
        return AppendFileList(most);
    }

    List<ReportFile> GenerateHeaderHubs()
    {
        var hhubs = _analytics.Items
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.AllIncludes.Count * kvp.Value.TranslationUnitsIncludedBy.Count)
            .Where(kvp => kvp.Value > 0)
            .OrderByDescending(kvp => kvp.Value);
        return AppendFileList(hhubs);
    }

    List<ReportFile> GeneratePrecompiled()
    {
        var pch = _project.Files
            .Where(kvp => kvp.Value.Precompiled)
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Lines)
            .OrderByDescending(kvp => kvp.Value);
        return AppendFileList(pch);
    }
}