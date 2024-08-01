namespace MakeMKV_Title_Decoder {
    partial class FolderManager {
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
            FolderTreeView = new TreeView();
            SelectBtn = new Button();
            AddBtn = new Button();
            NewFolderNameTextBox = new TextBox();
            DeleteBtn = new Button();
            label1 = new Label();
            InvalidNameLabel = new Label();
            SuspendLayout();
            // 
            // FolderTreeView
            // 
            FolderTreeView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            FolderTreeView.HideSelection = false;
            FolderTreeView.Location = new Point(12, 12);
            FolderTreeView.Name = "FolderTreeView";
            FolderTreeView.Size = new Size(315, 426);
            FolderTreeView.TabIndex = 0;
            FolderTreeView.AfterSelect += FolderTreeView_AfterSelect;
            // 
            // SelectBtn
            // 
            SelectBtn.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            SelectBtn.Enabled = false;
            SelectBtn.Location = new Point(333, 166);
            SelectBtn.Name = "SelectBtn";
            SelectBtn.Size = new Size(75, 23);
            SelectBtn.TabIndex = 1;
            SelectBtn.Text = "Select";
            SelectBtn.UseVisualStyleBackColor = true;
            SelectBtn.Click += SelectBtn_Click;
            // 
            // AddBtn
            // 
            AddBtn.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            AddBtn.Enabled = false;
            AddBtn.Location = new Point(333, 12);
            AddBtn.Name = "AddBtn";
            AddBtn.Size = new Size(75, 23);
            AddBtn.TabIndex = 2;
            AddBtn.Text = "Add";
            AddBtn.UseVisualStyleBackColor = true;
            AddBtn.Click += AddBtn_Click;
            // 
            // NewFolderNameTextBox
            // 
            NewFolderNameTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            NewFolderNameTextBox.Location = new Point(381, 41);
            NewFolderNameTextBox.Name = "NewFolderNameTextBox";
            NewFolderNameTextBox.Size = new Size(216, 23);
            NewFolderNameTextBox.TabIndex = 3;
            NewFolderNameTextBox.TextChanged += NewFolderNameTextBox_TextChanged;
            // 
            // DeleteBtn
            // 
            DeleteBtn.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            DeleteBtn.Enabled = false;
            DeleteBtn.Location = new Point(333, 97);
            DeleteBtn.Name = "DeleteBtn";
            DeleteBtn.Size = new Size(75, 23);
            DeleteBtn.TabIndex = 4;
            DeleteBtn.Text = "Delete";
            DeleteBtn.UseVisualStyleBackColor = true;
            DeleteBtn.Click += DeleteBtn_Click;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label1.AutoSize = true;
            label1.Location = new Point(333, 44);
            label1.Name = "label1";
            label1.Size = new Size(42, 15);
            label1.TabIndex = 5;
            label1.Text = "Name:";
            // 
            // InvalidNameLabel
            // 
            InvalidNameLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            InvalidNameLabel.AutoSize = true;
            InvalidNameLabel.ForeColor = Color.Red;
            InvalidNameLabel.Location = new Point(500, 67);
            InvalidNameLabel.Name = "InvalidNameLabel";
            InvalidNameLabel.Size = new Size(97, 15);
            InvalidNameLabel.TabIndex = 8;
            InvalidNameLabel.Text = "Invalid file name!";
            InvalidNameLabel.Visible = false;
            // 
            // FolderManager
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(610, 450);
            Controls.Add(InvalidNameLabel);
            Controls.Add(label1);
            Controls.Add(DeleteBtn);
            Controls.Add(NewFolderNameTextBox);
            Controls.Add(AddBtn);
            Controls.Add(SelectBtn);
            Controls.Add(FolderTreeView);
            Name = "FolderManager";
            Text = "FolderManager";
            FormClosing += FolderManager_FormClosing;
            Load += FolderManager_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TreeView FolderTreeView;
        private Button SelectBtn;
        private Button AddBtn;
        private TextBox NewFolderNameTextBox;
        private Button DeleteBtn;
        private Label label1;
        private Label InvalidNameLabel;
    }
}