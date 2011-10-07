using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace HeaderHero
{
    public partial class ReportForm : Form
    {
        Data.Project _project;
        Parser.Scanner _scanner;
        Parser.Analytics _analytics;
        
        public ReportForm(Data.Project project, Parser.Scanner scanner)
        {
            InitializeComponent();

            includedByListView.MouseDoubleClick += new MouseEventHandler(includedByListView_MouseDoubleClick);
            includesListView.MouseDoubleClick += new MouseEventHandler(includedByListView_MouseDoubleClick);

            Setup(project, scanner);
        }

        private void Setup(Data.Project project, Parser.Scanner scanner)
        {
            _project = project;
            _scanner = scanner;

            errorsListView.Items.Clear();
            foreach (string s in scanner.Errors)
                errorsListView.Items.Add(s);
            missingFilesListView.Items.Clear();
            foreach (string s in scanner.NotFound.OrderBy(s => s))
                missingFilesListView.Items.Add(s);

            Analyze();
            GenerateCss();
            GenerateReport();
        }

        void includedByListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListView lv = sender as ListView;
            if (lv.SelectedItems.Count == 1)
            {
                string file = lv.SelectedItems[0].Tag as string;
                Inspect(file);
            }
        }

        private void Analyze()
        {
            _analytics = Parser.Analytics.Analyze(_project);
        }

        public void GenerateCss()
        {
            string file = Path.Combine(Path.GetTempPath(), "header_hero_report.css");
            File.WriteAllBytes(file, Encoding.UTF8.GetBytes(Properties.Resources.Css));
        }

        public void AppendSummary(StringBuilder sb, IDictionary<string, string> count)
        {
            sb.AppendFormat("<table class=\"summary\">\n");
            foreach (var kvp in count)
                sb.AppendFormat("  <tr><th>{0}:</th> <td>{1}</td></tr>\n", kvp.Key, kvp.Value);
            sb.AppendFormat("</table>\n");
        }

        public void AppendFileList(StringBuilder sb, string header, IEnumerable< KeyValuePair<string, int> > count)
        {
            sb.AppendFormat("<h2>{0}</h2>\n\n", header);

            sb.AppendFormat("<table class=\"list\">\n");
            foreach (var kvp in count)
                sb.AppendFormat("  <tr><th>{1:### ### ###}</th> <td><a href=\"http://inspect?{0}\">{2}</a></td></tr>\n", kvp.Key, kvp.Value, Path.GetFileName(kvp.Key));
            sb.AppendFormat("</table>\n");
        }

        public void GenerateReport()
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
            
            string file = Path.Combine(Path.GetTempPath(), "header_hero_report.html");
            File.WriteAllBytes(file, Encoding.UTF8.GetBytes(html));

            if (reportBrowser.Url != null)
                reportBrowser.Refresh();
            else
                reportBrowser.Navigate(file);
            reportBrowser.Navigating += reportBrowser_Navigating;
        }

        private string _inspecting;

        private void Inspect(string file)
        {
            _inspecting = file;

            {
                fileListView.Items.Clear();
                ListViewItem item = new ListViewItem(Path.GetFileName(file));
                item.Tag = file;
                fileListView.Items.Add(item);
            }

            {
                includesListView.Items.Clear();
                foreach (string s in _project.Files[file].AbsoluteIncludes.OrderByDescending(f => _analytics.Items[f].AllIncludes.Count))
                {
                    string text = string.Format("{0} ({1})", Path.GetFileName(s), _analytics.Items[s].AllIncludes.Count);
                    ListViewItem item = new ListViewItem(text);
                    item.Tag = s;
                    includesListView.Items.Add(item);
                }
            }

            {
                includedByListView.Items.Clear();
                IEnumerable<string> included = _project.Files.Where(kvp => kvp.Value.AbsoluteIncludes.Contains(file)).Select(kvp => kvp.Key);
                foreach (string s in included.OrderByDescending(s => _analytics.Items[s].AllIncludedBy.Count))
                {
                    string text = string.Format("{0} ({1})", Path.GetFileName(s), _analytics.Items[s].AllIncludedBy.Count);
                    ListViewItem item = new ListViewItem(text);
                    item.Tag = s;
                    includedByListView.Items.Add(item);
                }
            }
        }

        private void reportBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            if (e.Url.Host == "inspect")
            {
                string file = e.Url.Query.Substring(1);
                e.Cancel = true;
                Inspect(Uri.UnescapeDataString(file));
                tabPages.SelectedTab = includeTab;
            }
        }

        private void rescanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DateTime started = DateTime.Now;
            ProgressDialog d = new ProgressDialog();
            d.Text = "Scanning source files...";
            d.Work = (feedback) => _scanner.Rescan(feedback);
            d.Start();
            _project.LastScan = started;

            Setup(_project, _scanner);
            if (_inspecting != null)
                Inspect(_inspecting);
        }
    }
}
