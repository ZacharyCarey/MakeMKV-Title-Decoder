using JsonSerializable;
using MakeMKV_Title_Decoder.MakeMKV;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.Data {
    public class Title : IJsonSerializable, IEquatable<Title> {
        public string? Name = null;
        public string? SourceFileName = null;
        public TimeSpan? Duration = null;
        public int? ChaptersCount = null;
        public List<int>? Segments = new();
        public DataSize? Size = null; 
        public string? OutputFileName = null;
        public string? Comment = null;

        // Hold any extra data that isn't explicitly stored in it's own variable
        public SerializableDictionary<JsonString> Data = new();

        public SerializableList<Track> Tracks = new();

        public string SourceFileExtension {
            get
            {
                if (SourceFileName == null)
                {
                    return null;
                }

                int dot = SourceFileName.LastIndexOf('.');
                string ext = SourceFileName.Substring(dot + 1);
                if (ext.EndsWith(')'))
                {
                    int paren = ext.LastIndexOf('(');
                    return ext.Substring(0, paren);
                } else
                {
                    return ext;
                }
            }
        }
        public int SourceFileDuplicateNumber {
            get
            {
                if (SourceFileName == null)
                {
                    return 0;
                }

                int dot = SourceFileName.LastIndexOf('.');
                string ext = SourceFileName.Substring(dot + 1);
                if (ext.EndsWith(')'))
                {
                    int paren = ext.LastIndexOf('(');
                    return int.Parse(ext.Substring(paren + 1, ext.Length - paren - 2));
                } else
                {
                    return 0;
                }
            }
        }
        public string SimplifiedFileName {
            get
            {
                if (this.OutputFileName == null) return "null";

                int underscore = this.OutputFileName.LastIndexOf('_');
                return this.OutputFileName.Substring(underscore + 1);
            }
        }

        public Title() { }

        public bool Equals(Title other) {
            return this == other;
        }

        public static bool operator ==(Title left, Title right) {
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

            bool result = left.Name == right.Name
                && left.SourceFileName == right.SourceFileName
                && left.Duration == right.Duration
                && left.ChaptersCount == right.ChaptersCount
                && left.OutputFileName == right.OutputFileName
                && ((left.Segments == null && right.Segments == null) || ((left.Segments != null && right.Segments != null) && (left.Segments.Count == right.Segments.Count && left.Segments.SequenceEqual(right.Segments))))
                && ((left.Size == null && right.Size == null) || ((left.Size != null && right.Size != null) && left.Size.Value.Near(right.Size.Value)))
                && left.Tracks.Count == right.Tracks.Count
                && left.Tracks.SequenceEqual(right.Tracks)
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

        public static bool operator !=(Title left, Title right) {
            return !(left == right);
        }

        public override string ToString() {
            return ToString(0);
        }

        public string ToString(int tabs) {
            StringBuilder sb = new();
            sb.Append('\t', tabs);
            sb.AppendLine($"Title '{this.SimplifiedFileName}': {{");

            sb.Append(tabs + 1, "Name: ", this.Name);
            sb.Append(tabs + 1, "Source file name: ", this.SourceFileName);
            sb.Append(tabs + 1, "Duration: ", this.Duration);
            sb.Append(tabs + 1, "Chapters count: ", this.ChaptersCount);
            sb.Append(tabs + 1, "Segments: ", this.Segments);
            sb.Append(tabs + 1, "Size: ", this.Size);
            sb.Append(tabs + 1, "File name: ", this.OutputFileName);
            sb.Append(tabs + 1, "Comment: ", this.Comment);

            sb.Append('\t', tabs + 1);
            sb.Append("Data: ");
            if (this.Data.Count == 0)
            {
                sb.AppendLine("{}");
            } else
            {
                sb.AppendLine();
                sb.Append('\t', tabs + 1);
                sb.AppendLine("{");

                sb.AppendLine(string.Join(",\r\n", this.Data.Select(x => $"{new string('\t', tabs + 2)}{x.Key}={x.Value}")));

                sb.Append('\t', tabs + 1);
                sb.AppendLine("}");
            }

            sb.Append('\t', tabs);
            sb.Append('}');

            sb.Append('\t', tabs + 1);
            sb.Append("Tracks: ");
            if (this.Tracks.Count == 0)
            {
                sb.AppendLine("[]");
            } else
            {
                sb.AppendLine();
                sb.Append('\t', tabs + 1);
                sb.AppendLine("[");

                sb.AppendLine(string.Join(",\r\n", this.Tracks.Select(x => x.ToString(tabs + 2))));

                sb.Append('\t', tabs + 1);
                sb.AppendLine("]");
            }

            return sb.ToString();
        }

        public JsonData SaveToJson() {
            JsonObject data = new();

            data.SaveToJson("Name", this.Name);
            data.SaveToJson("Source File Name", this.SourceFileName);
            data.SaveToJson("Duration", this.Duration);
            data.SaveToJson("Chapters Count", this.ChaptersCount);
            data.SaveToJson("Segments", this.Segments?.Select(x => new JsonInteger(x)));
            data.SaveToJson("Size", (IJsonSerializable?)this.Size);
            data.SaveToJson("File Name", this.OutputFileName);
            data.SaveToJson("Comment", this.Comment);
            
            data["Tracks"] = this.Tracks.SaveToJson();
            data["Data"] = this.Data.SaveToJson();

            return data;
        }

        public void LoadFromJson(JsonData Data) {
            JsonObject obj = (JsonObject)Data;

            obj["Name"].LoadFromJson(out this.Name);
            obj["Source File Name"].LoadFromJson(out this.SourceFileName);
            obj["Duration"].LoadFromJson(out this.Duration);
            obj["Chapters Count"].LoadFromJson(out this.ChaptersCount);
            obj["Segments"].LoadFromJson(out this.Segments, (JsonData data) => (int)(JsonInteger)data);
            obj["Size"].LoadSerializableFromJson(out this.Size);
            obj["File Name"].LoadFromJson(out this.OutputFileName);
            obj["Comment"].LoadFromJson(out this.Comment);

            this.Tracks.LoadFromJson(obj["Tracks"]);
            this.Data.LoadFromJson(obj["Data"]);
        }

        /// <exception cref="FormatException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="OverflowException"></exception>
        /// <exception cref="InvalidCastException"></exception>
        public static void ParseMakeMkv(Disc disc, MakeMkvMessage info) {
            if (info.Type != MakeMkvMessageType.TitleInfo || info.Arguments.Count != 4)
            {
                throw new FormatException("Incorrect message type.");
            }

            int index = (int)(long)info[0];
            ApItemAttributeId id = (ApItemAttributeId)(long)info[1];
            int code = (int)(long)info[2];
            string value = (string)info[3];

            if (disc.Titles.Count <= index)
            {
                disc.Titles.Add(new());
            }
            disc.Titles[index].ParseMakeMkv(id, code, value);
        }

        /// <exception cref="FormatException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="OverflowException"></exception>
        /// <exception cref="InvalidCastException"></exception>
        private void ParseMakeMkv(ApItemAttributeId id, int code, string value) {
            switch(id)
            {
                case ApItemAttributeId.Name:
                    this.Name = value;
                    break;
                case ApItemAttributeId.ChapterCount:
                    this.ChaptersCount = int.Parse(value);
                    break;
                case ApItemAttributeId.Duration:
                    this.Duration = TimeSpan.Parse(value);
                    break;
                case ApItemAttributeId.DiskSizeBytes:
                    this.Size = new DataSize(long.Parse(value), Unit.None);
                    break;
                case ApItemAttributeId.SourceFileName:
                    this.SourceFileName = value;
                    break;
                case ApItemAttributeId.SegmentsMap:
                    this.Segments = value.Split(',').Select(int.Parse).ToList();
                    break;
                case ApItemAttributeId.OutputFileName:
                    this.OutputFileName = value;
                    break;
                case ApItemAttributeId.Comment:
                    this.Comment = value;
                    break;
                case ApItemAttributeId.DiskSize: // String version of DiskSizeBytes
                case ApItemAttributeId.SegmentsCount: // We just add them dynamicly
                case ApItemAttributeId.MetadataLanguageCode:
                case ApItemAttributeId.MetadataLanguageName:
                case ApItemAttributeId.TreeInfo: // Summarized data - as seen in MakeMKV
                case ApItemAttributeId.PanelTitle: // MakeMKV  GUI stuff
                case ApItemAttributeId.OrderWeight: // MakeMKV GUI stuff
                    // Ignored;
                    break;
                default:
                    this.Data[id.ToString()] = new(value);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"WARNING: Unknown title attribute: {id.ToString()}={value}");
                    Console.ResetColor();
                    break;
            }
        }
    }
}
