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
            menuStrip1 = new MenuStrip();
            viewInfoToolStripMenuItem = new ToolStripMenuItem();
            testToolStripMenuItem = new ToolStripMenuItem();
            renameToolStripMenuItem = new ToolStripMenuItem();
            loadDiscToolStripMenuItem1 = new ToolStripMenuItem();
            clipRenamerToolStripMenuItem1 = new ToolStripMenuItem();
            renamesToolStripMenuItem = new ToolStripMenuItem();
            saveToolStripMenuItem = new ToolStripMenuItem();
            loadToolStripMenuItem1 = new ToolStripMenuItem();
            playlistCreatorToolStripMenuItem = new ToolStripMenuItem();
            fileRenamerToolStripMenuItem = new ToolStripMenuItem();
            backupToolStripMenuItem = new ToolStripMenuItem();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { renameToolStripMenuItem, viewInfoToolStripMenuItem, testToolStripMenuItem, backupToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(829, 24);
            menuStrip1.TabIndex = 6;
            menuStrip1.Text = "menuStrip1";
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
            renameToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { loadDiscToolStripMenuItem1, clipRenamerToolStripMenuItem1, renamesToolStripMenuItem, playlistCreatorToolStripMenuItem, fileRenamerToolStripMenuItem });
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
            // clipRenamerToolStripMenuItem1
            // 
            clipRenamerToolStripMenuItem1.Name = "clipRenamerToolStripMenuItem1";
            clipRenamerToolStripMenuItem1.Size = new Size(180, 22);
            clipRenamerToolStripMenuItem1.Text = "Clip Renamer";
            clipRenamerToolStripMenuItem1.Click += clipRenamerToolStripMenuItem1_Click;
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
            saveToolStripMenuItem.Size = new Size(100, 22);
            saveToolStripMenuItem.Text = "Save";
            saveToolStripMenuItem.Click += saveToolStripMenuItem_Click;
            // 
            // loadToolStripMenuItem1
            // 
            loadToolStripMenuItem1.Name = "loadToolStripMenuItem1";
            loadToolStripMenuItem1.Size = new Size(100, 22);
            loadToolStripMenuItem1.Text = "Load";
            loadToolStripMenuItem1.Click += loadToolStripMenuItem1_Click;
            // 
            // playlistCreatorToolStripMenuItem
            // 
            playlistCreatorToolStripMenuItem.Name = "playlistCreatorToolStripMenuItem";
            playlistCreatorToolStripMenuItem.Size = new Size(180, 22);
            playlistCreatorToolStripMenuItem.Text = "Playlist Creator";
            playlistCreatorToolStripMenuItem.Click += playlistCreatorToolStripMenuItem_Click;
            // 
            // fileRenamerToolStripMenuItem
            // 
            fileRenamerToolStripMenuItem.Name = "fileRenamerToolStripMenuItem";
            fileRenamerToolStripMenuItem.Size = new Size(180, 22);
            fileRenamerToolStripMenuItem.Text = "File Renamer";
            fileRenamerToolStripMenuItem.Click += fileRenamerToolStripMenuItem_Click;
            // 
            // backupToolStripMenuItem
            // 
            backupToolStripMenuItem.Name = "backupToolStripMenuItem";
            backupToolStripMenuItem.Size = new Size(58, 20);
            backupToolStripMenuItem.Text = "Backup";
            backupToolStripMenuItem.Click += backupToolStripMenuItem_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(829, 539);
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
        private ToolStripMenuItem renameToolStripMenuItem;
        private ToolStripMenuItem loadDiscToolStripMenuItem1;
        private ToolStripMenuItem clipRenamerToolStripMenuItem1;
        private ToolStripMenuItem renamesToolStripMenuItem;
        private ToolStripMenuItem saveToolStripMenuItem;
        private ToolStripMenuItem loadToolStripMenuItem1;
        private ToolStripMenuItem playlistCreatorToolStripMenuItem;
        private ToolStripMenuItem fileRenamerToolStripMenuItem;
        private ToolStripMenuItem backupToolStripMenuItem;
    }
}
