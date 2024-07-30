namespace MakeMKV_Title_Decoder {
    partial class FileRenamer {
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
            NextBtn = new Button();
            VideoSrubTrackBar = new TrackBar();
            VolumeTrackBar = new TrackBar();
            label1 = new Label();
            PlayBtn = new Button();
            PauseBtn = new Button();
            titleInfo1 = new TitleInfo();
            ReloadBtn = new Button();
            timer1 = new System.Windows.Forms.Timer(components);
            ((System.ComponentModel.ISupportInitialize)VideoViewVLC1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)VideoSrubTrackBar).BeginInit();
            ((System.ComponentModel.ISupportInitialize)VolumeTrackBar).BeginInit();
            SuspendLayout();
            // 
            // VideoViewVLC1
            // 
            VideoViewVLC1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            VideoViewVLC1.BackColor = Color.Black;
            VideoViewVLC1.Location = new Point(12, 12);
            VideoViewVLC1.MediaPlayer = null;
            VideoViewVLC1.Name = "VideoViewVLC1";
            VideoViewVLC1.Size = new Size(788, 285);
            VideoViewVLC1.TabIndex = 10;
            VideoViewVLC1.Text = "videoView1";
            // 
            // NextBtn
            // 
            NextBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            NextBtn.Location = new Point(114, 482);
            NextBtn.Name = "NextBtn";
            NextBtn.Size = new Size(75, 23);
            NextBtn.TabIndex = 12;
            NextBtn.Text = "Next";
            NextBtn.UseVisualStyleBackColor = true;
            NextBtn.Click += NextBtn_Click;
            // 
            // VideoSrubTrackBar
            // 
            VideoSrubTrackBar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            VideoSrubTrackBar.Enabled = false;
            VideoSrubTrackBar.Location = new Point(12, 303);
            VideoSrubTrackBar.Maximum = 1000;
            VideoSrubTrackBar.Name = "VideoSrubTrackBar";
            VideoSrubTrackBar.Size = new Size(788, 45);
            VideoSrubTrackBar.TabIndex = 13;
            VideoSrubTrackBar.TickStyle = TickStyle.None;
            VideoSrubTrackBar.Scroll += VideoSrubTrackBar_Scroll;
            VideoSrubTrackBar.MouseCaptureChanged += VideoSrubTrackBar_MouseCaptureChanged;
            // 
            // VolumeTrackBar
            // 
            VolumeTrackBar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            VolumeTrackBar.LargeChange = 10;
            VolumeTrackBar.Location = new Point(12, 354);
            VolumeTrackBar.Maximum = 100;
            VolumeTrackBar.Name = "VolumeTrackBar";
            VolumeTrackBar.Orientation = Orientation.Vertical;
            VolumeTrackBar.Size = new Size(45, 104);
            VolumeTrackBar.TabIndex = 14;
            VolumeTrackBar.TickFrequency = 10;
            VolumeTrackBar.Value = 50;
            VolumeTrackBar.Scroll += VolumeTrackBar_Scroll;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label1.AutoSize = true;
            label1.Location = new Point(10, 461);
            label1.Name = "label1";
            label1.Size = new Size(47, 15);
            label1.TabIndex = 15;
            label1.Text = "Volume";
            // 
            // PlayBtn
            // 
            PlayBtn.Enabled = false;
            PlayBtn.Location = new Point(190, 325);
            PlayBtn.Name = "PlayBtn";
            PlayBtn.Size = new Size(77, 23);
            PlayBtn.TabIndex = 16;
            PlayBtn.Text = "Play";
            PlayBtn.UseVisualStyleBackColor = true;
            PlayBtn.Click += PlayBtn_Click;
            // 
            // PauseBtn
            // 
            PauseBtn.Enabled = false;
            PauseBtn.Location = new Point(273, 325);
            PauseBtn.Name = "PauseBtn";
            PauseBtn.Size = new Size(75, 23);
            PauseBtn.TabIndex = 17;
            PauseBtn.Text = "Pause";
            PauseBtn.UseVisualStyleBackColor = true;
            PauseBtn.Click += PauseBtn_Click;
            // 
            // titleInfo1
            // 
            titleInfo1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            titleInfo1.Location = new Point(114, 364);
            titleInfo1.Name = "titleInfo1";
            titleInfo1.Size = new Size(708, 112);
            titleInfo1.TabIndex = 18;
            // 
            // ReloadBtn
            // 
            ReloadBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            ReloadBtn.Location = new Point(195, 482);
            ReloadBtn.Name = "ReloadBtn";
            ReloadBtn.Size = new Size(75, 23);
            ReloadBtn.TabIndex = 19;
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
            // FileRenamer
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(834, 513);
            Controls.Add(ReloadBtn);
            Controls.Add(titleInfo1);
            Controls.Add(PauseBtn);
            Controls.Add(PlayBtn);
            Controls.Add(label1);
            Controls.Add(VolumeTrackBar);
            Controls.Add(VideoSrubTrackBar);
            Controls.Add(NextBtn);
            Controls.Add(VideoViewVLC1);
            Name = "FileRenamer";
            Text = "FileRenamer";
            FormClosing += FileRenamer_FormClosing;
            Load += FileRenamer_Load;
            ((System.ComponentModel.ISupportInitialize)VideoViewVLC1).EndInit();
            ((System.ComponentModel.ISupportInitialize)VideoSrubTrackBar).EndInit();
            ((System.ComponentModel.ISupportInitialize)VolumeTrackBar).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private LibVLCSharp.WinForms.VideoView VideoViewVLC1;
        private Button NextBtn;
        private TrackBar VideoSrubTrackBar;
        private TrackBar VolumeTrackBar;
        private Label label1;
        private Button PlayBtn;
        private Button PauseBtn;
        private TitleInfo titleInfo1;
        private Button ReloadBtn;
        private System.Windows.Forms.Timer timer1;
    }
}