using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.Data.Matroska {

    public abstract class MkvTrack {
        public int Number;
        public string UID;
        public string Type;
        public string Name = "";
        public string Language;
        public string CodecID;
        public string CodecPrivateData;
        public bool DefaultTrack = false;

        protected MkvTrack() {

        }

        public static MkvTrack Parse(KeyValuePair<string, object?> data) {
            if (data.Key != "Track")
            {
                throw new FormatException();
            }
            var track = (Dictionary<string, object?>)data.Value;

            string type = (string)track[MkvInfoKeys.TrackType];

            MkvTrack result;
            if (type == "video" || track.ContainsKey(MkvInfoKeys.VideoTrack))
            {
                result = MkvVideoTrack.Parse(track);
            } else if (type == "audio" || track.ContainsKey(MkvInfoKeys.AudioTrack))
            {
                result = MkvAudioTrack.Parse(track);
            } else if (type == "subtitles" || track.ContainsKey(MkvInfoKeys.ContentEncodings))
            {
                result = MkvSubtitlesTrack.Parse(track);
            } else
            {
                throw new FormatException();
            }

            result.Number = (int)track[MkvInfoKeys.TrackNumber];
            result.UID = (string)track[MkvInfoKeys.TrackUID];
            result.Type = type;
            if (track.ContainsKey(MkvInfoKeys.Name))
            {
                result.Name = (string)track[MkvInfoKeys.Name];
            }
            result.Language = (string)track[MkvInfoKeys.LanguageIETF];
            result.CodecID = (string)track[MkvInfoKeys.CodecID];
            result.CodecPrivateData = (string)track[MkvInfoKeys.CodecPrivateData];
            if (track.ContainsKey(MkvInfoKeys.Flag_DefaultTrack))
            {
                result.DefaultTrack = (bool)track[MkvInfoKeys.Flag_DefaultTrack];
            }

            return result;
        }
    }

    public class MkvVideoTrack : MkvTrack {
        public int PixelWidth;
        public int PixelHeight;
        public Size PixelSize => new(PixelWidth, PixelHeight);

        public int DisplayWidth;
        public int DisplayHeight;
        public Size DisplaySize => new(DisplayWidth, DisplayHeight);

        public bool Lacing;

        protected MkvVideoTrack() {

        }

        public static new MkvVideoTrack Parse(Dictionary<string, object?> data) {
            MkvVideoTrack result = new();

            result.Lacing = (bool)data[MkvInfoKeys.Flag_Lacing];

            var track = (Dictionary<string, object?>)data[MkvInfoKeys.VideoTrack];
            result.PixelWidth = (int)track[MkvInfoKeys.PixelWidth];
            result.PixelHeight = (int)track[MkvInfoKeys.PixelHeight];
            result.DisplayWidth = (int)track[MkvInfoKeys.DisplayWidth];
            result.DisplayHeight = (int)track[MkvInfoKeys.DisplayHeight];

            return result;
        }
    }

    public class MkvAudioTrack : MkvTrack {
        public int SamplingFrequency;
        public int Channels;
        public int BitDepth;
        public bool CommentaryFlag = false;

        protected MkvAudioTrack() {

        }

        public static new MkvAudioTrack Parse(Dictionary<string, object?> data) {
            MkvAudioTrack result = new();

            var track = (Dictionary<string, object?>)data[MkvInfoKeys.AudioTrack];
            result.SamplingFrequency = (int)track[MkvInfoKeys.SamplingFrequency];
            result.Channels = (int)track[MkvInfoKeys.Channels];
            result.BitDepth = (int)track[MkvInfoKeys.BitDepth];

            if (data.ContainsKey(MkvInfoKeys.Flag_Commentary))
            {
                result.CommentaryFlag = (bool)data[MkvInfoKeys.Flag_Commentary];
            }

            return result;
        }
    }

    public class MkvSubtitlesTrack : MkvTrack {
        public bool Lacing;

        protected MkvSubtitlesTrack() {

        }

        public static new MkvSubtitlesTrack Parse(Dictionary<string, object?> data) {
            MkvSubtitlesTrack result = new();

            result.Lacing = (bool)data[MkvInfoKeys.Flag_Lacing];

            return result;
        }
    }
}
