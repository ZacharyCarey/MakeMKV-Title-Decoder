using MakeMKV_Title_Decoder.Data;
using MakeMKV_Title_Decoder.MkvToolNix.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder {
    public class TrackList : PropertiesList {

        public delegate void OnSelectionChangedDelegate(MkvTrack? track);
        public event OnSelectionChangedDelegate OnSelectionChanged;

        public string KeepIconKey { get; set; } = "dialog-ok-apply.png";
        public string DeleteIconKey { get; set; } = "dialog-cancel.png";

        private Dictionary<MkvTrack, PropertyItem> QuickLookup = new();

        public TrackList() {
            ColumnHeader col1 = new();
            col1.Text = "Codec";
            col1.Width = 160;
            col1.DisplayIndex = 0;
            this.Columns.Add(col1);

            ColumnHeader col2 = new();
            col2.Text = "Name";
            col2.Width = 160;
            col2.DisplayIndex = 1;
            this.Columns.Add(col2);

            ColumnHeader col7 = new();
            col7.Text = "Properties";
            col7.Width = 90;
            col7.DisplayIndex = 2;
            this.Columns.Add(col7);

            ColumnHeader col3 = new();
            col3.Text = "Lang";
            col3.Width = 40;
            col3.DisplayIndex = 3;
            this.Columns.Add(col3);

            ColumnHeader col4 = new();
            col4.Text = "Enabled";
            col4.Width = 60;
            col4.DisplayIndex = 4;
            this.Columns.Add(col4);

            ColumnHeader col8 = new();
            col8.Text = "Default";
            col8.Width = 60;
            col8.DisplayIndex = 5;
            this.Columns.Add(col8);

            ColumnHeader col5 = new();
            col5.Text = "Forced";
            col5.Width = 50;
            col5.DisplayIndex = 6;
            this.Columns.Add(col5);

            ColumnHeader col6 = new();
            col6.Text = "Commentary";
            col6.Width = 90;
            col6.DisplayIndex = 7;
            this.Columns.Add(col6);

            this.SelectedIndexChanged += Event_SelectedIndexChanged;
        }

        public void Add(MkvMergeID clip, MkvTrack track, RenameData2 renames) {
            PropertyItem source = new();
            source.Tag = track;
            this.Items.Add(source);

            this.QuickLookup[track] = source;

            for(int i = 0; i < 7; i++)
            {
                PropertySubItem item = new(source);
                source.SubItems.Add(item);
            }

            Update(clip, track, renames);
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
            item.SubItems[1].Text = data?.Name; // name
            item.SubItems[2].Text = string.Join(", ", trackProperties); // properties
            item.SubItems[3].Text = track.Properties?.Language ?? ""; // lang
            item.SubItems[4].Tag = GetBoolIcon(track.Properties?.EnabledTrack ?? true); // enabled
            item.SubItems[5].Tag = GetBoolIcon((data?.DefaultFlag) ?? (track.Properties?.DefaultTrack ?? true)); // default
            item.SubItems[6].Tag = GetBoolIcon(track.Properties?.ForcedTrack ?? false); // forced
            item.SubItems[7].Tag = GetBoolIcon((data?.CommentaryFlag) ?? (track.Properties?.FlagCommentary ?? false)); // commentary
        }

        public new void Clear() {
            this.QuickLookup.Clear();
            base.Items.Clear();
        }

        public MkvTrack? SelectedItem {
            get
            {
                if (base.SelectedItems.Count > 0)
                {
                    return (MkvTrack?)base.SelectedItems[0].Tag;
                }

                return null;
            }
        }

        private string? GetBoolIcon(bool? val) {
            if (val == null)
            {
                return null;
            }

            return val.Value ? KeepIconKey : DeleteIconKey;
        }

        private void Event_SelectedIndexChanged(object? sender, EventArgs e) {
            MkvTrack? selection = null;
            if (this.SelectedItems.Count > 0)
            {
                selection = (MkvTrack?)this.SelectedItems[0].Tag;
            }

            OnSelectionChanged?.Invoke(selection);
        }
    }
}
