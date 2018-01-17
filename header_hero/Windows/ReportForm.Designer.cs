namespace HeaderHero
{
    partial class ReportForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReportForm));
            this.tabPages = new System.Windows.Forms.TabControl();
            this.reportTab = new System.Windows.Forms.TabPage();
            this.reportBrowser = new System.Windows.Forms.WebBrowser();
            this.includeTab = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.includesListView = new System.Windows.Forms.ListView();
            this.IncludesFile = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.IncludesCount = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.IncludesLines = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.fileListView = new System.Windows.Forms.ListView();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.includedByListView = new System.Windows.Forms.ListView();
            this.ByFile = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ByCount = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ByLines = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.fileColHeader = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.btnBack = new System.Windows.Forms.Button();
            this.errorsTab = new System.Windows.Forms.TabPage();
            this.errorsListView = new System.Windows.Forms.ListView();
            this.missingTab = new System.Windows.Forms.TabPage();
            this.missingFilesListView = new System.Windows.Forms.ListView();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.scanToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rescanToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabPages.SuspendLayout();
            this.reportTab.SuspendLayout();
            this.includeTab.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.fileColHeader.SuspendLayout();
            this.errorsTab.SuspendLayout();
            this.missingTab.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabPages
            // 
            this.tabPages.Controls.Add(this.reportTab);
            this.tabPages.Controls.Add(this.includeTab);
            this.tabPages.Controls.Add(this.errorsTab);
            this.tabPages.Controls.Add(this.missingTab);
            this.tabPages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabPages.Location = new System.Drawing.Point(0, 35);
            this.tabPages.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabPages.Name = "tabPages";
            this.tabPages.SelectedIndex = 0;
            this.tabPages.Size = new System.Drawing.Size(1388, 1022);
            this.tabPages.TabIndex = 0;
            // 
            // reportTab
            // 
            this.reportTab.Controls.Add(this.reportBrowser);
            this.reportTab.Location = new System.Drawing.Point(4, 29);
            this.reportTab.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.reportTab.Name = "reportTab";
            this.reportTab.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.reportTab.Size = new System.Drawing.Size(1380, 989);
            this.reportTab.TabIndex = 0;
            this.reportTab.Text = "Report";
            this.reportTab.UseVisualStyleBackColor = true;
            // 
            // reportBrowser
            // 
            this.reportBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.reportBrowser.Location = new System.Drawing.Point(4, 5);
            this.reportBrowser.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.reportBrowser.MinimumSize = new System.Drawing.Size(30, 31);
            this.reportBrowser.Name = "reportBrowser";
            this.reportBrowser.Size = new System.Drawing.Size(1372, 979);
            this.reportBrowser.TabIndex = 0;
            // 
            // includeTab
            // 
            this.includeTab.Controls.Add(this.tableLayoutPanel1);
            this.includeTab.Location = new System.Drawing.Point(4, 29);
            this.includeTab.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.includeTab.Name = "includeTab";
            this.includeTab.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.includeTab.Size = new System.Drawing.Size(1380, 989);
            this.includeTab.TabIndex = 3;
            this.includeTab.Text = "Includes";
            this.includeTab.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.Controls.Add(this.includesListView, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.fileListView, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label3, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.includedByListView, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.fileColHeader, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(4, 5);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1372, 979);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // includesListView
            // 
            this.includesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.IncludesFile,
            this.IncludesCount,
            this.IncludesLines});
            this.includesListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.includesListView.Location = new System.Drawing.Point(918, 36);
            this.includesListView.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.includesListView.Name = "includesListView";
            this.includesListView.Size = new System.Drawing.Size(450, 938);
            this.includesListView.TabIndex = 5;
            this.includesListView.UseCompatibleStateImageBehavior = false;
            this.includesListView.View = System.Windows.Forms.View.Details;
            // 
            // IncludesFile
            // 
            this.IncludesFile.Text = "File";
            this.IncludesFile.Width = 160;
            // 
            // IncludesCount
            // 
            this.IncludesCount.Text = "Count";
            this.IncludesCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // IncludesLines
            // 
            this.IncludesLines.Text = "Lines";
            this.IncludesLines.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // fileListView
            // 
            this.fileListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileListView.Location = new System.Drawing.Point(461, 36);
            this.fileListView.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.fileListView.Name = "fileListView";
            this.fileListView.Size = new System.Drawing.Size(449, 938);
            this.fileListView.TabIndex = 4;
            this.fileListView.UseCompatibleStateImageBehavior = false;
            this.fileListView.View = System.Windows.Forms.View.List;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(918, 0);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 20);
            this.label3.TabIndex = 2;
            this.label3.Text = "Includes:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(94, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Included by:";
            // 
            // includedByListView
            // 
            this.includedByListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ByFile,
            this.ByCount,
            this.ByLines});
            this.includedByListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.includedByListView.Location = new System.Drawing.Point(4, 36);
            this.includedByListView.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.includedByListView.Name = "includedByListView";
            this.includedByListView.Size = new System.Drawing.Size(449, 938);
            this.includedByListView.TabIndex = 3;
            this.includedByListView.UseCompatibleStateImageBehavior = false;
            this.includedByListView.View = System.Windows.Forms.View.Details;
            // 
            // ByFile
            // 
            this.ByFile.Text = "File";
            this.ByFile.Width = 160;
            // 
            // ByCount
            // 
            this.ByCount.Text = "Count";
            this.ByCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // ByLines
            // 
            this.ByLines.Text = "Lines";
            this.ByLines.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // fileColHeader
            // 
            this.fileColHeader.ColumnCount = 2;
            this.fileColHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.fileColHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.fileColHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.fileColHeader.Controls.Add(this.label2, 0, 0);
            this.fileColHeader.Controls.Add(this.btnBack, 1, 0);
            this.fileColHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileColHeader.Location = new System.Drawing.Point(457, 0);
            this.fileColHeader.Margin = new System.Windows.Forms.Padding(0);
            this.fileColHeader.Name = "fileColHeader";
            this.fileColHeader.RowCount = 1;
            this.fileColHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.fileColHeader.Size = new System.Drawing.Size(457, 31);
            this.fileColHeader.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 20);
            this.label2.TabIndex = 0;
            this.label2.Text = "File:";
            // 
            // btnBack
            // 
            this.btnBack.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnBack.Location = new System.Drawing.Point(417, 0);
            this.btnBack.Margin = new System.Windows.Forms.Padding(0);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(40, 31);
            this.btnBack.TabIndex = 1;
            this.btnBack.Text = "<";
            this.btnBack.UseVisualStyleBackColor = true;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // errorsTab
            // 
            this.errorsTab.Controls.Add(this.errorsListView);
            this.errorsTab.Location = new System.Drawing.Point(4, 29);
            this.errorsTab.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.errorsTab.Name = "errorsTab";
            this.errorsTab.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.errorsTab.Size = new System.Drawing.Size(1380, 989);
            this.errorsTab.TabIndex = 1;
            this.errorsTab.Text = "Errors";
            this.errorsTab.UseVisualStyleBackColor = true;
            // 
            // errorsListView
            // 
            this.errorsListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.errorsListView.Location = new System.Drawing.Point(4, 5);
            this.errorsListView.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.errorsListView.Name = "errorsListView";
            this.errorsListView.Size = new System.Drawing.Size(1372, 979);
            this.errorsListView.TabIndex = 0;
            this.errorsListView.UseCompatibleStateImageBehavior = false;
            this.errorsListView.View = System.Windows.Forms.View.List;
            // 
            // missingTab
            // 
            this.missingTab.Controls.Add(this.missingFilesListView);
            this.missingTab.Location = new System.Drawing.Point(4, 29);
            this.missingTab.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.missingTab.Name = "missingTab";
            this.missingTab.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.missingTab.Size = new System.Drawing.Size(1380, 989);
            this.missingTab.TabIndex = 2;
            this.missingTab.Text = "Missing Files";
            this.missingTab.UseVisualStyleBackColor = true;
            // 
            // missingFilesListView
            // 
            this.missingFilesListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.missingFilesListView.Location = new System.Drawing.Point(4, 5);
            this.missingFilesListView.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.missingFilesListView.Name = "missingFilesListView";
            this.missingFilesListView.ShowItemToolTips = true;
            this.missingFilesListView.Size = new System.Drawing.Size(1372, 979);
            this.missingFilesListView.TabIndex = 1;
            this.missingFilesListView.UseCompatibleStateImageBehavior = false;
            this.missingFilesListView.View = System.Windows.Forms.View.List;
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.scanToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(9, 3, 0, 3);
            this.menuStrip1.Size = new System.Drawing.Size(1388, 35);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // scanToolStripMenuItem
            // 
            this.scanToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rescanToolStripMenuItem});
            this.scanToolStripMenuItem.Name = "scanToolStripMenuItem";
            this.scanToolStripMenuItem.Size = new System.Drawing.Size(61, 29);
            this.scanToolStripMenuItem.Text = "Scan";
            // 
            // rescanToolStripMenuItem
            // 
            this.rescanToolStripMenuItem.Name = "rescanToolStripMenuItem";
            this.rescanToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.rescanToolStripMenuItem.Size = new System.Drawing.Size(212, 30);
            this.rescanToolStripMenuItem.Text = "Rescan";
            this.rescanToolStripMenuItem.Click += new System.EventHandler(this.rescanToolStripMenuItem_Click);
            // 
            // ReportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1388, 1057);
            this.Controls.Add(this.tabPages);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "ReportForm";
            this.Text = "Report";
            this.tabPages.ResumeLayout(false);
            this.reportTab.ResumeLayout(false);
            this.includeTab.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.fileColHeader.ResumeLayout(false);
            this.fileColHeader.PerformLayout();
            this.errorsTab.ResumeLayout(false);
            this.missingTab.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabPages;
        private System.Windows.Forms.TabPage reportTab;
        private System.Windows.Forms.TabPage errorsTab;
        private System.Windows.Forms.TabPage missingTab;
        private System.Windows.Forms.ListView errorsListView;
        private System.Windows.Forms.ListView missingFilesListView;
        private System.Windows.Forms.TabPage includeTab;
        private System.Windows.Forms.WebBrowser reportBrowser;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ListView includesListView;
        private System.Windows.Forms.ListView fileListView;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListView includedByListView;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem scanToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rescanToolStripMenuItem;
        private System.Windows.Forms.ColumnHeader ByFile;
        private System.Windows.Forms.ColumnHeader ByCount;
        private System.Windows.Forms.ColumnHeader ByLines;
        private System.Windows.Forms.ColumnHeader IncludesFile;
        private System.Windows.Forms.ColumnHeader IncludesCount;
        private System.Windows.Forms.ColumnHeader IncludesLines;
        private System.Windows.Forms.TableLayoutPanel fileColHeader;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnBack;
    }
}