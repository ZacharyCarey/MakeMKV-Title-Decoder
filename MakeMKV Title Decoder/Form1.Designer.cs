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
            panel1 = new Panel();
            BonusFeaturesRadioBtn = new RadioButton();
            TvRadioBtn = new RadioButton();
            MovieRadioBtn = new RadioButton();
            toolTip1 = new ToolTip(components);
            IgnoreIncompleteCheckBox = new CheckBox();
            CollapseCheckBox = new CheckBox();
            ScrapeBtn = new Button();
            IncludeAttachmentsCheckBox = new CheckBox();
            RenameBtn = new Button();
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            debugDumpToolStripMenuItem = new ToolStripMenuItem();
            identifierToolStripMenuItem = new ToolStripMenuItem();
            saveToolStripMenuItem = new ToolStripMenuItem();
            loadToolStripMenuItem = new ToolStripMenuItem();
            checkAllToolStripMenuItem = new ToolStripMenuItem();
            loadNoInputToolStripMenuItem = new ToolStripMenuItem();
            panel1.SuspendLayout();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(BonusFeaturesRadioBtn);
            panel1.Controls.Add(TvRadioBtn);
            panel1.Controls.Add(MovieRadioBtn);
            panel1.Location = new Point(12, 40);
            panel1.Name = "panel1";
            panel1.Size = new Size(186, 75);
            panel1.TabIndex = 0;
            // 
            // BonusFeaturesRadioBtn
            // 
            BonusFeaturesRadioBtn.AutoSize = true;
            BonusFeaturesRadioBtn.Location = new Point(3, 53);
            BonusFeaturesRadioBtn.Name = "BonusFeaturesRadioBtn";
            BonusFeaturesRadioBtn.Size = new Size(105, 19);
            BonusFeaturesRadioBtn.TabIndex = 2;
            BonusFeaturesRadioBtn.Text = "Bonus Features";
            toolTip1.SetToolTip(BonusFeaturesRadioBtn, "Dedicated bonus features disc. Will separate all tracks instead of looking for a main feature.");
            BonusFeaturesRadioBtn.UseVisualStyleBackColor = true;
            // 
            // TvRadioBtn
            // 
            TvRadioBtn.AutoSize = true;
            TvRadioBtn.Location = new Point(3, 28);
            TvRadioBtn.Name = "TvRadioBtn";
            TvRadioBtn.Size = new Size(38, 19);
            TvRadioBtn.TabIndex = 1;
            TvRadioBtn.Text = "TV";
            toolTip1.SetToolTip(TvRadioBtn, "TV prefers individual episodes for the main feature");
            TvRadioBtn.UseVisualStyleBackColor = true;
            // 
            // MovieRadioBtn
            // 
            MovieRadioBtn.AutoSize = true;
            MovieRadioBtn.Checked = true;
            MovieRadioBtn.Location = new Point(3, 3);
            MovieRadioBtn.Name = "MovieRadioBtn";
            MovieRadioBtn.Size = new Size(58, 19);
            MovieRadioBtn.TabIndex = 0;
            MovieRadioBtn.TabStop = true;
            MovieRadioBtn.Text = "Movie";
            toolTip1.SetToolTip(MovieRadioBtn, "Movies prefer the single playlist for the main feature");
            MovieRadioBtn.UseVisualStyleBackColor = true;
            // 
            // IgnoreIncompleteCheckBox
            // 
            IgnoreIncompleteCheckBox.AutoSize = true;
            IgnoreIncompleteCheckBox.Checked = true;
            IgnoreIncompleteCheckBox.CheckState = CheckState.Checked;
            IgnoreIncompleteCheckBox.Location = new Point(15, 146);
            IgnoreIncompleteCheckBox.Name = "IgnoreIncompleteCheckBox";
            IgnoreIncompleteCheckBox.Size = new Size(155, 19);
            IgnoreIncompleteCheckBox.TabIndex = 4;
            IgnoreIncompleteCheckBox.Text = "Ignore incomplete video";
            toolTip1.SetToolTip(IgnoreIncompleteCheckBox, "If a stream contains only video or only audio it will be ignored");
            IgnoreIncompleteCheckBox.UseVisualStyleBackColor = true;
            // 
            // CollapseCheckBox
            // 
            CollapseCheckBox.AutoSize = true;
            CollapseCheckBox.Checked = true;
            CollapseCheckBox.CheckState = CheckState.Checked;
            CollapseCheckBox.Location = new Point(15, 171);
            CollapseCheckBox.Name = "CollapseCheckBox";
            CollapseCheckBox.Size = new Size(163, 19);
            CollapseCheckBox.TabIndex = 5;
            CollapseCheckBox.Text = "Collapse all when finished";
            toolTip1.SetToolTip(CollapseCheckBox, "When finished scraping, will collapse all titles.");
            CollapseCheckBox.UseVisualStyleBackColor = true;
            // 
            // ScrapeBtn
            // 
            ScrapeBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            ScrapeBtn.Location = new Point(12, 211);
            ScrapeBtn.Name = "ScrapeBtn";
            ScrapeBtn.Size = new Size(75, 23);
            ScrapeBtn.TabIndex = 1;
            ScrapeBtn.Text = "Scrape";
            ScrapeBtn.UseVisualStyleBackColor = true;
            ScrapeBtn.Click += ScrapeBtn_Click;
            // 
            // IncludeAttachmentsCheckBox
            // 
            IncludeAttachmentsCheckBox.AutoSize = true;
            IncludeAttachmentsCheckBox.Checked = true;
            IncludeAttachmentsCheckBox.CheckState = CheckState.Checked;
            IncludeAttachmentsCheckBox.Location = new Point(15, 121);
            IncludeAttachmentsCheckBox.Name = "IncludeAttachmentsCheckBox";
            IncludeAttachmentsCheckBox.Size = new Size(136, 19);
            IncludeAttachmentsCheckBox.TabIndex = 2;
            IncludeAttachmentsCheckBox.Text = "Include Attachments";
            IncludeAttachmentsCheckBox.UseVisualStyleBackColor = true;
            IncludeAttachmentsCheckBox.CheckedChanged += IncludeAttachmentsCheckBox_CheckedChanged;
            // 
            // RenameBtn
            // 
            RenameBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            RenameBtn.Location = new Point(123, 211);
            RenameBtn.Name = "RenameBtn";
            RenameBtn.Size = new Size(75, 23);
            RenameBtn.TabIndex = 3;
            RenameBtn.Text = "Rename";
            RenameBtn.UseVisualStyleBackColor = true;
            RenameBtn.Click += RenameBtn_Click;
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(435, 24);
            menuStrip1.TabIndex = 6;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { debugDumpToolStripMenuItem, identifierToolStripMenuItem, checkAllToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(54, 20);
            fileToolStripMenuItem.Text = "Debug";
            // 
            // debugDumpToolStripMenuItem
            // 
            debugDumpToolStripMenuItem.Name = "debugDumpToolStripMenuItem";
            debugDumpToolStripMenuItem.Size = new Size(180, 22);
            debugDumpToolStripMenuItem.Text = "Scraper Dump";
            debugDumpToolStripMenuItem.Click += debugDumpToolStripMenuItem_Click;
            // 
            // identifierToolStripMenuItem
            // 
            identifierToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { saveToolStripMenuItem, loadToolStripMenuItem, loadNoInputToolStripMenuItem });
            identifierToolStripMenuItem.Name = "identifierToolStripMenuItem";
            identifierToolStripMenuItem.Size = new Size(180, 22);
            identifierToolStripMenuItem.Text = "Scraper";
            // 
            // saveToolStripMenuItem
            // 
            saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            saveToolStripMenuItem.Size = new Size(180, 22);
            saveToolStripMenuItem.Text = "Save";
            saveToolStripMenuItem.Click += saveToolStripMenuItem_Click;
            // 
            // loadToolStripMenuItem
            // 
            loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            loadToolStripMenuItem.Size = new Size(180, 22);
            loadToolStripMenuItem.Text = "Load";
            loadToolStripMenuItem.Click += loadToolStripMenuItem_Click;
            // 
            // checkAllToolStripMenuItem
            // 
            checkAllToolStripMenuItem.Name = "checkAllToolStripMenuItem";
            checkAllToolStripMenuItem.Size = new Size(180, 22);
            checkAllToolStripMenuItem.Text = "Check All Titles";
            checkAllToolStripMenuItem.Click += checkAllToolStripMenuItem_Click;
            // 
            // loadNoInputToolStripMenuItem
            // 
            loadNoInputToolStripMenuItem.Name = "loadNoInputToolStripMenuItem";
            loadNoInputToolStripMenuItem.Size = new Size(180, 22);
            loadNoInputToolStripMenuItem.Text = "Load - No Input";
            loadNoInputToolStripMenuItem.Click += loadNoInputToolStripMenuItem_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(435, 246);
            Controls.Add(CollapseCheckBox);
            Controls.Add(IgnoreIncompleteCheckBox);
            Controls.Add(RenameBtn);
            Controls.Add(IncludeAttachmentsCheckBox);
            Controls.Add(ScrapeBtn);
            Controls.Add(panel1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel panel1;
        private RadioButton TvRadioBtn;
        private RadioButton MovieRadioBtn;
        private ToolTip toolTip1;
        private Button ScrapeBtn;
        private RadioButton BonusFeaturesRadioBtn;
        private CheckBox IncludeAttachmentsCheckBox;
        private Button RenameBtn;
        private CheckBox IgnoreIncompleteCheckBox;
        private CheckBox CollapseCheckBox;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem debugDumpToolStripMenuItem;
        private ToolStripMenuItem identifierToolStripMenuItem;
        private ToolStripMenuItem saveToolStripMenuItem;
        private ToolStripMenuItem loadToolStripMenuItem;
        private ToolStripMenuItem checkAllToolStripMenuItem;
        private ToolStripMenuItem loadNoInputToolStripMenuItem;
    }
}
