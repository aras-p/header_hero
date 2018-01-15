namespace HeaderHero
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            this.menuBarMenuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.openProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scanToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scanToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.cleanRescanToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.includeDirsTextBox = new System.Windows.Forms.TextBox();
            this.includeDirsContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.label3 = new System.Windows.Forms.Label();
            this.lastScanLabel = new System.Windows.Forms.Label();
            this.projectDirsTextBox = new System.Windows.Forms.TextBox();
            this.projectDirContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addDirectoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label4 = new System.Windows.Forms.Label();
            this.pchTextBox = new System.Windows.Forms.TextBox();
            this.menuBarMenuStrip.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.includeDirsContextMenuStrip.SuspendLayout();
            this.projectDirContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuBarMenuStrip
            // 
            this.menuBarMenuStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuBarMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.scanToolStripMenuItem});
            this.menuBarMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuBarMenuStrip.Name = "menuBarMenuStrip";
            this.menuBarMenuStrip.Padding = new System.Windows.Forms.Padding(9, 3, 0, 3);
            this.menuBarMenuStrip.Size = new System.Drawing.Size(908, 35);
            this.menuBarMenuStrip.TabIndex = 0;
            this.menuBarMenuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newProjectToolStripMenuItem,
            this.toolStripMenuItem1,
            this.openProjectToolStripMenuItem,
            this.closeProjectToolStripMenuItem,
            this.saveProjectToolStripMenuItem,
            this.toolStripMenuItem2,
            this.quitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(50, 29);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newProjectToolStripMenuItem
            // 
            this.newProjectToolStripMenuItem.Name = "newProjectToolStripMenuItem";
            this.newProjectToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newProjectToolStripMenuItem.Size = new System.Drawing.Size(276, 30);
            this.newProjectToolStripMenuItem.Text = "New Project";
            this.newProjectToolStripMenuItem.Click += new System.EventHandler(this.newProjectToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(273, 6);
            // 
            // openProjectToolStripMenuItem
            // 
            this.openProjectToolStripMenuItem.Name = "openProjectToolStripMenuItem";
            this.openProjectToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openProjectToolStripMenuItem.Size = new System.Drawing.Size(276, 30);
            this.openProjectToolStripMenuItem.Text = "Open Project...";
            this.openProjectToolStripMenuItem.Click += new System.EventHandler(this.openProjectToolStripMenuItem_Click);
            // 
            // closeProjectToolStripMenuItem
            // 
            this.closeProjectToolStripMenuItem.Name = "closeProjectToolStripMenuItem";
            this.closeProjectToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.W)));
            this.closeProjectToolStripMenuItem.Size = new System.Drawing.Size(276, 30);
            this.closeProjectToolStripMenuItem.Text = "Close Project";
            this.closeProjectToolStripMenuItem.Click += new System.EventHandler(this.closeProjectToolStripMenuItem_Click);
            // 
            // saveProjectToolStripMenuItem
            // 
            this.saveProjectToolStripMenuItem.Name = "saveProjectToolStripMenuItem";
            this.saveProjectToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveProjectToolStripMenuItem.Size = new System.Drawing.Size(276, 30);
            this.saveProjectToolStripMenuItem.Text = "Save Project...";
            this.saveProjectToolStripMenuItem.Click += new System.EventHandler(this.saveProjectToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(273, 6);
            // 
            // quitToolStripMenuItem
            // 
            this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
            this.quitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Q)));
            this.quitToolStripMenuItem.Size = new System.Drawing.Size(276, 30);
            this.quitToolStripMenuItem.Text = "Quit";
            // 
            // scanToolStripMenuItem
            // 
            this.scanToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.scanToolStripMenuItem1,
            this.cleanRescanToolStripMenuItem});
            this.scanToolStripMenuItem.Name = "scanToolStripMenuItem";
            this.scanToolStripMenuItem.Size = new System.Drawing.Size(61, 29);
            this.scanToolStripMenuItem.Text = "Scan";
            // 
            // scanToolStripMenuItem1
            // 
            this.scanToolStripMenuItem1.Name = "scanToolStripMenuItem1";
            this.scanToolStripMenuItem1.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.scanToolStripMenuItem1.Size = new System.Drawing.Size(198, 30);
            this.scanToolStripMenuItem1.Text = "Scan";
            this.scanToolStripMenuItem1.Click += new System.EventHandler(this.scanToolStripMenuItem1_Click);
            // 
            // cleanRescanToolStripMenuItem
            // 
            this.cleanRescanToolStripMenuItem.Name = "cleanRescanToolStripMenuItem";
            this.cleanRescanToolStripMenuItem.Size = new System.Drawing.Size(198, 30);
            this.cleanRescanToolStripMenuItem.Text = "Clean Rescan";
            this.cleanRescanToolStripMenuItem.Click += new System.EventHandler(this.cleanRescanToolStripMenuItem_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 31);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Project dirs:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 0);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "Last scan:";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.includeDirsTextBox, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.lastScanLabel, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.projectDirsTextBox, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.pchTextBox, 1, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 35);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(908, 661);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // includeDirsTextBox
            // 
            this.includeDirsTextBox.ContextMenuStrip = this.includeDirsContextMenuStrip;
            this.includeDirsTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.includeDirsTextBox.Location = new System.Drawing.Point(154, 333);
            this.includeDirsTextBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.includeDirsTextBox.Multiline = true;
            this.includeDirsTextBox.Name = "includeDirsTextBox";
            this.includeDirsTextBox.Size = new System.Drawing.Size(750, 287);
            this.includeDirsTextBox.TabIndex = 6;
            // 
            // includeDirsContextMenuStrip
            // 
            this.includeDirsContextMenuStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.includeDirsContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem3});
            this.includeDirsContextMenuStrip.Name = "projectDirContextMenuStrip";
            this.includeDirsContextMenuStrip.Size = new System.Drawing.Size(208, 34);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(207, 30);
            this.toolStripMenuItem3.Text = "Add Directory...";
            this.toolStripMenuItem3.Click += new System.EventHandler(this.include_AddDirectory_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 328);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(94, 20);
            this.label3.TabIndex = 3;
            this.label3.Text = "Include dirs:";
            // 
            // lastScanLabel
            // 
            this.lastScanLabel.AutoSize = true;
            this.lastScanLabel.Location = new System.Drawing.Point(154, 0);
            this.lastScanLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lastScanLabel.Name = "lastScanLabel";
            this.lastScanLabel.Size = new System.Drawing.Size(123, 20);
            this.lastScanLabel.TabIndex = 4;
            this.lastScanLabel.Text = "(never scanned)";
            // 
            // projectDirsTextBox
            // 
            this.projectDirsTextBox.ContextMenuStrip = this.projectDirContextMenuStrip;
            this.projectDirsTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.projectDirsTextBox.Location = new System.Drawing.Point(154, 36);
            this.projectDirsTextBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.projectDirsTextBox.Multiline = true;
            this.projectDirsTextBox.Name = "projectDirsTextBox";
            this.projectDirsTextBox.Size = new System.Drawing.Size(750, 287);
            this.projectDirsTextBox.TabIndex = 5;
            // 
            // projectDirContextMenuStrip
            // 
            this.projectDirContextMenuStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.projectDirContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addDirectoryToolStripMenuItem});
            this.projectDirContextMenuStrip.Name = "projectDirContextMenuStrip";
            this.projectDirContextMenuStrip.Size = new System.Drawing.Size(208, 34);
            // 
            // addDirectoryToolStripMenuItem
            // 
            this.addDirectoryToolStripMenuItem.Name = "addDirectoryToolStripMenuItem";
            this.addDirectoryToolStripMenuItem.Size = new System.Drawing.Size(207, 30);
            this.addDirectoryToolStripMenuItem.Text = "Add Directory...";
            this.addDirectoryToolStripMenuItem.Click += new System.EventHandler(this.scan_AddDirectory_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 625);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(134, 20);
            this.label4.TabIndex = 7;
            this.label4.Text = "Precomp. header:";
            // 
            // pchTextBox
            // 
            this.pchTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pchTextBox.Location = new System.Drawing.Point(153, 628);
            this.pchTextBox.Name = "pchTextBox";
            this.pchTextBox.Size = new System.Drawing.Size(752, 26);
            this.pchTextBox.TabIndex = 8;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(908, 696);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.menuBarMenuStrip);
            this.MainMenuStrip = this.menuBarMenuStrip;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "MainForm";
            this.Text = "Header Hero";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.menuBarMenuStrip.ResumeLayout(false);
            this.menuBarMenuStrip.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.includeDirsContextMenuStrip.ResumeLayout(false);
            this.projectDirContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuBarMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem openProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem quitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem scanToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem scanToolStripMenuItem1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lastScanLabel;
        private System.Windows.Forms.ContextMenuStrip projectDirContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem addDirectoryToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip includeDirsContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.TextBox includeDirsTextBox;
        private System.Windows.Forms.TextBox projectDirsTextBox;
        private System.Windows.Forms.ToolStripMenuItem cleanRescanToolStripMenuItem;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox pchTextBox;
    }
}

