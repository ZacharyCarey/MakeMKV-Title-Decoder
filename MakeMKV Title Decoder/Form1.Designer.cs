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
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            toolTip1 = new ToolTip(components);
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            saveToolStripMenuItem1 = new ToolStripMenuItem();
            loadToolStripMenuItem = new ToolStripMenuItem();
            viewInfoToolStripMenuItem = new ToolStripMenuItem();
            testToolStripMenuItem = new ToolStripMenuItem();
            label1 = new Label();
            LoadedDiscLabel = new Label();
            LoadDiscBtn = new Button();
            RenameClipsBtn = new Button();
            PlaylistsBtn = new Button();
            FileRenamerBtn = new Button();
            BackupBtn = new Button();
            AttachmentsBtn = new Button();
            CollectionsBtn = new Button();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(24, 24);
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, viewInfoToolStripMenuItem, testToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(309, 24);
            menuStrip1.TabIndex = 6;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { saveToolStripMenuItem1, loadToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 20);
            fileToolStripMenuItem.Text = "File";
            // 
            // saveToolStripMenuItem1
            // 
            saveToolStripMenuItem1.Name = "saveToolStripMenuItem1";
            saveToolStripMenuItem1.Size = new Size(100, 22);
            saveToolStripMenuItem1.Text = "Save";
            saveToolStripMenuItem1.Click += saveToolStripMenuItem1_Click;
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
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(8, 38);
            label1.Margin = new Padding(2, 0, 2, 0);
            label1.Name = "label1";
            label1.Size = new Size(74, 15);
            label1.TabIndex = 7;
            label1.Text = "Loaded Disc:";
            // 
            // LoadedDiscLabel
            // 
            LoadedDiscLabel.AutoSize = true;
            LoadedDiscLabel.Location = new Point(92, 38);
            LoadedDiscLabel.Margin = new Padding(2, 0, 2, 0);
            LoadedDiscLabel.Name = "LoadedDiscLabel";
            LoadedDiscLabel.Size = new Size(38, 15);
            LoadedDiscLabel.TabIndex = 8;
            LoadedDiscLabel.Text = "label2";
            // 
            // LoadDiscBtn
            // 
            LoadDiscBtn.Location = new Point(8, 55);
            LoadDiscBtn.Margin = new Padding(2);
            LoadDiscBtn.Name = "LoadDiscBtn";
            LoadDiscBtn.Size = new Size(78, 20);
            LoadDiscBtn.TabIndex = 9;
            LoadDiscBtn.Text = "Load";
            LoadDiscBtn.UseVisualStyleBackColor = true;
            LoadDiscBtn.Click += LoadDiscBtn_Click;
            // 
            // RenameClipsBtn
            // 
            RenameClipsBtn.Location = new Point(8, 112);
            RenameClipsBtn.Margin = new Padding(2);
            RenameClipsBtn.Name = "RenameClipsBtn";
            RenameClipsBtn.Size = new Size(102, 20);
            RenameClipsBtn.TabIndex = 10;
            RenameClipsBtn.Text = "Rename Clips";
            RenameClipsBtn.UseVisualStyleBackColor = true;
            RenameClipsBtn.Click += RenameClipsBtn_Click;
            // 
            // PlaylistsBtn
            // 
            PlaylistsBtn.Location = new Point(8, 136);
            PlaylistsBtn.Margin = new Padding(2);
            PlaylistsBtn.Name = "PlaylistsBtn";
            PlaylistsBtn.Size = new Size(102, 20);
            PlaylistsBtn.TabIndex = 12;
            PlaylistsBtn.Text = "Playlist Creator";
            PlaylistsBtn.UseVisualStyleBackColor = true;
            PlaylistsBtn.Click += PlaylistsBtn_Click;
            // 
            // FileRenamerBtn
            // 
            FileRenamerBtn.Location = new Point(8, 160);
            FileRenamerBtn.Margin = new Padding(2);
            FileRenamerBtn.Name = "FileRenamerBtn";
            FileRenamerBtn.Size = new Size(102, 20);
            FileRenamerBtn.TabIndex = 14;
            FileRenamerBtn.Text = "File Renamer";
            FileRenamerBtn.UseVisualStyleBackColor = true;
            FileRenamerBtn.Click += FileRenamerBtn_Click;
            // 
            // BackupBtn
            // 
            BackupBtn.Location = new Point(91, 55);
            BackupBtn.Margin = new Padding(2);
            BackupBtn.Name = "BackupBtn";
            BackupBtn.Size = new Size(78, 20);
            BackupBtn.TabIndex = 15;
            BackupBtn.Text = "Backup";
            BackupBtn.UseVisualStyleBackColor = true;
            BackupBtn.Click += BackupBtn_Click;
            // 
            // AttachmentsBtn
            // 
            AttachmentsBtn.Location = new Point(142, 112);
            AttachmentsBtn.Margin = new Padding(2);
            AttachmentsBtn.Name = "AttachmentsBtn";
            AttachmentsBtn.Size = new Size(102, 20);
            AttachmentsBtn.TabIndex = 16;
            AttachmentsBtn.Text = "Attachments";
            AttachmentsBtn.UseVisualStyleBackColor = true;
            AttachmentsBtn.Click += AttachmentsBtn_Click;
            // 
            // CollectionsBtn
            // 
            CollectionsBtn.Location = new Point(142, 136);
            CollectionsBtn.Margin = new Padding(2);
            CollectionsBtn.Name = "CollectionsBtn";
            CollectionsBtn.Size = new Size(102, 20);
            CollectionsBtn.TabIndex = 17;
            CollectionsBtn.Text = "Collections";
            CollectionsBtn.UseVisualStyleBackColor = true;
            CollectionsBtn.Click += CollectionsBtn_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(309, 247);
            Controls.Add(CollectionsBtn);
            Controls.Add(AttachmentsBtn);
            Controls.Add(BackupBtn);
            Controls.Add(FileRenamerBtn);
            Controls.Add(PlaylistsBtn);
            Controls.Add(RenameClipsBtn);
            Controls.Add(LoadDiscBtn);
            Controls.Add(LoadedDiscLabel);
            Controls.Add(label1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private ToolTip toolTip1;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem testToolStripMenuItem;
        private ToolStripMenuItem viewInfoToolStripMenuItem;
		private Label label1;
		private Label LoadedDiscLabel;
		private Button LoadDiscBtn;
		private Button RenameClipsBtn;
		private Button PlaylistsBtn;
		private Button FileRenamerBtn;
		private ToolStripMenuItem fileToolStripMenuItem;
		private ToolStripMenuItem saveToolStripMenuItem1;
		private ToolStripMenuItem loadToolStripMenuItem;
		private Button BackupBtn;
        private Button AttachmentsBtn;
        private Button CollectionsBtn;
    }
}
