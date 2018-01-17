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
            File.WriteAllBytes(CssFile, Encoding.UTF8.GetBytes(_css));
        }

        void AppendSummary(StringBuilder sb, IDictionary<string, string> count)
        {
            sb.AppendFormat("<table class=\"summary\">\n");
            foreach (var kvp in count)
                sb.AppendFormat("  <tr><th>{0}:</th> <td>{1}</td></tr>\n", kvp.Key, kvp.Value);
            sb.AppendFormat("</table>\n");
        }

        void AppendFileList(StringBuilder sb, string id, string header, IEnumerable<KeyValuePair<string, int>> count)
        {
            sb.AppendFormat("<a name=\"{0}\" />", id);
            sb.AppendFormat("<h2>{0}</h2>\n\n", header);

            sb.AppendFormat("<table class=\"list\">\n");
            foreach (var kvp in count)
                sb.AppendFormat("  <tr><th>{1:### ### ###}</th> <td><a href=\"http://inspect?{0}\">{2}</a></td></tr>\n", kvp.Key, kvp.Value, Path.GetFileName(kvp.Key));
            sb.AppendFormat("</table>\n");
        }

        void GenerateHtml()
        {
            string html = _html;

            StringBuilder sb = new StringBuilder();

            // Summary
            {
                int pch_lines = _project.Files.Where(kvp => kvp.Value.Precompiled).Sum(kvp => kvp.Value.Lines);
                int total_lines = _project.Files.Sum(kvp => kvp.Value.Lines) - pch_lines;
                int total_parsed = _analytics.Items
					.Where (kvp => Data.SourceFile.IsTranslationUnitPath(kvp.Key) && !_project.Files[kvp.Key].Precompiled)
					.Sum(kvp => kvp.Value.TotalIncludeLines + _project.Files[kvp.Key].Lines);
                float factor = (float)total_parsed / (float)total_lines;
                Dictionary<string, string> table = new Dictionary<string, string> {
                    {"Files", string.Format("{0:### ### ###}", _project.Files.Count)},
                    {"Total Lines", string.Format("{0:### ### ###}", total_lines)},
                    {"Total Precompiled", string.Format("{0:### ### ###} (<a href=\"#pch\">list</a>)", pch_lines)},
                    {"Total Parsed", string.Format("{0:### ### ###}", total_parsed)},
                    {"Blowup Factor", string.Format("{0:0.00} (<a href=\"#largest\">largest</a>, <a href=\"#hubs\">hubs</a>)", factor) }
                };
                AppendSummary(sb, table);
            }

            {
                var most = _analytics.Items
                    .ToDictionary(kvp => kvp.Key, kvp => _project.Files[kvp.Key].Lines *
                        kvp.Value.TranslationUnitsIncludedBy.Count)
                    .Where(kvp => !_project.Files[kvp.Key].Precompiled)
                    .Where(kvp => kvp.Value > 0)
                    .OrderByDescending(kvp => kvp.Value)
                    .Take(50);
                AppendFileList(sb, "largest", "Biggest Contributors", most);
            }

            {
                var hubs = _analytics.Items
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.AllIncludes.Count * kvp.Value.TranslationUnitsIncludedBy.Count)
                    .Where(kvp => kvp.Value > 0)
                    .OrderByDescending(kvp => kvp.Value)
                    .Take(50);
                AppendFileList(sb, "hubs", "Header Hubs", hubs);
            }

            {
                var pch = _project.Files
                    .Where(kvp => kvp.Value.Precompiled)
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Lines)
                    .OrderByDescending(kvp => kvp.Value)
                    .Take(50);
                AppendFileList(sb, "pch", "Precompiled Headers", pch);
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
		
		static private string _css = @"
/* Reset */

* {margin:0;padding:0;border:0;outline:0;font-weight:inherit;font-style:inherit;font-size:100%;font-family:inherit;vertical-align:baseline}
body {line-height:1;color:black;background:white}
ol,ul {list-style:none}
table {border-collapse:separate;border-spacing:0}
caption,th,td {text-align:left;font-weight:normal}
a {text-decoration:none;}

body {
  background: #fff; 
  font: 12px/16px 'Segoe UI', 'Lucida Grande', 'Lucida Sans Unicode', Helvetica, Arial, Verdana, sans-serif;
  font-weight: normal;
  overflow-y: scroll;
  margin: 10px;
}

h1 {
  font: 16px;
  margin: 10px 0px 10px 0px;
}

h2 {
  font: 16px;
  margin: 10px 0px 10px 0px;
}

.summary {
  margin-left: 10px;
}

.summary th {
  font-weight: bold;
  padding-right: 10px;
}

.list {
  margin-left: 20px;
}

.list th {
  text-align: right;
  padding-right: 10px;
}";
		static private string _html = @"
<html>
<head>
    <link rel='stylesheet' type='text/css' media='screen' href='header_hero_report.css'/>
</head>
<body>

<h1>Report</h1>

%CONTENT%

</body>
</html>".Replace ("'", "\"");
    }
}
