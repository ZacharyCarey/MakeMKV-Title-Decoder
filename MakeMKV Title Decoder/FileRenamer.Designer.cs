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
            VideoScrubTrackBar = new TrackBar();
            VolumeTrackBar = new TrackBar();
            label1 = new Label();
            PlayBtn = new Button();
            PauseBtn = new Button();
            titleInfo1 = new TitleInfo();
            ReloadBtn = new Button();
            timer1 = new System.Windows.Forms.Timer(components);
            UserInputPanel = new Panel();
            KeepOptionsPanel = new Panel();
            FolderLabel = new Label();
            NewFolderBtn = new Button();
            FolderLabelLabel = new Label();
            InvalidNameLabel = new Label();
            UniqueNameLabel = new Label();
            DeleteEpisodesCheckBox = new CheckBox();
            BonusFeatureRadioBtn = new RadioButton();
            EpisodeRadioRadioBtn = new RadioButton();
            NameTextBox = new TextBox();
            label2 = new Label();
            DeleteOptionsPanel = new Panel();
            DeleteAllChapters = new RadioButton();
            DeleteThisFile = new RadioButton();
            panel2 = new Panel();
            SameAsRadioBtn = new RadioButton();
            EpisodeComboBox = new ComboBox();
            KeepRadioBtn = new RadioButton();
            DeleteRadioBtn = new RadioButton();
            BreakApartRadioBtn = new RadioButton();
            CurrentTimeLabel = new Label();
            TotalTimeLabel = new Label();
            FileNotFoundLabel = new Label();
            ((System.ComponentModel.ISupportInitialize)VideoViewVLC1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)VideoScrubTrackBar).BeginInit();
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
            VideoViewVLC1.Size = new Size(508, 340);
            VideoViewVLC1.TabIndex = 10;
            VideoViewVLC1.Text = "videoView1";
            // 
            // NextBtn
            // 
            NextBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            NextBtn.Location = new Point(114, 537);
            NextBtn.Name = "NextBtn";
            NextBtn.Size = new Size(75, 23);
            NextBtn.TabIndex = 12;
            NextBtn.Text = "Next";
            NextBtn.UseVisualStyleBackColor = true;
            NextBtn.Click += NextBtn_Click;
            // 
            // VideoScrubTrackBar
            // 
            VideoScrubTrackBar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            VideoScrubTrackBar.Enabled = false;
            VideoScrubTrackBar.Location = new Point(12, 358);
            VideoScrubTrackBar.Maximum = 1000;
            VideoScrubTrackBar.Name = "VideoScrubTrackBar";
            VideoScrubTrackBar.Size = new Size(810, 45);
            VideoScrubTrackBar.TabIndex = 13;
            VideoScrubTrackBar.TickStyle = TickStyle.None;
            VideoScrubTrackBar.Scroll += VideoSrubTrackBar_Scroll;
            VideoScrubTrackBar.MouseCaptureChanged += VideoSrubTrackBar_MouseCaptureChanged;
            // 
            // VolumeTrackBar
            // 
            VolumeTrackBar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            VolumeTrackBar.LargeChange = 10;
            VolumeTrackBar.Location = new Point(12, 409);
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
            label1.Location = new Point(10, 516);
            label1.Name = "label1";
            label1.Size = new Size(47, 15);
            label1.TabIndex = 15;
            label1.Text = "Volume";
            // 
            // PlayBtn
            // 
            PlayBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            PlayBtn.Enabled = false;
            PlayBtn.Location = new Point(190, 380);
            PlayBtn.Name = "PlayBtn";
            PlayBtn.Size = new Size(77, 23);
            PlayBtn.TabIndex = 16;
            PlayBtn.Text = "Play";
            PlayBtn.UseVisualStyleBackColor = true;
            PlayBtn.Click += PlayBtn_Click;
            // 
            // PauseBtn
            // 
            PauseBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            PauseBtn.Enabled = false;
            PauseBtn.Location = new Point(273, 380);
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
            titleInfo1.Location = new Point(114, 419);
            titleInfo1.Name = "titleInfo1";
            titleInfo1.Size = new Size(708, 112);
            titleInfo1.TabIndex = 18;
            // 
            // ReloadBtn
            // 
            ReloadBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            ReloadBtn.Location = new Point(195, 537);
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
            UserInputPanel.Location = new Point(526, 12);
            UserInputPanel.Name = "UserInputPanel";
            UserInputPanel.Size = new Size(296, 340);
            UserInputPanel.TabIndex = 20;
            // 
            // KeepOptionsPanel
            // 
            KeepOptionsPanel.Controls.Add(FolderLabel);
            KeepOptionsPanel.Controls.Add(NewFolderBtn);
            KeepOptionsPanel.Controls.Add(FolderLabelLabel);
            KeepOptionsPanel.Controls.Add(InvalidNameLabel);
            KeepOptionsPanel.Controls.Add(UniqueNameLabel);
            KeepOptionsPanel.Controls.Add(DeleteEpisodesCheckBox);
            KeepOptionsPanel.Controls.Add(BonusFeatureRadioBtn);
            KeepOptionsPanel.Controls.Add(EpisodeRadioRadioBtn);
            KeepOptionsPanel.Controls.Add(NameTextBox);
            KeepOptionsPanel.Controls.Add(label2);
            KeepOptionsPanel.Location = new Point(3, 117);
            KeepOptionsPanel.Name = "KeepOptionsPanel";
            KeepOptionsPanel.Size = new Size(290, 157);
            KeepOptionsPanel.TabIndex = 7;
            // 
            // FolderLabel
            // 
            FolderLabel.AutoSize = true;
            FolderLabel.Enabled = false;
            FolderLabel.Location = new Point(51, 103);
            FolderLabel.Name = "FolderLabel";
            FolderLabel.Size = new Size(29, 15);
            FolderLabel.TabIndex = 11;
            FolderLabel.Text = "N/A";
            // 
            // NewFolderBtn
            // 
            NewFolderBtn.Enabled = false;
            NewFolderBtn.Location = new Point(200, 118);
            NewFolderBtn.Name = "NewFolderBtn";
            NewFolderBtn.Size = new Size(84, 23);
            NewFolderBtn.TabIndex = 10;
            NewFolderBtn.Text = "Select Folder";
            NewFolderBtn.UseVisualStyleBackColor = true;
            NewFolderBtn.Click += NewFolderBtn_Click;
            // 
            // FolderLabelLabel
            // 
            FolderLabelLabel.AutoSize = true;
            FolderLabelLabel.Enabled = false;
            FolderLabelLabel.Location = new Point(3, 103);
            FolderLabelLabel.Name = "FolderLabelLabel";
            FolderLabelLabel.Size = new Size(43, 15);
            FolderLabelLabel.TabIndex = 8;
            FolderLabelLabel.Text = "Folder:";
            // 
            // InvalidNameLabel
            // 
            InvalidNameLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            InvalidNameLabel.AutoSize = true;
            InvalidNameLabel.ForeColor = Color.Red;
            InvalidNameLabel.Location = new Point(3, 9);
            InvalidNameLabel.Name = "InvalidNameLabel";
            InvalidNameLabel.Size = new Size(97, 15);
            InvalidNameLabel.TabIndex = 7;
            InvalidNameLabel.Text = "Invalid file name!";
            InvalidNameLabel.Visible = false;
            // 
            // UniqueNameLabel
            // 
            UniqueNameLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            UniqueNameLabel.AutoSize = true;
            UniqueNameLabel.ForeColor = Color.Red;
            UniqueNameLabel.Location = new Point(106, 9);
            UniqueNameLabel.Name = "UniqueNameLabel";
            UniqueNameLabel.Size = new Size(128, 15);
            UniqueNameLabel.TabIndex = 6;
            UniqueNameLabel.Text = "Name must be unique!";
            UniqueNameLabel.Visible = false;
            // 
            // DeleteEpisodesCheckBox
            // 
            DeleteEpisodesCheckBox.AutoSize = true;
            DeleteEpisodesCheckBox.Location = new Point(3, 121);
            DeleteEpisodesCheckBox.Name = "DeleteEpisodesCheckBox";
            DeleteEpisodesCheckBox.Size = new Size(108, 19);
            DeleteEpisodesCheckBox.TabIndex = 5;
            DeleteEpisodesCheckBox.Text = "Delete Episodes";
            DeleteEpisodesCheckBox.UseVisualStyleBackColor = true;
            // 
            // BonusFeatureRadioBtn
            // 
            BonusFeatureRadioBtn.AutoSize = true;
            BonusFeatureRadioBtn.Location = new Point(3, 81);
            BonusFeatureRadioBtn.Name = "BonusFeatureRadioBtn";
            BonusFeatureRadioBtn.Size = new Size(100, 19);
            BonusFeatureRadioBtn.TabIndex = 1;
            BonusFeatureRadioBtn.TabStop = true;
            BonusFeatureRadioBtn.Text = "Bonus Feature";
            BonusFeatureRadioBtn.UseVisualStyleBackColor = true;
            BonusFeatureRadioBtn.CheckedChanged += BonusFeatureRadioBtn_CheckedChanged;
            // 
            // EpisodeRadioRadioBtn
            // 
            EpisodeRadioRadioBtn.AutoSize = true;
            EpisodeRadioRadioBtn.Checked = true;
            EpisodeRadioRadioBtn.Location = new Point(3, 56);
            EpisodeRadioRadioBtn.Name = "EpisodeRadioRadioBtn";
            EpisodeRadioRadioBtn.Size = new Size(66, 19);
            EpisodeRadioRadioBtn.TabIndex = 0;
            EpisodeRadioRadioBtn.TabStop = true;
            EpisodeRadioRadioBtn.Text = "Episode";
            EpisodeRadioRadioBtn.UseVisualStyleBackColor = true;
            // 
            // NameTextBox
            // 
            NameTextBox.Location = new Point(51, 27);
            NameTextBox.Name = "NameTextBox";
            NameTextBox.Size = new Size(233, 23);
            NameTextBox.TabIndex = 4;
            NameTextBox.TextChanged += NameTextBox_TextChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(3, 30);
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
            DeleteOptionsPanel.Location = new Point(3, 280);
            DeleteOptionsPanel.Name = "DeleteOptionsPanel";
            DeleteOptionsPanel.Size = new Size(290, 56);
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
            panel2.Controls.Add(SameAsRadioBtn);
            panel2.Controls.Add(EpisodeComboBox);
            panel2.Controls.Add(KeepRadioBtn);
            panel2.Controls.Add(DeleteRadioBtn);
            panel2.Controls.Add(BreakApartRadioBtn);
            panel2.Location = new Point(3, 3);
            panel2.Name = "panel2";
            panel2.Size = new Size(290, 108);
            panel2.TabIndex = 5;
            // 
            // SameAsRadioBtn
            // 
            SameAsRadioBtn.AutoSize = true;
            SameAsRadioBtn.Location = new Point(3, 78);
            SameAsRadioBtn.Name = "SameAsRadioBtn";
            SameAsRadioBtn.Size = new Size(71, 19);
            SameAsRadioBtn.TabIndex = 4;
            SameAsRadioBtn.Text = "Same as:";
            SameAsRadioBtn.UseVisualStyleBackColor = true;
            SameAsRadioBtn.CheckedChanged += SameAsRadioBtn_CheckedChanged;
            // 
            // EpisodeComboBox
            // 
            EpisodeComboBox.Enabled = false;
            EpisodeComboBox.FormattingEnabled = true;
            EpisodeComboBox.Location = new Point(80, 77);
            EpisodeComboBox.Name = "EpisodeComboBox";
            EpisodeComboBox.Size = new Size(207, 23);
            EpisodeComboBox.TabIndex = 3;
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
            BreakApartRadioBtn.Text = "Break apart";
            BreakApartRadioBtn.UseVisualStyleBackColor = true;
            // 
            // CurrentTimeLabel
            // 
            CurrentTimeLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            CurrentTimeLabel.AutoSize = true;
            CurrentTimeLabel.Location = new Point(12, 380);
            CurrentTimeLabel.Name = "CurrentTimeLabel";
            CurrentTimeLabel.Size = new Size(49, 15);
            CurrentTimeLabel.TabIndex = 21;
            CurrentTimeLabel.Text = "00:00:00";
            // 
            // TotalTimeLabel
            // 
            TotalTimeLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            TotalTimeLabel.AutoSize = true;
            TotalTimeLabel.Location = new Point(773, 380);
            TotalTimeLabel.Name = "TotalTimeLabel";
            TotalTimeLabel.Size = new Size(49, 15);
            TotalTimeLabel.TabIndex = 22;
            TotalTimeLabel.Text = "00:00:00";
            // 
            // FileNotFoundLabel
            // 
            FileNotFoundLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            FileNotFoundLabel.AutoSize = true;
            FileNotFoundLabel.ForeColor = Color.Red;
            FileNotFoundLabel.Location = new Point(368, 384);
            FileNotFoundLabel.Name = "FileNotFoundLabel";
            FileNotFoundLabel.Size = new Size(106, 15);
            FileNotFoundLabel.TabIndex = 23;
            FileNotFoundLabel.Text = "Could not find file:";
            FileNotFoundLabel.Visible = false;
            // 
            // FileRenamer
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(834, 568);
            Controls.Add(FileNotFoundLabel);
            Controls.Add(TotalTimeLabel);
            Controls.Add(CurrentTimeLabel);
            Controls.Add(UserInputPanel);
            Controls.Add(ReloadBtn);
            Controls.Add(titleInfo1);
            Controls.Add(PauseBtn);
            Controls.Add(PlayBtn);
            Controls.Add(label1);
            Controls.Add(VolumeTrackBar);
            Controls.Add(VideoScrubTrackBar);
            Controls.Add(NextBtn);
            Controls.Add(VideoViewVLC1);
            Name = "FileRenamer";
            Text = "FileRenamer";
            FormClosing += FileRenamer_FormClosing;
            Load += FileRenamer_Load;
            ((System.ComponentModel.ISupportInitialize)VideoViewVLC1).EndInit();
            ((System.ComponentModel.ISupportInitialize)VideoScrubTrackBar).EndInit();
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
        private TrackBar VideoScrubTrackBar;
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
        private RadioButton SameAsRadioBtn;
        private ComboBox EpisodeComboBox;
        private Label UniqueNameLabel;
        private Label CurrentTimeLabel;
        private Label TotalTimeLabel;
        private Label InvalidNameLabel;
        private Label FolderLabelLabel;
        private Button NewFolderBtn;
        private Label FolderLabel;
        private Label FileNotFoundLabel;
    }
}