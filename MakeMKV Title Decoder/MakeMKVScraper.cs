using JsonSerializable;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace MakeMKV_Title_Decoder {
    internal enum TrackType {
        Video,
        Audio,
        Subtitle,
        Attachment,
        Other
    }

    internal struct Title : IJsonSerializable {
        public string Name;
        public string SourceFileName;
        public TimeSpan Duration;
        public int ChaptersCount = 0;
        public List<int> Segments = new();
        public double Size; // Expressed in GB
        public string FileName;
        public int Index;
        public List<TrackType> Tracks = new();
        public string SourceFileExtension {
            get {
                int dot = SourceFileName.LastIndexOf('.');
                string ext = SourceFileName.Substring(dot + 1);
                if (ext.EndsWith(')')) {
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
                }
                else
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

        public static bool operator ==(Title left, Title right) {
            return left.Name == right.Name
                && left.SourceFileName == right.SourceFileName
                && left.Duration == right.Duration
                && left.ChaptersCount == right.ChaptersCount
                && left.FileName == right.FileName
                && left.Segments.Count == right.Segments.Count
                && left.Segments.SequenceEqual(right.Segments);
        }

        public static bool operator !=(Title left, Title right) {
            return !(left == right);
        }

        public override string ToString() {
            StringBuilder sb = new();
            sb.AppendLine("Title information:");
            if (this.Name != null) sb.AppendLine($"\tName: {this.Name}");
            if (this.SourceFileName != null) sb.AppendLine($"\tSource file name: {this.SourceFileName}");
            sb.AppendLine($"\tDuration: {this.Duration}");
            sb.AppendLine($"\tChapters count: {this.ChaptersCount}");
            sb.AppendLine($"\t Size: {this.Size} GB");
            sb.AppendLine($"\tSegment map: [{string.Join(", ", this.Segments)}]");
            sb.AppendLine($"\tTracks: [{string.Join(", ", this.Tracks.Select(x => x.ToString()))}]");
            sb.Append($"\tFile name: {this.FileName}");
            return sb.ToString();
        }

        public JsonData SaveToJson() {
            JsonObject data = new();
            data["Name"] = new JsonString(this.Name);
            data["Source File Name"] = new JsonString(this.SourceFileName);
            data["Duration"] = new JsonString(Duration.ToString());
            data["Chapters Count"] = new JsonInteger(ChaptersCount);
            data["Segments"] = new JsonArray(this.Segments.Select(x => (JsonData)new JsonInteger(x)).ToList());
            data["Size"] = new JsonDecimal(this.Size);
            data["File Name"] = new JsonString(this.FileName);
            data["Index"] = new JsonInteger(this.Index);
            data["Tracks"] = new JsonArray(this.Tracks.Select(x => (JsonData)new SerializableTrack(x).SaveToJson()).ToList());
            return data;
        }

        public void LoadFromJson(JsonData Data) {
            JsonObject data = (JsonObject)Data;
            this.Name = (JsonString)data["Name"];
            this.SourceFileName = (JsonString)data["Source File Name"];
            this.Duration = TimeSpan.Parse((JsonString)data["Duration"]);
            this.ChaptersCount = (int)(JsonInteger)data["Chapters Count"];
            this.Segments = ((JsonArray)data["Segments"]).Select(x => (int)(JsonInteger)x).ToList();
            this.Size = (JsonDecimal)data["Size"];
            this.FileName = (JsonString)data["File Name"];
            this.Index = (int)(JsonInteger)data["Index"];
            this.Tracks = ((JsonArray)data["Tracks"]).Select(x =>
            {
                var track = new SerializableTrack();
                track.LoadFromJson(x);
                return track.Value;
            }).ToList();
        }

        private struct SerializableTrack : IJsonSerializable {
            public TrackType Value;

            public SerializableTrack() {
                this.Value = TrackType.Other;
            }
            public SerializableTrack(TrackType type) {
                this.Value = type;
            }

            public JsonData SaveToJson() {
                return new JsonString(this.Value.ToString());
            }

            public void LoadFromJson(JsonData Data) {
                JsonString str = (JsonString)Data;
                switch(str.Value)
                {
                    case "Video":
                        this.Value = TrackType.Video;
                        break;
                    case "Audio":
                        this.Value = TrackType.Audio;
                        break;
                    case "Subtitle":
                        this.Value = TrackType.Subtitle;
                        break;
                    case "Attachment":
                        this.Value = TrackType.Attachment;
                        break;
                    case "Other":
                        this.Value = TrackType.Other;
                        break;
                    default:
                        throw new Exception("Unknown track type.");
                }
            }
        }
    }

    internal class MakeMKVScraper : IJsonSerializable {

        const bool verbose = true;
        public List<Title> Titles = new();
        MakeMKVInput input;

        public MakeMKVScraper(MakeMKVInput input) {
            this.input = input;
        }

        public void Scrape(int maxScrapesDebug = -1) {
            Console.WriteLine("Scraping...");

            //input.ResetCursor();
            Title lastTitle = ParseTitle(0);
            while (true)
            {
                if (verbose) Console.WriteLine($"Found title {lastTitle.SimplifiedFileName}");

                // Get track info
                input.OpenDropdown();
                string lastData = null;
                while (true)
                {
                    input.ScrollDown(1);
                    string data = input.CopyTitleInformation();
                    if (data == lastData)
                    {
                        // Found the end of the list while searching tracks
                        //if (verbose) Console.WriteLine($"\tFound end-of-list while searching tracks for {lastTitle.SimplifiedFileName}.");
                        lastData = null;
                        break;
                    } else
                    {
                        lastData = data;
                    }

                    // get track name
                    if (data.StartsWith("Track information"))
                    {
                        using (StringReader sr = new StringReader(data))
                        {
                            string line = sr.ReadLine(); // Track information
                            line = sr.ReadLine();
                            if (line == "Type: Subtitles")
                            {
                                lastTitle.Tracks.Add(TrackType.Subtitle);
                                if (verbose) Console.WriteLine("\tFound track 'Subtitle'.");
                            }
                            else if (line == "Type: Video")
                            {
                                lastTitle.Tracks.Add(TrackType.Video);
                                if (verbose) Console.WriteLine("\tFound track 'Video'.");
                            }
                            else if (line == "Type: Audio")
                            {
                                lastTitle.Tracks.Add(TrackType.Audio);
                                if (verbose) Console.WriteLine("\tFound track 'Audio'.");
                            } else
                            {
                                lastTitle.Tracks.Add(TrackType.Other);
                                if (verbose) Console.WriteLine("\tFound track 'Other'.");
                            }

                        }
                    }
                    else if (data.StartsWith("Attachment information"))
                    {
                        lastTitle.Tracks.Add(TrackType.Attachment);
                        if (verbose) Console.WriteLine("\tFound track 'Attachment'");
                    }
                    else if (data.StartsWith("Title information"))
                    {
                        // Found the next title while searching tracks
                        if (verbose) Console.WriteLine("Found title for next track, finishing up last track now.");
                        break;
                    }
                }

                if (lastData == null)
                {
                    // We found the last title
                    if (verbose) Console.WriteLine("\tFound the end of the list while searching for tracks, finishing up last track now.");
                    Titles.Add(lastTitle);
                    break;
                }

                // Add to master list of titles
                Titles.Add(lastTitle);

                if (Titles.Count == maxScrapesDebug)
                {
                    break;
                }

                // Parse the next title and repeat to parse the tracks
                lastTitle = ParseTitle(Titles.Count, lastData);
            }

            input.SetTitles(Titles, false);
            Console.WriteLine("Finished scraping.");
        }

        private Title ParseTitle(int index, string data = null) {
            if (data == null)
            {
                data = input.CopyTitleInformation();
            }
            Title title = new();
            title.Index = index;
            using (StringReader sr = new StringReader(data))
            {
                string line = sr.ReadLine();
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

                    switch(key)
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
                            string[] bits = value.Split();
                            double n = double.Parse(bits[0]);
                            switch (bits[1])
                            {
                                case "GB":
                                    title.Size = n;
                                    break;
                                case "MB":
                                    title.Size = n / 1000.0;
                                    break;
                                case "KB":
                                    title.Size = n / 1000000.0;
                                    break;
                                default:
                                    throw new Exception("Unknown title size multiplier: '" + bits[1] + "'");
                            }
                            break;
                        case "Segment map":
                            foreach(string range in value.Split(',')) {
                                int dash = range.IndexOf('-');
                                if (dash >= 0)
                                {
                                    int min = int.Parse(range.Substring(0, dash));
                                    int max = int.Parse(range.Substring(dash + 1));
                                    for(int i = min; i <= max; i++)
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
                        case "Segment count":
                        case "Comment":
                        case "Source title ID":
                            // Ignored
                            break;
                        default:
                            throw new Exception("Unknown title information: '" + key + "'");
                    }
                }
            }

            return title;
        }

        public JsonData SaveToJson() {
            return new JsonArray(Titles.Select(x => x.SaveToJson()).ToList());
        }

        public void LoadFromJson(JsonData Data) {
            this.Titles = ((JsonArray)Data).Select(x =>
            {
                Title title = new();
                title.LoadFromJson(x);
                return title;
            }).ToList();
        }

    }
}
