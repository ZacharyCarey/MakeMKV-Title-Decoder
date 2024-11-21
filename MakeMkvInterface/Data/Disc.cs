using MakeMKV_Title_Decoder.libs.MakeMKV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Utils;

namespace MakeMKV_Title_Decoder.libs.MakeMKV.Data
{

    public enum DiscType
    {
        DVD,
        HD_DVD,
        BluRay
    }

    public class Disc : IEquatable<Disc>
    {
        [JsonInclude]
        public DiscType? Type;

        [JsonInclude]
        public string? Name;

        [JsonInclude]
        public string? Language;

        [JsonInclude, JsonPropertyName("Volume name")]
        public string? VolumeName;

        [JsonInclude]
        public string? Comment;

        // Hold any extra data that isn't explicitly stored in it's own variable
        [JsonInclude]
        public Dictionary<string, string> Data = new();

        [JsonInclude]
        public List<Title> Titles = new();

        public Disc() { }

        public bool Equals(Disc other)
        {
            return this == other;
        }

        /*public static bool operator ==(Disc? left, Disc? right) {
            if(left is null)
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

            bool result = left.Type == right.Type
                && left.Name == right.Name
                && left.Language == right.Language
                && left.VolumeName == right.VolumeName
                && left.Data.Count == right.Data.Count
                && left.Comment == right.Comment;
            if (!result) return false;
            foreach (var pair in left.Data)
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

        public static bool operator !=(Disc? left, Disc? right) => !(left == right);
        */
        public override string ToString()
        {
            return ToString(0);
        }

        public string ToString(int tabs)
        {
            StringBuilder sb = new();
            sb.Append('\t', tabs);
            sb.AppendLine($"Disc '{Name}': {{");

            // TODO
            /*sb.Append(tabs + 1, "Type: ", Type);
            sb.Append(tabs + 1, "Name: ", Name);
            sb.Append(tabs + 1, "Language: ", Language);
            sb.Append(tabs + 1, "Volume name: ", VolumeName);
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

            sb.Append('\t', tabs + 1);
            sb.Append("Titles: ");
            if (Titles.Count == 0)
            {
                sb.AppendLine("[]");
            }
            else
            {
                sb.AppendLine();
                sb.Append('\t', tabs + 1);
                sb.AppendLine("[");

                sb.AppendLine(string.Join(",\r\n", Titles.Select(x => x.ToString(tabs + 2))));

                sb.Append('\t', tabs + 1);
                sb.AppendLine("]");
            }

            sb.Append('\t', tabs);
            sb.Append('}');

            return sb.ToString();
        }

        /// <exception cref="FormatException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="OverflowException"></exception>
        /// <exception cref="InvalidCastException"></exception>
        public void ParseMakeMkv(MakeMkvMessage info)
        {
            if (info.Type != MakeMkvMessageType.DiscInfo || info.Arguments.Count != 3)
            {
                throw new FormatException("Incorrect message type");
            }

            ApItemAttributeId id = (ApItemAttributeId)(long)info[0];
            int code = (int)(long)info[1];
            string value = (string)info[2];

            switch (id)
            {
                case ApItemAttributeId.Type:
                    Type = ParseDiscTypeFromMakeMkv(value);
                    break;
                case ApItemAttributeId.Name:
                    Name = value;
                    break;
                case ApItemAttributeId.MetadataLanguageName:
                    Language = value;
                    break;
                case ApItemAttributeId.VolumeName:
                    VolumeName = value;
                    break;
                case ApItemAttributeId.Comment:
                    Comment = value;
                    break;
                case ApItemAttributeId.MetadataLanguageCode:
                case ApItemAttributeId.TreeInfo:
                case ApItemAttributeId.PanelTitle:
                case ApItemAttributeId.OrderWeight:
                    // Ignored
                    break;
                default:
                    Data[id.ToString()] = new(value);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"WARNING: Unknown disc attribute: {id.ToString()}={value}");
                    Console.ResetColor();
                    break;
            }
        }

        /// <exception cref="FormatException"></exception>
        private static DiscType ParseDiscTypeFromJson(string value)
        {
            switch (value)
            {
                case nameof(DiscType.DVD):
                    return DiscType.DVD;
                case nameof(DiscType.HD_DVD):
                    return DiscType.HD_DVD;
                case nameof(DiscType.BluRay):
                    return DiscType.BluRay;
                default:
                    throw new FormatException("Unknown disc type: " + value);
            }
        }

        /// <exception cref="FormatException"></exception>
        private static DiscType ParseDiscTypeFromMakeMkv(string value)
        {
            switch (value)
            {
                case "Blu-ray disc":
                    return DiscType.BluRay;
                case "DVD disc":
                    return DiscType.DVD;
                case nameof(DiscType.HD_DVD):
                default:
                    throw new FormatException("Unknown disc type: " + value);
            }
        }
    }
}
