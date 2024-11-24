namespace MakeMKV_Title_Decoder.Forms
{
    partial class CollectionsForm
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
            splitContainer1 = new SplitContainer();
            CollectionsListBox = new ListBox();
            panel2 = new Panel();
            NewPlaylistButton = new Button();
            label1 = new Label();
            PlaylistPropertiesPanel = new Panel();
            DeleteCollectionButton = new Button();
            label2 = new Label();
            NameTextBox = new TextBox();
            ApplyNameButton = new Button();
            splitContainer2 = new SplitContainer();
            AttachmentsListBox = new ListBox();
            panel1 = new Panel();
            AttachmentDeleteBtn = new Button();
            SourceList = new ListBox();
            SourcePropertiesPanel = new Panel();
            SourceAddButton = new Button();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            panel2.SuspendLayout();
            PlaylistPropertiesPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
            splitContainer2.Panel1.SuspendLayout();
            splitContainer2.Panel2.SuspendLayout();
            splitContainer2.SuspendLayout();
            panel1.SuspendLayout();
            SourcePropertiesPanel.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainer1
            // 
            splitContainer1.BorderStyle = BorderStyle.Fixed3D;
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(CollectionsListBox);
            splitContainer1.Panel1.Controls.Add(panel2);
            splitContainer1.Panel1.Controls.Add(PlaylistPropertiesPanel);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(splitContainer2);
            splitContainer1.Size = new Size(1240, 942);
            splitContainer1.SplitterDistance = 370;
            splitContainer1.TabIndex = 1;
            // 
            // CollectionsListBox
            // 
            CollectionsListBox.Dock = DockStyle.Fill;
            CollectionsListBox.FormattingEnabled = true;
            CollectionsListBox.ItemHeight = 15;
            CollectionsListBox.Location = new Point(0, 31);
            CollectionsListBox.Name = "CollectionsListBox";
            CollectionsListBox.Size = new Size(366, 839);
            CollectionsListBox.TabIndex = 3;
            CollectionsListBox.SelectedIndexChanged += CollectionsListBox_SelectedIndexChanged;
            // 
            // panel2
            // 
            panel2.Controls.Add(NewPlaylistButton);
            panel2.Controls.Add(label1);
            panel2.Dock = DockStyle.Top;
            panel2.Location = new Point(0, 0);
            panel2.Name = "panel2";
            panel2.Size = new Size(366, 31);
            panel2.TabIndex = 2;
            // 
            // NewPlaylistButton
            // 
            NewPlaylistButton.Location = new Point(10, 3);
            NewPlaylistButton.Name = "NewPlaylistButton";
            NewPlaylistButton.Size = new Size(75, 23);
            NewPlaylistButton.TabIndex = 2;
            NewPlaylistButton.Text = "New";
            NewPlaylistButton.UseVisualStyleBackColor = true;
            NewPlaylistButton.Click += NewPlaylistButton_Click;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label1.Location = new Point(3, -60);
            label1.Name = "label1";
            label1.Size = new Size(50, 15);
            label1.TabIndex = 0;
            label1.Text = "Playlists";
            // 
            // PlaylistPropertiesPanel
            // 
            PlaylistPropertiesPanel.Controls.Add(DeleteCollectionButton);
            PlaylistPropertiesPanel.Controls.Add(label2);
            PlaylistPropertiesPanel.Controls.Add(NameTextBox);
            PlaylistPropertiesPanel.Controls.Add(ApplyNameButton);
            PlaylistPropertiesPanel.Dock = DockStyle.Bottom;
            PlaylistPropertiesPanel.Location = new Point(0, 870);
            PlaylistPropertiesPanel.Name = "PlaylistPropertiesPanel";
            PlaylistPropertiesPanel.Size = new Size(366, 68);
            PlaylistPropertiesPanel.TabIndex = 0;
            // 
            // DeleteCollectionButton
            // 
            DeleteCollectionButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            DeleteCollectionButton.Location = new Point(275, 35);
            DeleteCollectionButton.Name = "DeleteCollectionButton";
            DeleteCollectionButton.Size = new Size(75, 23);
            DeleteCollectionButton.TabIndex = 5;
            DeleteCollectionButton.Text = "Delete";
            DeleteCollectionButton.UseVisualStyleBackColor = true;
            DeleteCollectionButton.Click += DeleteCollectionButton_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 9);
            label2.Name = "label2";
            label2.Size = new Size(42, 15);
            label2.TabIndex = 3;
            label2.Text = "Name:";
            // 
            // NameTextBox
            // 
            NameTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            NameTextBox.Location = new Point(60, 6);
            NameTextBox.Name = "NameTextBox";
            NameTextBox.Size = new Size(290, 23);
            NameTextBox.TabIndex = 2;
            NameTextBox.TextChanged += NameTextBox_TextChanged;
            // 
            // ApplyNameButton
            // 
            ApplyNameButton.Location = new Point(12, 35);
            ApplyNameButton.Name = "ApplyNameButton";
            ApplyNameButton.Size = new Size(75, 23);
            ApplyNameButton.TabIndex = 1;
            ApplyNameButton.Text = "Apply";
            ApplyNameButton.UseVisualStyleBackColor = true;
            ApplyNameButton.Click += ApplyNameButton_Click;
            // 
            // splitContainer2
            // 
            splitContainer2.BorderStyle = BorderStyle.Fixed3D;
            splitContainer2.Dock = DockStyle.Fill;
            splitContainer2.Location = new Point(0, 0);
            splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            splitContainer2.Panel1.Controls.Add(AttachmentsListBox);
            splitContainer2.Panel1.Controls.Add(panel1);
            // 
            // splitContainer2.Panel2
            // 
            splitContainer2.Panel2.Controls.Add(SourceList);
            splitContainer2.Panel2.Controls.Add(SourcePropertiesPanel);
            splitContainer2.Size = new Size(866, 942);
            splitContainer2.SplitterDistance = 638;
            splitContainer2.TabIndex = 0;
            // 
            // AttachmentsListBox
            // 
            AttachmentsListBox.Dock = DockStyle.Fill;
            AttachmentsListBox.FormattingEnabled = true;
            AttachmentsListBox.ItemHeight = 15;
            AttachmentsListBox.Location = new Point(0, 0);
            AttachmentsListBox.Name = "AttachmentsListBox";
            AttachmentsListBox.Size = new Size(543, 938);
            AttachmentsListBox.TabIndex = 3;
            AttachmentsListBox.SelectedIndexChanged += AttachmentsListBox_SelectedIndexChanged;
            // 
            // panel1
            // 
            panel1.Controls.Add(AttachmentDeleteBtn);
            panel1.Dock = DockStyle.Right;
            panel1.Location = new Point(543, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(91, 938);
            panel1.TabIndex = 2;
            // 
            // AttachmentDeleteBtn
            // 
            AttachmentDeleteBtn.Location = new Point(6, 3);
            AttachmentDeleteBtn.Name = "AttachmentDeleteBtn";
            AttachmentDeleteBtn.Size = new Size(75, 23);
            AttachmentDeleteBtn.TabIndex = 7;
            AttachmentDeleteBtn.Text = "Delete";
            AttachmentDeleteBtn.UseVisualStyleBackColor = true;
            AttachmentDeleteBtn.Click += AttachmentDeleteBtn_Click;
            // 
            // SourceList
            // 
            SourceList.Dock = DockStyle.Fill;
            SourceList.FormattingEnabled = true;
            SourceList.ItemHeight = 15;
            SourceList.Items.AddRange(new object[] { "Test1", "Test2" });
            SourceList.Location = new Point(0, 0);
            SourceList.Name = "SourceList";
            SourceList.Size = new Size(220, 879);
            SourceList.TabIndex = 2;
            SourceList.SelectedIndexChanged += SourceList_SelectedIndexChanged;
            // 
            // SourcePropertiesPanel
            // 
            SourcePropertiesPanel.Controls.Add(SourceAddButton);
            SourcePropertiesPanel.Dock = DockStyle.Bottom;
            SourcePropertiesPanel.Location = new Point(0, 879);
            SourcePropertiesPanel.Name = "SourcePropertiesPanel";
            SourcePropertiesPanel.Size = new Size(220, 59);
            SourcePropertiesPanel.TabIndex = 1;
            // 
            // SourceAddButton
            // 
            SourceAddButton.Location = new Point(3, 6);
            SourceAddButton.Name = "SourceAddButton";
            SourceAddButton.Size = new Size(75, 23);
            SourceAddButton.TabIndex = 0;
            SourceAddButton.Text = "Add";
            SourceAddButton.UseVisualStyleBackColor = true;
            SourceAddButton.Click += SourceAddButton_Click;
            // 
            // CollectionsForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1240, 942);
            Controls.Add(splitContainer1);
            Name = "CollectionsForm";
            Text = "CollectionsForm";
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            PlaylistPropertiesPanel.ResumeLayout(false);
            PlaylistPropertiesPanel.PerformLayout();
            splitContainer2.Panel1.ResumeLayout(false);
            splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer2).EndInit();
            splitContainer2.ResumeLayout(false);
            panel1.ResumeLayout(false);
            SourcePropertiesPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private SplitContainer splitContainer1;
        private Panel panel2;
        private Button NewPlaylistButton;
        private Label label1;
        private Panel PlaylistPropertiesPanel;
        private Button DeleteCollectionButton;
        private Label label2;
        private TextBox NameTextBox;
        private Button ApplyNameButton;
        private SplitContainer splitContainer2;
        private ListBox SourceList;
        private Panel SourcePropertiesPanel;
        private Button SourceAddButton;
        private ListBox AttachmentsListBox;
        private Panel panel1;
        private Button AttachmentDeleteBtn;
        private ListBox CollectionsListBox;
    }
}