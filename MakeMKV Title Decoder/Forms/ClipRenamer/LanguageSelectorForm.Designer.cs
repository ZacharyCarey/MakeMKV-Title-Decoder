namespace MakeMKV_Title_Decoder.Forms.ClipRenamer
{
    partial class LanguageSelectorForm
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
            LanguageList = new ListBox();
            SelectBtn = new Button();
            SearchTextBox = new TextBox();
            SuspendLayout();
            // 
            // LanguageList
            // 
            LanguageList.FormattingEnabled = true;
            LanguageList.ItemHeight = 15;
            LanguageList.Location = new Point(12, 42);
            LanguageList.Name = "LanguageList";
            LanguageList.Size = new Size(254, 409);
            LanguageList.TabIndex = 0;
            // 
            // SelectBtn
            // 
            SelectBtn.Location = new Point(283, 12);
            SelectBtn.Name = "SelectBtn";
            SelectBtn.Size = new Size(75, 23);
            SelectBtn.TabIndex = 1;
            SelectBtn.Text = "Select";
            SelectBtn.UseVisualStyleBackColor = true;
            SelectBtn.Click += SelectBtn_Click;
            // 
            // SearchTextBox
            // 
            SearchTextBox.Location = new Point(12, 12);
            SearchTextBox.Name = "SearchTextBox";
            SearchTextBox.Size = new Size(254, 23);
            SearchTextBox.TabIndex = 2;
            SearchTextBox.TextChanged += SearchTextBox_TextChanged;
            // 
            // LanguageSelectorForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(370, 469);
            Controls.Add(SearchTextBox);
            Controls.Add(SelectBtn);
            Controls.Add(LanguageList);
            Name = "LanguageSelectorForm";
            Text = "LanguageSelectorForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListBox LanguageList;
        private Button SelectBtn;
        private TextBox SearchTextBox;
    }
}