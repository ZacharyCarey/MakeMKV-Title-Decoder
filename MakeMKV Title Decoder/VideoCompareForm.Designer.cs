namespace MakeMKV_Title_Decoder {
    partial class VideoCompareForm {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VideoCompareForm));
            ListViewItem listViewItem4 = new ListViewItem(new string[] { "", "003339.m2ts", "Hello" }, "dialog-ok-apply.png");
            ListViewItem listViewItem5 = new ListViewItem("test2");
            ListViewItem listViewItem6 = new ListViewItem("test3");
            splitContainer1 = new SplitContainer();
            splitContainer2 = new SplitContainer();
            ClipsListLeft = new ListView();
            columnHeader3 = new ColumnHeader();
            columnHeader1 = new ColumnHeader();
            columnHeader2 = new ColumnHeader();
            ImageList1 = new ImageList(components);
            VideoViewLeft = new LibVLCSharp.WinForms.VideoView();
            VideoPlayerLeft = new VideoPlayer();
            VideoPlayerRight = new VideoPlayer();
            splitContainer3 = new SplitContainer();
            VideoViewRight = new LibVLCSharp.WinForms.VideoView();
            ClipsListRight = new ListView();
            columnHeader4 = new ColumnHeader();
            columnHeader5 = new ColumnHeader();
            columnHeader6 = new ColumnHeader();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
            splitContainer2.Panel1.SuspendLayout();
            splitContainer2.Panel2.SuspendLayout();
            splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)VideoViewLeft).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitContainer3).BeginInit();
            splitContainer3.Panel1.SuspendLayout();
            splitContainer3.Panel2.SuspendLayout();
            splitContainer3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)VideoViewRight).BeginInit();
            SuspendLayout();
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(splitContainer3);
            splitContainer1.Size = new Size(1647, 529);
            splitContainer1.SplitterDistance = 834;
            splitContainer1.TabIndex = 0;
            // 
            // splitContainer2
            // 
            splitContainer2.Dock = DockStyle.Fill;
            splitContainer2.Location = new Point(0, 0);
            splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            splitContainer2.Panel1.Controls.Add(ClipsListLeft);
            // 
            // splitContainer2.Panel2
            // 
            splitContainer2.Panel2.Controls.Add(VideoViewLeft);
            splitContainer2.Panel2.Controls.Add(VideoPlayerLeft);
            splitContainer2.Size = new Size(834, 529);
            splitContainer2.SplitterDistance = 278;
            splitContainer2.TabIndex = 0;
            // 
            // ClipsListLeft
            // 
            ClipsListLeft.Columns.AddRange(new ColumnHeader[] { columnHeader3, columnHeader1, columnHeader2 });
            ClipsListLeft.Dock = DockStyle.Fill;
            ClipsListLeft.FullRowSelect = true;
            ClipsListLeft.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            ClipsListLeft.Items.AddRange(new ListViewItem[] { listViewItem1, listViewItem2, listViewItem3 });
            ClipsListLeft.Location = new Point(0, 0);
            ClipsListLeft.MultiSelect = false;
            ClipsListLeft.Name = "ClipsListLeft";
            ClipsListLeft.Size = new Size(278, 529);
            ClipsListLeft.SmallImageList = ImageList1;
            ClipsListLeft.TabIndex = 4;
            ClipsListLeft.UseCompatibleStateImageBehavior = false;
            ClipsListLeft.View = View.Details;
            ClipsListLeft.SelectedIndexChanged += ClipsList_SelectedIndexChanged;
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
            // VideoViewLeft
            // 
            VideoViewLeft.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            VideoViewLeft.BackColor = Color.Black;
            VideoViewLeft.Location = new Point(0, 3);
            VideoViewLeft.MediaPlayer = null;
            VideoViewLeft.Name = "VideoViewLeft";
            VideoViewLeft.Size = new Size(549, 436);
            VideoViewLeft.TabIndex = 1;
            VideoViewLeft.Text = "videoView1";
            // 
            // VideoPlayerLeft
            // 
            VideoPlayerLeft.Dock = DockStyle.Fill;
            VideoPlayerLeft.Location = new Point(0, 0);
            VideoPlayerLeft.Name = "VideoPlayerLeft";
            VideoPlayerLeft.Size = new Size(552, 529);
            VideoPlayerLeft.Sync = VideoPlayerRight;
            VideoPlayerLeft.TabIndex = 0;
            VideoPlayerLeft.VLC = null;
            VideoPlayerLeft.VlcViewer = null;
            // 
            // VideoPlayerRight
            // 
            VideoPlayerRight.Dock = DockStyle.Fill;
            VideoPlayerRight.Location = new Point(0, 0);
            VideoPlayerRight.Name = "VideoPlayerRight";
            VideoPlayerRight.Size = new Size(536, 529);
            VideoPlayerRight.Sync = VideoPlayerLeft;
            VideoPlayerRight.TabIndex = 1;
            VideoPlayerRight.VLC = null;
            VideoPlayerRight.VlcViewer = null;
            // 
            // splitContainer3
            // 
            splitContainer3.Dock = DockStyle.Fill;
            splitContainer3.Location = new Point(0, 0);
            splitContainer3.Name = "splitContainer3";
            // 
            // splitContainer3.Panel1
            // 
            splitContainer3.Panel1.Controls.Add(VideoViewRight);
            splitContainer3.Panel1.Controls.Add(VideoPlayerRight);
            // 
            // splitContainer3.Panel2
            // 
            splitContainer3.Panel2.Controls.Add(ClipsListRight);
            splitContainer3.Size = new Size(809, 529);
            splitContainer3.SplitterDistance = 536;
            splitContainer3.TabIndex = 0;
            // 
            // VideoViewRight
            // 
            VideoViewRight.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            VideoViewRight.BackColor = Color.Black;
            VideoViewRight.Location = new Point(3, 3);
            VideoViewRight.MediaPlayer = null;
            VideoViewRight.Name = "VideoViewRight";
            VideoViewRight.Size = new Size(530, 436);
            VideoViewRight.TabIndex = 2;
            VideoViewRight.Text = "videoView2";
            // 
            // ClipsListRight
            // 
            ClipsListRight.Columns.AddRange(new ColumnHeader[] { columnHeader4, columnHeader5, columnHeader6 });
            ClipsListRight.Dock = DockStyle.Fill;
            ClipsListRight.FullRowSelect = true;
            ClipsListRight.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            ClipsListRight.Items.AddRange(new ListViewItem[] { listViewItem4, listViewItem5, listViewItem6 });
            ClipsListRight.Location = new Point(0, 0);
            ClipsListRight.MultiSelect = false;
            ClipsListRight.Name = "ClipsListRight";
            ClipsListRight.Size = new Size(269, 529);
            ClipsListRight.SmallImageList = ImageList1;
            ClipsListRight.TabIndex = 4;
            ClipsListRight.UseCompatibleStateImageBehavior = false;
            ClipsListRight.View = View.Details;
            ClipsListRight.SelectedIndexChanged += ClipsList_SelectedIndexChanged;
            // 
            // columnHeader4
            // 
            columnHeader4.Text = "Keep";
            columnHeader4.Width = 40;
            // 
            // columnHeader5
            // 
            columnHeader5.Text = "Source";
            columnHeader5.Width = 80;
            // 
            // columnHeader6
            // 
            columnHeader6.Text = "Rename";
            columnHeader6.Width = 160;
            // 
            // VideoCompareForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1647, 529);
            Controls.Add(splitContainer1);
            Name = "VideoCompareForm";
            Text = "VideoCompareForm";
            FormClosing += VideoCompareForm_FormClosing;
            Load += VideoCompareForm_Load;
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            splitContainer2.Panel1.ResumeLayout(false);
            splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer2).EndInit();
            splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)VideoViewLeft).EndInit();
            splitContainer3.Panel1.ResumeLayout(false);
            splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer3).EndInit();
            splitContainer3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)VideoViewRight).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private SplitContainer splitContainer1;
        private SplitContainer splitContainer2;
        private ListView ClipsListLeft;
        private ColumnHeader columnHeader3;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
        private SplitContainer splitContainer3;
        private ListView ClipsListRight;
        private ColumnHeader columnHeader4;
        private ColumnHeader columnHeader5;
        private ColumnHeader columnHeader6;
        private VideoPlayer VideoPlayerLeft;
        private VideoPlayer VideoPlayerRight;
        private LibVLCSharp.WinForms.VideoView VideoViewLeft;
        private LibVLCSharp.WinForms.VideoView VideoViewRight;
        private ImageList ImageList1;
    }
}