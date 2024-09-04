using MakeMKV_Title_Decoder.MkvToolNix.Data;
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
    public partial class ClipRenamer : Form {

        const string KeepIconKey = "dialog-ok-apply.png";
        const string DeleteIconKey = "dialog-cancel.png";

        public readonly MkvToolNixDisc Disc;
        public Dictionary<MkvMergeID, string> Renames = new();
        public HashSet<MkvMergeID> Deletions = new();

        public ClipRenamer(MkvToolNixDisc disc) {
            this.Disc = disc;

            InitializeComponent();
            this.VideoPreview.VlcViewer = this.VideoView1;
            this.VideoPreview.LoadVLC();

            SelectClip(null);
            ClipsList.Items.Clear();
            foreach (var clip in disc.Streams)
            {
                this.Renames[clip] = "";

                ListViewItem keepIconItem = new("", KeepIconKey);
                keepIconItem.Tag = clip;
                this.ClipsList.Items.Add(keepIconItem);

                ListViewItem.ListViewSubItem sourceItem = new(keepIconItem, clip.FileName);
                keepIconItem.SubItems.Add(sourceItem);

                ListViewItem.ListViewSubItem renameItem = new(keepIconItem, "");
                keepIconItem.SubItems.Add(renameItem);
            }
        }

        private void ClipRenamer_FormClosing(object sender, FormClosingEventArgs e) {
            this.VideoPreview.LoadVideo(null);
        }

        private void SelectClip(MkvMergeID? clip) {
            this.NameTextBox.Text = (clip == null) ? "" : Renames[clip];
            this.DeleteCheckBox.Checked = (clip == null) ? false : this.Deletions.Contains(clip);

            this.PropertiesPanel.Enabled = (clip != null);
            if (clip == null)
            {
                this.VideoPreview.LoadVideo(null);
            } else
            {
                this.VideoPreview.LoadVideo(Path.Combine(clip.FileDirectory, clip.FileName));
            }
        }

        private void RefreshClipListItem(ListViewItem row, MkvMergeID data) {
            row.ImageKey = (this.Deletions.Contains(data) ? DeleteIconKey : KeepIconKey);
            row.SubItems[2].Text = this.Renames[data];
        }




        private void ClipRenamer_Load(object sender, EventArgs e) {

        }

        private void ClipsList_SelectedIndexChanged(object sender, EventArgs e) {
            if (ClipsList.SelectedItems.Count <= 0)
            {
                return;
            }

            MkvMergeID? selection = (MkvMergeID?)ClipsList.SelectedItems[0].Tag;
            SelectClip(selection);
        }

        private void ApplyBtn_Click(object sender, EventArgs e) {
            if (ClipsList.SelectedItems.Count <= 0)
            {
                return;
            }

            MkvMergeID? selection = (MkvMergeID?)ClipsList.SelectedItems[0].Tag;
            if (selection != null)
            {
                this.Renames[selection] = this.NameTextBox.Text;
                if (this.DeleteCheckBox.Checked)
                {
                    this.Deletions.Add(selection);
                } else
                {
                    this.Deletions.Remove(selection);
                }
                RefreshClipListItem(ClipsList.SelectedItems[0], selection);
            }
        }
    }
}
