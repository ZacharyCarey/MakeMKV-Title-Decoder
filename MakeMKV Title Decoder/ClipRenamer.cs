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

        private MkvMergeID? SelectedClip = null;
        private MkvTrack? SelectedVideoTrack = null;
        private MkvTrack? SelectedAudioTrack = null;

        public ClipRenamer(RenameData2 renames, MkvToolNixDisc disc) {
            this.Renames = renames;
            this.Disc = disc;

            InitializeComponent();
            this.VideoPreview.VlcViewer = this.VideoView1;
            this.VideoPreview.LoadVLC();

            SelectClip(null);
            VideoTrackList_OnSelectionChanged(null);
            AudioTrackList_OnSelectionChanged(null);
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
            this.SelectedClip = clip;
            this.NameTextBox.Text = (clip == null) ? "" : (Renames.GetClipRename(clip)?.Name ?? "");

            this.PropertiesPanel.Enabled = (clip != null);
            if (clip == null)
            {
                this.VideoPreview.LoadVideo(null);
            } else
            {
                this.VideoPreview.LoadVideo(clip.GetFullPath(this.Disc));
            }

            this.VideoTrackList.Clear();
            this.AudioTrackList.Clear();
            VideoTrackList_OnSelectionChanged(null);
            AudioTrackList_OnSelectionChanged(null);
            if (clip != null)
            {
                foreach (var track in clip.Tracks)
                {
                    if (track.Type == MkvTrackType.Video)
                    {
                        this.VideoTrackList.Add(clip, track, this.Renames);
                    } else if (track.Type == MkvTrackType.Audio)
                    {
                        this.AudioTrackList.Add(clip, track, this.Renames);
                    }
                }
            }
        }

        private void RefreshClipListItem(ListViewItem row, MkvMergeID data) {
            string name = (this.Renames.GetClipRename(data)?.Name ?? "");

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
            MkvMergeID? selection = this.SelectedClip;
            if (selection != null)
            {
                string? name = this.NameTextBox.Text;
                if (string.IsNullOrWhiteSpace(this.NameTextBox.Text))
                {
                    name = null;
                }

                this.Renames.GetClipRename(selection, true)?.SetName(name);
                RefreshClipListItem(ClipsList.SelectedItems[0], selection);
            }
        }

        private void compareToolStripMenuItem_Click(object sender, EventArgs e) {
            // TODO make it smart enough to handle changes to Renames while open
            // This can likely be done with a "onChange" event in renames
            new VideoCompareForm(this.Renames, this.Disc).ShowDialog();
        }

        private void SelectTrack(MkvTrack? track, MkvTrackType type, Panel propertiesPanel, TextBox nameTextBox, CheckBox commentaryCheckBox, CheckBox defaultCheckBox) {
            TrackRename? rename = null;
            if (this.SelectedClip != null && track != null) rename = Renames.GetClipRename(this.SelectedClip)?.GetTrackRename(track);

            nameTextBox.Text = (rename?.Name ?? "");
            commentaryCheckBox.Checked = (rename?.CommentaryFlag ?? false);
            defaultCheckBox.Checked = (rename?.DefaultFlag ?? true);
            propertiesPanel.Enabled = (track != null);

            switch (type)
            {
                case MkvTrackType.Video:
                    this.VideoPreview.VideoTrack = track?.Properties?.Number ?? -1;
                    break;
                case MkvTrackType.Audio:
                    this.VideoPreview.AudioTrack = track?.Properties?.Number ?? -1;
                    break;
            }
        }

        private void VideoTrackList_OnSelectionChanged(MkvTrack? track) {
            this.SelectedVideoTrack = track;
            SelectTrack(
                track, 
                MkvTrackType.Video, 
                VideoTrackPanel, 
                VideoTrackName, 
                VideoTrackCommentary, 
                VideoTrackDefault
            );
        }

        private void AudioTrackList_OnSelectionChanged(MkvTrack? track) {
            this.SelectedAudioTrack = track;
            SelectTrack(
                track, 
                MkvTrackType.Audio, 
                AudioTrackPanel, 
                AudioTrackName, 
                AudioTrackCommentary,
                AudioTrackDefault
            );
        }

        private void TrackApplyChanges(TrackList list, MkvTrack? track, string? name, bool? commentaryFlag, bool? defaultFlag) {
            if (this.SelectedClip != null && track != null)
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    name = null;
                }

                var renames = this.Renames.GetClipRename(this.SelectedClip, true)?.GetTrackRename(track, true);
                renames?.SetName(name);
                renames?.SetCommentaryFlag(commentaryFlag);
                renames?.SetDefaultFlag(defaultFlag);

                list.Update(this.SelectedClip, track, this.Renames);
            }
        }

        private void VideoTrackApply_Click(object sender, EventArgs e) {
            TrackApplyChanges(
                this.VideoTrackList,
                this.SelectedVideoTrack,
                this.VideoTrackName.Text,
                this.VideoTrackCommentary.Checked,
                this.VideoTrackDefault.Checked
            );
        }

        private void AudioTrackApply_Click(object sender, EventArgs e) {
            TrackApplyChanges(
                this.AudioTrackList,
                this.SelectedAudioTrack,
                this.AudioTrackName.Text,
                this.AudioTrackCommentary.Checked,
                this.AudioTrackDefault.Checked
            );
        }
    }
}
