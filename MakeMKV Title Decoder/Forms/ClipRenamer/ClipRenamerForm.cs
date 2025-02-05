using FFMpeg_Wrapper.ffmpeg;
using FFMpeg_Wrapper.ffprobe;
using Iso639;
using MakeMKV_Title_Decoder.Controls;
using MakeMKV_Title_Decoder.Data;
using MakeMKV_Title_Decoder.Data.Renames;
using MakeMKV_Title_Decoder.Forms;
using MakeMKV_Title_Decoder.Forms.ClipRenamer;
using MakeMKV_Title_Decoder.Util;
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

        private int NumSelectedClips => this.ClipsList.SelectedItems.Count;
        private IEnumerable<(ListViewItem Item, LoadedStream Stream)> AllSelectedClips {
            get
            {
                for (int i = 0; i < this.ClipsList.SelectedItems.Count; i++)
                {
                    LoadedStream? stream = (LoadedStream?)ClipsList.SelectedItems[i].Tag;
                    if (stream != null) yield return (ClipsList.SelectedItems[i], stream);
                }
            }
        }

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

            //this.DeinterlaceCheckBox.Enabled = disc.SupportsDeinterlacing;
        }

        private void ClipRenamer_FormClosing(object sender, FormClosingEventArgs e) {
            this.VideoPreview.LoadVideo(null);
        }

        private void SelectClip(LoadedStream? clip) {
            this.SelectedClip = clip;
            this.NameTextBox.Text = (clip == null) ? "" : (clip.RenameData.Name ?? "");
            this.DeinterlaceCheckBox.Checked = clip?.RenameData?.Deinterlaced ?? false;

            this.PropertiesPanel.Enabled = (clip != null);
            if (clip == null)
            {
                this.VideoPreview.LoadVideo(null);
            } else
            {
                string path = clip.GetFullPath(this.Disc);
                this.VideoPreview.LoadVideo(path);
            }

            this.VideoTrackList.Clear();
            this.AudioTrackList.Clear();
            this.OtherTrackList.Clear();
            VideoTrackList_OnSelectionChanged(null);
            AudioTrackList_OnSelectionChanged(null);
            if (clip != null)
            {
                foreach (LoadedTrack track in clip.Tracks)
                {
                    if (track.Identity.TrackType == TrackType.Video)
                    {
                        this.VideoTrackList.Add(track);
                    } else if (track.Identity.TrackType == TrackType.Audio)
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
            this.ClipsList.Invalidate();
        }




        private void ClipRenamer_Load(object sender, EventArgs e) {

        }

        private void ClipsList_SelectedIndexChanged(object sender, EventArgs e) {
            if (ClipsList.SelectedItems.Count <= 0)
            {
                SelectClip(null);
                return;
            }

            LoadedStream? selection = (LoadedStream?)ClipsList.SelectedItems[0].Tag;
            SelectClip(selection);
        }

        private void ApplyBtn_Click(object sender, EventArgs e) {
            if (this.NumSelectedClips > 1)
            {
                string? name = this.NameTextBox.Text;
                if (string.IsNullOrWhiteSpace(this.NameTextBox.Text))
                {
                    name = null;
                }

                foreach ((int index, (var item, var clip)) in this.AllSelectedClips.WithIndex())
                {
                    string? n = name;
                    if (n != null)
                    {
                        n = n + $" {(index + 1)}";
                    }
                    clip.RenameData.Name = n;
                    RefreshClipListItem(item, clip);
                }
            } else
            {
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
        }

        private void compareToolStripMenuItem_Click(object sender, EventArgs e) {
            // TODO make it smart enough to handle changes to Renames while open
            // This can likely be done with a "onChange" event in renames
            new VideoCompareForm(this.Disc).ShowDialog();
        }

        private void SelectTrack(LoadedTrack? track, TrackType type, Panel propertiesPanel, TextBox nameTextBox, CheckBox? commentaryCheckBox, CheckBox defaultCheckBox, TextBox langTextBox) {
            TrackRename? rename = track?.RenameData;

            nameTextBox.Text = (rename?.Name ?? "");
            if (commentaryCheckBox != null) commentaryCheckBox.Checked = (rename?.CommentaryFlag ?? false);
            defaultCheckBox.Checked = (rename?.DefaultFlag ?? false);
            propertiesPanel.Enabled = (track != null);

            long selectedIndex = -1;
            var selectedClip = this.SelectedClip;
            if (track != null && selectedClip != null)
            {
                selectedIndex = selectedClip.GetVlcID(track);
            }

            switch (type)
            {
                case TrackType.Video:
                    this.VideoPreview.VideoTrack = selectedIndex;
                    break;
                case TrackType.Audio:
                    this.VideoPreview.AudioTrack = selectedIndex;
                    break;
            }

            langTextBox.Text = track?.RenameData?.Language?.Part2 ?? "";
        }

        private void VideoTrackList_OnSelectionChanged(TrackListData? track) {
            this.SelectedVideoTrack = track?.Track;
            SelectTrack(
                track?.Track,
                TrackType.Video,
                VideoTrackPanel,
                VideoTrackName,
                VideoTrackCommentary,
                VideoTrackDefault,
                VideoLangTextBox
            );
        }

        private void AudioTrackList_OnSelectionChanged(TrackListData? track) {
            this.SelectedAudioTrack = track?.Track;
            SelectTrack(
                track?.Track,
                TrackType.Audio,
                AudioTrackPanel,
                AudioTrackName,
                AudioTrackCommentary,
                AudioTrackDefault,
                AudioLangTextBox
            );
        }

        private void TrackApplyChanges(TrackList list, LoadedTrack? track, string? name, bool? commentaryFlag, bool? defaultFlag, string? lang) {
            if (this.SelectedClip != null && track != null)
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    name = null;
                }

                Language? parsedLang = null;
                if (string.IsNullOrWhiteSpace(lang))
                {
                    parsedLang = null;
                } else
                {
                    parsedLang = Language.FromPart2(lang);
                    if (parsedLang == null)
                    {
                        MessageBox.Show("Please enter a valid language code.", "Invalid language");
                        return;
                    }
                }

                var renames = track.RenameData;
                renames.Name = name;
                renames.CommentaryFlag = commentaryFlag;
                renames.DefaultFlag = defaultFlag;
                renames.Language = parsedLang;

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
                this.VideoTrackDefault.Checked,
                this.VideoLangTextBox.Text
            );
        }

        private void AudioTrackApply_Click(object sender, EventArgs e) {
            TrackApplyChanges(
                this.AudioTrackList,
                this.SelectedAudioTrack,
                this.AudioTrackName.Text,
                this.AudioTrackCommentary.Checked,
                this.AudioTrackDefault.Checked,
                this.AudioLangTextBox.Text
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
                TrackType.Subtitle,
                OtherTrackPanel,
                OtherTrackName,
                null,
                OtherTrackDefault,
                OtherLangTextBox
            );
        }

        private void OtherTrackApply_Click(object sender, EventArgs e) {
            TrackApplyChanges(
                this.OtherTrackList,
                this.SelectedOtherTrack,
                this.OtherTrackName.Text,
                false,
                this.OtherTrackDefault.Checked,
                this.OtherLangTextBox.Text
            );
        }

        private void LangTextBox_TextChanged(object sender, EventArgs e) {
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.BackColor = SystemColors.Window;
            } else
            {
                if (Language.FromPart2(textBox.Text) != null)
                {
                    textBox.BackColor = SystemColors.Window;
                } else
                {
                    textBox.BackColor = Color.Red;
                }
            }
        }

        private void SelectLangBtn(TextBox langTextBox) {
            var langSelector = new LanguageSelectorForm();
            var result = langSelector.ShowDialog();
            if (result == DialogResult.OK)
            {
                langTextBox.Text = langSelector.SelectedLanguage?.Part2 ?? "";
            }
        }

        private void SelectVideoLangBtn_Click(object sender, EventArgs e) {
            SelectLangBtn(this.VideoLangTextBox);
        }

        private void SelectAudioLang_Click(object sender, EventArgs e) {
            SelectLangBtn(this.AudioLangTextBox);
        }

        private void SelectOtherLang_Click(object sender, EventArgs e) {
            SelectLangBtn(this.OtherLangTextBox);
        }

        private void viewFramesToolStripMenuItem_Click(object sender, EventArgs e) {
            var selected = this.SelectedClip;
            if (selected != null)
            {
                new FrameViewer(this.Disc, selected).Show();
            }
        }

        private void exportAllSelectedToolStripMenuItem_Click(object sender, EventArgs e) {
            string? ffprobePath = FileUtils.GetFFProbeExe();
            string? ffmpegPath = FileUtils.GetFFMpegExe();
            if (ffprobePath == null || ffmpegPath == null)
            {
                MessageBox.Show("Failed to find ffprobe/ffmpeg.");
                this.Close();
                return;
            }

            FFProbe ffprobe = new FFProbe(ffprobePath);
            //FFMpeg ffmpeg = new FFMpeg(ffmpegPath);

            bool success = true;
            foreach ((var item, var stream) in this.AllSelectedClips)
            {
                string filePath = stream.GetFullPath(this.Disc);
                DataSize? fileSize = DataSize.FromFile(filePath);
                if (fileSize == null)
                {
                    MessageBox.Show("Failed to find file.");
                    this.Close();
                    return;
                }
                if (fileSize >= new DataSize(10, Unit.Mega))
                {
                    var user = MessageBox.Show($"The file size is {fileSize}, this may take a while to process. Continue?", "Continue?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                    if (user != DialogResult.Yes)
                    {
                        this.Close();
                        return;
                    }
                }
            }

            foreach((var item, var stream) in this.AllSelectedClips)
            {
                uint nFrames = 0;
                uint? frames = ffprobe.GetNumberOfFrames(stream.GetFullPath(Disc), true);
                if (frames == null || frames < 0)
                {
                    nFrames = 0;
                } else
                {
                    nFrames = frames.Value;
                }
                Console.WriteLine($"Detected {nFrames} frames in {stream.Identity.SourceFile}");
                if (nFrames > 0)
                {
                    Attachment? attachment = SingleFrameExtracted.Extract(this.Disc, stream, 0);
                    if (attachment == null)
                    {
                        success = false;
                    } else
                    {
                        this.Disc.RenameData.Attachments.Add(attachment);
                    }
                }
            }

            if (!success)
            {
                MessageBox.Show("At least one attachment already exists or failed to extract image.");
            }
        }

        private void DeinterlaceCheckBox_CheckedChanged(object sender, EventArgs e) {
            bool state = this.DeinterlaceCheckBox.Checked;

            LoadedStream? selection = this.SelectedClip;
            if (selection != null)
            {
                selection.RenameData.Deinterlaced = state;
            }
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e) {

        }
    }
}
