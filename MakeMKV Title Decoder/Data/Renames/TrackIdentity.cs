using MkvToolNix.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.Data.Renames
{
    public class TrackIdentity
    {
        [JsonInclude]
        public readonly string Codec;

        [JsonInclude]
        public readonly MkvTrackType? Type;

        // Track -> Properties ->
        [JsonInclude]
        public readonly long? AudioBitsPerSample;

        [JsonInclude]
        public readonly long? AudioChannels;

        [JsonInclude]
        public readonly long? AudioSamplingFrequency;

        [JsonInclude]
        public readonly string? CodecID;

        [JsonInclude]
        public readonly string? DisplayDimensions;

        [JsonInclude]
        public readonly long? DisplayUnit;

        [JsonInclude]
        public readonly bool? FlagHearingImpaired;

        [JsonInclude]
        public readonly bool? FlagVisualImpaired;

        [JsonInclude]
        public readonly bool? FlagTextDescriptions;

        [JsonInclude]
        public readonly bool? FlagOriginal;

        [JsonInclude]
        public readonly bool? FlagCommentary;

        /// <summary>
        /// The track's language as an ISO 639-2 language code
        /// </summary>
        [JsonInclude]
        public readonly string? Language;

        [JsonInclude]
        public readonly long? Number;

        [JsonInclude]
        public readonly string? PixelDimensions;

        [JsonInclude]
        public readonly string? TrackName;

        [JsonInclude]
        public readonly bool? Enabled;

        [JsonInclude]
        public readonly bool? Forced;

        [JsonInclude]
        public readonly bool? Default;

        internal TrackIdentity(MkvTrack info)
        {
            this.Codec = info.Codec;
            this.Type = info.Type;
            this.AudioBitsPerSample = info.Properties?.AudioBitsPerSample;
            this.AudioChannels = info.Properties?.AudioChannels;
            this.AudioSamplingFrequency = info.Properties?.AudioSamplingFrequency;
            this.CodecID = info.Properties?.CodecID;
            this.DisplayDimensions = info.Properties?.DisplayDimensions;
            this.DisplayUnit = info.Properties?.DisplayUnit;
            this.FlagHearingImpaired = info.Properties?.FlagHearingImpaired;
            this.FlagVisualImpaired = info.Properties?.FlagVisualImpaired;
            this.FlagTextDescriptions = info.Properties?.FlagTextDescriptions;
            this.FlagOriginal = info.Properties?.FlagOriginal;
            this.FlagCommentary = info.Properties?.FlagCommentary;
            this.Language = info.Properties?.Language;
            this.Number = info.Properties?.Number;
            this.PixelDimensions = info.Properties?.PixelDimensions;
            this.TrackName = info.Properties?.TrackName;
            this.Enabled = info.Properties?.EnabledTrack;
            this.Forced = info.Properties?.ForcedTrack;
            this.Default = info.Properties?.DefaultTrack;
        }

        [JsonConstructor]
        private TrackIdentity(string codec, MkvTrackType? type, long? audioBitsPerSample, long? audioChannels, long? audioSamplingFrequency, string? codecID,
            string? displayDimensions, long? displayUnit, bool? flagHearingImpaired, bool? flagVisualImpaired, bool? flagTextDescriptions, bool? flagOriginal,
            bool? flagCommentary, string? language, long? number, string? pixelDimensions, string? trackName, bool? enabled, bool? forced, bool? Default) 
        { 
            this.Codec = codec;
            this.Type = type;
            this.AudioBitsPerSample = audioBitsPerSample;
            this.AudioChannels = audioChannels;
            this.AudioSamplingFrequency = audioSamplingFrequency;
            this.CodecID = codecID;
            this.DisplayDimensions = displayDimensions;
            this.DisplayUnit = displayUnit;
            this.FlagHearingImpaired = flagHearingImpaired;
            this.FlagVisualImpaired = flagVisualImpaired;
            this.FlagTextDescriptions = flagTextDescriptions;
            this.FlagOriginal = flagOriginal;
            this.FlagCommentary = flagCommentary;
            this.Language = language;
            this.Number = number;
            this.PixelDimensions = pixelDimensions;
            this.TrackName = trackName;
            this.Enabled = enabled;
            this.Forced = forced;
            this.Default = Default;
        }

        /// <summary>
        /// Returns the reason it didnt match, or null is matched successfully
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public string? Match(TrackIdentity other)
        {
            if (this.Codec != other.Codec) return "Codec did not match.";
            if (this.Type != other.Type) return "Track type did not match.";
            if (this.AudioBitsPerSample != other.AudioBitsPerSample) return "Audio bits per sample did not match.";
            if (this.AudioChannels != other.AudioChannels) return "Audio channels did not match.";
            if (this.AudioSamplingFrequency != other.AudioSamplingFrequency) return "Audio sampling frequency did not match.";
            if (this.CodecID != other.CodecID) return "Codec ID did not match.";
            if (this.DisplayDimensions != other.DisplayDimensions) return "Display dimensions did not match.";
            if (this.DisplayUnit != other.DisplayUnit) return "Display unit did not match.";
            if (this.FlagHearingImpaired != other.FlagHearingImpaired) return "Hearing impaired flag did not match.";
            if (this.FlagVisualImpaired != other.FlagVisualImpaired) return "Visual impaired flag did not match.";
            if (this.FlagTextDescriptions != other.FlagTextDescriptions) return "Text descriptions flag did not match.";
            if (this.FlagOriginal != other.FlagOriginal) return "Original flag did not match.";
            if (this.FlagCommentary != other.FlagCommentary) return "Commentary flag did not match.";
            if (this.Language != other.Language) return "Language did not match.";
            if (this.Number != other.Number) return "Number did not match.";
            if (this.PixelDimensions != other.PixelDimensions) return "Pixel dimensions did not match.";
            if (this.TrackName != other.TrackName) return "Track name did not match.";
            if (this.Enabled != other.Enabled) return "Track enabled did not match.";
            if (this.Forced != other.Forced) return "Track forced did not match";

            return null;
        }

    }
}
