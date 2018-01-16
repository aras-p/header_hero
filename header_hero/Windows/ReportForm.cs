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

            Cursor.Current = Cursors.WaitCursor;
            Setup(project, scanner);
            Cursor.Current = Cursors.Default;
        }

        private void Setup(Data.Project project, Parser.Scanner scanner)
        {
            _project = project;
            _scanner = scanner;
            _analytics = Parser.Analytics.Analyze(_project);
            
            errorsListView.Items.Clear();
            foreach (string s in scanner.Errors)
                errorsListView.Items.Add(s);
            missingFilesListView.Items.Clear();
            foreach (var s in scanner.NotFound.OrderBy(s => s))
            {
                var li = new ListViewItem(s);
                li.ToolTipText = scanner.NotFoundOrigins[s];
                missingFilesListView.Items.Add(li);
            }

            string file = Parser.Report.Generate(_project, _analytics);
            if (reportBrowser.Url != null)
                reportBrowser.Refresh();
            else
                reportBrowser.Navigate(file);
            reportBrowser.Navigating += reportBrowser_Navigating;
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

        private string _inspecting;

        private void Inspect(string file)
        {
            _inspecting = file;

            {
                fileListView.Items.Clear();
                string text = string.Format("{0} ({1})", Path.GetFileName(file), _analytics.Items[file].TotalIncludeLines);
                ListViewItem item = new ListViewItem(text);
                item.Tag = file;
                fileListView.Items.Add(item);
            }

            {
                includesListView.Items.Clear();
                foreach (string s in _project.Files[file].AbsoluteIncludes.OrderByDescending(f => _analytics.Items[f].AllIncludes.Count))
                {
                    ListViewItem item = new ListViewItem(new[] { Path.GetFileName(s), _analytics.Items[s].AllIncludes.Count.ToString(), _analytics.Items[s].TotalIncludeLines.ToString()});
                    item.Tag = s;
                    includesListView.Items.Add(item);
                }
            }

            {
                includedByListView.Items.Clear();
                IEnumerable<string> included = _project.Files.Where(kvp => kvp.Value.AbsoluteIncludes.Contains(file)).Select(kvp => kvp.Key);
                foreach (string s in included.OrderByDescending(s => _analytics.Items[s].AllIncludedBy.Count))
                {
                    ListViewItem item = new ListViewItem(new[] { Path.GetFileName(s), _analytics.Items[s].AllIncludedBy.Count.ToString(), _analytics.Items[s].TotalIncludeLines.ToString() });
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

            Cursor.Current = Cursors.WaitCursor;
            Setup(_project, _scanner);
            Cursor.Current = Cursors.Default;
            if (_inspecting != null)
                Inspect(_inspecting);
        }
    }
}
