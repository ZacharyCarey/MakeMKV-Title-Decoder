using JsonSerializable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder {
    public struct Title : IJsonSerializable, IEquatable<Title> {
        public string Name;
        public string? SourceFileName;
        public TimeSpan Duration;
        public int ChaptersCount = 0;
        public List<int> Segments = new();
        public DataSize Size; 
        public string FileName;
        public string? Comment = null;
        public int? SourceTitleID = null;
        public int? Angle = null;

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
                int underscore = this.FileName.LastIndexOf('_');
                return this.FileName.Substring(underscore + 1);
            }
        }

        public Title() { }

        public bool Equals(Title other) {
            return this == other;
        }

        public static bool operator ==(Title left, Title right) {
            return left.Name == right.Name
                && left.SourceFileName == right.SourceFileName
                && left.Duration == right.Duration
                && left.ChaptersCount == right.ChaptersCount
                && left.FileName == right.FileName
                && left.Segments.Count == right.Segments.Count
                && left.Segments.SequenceEqual(right.Segments)
                && left.Size.Near(right.Size)
                && left.Tracks.Count == right.Tracks.Count
                && left.Tracks.SequenceEqual(right.Tracks)
                && left.SourceTitleID == right.SourceTitleID
                && left.Angle == right.Angle;
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
            sb.AppendLine($"Title'{this.SimplifiedFileName}': {{");

            sb.Append('\t', tabs + 1);
            sb.AppendLine($"Source File Name: {this.Name}");

            sb.Append('\t', tabs + 1);
            sb.AppendLine($"Duration: {this.Duration}");

            sb.Append('\t', tabs + 1);
            sb.AppendLine($"Chapters Count: {this.ChaptersCount}");

            sb.Append('\t', tabs + 1);
            sb.Append("Segments: [");
            sb.Append(string.Join(", ", this.Segments));
            sb.AppendLine("]");

            sb.Append('\t', tabs + 1);
            sb.AppendLine($"Size: {this.Size}");

            sb.Append('\t', tabs + 1);
            sb.AppendLine($"File Name: {this.FileName}");

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

            if (Comment != null)
            {
                sb.Append('\t', tabs);
                sb.AppendLine($"Comment: {Comment}");
            }

            if (SourceTitleID.HasValue)
            {
                sb.Append('\t', tabs);
                sb.AppendLine($"Source title ID: {this.SourceTitleID.Value}");
            }

            if (Angle.HasValue)
            {
                sb.Append('\t', tabs);
                sb.AppendLine($"Angle: {this.Angle}");
            }


            sb.Append('\t', tabs);
            sb.Append('}');

            return sb.ToString();
        }

        public JsonData SaveToJson() {
            JsonObject data = new();
            data["Name"] = new JsonString(this.Name);
            data["Source File Name"] = new JsonString(this.SourceFileName);
            data["Duration"] = new JsonString(Duration.ToString());
            data["Chapters Count"] = new JsonInteger(ChaptersCount);
            data["Segments"] = new JsonArray(this.Segments.Select(x => (JsonData)new JsonInteger(x)));
            data["Size"] = this.Size.SaveToJson();
            data["File Name"] = new JsonString(this.FileName);
            data["Tracks"] = this.Tracks.SaveToJson();
            if (this.Comment != null) data["Comment"] = new JsonString(this.Comment);
            if (this.SourceTitleID.HasValue) data["Source Title ID"] = new JsonInteger(this.SourceTitleID.Value);
            if (this.Angle.HasValue) data["Angle"] = new JsonInteger(this.Angle.Value);
            return data;
        }

        public void LoadFromJson(JsonData Data) {
            JsonObject data = (JsonObject)Data;
            this.Name = (JsonString)data["Name"];
            this.SourceFileName = (JsonString)data["Source File Name"];
            this.Duration = TimeSpan.Parse((JsonString)data["Duration"]);
            this.ChaptersCount = (int)(JsonInteger)data["Chapters Count"];
            this.Segments = ((JsonArray)data["Segments"]).Select(x => (int)(JsonInteger)x).ToList();
            this.Size.LoadFromJson(data["Size"]);
            this.FileName = (JsonString)data["File Name"];
            this.Tracks.LoadFromJson(data["Tracks"]);

            JsonData commentData = data["Comment"];
            if (commentData == null) this.Comment = null;
            else this.Comment = (JsonString)commentData;

            JsonData sourceTitleIdData = data["Source Title ID"];
            if (sourceTitleIdData == null) this.SourceTitleID = null;
            else this.SourceTitleID = (int)(JsonInteger)sourceTitleIdData;

            JsonData angleData = data["Angle"];
            if (angleData == null) this.Angle = null;
            else this.Angle = (int)(JsonInteger)angleData;
        }

        public static Title Parse(string data) {
            Title title = new();
            using (StringReader sr = new StringReader(data))
            {
                string? line = sr.ReadLine();
                if (line == null)
                {
                    throw new Exception("Faield to read title information.");
                }
                if (line != "Title information")
                {
                    throw new Exception("Failed to read title information.");
                }

                while ((line = sr.ReadLine()) != null)
                {
                    int colon = line.IndexOf(':');
                    string key = line.Substring(0, colon);
                    string value = line.Substring(colon + 2); // removes empty space

                    switch (key)
                    {
                        case "Name":
                            title.Name = value;
                            break;
                        case "Source file name":
                            title.SourceFileName = value;
                            break;
                        case "Duration":
                            title.Duration = TimeSpan.Parse(value);
                            break;
                        case "Chapters count":
                            title.ChaptersCount = int.Parse(value);
                            break;
                        case "Size":
                            title.Size = DataSize.Parse(value);
                            break;
                        case "Segment map":
                            foreach (string range in value.Split(','))
                            {
                                int dash = range.IndexOf('-');
                                if (dash >= 0)
                                {
                                    int min = int.Parse(range.Substring(0, dash));
                                    int max = int.Parse(range.Substring(dash + 1));
                                    for (int i = min; i <= max; i++)
                                    {
                                        title.Segments.Add(i);
                                    }
                                } else
                                {
                                    title.Segments.Add(int.Parse(range));
                                }
                            }
                            break;
                        case "File name":
                            title.FileName = value;
                            break;
                        case "Comment":
                            title.Comment = value;
                            break;
                        case "Source title ID":
                            title.SourceTitleID = int.Parse(value);
                            break;
                        case "Angle":
                            title.Angle = int.Parse(value);
                            break;
                        case "Segment count":
                            // Ignored
                            break;
                        default:
                            throw new Exception("Unknown title information: '" + key + "'");
                    }
                }
            }

            return title;
        }
    }
}
