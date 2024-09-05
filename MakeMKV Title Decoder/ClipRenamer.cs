using MakeMKV_Title_Decoder.Data;
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
        public readonly RenameData2 Renames;

        public ClipRenamer(RenameData2 renames, MkvToolNixDisc disc) {
            this.Renames = renames;
            this.Disc = disc;

            InitializeComponent();
            this.VideoPreview.VlcViewer = this.VideoView1;
            this.VideoPreview.LoadVLC();

            SelectClip(null);
            ClipsList.Items.Clear();
            foreach (var clip in disc.Streams)
            {
                ListViewItem keepIconItem = new("", KeepIconKey);
                keepIconItem.Tag = clip;
                this.ClipsList.Items.Add(keepIconItem);

                ListViewItem.ListViewSubItem sourceItem = new(keepIconItem, clip.FileName);
                keepIconItem.SubItems.Add(sourceItem);

                ListViewItem.ListViewSubItem renameItem = new(keepIconItem, "");
                keepIconItem.SubItems.Add(renameItem);

                RefreshClipListItem(keepIconItem, clip);
            }
        }

        private void ClipRenamer_FormClosing(object sender, FormClosingEventArgs e) {
            this.VideoPreview.LoadVideo(null);
        }

        private void SelectClip(MkvMergeID? clip) {
            this.NameTextBox.Text = (clip == null) ? "" : (Renames.GetUserGivenName(clip) ?? "");

            this.PropertiesPanel.Enabled = (clip != null);
            if (clip == null)
            {
                this.VideoPreview.LoadVideo(null);
            } else
            {
                this.VideoPreview.LoadVideo(clip.GetFullPath(this.Disc));
            }
            //this.VideoView1.MediaPlayer.AudioTrackCount;
            //this.VideoView1.MediaPlayer.VideoTrackDescription[0].

            this.VideoTrackList.Clear();
            this.AudioTrackList.Clear();
            if (clip != null)
            {
                foreach (var track in clip.Tracks)
                {
                    if (track.Type == MkvTrackType.Video)
                    {
                        this.VideoTrackList.Add(track, this.Renames);
                    } else if (track.Type == MkvTrackType.Audio)
                    {
                        this.AudioTrackList.Add(track, this.Renames);
                    }
                }
            }
        }

        private void RefreshClipListItem(ListViewItem row, MkvMergeID data) {
            string name = (this.Renames.GetUserGivenName(data) ?? "");

            row.ImageKey = (string.IsNullOrWhiteSpace(name) ? DeleteIconKey : KeepIconKey);
            row.SubItems[2].Text = name;
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
                string? name = this.NameTextBox.Text;
                if (string.IsNullOrWhiteSpace(this.NameTextBox.Text))
                {
                    name = null;
                }

                this.Renames.SetUserGivenName(selection, name);
                RefreshClipListItem(ClipsList.SelectedItems[0], selection);
            }
        }

        private void compareToolStripMenuItem_Click(object sender, EventArgs e) {
            // TODO make it smart enough to handle changes to Renames while open
            // This can likely be done with a "onChange" event in renames
            new VideoCompareForm(this.Renames, this.Disc).ShowDialog();
        }

        private void AudioTrackList_OnSelectionChanged(MkvTrack? track) {
            var player = this.VideoView1.MediaPlayer;
            if (player != null && track != null)
            {
                player.SetAudioTrack(player.AudioTrackDescription[track.ID].Id);
                //int count = player.AudioTrackCount;
                //var description = player.AudioTrackDescription;
            }
        }
    }
}
