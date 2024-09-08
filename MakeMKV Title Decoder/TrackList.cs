using MakeMKV_Title_Decoder.Data;
using MakeMKV_Title_Decoder.MkvToolNix.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder {
    public class TrackListData {
        public MkvTrack? Track = null;
        public object? Tag = null;
    }

    public class TrackList : PropertiesList {

        public delegate void OnSelectionChangedDelegate(TrackListData? track);
        public event OnSelectionChangedDelegate OnSelectionChanged;

        public string KeepIconKey { get; set; } = "dialog-ok-apply.png";
        public string DeleteIconKey { get; set; } = "dialog-cancel.png";

        private Dictionary<MkvTrack, PropertyItem> QuickLookup = new();

        const int FirstColumnWidth = 160;
        private int padding = 0;
        public int ColumnPadding {
            get => padding;
            set
            {
                padding = Math.Max(0, value);
                if (this.Columns.Count > 0)
                {
                    this.Columns[0].Width = FirstColumnWidth + padding;
                }
            }
        }

        public TrackList() {
            ColumnHeader col1 = new();
            col1.Text = "Codec";
            col1.Width = FirstColumnWidth;
            col1.DisplayIndex = 0;
            this.Columns.Add(col1);

            ColumnHeader col9 = new();
            col9.Text = "Type";
            col9.Width = 90;
            col9.DisplayIndex = 1;
            this.Columns.Add(col9);

            ColumnHeader col2 = new();
            col2.Text = "Name";
            col2.Width = 160;
            col2.DisplayIndex = 2;
            this.Columns.Add(col2);

            ColumnHeader col7 = new();
            col7.Text = "Properties";
            col7.Width = 90;
            col7.DisplayIndex = 3;
            this.Columns.Add(col7);

            ColumnHeader col3 = new();
            col3.Text = "Lang";
            col3.Width = 40;
            col3.DisplayIndex = 4;
            this.Columns.Add(col3);

            ColumnHeader col4 = new();
            col4.Text = "Enabled";
            col4.Width = 60;
            col4.DisplayIndex = 5;
            this.Columns.Add(col4);

            ColumnHeader col8 = new();
            col8.Text = "Default";
            col8.Width = 60;
            col8.DisplayIndex = 6;
            this.Columns.Add(col8);

            ColumnHeader col5 = new();
            col5.Text = "Forced";
            col5.Width = 50;
            col5.DisplayIndex = 7;
            this.Columns.Add(col5);

            ColumnHeader col6 = new();
            col6.Text = "Commentary";
            col6.Width = 90;
            col6.DisplayIndex = 8;
            this.Columns.Add(col6);

            this.SelectedIndexChanged += Event_SelectedIndexChanged;
        }

        public TrackListData Add(MkvMergeID clip, MkvTrack track, RenameData2 renames, Color? color = null, int? padding = null, string? Icon = null, Color? backColor = null) {
            TrackListData data = new();
            PropertyItem source = new();
            source.IconColor = color;
            source.Padding = padding ?? 0;
            source.IconKey = Icon;
            source.Tag = data;
            if(backColor != null) source.BackColor = backColor.Value;
            this.Items.Add(source);

            this.QuickLookup[track] = source;

            for(int i = 0; i < 8; i++)
            {
                PropertySubItem item = new(source);
                if (backColor != null) item.BackColor = backColor.Value;
                source.SubItems.Add(item);
            }

            Update(clip, track, renames);
            return data;
        }

        public void Update(MkvMergeID clip, MkvTrack track, RenameData2 renames) {
            PropertyItem item = QuickLookup[track];
            var data = renames.GetClipRename(clip)?.GetTrackRename(track);

            List<string> trackProperties = new();
            switch(track.Type)
            {
                case MkvTrackType.Video:
                    var size = track.Properties?.PixelDimensions;
                    if (size != null) trackProperties.Add($"{size} pixels");
                    break;
                case MkvTrackType.Audio:
                    var freq = track.Properties?.AudioSamplingFrequency;
                    if (freq != null) trackProperties.Add($"{freq} Hz");

                    var channels = track.Properties?.AudioChannels;
                    if (channels != null) trackProperties.Add($"{channels} channels");
                    break;
            }

            item.Text = track.Codec;

            item.SubItems[1].Text = track.Type.ToString();
            ((PropertyData)item.SubItems[1].Tag).IconKey = GetTypeIcon(track.Type);

            item.SubItems[2].Text = data?.Name; // name
            item.SubItems[3].Text = string.Join(", ", trackProperties); // properties
            item.SubItems[4].Text = track.Properties?.Language ?? ""; // lang
            ((PropertyData)item.SubItems[5].Tag).IconKey = GetBoolIcon(track.Properties?.EnabledTrack ?? true); // enabled
            ((PropertyData)item.SubItems[6].Tag).IconKey = GetBoolIcon((data?.DefaultFlag) ?? (track.Properties?.DefaultTrack ?? true)); // default
            ((PropertyData)item.SubItems[7].Tag).IconKey = GetBoolIcon(track.Properties?.ForcedTrack ?? false); // forced
            ((PropertyData)item.SubItems[8].Tag).IconKey = GetBoolIcon((data?.CommentaryFlag) ?? (track.Properties?.FlagCommentary ?? false)); // commentary
        }

        public new void Clear() {
            this.QuickLookup.Clear();
            base.Items.Clear();
        }

        public new TrackListData? SelectedItem {
            get => (TrackListData?)base.SelectedItem?.Tag;
            set
            {
                for(int i = 0; i < base.Items.Count; i++)
                {
                    TrackListData? track = (TrackListData?)((PropertyData)base.Items[i].Tag).Tag;
                    if (track != null && track == value)
                    {
                        base.SelectedIndex = i;
                        return;
                    }
                }
                base.SelectedIndex = null;
            }
        }

        public void Select(MkvTrack? track) {
            if (track == null)
            {
                base.SelectedIndex = null;
                return;
            }

            for(int i = 0; i < base.Items.Count; i++)
            {
                TrackListData? data = (TrackListData?)((PropertyData)base.Items[i].Tag).Tag;
                if (data != null && data.Track == track)
                {
                    base.SelectedIndex = i;
                    return;
                }
            }
            base.SelectedIndex = null;
        }

        private string? GetBoolIcon(bool? val) {
            if (val == null)
            {
                return null;
            }

            return val.Value ? KeepIconKey : DeleteIconKey;
        }

        private string? GetTypeIcon(MkvTrackType type) {
            switch (type)
            {
                case MkvTrackType.Audio: return "audio-headphones.png";
                case MkvTrackType.Video: return "tool-animator.png";
                case MkvTrackType.Subtitles: return "text.png";
                case MkvTrackType.Unknown:
                default:
                    return "application-octet-stream.png";
            }
        }

        private void Event_SelectedIndexChanged(object? sender, EventArgs e) {
            OnSelectionChanged?.Invoke(this.SelectedItem);
        }
    }
}
