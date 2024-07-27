using JsonSerializable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder {
    public enum TrackType {
        Video,
        Audio,
        Subtitle,
        Attachment
    }

    public struct Track : IJsonSerializable, IEquatable<Track> {
        public TrackType Type;
        public string? Codec = null; // Video, Audio, Subtitles, Attachment
        public Size? Resolution = null; // Video, Attachment
        public SizeF? AspectRatio = null; // Video
        public double? FrameRate = null; // Video
        public string? Name = null; // Audio, Attachment
        public string? Language = null; // Audio, Subtitles
        public int? Channels = null; // Audio
        public string? ChannelLayout = null; // Audio
        public int? SampleRate = null; // Audio
        public int? BitsPerSample = null; // Audio
        public string? Flags = null; // Audio
        public string? SourceFileName = null; // Attachment
        public DataSize? Size = null; // Attachment

        public Track() {

        }

        public static bool TryParse(string info, out Track result) {
            try
            {
                result = Parse(info);
                return true;
            } catch(Exception ex)
            {
                result = new();
                return false;
            }
        }

        public static Track Parse(string info) {
            using (StringReader sr = new StringReader(info))
            {
                string? line = sr.ReadLine();
                if (line != "Track information" && line != "Attachment information")
                {
                    throw new FormatException("Invalid track info.");
                }
                return Parse(sr);
            }
        }

        private static Track Parse(StringReader info) {
            Track result = new();
            result.Type = ParseType(info.ReadLine());

            string? line;
            while ((line = info.ReadLine()) != null)
            {
                int colon = line.IndexOf(':');
                string key = line.Substring(0, colon);
                string value = line.Substring(colon + 2); // +2 to clear the space after the colon
                switch(key)
                {
                    case "Codec":
                        result.Codec = value;
                        break;
                    case "Resolution":
                        int x = value.IndexOf('x');
                        Size resolution = new();
                        resolution.Width = int.Parse(value.Substring(0, x));
                        resolution.Height = int.Parse(value.Substring(x + 1));
                        result.Resolution = resolution;
                        break;
                    case "Aspect ratio":
                        int divider = value.IndexOf(':');
                        SizeF aspect = new();
                        aspect.Width = float.Parse(value.Substring(0, divider));
                        aspect.Height = float.Parse(value.Substring(divider + 1));
                        result.AspectRatio = aspect;
                        break;
                    case "Frame rate":
                        result.FrameRate = float.Parse(value.Split()[0]);
                        break;
                    case "Name":
                        result.Name = value;
                        break;
                    case "Language":
                        result.Language = value;
                        break;
                    case "Channels":
                        result.Channels = int.Parse(value);
                        break;
                    case "Channel layout":
                        result.ChannelLayout = value;
                        break;
                    case "Sample rate":
                        result.SampleRate = int.Parse(value);
                        break;
                    case "Bits per sample":
                        result.BitsPerSample = int.Parse(value);
                        break;
                    case "Source file name":
                        result.SourceFileName = value;
                        break;
                    case "Size":
                        result.Size = DataSize.Parse(value);
                        break;
                    case "Flags":
                        result.Flags = value;
                        break;
                    default:
                        throw new FormatException("Unknown track data: " + key);
                }
            }

            return result;
        }

        private static TrackType ParseType(string? typeInfo) {
            switch(typeInfo)
            {
                case "Type: Subtitles": return TrackType.Subtitle;
                case "Type: Video": return TrackType.Video;
                case "Type: Audio": return TrackType.Audio;
                case "Type: Attachment": return TrackType.Attachment;
                default:
                    throw new FormatException("Unknown track type: " + typeInfo);
            }
        }

        public static bool operator ==(Track left, Track right) {
            return (left.Type == right.Type)
                && (left.Codec == right.Codec)
                && (left.Resolution == right.Resolution)
                && (left.AspectRatio == right.AspectRatio)
                && (left.FrameRate == right.FrameRate)
                && (left.Name == right.Name)
                && (left.Language == right.Language)
                && (left.Channels == right.Channels)
                && (left.ChannelLayout == right.ChannelLayout)
                && (left.SampleRate == right.SampleRate)
                && (left.BitsPerSample == right.BitsPerSample)
                && (left.Flags == right.Flags)
                && (left.SourceFileName == right.SourceFileName)
                && (left.Size.HasValue && right.Size.HasValue && left.Size.Value.Near(right.Size.Value));

        }

        public static bool operator !=(Track left, Track right) => !(left == right);

        public bool Equals(Track other) {
            return this == other;
        }

        public JsonData SaveToJson() {
            JsonObject obj = new();

            obj["Type"] = new JsonString(this.Type.ToString());
            if (this.Codec != null) obj["Codec"] = new JsonString(this.Codec);
            if (this.Resolution.HasValue) obj["Resolution"] = SaveToJson(this.Resolution.Value);
            if (this.AspectRatio.HasValue) obj["Aspect Ratio"] = SaveToJson(this.AspectRatio.Value);
            if (this.FrameRate.HasValue) obj["Frame Rate"] = new JsonDecimal(this.FrameRate.Value);
            if (this.Name != null) obj["Name"] = new JsonString(this.Name);
            if (this.Language != null) obj["Language"] = new JsonString(this.Language);
            if (this.Channels.HasValue) obj["Channels"] = new JsonInteger(this.Channels.Value);
            if (this.ChannelLayout != null) obj["Channel Layout"] = new JsonString(this.ChannelLayout);
            if (this.SampleRate.HasValue) obj["Sample Rate"] = new JsonInteger(this.SampleRate.Value);
            if (this.BitsPerSample.HasValue) obj["Bits per Sample"] = new JsonInteger(this.BitsPerSample.Value);
            if (this.Flags != null) obj["Flags"] = new JsonString(this.Flags);
            if (this.SourceFileName != null) obj["Source File Name"] = new JsonString(this.SourceFileName);
            if (this.Size.HasValue) obj["Size"] = this.Size.Value.SaveToJson();

            return obj;
        }

        public void LoadFromJson(JsonData Data) {
            JsonObject obj = (JsonObject)Data;

            this.Type = LoadTrackFromJson(obj["Type"]);
            this.Codec = LoadStringFromJson(obj["Codec"]);
            this.Resolution = LoadSizeFromJson(obj["Resolution"]);
            this.AspectRatio = LoadSizeFFromJson(obj["Aspect Ratio"]);
            this.FrameRate = LoadDoubleFromJson(obj["Frame Rate"]);
            this.Name = LoadStringFromJson(obj["Name"]);
            this.Language = LoadStringFromJson(obj["Language"]);
            this.Channels = LoadIntFromJson(obj["Channels"]);
            this.ChannelLayout = LoadStringFromJson(obj["Channel Layout"]);
            this.SampleRate = LoadIntFromJson(obj["Sample Rate"]);
            this.BitsPerSample = LoadIntFromJson(obj["Bits per Sampe"]);
            this.Flags = LoadStringFromJson(obj["Flags"]);
            this.SourceFileName = LoadStringFromJson(obj["Source File Name"]);
            this.Size = LoadDataSizeFromJson(obj["Size"]);
        }

        private static TrackType LoadTrackFromJson(JsonData data) {
            string str = (JsonString)data;
            switch(str)
            {
                case "Video": return TrackType.Video;
                case "Audio": return TrackType.Audio;
                case "Subtitle": return TrackType.Subtitle;
                case "Attachment": return TrackType.Attachment;
                default:
                    throw new FormatException("Unknown track type: " + str);
            }
        }

        private static JsonData SaveToJson(Size value) {
            JsonObject obj = new();
            obj["Width"] = new JsonInteger(value.Width);
            obj["Height"] = new JsonInteger(value.Height);
            return obj;
        }

        private static Size? LoadSizeFromJson(JsonData data) {
            if (data == null) return null;
            JsonObject obj = (JsonObject)data;
            return new Size(
                (int)(JsonInteger)obj["Width"],
                (int)(JsonInteger)obj["Height"]
            );
        }

        private static JsonData SaveToJson(SizeF value) {
            JsonObject obj = new();
            obj["Width"] = new JsonDecimal(value.Width);
            obj["Height"] = new JsonDecimal(value.Height);
            return obj;
        }

        private static SizeF? LoadSizeFFromJson(JsonData data) {
            if (data == null) return null;
            JsonObject obj = (JsonObject)data;
            return new SizeF(
                (float)(JsonDecimal)obj["Width"],
                (float)(JsonDecimal)obj["Height"]
            );
        }

        private static double? LoadDoubleFromJson(JsonData data) {
            if (data == null)
            {
                return null;
            } else
            {
                return (double)(JsonDecimal)data;
            }
        }

        private static int? LoadIntFromJson(JsonData data) {
            if (data == null)
            {
                return null;
            } else
            {
                return (int)(JsonInteger)data;
            }
        }

        private static string? LoadStringFromJson(JsonData data) {
            if (data == null)
            {
                return null;
            } else
            {
                return (JsonString)data;
            }
        }

        private static DataSize? LoadDataSizeFromJson(JsonData data) {
            if (data == null)
            {
                return null;
            } else
            {
                DataSize size = new();
                size.LoadFromJson(data);
                return size;
            }
        }

        public override string ToString() {
            return ToString(0);
        }

        public string ToString(int tabs) {
            StringBuilder sb = new();
            sb.Append('\t', tabs);
            sb.AppendLine($"{this.Type.ToString()}: {{");

            if (this.Codec != null)
            {
                sb.Append('\t', tabs + 1);
                sb.AppendLine($"Codec: {this.Codec}");
            }

            if (this.Resolution.HasValue)
            {
                sb.Append('\t', tabs + 1);
                sb.AppendLine($"Resolution: {this.Resolution.Value.Width}x{this.Resolution.Value.Height}");
            }

            if (this.AspectRatio.HasValue)
            {
                sb.Append('\t', tabs + 1);
                sb.AppendLine($"Aspect Ratio: {this.AspectRatio.Value.Width}:{this.AspectRatio.Value.Height}");
            }

            if (this.FrameRate.HasValue)
            {
                sb.Append('\t', tabs + 1);
                sb.AppendLine($"Frame Rate: {this.FrameRate.Value}");
            }

            if (this.Name != null)
            {
                sb.Append('\t', tabs + 1);
                sb.AppendLine($"Name: {this.Name}");
            }

            if (this.Language != null)
            {
                sb.Append('\t', tabs + 1);
                sb.AppendLine($"Language: {this.Language}");
            }

            if (this.Channels.HasValue)
            {
                sb.Append('\t', tabs + 1);
                sb.AppendLine($"Channels: {this.Channels.Value}");
            }

            if (this.ChannelLayout != null)
            {
                sb.Append('\t', tabs + 1);
                sb.AppendLine($"Channel Layout: {this.ChannelLayout}");
            }

            if (this.SampleRate.HasValue)
            {
                sb.Append('\t', tabs + 1);
                sb.AppendLine($"Sample Rate: {this.SampleRate.Value}");
            }

            if (this.BitsPerSample.HasValue)
            {
                sb.Append('\t', tabs + 1);
                sb.AppendLine($"Bits per Sample: {this.BitsPerSample.Value}");
            }

            if (this.Flags != null)
            {
                sb.Append('\t', tabs + 1);
                sb.AppendLine($"Flags: {this.Flags}");
            }

            if(this.SourceFileName != null)
            {
                sb.Append('\t', tabs + 1);
                sb.AppendLine($"Source File Name: {this.SourceFileName}");
            }

            if (this.Size.HasValue)
            {
                sb.Append('\t', tabs + 1);
                sb.AppendLine($"Size: {this.Size}");
            }

            sb.Append('\t', tabs);
            sb.Append('}');

            return sb.ToString();
        }
    }
}
