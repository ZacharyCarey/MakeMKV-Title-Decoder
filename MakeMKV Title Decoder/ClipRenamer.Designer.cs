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
            ListViewItem listViewItem4 = new ListViewItem(new string[] { "Test", "testSub" }, -1);
            ListViewItem listViewItem5 = new ListViewItem("test2");
            ListViewItem listViewItem6 = new ListViewItem("test3");
            panel1 = new Panel();
            ClipsList = new ListView();
            columnHeader1 = new ColumnHeader();
            columnHeader2 = new ColumnHeader();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Location = new Point(363, 314);
            panel1.Name = "panel1";
            panel1.Size = new Size(750, 368);
            panel1.TabIndex = 0;
            // 
            // ClipsList
            // 
            ClipsList.Columns.AddRange(new ColumnHeader[] { columnHeader1, columnHeader2 });
            ClipsList.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            ClipsList.Items.AddRange(new ListViewItem[] { listViewItem4, listViewItem5, listViewItem6 });
            ClipsList.Location = new Point(3, 12);
            ClipsList.MultiSelect = false;
            ClipsList.Name = "ClipsList";
            ClipsList.Size = new Size(354, 670);
            ClipsList.TabIndex = 2;
            ClipsList.UseCompatibleStateImageBehavior = false;
            ClipsList.View = View.Details;
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "Source";
            // 
            // columnHeader2
            // 
            columnHeader2.Text = "Rename";
            // 
            // ClipRenamer
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1125, 694);
            Controls.Add(ClipsList);
            Controls.Add(panel1);
            Name = "ClipRenamer";
            Text = "ClipRenamer";
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private ListView ClipsList;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
    }
}