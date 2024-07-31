namespace MakeMKV_Title_Decoder {
    partial class VideoComparer {
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
            components = new System.ComponentModel.Container();
            VideoViewVLC1 = new LibVLCSharp.WinForms.VideoView();
            VideoViewVLC2 = new LibVLCSharp.WinForms.VideoView();
            VideoScrubTrackBar = new TrackBar();
            PauseBtn = new Button();
            PlayBtn = new Button();
            label1 = new Label();
            VolumeTrackBar = new TrackBar();
            titleInfo1 = new TitleInfo();
            titleInfo2 = new TitleInfo();
            ReloadBtn = new Button();
            timer1 = new System.Windows.Forms.Timer(components);
            label2 = new Label();
            VolumeTrackBar2 = new TrackBar();
            LeftBtn = new Button();
            RightBtn = new Button();
            label3 = new Label();
            EpisodeNameLabel = new Label();
            CurrentTimeLabelLeft = new Label();
            TotalTimeLabelLeft = new Label();
            CurrentTimeLabelRight = new Label();
            TotalTimeLabelRight = new Label();
            ((System.ComponentModel.ISupportInitialize)VideoViewVLC1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)VideoViewVLC2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)VideoScrubTrackBar).BeginInit();
            ((System.ComponentModel.ISupportInitialize)VolumeTrackBar).BeginInit();
            ((System.ComponentModel.ISupportInitialize)VolumeTrackBar2).BeginInit();
            SuspendLayout();
            // 
            // VideoViewVLC1
            // 
            VideoViewVLC1.BackColor = Color.Black;
            VideoViewVLC1.Location = new Point(15, 43);
            VideoViewVLC1.MediaPlayer = null;
            VideoViewVLC1.Name = "VideoViewVLC1";
            VideoViewVLC1.Size = new Size(413, 266);
            VideoViewVLC1.TabIndex = 0;
            VideoViewVLC1.Text = "videoView1";
            // 
            // VideoViewVLC2
            // 
            VideoViewVLC2.BackColor = Color.Black;
            VideoViewVLC2.Location = new Point(434, 43);
            VideoViewVLC2.MediaPlayer = null;
            VideoViewVLC2.Name = "VideoViewVLC2";
            VideoViewVLC2.Size = new Size(415, 266);
            VideoViewVLC2.TabIndex = 1;
            VideoViewVLC2.Text = "videoView2";
            // 
            // VideoScrubTrackBar
            // 
            VideoScrubTrackBar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            VideoScrubTrackBar.Location = new Point(15, 315);
            VideoScrubTrackBar.Maximum = 1000;
            VideoScrubTrackBar.Name = "VideoScrubTrackBar";
            VideoScrubTrackBar.Size = new Size(834, 45);
            VideoScrubTrackBar.TabIndex = 14;
            VideoScrubTrackBar.TickStyle = TickStyle.None;
            VideoScrubTrackBar.Scroll += VideoSrubTrackBar_Scroll;
            // 
            // PauseBtn
            // 
            PauseBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            PauseBtn.Location = new Point(434, 372);
            PauseBtn.Name = "PauseBtn";
            PauseBtn.Size = new Size(75, 23);
            PauseBtn.TabIndex = 19;
            PauseBtn.Text = "Pause";
            PauseBtn.UseVisualStyleBackColor = true;
            PauseBtn.Click += PauseBtn_Click;
            // 
            // PlayBtn
            // 
            PlayBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            PlayBtn.Location = new Point(351, 372);
            PlayBtn.Name = "PlayBtn";
            PlayBtn.Size = new Size(77, 23);
            PlayBtn.TabIndex = 18;
            PlayBtn.Text = "Play";
            PlayBtn.UseVisualStyleBackColor = true;
            PlayBtn.Click += PlayBtn_Click;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label1.AutoSize = true;
            label1.Location = new Point(13, 521);
            label1.Name = "label1";
            label1.Size = new Size(47, 15);
            label1.TabIndex = 21;
            label1.Text = "Volume";
            // 
            // VolumeTrackBar
            // 
            VolumeTrackBar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            VolumeTrackBar.LargeChange = 10;
            VolumeTrackBar.Location = new Point(15, 414);
            VolumeTrackBar.Maximum = 100;
            VolumeTrackBar.Name = "VolumeTrackBar";
            VolumeTrackBar.Orientation = Orientation.Vertical;
            VolumeTrackBar.Size = new Size(45, 104);
            VolumeTrackBar.TabIndex = 20;
            VolumeTrackBar.TickFrequency = 10;
            VolumeTrackBar.Value = 50;
            VolumeTrackBar.Scroll += VolumeTrackBar_Scroll;
            // 
            // titleInfo1
            // 
            titleInfo1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            titleInfo1.Location = new Point(66, 414);
            titleInfo1.Name = "titleInfo1";
            titleInfo1.Size = new Size(360, 112);
            titleInfo1.TabIndex = 22;
            // 
            // titleInfo2
            // 
            titleInfo2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            titleInfo2.Location = new Point(487, 414);
            titleInfo2.Name = "titleInfo2";
            titleInfo2.Size = new Size(371, 112);
            titleInfo2.TabIndex = 23;
            // 
            // ReloadBtn
            // 
            ReloadBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            ReloadBtn.Location = new Point(584, 372);
            ReloadBtn.Name = "ReloadBtn";
            ReloadBtn.Size = new Size(75, 23);
            ReloadBtn.TabIndex = 24;
            ReloadBtn.Text = "Reload";
            ReloadBtn.UseVisualStyleBackColor = true;
            ReloadBtn.Click += ReloadBtn_Click;
            // 
            // timer1
            // 
            timer1.Enabled = true;
            timer1.Interval = 500;
            timer1.Tick += timer1_Tick;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label2.AutoSize = true;
            label2.Location = new Point(432, 521);
            label2.Name = "label2";
            label2.Size = new Size(47, 15);
            label2.TabIndex = 26;
            label2.Text = "Volume";
            // 
            // VolumeTrackBar2
            // 
            VolumeTrackBar2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            VolumeTrackBar2.LargeChange = 10;
            VolumeTrackBar2.Location = new Point(434, 414);
            VolumeTrackBar2.Maximum = 100;
            VolumeTrackBar2.Name = "VolumeTrackBar2";
            VolumeTrackBar2.Orientation = Orientation.Vertical;
            VolumeTrackBar2.Size = new Size(45, 104);
            VolumeTrackBar2.TabIndex = 25;
            VolumeTrackBar2.TickFrequency = 10;
            VolumeTrackBar2.Value = 50;
            VolumeTrackBar2.Scroll += VolumeTrackBar2_Scroll;
            // 
            // LeftBtn
            // 
            LeftBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            LeftBtn.Location = new Point(351, 564);
            LeftBtn.Name = "LeftBtn";
            LeftBtn.Size = new Size(77, 23);
            LeftBtn.TabIndex = 27;
            LeftBtn.Text = "Prefer Left";
            LeftBtn.UseVisualStyleBackColor = true;
            LeftBtn.Click += LeftBtn_Click;
            // 
            // RightBtn
            // 
            RightBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            RightBtn.Location = new Point(434, 564);
            RightBtn.Name = "RightBtn";
            RightBtn.Size = new Size(77, 23);
            RightBtn.TabIndex = 28;
            RightBtn.Text = "Prefer Right";
            RightBtn.UseVisualStyleBackColor = true;
            RightBtn.Click += RightBtn_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(15, 9);
            label3.Name = "label3";
            label3.Size = new Size(84, 15);
            label3.TabIndex = 29;
            label3.Text = "Episode name:";
            // 
            // EpisodeNameLabel
            // 
            EpisodeNameLabel.AutoSize = true;
            EpisodeNameLabel.Location = new Point(105, 9);
            EpisodeNameLabel.Name = "EpisodeNameLabel";
            EpisodeNameLabel.Size = new Size(29, 15);
            EpisodeNameLabel.TabIndex = 30;
            EpisodeNameLabel.Text = "N/A";
            // 
            // CurrentTimeLabelLeft
            // 
            CurrentTimeLabelLeft.AutoSize = true;
            CurrentTimeLabelLeft.Location = new Point(15, 345);
            CurrentTimeLabelLeft.Name = "CurrentTimeLabelLeft";
            CurrentTimeLabelLeft.Size = new Size(49, 15);
            CurrentTimeLabelLeft.TabIndex = 31;
            CurrentTimeLabelLeft.Text = "00:00:00";
            // 
            // TotalTimeLabelLeft
            // 
            TotalTimeLabelLeft.AutoSize = true;
            TotalTimeLabelLeft.Location = new Point(379, 345);
            TotalTimeLabelLeft.Name = "TotalTimeLabelLeft";
            TotalTimeLabelLeft.Size = new Size(49, 15);
            TotalTimeLabelLeft.TabIndex = 32;
            TotalTimeLabelLeft.Text = "00:00:00";
            // 
            // CurrentTimeLabelRight
            // 
            CurrentTimeLabelRight.AutoSize = true;
            CurrentTimeLabelRight.Location = new Point(434, 345);
            CurrentTimeLabelRight.Name = "CurrentTimeLabelRight";
            CurrentTimeLabelRight.Size = new Size(49, 15);
            CurrentTimeLabelRight.TabIndex = 33;
            CurrentTimeLabelRight.Text = "00:00:00";
            // 
            // TotalTimeLabelRight
            // 
            TotalTimeLabelRight.AutoSize = true;
            TotalTimeLabelRight.Location = new Point(800, 345);
            TotalTimeLabelRight.Name = "TotalTimeLabelRight";
            TotalTimeLabelRight.Size = new Size(49, 15);
            TotalTimeLabelRight.TabIndex = 34;
            TotalTimeLabelRight.Text = "00:00:00";
            // 
            // VideoComparer
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(870, 599);
            ControlBox = false;
            Controls.Add(TotalTimeLabelRight);
            Controls.Add(CurrentTimeLabelRight);
            Controls.Add(TotalTimeLabelLeft);
            Controls.Add(CurrentTimeLabelLeft);
            Controls.Add(EpisodeNameLabel);
            Controls.Add(label3);
            Controls.Add(RightBtn);
            Controls.Add(LeftBtn);
            Controls.Add(label2);
            Controls.Add(VolumeTrackBar2);
            Controls.Add(ReloadBtn);
            Controls.Add(titleInfo2);
            Controls.Add(titleInfo1);
            Controls.Add(label1);
            Controls.Add(VolumeTrackBar);
            Controls.Add(PauseBtn);
            Controls.Add(PlayBtn);
            Controls.Add(VideoScrubTrackBar);
            Controls.Add(VideoViewVLC2);
            Controls.Add(VideoViewVLC1);
            Name = "VideoComparer";
            Text = "VideoComparer";
            FormClosing += VideoComparer_FormClosing;
            Load += VideoComparer_Load;
            ((System.ComponentModel.ISupportInitialize)VideoViewVLC1).EndInit();
            ((System.ComponentModel.ISupportInitialize)VideoViewVLC2).EndInit();
            ((System.ComponentModel.ISupportInitialize)VideoScrubTrackBar).EndInit();
            ((System.ComponentModel.ISupportInitialize)VolumeTrackBar).EndInit();
            ((System.ComponentModel.ISupportInitialize)VolumeTrackBar2).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private LibVLCSharp.WinForms.VideoView VideoViewVLC1;
        private LibVLCSharp.WinForms.VideoView VideoViewVLC2;
        private TrackBar VideoScrubTrackBar;
        private Button PauseBtn;
        private Button PlayBtn;
        private Label label1;
        private TrackBar VolumeTrackBar;
        private TitleInfo titleInfo1;
        private TitleInfo titleInfo2;
        private Button ReloadBtn;
        private System.Windows.Forms.Timer timer1;
        private Label label2;
        private TrackBar VolumeTrackBar2;
        private Button LeftBtn;
        private Button RightBtn;
        private Label label3;
        private Label EpisodeNameLabel;
        private Label CurrentTimeLabelLeft;
        private Label TotalTimeLabelLeft;
        private Label CurrentTimeLabelRight;
        private Label TotalTimeLabelRight;
    }
}