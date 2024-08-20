using JsonSerializable;
using MakeMKV_Title_Decoder.MakeMKV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MakeMKV_Title_Decoder.MakeMKV.Data
{
    public enum TrackType
    {
        Video,
        Audio,
        Subtitle,
        Attachment
    }

    public class Track : IJsonSerializable, IEquatable<Track>
    {
        public TrackType? Type;
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
        public string? OutputFormat = null; // Audio
        public string? OutputDescription = null; // Audio

        // Hold any extra data that isn't explicitly stored in it's own variable
        public SerializableDictionary<JsonString> Data = new();

        public Track()
        {

        }

        /// <exception cref="FormatException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="OverflowException"></exception>
        /// <exception cref="InvalidCastException"></exception>
        public static void ParseMakeMkv(Disc disc, MakeMkvMessage info)
        {
            if (info.Type != MakeMkvMessageType.StreamInfo || info.Arguments.Count != 5)
            {
                throw new FormatException("Incorrect message type.");
            }

            int titleIndex = (int)(long)info[0];
            int trackIndex = (int)(long)info[1];
            ApItemAttributeId id = (ApItemAttributeId)(long)info[2];
            int code = (int)(long)info[3];
            string value = (string)info[4];

            // We expect to have the title already. If not just throw an exception
            Title title = disc.Titles[titleIndex];

            if (trackIndex >= title.Tracks.Count)
            {
                title.Tracks.Add(new Track());
            }
            title.Tracks[trackIndex].ParseMakeMkv(id, code, value);
        }

        /// <exception cref="FormatException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="OverflowException"></exception>
        /// <exception cref="InvalidCastException"></exception>
        private void ParseMakeMkv(ApItemAttributeId id, int code, string value)
        {
            switch (id)
            {
                case ApItemAttributeId.Type:
                    Type = ParseType(value);
                    break;
                case ApItemAttributeId.CodecLong:
                    Codec = value;
                    break;
                case ApItemAttributeId.VideoSize:
                    int x = value.IndexOf('x');
                    Size resolution = new();
                    resolution.Width = int.Parse(value.Substring(0, x));
                    resolution.Height = int.Parse(value.Substring(x + 1));
                    Resolution = resolution;
                    break;
                case ApItemAttributeId.VideoAspectRatio:
                    int divider = value.IndexOf(':');
                    SizeF aspect = new();
                    aspect.Width = float.Parse(value.Substring(0, divider));
                    aspect.Height = float.Parse(value.Substring(divider + 1));
                    AspectRatio = aspect;
                    break;
                case ApItemAttributeId.VideoFrameRate:
                    FrameRate = float.Parse(value.Split()[0]);
                    break;
                case ApItemAttributeId.MetadataLanguageName:
                    Language = value;
                    break;
                case ApItemAttributeId.MkvFlags:
                    if (!string.IsNullOrEmpty(value))
                    {
                        Flags = value;
                    }
                    break;
                case ApItemAttributeId.Name:
                    Name = value;
                    break;
                case ApItemAttributeId.Bitrate:
                    // Ignored for now
                    break;
                case ApItemAttributeId.AudioChannelsCount:
                    Channels = int.Parse(value);
                    break;
                case ApItemAttributeId.AudioSampleRate:
                    SampleRate = int.Parse(value);
                    break;
                case ApItemAttributeId.AudioChannelLayoutName:
                    ChannelLayout = value;
                    break;
                case ApItemAttributeId.AudioSampleSize:
                    BitsPerSample = int.Parse(value);
                    break;
                case ApItemAttributeId.OutputFormat:
                    OutputFormat = value;
                    break;
                case ApItemAttributeId.OutputFormatDescription:
                    OutputDescription = value;
                    break;
                case ApItemAttributeId.LangCode:
                case ApItemAttributeId.LangName:
                case ApItemAttributeId.MkvFlagsText:
                case ApItemAttributeId.CodecId:
                case ApItemAttributeId.CodecShort:
                case ApItemAttributeId.StreamFlags:
                case ApItemAttributeId.MetadataLanguageCode:
                case ApItemAttributeId.TreeInfo: // MakeMKV gui stuff
                case ApItemAttributeId.PanelTitle: // MakeMKV gui stuff
                case ApItemAttributeId.OrderWeight: // MakeMKV gui stuff
                case ApItemAttributeId.OutputConversionType:
                    // Ignored
                    break;
                default:
                    Data[id.ToString()] = new(value);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"WARNING: Unknown stream attribute: {id.ToString()}={value}");
                    Console.ResetColor();
                    break;
                    // TODO no attachment information???
            }
        }

        private static TrackType ParseType(string? typeInfo)
        {
            switch (typeInfo)
            {
                case "Subtitles": return TrackType.Subtitle;
                case "Video": return TrackType.Video;
                case "Audio": return TrackType.Audio;
                case "Attachment": return TrackType.Attachment;
                default:
                    throw new FormatException("Unknown track type: " + typeInfo);
            }
        }

        /*public static bool operator ==(Track left, Track right) {
            if (left is null)
            {
                if (right is null)
                {
                    return true;
                } else
                {
                    return false;
                }
            } else
            {
                if (right is null)
                {
                    return false;
                } else
                {
                    // Check data below
                }
            }

            bool result = (left.Type == right.Type)
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
                && (left.Size.HasValue && right.Size.HasValue && left.Size.Value.Near(right.Size.Value))
                && (left.OutputFormat == right.OutputFormat)
                && (left.OutputDescription == right.OutputDescription)
                && left.Data.Count == right.Data.Count;
            if (!result) return false;
            foreach(var pair in left.Data)
            {
                JsonString? value;
                if (!right.Data.TryGetValue(pair.Key, out value))
                {
                    return false;
                }
                if (pair.Value != value)
                {
                    return false;
                }
            }
            return true;
        }

        public static bool operator !=(Track left, Track right) => !(left == right);
        */
        public bool Equals(Track other)
        {
            return this == other;
        }

        public JsonData SaveToJson()
        {
            JsonObject obj = new();

            obj.SaveToJson("Type", Type);
            obj.SaveToJson("Codec", Codec);
            obj.SaveToJson("Resolution", Resolution, SaveToJson);
            obj.SaveToJson("Aspect Ratio", AspectRatio, SaveToJson);
            obj.SaveToJson("Frame Rate", FrameRate, (v) => new JsonDecimal(v));
            obj.SaveToJson("Name", Name);
            obj.SaveToJson("Language", Language);
            obj.SaveToJson("Channels", Channels);
            obj.SaveToJson("Channel Layout", ChannelLayout);
            obj.SaveToJson("Sample Rate", SampleRate);
            obj.SaveToJson("Bits per Sample", BitsPerSample);
            obj.SaveToJson("Flags", Flags);
            obj.SaveToJson("Source File Name", SourceFileName);
            obj.SaveToJson("Size", (IJsonSerializable?)Size);
            obj.SaveToJson("Output Format", OutputFormat);
            obj.SaveToJson("Output Description", OutputDescription);

            obj["Data"] = Data.SaveToJson();

            return obj;
        }

        public void LoadFromJson(JsonData Data)
        {
            JsonObject obj = (JsonObject)Data;

            obj["Type"].LoadFromJson(out Type, LoadTrackFromJson);
            obj["Codec"].LoadFromJson(out Codec);
            obj["Resolution"].LoadFromJson(out Resolution, LoadSizeFromJson);
            obj["Aspect Ratio"].LoadFromJson(out AspectRatio, LoadSizeFFromJson);
            obj["Frame Rate"].LoadFromJson(out FrameRate, x => (double)(JsonDecimal)x);
            obj["Name"].LoadFromJson(out Name);
            obj["Language"].LoadFromJson(out Language);
            obj["Channels"].LoadFromJson(out Channels, x => (int)(JsonInteger)x);
            obj["Channel Layout"].LoadFromJson(out ChannelLayout);
            obj["Sample Rate"].LoadFromJson(out SampleRate, x => (int)(JsonInteger)x);
            obj["Bits per Sample"].LoadFromJson(out BitsPerSample, x => (int)(JsonInteger)x);
            obj["Flags"].LoadFromJson(out Flags);
            obj["Source File Name"].LoadFromJson(out SourceFileName);
            obj["Size"].LoadSerializableFromJson(out Size);
            obj["Output Format"].LoadFromJson(out OutputFormat);
            obj["Output Description"].LoadFromJson(out OutputDescription);

            this.Data.LoadFromJson(obj["Data"]);
        }

        private static TrackType LoadTrackFromJson(JsonData data)
        {
            string str = (JsonString)data;
            switch (str)
            {
                case "Video": return TrackType.Video;
                case "Audio": return TrackType.Audio;
                case "Subtitle": return TrackType.Subtitle;
                case "Attachment": return TrackType.Attachment;
                default:
                    throw new FormatException("Unknown track type: " + str);
            }
        }

        private static JsonData SaveToJson(Size value)
        {
            JsonObject obj = new();
            obj["Width"] = new JsonInteger(value.Width);
            obj["Height"] = new JsonInteger(value.Height);
            return obj;
        }

        private static Size LoadSizeFromJson(JsonData data)
        {
            JsonObject obj = (JsonObject)data;
            return new Size(
                (int)(JsonInteger)obj["Width"],
                (int)(JsonInteger)obj["Height"]
            );
        }

        private static JsonData SaveToJson(SizeF value)
        {
            JsonObject obj = new();
            obj["Width"] = new JsonDecimal(value.Width);
            obj["Height"] = new JsonDecimal(value.Height);
            return obj;
        }

        private static SizeF LoadSizeFFromJson(JsonData data)
        {
            JsonObject obj = (JsonObject)data;
            return new SizeF(
                (float)(JsonDecimal)obj["Width"],
                (float)(JsonDecimal)obj["Height"]
            );
        }

        private static DataSize? LoadDataSizeFromJson(JsonData data)
        {
            if (data == null)
            {
                return null;
            }
            else
            {
                DataSize size = new();
                size.LoadFromJson(data);
                return size;
            }
        }

        public override string ToString()
        {
            return ToString(0);
        }

        public string ToString(int tabs)
        {
            StringBuilder sb = new();
            sb.Append('\t', tabs);
            sb.AppendLine($"{Type.ToString()}: {{");

            sb.Append(tabs + 1, "Codec: ", Codec);
            sb.Append(tabs + 1, "Resolution: ", Resolution, x => $"{x.Width}x{x.Height}");
            sb.Append(tabs + 1, "Aspect Ratio: ", AspectRatio, x => $"{x.Width}:{x.Height}");
            sb.Append(tabs + 1, "Frame Rate: ", FrameRate);
            sb.Append(tabs + 1, "Name: ", Name);
            sb.Append(tabs + 1, "Language: ", Language);
            sb.Append(tabs + 1, "Channels: ", Channels);
            sb.Append(tabs + 1, "Channel Layout: ", ChannelLayout);
            sb.Append(tabs + 1, "Sample Rate: ", SampleRate);
            sb.Append(tabs + 1, "Bits per Sample: ", BitsPerSample);
            sb.Append(tabs + 1, "Flags", Flags);
            sb.Append(tabs + 1, "Source File Name: ", SourceFileName);
            sb.Append(tabs + 1, "Size: ", Size);

            sb.Append('\t', tabs + 1);
            sb.Append("Data: ");
            if (Data.Count == 0)
            {
                sb.AppendLine("{}");
            }
            else
            {
                sb.AppendLine();
                sb.Append('\t', tabs + 1);
                sb.AppendLine("{");

                sb.AppendLine(string.Join(",\r\n", Data.Select(x => $"{new string('\t', tabs + 2)}{x.Key}={x.Value}")));

                sb.Append('\t', tabs + 1);
                sb.AppendLine("}");
            }

            sb.Append('\t', tabs);
            sb.Append('}');

            return sb.ToString();
        }

        public IEnumerable<string> GetData()
        {
            if (Type != null) yield return $"Type: {Type.ToString()}";
            if (Codec != null) yield return $"Codec: {Codec}";
            if (Resolution != null) yield return $"Resolution: {Resolution.Value.Width}x{Resolution.Value.Height}";
            if (AspectRatio != null) yield return $"Aspect Ratio: {AspectRatio.Value.Width}:{AspectRatio.Value.Height}";
            if (FrameRate != null) yield return $"Frame Rate: {FrameRate:0.000} fps";
            if (Name != null) yield return $"Name: {Name}";
            if (Language != null) yield return $"Language: {Language}";
            if (Channels != null) yield return $"Channels: {Channels}";
            if (ChannelLayout != null) yield return $"Channel Layout: {ChannelLayout}";
            if (SampleRate != null) yield return $"Sample Rate: {SampleRate}";
            if (BitsPerSample != null) yield return $"Bits per Sample: {BitsPerSample}";
            if (Flags != null) yield return $"Flags: {Flags}";
            if (SourceFileName != null) yield return $"Source File Name: {SourceFileName}";
            if (Size != null) yield return $"Size: {Size.Value.ToString()}";
            if (OutputFormat != null) yield return $"Output Format: {OutputFormat}";
            if (OutputDescription != null) yield return $"Output Description: {OutputDescription}";
            foreach (var pair in Data)
            {
                yield return $"{pair.Key}: {pair.Value.Value}";
            }
        }

        public string GetSimplifiedName()
        {
            switch (Type)
            {
                case TrackType.Attachment: return $"Attachment: {Codec ?? ""} {ResolutionName()} {Name ?? ""}";
                case TrackType.Audio: return $"Audio: {Codec ?? ""} {Name ?? ""} {Language ?? ""}";
                case TrackType.Subtitle: return $"Subtitles: {Codec ?? ""} {Language ?? ""} {Flags ?? ""}";
                case TrackType.Video: return $"Video: {Codec ?? ""}";
                default:
                    throw new Exception("Unknown track type.");
            }
        }

        private string ResolutionName()
        {
            if (Resolution == null)
            {
                return "";
            }
            else
            {
                return $"{Resolution.Value.Width}x{Resolution.Value.Height}";
            }
        }
    }
}
