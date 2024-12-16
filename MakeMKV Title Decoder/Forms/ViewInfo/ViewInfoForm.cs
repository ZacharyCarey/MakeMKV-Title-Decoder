using libbluray.bdnav;
using libbluray.bdnav.Clpi;
using libbluray.disc;
using libbluray.file;
using MakeMKV_Title_Decoder.Data;
using MakeMKV_Title_Decoder.Data.Renames;
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
using Utils;

namespace MakeMKV_Title_Decoder
{
    public partial class ViewInfoForm : Form {

        LoadedDisc disc;

        public ViewInfoForm(LoadedDisc disc) {
            this.disc = disc;
            InitializeComponent();

            this.TitleList.Items.AddRange(disc.Streams.ToArray()); // LoadedStream
            //this.TitleList.Items.AddRange(disc.GetPlaylists().Cast<object>().ToArray()); // DiscPlaylist
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

        private string? GetTypeIcon(TrackType? type) {
            if (type == null)
            {
                return null;
            }
            switch (type)
            {
                case TrackType.Audio: return "audio-headphones.png";
                case TrackType.Subtitle: return "text.png";
                case TrackType.Video: return "tool-animator.png";
                //case TrackType.Unknown:
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

        private string GetPropertiesString(LoadedTrack track) {
            List<string> props = new();
            switch (track.Identity.TrackType)
            {
                case TrackType.Audio:
                    if (track.Identity.SampleRateHz != null)
                        props.Add(track.Identity.SampleRateHz.ToString() + " Hz");
                    if (track.Identity.Channels != null)
                        props.Add(track.Identity.Channels.ToString() + " channels");
                    break;
                case TrackType.Video:
                    props.Add(new Size(track.Identity.Width ?? 0, track.Identity.Height ?? 0).ToString() + " pixels");
                    break;
                case TrackType.Subtitle:
                    break;
                default:
                    return "";
            }

            return string.Join(", ", props);
        }

        private void TitleList_SelectedIndexChanged(object sender, EventArgs e) {
            LoadedStream? stream = (LoadedStream?)TitleList.SelectedItem;
            this.ContainerLabel.Text = ""; //stream?.Container?.Type ?? "";
            this.FileSizeLabel.Text = (stream?.Identity?.FileSize ?? new DataSize()).ToString();
            this.DirectoryLabel.Text = stream?.Identity.SourceFile ?? "N/A";

            this.VideoPlayer.LoadVideo(null);
            TrackList.Items.Clear();
            if (stream != null)
            {
                // Track info
                foreach (var track in stream.Tracks)
                {
                    ListViewItem CodecItem = new ListViewItem(track.Identity.Codec ?? "");
                    CodecItem.UseItemStyleForSubItems = false;
                    CodecItem.Tag = track;
                    TrackList.Items.Add(CodecItem);

                    TrackType type = track.Identity.TrackType;
                    ListViewItem.ListViewSubItem TypeItem = new(CodecItem, type.ToString() ?? "");
                    TypeItem.Tag = GetTypeIcon(type);
                    CodecItem.SubItems.Add(TypeItem);

                    ListViewItem.ListViewSubItem LanguageItem = new(CodecItem, track.Identity.Language?.Part2 ?? "");
                    CodecItem.SubItems.Add(LanguageItem);

                    ListViewItem.ListViewSubItem NameItem = new(CodecItem, track.Identity.Title ?? "");
                    CodecItem.SubItems.Add(NameItem);

                    ListViewItem.ListViewSubItem IdItem = new(CodecItem, track.Identity.Index.ToString() ?? "");
                    CodecItem.SubItems.Add(IdItem);

                    bool? isDefault = track.RenameData.DefaultFlag;
                    ListViewItem.ListViewSubItem DefaultItem = new(CodecItem, isDefault?.ToString() ?? "");
                    DefaultItem.Tag = GetBoolIcon(isDefault);
                    CodecItem.SubItems.Add(DefaultItem);

                    bool? isForced = null;
                    ListViewItem.ListViewSubItem ForcedItem = new(CodecItem, isForced?.ToString() ?? "");
                    ForcedItem.Tag = GetBoolIcon(isForced);
                    CodecItem.SubItems.Add(ForcedItem);

                    ListViewItem.ListViewSubItem CharSetItem = new(CodecItem, "N/A"); // Encoding
                    CodecItem.SubItems.Add(CharSetItem);

                    ListViewItem.ListViewSubItem PropsItem = new(CodecItem, GetPropertiesString(track));
                    CodecItem.SubItems.Add(PropsItem);

                    ListViewItem.ListViewSubItem SourceFileItem = new(CodecItem, Path.GetFileName(stream.Identity.SourceFile));
                    CodecItem.SubItems.Add(SourceFileItem);

                    ListViewItem.ListViewSubItem SourceDirItem = new(CodecItem, Path.GetDirectoryName(stream.Identity.SourceFile));
                    CodecItem.SubItems.Add(SourceDirItem);

                    ListViewItem.ListViewSubItem ProgramItem = new(CodecItem, /*track.Identity.Number?.ToString() ??*/ "");
                    CodecItem.SubItems.Add(ProgramItem);

                    ListViewItem.ListViewSubItem DelayItem = new(CodecItem, "N/A"); // Codec delay
                    CodecItem.SubItems.Add(DelayItem);
                }

                if (stream.Identity.SourceFile != null)
                {
                    this.VideoPlayer.LoadVideo(stream.GetFullPath(disc));
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
                if (selectedItem.Tag != null && selectedItem.Tag is LoadedTrack track)
                {
                    AppendTrackTextLine("General", FontStyle.Bold);
                    AppendTrackTextLine($"Track name:\t\t{track.Identity.Title ?? ""}");
                    AppendTrackTextLine($"Language:\t\t{track.Identity.Language?.Part2 ?? ""}");
                    AppendTrackTextLine($"\"Default track\" flag:\t{track.Identity.DefaultFlag?.ToString() ?? ""}");
                    //AppendTrackTextLine($"\"Track enabled\" flag:\t{track.Identity.EnabledTrack?.ToString() ?? ""}");
                    AppendTrackTextLine($"\"Forced display\" flag:\t{track.Identity.ForcedFlag?.ToString() ?? ""}");
                    AppendTrackTextLine($"\"Hearing impaired\" flag:\t{track.Identity.HearingImpairedFlag?.ToString() ?? ""}");
                    AppendTrackTextLine($"\"Visual impaired\" flag:\t{track.Identity.VisualImpairedFlag?.ToString() ?? ""}");
                    AppendTrackTextLine($"\"Text descriptions\" flag:\t{track.Identity.DescriptionsFlag?.ToString() ?? ""}");
                    AppendTrackTextLine($"\"Original language\" flag:\t{track.Identity.OriginalFlag?.ToString() ?? ""}");
                    AppendTrackTextLine($"\"Commentary\" flag:\t\t{track.Identity.CommentFlag?.ToString() ?? ""}");

                    AppendTrackTextLine();

                    AppendTrackTextLine("Timestamps", FontStyle.Bold);
                    //AppendTrackTextLine($"Delay:\t\t{track.Identity.CodecDelay?.ToString() ?? ""}");
                    AppendTrackTextLine($"Default duration:\t{track.Identity.Duration.ToString() ?? ""}");

                    AppendTrackTextLine();

                    AppendTrackTextLine("Video", FontStyle.Bold);
                    AppendTrackTextLine($"Display size: {new Size(track.Identity.Width ?? 0, track.Identity.Height ?? 0)} {track.Identity.PixelFormat?.ToString() ?? ""}");
                }
            }
        }

        private void ViewInfo_FormClosing(object sender, FormClosingEventArgs e) {
            this.VideoPlayer.LoadVideo(null);
        }
    }
}
