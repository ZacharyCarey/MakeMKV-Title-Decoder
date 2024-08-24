namespace MakeMKV_Title_Decoder {
    partial class ViewInfo {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            label1 = new Label();
            SourceFileLabel = new Label();
            label2 = new Label();
            TitleList = new ComboBox();
            label3 = new Label();
            comboBox2 = new ComboBox();
            label4 = new Label();
            ChaptersLabel = new Label();
            label5 = new Label();
            DurationLabel = new Label();
            tabControl1 = new TabControl();
            SummaryTab = new TabPage();
            videoView1 = new LibVLCSharp.WinForms.VideoView();
            DimensionsTab = new TabPage();
            AspectRatioLabel = new Label();
            label9 = new Label();
            DisplaySizeLabel = new Label();
            label8 = new Label();
            label6 = new Label();
            FiltersTab = new TabPage();
            VideoTab = new TabPage();
            HDRLabel = new Label();
            FPSLabel = new Label();
            AudioTab = new TabPage();
            AudioList = new ListView();
            SourceColumnHeader1 = new ColumnHeader();
            CodeccolumnHeader1 = new ColumnHeader();
            QualitycolumnHeader1 = new ColumnHeader();
            ChannelsColumn = new ColumnHeader();
            SamplecolumnHeader1 = new ColumnHeader();
            SubtitlesTab = new TabPage();
            SubtitlesList = new ListView();
            columnHeader1 = new ColumnHeader();
            columnHeader2 = new ColumnHeader();
            columnHeader3 = new ColumnHeader();
            columnHeader4 = new ColumnHeader();
            ChaptersTab = new TabPage();
            ChaptersList = new ListView();
            columnHeader5 = new ColumnHeader();
            columnHeader6 = new ColumnHeader();
            columnHeader7 = new ColumnHeader();
            tabControl1.SuspendLayout();
            SummaryTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)videoView1).BeginInit();
            DimensionsTab.SuspendLayout();
            VideoTab.SuspendLayout();
            AudioTab.SuspendLayout();
            SubtitlesTab.SuspendLayout();
            ChaptersTab.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(49, 15);
            label1.TabIndex = 0;
            label1.Text = "Source: ";
            // 
            // SourceFileLabel
            // 
            SourceFileLabel.AutoSize = true;
            SourceFileLabel.Location = new Point(67, 9);
            SourceFileLabel.Name = "SourceFileLabel";
            SourceFileLabel.Size = new Size(37, 15);
            SourceFileLabel.TabIndex = 1;
            SourceFileLabel.Text = "00000";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 41);
            label2.Name = "label2";
            label2.Size = new Size(32, 15);
            label2.TabIndex = 3;
            label2.Text = "Title:";
            // 
            // TitleList
            // 
            TitleList.FormattingEnabled = true;
            TitleList.Location = new Point(50, 38);
            TitleList.Name = "TitleList";
            TitleList.Size = new Size(206, 23);
            TitleList.TabIndex = 4;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(275, 41);
            label3.Name = "label3";
            label3.Size = new Size(41, 15);
            label3.TabIndex = 5;
            label3.Text = "Angle:";
            // 
            // comboBox2
            // 
            comboBox2.FormattingEnabled = true;
            comboBox2.Location = new Point(322, 38);
            comboBox2.Name = "comboBox2";
            comboBox2.Size = new Size(72, 23);
            comboBox2.TabIndex = 6;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(416, 41);
            label4.Name = "label4";
            label4.Size = new Size(57, 15);
            label4.TabIndex = 7;
            label4.Text = "Chapters:";
            // 
            // ChaptersLabel
            // 
            ChaptersLabel.AutoSize = true;
            ChaptersLabel.Location = new Point(479, 41);
            ChaptersLabel.Name = "ChaptersLabel";
            ChaptersLabel.Size = new Size(38, 15);
            ChaptersLabel.TabIndex = 8;
            ChaptersLabel.Text = "label5";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(579, 41);
            label5.Name = "label5";
            label5.Size = new Size(56, 15);
            label5.TabIndex = 9;
            label5.Text = "Duration:";
            // 
            // DurationLabel
            // 
            DurationLabel.AutoSize = true;
            DurationLabel.Location = new Point(641, 41);
            DurationLabel.Name = "DurationLabel";
            DurationLabel.Size = new Size(38, 15);
            DurationLabel.TabIndex = 10;
            DurationLabel.Text = "label6";
            // 
            // tabControl1
            // 
            tabControl1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabControl1.Controls.Add(SummaryTab);
            tabControl1.Controls.Add(DimensionsTab);
            tabControl1.Controls.Add(FiltersTab);
            tabControl1.Controls.Add(VideoTab);
            tabControl1.Controls.Add(AudioTab);
            tabControl1.Controls.Add(SubtitlesTab);
            tabControl1.Controls.Add(ChaptersTab);
            tabControl1.Location = new Point(12, 67);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(870, 468);
            tabControl1.TabIndex = 11;
            // 
            // SummaryTab
            // 
            SummaryTab.Controls.Add(videoView1);
            SummaryTab.Location = new Point(4, 24);
            SummaryTab.Name = "SummaryTab";
            SummaryTab.Padding = new Padding(3);
            SummaryTab.Size = new Size(862, 440);
            SummaryTab.TabIndex = 0;
            SummaryTab.Text = "Summary";
            SummaryTab.UseVisualStyleBackColor = true;
            // 
            // videoView1
            // 
            videoView1.BackColor = Color.Black;
            videoView1.Dock = DockStyle.Fill;
            videoView1.Location = new Point(3, 3);
            videoView1.MediaPlayer = null;
            videoView1.Name = "videoView1";
            videoView1.Size = new Size(856, 434);
            videoView1.TabIndex = 0;
            videoView1.Text = "videoView1";
            // 
            // DimensionsTab
            // 
            DimensionsTab.Controls.Add(AspectRatioLabel);
            DimensionsTab.Controls.Add(label9);
            DimensionsTab.Controls.Add(DisplaySizeLabel);
            DimensionsTab.Controls.Add(label8);
            DimensionsTab.Controls.Add(label6);
            DimensionsTab.Location = new Point(4, 24);
            DimensionsTab.Name = "DimensionsTab";
            DimensionsTab.Padding = new Padding(3);
            DimensionsTab.Size = new Size(862, 440);
            DimensionsTab.TabIndex = 1;
            DimensionsTab.Text = "Dimensions";
            DimensionsTab.UseVisualStyleBackColor = true;
            // 
            // AspectRatioLabel
            // 
            AspectRatioLabel.AutoSize = true;
            AspectRatioLabel.Location = new Point(248, 18);
            AspectRatioLabel.Name = "AspectRatioLabel";
            AspectRatioLabel.Size = new Size(38, 15);
            AspectRatioLabel.TabIndex = 6;
            AspectRatioLabel.Text = "label8";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(166, 18);
            label9.Name = "label9";
            label9.Size = new Size(76, 15);
            label9.TabIndex = 5;
            label9.Text = "Aspect Ratio:";
            // 
            // DisplaySizeLabel
            // 
            DisplaySizeLabel.AutoSize = true;
            DisplaySizeLabel.Location = new Point(84, 18);
            DisplaySizeLabel.Name = "DisplaySizeLabel";
            DisplaySizeLabel.Size = new Size(38, 15);
            DisplaySizeLabel.TabIndex = 4;
            DisplaySizeLabel.Text = "label8";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(7, 18);
            label8.Name = "label8";
            label8.Size = new Size(71, 15);
            label8.TabIndex = 3;
            label8.Text = "Display Size:";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label6.Location = new Point(7, 3);
            label6.Name = "label6";
            label6.Size = new Size(116, 15);
            label6.TabIndex = 0;
            label6.Text = "Source Dimensions:";
            // 
            // FiltersTab
            // 
            FiltersTab.Location = new Point(4, 24);
            FiltersTab.Name = "FiltersTab";
            FiltersTab.Padding = new Padding(3);
            FiltersTab.Size = new Size(862, 440);
            FiltersTab.TabIndex = 2;
            FiltersTab.Text = "Filters";
            FiltersTab.UseVisualStyleBackColor = true;
            // 
            // VideoTab
            // 
            VideoTab.Controls.Add(HDRLabel);
            VideoTab.Controls.Add(FPSLabel);
            VideoTab.Location = new Point(4, 24);
            VideoTab.Name = "VideoTab";
            VideoTab.Padding = new Padding(3);
            VideoTab.Size = new Size(862, 440);
            VideoTab.TabIndex = 3;
            VideoTab.Text = "Video";
            VideoTab.UseVisualStyleBackColor = true;
            // 
            // HDRLabel
            // 
            HDRLabel.AutoSize = true;
            HDRLabel.Location = new Point(9, 29);
            HDRLabel.Name = "HDRLabel";
            HDRLabel.Size = new Size(43, 15);
            HDRLabel.TabIndex = 1;
            HDRLabel.Text = "HDR10";
            // 
            // FPSLabel
            // 
            FPSLabel.AutoSize = true;
            FPSLabel.Location = new Point(6, 3);
            FPSLabel.Name = "FPSLabel";
            FPSLabel.Size = new Size(47, 15);
            FPSLabel.TabIndex = 0;
            FPSLabel.Text = "120 FPS";
            // 
            // AudioTab
            // 
            AudioTab.Controls.Add(AudioList);
            AudioTab.Location = new Point(4, 24);
            AudioTab.Name = "AudioTab";
            AudioTab.Padding = new Padding(3);
            AudioTab.Size = new Size(862, 440);
            AudioTab.TabIndex = 4;
            AudioTab.Text = "Audio";
            AudioTab.UseVisualStyleBackColor = true;
            // 
            // AudioList
            // 
            AudioList.Columns.AddRange(new ColumnHeader[] { SourceColumnHeader1, CodeccolumnHeader1, QualitycolumnHeader1, ChannelsColumn, SamplecolumnHeader1 });
            AudioList.Dock = DockStyle.Fill;
            AudioList.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            AudioList.Location = new Point(3, 3);
            AudioList.MultiSelect = false;
            AudioList.Name = "AudioList";
            AudioList.Size = new Size(856, 434);
            AudioList.TabIndex = 0;
            AudioList.UseCompatibleStateImageBehavior = false;
            AudioList.View = View.Details;
            // 
            // SourceColumnHeader1
            // 
            SourceColumnHeader1.Text = "Source";
            // 
            // CodeccolumnHeader1
            // 
            CodeccolumnHeader1.Text = "Codec";
            // 
            // QualitycolumnHeader1
            // 
            QualitycolumnHeader1.Text = "Quality";
            // 
            // ChannelsColumn
            // 
            ChannelsColumn.Text = "Channels";
            // 
            // SamplecolumnHeader1
            // 
            SamplecolumnHeader1.Text = "Sample";
            // 
            // SubtitlesTab
            // 
            SubtitlesTab.Controls.Add(SubtitlesList);
            SubtitlesTab.Location = new Point(4, 24);
            SubtitlesTab.Name = "SubtitlesTab";
            SubtitlesTab.Size = new Size(862, 440);
            SubtitlesTab.TabIndex = 5;
            SubtitlesTab.Text = "Subtitles";
            SubtitlesTab.UseVisualStyleBackColor = true;
            // 
            // SubtitlesList
            // 
            SubtitlesList.Columns.AddRange(new ColumnHeader[] { columnHeader1, columnHeader2, columnHeader3, columnHeader4 });
            SubtitlesList.Dock = DockStyle.Fill;
            SubtitlesList.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            SubtitlesList.Location = new Point(0, 0);
            SubtitlesList.MultiSelect = false;
            SubtitlesList.Name = "SubtitlesList";
            SubtitlesList.Size = new Size(862, 440);
            SubtitlesList.TabIndex = 1;
            SubtitlesList.UseCompatibleStateImageBehavior = false;
            SubtitlesList.View = View.Details;
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "Source";
            // 
            // columnHeader2
            // 
            columnHeader2.Text = "Forced Only";
            columnHeader2.Width = 80;
            // 
            // columnHeader3
            // 
            columnHeader3.Text = "Burn In";
            // 
            // columnHeader4
            // 
            columnHeader4.Text = "Default";
            // 
            // ChaptersTab
            // 
            ChaptersTab.Controls.Add(ChaptersList);
            ChaptersTab.Location = new Point(4, 24);
            ChaptersTab.Name = "ChaptersTab";
            ChaptersTab.Size = new Size(862, 440);
            ChaptersTab.TabIndex = 6;
            ChaptersTab.Text = "Chapters";
            ChaptersTab.UseVisualStyleBackColor = true;
            // 
            // ChaptersList
            // 
            ChaptersList.Columns.AddRange(new ColumnHeader[] { columnHeader5, columnHeader6, columnHeader7 });
            ChaptersList.Dock = DockStyle.Fill;
            ChaptersList.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            ChaptersList.Location = new Point(0, 0);
            ChaptersList.MultiSelect = false;
            ChaptersList.Name = "ChaptersList";
            ChaptersList.Size = new Size(862, 440);
            ChaptersList.TabIndex = 2;
            ChaptersList.UseCompatibleStateImageBehavior = false;
            ChaptersList.View = View.Details;
            // 
            // columnHeader5
            // 
            columnHeader5.Text = "Chapter";
            // 
            // columnHeader6
            // 
            columnHeader6.Text = "Duration";
            columnHeader6.Width = 80;
            // 
            // columnHeader7
            // 
            columnHeader7.Text = "Name";
            // 
            // ViewInfo
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(894, 547);
            Controls.Add(tabControl1);
            Controls.Add(DurationLabel);
            Controls.Add(label5);
            Controls.Add(ChaptersLabel);
            Controls.Add(label4);
            Controls.Add(comboBox2);
            Controls.Add(label3);
            Controls.Add(TitleList);
            Controls.Add(label2);
            Controls.Add(SourceFileLabel);
            Controls.Add(label1);
            Name = "ViewInfo";
            Text = "ViewInfo";
            Load += ViewInfo_Load;
            tabControl1.ResumeLayout(false);
            SummaryTab.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)videoView1).EndInit();
            DimensionsTab.ResumeLayout(false);
            DimensionsTab.PerformLayout();
            VideoTab.ResumeLayout(false);
            VideoTab.PerformLayout();
            AudioTab.ResumeLayout(false);
            SubtitlesTab.ResumeLayout(false);
            ChaptersTab.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label SourceFileLabel;
        private Label label2;
        private ComboBox TitleList;
        private Label label3;
        private ComboBox comboBox2;
        private Label label4;
        private Label ChaptersLabel;
        private Label label5;
        private Label DurationLabel;
        private TabControl tabControl1;
        private TabPage SummaryTab;
        private TabPage DimensionsTab;
        private TabPage FiltersTab;
        private TabPage VideoTab;
        private TabPage AudioTab;
        private LibVLCSharp.WinForms.VideoView videoView1;
        private Label label6;
        private TabPage SubtitlesTab;
        private TabPage ChaptersTab;
        private Label AspectRatioLabel;
        private Label label9;
        private Label DisplaySizeLabel;
        private Label label8;
        private Label FPSLabel;
        private Label HDRLabel;
        private ListView AudioList;
        private ColumnHeader SourceColumnHeader1;
        private ColumnHeader CodeccolumnHeader1;
        private ColumnHeader QualitycolumnHeader1;
        private ColumnHeader ChannelsColumn;
        private ColumnHeader SamplecolumnHeader1;
        private ListView SubtitlesList;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
        private ColumnHeader columnHeader3;
        private ColumnHeader columnHeader4;
        private ListView ChaptersList;
        private ColumnHeader columnHeader5;
        private ColumnHeader columnHeader6;
        private ColumnHeader columnHeader7;
    }
}