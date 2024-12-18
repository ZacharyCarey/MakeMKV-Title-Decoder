using MakeMKV_Title_Decoder.Controls;

namespace MakeMKV_Title_Decoder
{
    partial class ClipRenamerForm {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ClipRenamerForm));
            ClipsList = new ListView();
            columnHeader3 = new ColumnHeader();
            columnHeader1 = new ColumnHeader();
            columnHeader2 = new ColumnHeader();
            ImageList1 = new ImageList(components);
            splitContainer1 = new SplitContainer();
            PropertiesPanel = new Panel();
            DeinterlaceCheckBox = new CheckBox();
            ApplyBtn = new Button();
            NameTextBox = new TextBox();
            label1 = new Label();
            splitContainer2 = new SplitContainer();
            splitContainer3 = new SplitContainer();
            VideoPreview = new VideoPlayer();
            VideoView1 = new LibVLCSharp.WinForms.VideoView();
            OtherTrackList = new TrackList();
            OtherTrackPanel = new Panel();
            SelectOtherLang = new Button();
            OtherLangTextBox = new TextBox();
            label7 = new Label();
            OtherTrackDefault = new CheckBox();
            OtherTrackApply = new Button();
            OtherTrackName = new TextBox();
            label4 = new Label();
            splitContainer4 = new SplitContainer();
            VideoTrackList = new TrackList();
            VideoTrackPanel = new Panel();
            SelectVideoLangBtn = new Button();
            VideoLangTextBox = new TextBox();
            label5 = new Label();
            VideoTrackDefault = new CheckBox();
            VideoTrackApply = new Button();
            VideoTrackCommentary = new CheckBox();
            VideoTrackName = new TextBox();
            label2 = new Label();
            AudioTrackList = new TrackList();
            AudioTrackPanel = new Panel();
            SelectAudioLang = new Button();
            AudioLangTextBox = new TextBox();
            label6 = new Label();
            AudioTrackDefault = new CheckBox();
            AudioTrackApply = new Button();
            AudioTrackCommentary = new CheckBox();
            AudioTrackName = new TextBox();
            label3 = new Label();
            menuStrip1 = new MenuStrip();
            compareToolStripMenuItem = new ToolStripMenuItem();
            viewFramesToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem1 = new ToolStripMenuItem();
            toolStripMenuItem2 = new ToolStripMenuItem();
            AllSelectedStripMenuItem = new ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            PropertiesPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
            splitContainer2.Panel1.SuspendLayout();
            splitContainer2.Panel2.SuspendLayout();
            splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer3).BeginInit();
            splitContainer3.Panel1.SuspendLayout();
            splitContainer3.Panel2.SuspendLayout();
            splitContainer3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)VideoView1).BeginInit();
            OtherTrackPanel.SuspendLayout();
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
            ClipsList.Name = "ClipsList";
            ClipsList.Size = new Size(370, 959);
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
            ImageList1.Images.SetKeyName(2, "tool-animator.png");
            ImageList1.Images.SetKeyName(3, "audio-headphones.png");
            ImageList1.Images.SetKeyName(4, "text.png");
            ImageList1.Images.SetKeyName(5, "clock.png");
            ImageList1.Images.SetKeyName(6, "application-octet-stream.png");
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
            PropertiesPanel.Controls.Add(DeinterlaceCheckBox);
            PropertiesPanel.Controls.Add(ApplyBtn);
            PropertiesPanel.Controls.Add(NameTextBox);
            PropertiesPanel.Controls.Add(label1);
            PropertiesPanel.Dock = DockStyle.Bottom;
            PropertiesPanel.Location = new Point(0, 959);
            PropertiesPanel.Name = "PropertiesPanel";
            PropertiesPanel.Size = new Size(370, 84);
            PropertiesPanel.TabIndex = 0;
            // 
            // DeinterlaceCheckBox
            // 
            DeinterlaceCheckBox.AutoSize = true;
            DeinterlaceCheckBox.Location = new Point(10, 32);
            DeinterlaceCheckBox.Name = "DeinterlaceCheckBox";
            DeinterlaceCheckBox.Size = new Size(85, 19);
            DeinterlaceCheckBox.TabIndex = 3;
            DeinterlaceCheckBox.Text = "Deinterlace";
            DeinterlaceCheckBox.UseVisualStyleBackColor = true;
            DeinterlaceCheckBox.CheckedChanged += DeinterlaceCheckBox_CheckedChanged;
            // 
            // ApplyBtn
            // 
            ApplyBtn.Location = new Point(10, 57);
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
            splitContainer2.Panel1.Controls.Add(splitContainer3);
            // 
            // splitContainer2.Panel2
            // 
            splitContainer2.Panel2.Controls.Add(splitContainer4);
            splitContainer2.Size = new Size(1523, 1047);
            splitContainer2.SplitterDistance = 933;
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
            splitContainer3.Panel1.Controls.Add(VideoPreview);
            splitContainer3.Panel1.Controls.Add(VideoView1);
            // 
            // splitContainer3.Panel2
            // 
            splitContainer3.Panel2.Controls.Add(OtherTrackList);
            splitContainer3.Panel2.Controls.Add(OtherTrackPanel);
            splitContainer3.Size = new Size(933, 1047);
            splitContainer3.SplitterDistance = 685;
            splitContainer3.TabIndex = 2;
            // 
            // VideoPreview
            // 
            VideoPreview.AudioTrack = -1L;
            VideoPreview.Dock = DockStyle.Bottom;
            VideoPreview.Location = new Point(0, 590);
            VideoPreview.Name = "VideoPreview";
            VideoPreview.Size = new Size(929, 91);
            VideoPreview.Sync = null;
            VideoPreview.TabIndex = 0;
            VideoPreview.VideoTrack = -1L;
            VideoPreview.VLC = null;
            VideoPreview.VlcViewer = null;
            // 
            // VideoView1
            // 
            VideoView1.BackColor = Color.Black;
            VideoView1.Dock = DockStyle.Fill;
            VideoView1.Location = new Point(0, 0);
            VideoView1.MediaPlayer = null;
            VideoView1.Name = "VideoView1";
            VideoView1.Size = new Size(929, 681);
            VideoView1.TabIndex = 1;
            VideoView1.Text = "videoView1";
            // 
            // OtherTrackList
            // 
            OtherTrackList.ColumnPadding = 0;
            OtherTrackList.DeleteIconKey = "dialog-cancel.png";
            OtherTrackList.Dock = DockStyle.Fill;
            OtherTrackList.FullRowSelect = true;
            OtherTrackList.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            OtherTrackList.KeepIconKey = "dialog-ok-apply.png";
            OtherTrackList.Location = new Point(0, 0);
            OtherTrackList.MultiSelect = false;
            OtherTrackList.Name = "OtherTrackList";
            OtherTrackList.OwnerDraw = true;
            OtherTrackList.SelectedIndex = null;
            OtherTrackList.SelectedItem = null;
            OtherTrackList.Size = new Size(929, 238);
            OtherTrackList.SmallImageList = ImageList1;
            OtherTrackList.TabIndex = 5;
            OtherTrackList.UseCompatibleStateImageBehavior = false;
            OtherTrackList.View = View.Details;
            OtherTrackList.OnSelectionChanged += OtherTrackList_OnSelectionChanged;
            // 
            // OtherTrackPanel
            // 
            OtherTrackPanel.Controls.Add(SelectOtherLang);
            OtherTrackPanel.Controls.Add(OtherLangTextBox);
            OtherTrackPanel.Controls.Add(label7);
            OtherTrackPanel.Controls.Add(OtherTrackDefault);
            OtherTrackPanel.Controls.Add(OtherTrackApply);
            OtherTrackPanel.Controls.Add(OtherTrackName);
            OtherTrackPanel.Controls.Add(label4);
            OtherTrackPanel.Dock = DockStyle.Bottom;
            OtherTrackPanel.Location = new Point(0, 238);
            OtherTrackPanel.Name = "OtherTrackPanel";
            OtherTrackPanel.Size = new Size(929, 116);
            OtherTrackPanel.TabIndex = 4;
            // 
            // SelectOtherLang
            // 
            SelectOtherLang.Location = new Point(177, 55);
            SelectOtherLang.Name = "SelectOtherLang";
            SelectOtherLang.Size = new Size(75, 23);
            SelectOtherLang.TabIndex = 12;
            SelectOtherLang.Text = "Select";
            SelectOtherLang.UseVisualStyleBackColor = true;
            SelectOtherLang.Click += SelectOtherLang_Click;
            // 
            // OtherLangTextBox
            // 
            OtherLangTextBox.Location = new Point(71, 56);
            OtherLangTextBox.Name = "OtherLangTextBox";
            OtherLangTextBox.Size = new Size(100, 23);
            OtherLangTextBox.TabIndex = 11;
            OtherLangTextBox.TextChanged += LangTextBox_TextChanged;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(3, 59);
            label7.Name = "label7";
            label7.Size = new Size(62, 15);
            label7.TabIndex = 10;
            label7.Text = "Language:";
            // 
            // OtherTrackDefault
            // 
            OtherTrackDefault.AutoSize = true;
            OtherTrackDefault.Location = new Point(3, 32);
            OtherTrackDefault.Name = "OtherTrackDefault";
            OtherTrackDefault.Size = new Size(64, 19);
            OtherTrackDefault.TabIndex = 7;
            OtherTrackDefault.Text = "Default";
            OtherTrackDefault.UseVisualStyleBackColor = true;
            // 
            // OtherTrackApply
            // 
            OtherTrackApply.Location = new Point(3, 85);
            OtherTrackApply.Name = "OtherTrackApply";
            OtherTrackApply.Size = new Size(75, 23);
            OtherTrackApply.TabIndex = 5;
            OtherTrackApply.Text = "Apply";
            OtherTrackApply.UseVisualStyleBackColor = true;
            OtherTrackApply.Click += OtherTrackApply_Click;
            // 
            // OtherTrackName
            // 
            OtherTrackName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            OtherTrackName.Location = new Point(51, 3);
            OtherTrackName.Name = "OtherTrackName";
            OtherTrackName.Size = new Size(875, 23);
            OtherTrackName.TabIndex = 3;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(3, 6);
            label4.Name = "label4";
            label4.Size = new Size(42, 15);
            label4.TabIndex = 2;
            label4.Text = "Name:";
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
            VideoTrackList.ColumnPadding = 0;
            VideoTrackList.DeleteIconKey = "dialog-cancel.png";
            VideoTrackList.Dock = DockStyle.Fill;
            VideoTrackList.FullRowSelect = true;
            VideoTrackList.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            VideoTrackList.KeepIconKey = "dialog-ok-apply.png";
            VideoTrackList.Location = new Point(0, 0);
            VideoTrackList.MultiSelect = false;
            VideoTrackList.Name = "VideoTrackList";
            VideoTrackList.OwnerDraw = true;
            VideoTrackList.SelectedIndex = null;
            VideoTrackList.SelectedItem = null;
            VideoTrackList.Size = new Size(582, 173);
            VideoTrackList.SmallImageList = ImageList1;
            VideoTrackList.TabIndex = 1;
            VideoTrackList.UseCompatibleStateImageBehavior = false;
            VideoTrackList.View = View.Details;
            VideoTrackList.OnSelectionChanged += VideoTrackList_OnSelectionChanged;
            VideoTrackList.SelectedIndexChanged += VideoTrackList_SelectedIndexChanged;
            // 
            // VideoTrackPanel
            // 
            VideoTrackPanel.Controls.Add(SelectVideoLangBtn);
            VideoTrackPanel.Controls.Add(VideoLangTextBox);
            VideoTrackPanel.Controls.Add(label5);
            VideoTrackPanel.Controls.Add(VideoTrackDefault);
            VideoTrackPanel.Controls.Add(VideoTrackApply);
            VideoTrackPanel.Controls.Add(VideoTrackCommentary);
            VideoTrackPanel.Controls.Add(VideoTrackName);
            VideoTrackPanel.Controls.Add(label2);
            VideoTrackPanel.Dock = DockStyle.Bottom;
            VideoTrackPanel.Location = new Point(0, 173);
            VideoTrackPanel.Name = "VideoTrackPanel";
            VideoTrackPanel.Size = new Size(582, 140);
            VideoTrackPanel.TabIndex = 0;
            // 
            // SelectVideoLangBtn
            // 
            SelectVideoLangBtn.Location = new Point(177, 81);
            SelectVideoLangBtn.Name = "SelectVideoLangBtn";
            SelectVideoLangBtn.Size = new Size(75, 23);
            SelectVideoLangBtn.TabIndex = 9;
            SelectVideoLangBtn.Text = "Select";
            SelectVideoLangBtn.UseVisualStyleBackColor = true;
            SelectVideoLangBtn.Click += SelectVideoLangBtn_Click;
            // 
            // VideoLangTextBox
            // 
            VideoLangTextBox.Location = new Point(71, 82);
            VideoLangTextBox.Name = "VideoLangTextBox";
            VideoLangTextBox.Size = new Size(100, 23);
            VideoLangTextBox.TabIndex = 8;
            VideoLangTextBox.TextChanged += LangTextBox_TextChanged;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(3, 85);
            label5.Name = "label5";
            label5.Size = new Size(62, 15);
            label5.TabIndex = 7;
            label5.Text = "Language:";
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
            // VideoTrackApply
            // 
            VideoTrackApply.Location = new Point(3, 111);
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
            VideoTrackName.BackColor = SystemColors.Window;
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
            AudioTrackList.ColumnPadding = 0;
            AudioTrackList.DeleteIconKey = "dialog-cancel.png";
            AudioTrackList.Dock = DockStyle.Fill;
            AudioTrackList.FullRowSelect = true;
            AudioTrackList.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            AudioTrackList.KeepIconKey = "dialog-ok-apply.png";
            AudioTrackList.Location = new Point(0, 0);
            AudioTrackList.MultiSelect = false;
            AudioTrackList.Name = "AudioTrackList";
            AudioTrackList.OwnerDraw = true;
            AudioTrackList.SelectedIndex = null;
            AudioTrackList.SelectedItem = null;
            AudioTrackList.Size = new Size(582, 582);
            AudioTrackList.SmallImageList = ImageList1;
            AudioTrackList.TabIndex = 2;
            AudioTrackList.UseCompatibleStateImageBehavior = false;
            AudioTrackList.View = View.Details;
            AudioTrackList.OnSelectionChanged += AudioTrackList_OnSelectionChanged;
            AudioTrackList.SelectedIndexChanged += AudioTrackList_SelectedIndexChanged;
            // 
            // AudioTrackPanel
            // 
            AudioTrackPanel.Controls.Add(SelectAudioLang);
            AudioTrackPanel.Controls.Add(AudioLangTextBox);
            AudioTrackPanel.Controls.Add(label6);
            AudioTrackPanel.Controls.Add(AudioTrackDefault);
            AudioTrackPanel.Controls.Add(AudioTrackApply);
            AudioTrackPanel.Controls.Add(AudioTrackCommentary);
            AudioTrackPanel.Controls.Add(AudioTrackName);
            AudioTrackPanel.Controls.Add(label3);
            AudioTrackPanel.Dock = DockStyle.Bottom;
            AudioTrackPanel.Location = new Point(0, 582);
            AudioTrackPanel.Name = "AudioTrackPanel";
            AudioTrackPanel.Size = new Size(582, 140);
            AudioTrackPanel.TabIndex = 3;
            // 
            // SelectAudioLang
            // 
            SelectAudioLang.Location = new Point(177, 81);
            SelectAudioLang.Name = "SelectAudioLang";
            SelectAudioLang.Size = new Size(75, 23);
            SelectAudioLang.TabIndex = 12;
            SelectAudioLang.Text = "Select";
            SelectAudioLang.UseVisualStyleBackColor = true;
            SelectAudioLang.Click += SelectAudioLang_Click;
            // 
            // AudioLangTextBox
            // 
            AudioLangTextBox.Location = new Point(71, 82);
            AudioLangTextBox.Name = "AudioLangTextBox";
            AudioLangTextBox.Size = new Size(100, 23);
            AudioLangTextBox.TabIndex = 11;
            AudioLangTextBox.TextChanged += LangTextBox_TextChanged;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(3, 85);
            label6.Name = "label6";
            label6.Size = new Size(62, 15);
            label6.TabIndex = 10;
            label6.Text = "Language:";
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
            // AudioTrackApply
            // 
            AudioTrackApply.Location = new Point(3, 111);
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
            menuStrip1.Items.AddRange(new ToolStripItem[] { compareToolStripMenuItem, viewFramesToolStripMenuItem, toolStripMenuItem1 });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1901, 24);
            menuStrip1.TabIndex = 4;
            menuStrip1.Text = "menuStrip1";
            menuStrip1.ItemClicked += menuStrip1_ItemClicked;
            // 
            // compareToolStripMenuItem
            // 
            compareToolStripMenuItem.Name = "compareToolStripMenuItem";
            compareToolStripMenuItem.Size = new Size(68, 20);
            compareToolStripMenuItem.Text = "Compare";
            compareToolStripMenuItem.Click += compareToolStripMenuItem_Click;
            // 
            // viewFramesToolStripMenuItem
            // 
            viewFramesToolStripMenuItem.Name = "viewFramesToolStripMenuItem";
            viewFramesToolStripMenuItem.Size = new Size(85, 20);
            viewFramesToolStripMenuItem.Text = "View Frames";
            viewFramesToolStripMenuItem.Click += viewFramesToolStripMenuItem_Click;
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.DropDownItems.AddRange(new ToolStripItem[] { toolStripMenuItem2 });
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new Size(57, 20);
            toolStripMenuItem1.Text = "Frames";
            // 
            // toolStripMenuItem2
            // 
            toolStripMenuItem2.DropDownItems.AddRange(new ToolStripItem[] { AllSelectedStripMenuItem });
            toolStripMenuItem2.Name = "toolStripMenuItem2";
            toolStripMenuItem2.Size = new Size(180, 22);
            toolStripMenuItem2.Text = "Export";
            // 
            // AllSelectedStripMenuItem
            // 
            AllSelectedStripMenuItem.Name = "AllSelectedStripMenuItem";
            AllSelectedStripMenuItem.Size = new Size(187, 22);
            AllSelectedStripMenuItem.Text = "AllSelectedMenuItem";
            AllSelectedStripMenuItem.Click += exportAllSelectedToolStripMenuItem_Click;
            // 
            // ClipRenamerForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1901, 1071);
            Controls.Add(splitContainer1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "ClipRenamerForm";
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
            splitContainer3.Panel1.ResumeLayout(false);
            splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer3).EndInit();
            splitContainer3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)VideoView1).EndInit();
            OtherTrackPanel.ResumeLayout(false);
            OtherTrackPanel.PerformLayout();
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
        private SplitContainer splitContainer3;
        private TrackList OtherTrackList;
        private Panel OtherTrackPanel;
        private CheckBox OtherTrackDefault;
        private Button OtherTrackApply;
        private TextBox OtherTrackName;
        private Label label4;
        private Button SelectVideoLangBtn;
        private TextBox VideoLangTextBox;
        private Label label5;
        private Button SelectAudioLang;
        private TextBox AudioLangTextBox;
        private Label label6;
        private Button SelectOtherLang;
        private TextBox OtherLangTextBox;
        private Label label7;
        private ToolStripMenuItem viewFramesToolStripMenuItem;
        private CheckBox DeinterlaceCheckBox;
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripMenuItem toolStripMenuItem2;
        private ToolStripMenuItem AllSelectedStripMenuItem;
    }
}