using JsonSerializable;
using MakeMKV_Title_Decoder.MakeMKV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.Data {

    public enum DiscType {
        DVD,
        HD_DVD,
        BluRay
    }

    public class Disc : IJsonSerializable, IEquatable<Disc> {

        public DiscType? Type;
        public string? Name;
        public string? Language;
        public string? VolumeName;
        public string? Comment;

        // Hold any extra data that isn't explicitly stored in it's own variable
        public SerializableDictionary<JsonString> Data = new();

        public SerializableList<Title> Titles = new();

        public Disc() { }

        public bool Equals(Disc other) {
            return this == other;
        }

        public static bool operator ==(Disc? left, Disc? right) {
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

        public override string ToString() {
            return this.ToString(0);
        }

        public string ToString(int tabs) {
            StringBuilder sb = new();
            sb.Append('\t', tabs);
            sb.AppendLine($"Disc '{this.Name}': {{");

            sb.Append(tabs + 1, "Type: ", this.Type);
            sb.Append(tabs + 1, "Name: ", this.Name);
            sb.Append(tabs + 1, "Language: ", this.Language);
            sb.Append(tabs + 1, "Volume name: ", this.VolumeName);
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

            sb.Append('\t', tabs + 1);
            sb.Append("Titles: ");
            if (this.Titles.Count == 0)
            {
                sb.AppendLine("[]");
            } else
            {
                sb.AppendLine();
                sb.Append('\t', tabs + 1);
                sb.AppendLine("[");

                sb.AppendLine(string.Join(",\r\n", this.Titles.Select(x => x.ToString(tabs + 2))));

                sb.Append('\t', tabs + 1);
                sb.AppendLine("]");
            }

            sb.Append('\t', tabs);
            sb.Append('}');

            return sb.ToString();
        }

        public JsonData SaveToJson() {
            JsonObject obj = new();

            obj.SaveToJson("Type", this.Type);
            obj.SaveToJson("Name", this.Name);
            obj.SaveToJson("Language", this.Language);
            obj.SaveToJson("Volume name", this.VolumeName);
            obj.SaveToJson("Comment", this.Comment);

            obj["Titles"] = this.Titles.SaveToJson();
            obj["Data"] = this.Data.SaveToJson();
            return obj;
        }

        public void LoadFromJson(JsonData data) {
            JsonObject obj = (JsonObject)data;

            obj["Type"].LoadFromJson(out this.Type, ParseDiscTypeFromJson);
            obj["Name"].LoadFromJson(out this.Name);
            obj["Language"].LoadFromJson(out this.Language);
            obj["Volume name"].LoadFromJson(out this.VolumeName);
            obj["Comment"].LoadFromJson(out this.Comment);

            this.Titles.LoadFromJson(obj["Titles"]);
            this.Data.LoadFromJson(obj["Data"]);
        }


        /// <exception cref="FormatException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="OverflowException"></exception>
        /// <exception cref="InvalidCastException"></exception>
        public void ParseMakeMkv(MakeMkvMessage info) {
            if (info.Type != MakeMkvMessageType.DiscInfo || info.Arguments.Count != 3)
            {
                throw new FormatException("Incorrect message type");
            }

            ApItemAttributeId id = (ApItemAttributeId)(long)info[0];
            int code = (int)(long)info[1];
            string value = (string)info[2];

            switch(id)
            {
                case ApItemAttributeId.Type:
                    this.Type = ParseDiscTypeFromMakeMkv(value);
                    break;
                case ApItemAttributeId.Name:
                    this.Name = value;
                    break;
                case ApItemAttributeId.MetadataLanguageName:
                    this.Language = value;
                    break;
                case ApItemAttributeId.VolumeName:
                    this.VolumeName = value;
                    break;
                case ApItemAttributeId.Comment:
                    this.Comment = value;
                    break;
                case ApItemAttributeId.MetadataLanguageCode:
                case ApItemAttributeId.TreeInfo:
                case ApItemAttributeId.PanelTitle:
                case ApItemAttributeId.OrderWeight:
                    // Ignored
                    break;
                default:
                    this.Data[id.ToString()] = new(value);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"WARNING: Unknown disc attribute: {id.ToString()}={value}");
                    Console.ResetColor();
                    break;
            }
        }

        /// <exception cref="FormatException"></exception>
        private static DiscType ParseDiscTypeFromJson(string value) {
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
        private static DiscType ParseDiscTypeFromMakeMkv(string value) {
            switch (value)
            {
                case "Blu-ray disc":
                    return DiscType.BluRay;
                case nameof(DiscType.DVD):
                case nameof(DiscType.HD_DVD):
                default:
                    throw new FormatException("Unknown disc type: " + value);
            }
        }
    }
}
