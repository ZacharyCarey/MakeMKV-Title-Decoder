using MakeMKV_Title_Decoder.Data;
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
        public LoadedTrack Track;
        public bool Enabled = true;
        public List<AppendedTrack> AppendedTracks = new();
        public Color? BackColor;

        public AppendedTrack(AppendedFile sourceFile, LoadedTrack sourceTrack, Color color, Color? backColor = null) {
            this.Source = sourceFile;
            this.Track = sourceTrack;
            this.Color = color;
            this.BackColor = backColor;
        }

        public bool IsCompatableWith(AppendedTrack other) {
            if (!Enabled) return true;
            if (this.Track.Data.Type != other.Track.Data.Type) return false;

            if (this.Track.Data.Type == MkvTrackType.Video)
            {
                return true;
            } else if (this.Track.Data.Type == MkvTrackType.Audio)
            {
                return this.Track.Data.Properties?.AudioChannels == other.Track.Data.Properties?.AudioChannels;
            } else if (this.Track.Data.Type == MkvTrackType.Subtitles)
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
