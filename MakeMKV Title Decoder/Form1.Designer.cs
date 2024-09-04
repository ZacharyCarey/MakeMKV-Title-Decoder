namespace MakeMKV_Title_Decoder {
    partial class Form1 {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            components = new System.ComponentModel.Container();
            toolTip1 = new ToolTip(components);
            IgnoreIncompleteCheckBox = new CheckBox();
            menuStrip1 = new MenuStrip();
            discToolStripMenuItem = new ToolStripMenuItem();
            saveDiscToolStripMenuItem = new ToolStripMenuItem();
            loadDiscToolStripMenuItem = new ToolStripMenuItem();
            renameDataToolStripMenuItem = new ToolStripMenuItem();
            loadToolStripMenuItem = new ToolStripMenuItem();
            viewInfoToolStripMenuItem = new ToolStripMenuItem();
            testToolStripMenuItem = new ToolStripMenuItem();
            renameToolStripMenuItem = new ToolStripMenuItem();
            loadDiscToolStripMenuItem1 = new ToolStripMenuItem();
            renamesToolStripMenuItem = new ToolStripMenuItem();
            saveToolStripMenuItem = new ToolStripMenuItem();
            loadToolStripMenuItem1 = new ToolStripMenuItem();
            clipRenamerToolStripMenuItem1 = new ToolStripMenuItem();
            DriveSelectionPanel = new Panel();
            SelectFolderBtn = new Button();
            LoadedFileLabel = new Label();
            label1 = new Label();
            DriveBtnPanel = new Panel();
            ReadBtn = new Button();
            DownloadBtn = new Button();
            RenameVideosBtn = new Button();
            TotalProgressBar = new ProgressBar();
            CurrentProgressBar = new ProgressBar();
            DiscNameLabel = new Label();
            label3 = new Label();
            DrivesComboBox = new ComboBox();
            RefreshDrivesBtn = new Button();
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            tabPage2 = new TabPage();
            menuStrip1.SuspendLayout();
            DriveSelectionPanel.SuspendLayout();
            DriveBtnPanel.SuspendLayout();
            tabControl1.SuspendLayout();
            SuspendLayout();
            // 
            // IgnoreIncompleteCheckBox
            // 
            IgnoreIncompleteCheckBox.AutoSize = true;
            IgnoreIncompleteCheckBox.Checked = true;
            IgnoreIncompleteCheckBox.CheckState = CheckState.Checked;
            IgnoreIncompleteCheckBox.Location = new Point(3, 128);
            IgnoreIncompleteCheckBox.Name = "IgnoreIncompleteCheckBox";
            IgnoreIncompleteCheckBox.Size = new Size(160, 19);
            IgnoreIncompleteCheckBox.TabIndex = 14;
            IgnoreIncompleteCheckBox.Text = "Ignore incomplete videos";
            toolTip1.SetToolTip(IgnoreIncompleteCheckBox, "Ignore any titles that are missing either video or audio. Usually federal warnings.");
            IgnoreIncompleteCheckBox.UseVisualStyleBackColor = true;
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { discToolStripMenuItem, renameDataToolStripMenuItem, viewInfoToolStripMenuItem, testToolStripMenuItem, renameToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(829, 24);
            menuStrip1.TabIndex = 6;
            menuStrip1.Text = "menuStrip1";
            // 
            // discToolStripMenuItem
            // 
            discToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { saveDiscToolStripMenuItem, loadDiscToolStripMenuItem });
            discToolStripMenuItem.Name = "discToolStripMenuItem";
            discToolStripMenuItem.Size = new Size(41, 20);
            discToolStripMenuItem.Text = "Disc";
            // 
            // saveDiscToolStripMenuItem
            // 
            saveDiscToolStripMenuItem.Name = "saveDiscToolStripMenuItem";
            saveDiscToolStripMenuItem.Size = new Size(100, 22);
            saveDiscToolStripMenuItem.Text = "Save";
            saveDiscToolStripMenuItem.Click += saveDiscToolStripMenuItem_Click;
            // 
            // loadDiscToolStripMenuItem
            // 
            loadDiscToolStripMenuItem.Name = "loadDiscToolStripMenuItem";
            loadDiscToolStripMenuItem.Size = new Size(100, 22);
            loadDiscToolStripMenuItem.Text = "Load";
            loadDiscToolStripMenuItem.Click += loadDiscToolStripMenuItem_Click;
            // 
            // renameDataToolStripMenuItem
            // 
            renameDataToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { loadToolStripMenuItem });
            renameDataToolStripMenuItem.Name = "renameDataToolStripMenuItem";
            renameDataToolStripMenuItem.Size = new Size(89, 20);
            renameDataToolStripMenuItem.Text = "Rename Data";
            // 
            // loadToolStripMenuItem
            // 
            loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            loadToolStripMenuItem.Size = new Size(100, 22);
            loadToolStripMenuItem.Text = "Load";
            loadToolStripMenuItem.Click += loadToolStripMenuItem_Click;
            // 
            // viewInfoToolStripMenuItem
            // 
            viewInfoToolStripMenuItem.Name = "viewInfoToolStripMenuItem";
            viewInfoToolStripMenuItem.Size = new Size(68, 20);
            viewInfoToolStripMenuItem.Text = "View Info";
            viewInfoToolStripMenuItem.Click += viewInfoToolStripMenuItem_Click;
            // 
            // testToolStripMenuItem
            // 
            testToolStripMenuItem.Name = "testToolStripMenuItem";
            testToolStripMenuItem.Size = new Size(39, 20);
            testToolStripMenuItem.Text = "Test";
            testToolStripMenuItem.Click += testToolStripMenuItem_Click;
            // 
            // renameToolStripMenuItem
            // 
            renameToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { loadDiscToolStripMenuItem1, clipRenamerToolStripMenuItem1, renamesToolStripMenuItem });
            renameToolStripMenuItem.Name = "renameToolStripMenuItem";
            renameToolStripMenuItem.Size = new Size(62, 20);
            renameToolStripMenuItem.Text = "Rename";
            // 
            // loadDiscToolStripMenuItem1
            // 
            loadDiscToolStripMenuItem1.Name = "loadDiscToolStripMenuItem1";
            loadDiscToolStripMenuItem1.Size = new Size(180, 22);
            loadDiscToolStripMenuItem1.Text = "Load Disc";
            loadDiscToolStripMenuItem1.Click += loadDiscToolStripMenuItem1_Click;
            // 
            // renamesToolStripMenuItem
            // 
            renamesToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { saveToolStripMenuItem, loadToolStripMenuItem1 });
            renamesToolStripMenuItem.Name = "renamesToolStripMenuItem";
            renamesToolStripMenuItem.Size = new Size(180, 22);
            renamesToolStripMenuItem.Text = "Renames File";
            // 
            // saveToolStripMenuItem
            // 
            saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            saveToolStripMenuItem.Size = new Size(180, 22);
            saveToolStripMenuItem.Text = "Save";
            saveToolStripMenuItem.Click += saveToolStripMenuItem_Click;
            // 
            // loadToolStripMenuItem1
            // 
            loadToolStripMenuItem1.Name = "loadToolStripMenuItem1";
            loadToolStripMenuItem1.Size = new Size(180, 22);
            loadToolStripMenuItem1.Text = "Load";
            loadToolStripMenuItem1.Click += loadToolStripMenuItem1_Click;
            // 
            // clipRenamerToolStripMenuItem1
            // 
            clipRenamerToolStripMenuItem1.Name = "clipRenamerToolStripMenuItem1";
            clipRenamerToolStripMenuItem1.Size = new Size(180, 22);
            clipRenamerToolStripMenuItem1.Text = "Clip Renamer";
            clipRenamerToolStripMenuItem1.Click += clipRenamerToolStripMenuItem1_Click;
            // 
            // DriveSelectionPanel
            // 
            DriveSelectionPanel.Controls.Add(SelectFolderBtn);
            DriveSelectionPanel.Controls.Add(LoadedFileLabel);
            DriveSelectionPanel.Controls.Add(label1);
            DriveSelectionPanel.Controls.Add(DriveBtnPanel);
            DriveSelectionPanel.Controls.Add(IgnoreIncompleteCheckBox);
            DriveSelectionPanel.Controls.Add(RenameVideosBtn);
            DriveSelectionPanel.Controls.Add(TotalProgressBar);
            DriveSelectionPanel.Controls.Add(CurrentProgressBar);
            DriveSelectionPanel.Controls.Add(DiscNameLabel);
            DriveSelectionPanel.Controls.Add(label3);
            DriveSelectionPanel.Controls.Add(DrivesComboBox);
            DriveSelectionPanel.Controls.Add(RefreshDrivesBtn);
            DriveSelectionPanel.Location = new Point(12, 27);
            DriveSelectionPanel.Name = "DriveSelectionPanel";
            DriveSelectionPanel.Size = new Size(525, 184);
            DriveSelectionPanel.TabIndex = 12;
            // 
            // SelectFolderBtn
            // 
            SelectFolderBtn.Location = new Point(105, 153);
            SelectFolderBtn.Name = "SelectFolderBtn";
            SelectFolderBtn.Size = new Size(96, 23);
            SelectFolderBtn.TabIndex = 17;
            SelectFolderBtn.Text = "Select Folder";
            SelectFolderBtn.UseVisualStyleBackColor = true;
            SelectFolderBtn.Click += SelectFolderBtn_Click;
            // 
            // LoadedFileLabel
            // 
            LoadedFileLabel.AutoSize = true;
            LoadedFileLabel.Location = new Point(281, 157);
            LoadedFileLabel.Name = "LoadedFileLabel";
            LoadedFileLabel.Size = new Size(29, 15);
            LoadedFileLabel.TabIndex = 16;
            LoadedFileLabel.Text = "N/A";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(207, 157);
            label1.Name = "label1";
            label1.Size = new Size(68, 15);
            label1.TabIndex = 15;
            label1.Text = "Loaded file:";
            // 
            // DriveBtnPanel
            // 
            DriveBtnPanel.Controls.Add(ReadBtn);
            DriveBtnPanel.Controls.Add(DownloadBtn);
            DriveBtnPanel.Enabled = false;
            DriveBtnPanel.Location = new Point(417, 3);
            DriveBtnPanel.Name = "DriveBtnPanel";
            DriveBtnPanel.Size = new Size(101, 56);
            DriveBtnPanel.TabIndex = 8;
            // 
            // ReadBtn
            // 
            ReadBtn.Location = new Point(3, 4);
            ReadBtn.Name = "ReadBtn";
            ReadBtn.Size = new Size(75, 23);
            ReadBtn.TabIndex = 7;
            ReadBtn.Text = "Read";
            ReadBtn.UseVisualStyleBackColor = true;
            ReadBtn.Click += ReadBtn_Click;
            // 
            // DownloadBtn
            // 
            DownloadBtn.Enabled = false;
            DownloadBtn.Location = new Point(3, 33);
            DownloadBtn.Name = "DownloadBtn";
            DownloadBtn.Size = new Size(75, 23);
            DownloadBtn.TabIndex = 5;
            DownloadBtn.Text = "Rip";
            DownloadBtn.UseVisualStyleBackColor = true;
            DownloadBtn.Click += DownloadBtn_Click;
            // 
            // RenameVideosBtn
            // 
            RenameVideosBtn.Enabled = false;
            RenameVideosBtn.Location = new Point(3, 153);
            RenameVideosBtn.Name = "RenameVideosBtn";
            RenameVideosBtn.Size = new Size(96, 23);
            RenameVideosBtn.TabIndex = 13;
            RenameVideosBtn.Text = "Rename Videos";
            RenameVideosBtn.UseVisualStyleBackColor = true;
            RenameVideosBtn.Click += RenameVideosBtn_Click;
            // 
            // TotalProgressBar
            // 
            TotalProgressBar.Location = new Point(3, 91);
            TotalProgressBar.Maximum = 65536;
            TotalProgressBar.Name = "TotalProgressBar";
            TotalProgressBar.Size = new Size(515, 23);
            TotalProgressBar.TabIndex = 6;
            // 
            // CurrentProgressBar
            // 
            CurrentProgressBar.Location = new Point(3, 62);
            CurrentProgressBar.Maximum = 65536;
            CurrentProgressBar.Name = "CurrentProgressBar";
            CurrentProgressBar.Size = new Size(515, 23);
            CurrentProgressBar.TabIndex = 4;
            // 
            // DiscNameLabel
            // 
            DiscNameLabel.AutoSize = true;
            DiscNameLabel.Location = new Point(158, 30);
            DiscNameLabel.Name = "DiscNameLabel";
            DiscNameLabel.Size = new Size(29, 15);
            DiscNameLabel.TabIndex = 3;
            DiscNameLabel.Text = "N/A";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(84, 30);
            label3.Name = "label3";
            label3.Size = new Size(68, 15);
            label3.TabIndex = 2;
            label3.Text = "Disc name: ";
            // 
            // DrivesComboBox
            // 
            DrivesComboBox.FormattingEnabled = true;
            DrivesComboBox.Location = new Point(84, 4);
            DrivesComboBox.Name = "DrivesComboBox";
            DrivesComboBox.Size = new Size(309, 23);
            DrivesComboBox.TabIndex = 1;
            DrivesComboBox.SelectedIndexChanged += DrivesComboBox_SelectedIndexChanged;
            // 
            // RefreshDrivesBtn
            // 
            RefreshDrivesBtn.Location = new Point(3, 4);
            RefreshDrivesBtn.Name = "RefreshDrivesBtn";
            RefreshDrivesBtn.Size = new Size(75, 23);
            RefreshDrivesBtn.TabIndex = 0;
            RefreshDrivesBtn.Text = "Refresh";
            RefreshDrivesBtn.UseVisualStyleBackColor = true;
            RefreshDrivesBtn.Click += RefreshDrivesBtn_Click;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Location = new Point(12, 232);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(761, 295);
            tabControl1.TabIndex = 13;
            // 
            // tabPage1
            // 
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(753, 267);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "tabPage1";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            tabPage2.Location = new Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(753, 267);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "tabPage2";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(829, 539);
            Controls.Add(tabControl1);
            Controls.Add(DriveSelectionPanel);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            DriveSelectionPanel.ResumeLayout(false);
            DriveSelectionPanel.PerformLayout();
            DriveBtnPanel.ResumeLayout(false);
            tabControl1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private ToolTip toolTip1;
        private MenuStrip menuStrip1;
        private Panel DriveSelectionPanel;
        private Button RefreshDrivesBtn;
        private ComboBox DrivesComboBox;
        private Label DiscNameLabel;
        private Label label3;
        private ProgressBar CurrentProgressBar;
        private Button DownloadBtn;
        private ProgressBar TotalProgressBar;
        private Button ReadBtn;
        private Panel DriveBtnPanel;
        private ToolStripMenuItem discToolStripMenuItem;
        private ToolStripMenuItem saveDiscToolStripMenuItem;
        private ToolStripMenuItem loadDiscToolStripMenuItem;
        private Button RenameVideosBtn;
        private CheckBox IgnoreIncompleteCheckBox;
        private Label LoadedFileLabel;
        private Label label1;
        private Button SelectFolderBtn;
        private ToolStripMenuItem renameDataToolStripMenuItem;
        private ToolStripMenuItem loadToolStripMenuItem;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private ToolStripMenuItem testToolStripMenuItem;
        private ToolStripMenuItem viewInfoToolStripMenuItem;
        private ToolStripMenuItem renameToolStripMenuItem;
        private ToolStripMenuItem loadDiscToolStripMenuItem1;
        private ToolStripMenuItem clipRenamerToolStripMenuItem1;
        private ToolStripMenuItem renamesToolStripMenuItem;
        private ToolStripMenuItem saveToolStripMenuItem;
        private ToolStripMenuItem loadToolStripMenuItem1;
    }
}
