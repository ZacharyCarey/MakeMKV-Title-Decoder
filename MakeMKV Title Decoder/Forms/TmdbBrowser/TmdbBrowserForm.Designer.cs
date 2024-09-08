namespace MakeMKV_Title_Decoder.Forms.TmdbBrowser {
    partial class TmdbBrowserForm {
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
            panel1 = new Panel();
            SelectButton = new Button();
            WebView1 = new Microsoft.Web.WebView2.WinForms.WebView2();
            IdLabel = new Label();
            TypeLabel = new Label();
            SeasonLabel = new Label();
            EpisodeLabel = new Label();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)WebView1).BeginInit();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(EpisodeLabel);
            panel1.Controls.Add(SeasonLabel);
            panel1.Controls.Add(TypeLabel);
            panel1.Controls.Add(IdLabel);
            panel1.Controls.Add(SelectButton);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(1178, 30);
            panel1.TabIndex = 0;
            // 
            // SelectButton
            // 
            SelectButton.Location = new Point(12, 4);
            SelectButton.Name = "SelectButton";
            SelectButton.Size = new Size(75, 23);
            SelectButton.TabIndex = 0;
            SelectButton.Text = "Select";
            SelectButton.UseVisualStyleBackColor = true;
            SelectButton.Click += SelectButton_Click;
            // 
            // WebView1
            // 
            WebView1.AllowExternalDrop = true;
            WebView1.CreationProperties = null;
            WebView1.DefaultBackgroundColor = Color.White;
            WebView1.Dock = DockStyle.Fill;
            WebView1.Location = new Point(0, 30);
            WebView1.Name = "WebView1";
            WebView1.Size = new Size(1178, 693);
            WebView1.TabIndex = 1;
            WebView1.ZoomFactor = 1D;
            WebView1.SourceChanged += WebView1_SourceChanged;
            // 
            // IdLabel
            // 
            IdLabel.AutoSize = true;
            IdLabel.Location = new Point(112, 9);
            IdLabel.Name = "IdLabel";
            IdLabel.Size = new Size(21, 15);
            IdLabel.TabIndex = 1;
            IdLabel.Text = "ID:";
            // 
            // TypeLabel
            // 
            TypeLabel.AutoSize = true;
            TypeLabel.Location = new Point(192, 8);
            TypeLabel.Name = "TypeLabel";
            TypeLabel.Size = new Size(34, 15);
            TypeLabel.TabIndex = 2;
            TypeLabel.Text = "Type:";
            // 
            // SeasonLabel
            // 
            SeasonLabel.AutoSize = true;
            SeasonLabel.Location = new Point(308, 8);
            SeasonLabel.Name = "SeasonLabel";
            SeasonLabel.Size = new Size(47, 15);
            SeasonLabel.TabIndex = 3;
            SeasonLabel.Text = "Season:";
            // 
            // EpisodeLabel
            // 
            EpisodeLabel.AutoSize = true;
            EpisodeLabel.Location = new Point(416, 9);
            EpisodeLabel.Name = "EpisodeLabel";
            EpisodeLabel.Size = new Size(48, 15);
            EpisodeLabel.TabIndex = 4;
            EpisodeLabel.Text = "Episode";
            // 
            // TmdbBrowserForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1178, 723);
            Controls.Add(WebView1);
            Controls.Add(panel1);
            Name = "TmdbBrowserForm";
            Text = "TmdbBrowserForm";
            Load += TmdbBrowserForm_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)WebView1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Button SelectButton;
        private Microsoft.Web.WebView2.WinForms.WebView2 WebView1;
        private Label EpisodeLabel;
        private Label SeasonLabel;
        private Label TypeLabel;
        private Label IdLabel;
    }
}