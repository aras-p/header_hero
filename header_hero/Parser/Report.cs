using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HeaderHero.Parser
{
    class Report
    {
        Data.Project _project;
        Analytics _analytics;

        public string HtmlFile { get { return Path.Combine(Path.GetTempPath(), "header_hero_report.html"); } }
        public string CssFile { get { return Path.Combine(Path.GetTempPath(), "header_hero_report.css"); } }

        public Report(Data.Project project, Analytics analytics)
        {
            _project = project;
            _analytics = analytics;
        }

        public void Generate()
        {
            GenerateCss();
            GenerateHtml();
        }

        void GenerateCss()
        {
            File.WriteAllBytes(CssFile, Encoding.UTF8.GetBytes(Properties.Resources.Css));
        }

        void AppendSummary(StringBuilder sb, IDictionary<string, string> count)
        {
            sb.AppendFormat("<table class=\"summary\">\n");
            foreach (var kvp in count)
                sb.AppendFormat("  <tr><th>{0}:</th> <td>{1}</td></tr>\n", kvp.Key, kvp.Value);
            sb.AppendFormat("</table>\n");
        }

        void AppendFileList(StringBuilder sb, string header, IEnumerable<KeyValuePair<string, int>> count)
        {
            sb.AppendFormat("<h2>{0}</h2>\n\n", header);

            sb.AppendFormat("<table class=\"list\">\n");
            foreach (var kvp in count)
                sb.AppendFormat("  <tr><th>{1:### ### ###}</th> <td><a href=\"http://inspect?{0}\">{2}</a></td></tr>\n", kvp.Key, kvp.Value, Path.GetFileName(kvp.Key));
            sb.AppendFormat("</table>\n");
        }

        void GenerateHtml()
        {
            string html = Properties.Resources.Html;

            StringBuilder sb = new StringBuilder();

            // Summary
            {
                int total_lines = _project.Files.Sum(kvp => kvp.Value.Lines);
                int total_parsed = _analytics.Items.Sum(kvp => kvp.Value.TotalIncludeLines + _project.Files[kvp.Key].Lines);
                float factor = (float)total_parsed / (float)total_lines;
                Dictionary<string, string> table = new Dictionary<string, string> {
                    {"Files", string.Format("{0:### ### ###}", _project.Files.Count)},
                    {"Total Lines", string.Format("{0:### ### ###}", total_lines)},
                    {"Total Parsed", string.Format("{0:### ### ###}", total_parsed)},
                    {"Blowup Factor", string.Format("{0:0.00}", factor) }
                };
                AppendSummary(sb, table);
            }

            {
                var most = _analytics.Items
                    .ToDictionary(kvp => kvp.Key, kvp => _project.Files[kvp.Key].Lines * kvp.Value.AllIncludedBy.Count)
                    .Where(kvp => kvp.Value > 0)
                    .OrderByDescending(kvp => kvp.Value);
                AppendFileList(sb, "Biggest Contributors", most);
            }

            {
                var hubs = _analytics.Items
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.AllIncludes.Count * kvp.Value.AllIncludedBy.Count)
                    .Where(kvp => kvp.Value > 0)
                    .OrderByDescending(kvp => kvp.Value);
                AppendFileList(sb, "Header Hubs", hubs);
            }

            html = html.Replace("%CONTENT%", sb.ToString());

            File.WriteAllBytes(HtmlFile, Encoding.UTF8.GetBytes(html));
        }

        public static string Generate(Data.Project p, Analytics a)
        {
            Report r = new Report(p, a);
            r.Generate();
            return r.HtmlFile;
        }
    }
}
