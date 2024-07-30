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
            UserInputPanel = new Panel();
            KeepOptionsPanel = new Panel();
            BonusFeatureRadioBtn = new RadioButton();
            EpisodeRadioRadioBtn = new RadioButton();
            NameTextBox = new TextBox();
            label2 = new Label();
            DeleteOptionsPanel = new Panel();
            DeleteAllChapters = new RadioButton();
            DeleteThisFile = new RadioButton();
            panel2 = new Panel();
            KeepRadioBtn = new RadioButton();
            DeleteRadioBtn = new RadioButton();
            BreakApartRadioBtn = new RadioButton();
            DeleteEpisodesCheckBox = new CheckBox();
            ((System.ComponentModel.ISupportInitialize)VideoViewVLC1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)VideoSrubTrackBar).BeginInit();
            ((System.ComponentModel.ISupportInitialize)VolumeTrackBar).BeginInit();
            UserInputPanel.SuspendLayout();
            KeepOptionsPanel.SuspendLayout();
            DeleteOptionsPanel.SuspendLayout();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // VideoViewVLC1
            // 
            VideoViewVLC1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            VideoViewVLC1.BackColor = Color.Black;
            VideoViewVLC1.Location = new Point(12, 12);
            VideoViewVLC1.MediaPlayer = null;
            VideoViewVLC1.Name = "VideoViewVLC1";
            VideoViewVLC1.Size = new Size(625, 285);
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
            VideoSrubTrackBar.Size = new Size(810, 45);
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
            // UserInputPanel
            // 
            UserInputPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            UserInputPanel.Controls.Add(KeepOptionsPanel);
            UserInputPanel.Controls.Add(DeleteOptionsPanel);
            UserInputPanel.Controls.Add(panel2);
            UserInputPanel.Location = new Point(643, 12);
            UserInputPanel.Name = "UserInputPanel";
            UserInputPanel.Size = new Size(179, 285);
            UserInputPanel.TabIndex = 20;
            // 
            // KeepOptionsPanel
            // 
            KeepOptionsPanel.Controls.Add(DeleteEpisodesCheckBox);
            KeepOptionsPanel.Controls.Add(BonusFeatureRadioBtn);
            KeepOptionsPanel.Controls.Add(EpisodeRadioRadioBtn);
            KeepOptionsPanel.Controls.Add(NameTextBox);
            KeepOptionsPanel.Controls.Add(label2);
            KeepOptionsPanel.Location = new Point(3, 88);
            KeepOptionsPanel.Name = "KeepOptionsPanel";
            KeepOptionsPanel.Size = new Size(167, 103);
            KeepOptionsPanel.TabIndex = 7;
            // 
            // BonusFeatureRadioBtn
            // 
            BonusFeatureRadioBtn.AutoSize = true;
            BonusFeatureRadioBtn.Location = new Point(3, 57);
            BonusFeatureRadioBtn.Name = "BonusFeatureRadioBtn";
            BonusFeatureRadioBtn.Size = new Size(100, 19);
            BonusFeatureRadioBtn.TabIndex = 1;
            BonusFeatureRadioBtn.TabStop = true;
            BonusFeatureRadioBtn.Text = "Bonus Feature";
            BonusFeatureRadioBtn.UseVisualStyleBackColor = true;
            // 
            // EpisodeRadioRadioBtn
            // 
            EpisodeRadioRadioBtn.AutoSize = true;
            EpisodeRadioRadioBtn.Checked = true;
            EpisodeRadioRadioBtn.Location = new Point(3, 32);
            EpisodeRadioRadioBtn.Name = "EpisodeRadioRadioBtn";
            EpisodeRadioRadioBtn.Size = new Size(66, 19);
            EpisodeRadioRadioBtn.TabIndex = 0;
            EpisodeRadioRadioBtn.TabStop = true;
            EpisodeRadioRadioBtn.Text = "Episode";
            EpisodeRadioRadioBtn.UseVisualStyleBackColor = true;
            // 
            // NameTextBox
            // 
            NameTextBox.Location = new Point(54, 3);
            NameTextBox.Name = "NameTextBox";
            NameTextBox.Size = new Size(110, 23);
            NameTextBox.TabIndex = 4;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(6, 6);
            label2.Name = "label2";
            label2.Size = new Size(42, 15);
            label2.TabIndex = 3;
            label2.Text = "Name:";
            // 
            // DeleteOptionsPanel
            // 
            DeleteOptionsPanel.Controls.Add(DeleteAllChapters);
            DeleteOptionsPanel.Controls.Add(DeleteThisFile);
            DeleteOptionsPanel.Enabled = false;
            DeleteOptionsPanel.Location = new Point(3, 197);
            DeleteOptionsPanel.Name = "DeleteOptionsPanel";
            DeleteOptionsPanel.Size = new Size(112, 56);
            DeleteOptionsPanel.TabIndex = 6;
            // 
            // DeleteAllChapters
            // 
            DeleteAllChapters.AutoSize = true;
            DeleteAllChapters.Location = new Point(3, 28);
            DeleteAllChapters.Name = "DeleteAllChapters";
            DeleteAllChapters.Size = new Size(88, 19);
            DeleteAllChapters.TabIndex = 1;
            DeleteAllChapters.TabStop = true;
            DeleteAllChapters.Text = "All Episodes";
            DeleteAllChapters.UseVisualStyleBackColor = true;
            // 
            // DeleteThisFile
            // 
            DeleteThisFile.AutoSize = true;
            DeleteThisFile.Checked = true;
            DeleteThisFile.Location = new Point(3, 3);
            DeleteThisFile.Name = "DeleteThisFile";
            DeleteThisFile.Size = new Size(91, 19);
            DeleteThisFile.TabIndex = 0;
            DeleteThisFile.TabStop = true;
            DeleteThisFile.Text = "Only this file";
            DeleteThisFile.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            panel2.Controls.Add(KeepRadioBtn);
            panel2.Controls.Add(DeleteRadioBtn);
            panel2.Controls.Add(BreakApartRadioBtn);
            panel2.Location = new Point(3, 3);
            panel2.Name = "panel2";
            panel2.Size = new Size(93, 79);
            panel2.TabIndex = 5;
            // 
            // KeepRadioBtn
            // 
            KeepRadioBtn.AutoSize = true;
            KeepRadioBtn.Checked = true;
            KeepRadioBtn.Location = new Point(3, 3);
            KeepRadioBtn.Name = "KeepRadioBtn";
            KeepRadioBtn.Size = new Size(51, 19);
            KeepRadioBtn.TabIndex = 0;
            KeepRadioBtn.TabStop = true;
            KeepRadioBtn.Text = "Keep";
            KeepRadioBtn.UseVisualStyleBackColor = true;
            KeepRadioBtn.CheckedChanged += KeepRadioBtn_CheckedChanged;
            // 
            // DeleteRadioBtn
            // 
            DeleteRadioBtn.AutoSize = true;
            DeleteRadioBtn.Location = new Point(3, 28);
            DeleteRadioBtn.Name = "DeleteRadioBtn";
            DeleteRadioBtn.Size = new Size(58, 19);
            DeleteRadioBtn.TabIndex = 1;
            DeleteRadioBtn.Text = "Delete";
            DeleteRadioBtn.UseVisualStyleBackColor = true;
            DeleteRadioBtn.CheckedChanged += DeleteRadioBtn_CheckedChanged;
            // 
            // BreakApartRadioBtn
            // 
            BreakApartRadioBtn.AutoSize = true;
            BreakApartRadioBtn.Location = new Point(3, 53);
            BreakApartRadioBtn.Name = "BreakApartRadioBtn";
            BreakApartRadioBtn.Size = new Size(84, 19);
            BreakApartRadioBtn.TabIndex = 2;
            BreakApartRadioBtn.TabStop = true;
            BreakApartRadioBtn.Text = "Break apart";
            BreakApartRadioBtn.UseVisualStyleBackColor = true;
            // 
            // DeleteEpisodesCheckBox
            // 
            DeleteEpisodesCheckBox.AutoSize = true;
            DeleteEpisodesCheckBox.Location = new Point(3, 82);
            DeleteEpisodesCheckBox.Name = "DeleteEpisodesCheckBox";
            DeleteEpisodesCheckBox.Size = new Size(108, 19);
            DeleteEpisodesCheckBox.TabIndex = 5;
            DeleteEpisodesCheckBox.Text = "Delete Episodes";
            DeleteEpisodesCheckBox.UseVisualStyleBackColor = true;
            // 
            // FileRenamer
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(834, 513);
            Controls.Add(UserInputPanel);
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
            UserInputPanel.ResumeLayout(false);
            KeepOptionsPanel.ResumeLayout(false);
            KeepOptionsPanel.PerformLayout();
            DeleteOptionsPanel.ResumeLayout(false);
            DeleteOptionsPanel.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
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
        private Panel UserInputPanel;
        private Panel panel2;
        private RadioButton KeepRadioBtn;
        private RadioButton DeleteRadioBtn;
        private RadioButton BreakApartRadioBtn;
        private TextBox NameTextBox;
        private Label label2;
        private Panel DeleteOptionsPanel;
        private RadioButton DeleteAllChapters;
        private RadioButton DeleteThisFile;
        private Panel KeepOptionsPanel;
        private RadioButton BonusFeatureRadioBtn;
        private RadioButton EpisodeRadioRadioBtn;
        private CheckBox DeleteEpisodesCheckBox;
    }
}