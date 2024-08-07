using MakeMKV_Title_Decoder.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MakeMKV_Title_Decoder {
    public partial class CompareTracks : Form {
        public CompareTracks(Title leftTitle, Title rightTitle) {
            InitializeComponent();
            this.TreeViewLeft.Tag = this.TextBoxLeft;
            this.TreeViewRight.Tag = this.TextBoxRight;

            LoadTitleTracks(this.TreeViewLeft, leftTitle);
            LoadTitleTracks(this.TreeViewRight, rightTitle);
        }

        private void CompareTracks_Load(object sender, EventArgs e) {

        }

        private void LoadTitleTracks(TreeView tree, Title title) {
            TreeNode rootNode = new(title.OutputFileName);
            rootNode.Tag = title;
            tree.Nodes.Add(rootNode);

            foreach(Track track in title.Tracks)
            {
                TreeNode trackNode = new(track.GetSimplifiedName());
                trackNode.Tag = track;
                rootNode.Nodes.Add(trackNode);

                foreach(string data in track.GetData())
                {
                    TreeNode dataNode = new(data);
                    trackNode.Nodes.Add(dataNode);
                }
            }

            rootNode.Expand();
        }

        private void TreeView_AfterSelect(object sender, TreeViewEventArgs e) {
            TreeView treeView = (TreeView)sender;
            TextBox textBox = (TextBox)treeView.Tag;

            TreeNode node = treeView.SelectedNode;
            if (node.Tag != null && node.Tag is Track track)
            {
                StringBuilder sb = new();
                sb.AppendLine("Track information:");
                foreach (string data in track.GetData())
                {
                    sb.AppendLine(data);
                }
                textBox.Text = sb.ToString();
            } else if (node.Tag != null && node.Tag is Title title)
            {
                StringBuilder sb = new();
                sb.AppendLine("Title information:");
                foreach (string data in title.GetData())
                {
                    sb.AppendLine(data);
                }
                textBox.Text = sb.ToString();
            } else
            {
                textBox.Text = "";
            }
        }

    }
}
