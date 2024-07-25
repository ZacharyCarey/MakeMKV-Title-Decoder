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

    internal struct Title {
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
    }

    internal class MakeMKVScraper {

        const bool verbose = true;
        public List<Title> Titles = new();
        MakeMKVInput input;

        public MakeMKVScraper(MakeMKVInput input) {
            this.input = input;
        }

        public void Scrape(int maxScrapesDebug = -1) {
            Console.WriteLine("Scraping...");

            input.ResetCursor();
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
                    input.ScrollUp(lastTitle.Tracks.Count - 1); // Note: the last "scroll down" didnt move so this accounts for that
                    input.CloseDropdown();
                    Titles.Add(lastTitle);
                    break;
                }

                // Close the current dropdown
                input.ScrollTo(lastTitle.Index); //input.ScrollUp(lastTitle.Tracks.Count + 1); // Plus one since we should be on the next title now
                input.CloseDropdown();
                Titles.Add(lastTitle);

                // Assert for expected index
                if (input.CurrentIndex != lastTitle.Index)
                {
                    throw new Exception("Assert failed while scraping: expected title index did not match.");
                }

                if (input.CurrentIndex == maxScrapesDebug)
                {
                    break;
                }

                // Move to the next title
                input.ScrollDown(1);

                // Double check the index is correct
                if (input.CurrentIndex != lastTitle.Index + 1)
                {
                    throw new Exception("Assert failed while scraping: expected title index does not follow the previous title index.");
                }

                // Parse the title and repeat to parse the tracks
                lastTitle = ParseTitle(Titles.Count, lastData);
            }

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

    }
}
