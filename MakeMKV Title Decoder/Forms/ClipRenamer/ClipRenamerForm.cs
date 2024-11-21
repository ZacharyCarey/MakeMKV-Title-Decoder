using MakeMKV_Title_Decoder.Controls;
using MakeMKV_Title_Decoder.Data;
using MakeMKV_Title_Decoder.Data.Renames;
using MkvToolNix.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;

namespace MakeMKV_Title_Decoder
{
    public partial class ClipRenamerForm : Form {

        const string KeepIconKey = "dialog-ok-apply.png";
        const string DeleteIconKey = "dialog-cancel.png";

        public readonly LoadedDisc Disc;

        private LoadedStream? SelectedClip = null;
        private LoadedTrack? SelectedVideoTrack = null;
        private LoadedTrack? SelectedAudioTrack = null;
        private LoadedTrack? SelectedOtherTrack = null;

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

                ListViewItem.ListViewSubItem sourceItem = new(keepIconItem, clip.Identity.SourceFile);
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
            this.NameTextBox.Text = (clip == null) ? "" : (clip.RenameData.Name ?? "");

            this.PropertiesPanel.Enabled = (clip != null);
            if (clip == null)
            {
                this.VideoPreview.LoadVideo(null);
            } else
            {
                string path = Path.Combine(this.Disc.Root, clip.Identity.SourceFile);
                this.VideoPreview.LoadVideo(path);
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
                    if (track.Identity.Type == MkvTrackType.Video)
                    {
                        this.VideoTrackList.Add(track);
                    } else if (track.Identity.Type == MkvTrackType.Audio)
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
            string name = (data.RenameData.Name ?? "");

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

                selection.RenameData.Name = name;
                RefreshClipListItem(ClipsList.SelectedItems[0], selection);
            }
        }

        private void compareToolStripMenuItem_Click(object sender, EventArgs e) {
            // TODO make it smart enough to handle changes to Renames while open
            // This can likely be done with a "onChange" event in renames
            new VideoCompareForm(this.Disc).ShowDialog();
        }

        private void SelectTrack(LoadedTrack? track, MkvTrackType type, Panel propertiesPanel, TextBox nameTextBox, CheckBox? commentaryCheckBox, CheckBox defaultCheckBox) {
            TrackRename? rename = track?.RenameData;

            nameTextBox.Text = (rename?.Name ?? "");
            if (commentaryCheckBox != null) commentaryCheckBox.Checked = (rename?.CommentaryFlag ?? false);
            defaultCheckBox.Checked = (rename?.DefaultFlag ?? false);
            propertiesPanel.Enabled = (track != null);

            long selectedIndex = (track?.Identity?.Number ?? -1);
            var selectedClip = this.SelectedClip;
            if (this.Disc.ForceVlcTrackIndex && track != null && selectedClip != null)
            {
                try
                {
                    selectedIndex = selectedClip.Tracks.Where(x => x.Identity.Type == track.Identity.Type).WithIndex().First(x => x.Value == track).Index;
                }
                catch (Exception) {
                    selectedIndex = -1;
                }
            }

            switch (type)
            {
                case MkvTrackType.Video:
                    this.VideoPreview.VideoTrack = selectedIndex;
                    break;
                case MkvTrackType.Audio:
                    this.VideoPreview.AudioTrack = selectedIndex;
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

                var renames = track.RenameData;
                renames.Name = name;
                renames.CommentaryFlag = commentaryFlag;
                renames.DefaultFlag = defaultFlag;

                list.Update(track);
                list.Invalidate();
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

        private void AudioTrackList_SelectedIndexChanged(object sender, EventArgs e) {

        }

        private void OtherTrackList_OnSelectionChanged(TrackListData track) {
            this.SelectedOtherTrack = track?.Track;
            SelectTrack(
                track?.Track,
                MkvTrackType.Unknown,
                OtherTrackPanel,
                OtherTrackName,
                null,
                OtherTrackDefault
            );
        }

        private void OtherTrackApply_Click(object sender, EventArgs e) {
            TrackApplyChanges(
                this.OtherTrackList,
                this.SelectedOtherTrack,
                this.OtherTrackName.Text,
                false,
                this.OtherTrackDefault.Checked
            );
        }
    }
}
