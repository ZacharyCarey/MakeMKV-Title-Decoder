namespace MakeMKV_Title_Decoder.Forms.FileRenamer
{
    partial class ShowSelector
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
            ShowsList = new ListBox();
            AddBtn = new Button();
            EditBtn = new Button();
            SelectBtn = new Button();
            DeleteButton = new Button();
            SuspendLayout();
            // 
            // ShowsList
            // 
            ShowsList.FormattingEnabled = true;
            ShowsList.ItemHeight = 15;
            ShowsList.Location = new Point(12, 12);
            ShowsList.Name = "ShowsList";
            ShowsList.Size = new Size(232, 424);
            ShowsList.TabIndex = 0;
            ShowsList.SelectedIndexChanged += ShowsList_SelectedIndexChanged;
            // 
            // AddBtn
            // 
            AddBtn.Location = new Point(250, 22);
            AddBtn.Name = "AddBtn";
            AddBtn.Size = new Size(75, 23);
            AddBtn.TabIndex = 1;
            AddBtn.Text = "Add";
            AddBtn.UseVisualStyleBackColor = true;
            AddBtn.Click += AddBtn_Click;
            // 
            // EditBtn
            // 
            EditBtn.Location = new Point(250, 66);
            EditBtn.Name = "EditBtn";
            EditBtn.Size = new Size(75, 23);
            EditBtn.TabIndex = 2;
            EditBtn.Text = "Edit";
            EditBtn.UseVisualStyleBackColor = true;
            EditBtn.Click += EditBtn_Click;
            // 
            // SelectBtn
            // 
            SelectBtn.Location = new Point(250, 187);
            SelectBtn.Name = "SelectBtn";
            SelectBtn.Size = new Size(75, 23);
            SelectBtn.TabIndex = 3;
            SelectBtn.Text = "Select";
            SelectBtn.UseVisualStyleBackColor = true;
            SelectBtn.Click += SelectBtn_Click;
            // 
            // DeleteButton
            // 
            DeleteButton.Location = new Point(250, 111);
            DeleteButton.Name = "DeleteButton";
            DeleteButton.Size = new Size(75, 23);
            DeleteButton.TabIndex = 4;
            DeleteButton.Text = "Delete";
            DeleteButton.UseVisualStyleBackColor = true;
            DeleteButton.Click += DeleteButton_Click;
            // 
            // ShowSelector
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(391, 450);
            Controls.Add(DeleteButton);
            Controls.Add(SelectBtn);
            Controls.Add(EditBtn);
            Controls.Add(AddBtn);
            Controls.Add(ShowsList);
            Name = "ShowSelector";
            Text = "ShowSelector";
            FormClosing += ShowSelector_FormClosing;
            ResumeLayout(false);
        }

        #endregion

        private ListBox ShowsList;
        private Button AddBtn;
        private Button EditBtn;
        private Button SelectBtn;
        private Button DeleteButton;
    }
}