﻿using MakeMKV_Title_Decoder.Data;
using MakeMKV_Title_Decoder.Forms.PlaylistCreator;
using MakeMKV_Title_Decoder.libs.MkvToolNix.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MakeMKV_Title_Decoder.Controls
{
    public class TrackListData : PropertyData
    {
        public LoadedTrack? Track = null;
    }

    public class TrackList : PropertiesList
    {

        public delegate void OnSelectionChangedDelegate(TrackListData? track);
        public event OnSelectionChangedDelegate OnSelectionChanged;

        public string KeepIconKey { get; set; } = "dialog-ok-apply.png";
        public string DeleteIconKey { get; set; } = "dialog-cancel.png";

        const int FirstColumnWidth = 160;
        private int padding = 0;
        public int ColumnPadding
        {
            get => padding;
            set
            {
                padding = Math.Max(0, value);
                if (Columns.Count > 0)
                {
                    Columns[0].Width = FirstColumnWidth + padding;
                }
            }
        }

        public new IEnumerable<TrackListData> Items => base.Items.Cast<TrackListData>();

        public TrackList()
        {
            ColumnHeader col1 = new();
            col1.Text = "Codec";
            col1.Width = FirstColumnWidth;
            col1.DisplayIndex = 0;
            Columns.Add(col1);

            ColumnHeader col9 = new();
            col9.Text = "Type";
            col9.Width = 90;
            col9.DisplayIndex = 1;
            Columns.Add(col9);

            ColumnHeader col2 = new();
            col2.Text = "Name";
            col2.Width = 160;
            col2.DisplayIndex = 2;
            Columns.Add(col2);

            ColumnHeader col7 = new();
            col7.Text = "Properties";
            col7.Width = 90;
            col7.DisplayIndex = 3;
            Columns.Add(col7);

            ColumnHeader col3 = new();
            col3.Text = "Lang";
            col3.Width = 40;
            col3.DisplayIndex = 4;
            Columns.Add(col3);

            ColumnHeader col4 = new();
            col4.Text = "Enabled";
            col4.Width = 60;
            col4.DisplayIndex = 5;
            Columns.Add(col4);

            ColumnHeader col8 = new();
            col8.Text = "Default";
            col8.Width = 60;
            col8.DisplayIndex = 6;
            Columns.Add(col8);

            ColumnHeader col5 = new();
            col5.Text = "Forced";
            col5.Width = 50;
            col5.DisplayIndex = 7;
            Columns.Add(col5);

            ColumnHeader col6 = new();
            col6.Text = "Commentary";
            col6.Width = 90;
            col6.DisplayIndex = 8;
            Columns.Add(col6);

            SelectedIndexChanged += Event_SelectedIndexChanged;
        }

        public TrackListData Add(LoadedTrack track, Color? color = null, int? padding = null, string? Icon = null, Color? backColor = null, object? Tag = null)
        {
            TrackListData data = new();
            data.Track = track;
            data.IconColor = color;
            data.Padding = padding ?? 0;
            data.IconKey = Icon;
            data.BackColor = backColor;
            data.Tag = null;

            TrackListData[] subItems = new TrackListData[8];
            for (int i = 0; i < subItems.Length; i++)
            {
                subItems[i] = new TrackListData();
                subItems[i].BackColor = backColor;
            }

            base.Add(data, subItems);

            Update(track);
            return data;
        }

        public TrackListData Add(TrackDelay delay, Color color, int padding, Color? backColor) {
            TrackListData data = new();
            data.Track = null;
            data.IconColor = color;
            data.Padding = padding;
            data.BackColor = BackColor;
            data.Tag = delay;

            TrackListData[] subItems = new TrackListData[8];
            for(int i = 0; i < subItems.Length; i++)
            {
                subItems[i] = new TrackListData();
                subItems[i].BackColor = backColor;
            }

            base.Add(data, subItems);

            Update(delay);
            return data;
        }

        public void Update(LoadedTrack track)
        {
            foreach(var item in base.Items.Cast<TrackListData>())
            {
                if (item.Track == track)
                {
                    var data = track.Rename;

                    List<string> trackProperties = new();
                    switch (track.Data.Type)
                    {
                        case MkvTrackType.Video:
                            var size = track.Data.Properties?.PixelDimensions;
                            if (size != null) trackProperties.Add($"{size} pixels");
                            break;
                        case MkvTrackType.Audio:
                            var freq = track.Data.Properties?.AudioSamplingFrequency;
                            if (freq != null) trackProperties.Add($"{freq} Hz");

                            var channels = track.Data.Properties?.AudioChannels;
                            if (channels != null) trackProperties.Add($"{channels} channels");
                            break;
                    }

                    item.Text = track.Data.Codec;

                    item.SubItems[0].Text = track.Data.Type.ToString();
                    item.SubItems[0].IconKey = GetTypeIcon(track.Data.Type);

                    item.SubItems[1].Text = data?.Name ?? ""; // name
                    item.SubItems[1].IconColor = null;
                    item.SubItems[2].Text = string.Join(", ", trackProperties); // properties
                    item.SubItems[3].Text = track.Data.Properties?.Language ?? ""; // lang
                    item.SubItems[4].IconKey = GetBoolIcon(track.Data.Properties?.EnabledTrack ?? true); // enabled
                    item.SubItems[5].IconKey = GetBoolIcon((data?.DefaultFlag) ?? track.Data.Properties?.DefaultTrack ?? true); // default
                    item.SubItems[6].IconKey = GetBoolIcon(track.Data.Properties?.ForcedTrack ?? false); // forced
                    item.SubItems[7].IconKey = GetBoolIcon((data?.CommentaryFlag) ?? track.Data.Properties?.FlagCommentary ?? false); // commentary

                    return;
                }
            }

            // TODO throw error
        }

        public void Update(TrackDelay delay) {
            foreach(var item in base.Items.Cast<TrackListData>())
            {
                if (item.Tag != null && item.Tag is TrackDelay otherDelay && otherDelay == delay)
                {
                    item.Text = "Delay";
                    item.SubItems[0].IconKey = null;
                    item.SubItems[2].Text = "";
                    item.SubItems[3].Text = "";
                    item.SubItems[4].IconKey = null;
                    item.SubItems[5].IconKey = null;
                    item.SubItems[6].IconKey = null;
                    item.SubItems[7].IconKey = null;

                    if (delay.ClipDelay != null)
                    {
                        item.SubItems[0].Text = "Clip Sync";
                        item.SubItems[1].Text = delay.ClipDelay.Source.Rename.Name ?? "null";
                        item.SubItems[1].IconColor = delay.ClipDelay.Color;
                    } else
                    {
                        item.SubItems[0].Text = "Manual";
                        item.SubItems[1].Text = $"{delay.MillisecondDelay} ms";
                    }

                    return;
                }
            }

            // TODO throw error
        }

        public new void Clear()
        {
            base.Clear();
        }

        public new TrackListData? SelectedItem
        {
            get => (TrackListData?)base.SelectedItem;
            set => base.SelectedItem = (PropertyData?)value;
            /*set
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    TrackListData? track = (TrackListData?)((PropertyData)Items[i].Tag).Tag;
                    if (track != null && track == value)
                    {
                        SelectedIndex = i;
                        return;
                    }
                }
                SelectedIndex = null;
            }*/
        }

        public void Select(LoadedTrack? track)
        {
            if (track == null)
            {
                SelectedIndex = null;
                return;
            }

            foreach(var pair in this.Items.WithIndex())
            {
                if (pair.Value.Track == track)
                {
                    base.SelectedIndex = pair.Index;
                    return;
                }
            }

            SelectedIndex = null;
        }

        private string? GetBoolIcon(bool? val)
        {
            if (val == null)
            {
                return null;
            }

            return val.Value ? KeepIconKey : DeleteIconKey;
        }

        private string? GetTypeIcon(MkvTrackType type)
        {
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

        private void Event_SelectedIndexChanged(object? sender, EventArgs e)
        {
            OnSelectionChanged?.Invoke(SelectedItem);
        }
    }
}
