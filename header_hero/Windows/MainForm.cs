using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using HeaderHero.Serialization;

namespace HeaderHero
{
    public partial class MainForm : Form
    {
        string _file;
        string _last_save;
        Data.Project _project;

        public MainForm()
        {
            InitializeComponent();

            _project = new Data.Project();
            DisplayProject();
            MarkSave();
            
            projectDirsTextBox.MouseDoubleClick += (_1, _2) => scan_AddDirectory_Click(_1, null);
            includeDirsTextBox.MouseDoubleClick += (_1, _2) => include_AddDirectory_Click(_1, null);

            string last = Properties.Settings.Default.LastProject;
            if (last != null && last != "" && File.Exists(last))
            {
                Open(last);
                scanToolStripMenuItem1_Click(null, null);
            }
        }

        private void DisplayProject()
        {
            lastScanLabel.Text = _project.LastScan.ToString();
            projectDirsTextBox.Text = String.Join("\r\n", _project.ScanDirectories.ToArray());
            includeDirsTextBox.Text = String.Join("\r\n", _project.IncludeDirectories.ToArray());
        }

        private void ParseProject()
        {
            _project.ScanDirectories = projectDirsTextBox.Text.Trim().Split('\n', '\r').Where(s => s.Trim().Length > 0).ToList();
            _project.IncludeDirectories = includeDirsTextBox.Text.Trim().Split('\n', '\r').Where(s => s.Trim().Length > 0).ToList();
        }

        #region Project Dirs

        private void scan_AddDirectory_Click(object sender, EventArgs e)
        {
            ParseProject();
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK) {
                _project.ScanDirectories.Add( fbd.SelectedPath );
                DisplayProject();
            }
        }

        #endregion

        #region Include Dirs

        private void include_AddDirectory_Click(object sender, EventArgs e)
        {
            ParseProject();
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                _project.IncludeDirectories.Add(fbd.SelectedPath);
                DisplayProject();
            }
        }

        #endregion

        #region File Menu

        private string _filter = "Header Hero (*.header_hero)|*.header_hero";

        private void saveProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_file == null)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = _filter;
                if (sfd.ShowDialog() == DialogResult.OK)
                    _file = sfd.FileName;
            }

            if (_file == null)
                return;

            Properties.Settings.Default.LastProject = _file;
            ParseProject();
            Sjson.Save(JsonSerializer.Save(_project), _file);
            MarkSave();
        }

        private void MarkSave()
        {
            if (_file != null)
                Text = "Header Hero - " + _file;
            else
                Text = "Header Hero";
            _last_save = Sjson.Encode( JsonSerializer.Save(_project) );
        }

        private bool CheckSave()
        {
            ParseProject();
            if (_last_save == Sjson.Encode(JsonSerializer.Save(_project)))
                return true;
            saveProjectToolStripMenuItem_Click(null, null);
            return true;
        }

        private void newProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CheckSave())
            {
                _file = null;
                _project = new Data.Project();
                DisplayProject();
                MarkSave();
            }
        }

        private void closeProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            newProjectToolStripMenuItem_Click(sender, e);
        }

        private void Open(string path)
        {
            _file = path;
            Properties.Settings.Default.LastProject = _file;
            _project = new Data.Project();
            JsonSerializer.Load(_project, Sjson.Load(_file));
            MarkSave();
            DisplayProject();
        }

        private void openProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CheckSave())
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = _filter;
                if (ofd.ShowDialog() == DialogResult.OK)
                    Open(ofd.FileName);
            }

        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();
            if (!CheckSave())
                e.Cancel = true;
        }

        #endregion

        #region Scan Menu

        private void scanToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ParseProject();
            Parser.Scanner scanner = new Parser.Scanner(_project);
            
            DateTime started = DateTime.Now;
            ProgressDialog d = new ProgressDialog();
            d.Text = "Scanning source files...";
            d.Work = (feedback) => scanner.Rescan(feedback);
            d.Start();
            _project.LastScan = started;

            DisplayProject();

            ReportForm rf = new ReportForm(_project, scanner);
            rf.Show();
        }

        private void cleanRescanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _project.Clean();
            scanToolStripMenuItem1_Click(sender, e);
        }

        #endregion
    }
}
