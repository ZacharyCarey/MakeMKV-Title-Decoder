namespace MakeMKV_Title_Decoder.Forms
{
    partial class AttachmentsForm
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
            components = new System.ComponentModel.Container();
            ListViewItem listViewItem1 = new ListViewItem(new string[] { "", "003339.m2ts", "Hello" }, "dialog-ok-apply.png");
            ListViewItem listViewItem2 = new ListViewItem("test2");
            ListViewItem listViewItem3 = new ListViewItem("test3");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AttachmentsForm));
            splitContainer1 = new SplitContainer();
            AttachmentsList = new ListView();
            columnHeader3 = new ColumnHeader();
            columnHeader1 = new ColumnHeader();
            columnHeader2 = new ColumnHeader();
            ImageList1 = new ImageList(components);
            PropertiesPanel = new Panel();
            ApplyBtn = new Button();
            NameTextBox = new TextBox();
            label1 = new Label();
            videoView1 = new LibVLCSharp.WinForms.VideoView();
            VideoPreview = new VideoPlayer();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            PropertiesPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)videoView1).BeginInit();
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
            splitContainer1.Panel1.Controls.Add(AttachmentsList);
            splitContainer1.Panel1.Controls.Add(PropertiesPanel);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(videoView1);
            splitContainer1.Panel2.Controls.Add(VideoPreview);
            splitContainer1.Size = new Size(1539, 938);
            splitContainer1.SplitterDistance = 554;
            splitContainer1.TabIndex = 4;
            // 
            // AttachmentsList
            // 
            AttachmentsList.Columns.AddRange(new ColumnHeader[] { columnHeader3, columnHeader1, columnHeader2 });
            AttachmentsList.Dock = DockStyle.Fill;
            AttachmentsList.FullRowSelect = true;
            AttachmentsList.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            AttachmentsList.Items.AddRange(new ListViewItem[] { listViewItem1, listViewItem2, listViewItem3 });
            AttachmentsList.Location = new Point(0, 0);
            AttachmentsList.MultiSelect = false;
            AttachmentsList.Name = "AttachmentsList";
            AttachmentsList.Size = new Size(550, 868);
            AttachmentsList.SmallImageList = ImageList1;
            AttachmentsList.TabIndex = 2;
            AttachmentsList.UseCompatibleStateImageBehavior = false;
            AttachmentsList.View = View.Details;
            AttachmentsList.SelectedIndexChanged += AttachmentsList_SelectedIndexChanged;
            // 
            // columnHeader3
            // 
            columnHeader3.Text = "Keep";
            columnHeader3.Width = 40;
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "Source";
            columnHeader1.Width = 240;
            // 
            // columnHeader2
            // 
            columnHeader2.Text = "Rename";
            columnHeader2.Width = 240;
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
            // PropertiesPanel
            // 
            PropertiesPanel.Controls.Add(ApplyBtn);
            PropertiesPanel.Controls.Add(NameTextBox);
            PropertiesPanel.Controls.Add(label1);
            PropertiesPanel.Dock = DockStyle.Bottom;
            PropertiesPanel.Location = new Point(0, 868);
            PropertiesPanel.Name = "PropertiesPanel";
            PropertiesPanel.Size = new Size(550, 66);
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
            NameTextBox.Size = new Size(496, 23);
            NameTextBox.TabIndex = 1;
            NameTextBox.TextChanged += NameTextBox_TextChanged;
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
            // videoView1
            // 
            videoView1.BackColor = Color.Black;
            videoView1.Dock = DockStyle.Fill;
            videoView1.Location = new Point(0, 0);
            videoView1.MediaPlayer = null;
            videoView1.Name = "videoView1";
            videoView1.Size = new Size(977, 843);
            videoView1.TabIndex = 2;
            videoView1.Text = "videoView2";
            // 
            // VideoPreview
            // 
            VideoPreview.AudioTrack = -1L;
            VideoPreview.Dock = DockStyle.Bottom;
            VideoPreview.Location = new Point(0, 843);
            VideoPreview.Name = "VideoPreview";
            VideoPreview.Size = new Size(977, 91);
            VideoPreview.Sync = null;
            VideoPreview.TabIndex = 1;
            VideoPreview.VideoTrack = -1L;
            VideoPreview.VLC = null;
            VideoPreview.VlcViewer = videoView1;
            // 
            // AttachmentsForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1539, 938);
            Controls.Add(splitContainer1);
            Name = "AttachmentsForm";
            Text = "AttachmentsForm";
            FormClosing += AttachmentsForm_FormClosing;
            Load += AttachmentsForm_Load;
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            PropertiesPanel.ResumeLayout(false);
            PropertiesPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)videoView1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private SplitContainer splitContainer1;
        private ListView AttachmentsList;
        private ColumnHeader columnHeader3;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
        private Panel PropertiesPanel;
        private Button ApplyBtn;
        private TextBox NameTextBox;
        private Label label1;
        private VideoPlayer VideoPreview;
        private LibVLCSharp.WinForms.VideoView videoView1;
        private ImageList ImageList1;
    }
}