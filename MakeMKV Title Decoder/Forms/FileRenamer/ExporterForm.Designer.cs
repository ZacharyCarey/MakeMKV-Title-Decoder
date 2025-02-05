namespace MakeMKV_Title_Decoder.Forms.FileRenamer {
    partial class ExporterForm {
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
            ExportOriginalCheckBox = new CheckBox();
            DVDWarningLabel = new Label();
            ExportBtn = new Button();
            label2 = new Label();
            MainFeatureLabel = new Label();
            panel1 = new Panel();
            TvEncoding480pCheckBox = new RadioButton();
            TvEncoding720pCheckBox = new RadioButton();
            TvEncoding1080pCheckBox = new RadioButton();
            TvEncodingCheckBox = new RadioButton();
            MobileEncoding480pCheckBox = new RadioButton();
            MobileEncoding720pCheckBox = new RadioButton();
            MobileEncodingCheckBox = new RadioButton();
            panel2 = new Panel();
            radioButton1 = new RadioButton();
            Export480pRadioButton = new RadioButton();
            Export720pRadioButton = new RadioButton();
            Export1080pRadioButton = new RadioButton();
            Export4kRadioButton = new RadioButton();
            ExportOriginalRadioButton = new RadioButton();
            ExtrasLabel = new Label();
            panel3 = new Panel();
            radioButton2 = new RadioButton();
            radioButton3 = new RadioButton();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            panel3.SuspendLayout();
            SuspendLayout();
            // 
            // ExportOriginalCheckBox
            // 
            ExportOriginalCheckBox.AutoSize = true;
            ExportOriginalCheckBox.Location = new Point(3, 18);
            ExportOriginalCheckBox.Name = "ExportOriginalCheckBox";
            ExportOriginalCheckBox.Size = new Size(105, 19);
            ExportOriginalCheckBox.TabIndex = 23;
            ExportOriginalCheckBox.Text = "Export Original";
            ExportOriginalCheckBox.UseVisualStyleBackColor = true;
            // 
            // DVDWarningLabel
            // 
            DVDWarningLabel.ForeColor = Color.Red;
            DVDWarningLabel.Location = new Point(12, 24);
            DVDWarningLabel.Name = "DVDWarningLabel";
            DVDWarningLabel.Size = new Size(348, 31);
            DVDWarningLabel.TabIndex = 29;
            DVDWarningLabel.Text = "Due to streaming issues with MPEG, all DVDs MUST be transcoded and can't be saved in their original format.";
            // 
            // ExportBtn
            // 
            ExportBtn.Location = new Point(12, 362);
            ExportBtn.Name = "ExportBtn";
            ExportBtn.Size = new Size(75, 23);
            ExportBtn.TabIndex = 38;
            ExportBtn.Text = "Export";
            ExportBtn.UseVisualStyleBackColor = true;
            ExportBtn.Click += ExportBtn_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 9);
            label2.Name = "label2";
            label2.Size = new Size(118, 15);
            label2.TabIndex = 43;
            label2.Text = "Transcoding settings:";
            // 
            // MainFeatureLabel
            // 
            MainFeatureLabel.AutoSize = true;
            MainFeatureLabel.Location = new Point(3, 0);
            MainFeatureLabel.Name = "MainFeatureLabel";
            MainFeatureLabel.Size = new Size(79, 15);
            MainFeatureLabel.TabIndex = 44;
            MainFeatureLabel.Text = "Main Feature:";
            // 
            // panel1
            // 
            panel1.Controls.Add(radioButton3);
            panel1.Controls.Add(TvEncoding480pCheckBox);
            panel1.Controls.Add(TvEncoding720pCheckBox);
            panel1.Controls.Add(TvEncoding1080pCheckBox);
            panel1.Controls.Add(TvEncodingCheckBox);
            panel1.Controls.Add(MainFeatureLabel);
            panel1.Controls.Add(ExportOriginalCheckBox);
            panel1.Location = new Point(12, 58);
            panel1.Name = "panel1";
            panel1.Size = new Size(219, 164);
            panel1.TabIndex = 45;
            // 
            // TvEncoding480pCheckBox
            // 
            TvEncoding480pCheckBox.AutoSize = true;
            TvEncoding480pCheckBox.Location = new Point(3, 117);
            TvEncoding480pCheckBox.Name = "TvEncoding480pCheckBox";
            TvEncoding480pCheckBox.Size = new Size(134, 19);
            TvEncoding480pCheckBox.TabIndex = 49;
            TvEncoding480pCheckBox.Text = "TV (480p, AV1, EAC3)";
            TvEncoding480pCheckBox.UseVisualStyleBackColor = true;
            // 
            // TvEncoding720pCheckBox
            // 
            TvEncoding720pCheckBox.AutoSize = true;
            TvEncoding720pCheckBox.Location = new Point(3, 93);
            TvEncoding720pCheckBox.Name = "TvEncoding720pCheckBox";
            TvEncoding720pCheckBox.Size = new Size(134, 19);
            TvEncoding720pCheckBox.TabIndex = 48;
            TvEncoding720pCheckBox.Text = "TV (720p, AV1, EAC3)";
            TvEncoding720pCheckBox.UseVisualStyleBackColor = true;
            // 
            // TvEncoding1080pCheckBox
            // 
            TvEncoding1080pCheckBox.AutoSize = true;
            TvEncoding1080pCheckBox.Location = new Point(3, 68);
            TvEncoding1080pCheckBox.Name = "TvEncoding1080pCheckBox";
            TvEncoding1080pCheckBox.Size = new Size(140, 19);
            TvEncoding1080pCheckBox.TabIndex = 47;
            TvEncoding1080pCheckBox.Text = "TV (1080p, AV1, EAC3)";
            TvEncoding1080pCheckBox.UseVisualStyleBackColor = true;
            // 
            // TvEncodingCheckBox
            // 
            TvEncodingCheckBox.AutoSize = true;
            TvEncodingCheckBox.Location = new Point(3, 43);
            TvEncodingCheckBox.Name = "TvEncodingCheckBox";
            TvEncodingCheckBox.Size = new Size(121, 19);
            TvEncodingCheckBox.TabIndex = 45;
            TvEncodingCheckBox.Text = "TV (4k, AV1, EAC3)";
            TvEncodingCheckBox.UseVisualStyleBackColor = true;
            // 
            // MobileEncoding480pCheckBox
            // 
            MobileEncoding480pCheckBox.AutoSize = true;
            MobileEncoding480pCheckBox.Location = new Point(3, 53);
            MobileEncoding480pCheckBox.Name = "MobileEncoding480pCheckBox";
            MobileEncoding480pCheckBox.Size = new Size(199, 19);
            MobileEncoding480pCheckBox.TabIndex = 51;
            MobileEncoding480pCheckBox.Text = "Mobile (480p, AV1, AAC, 2 Mbps)";
            MobileEncoding480pCheckBox.UseVisualStyleBackColor = true;
            // 
            // MobileEncoding720pCheckBox
            // 
            MobileEncoding720pCheckBox.AutoSize = true;
            MobileEncoding720pCheckBox.Location = new Point(3, 28);
            MobileEncoding720pCheckBox.Name = "MobileEncoding720pCheckBox";
            MobileEncoding720pCheckBox.Size = new Size(199, 19);
            MobileEncoding720pCheckBox.TabIndex = 50;
            MobileEncoding720pCheckBox.Text = "Mobile (720p, AV1, AAC, 2 Mbps)";
            MobileEncoding720pCheckBox.UseVisualStyleBackColor = true;
            // 
            // MobileEncodingCheckBox
            // 
            MobileEncodingCheckBox.AutoSize = true;
            MobileEncodingCheckBox.Location = new Point(3, 3);
            MobileEncodingCheckBox.Name = "MobileEncodingCheckBox";
            MobileEncodingCheckBox.Size = new Size(205, 19);
            MobileEncodingCheckBox.TabIndex = 46;
            MobileEncodingCheckBox.Text = "Mobile (1080p, AV1, AAC, 2 Mbps)";
            MobileEncodingCheckBox.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            panel2.Controls.Add(radioButton1);
            panel2.Controls.Add(Export480pRadioButton);
            panel2.Controls.Add(Export720pRadioButton);
            panel2.Controls.Add(Export1080pRadioButton);
            panel2.Controls.Add(Export4kRadioButton);
            panel2.Controls.Add(ExportOriginalRadioButton);
            panel2.Controls.Add(ExtrasLabel);
            panel2.Location = new Point(237, 58);
            panel2.Name = "panel2";
            panel2.Size = new Size(123, 164);
            panel2.TabIndex = 46;
            // 
            // radioButton1
            // 
            radioButton1.AutoSize = true;
            radioButton1.Checked = true;
            radioButton1.Location = new Point(3, 142);
            radioButton1.Name = "radioButton1";
            radioButton1.Size = new Size(54, 19);
            radioButton1.TabIndex = 50;
            radioButton1.TabStop = true;
            radioButton1.Text = "None";
            radioButton1.UseVisualStyleBackColor = true;
            // 
            // Export480pRadioButton
            // 
            Export480pRadioButton.AutoSize = true;
            Export480pRadioButton.Location = new Point(3, 117);
            Export480pRadioButton.Name = "Export480pRadioButton";
            Export480pRadioButton.Size = new Size(87, 19);
            Export480pRadioButton.TabIndex = 49;
            Export480pRadioButton.Text = "Export 480p";
            Export480pRadioButton.UseVisualStyleBackColor = true;
            // 
            // Export720pRadioButton
            // 
            Export720pRadioButton.AutoSize = true;
            Export720pRadioButton.Location = new Point(3, 93);
            Export720pRadioButton.Name = "Export720pRadioButton";
            Export720pRadioButton.Size = new Size(87, 19);
            Export720pRadioButton.TabIndex = 48;
            Export720pRadioButton.Text = "Export 720p";
            Export720pRadioButton.UseVisualStyleBackColor = true;
            // 
            // Export1080pRadioButton
            // 
            Export1080pRadioButton.AutoSize = true;
            Export1080pRadioButton.Location = new Point(3, 67);
            Export1080pRadioButton.Name = "Export1080pRadioButton";
            Export1080pRadioButton.Size = new Size(93, 19);
            Export1080pRadioButton.TabIndex = 47;
            Export1080pRadioButton.Text = "Export 1080p";
            Export1080pRadioButton.UseVisualStyleBackColor = true;
            // 
            // Export4kRadioButton
            // 
            Export4kRadioButton.AutoSize = true;
            Export4kRadioButton.Location = new Point(3, 43);
            Export4kRadioButton.Name = "Export4kRadioButton";
            Export4kRadioButton.Size = new Size(74, 19);
            Export4kRadioButton.TabIndex = 46;
            Export4kRadioButton.Text = "Export 4k";
            Export4kRadioButton.UseVisualStyleBackColor = true;
            // 
            // ExportOriginalRadioButton
            // 
            ExportOriginalRadioButton.AutoSize = true;
            ExportOriginalRadioButton.Location = new Point(3, 18);
            ExportOriginalRadioButton.Name = "ExportOriginalRadioButton";
            ExportOriginalRadioButton.Size = new Size(104, 19);
            ExportOriginalRadioButton.TabIndex = 45;
            ExportOriginalRadioButton.Text = "Export Original";
            ExportOriginalRadioButton.UseVisualStyleBackColor = true;
            // 
            // ExtrasLabel
            // 
            ExtrasLabel.AutoSize = true;
            ExtrasLabel.Location = new Point(3, 0);
            ExtrasLabel.Name = "ExtrasLabel";
            ExtrasLabel.Size = new Size(41, 15);
            ExtrasLabel.TabIndex = 44;
            ExtrasLabel.Text = "Extras:";
            // 
            // panel3
            // 
            panel3.Controls.Add(radioButton2);
            panel3.Controls.Add(MobileEncoding480pCheckBox);
            panel3.Controls.Add(MobileEncodingCheckBox);
            panel3.Controls.Add(MobileEncoding720pCheckBox);
            panel3.Location = new Point(12, 243);
            panel3.Name = "panel3";
            panel3.Size = new Size(219, 103);
            panel3.TabIndex = 47;
            // 
            // radioButton2
            // 
            radioButton2.AutoSize = true;
            radioButton2.Checked = true;
            radioButton2.Location = new Point(3, 78);
            radioButton2.Name = "radioButton2";
            radioButton2.Size = new Size(54, 19);
            radioButton2.TabIndex = 52;
            radioButton2.TabStop = true;
            radioButton2.Text = "None";
            radioButton2.UseVisualStyleBackColor = true;
            // 
            // radioButton3
            // 
            radioButton3.AutoSize = true;
            radioButton3.Checked = true;
            radioButton3.Location = new Point(3, 142);
            radioButton3.Name = "radioButton3";
            radioButton3.Size = new Size(54, 19);
            radioButton3.TabIndex = 51;
            radioButton3.TabStop = true;
            radioButton3.Text = "None";
            radioButton3.UseVisualStyleBackColor = true;
            // 
            // ExporterForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(452, 402);
            Controls.Add(panel3);
            Controls.Add(panel2);
            Controls.Add(panel1);
            Controls.Add(label2);
            Controls.Add(ExportBtn);
            Controls.Add(DVDWarningLabel);
            Name = "ExporterForm";
            Text = "ExporterForm";
            Load += ExporterForm_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private CheckBox ExportOriginalCheckBox;
        private Label DVDWarningLabel;
        private Button ExportBtn;
        private Label label2;
        private Label MainFeatureLabel;
        private Panel panel1;
        private Panel panel2;
        private Label ExtrasLabel;
        private RadioButton Export480pRadioButton;
        private RadioButton Export720pRadioButton;
        private RadioButton Export1080pRadioButton;
        private RadioButton Export4kRadioButton;
        private RadioButton ExportOriginalRadioButton;
        private RadioButton radioButton1;
        private RadioButton TvEncodingCheckBox;
        private RadioButton MobileEncodingCheckBox;
        private CheckBox checkBox1;
        private CheckBox checkBox4;
        private RadioButton MobileEncoding720pCheckBox;
        private RadioButton TvEncoding480pCheckBox;
        private RadioButton TvEncoding720pCheckBox;
        private RadioButton TvEncoding1080pCheckBox;
        private RadioButton MobileEncoding480pCheckBox;
        private Panel panel3;
        private RadioButton radioButton3;
        private RadioButton radioButton2;
    }
}