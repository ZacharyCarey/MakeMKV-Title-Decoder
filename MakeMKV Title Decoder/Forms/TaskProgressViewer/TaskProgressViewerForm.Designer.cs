namespace MakeMKV_Title_Decoder {
    partial class TaskProgressViewerForm {
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
            CurrentProgress = new ProgressBar();
            TotalProgress = new ProgressBar();
            SuspendLayout();
            // 
            // CurrentProgress
            // 
            CurrentProgress.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            CurrentProgress.Location = new Point(12, 12);
            CurrentProgress.Name = "CurrentProgress";
            CurrentProgress.Size = new Size(631, 23);
            CurrentProgress.TabIndex = 0;
            // 
            // TotalProgress
            // 
            TotalProgress.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            TotalProgress.Location = new Point(12, 41);
            TotalProgress.Name = "TotalProgress";
            TotalProgress.Size = new Size(631, 23);
            TotalProgress.TabIndex = 1;
            // 
            // TaskProgress
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(655, 83);
            Controls.Add(TotalProgress);
            Controls.Add(CurrentProgress);
            Name = "TaskProgress";
            Text = "TaskProgress";
            Load += TaskProgress_Load;
            ResumeLayout(false);
        }

        #endregion

        private ProgressBar CurrentProgress;
        private ProgressBar TotalProgress;
    }
}