namespace MakeMKV_Title_Decoder {
    partial class ClipRenamer {
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
            ListViewItem listViewItem1 = new ListViewItem(new string[] { "", "003339.m2ts", "Hello" }, "dialog-ok-apply.png");
            ListViewItem listViewItem2 = new ListViewItem("test2");
            ListViewItem listViewItem3 = new ListViewItem("test3");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ClipRenamer));
            ClipsList = new ListView();
            columnHeader3 = new ColumnHeader();
            columnHeader1 = new ColumnHeader();
            columnHeader2 = new ColumnHeader();
            ImageList1 = new ImageList(components);
            splitContainer1 = new SplitContainer();
            PropertiesPanel = new Panel();
            ApplyBtn = new Button();
            NameTextBox = new TextBox();
            label1 = new Label();
            splitContainer2 = new SplitContainer();
            VideoView1 = new LibVLCSharp.WinForms.VideoView();
            VideoPreview = new VideoPlayer();
            splitContainer4 = new SplitContainer();
            VideoTrackList = new TrackList();
            VideoTrackPanel = new Panel();
            VideoTrackApply = new Button();
            VideoTrackCommentary = new CheckBox();
            VideoTrackName = new TextBox();
            label2 = new Label();
            AudioTrackList = new TrackList();
            AudioTrackPanel = new Panel();
            AudioTrackApply = new Button();
            AudioTrackCommentary = new CheckBox();
            AudioTrackName = new TextBox();
            label3 = new Label();
            menuStrip1 = new MenuStrip();
            compareToolStripMenuItem = new ToolStripMenuItem();
            VideoTrackDefault = new CheckBox();
            AudioTrackDefault = new CheckBox();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            PropertiesPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
            splitContainer2.Panel1.SuspendLayout();
            splitContainer2.Panel2.SuspendLayout();
            splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)VideoView1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitContainer4).BeginInit();
            splitContainer4.Panel1.SuspendLayout();
            splitContainer4.Panel2.SuspendLayout();
            splitContainer4.SuspendLayout();
            VideoTrackPanel.SuspendLayout();
            AudioTrackPanel.SuspendLayout();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // ClipsList
            // 
            ClipsList.Columns.AddRange(new ColumnHeader[] { columnHeader3, columnHeader1, columnHeader2 });
            ClipsList.Dock = DockStyle.Fill;
            ClipsList.FullRowSelect = true;
            ClipsList.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            ClipsList.Items.AddRange(new ListViewItem[] { listViewItem1, listViewItem2, listViewItem3 });
            ClipsList.Location = new Point(0, 0);
            ClipsList.MultiSelect = false;
            ClipsList.Name = "ClipsList";
            ClipsList.Size = new Size(370, 977);
            ClipsList.SmallImageList = ImageList1;
            ClipsList.TabIndex = 2;
            ClipsList.UseCompatibleStateImageBehavior = false;
            ClipsList.View = View.Details;
            ClipsList.SelectedIndexChanged += ClipsList_SelectedIndexChanged;
            // 
            // columnHeader3
            // 
            columnHeader3.Text = "Keep";
            columnHeader3.Width = 40;
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "Source";
            columnHeader1.Width = 80;
            // 
            // columnHeader2
            // 
            columnHeader2.Text = "Rename";
            columnHeader2.Width = 160;
            // 
            // ImageList1
            // 
            ImageList1.ColorDepth = ColorDepth.Depth32Bit;
            ImageList1.ImageStream = (ImageListStreamer)resources.GetObject("ImageList1.ImageStream");
            ImageList1.TransparentColor = Color.Transparent;
            ImageList1.Images.SetKeyName(0, "dialog-ok-apply.png");
            ImageList1.Images.SetKeyName(1, "dialog-cancel.png");
            // 
            // splitContainer1
            // 
            splitContainer1.BorderStyle = BorderStyle.Fixed3D;
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 24);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(ClipsList);
            splitContainer1.Panel1.Controls.Add(PropertiesPanel);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(splitContainer2);
            splitContainer1.Size = new Size(1901, 1047);
            splitContainer1.SplitterDistance = 374;
            splitContainer1.TabIndex = 3;
            // 
            // PropertiesPanel
            // 
            PropertiesPanel.Controls.Add(ApplyBtn);
            PropertiesPanel.Controls.Add(NameTextBox);
            PropertiesPanel.Controls.Add(label1);
            PropertiesPanel.Dock = DockStyle.Bottom;
            PropertiesPanel.Location = new Point(0, 977);
            PropertiesPanel.Name = "PropertiesPanel";
            PropertiesPanel.Size = new Size(370, 66);
            PropertiesPanel.TabIndex = 0;
            // 
            // ApplyBtn
            // 
            ApplyBtn.Location = new Point(3, 32);
            ApplyBtn.Name = "ApplyBtn";
            ApplyBtn.Size = new Size(75, 23);
            ApplyBtn.TabIndex = 2;
            ApplyBtn.Text = "Apply";
            ApplyBtn.UseVisualStyleBackColor = true;
            ApplyBtn.Click += ApplyBtn_Click;
            // 
            // NameTextBox
            // 
            NameTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            NameTextBox.Location = new Point(51, 3);
            NameTextBox.Name = "NameTextBox";
            NameTextBox.Size = new Size(318, 23);
            NameTextBox.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(3, 6);
            label1.Name = "label1";
            label1.Size = new Size(42, 15);
            label1.TabIndex = 0;
            label1.Text = "Name:";
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
            splitContainer2.Panel1.Controls.Add(VideoView1);
            splitContainer2.Panel1.Controls.Add(VideoPreview);
            // 
            // splitContainer2.Panel2
            // 
            splitContainer2.Panel2.Controls.Add(splitContainer4);
            splitContainer2.Size = new Size(1523, 1047);
            splitContainer2.SplitterDistance = 933;
            splitContainer2.TabIndex = 0;
            // 
            // VideoView1
            // 
            VideoView1.BackColor = Color.Black;
            VideoView1.Dock = DockStyle.Fill;
            VideoView1.Location = new Point(0, 0);
            VideoView1.MediaPlayer = null;
            VideoView1.Name = "VideoView1";
            VideoView1.Size = new Size(929, 947);
            VideoView1.TabIndex = 1;
            VideoView1.Text = "videoView1";
            // 
            // VideoPreview
            // 
            VideoPreview.AudioTrack = -1L;
            VideoPreview.Dock = DockStyle.Bottom;
            VideoPreview.Location = new Point(0, 947);
            VideoPreview.Name = "VideoPreview";
            VideoPreview.Size = new Size(929, 96);
            VideoPreview.Sync = null;
            VideoPreview.TabIndex = 0;
            VideoPreview.VideoTrack = -1L;
            VideoPreview.VLC = null;
            VideoPreview.VlcViewer = null;
            // 
            // splitContainer4
            // 
            splitContainer4.BorderStyle = BorderStyle.Fixed3D;
            splitContainer4.Dock = DockStyle.Fill;
            splitContainer4.Location = new Point(0, 0);
            splitContainer4.Name = "splitContainer4";
            splitContainer4.Orientation = Orientation.Horizontal;
            // 
            // splitContainer4.Panel1
            // 
            splitContainer4.Panel1.Controls.Add(VideoTrackList);
            splitContainer4.Panel1.Controls.Add(VideoTrackPanel);
            // 
            // splitContainer4.Panel2
            // 
            splitContainer4.Panel2.Controls.Add(AudioTrackList);
            splitContainer4.Panel2.Controls.Add(AudioTrackPanel);
            splitContainer4.Size = new Size(586, 1047);
            splitContainer4.SplitterDistance = 317;
            splitContainer4.TabIndex = 0;
            // 
            // VideoTrackList
            // 
            VideoTrackList.DeleteIconKey = "dialog-cancel.png";
            VideoTrackList.Dock = DockStyle.Fill;
            VideoTrackList.FullRowSelect = true;
            VideoTrackList.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            VideoTrackList.KeepIconKey = "dialog-ok-apply.png";
            VideoTrackList.Location = new Point(0, 0);
            VideoTrackList.MultiSelect = false;
            VideoTrackList.Name = "VideoTrackList";
            VideoTrackList.OwnerDraw = true;
            VideoTrackList.Size = new Size(582, 203);
            VideoTrackList.SmallImageList = ImageList1;
            VideoTrackList.TabIndex = 1;
            VideoTrackList.UseCompatibleStateImageBehavior = false;
            VideoTrackList.View = View.Details;
            VideoTrackList.OnSelectionChanged += VideoTrackList_OnSelectionChanged;
            // 
            // VideoTrackPanel
            // 
            VideoTrackPanel.Controls.Add(VideoTrackDefault);
            VideoTrackPanel.Controls.Add(VideoTrackApply);
            VideoTrackPanel.Controls.Add(VideoTrackCommentary);
            VideoTrackPanel.Controls.Add(VideoTrackName);
            VideoTrackPanel.Controls.Add(label2);
            VideoTrackPanel.Dock = DockStyle.Bottom;
            VideoTrackPanel.Location = new Point(0, 203);
            VideoTrackPanel.Name = "VideoTrackPanel";
            VideoTrackPanel.Size = new Size(582, 110);
            VideoTrackPanel.TabIndex = 0;
            // 
            // VideoTrackApply
            // 
            VideoTrackApply.Location = new Point(3, 82);
            VideoTrackApply.Name = "VideoTrackApply";
            VideoTrackApply.Size = new Size(75, 23);
            VideoTrackApply.TabIndex = 5;
            VideoTrackApply.Text = "Apply";
            VideoTrackApply.UseVisualStyleBackColor = true;
            VideoTrackApply.Click += VideoTrackApply_Click;
            // 
            // VideoTrackCommentary
            // 
            VideoTrackCommentary.AutoSize = true;
            VideoTrackCommentary.Location = new Point(3, 57);
            VideoTrackCommentary.Name = "VideoTrackCommentary";
            VideoTrackCommentary.Size = new Size(96, 19);
            VideoTrackCommentary.TabIndex = 4;
            VideoTrackCommentary.Text = "Commentary";
            VideoTrackCommentary.UseVisualStyleBackColor = true;
            // 
            // VideoTrackName
            // 
            VideoTrackName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            VideoTrackName.Location = new Point(51, 3);
            VideoTrackName.Name = "VideoTrackName";
            VideoTrackName.Size = new Size(528, 23);
            VideoTrackName.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(3, 6);
            label2.Name = "label2";
            label2.Size = new Size(42, 15);
            label2.TabIndex = 2;
            label2.Text = "Name:";
            // 
            // AudioTrackList
            // 
            AudioTrackList.DeleteIconKey = "dialog-cancel.png";
            AudioTrackList.Dock = DockStyle.Fill;
            AudioTrackList.FullRowSelect = true;
            AudioTrackList.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            AudioTrackList.KeepIconKey = "dialog-ok-apply.png";
            AudioTrackList.Location = new Point(0, 0);
            AudioTrackList.MultiSelect = false;
            AudioTrackList.Name = "AudioTrackList";
            AudioTrackList.OwnerDraw = true;
            AudioTrackList.Size = new Size(582, 611);
            AudioTrackList.SmallImageList = ImageList1;
            AudioTrackList.TabIndex = 2;
            AudioTrackList.UseCompatibleStateImageBehavior = false;
            AudioTrackList.View = View.Details;
            AudioTrackList.OnSelectionChanged += AudioTrackList_OnSelectionChanged;
            // 
            // AudioTrackPanel
            // 
            AudioTrackPanel.Controls.Add(AudioTrackDefault);
            AudioTrackPanel.Controls.Add(AudioTrackApply);
            AudioTrackPanel.Controls.Add(AudioTrackCommentary);
            AudioTrackPanel.Controls.Add(AudioTrackName);
            AudioTrackPanel.Controls.Add(label3);
            AudioTrackPanel.Dock = DockStyle.Bottom;
            AudioTrackPanel.Location = new Point(0, 611);
            AudioTrackPanel.Name = "AudioTrackPanel";
            AudioTrackPanel.Size = new Size(582, 111);
            AudioTrackPanel.TabIndex = 3;
            // 
            // AudioTrackApply
            // 
            AudioTrackApply.Location = new Point(3, 82);
            AudioTrackApply.Name = "AudioTrackApply";
            AudioTrackApply.Size = new Size(75, 23);
            AudioTrackApply.TabIndex = 5;
            AudioTrackApply.Text = "Apply";
            AudioTrackApply.UseVisualStyleBackColor = true;
            AudioTrackApply.Click += AudioTrackApply_Click;
            // 
            // AudioTrackCommentary
            // 
            AudioTrackCommentary.AutoSize = true;
            AudioTrackCommentary.Location = new Point(3, 57);
            AudioTrackCommentary.Name = "AudioTrackCommentary";
            AudioTrackCommentary.Size = new Size(96, 19);
            AudioTrackCommentary.TabIndex = 4;
            AudioTrackCommentary.Text = "Commentary";
            AudioTrackCommentary.UseVisualStyleBackColor = true;
            // 
            // AudioTrackName
            // 
            AudioTrackName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            AudioTrackName.Location = new Point(51, 3);
            AudioTrackName.Name = "AudioTrackName";
            AudioTrackName.Size = new Size(528, 23);
            AudioTrackName.TabIndex = 3;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(3, 6);
            label3.Name = "label3";
            label3.Size = new Size(42, 15);
            label3.TabIndex = 2;
            label3.Text = "Name:";
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { compareToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1901, 24);
            menuStrip1.TabIndex = 4;
            menuStrip1.Text = "menuStrip1";
            // 
            // compareToolStripMenuItem
            // 
            compareToolStripMenuItem.Name = "compareToolStripMenuItem";
            compareToolStripMenuItem.Size = new Size(68, 20);
            compareToolStripMenuItem.Text = "Compare";
            compareToolStripMenuItem.Click += compareToolStripMenuItem_Click;
            // 
            // VideoTrackDefault
            // 
            VideoTrackDefault.AutoSize = true;
            VideoTrackDefault.Location = new Point(3, 32);
            VideoTrackDefault.Name = "VideoTrackDefault";
            VideoTrackDefault.Size = new Size(64, 19);
            VideoTrackDefault.TabIndex = 6;
            VideoTrackDefault.Text = "Default";
            VideoTrackDefault.UseVisualStyleBackColor = true;
            // 
            // AudioTrackDefault
            // 
            AudioTrackDefault.AutoSize = true;
            AudioTrackDefault.Location = new Point(3, 32);
            AudioTrackDefault.Name = "AudioTrackDefault";
            AudioTrackDefault.Size = new Size(64, 19);
            AudioTrackDefault.TabIndex = 7;
            AudioTrackDefault.Text = "Default";
            AudioTrackDefault.UseVisualStyleBackColor = true;
            // 
            // ClipRenamer
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1901, 1071);
            Controls.Add(splitContainer1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "ClipRenamer";
            Text = "ClipRenamer";
            FormClosing += ClipRenamer_FormClosing;
            Load += ClipRenamer_Load;
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            PropertiesPanel.ResumeLayout(false);
            PropertiesPanel.PerformLayout();
            splitContainer2.Panel1.ResumeLayout(false);
            splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer2).EndInit();
            splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)VideoView1).EndInit();
            splitContainer4.Panel1.ResumeLayout(false);
            splitContainer4.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer4).EndInit();
            splitContainer4.ResumeLayout(false);
            VideoTrackPanel.ResumeLayout(false);
            VideoTrackPanel.PerformLayout();
            AudioTrackPanel.ResumeLayout(false);
            AudioTrackPanel.PerformLayout();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private ListView ClipsList;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
        private SplitContainer splitContainer1;
        private SplitContainer splitContainer2;
        private VideoPlayer VideoPreview;
        private ColumnHeader columnHeader3;
        private ImageList ImageList1;
        private Panel PropertiesPanel;
        private Button ApplyBtn;
        private TextBox NameTextBox;
        private Label label1;
        private LibVLCSharp.WinForms.VideoView VideoView1;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem compareToolStripMenuItem;
        private SplitContainer splitContainer4;
        private Panel VideoTrackPanel;
        private TextBox VideoTrackName;
        private Label label2;
        private CheckBox VideoTrackCommentary;
        private Button VideoTrackApply;
        private TrackList VideoTrackList;
        private Panel AudioTrackPanel;
        private Button AudioTrackApply;
        private CheckBox AudioTrackCommentary;
        private TextBox AudioTrackName;
        private Label label3;
        private TrackList AudioTrackList;
        private CheckBox VideoTrackDefault;
        private CheckBox AudioTrackDefault;
    }
}