namespace MakeMKV_Title_Decoder {
    partial class TitleInfo {
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
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            label7 = new Label();
            SourceFileNameLabel = new Label();
            NameLabel = new Label();
            DurationLabel = new Label();
            SizeLabel = new Label();
            SegmentsLabel = new Label();
            CommentLabel = new Label();
            FileNameLabel = new Label();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(3, 0);
            label1.Name = "label1";
            label1.Size = new Size(45, 15);
            label1.TabIndex = 0;
            label1.Text = "Name: ";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(3, 75);
            label2.Name = "label2";
            label2.Size = new Size(64, 15);
            label2.TabIndex = 1;
            label2.Text = "Comment:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(3, 15);
            label3.Name = "label3";
            label3.Size = new Size(101, 15);
            label3.TabIndex = 2;
            label3.Text = "Source file name: ";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(3, 30);
            label4.Name = "label4";
            label4.Size = new Size(56, 15);
            label4.TabIndex = 3;
            label4.Text = "Duration:";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(3, 45);
            label5.Name = "label5";
            label5.Size = new Size(30, 15);
            label5.TabIndex = 4;
            label5.Text = "Size:";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(3, 60);
            label6.Name = "label6";
            label6.Size = new Size(84, 15);
            label6.TabIndex = 5;
            label6.Text = "Segment map:";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(3, 90);
            label7.Name = "label7";
            label7.Size = new Size(61, 15);
            label7.TabIndex = 6;
            label7.Text = "File name:";
            // 
            // SourceFileNameLabel
            // 
            SourceFileNameLabel.AutoSize = true;
            SourceFileNameLabel.Location = new Point(110, 15);
            SourceFileNameLabel.Name = "SourceFileNameLabel";
            SourceFileNameLabel.Size = new Size(29, 15);
            SourceFileNameLabel.TabIndex = 7;
            SourceFileNameLabel.Text = "N/A";
            // 
            // NameLabel
            // 
            NameLabel.AutoSize = true;
            NameLabel.Location = new Point(110, 0);
            NameLabel.Name = "NameLabel";
            NameLabel.Size = new Size(29, 15);
            NameLabel.TabIndex = 8;
            NameLabel.Text = "N/A";
            // 
            // DurationLabel
            // 
            DurationLabel.AutoSize = true;
            DurationLabel.Location = new Point(110, 30);
            DurationLabel.Name = "DurationLabel";
            DurationLabel.Size = new Size(29, 15);
            DurationLabel.TabIndex = 9;
            DurationLabel.Text = "N/A";
            // 
            // SizeLabel
            // 
            SizeLabel.AutoSize = true;
            SizeLabel.Location = new Point(110, 45);
            SizeLabel.Name = "SizeLabel";
            SizeLabel.Size = new Size(29, 15);
            SizeLabel.TabIndex = 10;
            SizeLabel.Text = "N/A";
            // 
            // SegmentsLabel
            // 
            SegmentsLabel.AutoSize = true;
            SegmentsLabel.Location = new Point(110, 60);
            SegmentsLabel.Name = "SegmentsLabel";
            SegmentsLabel.Size = new Size(29, 15);
            SegmentsLabel.TabIndex = 11;
            SegmentsLabel.Text = "N/A";
            // 
            // CommentLabel
            // 
            CommentLabel.AutoSize = true;
            CommentLabel.Location = new Point(110, 75);
            CommentLabel.Name = "CommentLabel";
            CommentLabel.Size = new Size(29, 15);
            CommentLabel.TabIndex = 12;
            CommentLabel.Text = "N/A";
            // 
            // FileNameLabel
            // 
            FileNameLabel.AutoSize = true;
            FileNameLabel.Location = new Point(110, 90);
            FileNameLabel.Name = "FileNameLabel";
            FileNameLabel.Size = new Size(29, 15);
            FileNameLabel.TabIndex = 13;
            FileNameLabel.Text = "N/A";
            // 
            // TitleInfo
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(FileNameLabel);
            Controls.Add(CommentLabel);
            Controls.Add(SegmentsLabel);
            Controls.Add(SizeLabel);
            Controls.Add(DurationLabel);
            Controls.Add(NameLabel);
            Controls.Add(SourceFileNameLabel);
            Controls.Add(label7);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Name = "TitleInfo";
            Size = new Size(153, 112);
            Load += TitleInfo_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private Label label7;
        private Label SourceFileNameLabel;
        private Label NameLabel;
        private Label DurationLabel;
        private Label SizeLabel;
        private Label SegmentsLabel;
        private Label CommentLabel;
        private Label FileNameLabel;
    }
}
