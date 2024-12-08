using MakeMKV_Title_Decoder.Controls;
using MakeMKV_Title_Decoder.Forms.PlaylistCreator;

namespace MakeMKV_Title_Decoder
{
    partial class PlaylistCreatorForm {
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PlaylistCreatorForm));
            splitContainer1 = new SplitContainer();
            PlaylistsListBox = new LoadedPlaylistListBox();
            imageList1 = new ImageList(components);
            panel2 = new Panel();
            NewPlaylistButton = new Button();
            ImportPlaylists = new Button();
            label1 = new Label();
            PlaylistPropertiesPanel = new Panel();
            DeletePlaylistButton = new Button();
            label2 = new Label();
            PlaylistNameTextBox = new TextBox();
            ApplyPlaylistButton = new Button();
            button1 = new Button();
            splitContainer2 = new SplitContainer();
            splitContainer3 = new SplitContainer();
            PlaylistFilesList = new PropertiesList();
            columnHeader1 = new ColumnHeader();
            columnHeader2 = new ColumnHeader();
            columnHeader3 = new ColumnHeader();
            columnHeader4 = new ColumnHeader();
            panel1 = new Panel();
            PlaylistSourceDeleteBtn = new Button();
            PlaylistTrackOrder = new TrackList();
            panel4 = new Panel();
            DeleteDelayBtn = new Button();
            EditDelayBtn = new Button();
            AddDelayBtn = new Button();
            label3 = new Label();
            DownTrackBtn = new Button();
            TrackUpBtn = new Button();
            DisableTrackBtn = new Button();
            EnableTrackBtn = new Button();
            SourceList = new ListBox();
            SourcePropertiesPanel = new Panel();
            SourceApplyButton = new Button();
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
            ((System.ComponentModel.ISupportInitialize)splitContainer3).BeginInit();
            splitContainer3.Panel1.SuspendLayout();
            splitContainer3.Panel2.SuspendLayout();
            splitContainer3.SuspendLayout();
            panel1.SuspendLayout();
            panel4.SuspendLayout();
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
            splitContainer1.Panel1.Controls.Add(PlaylistsListBox);
            splitContainer1.Panel1.Controls.Add(panel2);
            splitContainer1.Panel1.Controls.Add(PlaylistPropertiesPanel);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(splitContainer2);
            splitContainer1.Size = new Size(1840, 1014);
            splitContainer1.SplitterDistance = 314;
            splitContainer1.TabIndex = 0;
            // 
            // PlaylistsListBox
            // 
            PlaylistsListBox.Dock = DockStyle.Fill;
            PlaylistsListBox.DrawMode = DrawMode.OwnerDrawFixed;
            PlaylistsListBox.FormattingEnabled = true;
            PlaylistsListBox.HorizontalScrollbar = true;
            PlaylistsListBox.ItemHeight = 15;
            PlaylistsListBox.Location = new Point(0, 31);
            PlaylistsListBox.Name = "PlaylistsListBox";
            PlaylistsListBox.SelectedItem = null;
            PlaylistsListBox.Size = new Size(310, 911);
            PlaylistsListBox.SmallIconList = imageList1;
            PlaylistsListBox.TabIndex = 3;
            PlaylistsListBox.SelectedIndexChanged += PlaylistsListBox_SelectedIndexChanged;
            // 
            // imageList1
            // 
            imageList1.ColorDepth = ColorDepth.Depth32Bit;
            imageList1.ImageStream = (ImageListStreamer)resources.GetObject("imageList1.ImageStream");
            imageList1.TransparentColor = Color.Transparent;
            imageList1.Images.SetKeyName(0, "dialog-cancel.png");
            imageList1.Images.SetKeyName(1, "dialog-ok-apply.png");
            imageList1.Images.SetKeyName(2, "tool-animator.png");
            imageList1.Images.SetKeyName(3, "audio-headphones.png");
            imageList1.Images.SetKeyName(4, "text.png");
            imageList1.Images.SetKeyName(5, "clock.png");
            imageList1.Images.SetKeyName(6, "application-octet-stream.png");
            imageList1.Images.SetKeyName(7, "document-save-as.png");
            imageList1.Images.SetKeyName(8, "dialog-warning.png");
            // 
            // panel2
            // 
            panel2.Controls.Add(NewPlaylistButton);
            panel2.Controls.Add(ImportPlaylists);
            panel2.Controls.Add(label1);
            panel2.Dock = DockStyle.Top;
            panel2.Location = new Point(0, 0);
            panel2.Name = "panel2";
            panel2.Size = new Size(310, 31);
            panel2.TabIndex = 2;
            // 
            // NewPlaylistButton
            // 
            NewPlaylistButton.Location = new Point(96, 5);
            NewPlaylistButton.Name = "NewPlaylistButton";
            NewPlaylistButton.Size = new Size(75, 23);
            NewPlaylistButton.TabIndex = 2;
            NewPlaylistButton.Text = "New";
            NewPlaylistButton.UseVisualStyleBackColor = true;
            NewPlaylistButton.Click += NewPlaylistButton_Click;
            // 
            // ImportPlaylists
            // 
            ImportPlaylists.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            ImportPlaylists.Location = new Point(190, 5);
            ImportPlaylists.Name = "ImportPlaylists";
            ImportPlaylists.Size = new Size(117, 23);
            ImportPlaylists.TabIndex = 1;
            ImportPlaylists.Text = "Import from disc";
            ImportPlaylists.UseVisualStyleBackColor = true;
            ImportPlaylists.Click += ImportPlaylists_Click;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label1.Location = new Point(3, 9);
            label1.Name = "label1";
            label1.Size = new Size(50, 15);
            label1.TabIndex = 0;
            label1.Text = "Playlists";
            // 
            // PlaylistPropertiesPanel
            // 
            PlaylistPropertiesPanel.Controls.Add(DeletePlaylistButton);
            PlaylistPropertiesPanel.Controls.Add(label2);
            PlaylistPropertiesPanel.Controls.Add(PlaylistNameTextBox);
            PlaylistPropertiesPanel.Controls.Add(ApplyPlaylistButton);
            PlaylistPropertiesPanel.Controls.Add(button1);
            PlaylistPropertiesPanel.Dock = DockStyle.Bottom;
            PlaylistPropertiesPanel.Location = new Point(0, 942);
            PlaylistPropertiesPanel.Name = "PlaylistPropertiesPanel";
            PlaylistPropertiesPanel.Size = new Size(310, 68);
            PlaylistPropertiesPanel.TabIndex = 0;
            // 
            // DeletePlaylistButton
            // 
            DeletePlaylistButton.Location = new Point(232, 35);
            DeletePlaylistButton.Name = "DeletePlaylistButton";
            DeletePlaylistButton.Size = new Size(75, 23);
            DeletePlaylistButton.TabIndex = 5;
            DeletePlaylistButton.Text = "Delete";
            DeletePlaylistButton.UseVisualStyleBackColor = true;
            DeletePlaylistButton.Click += DeletePlaylistButton_Click;
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
            // PlaylistNameTextBox
            // 
            PlaylistNameTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            PlaylistNameTextBox.Location = new Point(60, 6);
            PlaylistNameTextBox.Name = "PlaylistNameTextBox";
            PlaylistNameTextBox.Size = new Size(247, 23);
            PlaylistNameTextBox.TabIndex = 2;
            // 
            // ApplyPlaylistButton
            // 
            ApplyPlaylistButton.Location = new Point(12, 35);
            ApplyPlaylistButton.Name = "ApplyPlaylistButton";
            ApplyPlaylistButton.Size = new Size(75, 23);
            ApplyPlaylistButton.TabIndex = 1;
            ApplyPlaylistButton.Text = "Apply";
            ApplyPlaylistButton.UseVisualStyleBackColor = true;
            ApplyPlaylistButton.Click += ApplyPlaylistButton_Click;
            // 
            // button1
            // 
            button1.Location = new Point(535, 35);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 0;
            button1.Text = "Delete";
            button1.UseVisualStyleBackColor = true;
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
            splitContainer2.Panel1.Controls.Add(splitContainer3);
            // 
            // splitContainer2.Panel2
            // 
            splitContainer2.Panel2.Controls.Add(SourceList);
            splitContainer2.Panel2.Controls.Add(SourcePropertiesPanel);
            splitContainer2.Size = new Size(1522, 1014);
            splitContainer2.SplitterDistance = 1122;
            splitContainer2.TabIndex = 0;
            // 
            // splitContainer3
            // 
            splitContainer3.BorderStyle = BorderStyle.Fixed3D;
            splitContainer3.Dock = DockStyle.Fill;
            splitContainer3.Location = new Point(0, 0);
            splitContainer3.Name = "splitContainer3";
            splitContainer3.Orientation = Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            splitContainer3.Panel1.Controls.Add(PlaylistFilesList);
            splitContainer3.Panel1.Controls.Add(panel1);
            // 
            // splitContainer3.Panel2
            // 
            splitContainer3.Panel2.Controls.Add(PlaylistTrackOrder);
            splitContainer3.Panel2.Controls.Add(panel4);
            splitContainer3.Size = new Size(1122, 1014);
            splitContainer3.SplitterDistance = 285;
            splitContainer3.TabIndex = 2;
            // 
            // PlaylistFilesList
            // 
            PlaylistFilesList.Columns.AddRange(new ColumnHeader[] { columnHeader1, columnHeader2, columnHeader3, columnHeader4 });
            PlaylistFilesList.Dock = DockStyle.Fill;
            PlaylistFilesList.FullRowSelect = true;
            PlaylistFilesList.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            PlaylistFilesList.Location = new Point(0, 0);
            PlaylistFilesList.MultiSelect = false;
            PlaylistFilesList.Name = "PlaylistFilesList";
            PlaylistFilesList.OwnerDraw = true;
            PlaylistFilesList.SelectedIndex = null;
            PlaylistFilesList.SelectedItem = null;
            PlaylistFilesList.Size = new Size(1027, 281);
            PlaylistFilesList.SmallImageList = imageList1;
            PlaylistFilesList.TabIndex = 0;
            PlaylistFilesList.UseCompatibleStateImageBehavior = false;
            PlaylistFilesList.View = View.Details;
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "Clip Name";
            columnHeader1.Width = 300;
            // 
            // columnHeader2
            // 
            columnHeader2.Text = "Container";
            columnHeader2.Width = 200;
            // 
            // columnHeader3
            // 
            columnHeader3.Text = "File Size";
            columnHeader3.Width = 100;
            // 
            // columnHeader4
            // 
            columnHeader4.Text = "Directory";
            columnHeader4.Width = 400;
            // 
            // panel1
            // 
            panel1.Controls.Add(PlaylistSourceDeleteBtn);
            panel1.Dock = DockStyle.Right;
            panel1.Location = new Point(1027, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(91, 281);
            panel1.TabIndex = 1;
            // 
            // PlaylistSourceDeleteBtn
            // 
            PlaylistSourceDeleteBtn.Location = new Point(6, 3);
            PlaylistSourceDeleteBtn.Name = "PlaylistSourceDeleteBtn";
            PlaylistSourceDeleteBtn.Size = new Size(75, 23);
            PlaylistSourceDeleteBtn.TabIndex = 7;
            PlaylistSourceDeleteBtn.Text = "Delete";
            PlaylistSourceDeleteBtn.UseVisualStyleBackColor = true;
            PlaylistSourceDeleteBtn.Click += PlaylistSourceDeleteBtn_Click;
            // 
            // PlaylistTrackOrder
            // 
            PlaylistTrackOrder.ColumnPadding = 25;
            PlaylistTrackOrder.DeleteIconKey = "dialog-cancel.png";
            PlaylistTrackOrder.Dock = DockStyle.Fill;
            PlaylistTrackOrder.FullRowSelect = true;
            PlaylistTrackOrder.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            PlaylistTrackOrder.KeepIconKey = "dialog-ok-apply.png";
            PlaylistTrackOrder.Location = new Point(0, 0);
            PlaylistTrackOrder.MultiSelect = false;
            PlaylistTrackOrder.Name = "PlaylistTrackOrder";
            PlaylistTrackOrder.OwnerDraw = true;
            PlaylistTrackOrder.SelectedIndex = null;
            PlaylistTrackOrder.SelectedItem = null;
            PlaylistTrackOrder.Size = new Size(1027, 721);
            PlaylistTrackOrder.SmallImageList = imageList1;
            PlaylistTrackOrder.TabIndex = 1;
            PlaylistTrackOrder.UseCompatibleStateImageBehavior = false;
            PlaylistTrackOrder.View = View.Details;
            // 
            // panel4
            // 
            panel4.Controls.Add(DeleteDelayBtn);
            panel4.Controls.Add(EditDelayBtn);
            panel4.Controls.Add(AddDelayBtn);
            panel4.Controls.Add(label3);
            panel4.Controls.Add(DownTrackBtn);
            panel4.Controls.Add(TrackUpBtn);
            panel4.Controls.Add(DisableTrackBtn);
            panel4.Controls.Add(EnableTrackBtn);
            panel4.Dock = DockStyle.Right;
            panel4.Location = new Point(1027, 0);
            panel4.Name = "panel4";
            panel4.Size = new Size(91, 721);
            panel4.TabIndex = 0;
            // 
            // DeleteDelayBtn
            // 
            DeleteDelayBtn.Location = new Point(6, 275);
            DeleteDelayBtn.Name = "DeleteDelayBtn";
            DeleteDelayBtn.Size = new Size(75, 23);
            DeleteDelayBtn.TabIndex = 16;
            DeleteDelayBtn.Text = "Delete";
            DeleteDelayBtn.UseVisualStyleBackColor = true;
            DeleteDelayBtn.Click += DeleteDelayBtn_Click;
            // 
            // EditDelayBtn
            // 
            EditDelayBtn.Location = new Point(6, 246);
            EditDelayBtn.Name = "EditDelayBtn";
            EditDelayBtn.Size = new Size(75, 23);
            EditDelayBtn.TabIndex = 15;
            EditDelayBtn.Text = "Edit";
            EditDelayBtn.UseVisualStyleBackColor = true;
            EditDelayBtn.Click += EditDelayBtn_Click;
            // 
            // AddDelayBtn
            // 
            AddDelayBtn.Location = new Point(6, 217);
            AddDelayBtn.Name = "AddDelayBtn";
            AddDelayBtn.Size = new Size(75, 23);
            AddDelayBtn.TabIndex = 14;
            AddDelayBtn.Text = "Add";
            AddDelayBtn.UseVisualStyleBackColor = true;
            AddDelayBtn.Click += AddDelayBtn_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(6, 199);
            label3.Name = "label3";
            label3.Size = new Size(36, 15);
            label3.TabIndex = 13;
            label3.Text = "Delay";
            // 
            // DownTrackBtn
            // 
            DownTrackBtn.Location = new Point(6, 133);
            DownTrackBtn.Name = "DownTrackBtn";
            DownTrackBtn.Size = new Size(75, 23);
            DownTrackBtn.TabIndex = 12;
            DownTrackBtn.Text = "Down";
            DownTrackBtn.UseVisualStyleBackColor = true;
            DownTrackBtn.Click += DownTrackBtn_Click;
            // 
            // TrackUpBtn
            // 
            TrackUpBtn.Location = new Point(6, 104);
            TrackUpBtn.Name = "TrackUpBtn";
            TrackUpBtn.Size = new Size(75, 23);
            TrackUpBtn.TabIndex = 11;
            TrackUpBtn.Text = "Up";
            TrackUpBtn.UseVisualStyleBackColor = true;
            TrackUpBtn.Click += TrackUpBtn_Click;
            // 
            // DisableTrackBtn
            // 
            DisableTrackBtn.Enabled = false;
            DisableTrackBtn.Location = new Point(6, 32);
            DisableTrackBtn.Name = "DisableTrackBtn";
            DisableTrackBtn.Size = new Size(75, 23);
            DisableTrackBtn.TabIndex = 9;
            DisableTrackBtn.Text = "Disable";
            DisableTrackBtn.UseVisualStyleBackColor = true;
            DisableTrackBtn.Click += DisableTrackBtn_Click;
            // 
            // EnableTrackBtn
            // 
            EnableTrackBtn.Enabled = false;
            EnableTrackBtn.Location = new Point(6, 3);
            EnableTrackBtn.Name = "EnableTrackBtn";
            EnableTrackBtn.Size = new Size(75, 23);
            EnableTrackBtn.TabIndex = 8;
            EnableTrackBtn.Text = "Enable";
            EnableTrackBtn.UseVisualStyleBackColor = true;
            EnableTrackBtn.Click += EnableTrackBtn_Click;
            // 
            // SourceList
            // 
            SourceList.Dock = DockStyle.Fill;
            SourceList.FormattingEnabled = true;
            SourceList.ItemHeight = 15;
            SourceList.Items.AddRange(new object[] { "Test1", "Test2" });
            SourceList.Location = new Point(0, 0);
            SourceList.Name = "SourceList";
            SourceList.Size = new Size(392, 951);
            SourceList.TabIndex = 2;
            SourceList.SelectedIndexChanged += SourceList_SelectedIndexChanged;
            // 
            // SourcePropertiesPanel
            // 
            SourcePropertiesPanel.Controls.Add(SourceApplyButton);
            SourcePropertiesPanel.Dock = DockStyle.Bottom;
            SourcePropertiesPanel.Location = new Point(0, 951);
            SourcePropertiesPanel.Name = "SourcePropertiesPanel";
            SourcePropertiesPanel.Size = new Size(392, 59);
            SourcePropertiesPanel.TabIndex = 1;
            // 
            // SourceApplyButton
            // 
            SourceApplyButton.Location = new Point(318, 6);
            SourceApplyButton.Name = "SourceApplyButton";
            SourceApplyButton.Size = new Size(75, 23);
            SourceApplyButton.TabIndex = 0;
            SourceApplyButton.Text = "Add";
            SourceApplyButton.UseVisualStyleBackColor = true;
            SourceApplyButton.Click += SourceApplyButton_Click;
            // 
            // PlaylistCreatorForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1840, 1014);
            Controls.Add(splitContainer1);
            Name = "PlaylistCreatorForm";
            Text = "PlaylistCreatorForm";
            FormClosing += PlaylistCreatorForm_FormClosing;
            Load += PlaylistCreatorForm_Load;
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
            splitContainer3.Panel1.ResumeLayout(false);
            splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer3).EndInit();
            splitContainer3.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel4.ResumeLayout(false);
            panel4.PerformLayout();
            SourcePropertiesPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private SplitContainer splitContainer1;
        private LoadedPlaylistListBox PlaylistsListBox;
        private Panel panel2;
        private Button ImportPlaylists;
        private Label label1;
        private Panel PlaylistPropertiesPanel;
        private Button ApplyPlaylistButton;
        private Button button1;
        private Label label2;
        private TextBox PlaylistNameTextBox;
        private SplitContainer splitContainer2;
        private Panel panel4;
        private ListBox SourceList;
        private Panel SourcePropertiesPanel;
        private Button SourceApplyButton;
        private TrackList PlaylistTrackOrder;
        private Button DeletePlaylistButton;
        private Button NewPlaylistButton;
        private Button DisableTrackBtn;
        private Button EnableTrackBtn;
        private Button DownTrackBtn;
        private Button TrackUpBtn;
        private SplitContainer splitContainer3;
        private PropertiesList PlaylistFilesList;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
        private ColumnHeader columnHeader3;
        private ColumnHeader columnHeader4;
        private Panel panel1;
        private Button PlaylistSourceDeleteBtn;
        private ImageList imageList1;
        private Button EditDelayBtn;
        private Button AddDelayBtn;
        private Label label3;
        private Button DeleteDelayBtn;
    }
}