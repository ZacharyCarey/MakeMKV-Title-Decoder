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
    public partial class FolderManager : Form {
        public Folder? SelectedFolder = null;

        Folder RootFolder;

        public FolderManager(Folder rootFolder) {
            this.DialogResult = DialogResult.None;
            this.RootFolder = rootFolder;
            InitializeComponent();
        }

        private void FolderManager_Load(object sender, EventArgs e) {
            AddFolderNode(RootFolder, null);
            this.FolderTreeView.ExpandAll();
        }

        private TreeNode AddFolderNode(Folder folder, TreeNode? parentNode) {
            TreeNode node = new(folder.Name);
            node.Tag = folder;
            if (parentNode == null)
            {
                this.FolderTreeView.Nodes.Add(node);
            } else
            {
                parentNode.Nodes.Add(node);
            }

            foreach (Folder child in folder.Children)
            {
                AddFolderNode(child, node);
            }

            return node;
        }

        private void FolderManager_FormClosing(object sender, FormClosingEventArgs e) {
            if (this.DialogResult != DialogResult.OK)
            {
                SelectedFolder = null;
            }
        }

        private void AddBtn_Click(object sender, EventArgs e) {
            TreeNode selectedNode = FolderTreeView.SelectedNode;
            if (selectedNode == null) return;
            Folder selectedFolder = (Folder)selectedNode.Tag;

            string name = NewFolderNameTextBox.Text;
            if (string.IsNullOrWhiteSpace(name) || !isValidFileName(name))
            {
                MessageBox.Show("Invalid folder name.");
                return;
            }
            if (!uniqueName(selectedFolder, name))
            {
                MessageBox.Show("A folder with that name already exists");
                return;
            }

            Folder folder = new Folder();
            folder.Parent = selectedFolder;
            folder.Name = name;
            selectedFolder.Children.Add(folder);

            TreeNode newNode = AddFolderNode(folder, selectedNode);
            selectedNode.Expand();
            this.FolderTreeView.SelectedNode = newNode;
        }

        private void DeleteBtn_Click(object sender, EventArgs e) {
            TreeNode node = FolderTreeView.SelectedNode;
            if (node != null)
            {
                Folder folder = (Folder)node.Tag;
                if (folder == RootFolder)
                {
                    MessageBox.Show("Can't delete the root folder.");
                    return;
                }

                // Restructure folders
                folder.Parent?.Children?.Remove(folder);
                DeleteFolders(folder);

                // Restructure UI
                node.Remove();
            }
        }

        private void DeleteFolders(Folder folder) {
            foreach (Folder f in folder.Children)
            {
                DeleteFolders(f);
            }
            folder.Reset();
        }

        private void SelectBtn_Click(object sender, EventArgs e) {
            this.DialogResult = DialogResult.OK;
            TreeNode selectedNode = this.FolderTreeView.SelectedNode;
            if (selectedNode != null)
            {
                Folder folder = (Folder)selectedNode.Tag;
                this.SelectedFolder = folder;
            }

            this.Close();
        }

        private void FolderTreeView_AfterSelect(object sender, TreeViewEventArgs e) {
            TreeNode node = FolderTreeView.SelectedNode;
            bool valid = false;
            if (node != null)
            {
                Folder selectedFolder = (Folder)node.Tag;
                valid = (selectedFolder != null && selectedFolder.IsValidFolder);
            }

            this.AddBtn.Enabled = valid;
            this.DeleteBtn.Enabled = valid;
            this.SelectBtn.Enabled = valid;
        }

        private void NewFolderNameTextBox_TextChanged(object sender, EventArgs e) {
            this.InvalidNameLabel.Visible = !isValidFileName(NewFolderNameTextBox.Text);
        }

        private bool uniqueName(Folder parent, string name) {
            name = name.ToLower();
            foreach (var children in parent.Children)
            {
                if (children.Name.ToLower() == name)
                {
                    return false;
                }
            }
            return true;
        }

        private bool isValidFileName(string name) {
            char[] fileChars = Path.GetInvalidFileNameChars();
            char[] pathChars = Path.GetInvalidPathChars();
            foreach (char c in name)
            {
                if (fileChars.Contains(c) || pathChars.Contains(c))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
