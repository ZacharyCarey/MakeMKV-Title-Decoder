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
            splitContainer3 = new SplitContainer();
            PropertiesPanel = new Panel();
            ApplyBtn = new Button();
            NameTextBox = new TextBox();
            label1 = new Label();
            splitContainer2 = new SplitContainer();
            VideoView1 = new LibVLCSharp.WinForms.VideoView();
            VideoPreview = new VideoPlayer();
            splitContainer4 = new SplitContainer();
            VideoTrackList = new TrackList();
            panel1 = new Panel();
            button1 = new Button();
            checkBox1 = new CheckBox();
            textBox1 = new TextBox();
            label2 = new Label();
            panel2 = new Panel();
            button2 = new Button();
            checkBox2 = new CheckBox();
            textBox2 = new TextBox();
            label3 = new Label();
            AudioTrackList = new TrackList();
            menuStrip1 = new MenuStrip();
            compareToolStripMenuItem = new ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer3).BeginInit();
            splitContainer3.Panel1.SuspendLayout();
            splitContainer3.Panel2.SuspendLayout();
            splitContainer3.SuspendLayout();
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
            panel1.SuspendLayout();
            panel2.SuspendLayout();
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
            ClipsList.Size = new Size(334, 735);
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
            splitContainer1.Panel1.Controls.Add(splitContainer3);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(splitContainer2);
            splitContainer1.Size = new Size(1715, 1047);
            splitContainer1.SplitterDistance = 338;
            splitContainer1.TabIndex = 3;
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
            splitContainer3.Panel1.Controls.Add(ClipsList);
            // 
            // splitContainer3.Panel2
            // 
            splitContainer3.Panel2.Controls.Add(PropertiesPanel);
            splitContainer3.Size = new Size(338, 1047);
            splitContainer3.SplitterDistance = 739;
            splitContainer3.TabIndex = 0;
            // 
            // PropertiesPanel
            // 
            PropertiesPanel.AutoScroll = true;
            PropertiesPanel.BorderStyle = BorderStyle.Fixed3D;
            PropertiesPanel.Controls.Add(ApplyBtn);
            PropertiesPanel.Controls.Add(NameTextBox);
            PropertiesPanel.Controls.Add(label1);
            PropertiesPanel.Dock = DockStyle.Fill;
            PropertiesPanel.Location = new Point(0, 0);
            PropertiesPanel.Name = "PropertiesPanel";
            PropertiesPanel.Size = new Size(334, 300);
            PropertiesPanel.TabIndex = 0;
            // 
            // ApplyBtn
            // 
            ApplyBtn.Location = new Point(3, 72);
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
            NameTextBox.Size = new Size(278, 23);
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
            splitContainer2.Size = new Size(1373, 1047);
            splitContainer2.SplitterDistance = 842;
            splitContainer2.TabIndex = 0;
            // 
            // VideoView1
            // 
            VideoView1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            VideoView1.BackColor = Color.Black;
            VideoView1.Location = new Point(0, 3);
            VideoView1.MediaPlayer = null;
            VideoView1.Name = "VideoView1";
            VideoView1.Size = new Size(835, 947);
            VideoView1.TabIndex = 1;
            VideoView1.Text = "videoView1";
            // 
            // VideoPreview
            // 
            VideoPreview.Dock = DockStyle.Fill;
            VideoPreview.Location = new Point(0, 0);
            VideoPreview.Name = "VideoPreview";
            VideoPreview.Size = new Size(838, 1043);
            VideoPreview.Sync = null;
            VideoPreview.TabIndex = 0;
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
            splitContainer4.Panel1.Controls.Add(panel1);
            // 
            // splitContainer4.Panel2
            // 
            splitContainer4.Panel2.Controls.Add(panel2);
            splitContainer4.Panel2.Controls.Add(AudioTrackList);
            splitContainer4.Size = new Size(527, 1047);
            splitContainer4.SplitterDistance = 550;
            splitContainer4.TabIndex = 0;
            // 
            // VideoTrackList
            // 
            VideoTrackList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            VideoTrackList.DeleteIconKey = "dialog-cancel.png";
            VideoTrackList.FullRowSelect = true;
            VideoTrackList.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            VideoTrackList.KeepIconKey = "dialog-ok-apply.png";
            VideoTrackList.Location = new Point(3, 3);
            VideoTrackList.MultiSelect = false;
            VideoTrackList.Name = "VideoTrackList";
            VideoTrackList.OwnerDraw = true;
            VideoTrackList.Size = new Size(517, 450);
            VideoTrackList.SmallImageList = ImageList1;
            VideoTrackList.TabIndex = 1;
            VideoTrackList.UseCompatibleStateImageBehavior = false;
            VideoTrackList.View = View.Details;
            // 
            // panel1
            // 
            panel1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panel1.Controls.Add(button1);
            panel1.Controls.Add(checkBox1);
            panel1.Controls.Add(textBox1);
            panel1.Controls.Add(label2);
            panel1.Location = new Point(3, 459);
            panel1.Name = "panel1";
            panel1.Size = new Size(517, 84);
            panel1.TabIndex = 0;
            // 
            // button1
            // 
            button1.Location = new Point(3, 57);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 5;
            button1.Text = "Apply";
            button1.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Location = new Point(3, 32);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(96, 19);
            checkBox1.TabIndex = 4;
            checkBox1.Text = "Commentary";
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            textBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBox1.Location = new Point(51, 3);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(463, 23);
            textBox1.TabIndex = 3;
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
            // panel2
            // 
            panel2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panel2.Controls.Add(button2);
            panel2.Controls.Add(checkBox2);
            panel2.Controls.Add(textBox2);
            panel2.Controls.Add(label3);
            panel2.Location = new Point(3, 402);
            panel2.Name = "panel2";
            panel2.Size = new Size(517, 84);
            panel2.TabIndex = 3;
            // 
            // button2
            // 
            button2.Location = new Point(3, 57);
            button2.Name = "button2";
            button2.Size = new Size(75, 23);
            button2.TabIndex = 5;
            button2.Text = "Apply";
            button2.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            checkBox2.AutoSize = true;
            checkBox2.Location = new Point(3, 32);
            checkBox2.Name = "checkBox2";
            checkBox2.Size = new Size(96, 19);
            checkBox2.TabIndex = 4;
            checkBox2.Text = "Commentary";
            checkBox2.UseVisualStyleBackColor = true;
            // 
            // textBox2
            // 
            textBox2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBox2.Location = new Point(51, 3);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(784, 23);
            textBox2.TabIndex = 3;
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
            // AudioTrackList
            // 
            AudioTrackList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            AudioTrackList.DeleteIconKey = "dialog-cancel.png";
            AudioTrackList.FullRowSelect = true;
            AudioTrackList.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            AudioTrackList.KeepIconKey = "dialog-ok-apply.png";
            AudioTrackList.Location = new Point(2, 3);
            AudioTrackList.MultiSelect = false;
            AudioTrackList.Name = "AudioTrackList";
            AudioTrackList.OwnerDraw = true;
            AudioTrackList.Size = new Size(517, 396);
            AudioTrackList.SmallImageList = ImageList1;
            AudioTrackList.TabIndex = 2;
            AudioTrackList.UseCompatibleStateImageBehavior = false;
            AudioTrackList.View = View.Details;
            AudioTrackList.OnSelectionChanged += AudioTrackList_OnSelectionChanged;
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { compareToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1715, 24);
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
            // ClipRenamer
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1715, 1071);
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
            splitContainer3.Panel1.ResumeLayout(false);
            splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer3).EndInit();
            splitContainer3.ResumeLayout(false);
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
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
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
        private SplitContainer splitContainer3;
        private SplitContainer splitContainer4;
        private Panel panel1;
        private TextBox textBox1;
        private Label label2;
        private CheckBox checkBox1;
        private Button button1;
        private TrackList VideoTrackList;
        private Panel panel2;
        private Button button2;
        private CheckBox checkBox2;
        private TextBox textBox2;
        private Label label3;
        private TrackList AudioTrackList;
    }
}