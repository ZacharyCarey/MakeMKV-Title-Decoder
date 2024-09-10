using MakeMKV_Title_Decoder.libs.MkvToolNix.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.Forms.PlaylistCreator
{
    public class AppendedTrack {
        public Color Color;
        public AppendedFile Source;
        public MkvTrack Track;
        public bool Enabled = true;
        public List<AppendedTrack> AppendedTracks = new();
        public Color? BackColor;

        public AppendedTrack(AppendedFile sourceFile, MkvTrack sourceTrack, Color color, Color? backColor = null) {
            this.Source = sourceFile;
            this.Track = sourceTrack;
            this.Color = color;
            this.BackColor = backColor;
        }

        public bool IsCompatableWith(AppendedTrack other) {
            if (!Enabled) return true;
            if (this.Track.Type != other.Track.Type) return false;

            if (this.Track.Type == MkvTrackType.Video)
            {
                return true;
            } else if (this.Track.Type == MkvTrackType.Audio)
            {
                return this.Track?.Properties?.AudioChannels == other.Track.Properties?.AudioChannels;
            } else if (this.Track.Type == MkvTrackType.Subtitles)
            {
                return true;
            } else
            {
                // unknown
                return true;
            }
        }
    }
}
