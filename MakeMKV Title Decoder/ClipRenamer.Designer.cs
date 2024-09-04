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
            ListViewItem listViewItem4 = new ListViewItem(new string[] { "", "003339.m2ts", "Hello" }, "dialog-ok-apply.png");
            ListViewItem listViewItem5 = new ListViewItem("test2");
            ListViewItem listViewItem6 = new ListViewItem("test3");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ClipRenamer));
            ClipsList = new ListView();
            columnHeader3 = new ColumnHeader();
            columnHeader1 = new ColumnHeader();
            columnHeader2 = new ColumnHeader();
            ImageList1 = new ImageList(components);
            splitContainer1 = new SplitContainer();
            splitContainer2 = new SplitContainer();
            VideoView1 = new LibVLCSharp.WinForms.VideoView();
            VideoPreview = new VideoPlayer();
            PropertiesPanel = new Panel();
            ApplyBtn = new Button();
            NameTextBox = new TextBox();
            label1 = new Label();
            menuStrip1 = new MenuStrip();
            compareToolStripMenuItem = new ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
            splitContainer2.Panel1.SuspendLayout();
            splitContainer2.Panel2.SuspendLayout();
            splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)VideoView1).BeginInit();
            PropertiesPanel.SuspendLayout();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // ClipsList
            // 
            ClipsList.Columns.AddRange(new ColumnHeader[] { columnHeader3, columnHeader1, columnHeader2 });
            ClipsList.Dock = DockStyle.Fill;
            ClipsList.FullRowSelect = true;
            ClipsList.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            ClipsList.Items.AddRange(new ListViewItem[] { listViewItem4, listViewItem5, listViewItem6 });
            ClipsList.Location = new Point(0, 0);
            ClipsList.MultiSelect = false;
            ClipsList.Name = "ClipsList";
            ClipsList.Size = new Size(282, 670);
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
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 24);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(ClipsList);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(splitContainer2);
            splitContainer1.Size = new Size(1125, 670);
            splitContainer1.SplitterDistance = 282;
            splitContainer1.TabIndex = 3;
            // 
            // splitContainer2
            // 
            splitContainer2.Dock = DockStyle.Fill;
            splitContainer2.Location = new Point(0, 0);
            splitContainer2.Name = "splitContainer2";
            splitContainer2.Orientation = Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            splitContainer2.Panel1.Controls.Add(VideoView1);
            splitContainer2.Panel1.Controls.Add(VideoPreview);
            // 
            // splitContainer2.Panel2
            // 
            splitContainer2.Panel2.Controls.Add(PropertiesPanel);
            splitContainer2.Size = new Size(839, 670);
            splitContainer2.SplitterDistance = 558;
            splitContainer2.TabIndex = 0;
            // 
            // VideoView1
            // 
            VideoView1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            VideoView1.BackColor = Color.Black;
            VideoView1.Location = new Point(0, 3);
            VideoView1.MediaPlayer = null;
            VideoView1.Name = "VideoView1";
            VideoView1.Size = new Size(836, 462);
            VideoView1.TabIndex = 1;
            VideoView1.Text = "videoView1";
            // 
            // VideoPreview
            // 
            VideoPreview.Dock = DockStyle.Fill;
            VideoPreview.Location = new Point(0, 0);
            VideoPreview.Name = "VideoPreview";
            VideoPreview.Size = new Size(839, 558);
            VideoPreview.TabIndex = 0;
            VideoPreview.VLC = null;
            VideoPreview.VlcViewer = null;
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
            PropertiesPanel.Size = new Size(839, 108);
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
            NameTextBox.Location = new Point(51, 3);
            NameTextBox.Name = "NameTextBox";
            NameTextBox.Size = new Size(275, 23);
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
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { compareToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1125, 24);
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
            ClientSize = new Size(1125, 694);
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
            splitContainer2.Panel1.ResumeLayout(false);
            splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer2).EndInit();
            splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)VideoView1).EndInit();
            PropertiesPanel.ResumeLayout(false);
            PropertiesPanel.PerformLayout();
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
    }
}