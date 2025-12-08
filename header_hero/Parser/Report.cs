using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HeaderHero.Parser
{
    public class ReportFile
    {
        public int count { get; set; }
        public string path { get; set; }
        public string name { get; set; }

        public ReportFile(int count, string path)
        {
            this.count = count;
            this.path = path;
            name = Path.GetFileName(path);
        }
    }

    class Report
    {
        Data.Project _project;
        Analytics _analytics;

        public string summary;
        public List<ReportFile> largest;
        public List<ReportFile> hubs;
        public List<ReportFile> precompiled;

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
            float factor = (float)total_parsed / (float)total_lines;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Files             {_project.Files.Count:### ### ###}");
            sb.AppendLine($"Total Lines       {total_lines:### ### ###}");
            sb.AppendLine($"Total Precompiled {pch_lines:### ### ###}"); //@TODO link (list)
            sb.AppendLine($"Total Parsed      {total_parsed:### ### ###}");
            sb.AppendLine($"Blowup Factor     {factor:0.00}"); //@TODO: links (largest, hubs)
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
}
