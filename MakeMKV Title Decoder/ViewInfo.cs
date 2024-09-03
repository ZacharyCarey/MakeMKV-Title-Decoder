using libbluray.bdnav;
using libbluray.bdnav.Clpi;
using libbluray.disc;
using libbluray.file;
using MakeMKV_Title_Decoder.MkvToolNix.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MakeMKV_Title_Decoder {
    public partial class ViewInfo : Form {

        MkvToolNixDisc disc;

        public ViewInfo(MkvToolNixDisc disc) {
            this.disc = disc;
            InitializeComponent();

            this.TitleList.Items.AddRange(disc.Streams);
            this.TitleList.Items.AddRange(disc.Playlists);
            this.DiscNameLabel.Text = disc.Title ?? "";
            this.DiscSetLabel.Text = $"{disc.SetNumber?.ToString() ?? "?"}/{disc.NumberOfSets?.ToString() ?? "?"}";
        }

        private void ViewInfo_Load(object sender, EventArgs e) {
            /*PlayList? playlist = mpls.get(disc, "00001.mpls");
            this.SourceFileLabel.Text = "00001";
            if (playlist != null)
            {
                StreamEntry stream = playlist.play_item[0].stn.PrimaryVideoStreams[0];
                DisplaySizeLabel.Text = GetResolutionText(stream.video_format);
                FPSLabel.Text = $"{GetFPSText(stream.video_rate)} FPS";
            }


            var dir = disc.open_dir(Path.Combine("BDMV", "CLIPINF"));
            string filename;
            while(dir.read(out filename))
            {
                TitleList.Items.Add(Path.GetFileNameWithoutExtension(filename));
            }

            var clip = CLPI_CL.get(disc, "00001.clpi");
            Console.WriteLine(clip != null);*/

            /*            var file = file_win32.OpenFile(Path.Combine(this.disc.DiscPath, "BDMV", "CLIPINF", "00339.clpi"), FileMode.Open);
                        ClpiFile? clip = ClpiFile.Parse(file);
                        if (clip == null)
                        {
                            //Console.WriteLine(clip.type_indicator);
                            return;
                        }
                        var attribs = clip.program.progs[0].streams[0].attributes;
                        TitleList.Items.Add("00339");
            */            //            DisplaySizeLabel.Text = GetResolutionText(attribs.video_format);
                          //            FPSLabel.Text = $"{GetFPSText(attribs.video_rate)} FPS";
            if (this.TitleList.Items.Count >= 1)
            {
                this.TitleList.SelectedIndex = 0;
            }

            this.VideoPlayer.LoadVLC();
        }

        private static string GetResolutionText(VideoFormat format) {
            switch (format)
            {
                case VideoFormat._480I:
                case VideoFormat._480P:
                    return "720x480";
                case VideoFormat._576I:
                case VideoFormat._576P:
                    return "720x576";
                case VideoFormat._720P:
                    return "1280x720";
                case VideoFormat._1080I:
                case VideoFormat._1080P:
                    return "1920x1080";
                case VideoFormat._2160P:
                    return "3840x2160";
                default:
                    return "Unknown";
            }
        }

        private static string GetFPSText(VideoRate rate) {
            switch (rate)
            {
                case VideoRate._24000_1001: return "23.976";
                case VideoRate._24: return "24";
                case VideoRate._25: return "25";
                case VideoRate._30000_1001: return "29.97";
                case VideoRate._50: return "50";
                case VideoRate._60000_1001: return "59.94";
                default:
                    return "Unknown";
            }
        }

        private string? GetTypeIcon(MkvTrackType? type) {
            if (type == null)
            {
                return null;
            }
            switch (type)
            {
                case MkvTrackType.Audio: return "audio-headphones.png";
                case MkvTrackType.Subtitles: return "text.png";
                case MkvTrackType.Video: return "tool-animator.png";
                case MkvTrackType.Unknown:
                default:
                    return null;
            }
        }

        private string? GetBoolIcon(bool? val) {
            if (val == null)
            {
                return null;
            }

            return val.Value ? "dialog-ok-apply.png" : "dialog-cancel.png";
        }

        private string GetPropertiesString(MkvTrack track) {
            if (track.Properties == null)
            {
                return "";
            }

            List<string> props = new();
            switch (track.Type)
            {
                case MkvTrackType.Audio:
                    if (track.Properties.AudioSamplingFrequency != null)
                        props.Add(track.Properties.AudioSamplingFrequency.ToString() + " Hz");
                    if (track.Properties.AudioChannels != null)
                        props.Add(track.Properties.AudioChannels.ToString() + " channels");
                    break;
                case MkvTrackType.Video:
                    if (track.Properties.PixelDimensions != null)
                        props.Add(track.Properties.PixelDimensions + " pixels");
                    break;
                case MkvTrackType.Subtitles:

                    break;
                default:
                    return "";
            }

            return string.Join(", ", props);
        }

        private void TitleList_SelectedIndexChanged(object sender, EventArgs e) {
            MkvMergeID? stream = (MkvMergeID?)TitleList.SelectedItem;
            this.ContainerLabel.Text = stream?.Container?.Type ?? "";
            this.FileSizeLabel.Text = (stream?.FileSize ?? new DataSize()).ToString();
            this.DirectoryLabel.Text = stream?.FileDirectory ?? "N/A";

            this.VideoPlayer.LoadVideo(null);
            TrackList.Items.Clear();
            if (stream != null)
            {
                // Track info
                foreach (var track in stream.Tracks)
                {
                    ListViewItem CodecItem = new ListViewItem(track.Codec ?? "");
                    CodecItem.UseItemStyleForSubItems = false;
                    CodecItem.Tag = track;
                    TrackList.Items.Add(CodecItem);

                    MkvTrackType? type = track.Type;
                    if (type == MkvTrackType.Unknown)
                    {
                        type = null;
                    }
                    ListViewItem.ListViewSubItem TypeItem = new(CodecItem, type?.ToString() ?? "");
                    TypeItem.Tag = GetTypeIcon(type);
                    CodecItem.SubItems.Add(TypeItem);

                    ListViewItem.ListViewSubItem LanguageItem = new(CodecItem, track.Properties?.Language ?? "");
                    CodecItem.SubItems.Add(LanguageItem);

                    ListViewItem.ListViewSubItem NameItem = new(CodecItem, track.Properties?.TrackName ?? "");
                    CodecItem.SubItems.Add(NameItem);

                    ListViewItem.ListViewSubItem IdItem = new(CodecItem, track.ID.ToString() ?? "");
                    CodecItem.SubItems.Add(IdItem);

                    bool? isDefault = track.Properties?.DefaultTrack;
                    ListViewItem.ListViewSubItem DefaultItem = new(CodecItem, isDefault?.ToString() ?? "");
                    DefaultItem.Tag = GetBoolIcon(isDefault);
                    CodecItem.SubItems.Add(DefaultItem);

                    bool? isForced = track.Properties?.ForcedTrack;
                    ListViewItem.ListViewSubItem ForcedItem = new(CodecItem, isForced?.ToString() ?? "");
                    ForcedItem.Tag = GetBoolIcon(isForced);
                    CodecItem.SubItems.Add(ForcedItem);

                    ListViewItem.ListViewSubItem CharSetItem = new(CodecItem, track.Properties?.Encoding);
                    CodecItem.SubItems.Add(CharSetItem);

                    ListViewItem.ListViewSubItem PropsItem = new(CodecItem, GetPropertiesString(track));
                    CodecItem.SubItems.Add(PropsItem);

                    ListViewItem.ListViewSubItem SourceFileItem = new(CodecItem, stream.FileName ?? "");
                    CodecItem.SubItems.Add(SourceFileItem);

                    ListViewItem.ListViewSubItem SourceDirItem = new(CodecItem, stream.FileDirectory ?? "");
                    CodecItem.SubItems.Add(SourceDirItem);

                    ListViewItem.ListViewSubItem ProgramItem = new(CodecItem, track.Properties?.ProgramNumber?.ToString() ?? "");
                    CodecItem.SubItems.Add(ProgramItem);

                    ListViewItem.ListViewSubItem DelayItem = new(CodecItem, track.Properties?.CodecDelay?.ToString() ?? "");
                    CodecItem.SubItems.Add(DelayItem);
                }

                if (stream.FileDirectory != null && stream.FileName != null && stream.FileName.EndsWith(".m2ts"))
                {
                    this.VideoPlayer.LoadVideo(Path.Combine(stream.FileDirectory, stream.FileName));
                }
            }
        }

        private void TrackList_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e) {
            e.DrawDefault = true;
        }

        private void TrackList_DrawItem(object sender, DrawListViewItemEventArgs e) {
            e.DrawDefault = true;
        }

        private void TrackList_DrawSubItem(object sender, DrawListViewSubItemEventArgs e) {
            e.DrawBackground();
            Size sz = TrackList.SmallImageList.ImageSize;

            Image? img = null;
            if (e.SubItem.Tag != null)
            {
                img = TrackList.SmallImageList.Images[(string)e.SubItem.Tag];
            }

            Rectangle rTgt = new(e.Bounds.Location, sz);
            bool selected = e.Item.Selected; //e.ItemState.HasFlag(ListViewItemStates.Selected);
            // optionally show selection
            if (selected)
            {
                e.Graphics.FillRectangle(SystemBrushes.Highlight, e.Bounds);
            }
            if (img != null)
            {
                e.Graphics.DrawImage(img, rTgt);
            }

            // Optionally draw text
            e.Graphics.DrawString(e.SubItem.Text, e.SubItem.Font, selected ? SystemBrushes.HighlightText : SystemBrushes.WindowText, e.Bounds.X + sz.Width + 2, e.Bounds.Y + 2);
        }

        private void AppendTrackText(string str, FontStyle? style = null) {
            int start = this.TrackProperties.Text.Length;
            this.TrackProperties.AppendText(str);
            if (style != null)
            {
                this.TrackProperties.Select(start, str.Length);
                this.TrackProperties.SelectionFont = new Font(this.TrackProperties.Font, style.Value);
            }
        }

        private void AppendTrackTextLine(string str = "", FontStyle? style = null) {
            AppendTrackText(str + Environment.NewLine, style);
        }

        private void TrackList_SelectedIndexChanged(object sender, EventArgs e) {
            this.TrackProperties.Clear();

            if (TrackList.SelectedItems.Count == 1)
            {
                ListViewItem selectedItem = TrackList.SelectedItems[0];
                if (selectedItem.Tag != null)
                {
                    MkvTrack track = (MkvTrack)selectedItem.Tag;

                    AppendTrackTextLine("General", FontStyle.Bold);
                    AppendTrackTextLine($"Track name:\t\t{track.Properties?.TrackName ?? ""}");
                    AppendTrackTextLine($"Language:\t\t{track.Properties?.Language ?? ""}");
                    AppendTrackTextLine($"\"Default track\" flag:\t{track.Properties?.DefaultTrack?.ToString() ?? ""}");
                    AppendTrackTextLine($"\"Track enabled\" flag:\t{track.Properties?.EnabledTrack?.ToString() ?? ""}");
                    AppendTrackTextLine($"\"Forced display\" flag:\t{track.Properties?.ForcedTrack?.ToString() ?? ""}");
                    AppendTrackTextLine($"\"Hearing impaired\" flag:\t{track.Properties?.FlagHearingImpaired?.ToString() ?? ""}");
                    AppendTrackTextLine($"\"Visual impaired\" flag:\t{track.Properties?.FlagVisualImpaired?.ToString() ?? ""}");
                    AppendTrackTextLine($"\"Text descriptions\" flag:\t{track.Properties?.FlagTextDescriptions?.ToString() ?? ""}");
                    AppendTrackTextLine($"\"Original language\" flag:\t{track.Properties?.FlagOriginal?.ToString() ?? ""}");
                    AppendTrackTextLine($"\"Commentary\" flag:\t\t{track.Properties?.FlagCommentary?.ToString() ?? ""}");

                    AppendTrackTextLine();

                    AppendTrackTextLine("Timestamps", FontStyle.Bold);
                    AppendTrackTextLine($"Delay:\t\t{track.Properties?.CodecDelay?.ToString() ?? ""}");
                    AppendTrackTextLine($"Default duration:\t{track.Properties?.DefaultDuration?.ToString() ?? ""}");

                    AppendTrackTextLine();

                    AppendTrackTextLine("Video", FontStyle.Bold);
                    AppendTrackTextLine($"Display size: {track.Properties?.DisplayDimensions ?? ""} {track.Properties?.DisplayUnit?.ToString() ?? ""}");
                }
            }
        }

        private void ViewInfo_FormClosing(object sender, FormClosingEventArgs e) {
            this.VideoPlayer.LoadVideo(null);
        }
    }
}
