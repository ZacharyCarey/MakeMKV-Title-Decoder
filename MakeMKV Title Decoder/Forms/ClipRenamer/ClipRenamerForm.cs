using FfmpegInterface;
using MakeMKV_Title_Decoder.Controls;
using MakeMKV_Title_Decoder.Data;
using MakeMKV_Title_Decoder.Data.Renames;
using MakeMKV_Title_Decoder.Forms.ClipRenamer;
using MakeMKV_Title_Decoder.Util;
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
    public partial class ClipRenamerForm : Form
    {

        const string KeepIconKey = "dialog-ok-apply.png";
        const string DeleteIconKey = "dialog-cancel.png";

        public readonly LoadedDisc Disc;

        private LoadedStream? SelectedClip = null;
        private LoadedTrack? SelectedVideoTrack = null;
        private LoadedTrack? SelectedAudioTrack = null;
        private LoadedTrack? SelectedOtherTrack = null;

        public ClipRenamerForm(LoadedDisc disc)
        {
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

        private void ClipRenamer_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.VideoPreview.LoadVideo(null);
        }

        private void SelectClip(LoadedStream? clip)
        {
            this.SelectedClip = clip;
            this.NameTextBox.Text = (clip == null) ? "" : (clip.RenameData.Name ?? "");

            this.PropertiesPanel.Enabled = (clip != null);
            if (clip == null)
            {
                this.VideoPreview.LoadVideo(null);
            }
            else
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
                    }
                    else if (track.Identity.Type == MkvTrackType.Audio)
                    {
                        this.AudioTrackList.Add(track);
                    }
                    else
                    {
                        this.OtherTrackList.Add(track);
                    }
                }
            }
        }

        private void RefreshClipListItem(ListViewItem row, LoadedStream data)
        {
            string name = (data.RenameData.Name ?? "");

            row.ImageKey = (string.IsNullOrWhiteSpace(name) ? DeleteIconKey : KeepIconKey);
            row.SubItems[2].Text = name;
        }




        private void ClipRenamer_Load(object sender, EventArgs e)
        {

        }

        private void ClipsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ClipsList.SelectedItems.Count <= 0)
            {
                SelectClip(null);
                return;
            }

            LoadedStream? selection = (LoadedStream?)ClipsList.SelectedItems[0].Tag;
            SelectClip(selection);
        }

        private void ApplyBtn_Click(object sender, EventArgs e)
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

        private void compareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // TODO make it smart enough to handle changes to Renames while open
            // This can likely be done with a "onChange" event in renames
            new VideoCompareForm(this.Disc).ShowDialog();
        }

        private void SelectTrack(LoadedTrack? track, MkvTrackType type, Panel propertiesPanel, TextBox nameTextBox, CheckBox? commentaryCheckBox, CheckBox defaultCheckBox, TextBox langTextBox)
        {
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
                catch (Exception)
                {
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

            langTextBox.Text = track?.RenameData.Language ?? "";
        }

        private void VideoTrackList_OnSelectionChanged(TrackListData? track)
        {
            this.SelectedVideoTrack = track?.Track;
            SelectTrack(
                track?.Track,
                MkvTrackType.Video,
                VideoTrackPanel,
                VideoTrackName,
                VideoTrackCommentary,
                VideoTrackDefault,
                VideoLangTextBox
            );
        }

        private void AudioTrackList_OnSelectionChanged(TrackListData? track)
        {
            this.SelectedAudioTrack = track?.Track;
            SelectTrack(
                track?.Track,
                MkvTrackType.Audio,
                AudioTrackPanel,
                AudioTrackName,
                AudioTrackCommentary,
                AudioTrackDefault,
                AudioLangTextBox
            );
        }

        private void TrackApplyChanges(TrackList list, LoadedTrack? track, string? name, bool? commentaryFlag, bool? defaultFlag, string? lang)
        {
            if (this.SelectedClip != null && track != null)
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    name = null;
                }

                if (string.IsNullOrWhiteSpace(lang))
                {
                    lang = null;
                }
                else
                {
                    if (!Languages.IsValidLanguageCode(lang))
                    {
                        MessageBox.Show("Please enter a valid language code.", "Invalid language");
                        return;
                    }
                }

                var renames = track.RenameData;
                renames.Name = name;
                renames.CommentaryFlag = commentaryFlag;
                renames.DefaultFlag = defaultFlag;
                renames.Language = lang;

                list.Update(track);
                list.Invalidate();
            }
        }

        private void VideoTrackApply_Click(object sender, EventArgs e)
        {
            TrackApplyChanges(
                this.VideoTrackList,
                this.SelectedVideoTrack,
                this.VideoTrackName.Text,
                this.VideoTrackCommentary.Checked,
                this.VideoTrackDefault.Checked,
                this.VideoLangTextBox.Text
            );
        }

        private void AudioTrackApply_Click(object sender, EventArgs e)
        {
            TrackApplyChanges(
                this.AudioTrackList,
                this.SelectedAudioTrack,
                this.AudioTrackName.Text,
                this.AudioTrackCommentary.Checked,
                this.AudioTrackDefault.Checked,
                this.AudioLangTextBox.Text
            );
        }

        private void VideoTrackList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void AudioTrackList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void OtherTrackList_OnSelectionChanged(TrackListData track)
        {
            this.SelectedOtherTrack = track?.Track;
            SelectTrack(
                track?.Track,
                MkvTrackType.Unknown,
                OtherTrackPanel,
                OtherTrackName,
                null,
                OtherTrackDefault,
                OtherLangTextBox
            );
        }

        private void OtherTrackApply_Click(object sender, EventArgs e)
        {
            TrackApplyChanges(
                this.OtherTrackList,
                this.SelectedOtherTrack,
                this.OtherTrackName.Text,
                false,
                this.OtherTrackDefault.Checked,
                this.OtherLangTextBox.Text
            );
        }

        private void LangTextBox_TextChanged(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.BackColor = SystemColors.Window;
            }
            else
            {
                if (Languages.IsValidLanguageCode(textBox.Text))
                {
                    textBox.BackColor = SystemColors.Window;
                }
                else
                {
                    textBox.BackColor = Color.Red;
                }
            }
        }

        private void SelectLangBtn(TextBox langTextBox)
        {
            var langSelector = new LanguageSelectorForm();
            var result = langSelector.ShowDialog();
            if (result == DialogResult.OK)
            {
                langTextBox.Text = langSelector.SelectedLanguageCode ?? "";
            }
        }

        private void SelectVideoLangBtn_Click(object sender, EventArgs e)
        {
            SelectLangBtn(this.VideoLangTextBox);
        }

        private void SelectAudioLang_Click(object sender, EventArgs e)
        {
            SelectLangBtn(this.AudioLangTextBox);
        }

        private void SelectOtherLang_Click(object sender, EventArgs e)
        {
            SelectLangBtn(this.OtherLangTextBox);
        }

        private void ExportAllFramesBtn_Click(object sender, EventArgs e)
        {
            LoadedStream? selection = this.SelectedClip;
            if (selection != null)
            {
                DialogResult result;
                if (selection.Identity.Duration.TotalMinutes < 1)
                {
                    result = MessageBox.Show("Clip is <1 minute long. Export all frames?", "Export Frames", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                }
                else
                {
                    int min = (int)selection.Identity.Duration.TotalMinutes;
                    string plural = "";
                    if (min > 1)
                    {
                        plural = "s";
                    }

                    result = MessageBox.Show($"Clip is {min} minute{plural} long. Export all frames?", "Export Frames", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                }

                if (result == DialogResult.Yes)
                {
                    bool any = false;
                    foreach(Attachment attachment in AllFramesExtracted.Extract(this.Disc, selection))
                    {
                        any = true;
                        this.Disc.RenameData.Attachments.Add(attachment);
                    }

                    if (any)
                    {
                        MessageBox.Show("Successfully extracted images.");
                    } else
                    {
                        MessageBox.Show("Attachments already exists or failed to extract images.");
                    }
                }
            }
        }

        private void ExportSingleFrameBtn_Click(object sender, EventArgs e)
        {
            LoadedStream? selection = this.SelectedClip;
            if (selection != null)
            {
                Attachment? attachment = SingleFrameExtracted.Extract(this.Disc, selection);
                if (attachment == null)
                {
                    MessageBox.Show("Attachment already exists or failed to extract image.");
                    return;
                } else
                {
                    MessageBox.Show("Successfully extracted image.");
                    this.Disc.RenameData.Attachments.Add(attachment);
                }
            }
        }
    }
}
