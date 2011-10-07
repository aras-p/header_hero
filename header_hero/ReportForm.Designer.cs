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
            this.tabPages = new System.Windows.Forms.TabControl();
            this.reportTab = new System.Windows.Forms.TabPage();
            this.reportBrowser = new System.Windows.Forms.WebBrowser();
            this.includeTab = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.includesListView = new System.Windows.Forms.ListView();
            this.fileListView = new System.Windows.Forms.ListView();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.includedByListView = new System.Windows.Forms.ListView();
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
            this.tabPages.Location = new System.Drawing.Point(0, 24);
            this.tabPages.Name = "tabPages";
            this.tabPages.SelectedIndex = 0;
            this.tabPages.Size = new System.Drawing.Size(641, 663);
            this.tabPages.TabIndex = 0;
            // 
            // reportTab
            // 
            this.reportTab.Controls.Add(this.reportBrowser);
            this.reportTab.Location = new System.Drawing.Point(4, 22);
            this.reportTab.Name = "reportTab";
            this.reportTab.Padding = new System.Windows.Forms.Padding(3);
            this.reportTab.Size = new System.Drawing.Size(633, 637);
            this.reportTab.TabIndex = 0;
            this.reportTab.Text = "Report";
            this.reportTab.UseVisualStyleBackColor = true;
            // 
            // reportBrowser
            // 
            this.reportBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.reportBrowser.Location = new System.Drawing.Point(3, 3);
            this.reportBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.reportBrowser.Name = "reportBrowser";
            this.reportBrowser.Size = new System.Drawing.Size(627, 631);
            this.reportBrowser.TabIndex = 0;
            // 
            // includeTab
            // 
            this.includeTab.Controls.Add(this.tableLayoutPanel1);
            this.includeTab.Location = new System.Drawing.Point(4, 22);
            this.includeTab.Name = "includeTab";
            this.includeTab.Padding = new System.Windows.Forms.Padding(3);
            this.includeTab.Size = new System.Drawing.Size(633, 661);
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
            this.tableLayoutPanel1.Controls.Add(this.label2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.includedByListView, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(627, 655);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // includesListView
            // 
            this.includesListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.includesListView.Location = new System.Drawing.Point(421, 23);
            this.includesListView.Name = "includesListView";
            this.includesListView.Size = new System.Drawing.Size(203, 629);
            this.includesListView.TabIndex = 5;
            this.includesListView.UseCompatibleStateImageBehavior = false;
            this.includesListView.View = System.Windows.Forms.View.List;
            // 
            // fileListView
            // 
            this.fileListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileListView.Location = new System.Drawing.Point(212, 23);
            this.fileListView.Name = "fileListView";
            this.fileListView.Size = new System.Drawing.Size(203, 629);
            this.fileListView.TabIndex = 4;
            this.fileListView.UseCompatibleStateImageBehavior = false;
            this.fileListView.View = System.Windows.Forms.View.List;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(421, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Includes:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(212, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "File:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Included by:";
            // 
            // includedByListView
            // 
            this.includedByListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.includedByListView.Location = new System.Drawing.Point(3, 23);
            this.includedByListView.Name = "includedByListView";
            this.includedByListView.Size = new System.Drawing.Size(203, 629);
            this.includedByListView.TabIndex = 3;
            this.includedByListView.UseCompatibleStateImageBehavior = false;
            this.includedByListView.View = System.Windows.Forms.View.List;
            // 
            // errorsTab
            // 
            this.errorsTab.Controls.Add(this.errorsListView);
            this.errorsTab.Location = new System.Drawing.Point(4, 22);
            this.errorsTab.Name = "errorsTab";
            this.errorsTab.Padding = new System.Windows.Forms.Padding(3);
            this.errorsTab.Size = new System.Drawing.Size(633, 661);
            this.errorsTab.TabIndex = 1;
            this.errorsTab.Text = "Errors";
            this.errorsTab.UseVisualStyleBackColor = true;
            // 
            // errorsListView
            // 
            this.errorsListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.errorsListView.Location = new System.Drawing.Point(3, 3);
            this.errorsListView.Name = "errorsListView";
            this.errorsListView.Size = new System.Drawing.Size(627, 655);
            this.errorsListView.TabIndex = 0;
            this.errorsListView.UseCompatibleStateImageBehavior = false;
            this.errorsListView.View = System.Windows.Forms.View.List;
            // 
            // missingTab
            // 
            this.missingTab.Controls.Add(this.missingFilesListView);
            this.missingTab.Location = new System.Drawing.Point(4, 22);
            this.missingTab.Name = "missingTab";
            this.missingTab.Padding = new System.Windows.Forms.Padding(3);
            this.missingTab.Size = new System.Drawing.Size(633, 661);
            this.missingTab.TabIndex = 2;
            this.missingTab.Text = "Missing Files";
            this.missingTab.UseVisualStyleBackColor = true;
            // 
            // missingFilesListView
            // 
            this.missingFilesListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.missingFilesListView.Location = new System.Drawing.Point(3, 3);
            this.missingFilesListView.Name = "missingFilesListView";
            this.missingFilesListView.Size = new System.Drawing.Size(627, 655);
            this.missingFilesListView.TabIndex = 1;
            this.missingFilesListView.UseCompatibleStateImageBehavior = false;
            this.missingFilesListView.View = System.Windows.Forms.View.List;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.scanToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(641, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // scanToolStripMenuItem
            // 
            this.scanToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rescanToolStripMenuItem});
            this.scanToolStripMenuItem.Name = "scanToolStripMenuItem";
            this.scanToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.scanToolStripMenuItem.Text = "Scan";
            // 
            // rescanToolStripMenuItem
            // 
            this.rescanToolStripMenuItem.Name = "rescanToolStripMenuItem";
            this.rescanToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.rescanToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.rescanToolStripMenuItem.Text = "Rescan";
            this.rescanToolStripMenuItem.Click += new System.EventHandler(this.rescanToolStripMenuItem_Click);
            // 
            // ReportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(641, 687);
            this.Controls.Add(this.tabPages);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "ReportForm";
            this.Text = "Report";
            this.tabPages.ResumeLayout(false);
            this.reportTab.ResumeLayout(false);
            this.includeTab.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
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
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListView includedByListView;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem scanToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rescanToolStripMenuItem;
    }
}