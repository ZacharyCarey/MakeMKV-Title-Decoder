namespace MakeMKV_Title_Decoder.Forms {
    partial class FrameViewer {
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
            FramePictureBox = new PictureBox();
            PrevBtn = new Button();
            NextBtn = new Button();
            FrameLabel = new Label();
            panel1 = new Panel();
            ExportSingleBtn = new Button();
            ExportAllBtn = new Button();
            ((System.ComponentModel.ISupportInitialize)FramePictureBox).BeginInit();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // FramePictureBox
            // 
            FramePictureBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            FramePictureBox.Location = new Point(12, 12);
            FramePictureBox.Name = "FramePictureBox";
            FramePictureBox.Size = new Size(940, 457);
            FramePictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            FramePictureBox.TabIndex = 0;
            FramePictureBox.TabStop = false;
            // 
            // PrevBtn
            // 
            PrevBtn.Location = new Point(3, 10);
            PrevBtn.Name = "PrevBtn";
            PrevBtn.Size = new Size(75, 23);
            PrevBtn.TabIndex = 1;
            PrevBtn.Text = "<---";
            PrevBtn.UseVisualStyleBackColor = true;
            PrevBtn.Click += PrevBtn_Click;
            // 
            // NextBtn
            // 
            NextBtn.Location = new Point(289, 8);
            NextBtn.Name = "NextBtn";
            NextBtn.Size = new Size(75, 23);
            NextBtn.TabIndex = 2;
            NextBtn.Text = "--->";
            NextBtn.UseVisualStyleBackColor = true;
            NextBtn.Click += NextBtn_Click;
            // 
            // FrameLabel
            // 
            FrameLabel.Location = new Point(84, 12);
            FrameLabel.Name = "FrameLabel";
            FrameLabel.Size = new Size(199, 19);
            FrameLabel.TabIndex = 3;
            FrameLabel.Text = "Frame 4,294,967,295/4,294,967,295";
            FrameLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // panel1
            // 
            panel1.Anchor = AnchorStyles.Bottom;
            panel1.Controls.Add(ExportAllBtn);
            panel1.Controls.Add(ExportSingleBtn);
            panel1.Controls.Add(NextBtn);
            panel1.Controls.Add(PrevBtn);
            panel1.Controls.Add(FrameLabel);
            panel1.Location = new Point(297, 475);
            panel1.Name = "panel1";
            panel1.Size = new Size(371, 70);
            panel1.TabIndex = 4;
            // 
            // ExportSingleBtn
            // 
            ExportSingleBtn.Location = new Point(81, 39);
            ExportSingleBtn.Name = "ExportSingleBtn";
            ExportSingleBtn.Size = new Size(98, 23);
            ExportSingleBtn.TabIndex = 4;
            ExportSingleBtn.Text = "Export Single";
            ExportSingleBtn.UseVisualStyleBackColor = true;
            ExportSingleBtn.Click += ExportSingleBtn_Click;
            // 
            // ExportAllBtn
            // 
            ExportAllBtn.Location = new Point(185, 39);
            ExportAllBtn.Name = "ExportAllBtn";
            ExportAllBtn.Size = new Size(98, 23);
            ExportAllBtn.TabIndex = 5;
            ExportAllBtn.Text = "Export All";
            ExportAllBtn.UseVisualStyleBackColor = true;
            ExportAllBtn.Click += ExportAllBtn_Click;
            // 
            // FrameViewer
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(964, 546);
            Controls.Add(panel1);
            Controls.Add(FramePictureBox);
            Name = "FrameViewer";
            Text = "FrameViewer";
            FormClosing += FrameViewer_FormClosing;
            Load += FrameViewer_Load;
            ((System.ComponentModel.ISupportInitialize)FramePictureBox).EndInit();
            panel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private PictureBox FramePictureBox;
        private Button PrevBtn;
        private Button NextBtn;
        private Label FrameLabel;
        private Panel panel1;
        private Button ExportAllBtn;
        private Button ExportSingleBtn;
    }
}