﻿namespace MakeMKV_Title_Decoder {
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
			ExportButton = new Button();
			menuStrip1.SuspendLayout();
			SuspendLayout();
			// 
			// menuStrip1
			// 
			menuStrip1.ImageScalingSize = new Size(24, 24);
			menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, viewInfoToolStripMenuItem, testToolStripMenuItem });
			menuStrip1.Location = new Point(0, 0);
			menuStrip1.Name = "menuStrip1";
			menuStrip1.Padding = new Padding(9, 3, 0, 3);
			menuStrip1.Size = new Size(441, 35);
			menuStrip1.TabIndex = 6;
			menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { saveToolStripMenuItem1, loadToolStripMenuItem });
			fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			fileToolStripMenuItem.Size = new Size(54, 29);
			fileToolStripMenuItem.Text = "File";
			// 
			// saveToolStripMenuItem1
			// 
			saveToolStripMenuItem1.Name = "saveToolStripMenuItem1";
			saveToolStripMenuItem1.Size = new Size(153, 34);
			saveToolStripMenuItem1.Text = "Save";
			saveToolStripMenuItem1.Click += saveToolStripMenuItem1_Click;
			// 
			// loadToolStripMenuItem
			// 
			loadToolStripMenuItem.Name = "loadToolStripMenuItem";
			loadToolStripMenuItem.Size = new Size(153, 34);
			loadToolStripMenuItem.Text = "Load";
			loadToolStripMenuItem.Click += loadToolStripMenuItem_Click;
			// 
			// viewInfoToolStripMenuItem
			// 
			viewInfoToolStripMenuItem.Name = "viewInfoToolStripMenuItem";
			viewInfoToolStripMenuItem.Size = new Size(102, 29);
			viewInfoToolStripMenuItem.Text = "View Info";
			viewInfoToolStripMenuItem.Click += viewInfoToolStripMenuItem_Click;
			// 
			// testToolStripMenuItem
			// 
			testToolStripMenuItem.Name = "testToolStripMenuItem";
			testToolStripMenuItem.Size = new Size(58, 29);
			testToolStripMenuItem.Text = "Test";
			testToolStripMenuItem.Click += testToolStripMenuItem_Click;
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new Point(12, 63);
			label1.Name = "label1";
			label1.Size = new Size(113, 25);
			label1.TabIndex = 7;
			label1.Text = "Loaded Disc:";
			// 
			// LoadedDiscLabel
			// 
			LoadedDiscLabel.AutoSize = true;
			LoadedDiscLabel.Location = new Point(131, 63);
			LoadedDiscLabel.Name = "LoadedDiscLabel";
			LoadedDiscLabel.Size = new Size(59, 25);
			LoadedDiscLabel.TabIndex = 8;
			LoadedDiscLabel.Text = "label2";
			// 
			// LoadDiscBtn
			// 
			LoadDiscBtn.Location = new Point(12, 91);
			LoadDiscBtn.Name = "LoadDiscBtn";
			LoadDiscBtn.Size = new Size(112, 34);
			LoadDiscBtn.TabIndex = 9;
			LoadDiscBtn.Text = "Load";
			LoadDiscBtn.UseVisualStyleBackColor = true;
			LoadDiscBtn.Click += LoadDiscBtn_Click;
			// 
			// RenameClipsBtn
			// 
			RenameClipsBtn.Location = new Point(12, 186);
			RenameClipsBtn.Name = "RenameClipsBtn";
			RenameClipsBtn.Size = new Size(146, 34);
			RenameClipsBtn.TabIndex = 10;
			RenameClipsBtn.Text = "Rename Clips";
			RenameClipsBtn.UseVisualStyleBackColor = true;
			RenameClipsBtn.Click += RenameClipsBtn_Click;
			// 
			// PlaylistsBtn
			// 
			PlaylistsBtn.Location = new Point(12, 226);
			PlaylistsBtn.Name = "PlaylistsBtn";
			PlaylistsBtn.Size = new Size(145, 34);
			PlaylistsBtn.TabIndex = 12;
			PlaylistsBtn.Text = "Playlist Creator";
			PlaylistsBtn.UseVisualStyleBackColor = true;
			PlaylistsBtn.Click += PlaylistsBtn_Click;
			// 
			// FileRenamerBtn
			// 
			FileRenamerBtn.Location = new Point(12, 266);
			FileRenamerBtn.Name = "FileRenamerBtn";
			FileRenamerBtn.Size = new Size(146, 34);
			FileRenamerBtn.TabIndex = 14;
			FileRenamerBtn.Text = "File Renamer";
			FileRenamerBtn.UseVisualStyleBackColor = true;
			FileRenamerBtn.Click += FileRenamerBtn_Click;
			// 
			// BackupBtn
			// 
			BackupBtn.Location = new Point(130, 91);
			BackupBtn.Name = "BackupBtn";
			BackupBtn.Size = new Size(112, 34);
			BackupBtn.TabIndex = 15;
			BackupBtn.Text = "Backup";
			BackupBtn.UseVisualStyleBackColor = true;
			BackupBtn.Click += BackupBtn_Click;
			// 
			// ExportButton
			// 
			ExportButton.Location = new Point(12, 356);
			ExportButton.Name = "ExportButton";
			ExportButton.Size = new Size(146, 34);
			ExportButton.TabIndex = 16;
			ExportButton.Text = "Export";
			ExportButton.UseVisualStyleBackColor = true;
			ExportButton.Click += ExportButton_Click;
			// 
			// Form1
			// 
			AutoScaleDimensions = new SizeF(10F, 25F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(441, 412);
			Controls.Add(ExportButton);
			Controls.Add(BackupBtn);
			Controls.Add(FileRenamerBtn);
			Controls.Add(PlaylistsBtn);
			Controls.Add(RenameClipsBtn);
			Controls.Add(LoadDiscBtn);
			Controls.Add(LoadedDiscLabel);
			Controls.Add(label1);
			Controls.Add(menuStrip1);
			MainMenuStrip = menuStrip1;
			Margin = new Padding(4, 5, 4, 5);
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
		private Button ExportButton;
	}
}
