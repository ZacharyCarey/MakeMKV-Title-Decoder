namespace MakeMKV_Title_Decoder.Forms.FileRenamer
{
    partial class ShowEditor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
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
        private void InitializeComponent()
        {
            TmdbIdLabel = new Label();
            TmdbBtn = new Button();
            label1 = new Label();
            IdTextBox = new TextBox();
            label2 = new Label();
            TypeComboBox = new ComboBox();
            ShowNameTextBox = new TextBox();
            label5 = new Label();
            SaveBtn = new Button();
            ErrorText = new Label();
            CancelBtn = new Button();
            SuspendLayout();
            // 
            // TmdbIdLabel
            // 
            TmdbIdLabel.AutoSize = true;
            TmdbIdLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            TmdbIdLabel.Location = new Point(10, 14);
            TmdbIdLabel.Name = "TmdbIdLabel";
            TmdbIdLabel.Size = new Size(45, 15);
            TmdbIdLabel.TabIndex = 16;
            TmdbIdLabel.Text = "TMDB:";
            // 
            // TmdbBtn
            // 
            TmdbBtn.Location = new Point(61, 10);
            TmdbBtn.Name = "TmdbBtn";
            TmdbBtn.Size = new Size(75, 23);
            TmdbBtn.TabIndex = 17;
            TmdbBtn.Text = "Select";
            TmdbBtn.UseVisualStyleBackColor = true;
            TmdbBtn.Click += TmdbBtn_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(10, 71);
            label1.Name = "label1";
            label1.Size = new Size(21, 15);
            label1.TabIndex = 18;
            label1.Text = "ID:";
            // 
            // IdTextBox
            // 
            IdTextBox.Location = new Point(61, 68);
            IdTextBox.Name = "IdTextBox";
            IdTextBox.Size = new Size(153, 23);
            IdTextBox.TabIndex = 19;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(10, 42);
            label2.Name = "label2";
            label2.Size = new Size(34, 15);
            label2.TabIndex = 20;
            label2.Text = "Type:";
            // 
            // TypeComboBox
            // 
            TypeComboBox.FormattingEnabled = true;
            TypeComboBox.Location = new Point(61, 39);
            TypeComboBox.Name = "TypeComboBox";
            TypeComboBox.Size = new Size(153, 23);
            TypeComboBox.TabIndex = 21;
            // 
            // ShowNameTextBox
            // 
            ShowNameTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            ShowNameTextBox.Location = new Point(90, 117);
            ShowNameTextBox.Name = "ShowNameTextBox";
            ShowNameTextBox.Size = new Size(403, 23);
            ShowNameTextBox.TabIndex = 23;
            ShowNameTextBox.TextChanged += ShowNameTextBox_TextChanged;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(10, 120);
            label5.Name = "label5";
            label5.Size = new Size(74, 15);
            label5.TabIndex = 22;
            label5.Text = "Show Name:";
            // 
            // SaveBtn
            // 
            SaveBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            SaveBtn.Location = new Point(12, 167);
            SaveBtn.Name = "SaveBtn";
            SaveBtn.Size = new Size(75, 23);
            SaveBtn.TabIndex = 24;
            SaveBtn.Text = "Save";
            SaveBtn.UseVisualStyleBackColor = true;
            SaveBtn.Click += SaveBtn_Click;
            // 
            // ErrorText
            // 
            ErrorText.AutoSize = true;
            ErrorText.ForeColor = Color.Red;
            ErrorText.Location = new Point(12, 99);
            ErrorText.Name = "ErrorText";
            ErrorText.Size = new Size(109, 15);
            ErrorText.TabIndex = 25;
            ErrorText.Text = "error text goes here";
            ErrorText.Visible = false;
            // 
            // CancelBtn
            // 
            CancelBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            CancelBtn.Location = new Point(156, 167);
            CancelBtn.Name = "CancelBtn";
            CancelBtn.Size = new Size(75, 23);
            CancelBtn.TabIndex = 26;
            CancelBtn.Text = "Cancel";
            CancelBtn.UseVisualStyleBackColor = true;
            CancelBtn.Click += CancelBtn_Click;
            // 
            // ShowEditor
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(505, 202);
            Controls.Add(CancelBtn);
            Controls.Add(ErrorText);
            Controls.Add(SaveBtn);
            Controls.Add(TmdbIdLabel);
            Controls.Add(TmdbBtn);
            Controls.Add(label1);
            Controls.Add(IdTextBox);
            Controls.Add(label2);
            Controls.Add(TypeComboBox);
            Controls.Add(ShowNameTextBox);
            Controls.Add(label5);
            Name = "ShowEditor";
            Text = "ShowEditor";
            FormClosing += ShowEditor_FormClosing;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label TmdbIdLabel;
        private Button TmdbBtn;
        private Label label1;
        private TextBox IdTextBox;
        private Label label2;
        private ComboBox TypeComboBox;
        private TextBox ShowNameTextBox;
        private Label label5;
        private Button SaveBtn;
        private Label ErrorText;
        private Button CancelBtn;
    }
}