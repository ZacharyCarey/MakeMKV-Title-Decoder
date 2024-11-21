using MakeMKV_Title_Decoder.libs.MakeMKV;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Utils;

namespace MakeMKV_Title_Decoder.libs.MakeMKV.Data
{
    public class Title : IEquatable<Title>
    {
        [JsonInclude]
        public string? Name = null;

        [JsonInclude, JsonPropertyName("Source File Name")]
        public string? SourceFileName = null;

        [JsonInclude]
        public TimeSpan? Duration = null;

        [JsonInclude, JsonPropertyName("Chapters Count")]
        public int? ChaptersCount = null;

        [JsonInclude]
        public List<int>? Segments = new();

        [JsonInclude]
        public DataSize? Size = null;

        [JsonInclude, JsonPropertyName("File Name")]
        public string? OutputFileName = null;

        [JsonInclude]
        public string? Comment = null;

        // Hold any extra data that isn't explicitly stored in it's own variable
        [JsonInclude]
        public Dictionary<string, string> Data = new();

        [JsonInclude]
        public List<Track> Tracks = new();

        // Used by FileRenamer, not an actual data point to be saved
        //public Folder? Folder = null;
        public string? UserName = null;

        public string SourceFileExtension
        {
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
                }
                else
                {
                    return ext;
                }
            }
        }
        public int SourceFileDuplicateNumber
        {
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
                }
                else
                {
                    return 0;
                }
            }
        }
        public string SimplifiedFileName
        {
            get
            {
                if (OutputFileName == null) return "null";

                int underscore = OutputFileName.LastIndexOf('_');
                return OutputFileName.Substring(underscore + 1);
            }
        }

        public Title() { }

        public bool Equals(Title other)
        {
            return this == other;
        }

        /*public static bool operator ==(Title left, Title right) {
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
        }*/

        public override string ToString()
        {
            return ToString(0);
        }

        public string ToString(int tabs)
        {
            StringBuilder sb = new();
            sb.Append('\t', tabs);
            sb.AppendLine($"Title '{SimplifiedFileName}': {{");

            // TODO
            /*
            sb.Append(tabs + 1, "Name: ", Name);
            sb.Append(tabs + 1, "Source file name: ", SourceFileName);
            sb.Append(tabs + 1, "Duration: ", Duration);
            sb.Append(tabs + 1, "Chapters count: ", ChaptersCount);
            sb.Append(tabs + 1, "Segments: ", Segments);
            sb.Append(tabs + 1, "Size: ", Size);
            sb.Append(tabs + 1, "File name: ", OutputFileName);
            sb.Append(tabs + 1, "Comment: ", Comment);*/

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

            sb.Append('\t', tabs + 1);
            sb.Append("Tracks: ");
            if (Tracks.Count == 0)
            {
                sb.AppendLine("[]");
            }
            else
            {
                sb.AppendLine();
                sb.Append('\t', tabs + 1);
                sb.AppendLine("[");

                sb.AppendLine(string.Join(",\r\n", Tracks.Select(x => x.ToString(tabs + 2))));

                sb.Append('\t', tabs + 1);
                sb.AppendLine("]");
            }

            return sb.ToString();
        }

        /// <exception cref="FormatException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="OverflowException"></exception>
        /// <exception cref="InvalidCastException"></exception>
        public static void ParseMakeMkv(Disc disc, MakeMkvMessage info)
        {
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
        private void ParseMakeMkv(ApItemAttributeId id, int code, string value)
        {
            switch (id)
            {
                case ApItemAttributeId.Name:
                    Name = value;
                    break;
                case ApItemAttributeId.ChapterCount:
                    ChaptersCount = int.Parse(value);
                    break;
                case ApItemAttributeId.Duration:
                    Duration = TimeSpan.Parse(value);
                    break;
                case ApItemAttributeId.DiskSizeBytes:
                    Size = new DataSize(long.Parse(value), Unit.None);
                    break;
                case ApItemAttributeId.SourceFileName:
                    SourceFileName = value;
                    break;
                case ApItemAttributeId.SegmentsMap:
                    Segments = ParseSegments(value).ToList();
                    break;
                case ApItemAttributeId.OutputFileName:
                    OutputFileName = value;
                    break;
                case ApItemAttributeId.Comment:
                    Comment = value;
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
                    Data[id.ToString()] = new(value);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"WARNING: Unknown title attribute: {id.ToString()}={value}");
                    Console.ResetColor();
                    break;
            }
        }

        public override int GetHashCode()
        {
            return (SourceFileName ?? "").GetHashCode();
        }

        private IEnumerable<int> ParseSegments(string value)
        {
            foreach (string str in value.Split(','))
            {
                int rangeIndex = str.IndexOf('-');
                if (rangeIndex < 0)
                {
                    yield return int.Parse(str);
                }
                else
                {
                    int min = int.Parse(str.Substring(0, rangeIndex));
                    int max = int.Parse(str.Substring(rangeIndex + 1));
                    for (int i = min; i <= max; i++)
                    {
                        yield return i;
                    }
                }
            }
        }

        public IEnumerable<string> GetData()
        {
            if (Name != null) yield return $"Name: {Name}";
            if (SourceFileName != null) yield return $"Source File Name: {SourceFileName}";
            if (Duration != null) yield return $"Duration: {Duration.Value.ToString("hh':'mm':'ss")}";
            if (ChaptersCount != null) yield return $"Chapters Count: {ChaptersCount}";
            if (Segments != null) yield return $"Segments: [{string.Join(", ", Segments)}]";
            if (Size != null) yield return $"Size: {Size.ToString()}";
            if (OutputFileName != null) yield return $"Output File Name: {OutputFileName}";
            if (Comment != null) yield return $"Comment: {Comment}";
            foreach (var pair in Data)
            {
                yield return $"{pair.Key}: {pair.Value}";
            }
        }
    }
}
