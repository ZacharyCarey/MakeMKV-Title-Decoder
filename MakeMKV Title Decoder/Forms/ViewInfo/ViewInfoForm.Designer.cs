namespace MakeMKV_Title_Decoder {
    partial class ViewInfoForm {
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
            ListViewItem listViewItem1 = new ListViewItem(new string[] { "1", "3", "4" }, -1);
            ListViewItem listViewItem2 = new ListViewItem("2");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ViewInfoForm));
            label2 = new Label();
            TitleList = new ComboBox();
            label4 = new Label();
            label5 = new Label();
            AudioTab = new TabPage();
            splitContainer1 = new SplitContainer();
            TrackList = new ListView();
            columnHeader1 = new ColumnHeader();
            columnHeader2 = new ColumnHeader();
            columnHeader3 = new ColumnHeader();
            columnHeader4 = new ColumnHeader();
            columnHeader5 = new ColumnHeader();
            columnHeader6 = new ColumnHeader();
            columnHeader12 = new ColumnHeader();
            columnHeader7 = new ColumnHeader();
            columnHeader13 = new ColumnHeader();
            columnHeader8 = new ColumnHeader();
            columnHeader9 = new ColumnHeader();
            columnHeader10 = new ColumnHeader();
            columnHeader11 = new ColumnHeader();
            IconImageList = new ImageList(components);
            TrackProperties = new RichTextBox();
            tabControl1 = new TabControl();
            SummaryTab = new TabPage();
            VideoPreview = new LibVLCSharp.WinForms.VideoView();
            VideoPlayer = new VideoPlayer();
            tabPage1 = new TabPage();
            label3 = new Label();
            ContainerLabel = new Label();
            FileSizeLabel = new Label();
            DirectoryLabel = new Label();
            label1 = new Label();
            panel1 = new Panel();
            DiscNameLabel = new Label();
            DiscSetLabel = new Label();
            label6 = new Label();
            AudioTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            tabControl1.SuspendLayout();
            SummaryTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)VideoPreview).BeginInit();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(3, 48);
            label2.Name = "label2";
            label2.Size = new Size(46, 15);
            label2.TabIndex = 3;
            label2.Text = "Source:";
            // 
            // TitleList
            // 
            TitleList.FormattingEnabled = true;
            TitleList.Location = new Point(55, 45);
            TitleList.Name = "TitleList";
            TitleList.Size = new Size(288, 23);
            TitleList.TabIndex = 4;
            TitleList.SelectedIndexChanged += TitleList_SelectedIndexChanged;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(3, 71);
            label4.Name = "label4";
            label4.Size = new Size(62, 15);
            label4.TabIndex = 7;
            label4.Text = "Container:";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(3, 86);
            label5.Name = "label5";
            label5.Size = new Size(50, 15);
            label5.TabIndex = 9;
            label5.Text = "File size:";
            // 
            // AudioTab
            // 
            AudioTab.Controls.Add(splitContainer1);
            AudioTab.Location = new Point(4, 24);
            AudioTab.Name = "AudioTab";
            AudioTab.Padding = new Padding(3);
            AudioTab.Size = new Size(1399, 843);
            AudioTab.TabIndex = 4;
            AudioTab.Text = "Tracks";
            AudioTab.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(3, 3);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(TrackList);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.AutoScroll = true;
            splitContainer1.Panel2.Controls.Add(TrackProperties);
            splitContainer1.Size = new Size(1393, 837);
            splitContainer1.SplitterDistance = 948;
            splitContainer1.TabIndex = 0;
            // 
            // TrackList
            // 
            TrackList.Columns.AddRange(new ColumnHeader[] { columnHeader1, columnHeader2, columnHeader3, columnHeader4, columnHeader5, columnHeader6, columnHeader12, columnHeader7, columnHeader13, columnHeader8, columnHeader9, columnHeader10, columnHeader11 });
            TrackList.Dock = DockStyle.Fill;
            TrackList.FullRowSelect = true;
            TrackList.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            TrackList.Items.AddRange(new ListViewItem[] { listViewItem1, listViewItem2 });
            TrackList.Location = new Point(0, 0);
            TrackList.MultiSelect = false;
            TrackList.Name = "TrackList";
            TrackList.OwnerDraw = true;
            TrackList.Size = new Size(948, 837);
            TrackList.SmallImageList = IconImageList;
            TrackList.TabIndex = 0;
            TrackList.UseCompatibleStateImageBehavior = false;
            TrackList.View = View.Details;
            TrackList.DrawColumnHeader += TrackList_DrawColumnHeader;
            TrackList.DrawItem += TrackList_DrawItem;
            TrackList.DrawSubItem += TrackList_DrawSubItem;
            TrackList.SelectedIndexChanged += TrackList_SelectedIndexChanged;
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "Codec";
            columnHeader1.Width = 205;
            // 
            // columnHeader2
            // 
            columnHeader2.Text = "Type";
            columnHeader2.Width = 80;
            // 
            // columnHeader3
            // 
            columnHeader3.Text = "Language";
            columnHeader3.Width = 100;
            // 
            // columnHeader4
            // 
            columnHeader4.Text = "Name";
            columnHeader4.Width = 150;
            // 
            // columnHeader5
            // 
            columnHeader5.Text = "ID";
            columnHeader5.Width = 100;
            // 
            // columnHeader6
            // 
            columnHeader6.Text = "Default Track";
            columnHeader6.Width = 100;
            // 
            // columnHeader12
            // 
            columnHeader12.DisplayIndex = 12;
            columnHeader12.Text = "Forced display";
            columnHeader12.Width = 100;
            // 
            // columnHeader7
            // 
            columnHeader7.DisplayIndex = 6;
            columnHeader7.Text = "Character Set";
            columnHeader7.Width = 100;
            // 
            // columnHeader13
            // 
            columnHeader13.DisplayIndex = 7;
            columnHeader13.Text = "Properties";
            columnHeader13.Width = 150;
            // 
            // columnHeader8
            // 
            columnHeader8.DisplayIndex = 8;
            columnHeader8.Text = "Source file";
            columnHeader8.Width = 100;
            // 
            // columnHeader9
            // 
            columnHeader9.DisplayIndex = 9;
            columnHeader9.Text = "Source file directory";
            columnHeader9.Width = 265;
            // 
            // columnHeader10
            // 
            columnHeader10.DisplayIndex = 10;
            columnHeader10.Text = "Program";
            columnHeader10.Width = 100;
            // 
            // columnHeader11
            // 
            columnHeader11.DisplayIndex = 11;
            columnHeader11.Text = "Delay";
            columnHeader11.Width = 100;
            // 
            // IconImageList
            // 
            IconImageList.ColorDepth = ColorDepth.Depth32Bit;
            IconImageList.ImageStream = (ImageListStreamer)resources.GetObject("IconImageList.ImageStream");
            IconImageList.TransparentColor = Color.Transparent;
            IconImageList.Images.SetKeyName(0, "tool-animator.png");
            IconImageList.Images.SetKeyName(1, "audio-headphones.png");
            IconImageList.Images.SetKeyName(2, "text.png");
            IconImageList.Images.SetKeyName(3, "clock.png");
            IconImageList.Images.SetKeyName(4, "dialog-ok-apply.png");
            IconImageList.Images.SetKeyName(5, "dialog-cancel.png");
            IconImageList.Images.SetKeyName(6, "application-octet-stream.png");
            // 
            // TrackProperties
            // 
            TrackProperties.Dock = DockStyle.Fill;
            TrackProperties.Location = new Point(0, 0);
            TrackProperties.Name = "TrackProperties";
            TrackProperties.ReadOnly = true;
            TrackProperties.Size = new Size(441, 837);
            TrackProperties.TabIndex = 0;
            TrackProperties.Text = "";
            // 
            // tabControl1
            // 
            tabControl1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabControl1.Controls.Add(SummaryTab);
            tabControl1.Controls.Add(AudioTab);
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Location = new Point(12, 138);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(1407, 871);
            tabControl1.TabIndex = 11;
            // 
            // SummaryTab
            // 
            SummaryTab.Controls.Add(VideoPreview);
            SummaryTab.Controls.Add(VideoPlayer);
            SummaryTab.Location = new Point(4, 24);
            SummaryTab.Name = "SummaryTab";
            SummaryTab.Padding = new Padding(3);
            SummaryTab.Size = new Size(1399, 843);
            SummaryTab.TabIndex = 0;
            SummaryTab.Text = "Preview";
            SummaryTab.UseVisualStyleBackColor = true;
            // 
            // VideoPreview
            // 
            VideoPreview.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            VideoPreview.BackColor = Color.Black;
            VideoPreview.Location = new Point(3, 3);
            VideoPreview.MediaPlayer = null;
            VideoPreview.Name = "VideoPreview";
            VideoPreview.Size = new Size(1390, 746);
            VideoPreview.TabIndex = 1;
            VideoPreview.Text = "videoView1";
            // 
            // VideoPlayer
            // 
            VideoPlayer.AudioTrack = -1L;
            VideoPlayer.Dock = DockStyle.Fill;
            VideoPlayer.Location = new Point(3, 3);
            VideoPlayer.Name = "VideoPlayer";
            VideoPlayer.Size = new Size(1393, 837);
            VideoPlayer.Sync = null;
            VideoPlayer.TabIndex = 0;
            VideoPlayer.VideoTrack = -1L;
            VideoPlayer.VLC = null;
            VideoPlayer.VlcViewer = VideoPreview;
            // 
            // tabPage1
            // 
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(1399, 843);
            tabPage1.TabIndex = 5;
            tabPage1.Text = "Attachments";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(3, 101);
            label3.Name = "label3";
            label3.Size = new Size(58, 15);
            label3.TabIndex = 12;
            label3.Text = "Directory:";
            // 
            // ContainerLabel
            // 
            ContainerLabel.AutoSize = true;
            ContainerLabel.Location = new Point(71, 71);
            ContainerLabel.Name = "ContainerLabel";
            ContainerLabel.Size = new Size(38, 15);
            ContainerLabel.TabIndex = 13;
            ContainerLabel.Text = "label6";
            // 
            // FileSizeLabel
            // 
            FileSizeLabel.AutoSize = true;
            FileSizeLabel.Location = new Point(71, 86);
            FileSizeLabel.Name = "FileSizeLabel";
            FileSizeLabel.Size = new Size(38, 15);
            FileSizeLabel.TabIndex = 14;
            FileSizeLabel.Text = "label6";
            // 
            // DirectoryLabel
            // 
            DirectoryLabel.AutoSize = true;
            DirectoryLabel.Location = new Point(71, 101);
            DirectoryLabel.Name = "DirectoryLabel";
            DirectoryLabel.Size = new Size(38, 15);
            DirectoryLabel.TabIndex = 15;
            DirectoryLabel.Text = "label6";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(3, 12);
            label1.Name = "label1";
            label1.Size = new Size(65, 15);
            label1.TabIndex = 16;
            label1.Text = "Disc name:";
            // 
            // panel1
            // 
            panel1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panel1.Controls.Add(DiscNameLabel);
            panel1.Controls.Add(DirectoryLabel);
            panel1.Controls.Add(DiscSetLabel);
            panel1.Controls.Add(FileSizeLabel);
            panel1.Controls.Add(label6);
            panel1.Controls.Add(ContainerLabel);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(label3);
            panel1.Controls.Add(TitleList);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(label5);
            panel1.Controls.Add(label4);
            panel1.Location = new Point(12, 12);
            panel1.Name = "panel1";
            panel1.Size = new Size(1407, 120);
            panel1.TabIndex = 17;
            // 
            // DiscNameLabel
            // 
            DiscNameLabel.AutoSize = true;
            DiscNameLabel.Location = new Point(104, 12);
            DiscNameLabel.Name = "DiscNameLabel";
            DiscNameLabel.Size = new Size(59, 15);
            DiscNameLabel.TabIndex = 19;
            DiscNameLabel.Text = "TestName";
            // 
            // DiscSetLabel
            // 
            DiscSetLabel.AutoSize = true;
            DiscSetLabel.Location = new Point(104, 27);
            DiscSetLabel.Name = "DiscSetLabel";
            DiscSetLabel.Size = new Size(24, 15);
            DiscSetLabel.TabIndex = 18;
            DiscSetLabel.Text = "1/1";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(3, 27);
            label6.Name = "label6";
            label6.Size = new Size(95, 15);
            label6.TabIndex = 17;
            label6.Text = "Disc set number:";
            // 
            // ViewInfo
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoScroll = true;
            ClientSize = new Size(1431, 1021);
            Controls.Add(panel1);
            Controls.Add(tabControl1);
            Name = "ViewInfo";
            Text = "ViewInfo";
            FormClosing += ViewInfo_FormClosing;
            Load += ViewInfo_Load;
            AudioTab.ResumeLayout(false);
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            tabControl1.ResumeLayout(false);
            SummaryTab.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)VideoPreview).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private Label label2;
        private ComboBox TitleList;
        private Label label4;
        private Label label5;
        private TabPage AudioTab;
        private ListView TrackList;
        private TabControl tabControl1;
        private SplitContainer splitContainer1;
        private TabPage tabPage1;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
        private ColumnHeader columnHeader3;
        private ColumnHeader columnHeader4;
        private ColumnHeader columnHeader5;
        private ColumnHeader columnHeader6;
        private ColumnHeader columnHeader12;
        private ColumnHeader columnHeader7;
        private ColumnHeader columnHeader13;
        private ColumnHeader columnHeader8;
        private ColumnHeader columnHeader9;
        private ColumnHeader columnHeader10;
        private ColumnHeader columnHeader11;
        private RichTextBox TrackProperties;
        private Label label3;
        private Label ContainerLabel;
        private Label FileSizeLabel;
        private Label DirectoryLabel;
        private ImageList IconImageList;
        private Label label1;
        private Panel panel1;
        private Label DiscNameLabel;
        private Label DiscSetLabel;
        private Label label6;
        private TabPage SummaryTab;
        private LibVLCSharp.WinForms.VideoView VideoPreview;
        private VideoPlayer VideoPlayer;
    }
}