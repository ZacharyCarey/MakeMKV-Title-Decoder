namespace MakeMKV_Title_Decoder {
    partial class Form1 {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            components = new System.ComponentModel.Container();
            panel1 = new Panel();
            BonusFeaturesRadioBtn = new RadioButton();
            TvRadioBtn = new RadioButton();
            MovieRadioBtn = new RadioButton();
            toolTip1 = new ToolTip(components);
            IgnoreIncompleteCheckBox = new CheckBox();
            ScrapeBtn = new Button();
            IncludeAttachmentsCheckBox = new CheckBox();
            RenameBtn = new Button();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(BonusFeaturesRadioBtn);
            panel1.Controls.Add(TvRadioBtn);
            panel1.Controls.Add(MovieRadioBtn);
            panel1.Location = new Point(12, 12);
            panel1.Name = "panel1";
            panel1.Size = new Size(186, 75);
            panel1.TabIndex = 0;
            // 
            // BonusFeaturesRadioBtn
            // 
            BonusFeaturesRadioBtn.AutoSize = true;
            BonusFeaturesRadioBtn.Location = new Point(3, 53);
            BonusFeaturesRadioBtn.Name = "BonusFeaturesRadioBtn";
            BonusFeaturesRadioBtn.Size = new Size(105, 19);
            BonusFeaturesRadioBtn.TabIndex = 2;
            BonusFeaturesRadioBtn.Text = "Bonus Features";
            toolTip1.SetToolTip(BonusFeaturesRadioBtn, "Dedicated bonus features disc. Will separate all tracks instead of looking for a main feature.");
            BonusFeaturesRadioBtn.UseVisualStyleBackColor = true;
            // 
            // TvRadioBtn
            // 
            TvRadioBtn.AutoSize = true;
            TvRadioBtn.Location = new Point(3, 28);
            TvRadioBtn.Name = "TvRadioBtn";
            TvRadioBtn.Size = new Size(38, 19);
            TvRadioBtn.TabIndex = 1;
            TvRadioBtn.Text = "TV";
            toolTip1.SetToolTip(TvRadioBtn, "TV prefers individual episodes for the main feature");
            TvRadioBtn.UseVisualStyleBackColor = true;
            // 
            // MovieRadioBtn
            // 
            MovieRadioBtn.AutoSize = true;
            MovieRadioBtn.Checked = true;
            MovieRadioBtn.Location = new Point(3, 3);
            MovieRadioBtn.Name = "MovieRadioBtn";
            MovieRadioBtn.Size = new Size(58, 19);
            MovieRadioBtn.TabIndex = 0;
            MovieRadioBtn.TabStop = true;
            MovieRadioBtn.Text = "Movie";
            toolTip1.SetToolTip(MovieRadioBtn, "Movies prefer the single playlist for the main feature");
            MovieRadioBtn.UseVisualStyleBackColor = true;
            // 
            // IgnoreIncompleteCheckBox
            // 
            IgnoreIncompleteCheckBox.AutoSize = true;
            IgnoreIncompleteCheckBox.Checked = true;
            IgnoreIncompleteCheckBox.CheckState = CheckState.Checked;
            IgnoreIncompleteCheckBox.Location = new Point(15, 118);
            IgnoreIncompleteCheckBox.Name = "IgnoreIncompleteCheckBox";
            IgnoreIncompleteCheckBox.Size = new Size(155, 19);
            IgnoreIncompleteCheckBox.TabIndex = 4;
            IgnoreIncompleteCheckBox.Text = "Ignore incomplete video";
            toolTip1.SetToolTip(IgnoreIncompleteCheckBox, "If a stream contains only video or only audio it will be ignored");
            IgnoreIncompleteCheckBox.UseVisualStyleBackColor = true;
            // 
            // ScrapeBtn
            // 
            ScrapeBtn.Location = new Point(12, 154);
            ScrapeBtn.Name = "ScrapeBtn";
            ScrapeBtn.Size = new Size(75, 23);
            ScrapeBtn.TabIndex = 1;
            ScrapeBtn.Text = "Scrape";
            ScrapeBtn.UseVisualStyleBackColor = true;
            ScrapeBtn.Click += ScrapeBtn_Click;
            // 
            // IncludeAttachmentsCheckBox
            // 
            IncludeAttachmentsCheckBox.AutoSize = true;
            IncludeAttachmentsCheckBox.Checked = true;
            IncludeAttachmentsCheckBox.CheckState = CheckState.Checked;
            IncludeAttachmentsCheckBox.Location = new Point(15, 93);
            IncludeAttachmentsCheckBox.Name = "IncludeAttachmentsCheckBox";
            IncludeAttachmentsCheckBox.Size = new Size(136, 19);
            IncludeAttachmentsCheckBox.TabIndex = 2;
            IncludeAttachmentsCheckBox.Text = "Include Attachments";
            IncludeAttachmentsCheckBox.UseVisualStyleBackColor = true;
            // 
            // RenameBtn
            // 
            RenameBtn.Location = new Point(123, 154);
            RenameBtn.Name = "RenameBtn";
            RenameBtn.Size = new Size(75, 23);
            RenameBtn.TabIndex = 3;
            RenameBtn.Text = "Rename";
            RenameBtn.UseVisualStyleBackColor = true;
            RenameBtn.Click += RenameBtn_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(435, 219);
            Controls.Add(IgnoreIncompleteCheckBox);
            Controls.Add(RenameBtn);
            Controls.Add(IncludeAttachmentsCheckBox);
            Controls.Add(ScrapeBtn);
            Controls.Add(panel1);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel panel1;
        private RadioButton TvRadioBtn;
        private RadioButton MovieRadioBtn;
        private ToolTip toolTip1;
        private Button ScrapeBtn;
        private RadioButton BonusFeaturesRadioBtn;
        private CheckBox IncludeAttachmentsCheckBox;
        private Button RenameBtn;
        private CheckBox IgnoreIncompleteCheckBox;
    }
}
