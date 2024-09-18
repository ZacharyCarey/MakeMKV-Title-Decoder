using MakeMKV_Title_Decoder.Controls;
using MakeMKV_Title_Decoder.Data;
using MakeMKV_Title_Decoder.Data.Renames;
using MakeMKV_Title_Decoder.libs.MkvToolNix.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MakeMKV_Title_Decoder
{
    public partial class ClipRenamerForm : Form {

        const string KeepIconKey = "dialog-ok-apply.png";
        const string DeleteIconKey = "dialog-cancel.png";

        public readonly LoadedDisc Disc;

        private LoadedStream? SelectedClip = null;
        private LoadedTrack? SelectedVideoTrack = null;
        private LoadedTrack? SelectedAudioTrack = null;

        public ClipRenamerForm(LoadedDisc disc) {
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

                ListViewItem.ListViewSubItem sourceItem = new(keepIconItem, clip.Data.FileName);
                keepIconItem.SubItems.Add(sourceItem);

                ListViewItem.ListViewSubItem renameItem = new(keepIconItem, "");
                keepIconItem.SubItems.Add(renameItem);

                RefreshClipListItem(keepIconItem, clip);
            }
        }

        private void ClipRenamer_FormClosing(object sender, FormClosingEventArgs e) {
            this.VideoPreview.LoadVideo(null);
        }

        private void SelectClip(LoadedStream? clip) {
            this.SelectedClip = clip;
            this.NameTextBox.Text = (clip == null) ? "" : (clip.Rename.Name ?? "");

            this.PropertiesPanel.Enabled = (clip != null);
            if (clip == null)
            {
                this.VideoPreview.LoadVideo(null);
            } else
            {
                this.VideoPreview.LoadVideo(clip.Data.GetFullPath(this.Disc.Disc));
            }

            this.VideoTrackList.Clear();
            this.AudioTrackList.Clear();
            this.OtherTrackList.Clear();
            VideoTrackList_OnSelectionChanged(null);
            AudioTrackList_OnSelectionChanged(null);
            if (clip != null)
            {
                foreach (var track in clip.Tracks)
                {
                    if (track.Data.Type == MkvTrackType.Video)
                    {
                        this.VideoTrackList.Add(track);
                    } else if (track.Data.Type == MkvTrackType.Audio)
                    {
                        this.AudioTrackList.Add(track);
                    } else
                    {
                        this.OtherTrackList.Add(track);
                    }
                }
            }
        }

        private void RefreshClipListItem(ListViewItem row, LoadedStream data) {
            string name = (data.Rename.Name ?? "");

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

            LoadedStream? selection = (LoadedStream?)ClipsList.SelectedItems[0].Tag;
            SelectClip(selection);
        }

        private void ApplyBtn_Click(object sender, EventArgs e) {
            LoadedStream? selection = this.SelectedClip;
            if (selection != null)
            {
                string? name = this.NameTextBox.Text;
                if (string.IsNullOrWhiteSpace(this.NameTextBox.Text))
                {
                    name = null;
                }

                selection.Rename.Name = name;
                RefreshClipListItem(ClipsList.SelectedItems[0], selection);
            }
        }

        private void compareToolStripMenuItem_Click(object sender, EventArgs e) {
            // TODO make it smart enough to handle changes to Renames while open
            // This can likely be done with a "onChange" event in renames
            new VideoCompareForm(this.Disc).ShowDialog();
        }

        private void SelectTrack(LoadedTrack? track, MkvTrackType type, Panel propertiesPanel, TextBox nameTextBox, CheckBox commentaryCheckBox, CheckBox defaultCheckBox) {
            TrackRename? rename = track?.Rename;

            nameTextBox.Text = (rename?.Name ?? "");
            commentaryCheckBox.Checked = (rename?.CommentaryFlag ?? false);
            defaultCheckBox.Checked = (rename?.DefaultFlag ?? true);
            propertiesPanel.Enabled = (track != null);

            switch (type)
            {
                case MkvTrackType.Video:
                    this.VideoPreview.VideoTrack = track?.Data?.Properties?.Number ?? -1;
                    break;
                case MkvTrackType.Audio:
                    this.VideoPreview.AudioTrack = track?.Data?.Properties?.Number ?? -1;
                    break;
            }
        }

        private void VideoTrackList_OnSelectionChanged(TrackListData? track) {
            this.SelectedVideoTrack = track?.Track;
            SelectTrack(
                track?.Track,
                MkvTrackType.Video,
                VideoTrackPanel,
                VideoTrackName,
                VideoTrackCommentary,
                VideoTrackDefault
            );
        }

        private void AudioTrackList_OnSelectionChanged(TrackListData? track) {
            this.SelectedAudioTrack = track?.Track;
            SelectTrack(
                track?.Track,
                MkvTrackType.Audio,
                AudioTrackPanel,
                AudioTrackName,
                AudioTrackCommentary,
                AudioTrackDefault
            );
        }

        private void TrackApplyChanges(TrackList list, LoadedTrack? track, string? name, bool? commentaryFlag, bool? defaultFlag) {
            if (this.SelectedClip != null && track != null)
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    name = null;
                }

                var renames = track.Rename;
                renames.Name = name;
                renames.CommentaryFlag = commentaryFlag;
                renames.DefaultFlag = defaultFlag;

                list.Update(track);
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

        private void VideoTrackList_SelectedIndexChanged(object sender, EventArgs e) {

        }
    }
}
