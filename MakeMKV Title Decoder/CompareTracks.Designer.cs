namespace MakeMKV_Title_Decoder {
    partial class CompareTracks {
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
            splitContainer1 = new SplitContainer();
            splitContainer2 = new SplitContainer();
            splitContainer3 = new SplitContainer();
            TreeViewLeft = new TreeView();
            TreeViewRight = new TreeView();
            TextBoxLeft = new TextBox();
            TextBoxRight = new TextBox();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
            splitContainer2.Panel1.SuspendLayout();
            splitContainer2.Panel2.SuspendLayout();
            splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer3).BeginInit();
            splitContainer3.Panel1.SuspendLayout();
            splitContainer3.Panel2.SuspendLayout();
            splitContainer3.SuspendLayout();
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
            splitContainer1.Size = new Size(1054, 769);
            splitContainer1.SplitterDistance = 446;
            splitContainer1.TabIndex = 0;
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
            splitContainer2.Panel1.Controls.Add(TreeViewLeft);
            // 
            // splitContainer2.Panel2
            // 
            splitContainer2.Panel2.Controls.Add(TextBoxLeft);
            splitContainer2.Size = new Size(446, 769);
            splitContainer2.SplitterDistance = 547;
            splitContainer2.TabIndex = 0;
            // 
            // splitContainer3
            // 
            splitContainer3.Dock = DockStyle.Fill;
            splitContainer3.Location = new Point(0, 0);
            splitContainer3.Name = "splitContainer3";
            splitContainer3.Orientation = Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            splitContainer3.Panel1.Controls.Add(TreeViewRight);
            // 
            // splitContainer3.Panel2
            // 
            splitContainer3.Panel2.Controls.Add(TextBoxRight);
            splitContainer3.Size = new Size(604, 769);
            splitContainer3.SplitterDistance = 519;
            splitContainer3.TabIndex = 0;
            // 
            // TreeViewLeft
            // 
            TreeViewLeft.Dock = DockStyle.Fill;
            TreeViewLeft.HideSelection = false;
            TreeViewLeft.Location = new Point(0, 0);
            TreeViewLeft.Name = "TreeViewLeft";
            TreeViewLeft.Size = new Size(446, 547);
            TreeViewLeft.TabIndex = 0;
            TreeViewLeft.AfterSelect += TreeView_AfterSelect;
            // 
            // TreeViewRight
            // 
            TreeViewRight.Dock = DockStyle.Fill;
            TreeViewRight.HideSelection = false;
            TreeViewRight.Location = new Point(0, 0);
            TreeViewRight.Name = "TreeViewRight";
            TreeViewRight.Size = new Size(604, 519);
            TreeViewRight.TabIndex = 0;
            TreeViewRight.AfterSelect += TreeView_AfterSelect;
            // 
            // TextBoxLeft
            // 
            TextBoxLeft.Dock = DockStyle.Fill;
            TextBoxLeft.Location = new Point(0, 0);
            TextBoxLeft.Multiline = true;
            TextBoxLeft.Name = "TextBoxLeft";
            TextBoxLeft.ReadOnly = true;
            TextBoxLeft.ScrollBars = ScrollBars.Both;
            TextBoxLeft.Size = new Size(446, 218);
            TextBoxLeft.TabIndex = 0;
            // 
            // TextBoxRight
            // 
            TextBoxRight.Dock = DockStyle.Fill;
            TextBoxRight.Location = new Point(0, 0);
            TextBoxRight.Multiline = true;
            TextBoxRight.Name = "TextBoxRight";
            TextBoxRight.ReadOnly = true;
            TextBoxRight.ScrollBars = ScrollBars.Both;
            TextBoxRight.Size = new Size(604, 246);
            TextBoxRight.TabIndex = 0;
            // 
            // CompareTracks
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1054, 769);
            Controls.Add(splitContainer1);
            Name = "CompareTracks";
            Text = "CompareTracks";
            Load += CompareTracks_Load;
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            splitContainer2.Panel1.ResumeLayout(false);
            splitContainer2.Panel2.ResumeLayout(false);
            splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).EndInit();
            splitContainer2.ResumeLayout(false);
            splitContainer3.Panel1.ResumeLayout(false);
            splitContainer3.Panel2.ResumeLayout(false);
            splitContainer3.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer3).EndInit();
            splitContainer3.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private SplitContainer splitContainer1;
        private SplitContainer splitContainer2;
        private TreeView TreeViewLeft;
        private TextBox TextBoxLeft;
        private SplitContainer splitContainer3;
        private TreeView TreeViewRight;
        private TextBox TextBoxRight;
    }
}