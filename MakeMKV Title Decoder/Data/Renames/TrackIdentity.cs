using FFMpeg_Wrapper.ffprobe;
using Iso639;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Utils;

namespace MakeMKV_Title_Decoder.Data.Renames
{
    public class TrackIdentity
    {
        #region Common
        [JsonInclude, JsonConverter(typeof(JsonStringEnumConverter))]
        public readonly TrackType TrackType;

        /// <summary>
        /// The stream index as reported my ffprobe
        /// </summary>
        [JsonInclude]
        public readonly int Index;

        [JsonInclude]
        public readonly string Codec;

        [JsonInclude]
        public readonly string Type;

        [JsonInclude]
        public readonly long BitRate;

        [JsonInclude]
        public readonly TimeSpan Duration;

        [JsonInclude]
        public readonly string? Title = null;

        /// <summary>
        /// The track's language as an ISO 639-2 language code
        /// </summary>
        [JsonInclude, JsonConverter(typeof(LanguageJsonConverter))]
        public Language? Language;

        [JsonInclude]
        public readonly int? BitDepth;

        [JsonInclude]
        public readonly bool? DefaultFlag;

        [JsonInclude]
        public readonly bool? ForcedFlag;

        [JsonInclude]
        public readonly bool? DubFlag;

        [JsonInclude]
        public readonly bool? OriginalFlag;

        [JsonInclude]
        public readonly bool? CommentFlag;

        [JsonInclude]
        public readonly bool? LyricsFlag;

        [JsonInclude]
        public readonly bool? KaraokeFlag;

        [JsonInclude]
        public readonly bool? HearingImpairedFlag;

        [JsonInclude]
        public readonly bool? VisualImpairedFlag;

        [JsonInclude]
        public readonly bool? CaptionsFlag;

        [JsonInclude]
        public readonly bool? DescriptionsFlag;

        [JsonInclude]
        public readonly int? ID;
        #endregion

        #region Video
        [JsonInclude]
        public readonly int? Width = null;

        [JsonInclude]
        public readonly int? Height = null;

        [JsonInclude]
        public readonly string? PixelFormat = null;

        [JsonInclude]
        public readonly string? ColorRange = null;

        [JsonInclude]
        public readonly string? ColorSpace = null;

        [JsonInclude]
        public readonly bool Interlaced = false;
        #endregion

        #region Audio
        [JsonInclude]
        public readonly int? Channels = null;

        [JsonInclude]
        public readonly string? ChannelLayout = null;

        [JsonInclude]
        public readonly int? SampleRateHz = null;
        #endregion

        private TrackIdentity(MediaStream info, TrackType type) {
            this.TrackType = type;
            this.Index = info.Index;
            this.Codec = info.CodecName;
            this.BitRate = info.BitRate;
            this.Duration = info.Duration;
            this.Language = info.Language;
            this.BitDepth = info.BitDepth;
            this.ID = info.ID;

            // Disposition flags
            DefaultFlag = TryGetDispositionFlag(info.Disposition, "default");
            ForcedFlag = TryGetDispositionFlag(info.Disposition, "forced");
            DubFlag = TryGetDispositionFlag(info.Disposition, "dub");
            OriginalFlag = TryGetDispositionFlag(info.Disposition, "original");
            CommentFlag = TryGetDispositionFlag(info.Disposition, "comment");
            LyricsFlag = TryGetDispositionFlag(info.Disposition, "lyrics");
            KaraokeFlag = TryGetDispositionFlag(info.Disposition, "karaoke");
            HearingImpairedFlag = TryGetDispositionFlag(info.Disposition, "hearing_impaired");
            VisualImpairedFlag = TryGetDispositionFlag(info.Disposition, "visual_impaired");
            CaptionsFlag = TryGetDispositionFlag(info.Disposition, "captions");
            DescriptionsFlag = TryGetDispositionFlag(info.Disposition, "descriptions");

            // Tags
            this.Language = info.Language;
            if (info.Tags != null)
            {
                if (!info.Tags.TryGetValue("title", out this.Title))
                {
                    this.Title = null;
                }
            }
        }

        internal TrackIdentity(AudioStream info) : this(info, TrackType.Audio) {
            this.Channels = info.Channels;
            this.ChannelLayout = info.ChannelLayout;
            this.SampleRateHz = info.SampleRateHz;
        }

        internal TrackIdentity(VideoStream info) : this(info, TrackType.Video)
        {
            this.Width = info.Width;
            this.Height = info.Height;
            this.PixelFormat = info.PixelFormat;
            this.ColorRange = info.ColorRange;
            this.ColorSpace = info.ColorSpace;

            // Auto-check for interlacing
            if (!string.IsNullOrWhiteSpace(info.FieldOrder))
            {
                switch(info.FieldOrder)
                {
                    case "tt":
                    case "bb":
                    case "tb":
                    case "bt":
                        this.Interlaced = true;
                        break;
                    default:
                        break;
                }
            }
        }

        internal TrackIdentity(SubtitleStream info) : this(info, TrackType.Subtitle)
        {

        }

        private bool? TryGetDispositionFlag(Dictionary<string, bool>? disposition, string key) {
            if (disposition == null) return null;
            if (disposition.TryGetValue(key, out bool value))
            {
                return value;
            }

            return null;
        }

        [JsonConstructor]
        private TrackIdentity(TrackType trackType, int index, string codec, string type, long bitRate, TimeSpan duration, Language? language,
            int? bitDepth, bool? defaultFlag, bool? forcedFlag, bool? dubFlag, bool? originalFlag, bool? commentFlag,
            bool? lyricsFlag, bool? karaokeFlag, bool? hearingImpairedFlag, bool? visualImpairedFlag, bool? captionsFlag,
            bool? descriptionsFlag, int? id, int? width, int? height, string? pixelFormat, string? colorRange, string? colorSpace,
            int? channels, string? channelLayout, int? sampleRateHz) 
        {
            this.TrackType = trackType;
            Index = index;
            Codec = codec;
            Type = type;
            BitRate = bitRate;
            Duration = duration;
            Language = language;
            BitDepth = bitDepth;
            DefaultFlag = defaultFlag;
            ForcedFlag = forcedFlag;
            DubFlag = dubFlag;
            OriginalFlag = originalFlag;
            CommentFlag = commentFlag;
            LyricsFlag = lyricsFlag;
            KaraokeFlag = karaokeFlag;
            HearingImpairedFlag = hearingImpairedFlag;
            VisualImpairedFlag = visualImpairedFlag;
            CaptionsFlag = captionsFlag;
            DescriptionsFlag = descriptionsFlag;
            ID = id;
            Width = width;
            Height = height;
            PixelFormat = pixelFormat;
            ColorRange = colorRange;
            ColorSpace = colorSpace;
            Channels = channels;    
            ChannelLayout = channelLayout;
            SampleRateHz = sampleRateHz;
        }

        /// <summary>
        /// Returns the reason it didnt match, or null is matched successfully
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public string? Match(TrackIdentity other)
        {
            if (this.TrackType != other.TrackType) return "Track type did not match.";
            if (this.Index != other.Index) return "Index did not match.";
            if (this.Codec != other.Codec) return "Codec did not match.";
            if (this.Type != other.Type) return "Track type did not match.";
            //if (this.BitRate != other.BitRate) return "BitRate did not match.";
            //if (this.Duration != other.Duration) return "Duration did not match.";
            if (this.Language != other.Language) return "Language did not match.";
            //if (this.BitDepth != other.BitDepth) return "Bit depth did not match";
            if (this.DefaultFlag != other.DefaultFlag) return "Default flag did not match.";
            if (this.ForcedFlag != other.ForcedFlag) return "Forced flag did not match.";
            if (this.DubFlag != other.DubFlag) return "Dub flag did not match.";
            if (this.OriginalFlag != other.OriginalFlag) return "Original flag did not match.";
            if (this.CommentFlag != other.CommentFlag) return "Comment flag did not match.";
            if (this.LyricsFlag != other.LyricsFlag) return "Lyrics flag did not match.";
            if (this.KaraokeFlag != other.KaraokeFlag) return "Karaoke flag did not match.";
            if (this.HearingImpairedFlag != other.HearingImpairedFlag) return "Hearing impaired flag did not match.";
            if (this.VisualImpairedFlag != other.VisualImpairedFlag) return "Visual impaired flag did not match.";
            if (this.CaptionsFlag != other.CaptionsFlag) return "Captions flag did not match.";
            if (this.DescriptionsFlag != other.DescriptionsFlag) return "Descriptions flag did not match.";
            if (this.Width != other.Width || this.Height != other.Height) return "Video size did not match.";
            if (this.PixelFormat != other.PixelFormat) return "Pixel format did not match.";
            if (this.ColorRange != other.ColorRange) return "Color range did not match.";
            if (this.ColorSpace != other.ColorSpace) return "Color space did not match.";
            if (this.ChannelLayout != other.ChannelLayout) return "Channel layout did not match.";
            if (this.SampleRateHz != other.SampleRateHz) return "Sample rate hz did not match.";
            if (this.ID != other.ID) return "Track ID did not match";

            return null;
        }

    }

    public enum TrackType {
        Video,
        Audio,
        Subtitle
    }
}
