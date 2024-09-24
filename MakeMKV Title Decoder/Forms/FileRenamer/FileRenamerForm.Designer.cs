namespace MakeMKV_Title_Decoder.Forms.FileRenamer {
    partial class FileRenamerForm {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FileRenamerForm));
            splitContainer1 = new SplitContainer();
            PlaylistListBox1 = new PlaylistListBox();
            imageList1 = new ImageList(components);
            panel1 = new Panel();
            ExportAllBtn = new Button();
            panel2 = new Panel();
            ApplyChangesBtn = new Button();
            OptionsPanel = new Panel();
            ExtraName = new Label();
            ExtraNameTextBox = new TextBox();
            MainFeatureRadioButton = new RadioButton();
            TrailersRadioButton = new RadioButton();
            InterviewsRadioButton = new RadioButton();
            DeletedScenesRadioButton = new RadioButton();
            BehindTheScenesRadioButton = new RadioButton();
            FeaturettesRadioButton = new RadioButton();
            ScenesRadioButton = new RadioButton();
            ShortsRadioButton = new RadioButton();
            SpecialsRadioButton = new RadioButton();
            ExtrasRadioButton = new RadioButton();
            MultiVersionTextBox = new TextBox();
            MultiVersionCheckBox = new CheckBox();
            ShowNameTextBox = new TextBox();
            label5 = new Label();
            CreateFolderBtn = new Button();
            OutputFolderLabel = new Label();
            SelectOutputFolderBtn = new Button();
            panel3 = new Panel();
            TmdbIdLabel = new Label();
            EpisodeTextBox = new TextBox();
            TmdbBtn = new Button();
            SeasonTextBox = new TextBox();
            label1 = new Label();
            label4 = new Label();
            IdTextBox = new TextBox();
            label3 = new Label();
            label2 = new Label();
            TypeComboBox = new ComboBox();
            ExportSelectedBtn = new Button();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            OptionsPanel.SuspendLayout();
            panel3.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(PlaylistListBox1);
            splitContainer1.Panel1.Controls.Add(panel1);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(panel2);
            splitContainer1.Size = new Size(1366, 818);
            splitContainer1.SplitterDistance = 452;
            splitContainer1.TabIndex = 0;
            // 
            // PlaylistListBox1
            // 
            PlaylistListBox1.Dock = DockStyle.Fill;
            PlaylistListBox1.DrawMode = DrawMode.OwnerDrawFixed;
            PlaylistListBox1.FormattingEnabled = true;
            PlaylistListBox1.Location = new Point(0, 0);
            PlaylistListBox1.Name = "PlaylistListBox1";
            PlaylistListBox1.SelectedItem = null;
            PlaylistListBox1.Size = new Size(452, 775);
            PlaylistListBox1.SmallIconList = imageList1;
            PlaylistListBox1.TabIndex = 1;
            PlaylistListBox1.SelectedIndexChanged += PlaylistListBox1_SelectedIndexChanged;
            // 
            // imageList1
            // 
            imageList1.ColorDepth = ColorDepth.Depth32Bit;
            imageList1.ImageStream = (ImageListStreamer)resources.GetObject("imageList1.ImageStream");
            imageList1.TransparentColor = Color.Transparent;
            imageList1.Images.SetKeyName(0, "dialog-error.png");
            imageList1.Images.SetKeyName(1, "dialog-ok-apply.png");
            imageList1.Images.SetKeyName(2, "dialog-warning.png");
            // 
            // panel1
            // 
            panel1.Controls.Add(ExportSelectedBtn);
            panel1.Controls.Add(ExportAllBtn);
            panel1.Dock = DockStyle.Bottom;
            panel1.Location = new Point(0, 775);
            panel1.Name = "panel1";
            panel1.Size = new Size(452, 43);
            panel1.TabIndex = 0;
            // 
            // ExportAllBtn
            // 
            ExportAllBtn.Location = new Point(12, 6);
            ExportAllBtn.Name = "ExportAllBtn";
            ExportAllBtn.Size = new Size(75, 23);
            ExportAllBtn.TabIndex = 0;
            ExportAllBtn.Text = "Export All";
            ExportAllBtn.UseVisualStyleBackColor = true;
            ExportAllBtn.Click += ExportAllBtn_Click;
            // 
            // panel2
            // 
            panel2.Controls.Add(ApplyChangesBtn);
            panel2.Controls.Add(OptionsPanel);
            panel2.Controls.Add(ShowNameTextBox);
            panel2.Controls.Add(label5);
            panel2.Controls.Add(CreateFolderBtn);
            panel2.Controls.Add(OutputFolderLabel);
            panel2.Controls.Add(SelectOutputFolderBtn);
            panel2.Controls.Add(panel3);
            panel2.Dock = DockStyle.Fill;
            panel2.Location = new Point(0, 0);
            panel2.Name = "panel2";
            panel2.Size = new Size(910, 818);
            panel2.TabIndex = 2;
            // 
            // ApplyChangesBtn
            // 
            ApplyChangesBtn.Location = new Point(6, 699);
            ApplyChangesBtn.Name = "ApplyChangesBtn";
            ApplyChangesBtn.Size = new Size(75, 23);
            ApplyChangesBtn.TabIndex = 17;
            ApplyChangesBtn.Text = "Apply";
            ApplyChangesBtn.UseVisualStyleBackColor = true;
            ApplyChangesBtn.Click += ApplyChangesBtn_Click;
            // 
            // OptionsPanel
            // 
            OptionsPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            OptionsPanel.Controls.Add(ExtraName);
            OptionsPanel.Controls.Add(ExtraNameTextBox);
            OptionsPanel.Controls.Add(MainFeatureRadioButton);
            OptionsPanel.Controls.Add(TrailersRadioButton);
            OptionsPanel.Controls.Add(InterviewsRadioButton);
            OptionsPanel.Controls.Add(DeletedScenesRadioButton);
            OptionsPanel.Controls.Add(BehindTheScenesRadioButton);
            OptionsPanel.Controls.Add(FeaturettesRadioButton);
            OptionsPanel.Controls.Add(ScenesRadioButton);
            OptionsPanel.Controls.Add(ShortsRadioButton);
            OptionsPanel.Controls.Add(SpecialsRadioButton);
            OptionsPanel.Controls.Add(ExtrasRadioButton);
            OptionsPanel.Controls.Add(MultiVersionTextBox);
            OptionsPanel.Controls.Add(MultiVersionCheckBox);
            OptionsPanel.Location = new Point(6, 305);
            OptionsPanel.Name = "OptionsPanel";
            OptionsPanel.Size = new Size(892, 373);
            OptionsPanel.TabIndex = 16;
            // 
            // ExtraName
            // 
            ExtraName.AutoSize = true;
            ExtraName.Location = new Point(3, 344);
            ExtraName.Name = "ExtraName";
            ExtraName.Size = new Size(42, 15);
            ExtraName.TabIndex = 15;
            ExtraName.Text = "Name:";
            // 
            // ExtraNameTextBox
            // 
            ExtraNameTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            ExtraNameTextBox.Enabled = false;
            ExtraNameTextBox.Location = new Point(51, 341);
            ExtraNameTextBox.Name = "ExtraNameTextBox";
            ExtraNameTextBox.Size = new Size(838, 23);
            ExtraNameTextBox.TabIndex = 12;
            // 
            // MainFeatureRadioButton
            // 
            MainFeatureRadioButton.AutoSize = true;
            MainFeatureRadioButton.Checked = true;
            MainFeatureRadioButton.Location = new Point(3, 78);
            MainFeatureRadioButton.Name = "MainFeatureRadioButton";
            MainFeatureRadioButton.Size = new Size(94, 19);
            MainFeatureRadioButton.TabIndex = 11;
            MainFeatureRadioButton.TabStop = true;
            MainFeatureRadioButton.Text = "Main Feature";
            MainFeatureRadioButton.UseVisualStyleBackColor = true;
            MainFeatureRadioButton.CheckedChanged += FeatureTypeRadioButton_CheckedChanged;
            // 
            // TrailersRadioButton
            // 
            TrailersRadioButton.AutoSize = true;
            TrailersRadioButton.Location = new Point(3, 116);
            TrailersRadioButton.Name = "TrailersRadioButton";
            TrailersRadioButton.Size = new Size(61, 19);
            TrailersRadioButton.TabIndex = 10;
            TrailersRadioButton.Text = "Trailers";
            TrailersRadioButton.UseVisualStyleBackColor = true;
            TrailersRadioButton.CheckedChanged += FeatureTypeRadioButton_CheckedChanged;
            // 
            // InterviewsRadioButton
            // 
            InterviewsRadioButton.AutoSize = true;
            InterviewsRadioButton.Location = new Point(3, 316);
            InterviewsRadioButton.Name = "InterviewsRadioButton";
            InterviewsRadioButton.Size = new Size(78, 19);
            InterviewsRadioButton.TabIndex = 9;
            InterviewsRadioButton.Text = "Interviews";
            InterviewsRadioButton.UseVisualStyleBackColor = true;
            InterviewsRadioButton.CheckedChanged += FeatureTypeRadioButton_CheckedChanged;
            // 
            // DeletedScenesRadioButton
            // 
            DeletedScenesRadioButton.AutoSize = true;
            DeletedScenesRadioButton.Location = new Point(3, 291);
            DeletedScenesRadioButton.Name = "DeletedScenesRadioButton";
            DeletedScenesRadioButton.Size = new Size(104, 19);
            DeletedScenesRadioButton.TabIndex = 8;
            DeletedScenesRadioButton.Text = "Deleted Scenes";
            DeletedScenesRadioButton.UseVisualStyleBackColor = true;
            DeletedScenesRadioButton.CheckedChanged += FeatureTypeRadioButton_CheckedChanged;
            // 
            // BehindTheScenesRadioButton
            // 
            BehindTheScenesRadioButton.AutoSize = true;
            BehindTheScenesRadioButton.Location = new Point(3, 266);
            BehindTheScenesRadioButton.Name = "BehindTheScenesRadioButton";
            BehindTheScenesRadioButton.Size = new Size(123, 19);
            BehindTheScenesRadioButton.TabIndex = 7;
            BehindTheScenesRadioButton.Text = "Behind The Scenes";
            BehindTheScenesRadioButton.UseVisualStyleBackColor = true;
            BehindTheScenesRadioButton.CheckedChanged += FeatureTypeRadioButton_CheckedChanged;
            // 
            // FeaturettesRadioButton
            // 
            FeaturettesRadioButton.AutoSize = true;
            FeaturettesRadioButton.Location = new Point(3, 241);
            FeaturettesRadioButton.Name = "FeaturettesRadioButton";
            FeaturettesRadioButton.Size = new Size(83, 19);
            FeaturettesRadioButton.TabIndex = 6;
            FeaturettesRadioButton.Text = "Featurettes";
            FeaturettesRadioButton.UseVisualStyleBackColor = true;
            FeaturettesRadioButton.CheckedChanged += FeatureTypeRadioButton_CheckedChanged;
            // 
            // ScenesRadioButton
            // 
            ScenesRadioButton.AutoSize = true;
            ScenesRadioButton.Location = new Point(3, 216);
            ScenesRadioButton.Name = "ScenesRadioButton";
            ScenesRadioButton.Size = new Size(61, 19);
            ScenesRadioButton.TabIndex = 5;
            ScenesRadioButton.Text = "Scenes";
            ScenesRadioButton.UseVisualStyleBackColor = true;
            ScenesRadioButton.CheckedChanged += FeatureTypeRadioButton_CheckedChanged;
            // 
            // ShortsRadioButton
            // 
            ShortsRadioButton.AutoSize = true;
            ShortsRadioButton.Location = new Point(3, 191);
            ShortsRadioButton.Name = "ShortsRadioButton";
            ShortsRadioButton.Size = new Size(58, 19);
            ShortsRadioButton.TabIndex = 4;
            ShortsRadioButton.Text = "Shorts";
            ShortsRadioButton.UseVisualStyleBackColor = true;
            ShortsRadioButton.CheckedChanged += FeatureTypeRadioButton_CheckedChanged;
            // 
            // SpecialsRadioButton
            // 
            SpecialsRadioButton.AutoSize = true;
            SpecialsRadioButton.Location = new Point(3, 166);
            SpecialsRadioButton.Name = "SpecialsRadioButton";
            SpecialsRadioButton.Size = new Size(67, 19);
            SpecialsRadioButton.TabIndex = 3;
            SpecialsRadioButton.Text = "Specials";
            SpecialsRadioButton.UseVisualStyleBackColor = true;
            SpecialsRadioButton.CheckedChanged += FeatureTypeRadioButton_CheckedChanged;
            // 
            // ExtrasRadioButton
            // 
            ExtrasRadioButton.AutoSize = true;
            ExtrasRadioButton.Location = new Point(3, 141);
            ExtrasRadioButton.Name = "ExtrasRadioButton";
            ExtrasRadioButton.Size = new Size(56, 19);
            ExtrasRadioButton.TabIndex = 2;
            ExtrasRadioButton.Text = "Extras";
            ExtrasRadioButton.UseVisualStyleBackColor = true;
            ExtrasRadioButton.CheckedChanged += FeatureTypeRadioButton_CheckedChanged;
            // 
            // MultiVersionTextBox
            // 
            MultiVersionTextBox.Enabled = false;
            MultiVersionTextBox.Location = new Point(3, 28);
            MultiVersionTextBox.Name = "MultiVersionTextBox";
            MultiVersionTextBox.Size = new Size(272, 23);
            MultiVersionTextBox.TabIndex = 1;
            // 
            // MultiVersionCheckBox
            // 
            MultiVersionCheckBox.AutoSize = true;
            MultiVersionCheckBox.Location = new Point(3, 3);
            MultiVersionCheckBox.Name = "MultiVersionCheckBox";
            MultiVersionCheckBox.Size = new Size(97, 19);
            MultiVersionCheckBox.TabIndex = 0;
            MultiVersionCheckBox.Text = "Mutli-Version";
            MultiVersionCheckBox.UseVisualStyleBackColor = true;
            MultiVersionCheckBox.CheckedChanged += MultiVersionCheckBox_CheckedChanged;
            // 
            // ShowNameTextBox
            // 
            ShowNameTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            ShowNameTextBox.Location = new Point(86, 196);
            ShowNameTextBox.Name = "ShowNameTextBox";
            ShowNameTextBox.Size = new Size(812, 23);
            ShowNameTextBox.TabIndex = 15;
            ShowNameTextBox.TextChanged += ShowNameTextBox_TextChanged_1;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(6, 199);
            label5.Name = "label5";
            label5.Size = new Size(74, 15);
            label5.TabIndex = 14;
            label5.Text = "Show Name:";
            // 
            // CreateFolderBtn
            // 
            CreateFolderBtn.Location = new Point(87, 242);
            CreateFolderBtn.Name = "CreateFolderBtn";
            CreateFolderBtn.Size = new Size(75, 23);
            CreateFolderBtn.TabIndex = 13;
            CreateFolderBtn.Text = "Create";
            CreateFolderBtn.UseVisualStyleBackColor = true;
            CreateFolderBtn.Click += CreateFolderBtn_Click;
            // 
            // OutputFolderLabel
            // 
            OutputFolderLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            OutputFolderLabel.AutoSize = true;
            OutputFolderLabel.Location = new Point(6, 224);
            OutputFolderLabel.Name = "OutputFolderLabel";
            OutputFolderLabel.Size = new Size(84, 15);
            OutputFolderLabel.TabIndex = 12;
            OutputFolderLabel.Text = "Output Folder:";
            // 
            // SelectOutputFolderBtn
            // 
            SelectOutputFolderBtn.Location = new Point(6, 242);
            SelectOutputFolderBtn.Name = "SelectOutputFolderBtn";
            SelectOutputFolderBtn.Size = new Size(75, 23);
            SelectOutputFolderBtn.TabIndex = 11;
            SelectOutputFolderBtn.Text = "Select";
            SelectOutputFolderBtn.UseVisualStyleBackColor = true;
            SelectOutputFolderBtn.Click += SelectOutputFolderBtn_Click;
            // 
            // panel3
            // 
            panel3.Controls.Add(TmdbIdLabel);
            panel3.Controls.Add(EpisodeTextBox);
            panel3.Controls.Add(TmdbBtn);
            panel3.Controls.Add(SeasonTextBox);
            panel3.Controls.Add(label1);
            panel3.Controls.Add(label4);
            panel3.Controls.Add(IdTextBox);
            panel3.Controls.Add(label3);
            panel3.Controls.Add(label2);
            panel3.Controls.Add(TypeComboBox);
            panel3.Location = new Point(3, 12);
            panel3.Name = "panel3";
            panel3.Size = new Size(264, 158);
            panel3.TabIndex = 10;
            // 
            // TmdbIdLabel
            // 
            TmdbIdLabel.AutoSize = true;
            TmdbIdLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            TmdbIdLabel.Location = new Point(3, 9);
            TmdbIdLabel.Name = "TmdbIdLabel";
            TmdbIdLabel.Size = new Size(45, 15);
            TmdbIdLabel.TabIndex = 0;
            TmdbIdLabel.Text = "TMDB:";
            // 
            // EpisodeTextBox
            // 
            EpisodeTextBox.Location = new Point(90, 123);
            EpisodeTextBox.Name = "EpisodeTextBox";
            EpisodeTextBox.Size = new Size(153, 23);
            EpisodeTextBox.TabIndex = 9;
            EpisodeTextBox.TextChanged += EpisodeTextBox_TextChanged;
            // 
            // TmdbBtn
            // 
            TmdbBtn.Location = new Point(54, 5);
            TmdbBtn.Name = "TmdbBtn";
            TmdbBtn.Size = new Size(75, 23);
            TmdbBtn.TabIndex = 1;
            TmdbBtn.Text = "Select";
            TmdbBtn.UseVisualStyleBackColor = true;
            TmdbBtn.Click += TmdbBtn_Click;
            // 
            // SeasonTextBox
            // 
            SeasonTextBox.Location = new Point(90, 94);
            SeasonTextBox.Name = "SeasonTextBox";
            SeasonTextBox.Size = new Size(153, 23);
            SeasonTextBox.TabIndex = 8;
            SeasonTextBox.TextChanged += SeasonTextBox_TextChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(33, 66);
            label1.Name = "label1";
            label1.Size = new Size(21, 15);
            label1.TabIndex = 2;
            label1.Text = "ID:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(33, 126);
            label4.Name = "label4";
            label4.Size = new Size(51, 15);
            label4.TabIndex = 7;
            label4.Text = "Episode:";
            // 
            // IdTextBox
            // 
            IdTextBox.Location = new Point(90, 63);
            IdTextBox.Name = "IdTextBox";
            IdTextBox.Size = new Size(153, 23);
            IdTextBox.TabIndex = 3;
            IdTextBox.TextChanged += IdTextBox_TextChanged_1;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(33, 97);
            label3.Name = "label3";
            label3.Size = new Size(47, 15);
            label3.TabIndex = 6;
            label3.Text = "Season:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(33, 37);
            label2.Name = "label2";
            label2.Size = new Size(34, 15);
            label2.TabIndex = 4;
            label2.Text = "Type:";
            // 
            // TypeComboBox
            // 
            TypeComboBox.FormattingEnabled = true;
            TypeComboBox.Location = new Point(90, 34);
            TypeComboBox.Name = "TypeComboBox";
            TypeComboBox.Size = new Size(153, 23);
            TypeComboBox.TabIndex = 5;
            TypeComboBox.SelectedIndexChanged += TypeComboBox_SelectedIndexChanged;
            // 
            // ExportSelectedBtn
            // 
            ExportSelectedBtn.Location = new Point(124, 6);
            ExportSelectedBtn.Name = "ExportSelectedBtn";
            ExportSelectedBtn.Size = new Size(102, 23);
            ExportSelectedBtn.TabIndex = 1;
            ExportSelectedBtn.Text = "Export Selected";
            ExportSelectedBtn.UseVisualStyleBackColor = true;
            ExportSelectedBtn.Click += ExportSelectedBtn_Click;
            // 
            // FileRenamerForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1366, 818);
            Controls.Add(splitContainer1);
            Name = "FileRenamerForm";
            Text = "FileRenamerForm";
            Load += FileRenamerForm_Load;
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            OptionsPanel.ResumeLayout(false);
            OptionsPanel.PerformLayout();
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private SplitContainer splitContainer1;
        private PlaylistListBox PlaylistListBox1;
        private ImageList imageList1;
        private Panel panel1;
        private Label TmdbIdLabel;
        private Panel panel2;
        private TextBox EpisodeTextBox;
        private TextBox SeasonTextBox;
        private Label label4;
        private Label label3;
        private ComboBox TypeComboBox;
        private Label label2;
        private TextBox IdTextBox;
        private Label label1;
        private Button TmdbBtn;
        private Button ExportAllBtn;
        private Button SelectOutputFolderBtn;
        private Panel panel3;
        private Button CreateFolderBtn;
        private Label OutputFolderLabel;
        private TextBox ShowNameTextBox;
        private Label label5;
        private Panel OptionsPanel;
        private TextBox MultiVersionTextBox;
        private CheckBox MultiVersionCheckBox;
        private RadioButton InterviewsRadioButton;
        private RadioButton DeletedScenesRadioButton;
        private RadioButton BehindTheScenesRadioButton;
        private RadioButton FeaturettesRadioButton;
        private RadioButton ScenesRadioButton;
        private RadioButton ShortsRadioButton;
        private RadioButton SpecialsRadioButton;
        private RadioButton ExtrasRadioButton;
        private RadioButton TrailersRadioButton;
        private RadioButton MainFeatureRadioButton;
        private Button ApplyChangesBtn;
        private Label ExtraName;
        private TextBox ExtraNameTextBox;
        private Button ExportSelectedBtn;
    }
}