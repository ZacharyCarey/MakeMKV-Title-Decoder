namespace MakeMKV_Title_Decoder.Forms.DiscBackup {
    partial class DiscBakupForm {
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
            DownloadBtn = new Button();
            TotalProgressBar = new ProgressBar();
            CurrentProgressBar = new ProgressBar();
            DiscNameLabel = new Label();
            label3 = new Label();
            DrivesComboBox = new ComboBox();
            RefreshDrivesBtn = new Button();
            SuspendLayout();
            // 
            // DownloadBtn
            // 
            DownloadBtn.Enabled = false;
            DownloadBtn.Location = new Point(12, 81);
            DownloadBtn.Name = "DownloadBtn";
            DownloadBtn.Size = new Size(75, 23);
            DownloadBtn.TabIndex = 5;
            DownloadBtn.Text = "Backup";
            DownloadBtn.UseVisualStyleBackColor = true;
            DownloadBtn.Click += DownloadBtn_Click;
            // 
            // TotalProgressBar
            // 
            TotalProgressBar.Location = new Point(12, 139);
            TotalProgressBar.Maximum = 65536;
            TotalProgressBar.Name = "TotalProgressBar";
            TotalProgressBar.Size = new Size(515, 23);
            TotalProgressBar.TabIndex = 6;
            // 
            // CurrentProgressBar
            // 
            CurrentProgressBar.Location = new Point(12, 110);
            CurrentProgressBar.Maximum = 65536;
            CurrentProgressBar.Name = "CurrentProgressBar";
            CurrentProgressBar.Size = new Size(515, 23);
            CurrentProgressBar.TabIndex = 4;
            // 
            // DiscNameLabel
            // 
            DiscNameLabel.AutoSize = true;
            DiscNameLabel.Location = new Point(86, 39);
            DiscNameLabel.Name = "DiscNameLabel";
            DiscNameLabel.Size = new Size(29, 15);
            DiscNameLabel.TabIndex = 3;
            DiscNameLabel.Text = "N/A";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 38);
            label3.Name = "label3";
            label3.Size = new Size(68, 15);
            label3.TabIndex = 2;
            label3.Text = "Disc name: ";
            // 
            // DrivesComboBox
            // 
            DrivesComboBox.FormattingEnabled = true;
            DrivesComboBox.Location = new Point(93, 13);
            DrivesComboBox.Name = "DrivesComboBox";
            DrivesComboBox.Size = new Size(309, 23);
            DrivesComboBox.TabIndex = 1;
            DrivesComboBox.SelectedIndexChanged += DrivesComboBox_SelectedIndexChanged;
            // 
            // RefreshDrivesBtn
            // 
            RefreshDrivesBtn.Location = new Point(12, 12);
            RefreshDrivesBtn.Name = "RefreshDrivesBtn";
            RefreshDrivesBtn.Size = new Size(75, 23);
            RefreshDrivesBtn.TabIndex = 0;
            RefreshDrivesBtn.Text = "Refresh";
            RefreshDrivesBtn.UseVisualStyleBackColor = true;
            RefreshDrivesBtn.Click += RefreshDrivesBtn_Click;
            // 
            // DiscBakupForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(549, 178);
            Controls.Add(DownloadBtn);
            Controls.Add(TotalProgressBar);
            Controls.Add(RefreshDrivesBtn);
            Controls.Add(CurrentProgressBar);
            Controls.Add(DrivesComboBox);
            Controls.Add(DiscNameLabel);
            Controls.Add(label3);
            Name = "DiscBakupForm";
            Text = "DiscBakupForm";
            Load += DiscBakupForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button DownloadBtn;
        private ProgressBar TotalProgressBar;
        private ProgressBar CurrentProgressBar;
        private Label DiscNameLabel;
        private Label label3;
        private ComboBox DrivesComboBox;
        private Button RefreshDrivesBtn;
    }
}