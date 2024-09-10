namespace MakeMKV_Title_Decoder {
    partial class VideoPlayer {
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            components = new System.ComponentModel.Container();
            TotalTimeLabel = new Label();
            CurrentTimeLabel = new Label();
            VideoScrubTrackBar = new TrackBar();
            VolumeTrackBar = new TrackBar();
            label1 = new Label();
            PlayBtn = new Button();
            PauseBtn = new Button();
            ReloadBtn = new Button();
            FileNotFoundLabel = new Label();
            timer1 = new System.Windows.Forms.Timer(components);
            panel1 = new Panel();
            ((System.ComponentModel.ISupportInitialize)VideoScrubTrackBar).BeginInit();
            ((System.ComponentModel.ISupportInitialize)VolumeTrackBar).BeginInit();
            SuspendLayout();
            // 
            // TotalTimeLabel
            // 
            TotalTimeLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            TotalTimeLabel.AutoSize = true;
            TotalTimeLabel.Location = new Point(789, 361);
            TotalTimeLabel.Name = "TotalTimeLabel";
            TotalTimeLabel.Size = new Size(49, 15);
            TotalTimeLabel.TabIndex = 25;
            TotalTimeLabel.Text = "00:00:00";
            // 
            // CurrentTimeLabel
            // 
            CurrentTimeLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            CurrentTimeLabel.AutoSize = true;
            CurrentTimeLabel.Location = new Point(3, 361);
            CurrentTimeLabel.Name = "CurrentTimeLabel";
            CurrentTimeLabel.Size = new Size(49, 15);
            CurrentTimeLabel.TabIndex = 24;
            CurrentTimeLabel.Text = "00:00:00";
            // 
            // VideoScrubTrackBar
            // 
            VideoScrubTrackBar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            VideoScrubTrackBar.Enabled = false;
            VideoScrubTrackBar.Location = new Point(3, 339);
            VideoScrubTrackBar.Maximum = 1000;
            VideoScrubTrackBar.Name = "VideoScrubTrackBar";
            VideoScrubTrackBar.Size = new Size(838, 45);
            VideoScrubTrackBar.TabIndex = 23;
            VideoScrubTrackBar.TickStyle = TickStyle.None;
            VideoScrubTrackBar.Scroll += VideoScrubTrackBar_Scroll;
            // 
            // VolumeTrackBar
            // 
            VolumeTrackBar.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            VolumeTrackBar.LargeChange = 10;
            VolumeTrackBar.Location = new Point(694, 379);
            VolumeTrackBar.Maximum = 100;
            VolumeTrackBar.Name = "VolumeTrackBar";
            VolumeTrackBar.Size = new Size(144, 45);
            VolumeTrackBar.TabIndex = 26;
            VolumeTrackBar.TickFrequency = 10;
            VolumeTrackBar.Value = 50;
            VolumeTrackBar.Scroll += VolumeTrackBar_Scroll;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            label1.AutoSize = true;
            label1.Location = new Point(641, 387);
            label1.Name = "label1";
            label1.Size = new Size(50, 15);
            label1.TabIndex = 27;
            label1.Text = "Volume:";
            // 
            // PlayBtn
            // 
            PlayBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            PlayBtn.Enabled = false;
            PlayBtn.Location = new Point(3, 383);
            PlayBtn.Name = "PlayBtn";
            PlayBtn.Size = new Size(77, 23);
            PlayBtn.TabIndex = 28;
            PlayBtn.Text = "Play";
            PlayBtn.UseVisualStyleBackColor = true;
            PlayBtn.Click += PlayBtn_Click;
            // 
            // PauseBtn
            // 
            PauseBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            PauseBtn.Enabled = false;
            PauseBtn.Location = new Point(86, 383);
            PauseBtn.Name = "PauseBtn";
            PauseBtn.Size = new Size(75, 23);
            PauseBtn.TabIndex = 29;
            PauseBtn.Text = "Pause";
            PauseBtn.UseVisualStyleBackColor = true;
            PauseBtn.Click += PauseBtn_Click;
            // 
            // ReloadBtn
            // 
            ReloadBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            ReloadBtn.Location = new Point(194, 383);
            ReloadBtn.Name = "ReloadBtn";
            ReloadBtn.Size = new Size(75, 23);
            ReloadBtn.TabIndex = 30;
            ReloadBtn.Text = "Reload";
            ReloadBtn.UseVisualStyleBackColor = true;
            ReloadBtn.Click += ReloadBtn_Click;
            // 
            // FileNotFoundLabel
            // 
            FileNotFoundLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            FileNotFoundLabel.AutoSize = true;
            FileNotFoundLabel.ForeColor = Color.Red;
            FileNotFoundLabel.Location = new Point(86, 361);
            FileNotFoundLabel.Name = "FileNotFoundLabel";
            FileNotFoundLabel.Size = new Size(106, 15);
            FileNotFoundLabel.TabIndex = 31;
            FileNotFoundLabel.Text = "Could not find file:";
            FileNotFoundLabel.Visible = false;
            // 
            // timer1
            // 
            timer1.Interval = 500;
            timer1.Tick += timer1_Tick;
            // 
            // panel1
            // 
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(844, 426);
            panel1.TabIndex = 32;
            // 
            // VideoPlayer
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(FileNotFoundLabel);
            Controls.Add(ReloadBtn);
            Controls.Add(PauseBtn);
            Controls.Add(PlayBtn);
            Controls.Add(label1);
            Controls.Add(VolumeTrackBar);
            Controls.Add(TotalTimeLabel);
            Controls.Add(CurrentTimeLabel);
            Controls.Add(VideoScrubTrackBar);
            Controls.Add(panel1);
            Name = "VideoPlayer";
            Size = new Size(844, 426);
            Load += VideoPlayer_Load;
            ((System.ComponentModel.ISupportInitialize)VideoScrubTrackBar).EndInit();
            ((System.ComponentModel.ISupportInitialize)VolumeTrackBar).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Label TotalTimeLabel;
        private Label CurrentTimeLabel;
        private TrackBar VideoScrubTrackBar;
        private TrackBar VolumeTrackBar;
        private Label label1;
        private Button PlayBtn;
        private Button PauseBtn;
        private Button ReloadBtn;
        private Label FileNotFoundLabel;
        private System.Windows.Forms.Timer timer1;
        private Panel panel1;
    }
}
