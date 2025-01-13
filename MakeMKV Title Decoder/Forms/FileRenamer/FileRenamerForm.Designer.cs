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
            ExportableListBox1 = new ExportableListBox();
            imageList1 = new ImageList(components);
            panel1 = new Panel();
            ExportSelectedBtn = new Button();
            ExportAllBtn = new Button();
            panel2 = new Panel();
            SelectShowBtn = new Button();
            SelectedShowLabel = new Label();
            CopyToOthersBtn = new Button();
            ApplyChangesBtn = new Button();
            OptionsPanel = new Panel();
            GalleryRadioButton = new RadioButton();
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
            panel3 = new Panel();
            TmdbBtn = new Button();
            EpisodeTextBox = new TextBox();
            SeasonTextBox = new TextBox();
            label4 = new Label();
            label3 = new Label();
            EpisodeRangeTextBox = new TextBox();
            EpisodeRangeLabel = new Label();
            EpisodeRangeCheckBox = new CheckBox();
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
            splitContainer1.Panel1.Controls.Add(ExportableListBox1);
            splitContainer1.Panel1.Controls.Add(panel1);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(panel2);
            splitContainer1.Size = new Size(1366, 818);
            splitContainer1.SplitterDistance = 452;
            splitContainer1.TabIndex = 0;
            // 
            // ExportableListBox1
            // 
            ExportableListBox1.Dock = DockStyle.Fill;
            ExportableListBox1.DrawMode = DrawMode.OwnerDrawFixed;
            ExportableListBox1.FormattingEnabled = true;
            ExportableListBox1.Location = new Point(0, 0);
            ExportableListBox1.Name = "ExportableListBox1";
            ExportableListBox1.SelectedItem = null;
            ExportableListBox1.Size = new Size(452, 775);
            ExportableListBox1.SmallIconList = imageList1;
            ExportableListBox1.TabIndex = 1;
            ExportableListBox1.SelectedIndexChanged += ExportableListBox1_SelectedIndexChanged;
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
            panel2.Controls.Add(SelectShowBtn);
            panel2.Controls.Add(SelectedShowLabel);
            panel2.Controls.Add(CopyToOthersBtn);
            panel2.Controls.Add(ApplyChangesBtn);
            panel2.Controls.Add(OptionsPanel);
            panel2.Controls.Add(panel3);
            panel2.Dock = DockStyle.Fill;
            panel2.Location = new Point(0, 0);
            panel2.Name = "panel2";
            panel2.Size = new Size(910, 818);
            panel2.TabIndex = 2;
            // 
            // SelectShowBtn
            // 
            SelectShowBtn.Location = new Point(9, 27);
            SelectShowBtn.Name = "SelectShowBtn";
            SelectShowBtn.Size = new Size(97, 23);
            SelectShowBtn.TabIndex = 20;
            SelectShowBtn.Text = "Select";
            SelectShowBtn.UseVisualStyleBackColor = true;
            SelectShowBtn.Click += SelectShowBtn_Click;
            // 
            // SelectedShowLabel
            // 
            SelectedShowLabel.AutoSize = true;
            SelectedShowLabel.Location = new Point(9, 9);
            SelectedShowLabel.Name = "SelectedShowLabel";
            SelectedShowLabel.Size = new Size(39, 15);
            SelectedShowLabel.TabIndex = 19;
            SelectedShowLabel.Text = "Show:";
            // 
            // CopyToOthersBtn
            // 
            CopyToOthersBtn.Location = new Point(9, 56);
            CopyToOthersBtn.Name = "CopyToOthersBtn";
            CopyToOthersBtn.Size = new Size(97, 23);
            CopyToOthersBtn.TabIndex = 18;
            CopyToOthersBtn.Text = "Copy to Others";
            CopyToOthersBtn.UseVisualStyleBackColor = true;
            CopyToOthersBtn.Click += CopyToOthersBtn_Click;
            // 
            // ApplyChangesBtn
            // 
            ApplyChangesBtn.Location = new Point(6, 722);
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
            OptionsPanel.Controls.Add(GalleryRadioButton);
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
            OptionsPanel.Location = new Point(6, 305);
            OptionsPanel.Name = "OptionsPanel";
            OptionsPanel.Size = new Size(892, 411);
            OptionsPanel.TabIndex = 16;
            // 
            // GalleryRadioButton
            // 
            GalleryRadioButton.AutoSize = true;
            GalleryRadioButton.Location = new Point(3, 341);
            GalleryRadioButton.Name = "GalleryRadioButton";
            GalleryRadioButton.Size = new Size(61, 19);
            GalleryRadioButton.TabIndex = 16;
            GalleryRadioButton.Text = "Gallery";
            GalleryRadioButton.UseVisualStyleBackColor = true;
            GalleryRadioButton.CheckedChanged += FeatureTypeRadioButton_CheckedChanged;
            // 
            // ExtraName
            // 
            ExtraName.AutoSize = true;
            ExtraName.Location = new Point(3, 369);
            ExtraName.Name = "ExtraName";
            ExtraName.Size = new Size(42, 15);
            ExtraName.TabIndex = 15;
            ExtraName.Text = "Name:";
            // 
            // ExtraNameTextBox
            // 
            ExtraNameTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            ExtraNameTextBox.BackColor = SystemColors.Window;
            ExtraNameTextBox.Enabled = false;
            ExtraNameTextBox.Location = new Point(51, 366);
            ExtraNameTextBox.Name = "ExtraNameTextBox";
            ExtraNameTextBox.Size = new Size(838, 23);
            ExtraNameTextBox.TabIndex = 12;
            ExtraNameTextBox.TextChanged += ExtraNameTextBox_TextChanged;
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
            // panel3
            // 
            panel3.Controls.Add(EpisodeRangeCheckBox);
            panel3.Controls.Add(EpisodeRangeTextBox);
            panel3.Controls.Add(EpisodeRangeLabel);
            panel3.Controls.Add(TmdbBtn);
            panel3.Controls.Add(EpisodeTextBox);
            panel3.Controls.Add(SeasonTextBox);
            panel3.Controls.Add(label4);
            panel3.Controls.Add(label3);
            panel3.Location = new Point(9, 146);
            panel3.Name = "panel3";
            panel3.Size = new Size(501, 84);
            panel3.TabIndex = 10;
            // 
            // TmdbBtn
            // 
            TmdbBtn.Location = new Point(9, 61);
            TmdbBtn.Name = "TmdbBtn";
            TmdbBtn.Size = new Size(136, 23);
            TmdbBtn.TabIndex = 19;
            TmdbBtn.Text = "Select with TMDB";
            TmdbBtn.UseVisualStyleBackColor = true;
            TmdbBtn.Click += TmdbBtn_Click;
            // 
            // EpisodeTextBox
            // 
            EpisodeTextBox.Location = new Point(66, 32);
            EpisodeTextBox.Name = "EpisodeTextBox";
            EpisodeTextBox.Size = new Size(153, 23);
            EpisodeTextBox.TabIndex = 9;
            EpisodeTextBox.TextChanged += EpisodeTextBox_TextChanged;
            // 
            // SeasonTextBox
            // 
            SeasonTextBox.Location = new Point(66, 3);
            SeasonTextBox.Name = "SeasonTextBox";
            SeasonTextBox.Size = new Size(153, 23);
            SeasonTextBox.TabIndex = 8;
            SeasonTextBox.TextChanged += SeasonTextBox_TextChanged;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(9, 35);
            label4.Name = "label4";
            label4.Size = new Size(51, 15);
            label4.TabIndex = 7;
            label4.Text = "Episode:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(9, 6);
            label3.Name = "label3";
            label3.Size = new Size(47, 15);
            label3.TabIndex = 6;
            label3.Text = "Season:";
            // 
            // EpisodeRangeTextBox
            // 
            EpisodeRangeTextBox.Location = new Point(324, 32);
            EpisodeRangeTextBox.Name = "EpisodeRangeTextBox";
            EpisodeRangeTextBox.Size = new Size(153, 23);
            EpisodeRangeTextBox.TabIndex = 21;
            // 
            // EpisodeRangeLabel
            // 
            EpisodeRangeLabel.AutoSize = true;
            EpisodeRangeLabel.Location = new Point(267, 35);
            EpisodeRangeLabel.Name = "EpisodeRangeLabel";
            EpisodeRangeLabel.Size = new Size(51, 15);
            EpisodeRangeLabel.TabIndex = 20;
            EpisodeRangeLabel.Text = "Episode:";
            // 
            // EpisodeRangeCheckBox
            // 
            EpisodeRangeCheckBox.AutoSize = true;
            EpisodeRangeCheckBox.Location = new Point(267, 5);
            EpisodeRangeCheckBox.Name = "EpisodeRangeCheckBox";
            EpisodeRangeCheckBox.Size = new Size(119, 19);
            EpisodeRangeCheckBox.TabIndex = 22;
            EpisodeRangeCheckBox.Text = "Multiple Episodes";
            EpisodeRangeCheckBox.UseVisualStyleBackColor = true;
            EpisodeRangeCheckBox.CheckedChanged += EpisodeRangeCheckBox_CheckedChanged;
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
        private ExportableListBox ExportableListBox1;
        private ImageList imageList1;
        private Panel panel1;
        private Panel panel2;
        private TextBox EpisodeTextBox;
        private TextBox SeasonTextBox;
        private Label label4;
        private Label label3;
        private Button ExportAllBtn;
        private Panel panel3;
        private Panel OptionsPanel;
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
        private Label ExtraName;
        private TextBox ExtraNameTextBox;
        private Button ExportSelectedBtn;
        private Button CopyToOthersBtn;
        private Button SelectShowBtn;
        private Label SelectedShowLabel;
        private Button ApplyChangesBtn;
        private Button TmdbBtn;
        private RadioButton GalleryRadioButton;
        private CheckBox EpisodeRangeCheckBox;
        private TextBox EpisodeRangeTextBox;
        private Label EpisodeRangeLabel;
    }
}