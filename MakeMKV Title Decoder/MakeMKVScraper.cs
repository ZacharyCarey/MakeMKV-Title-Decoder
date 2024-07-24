using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace MakeMKV_Title_Decoder {
    internal struct Title {
        public string Name;
        public string SourceFileName;
        public TimeSpan Duration;
        public int ChaptersCount = 0;
        public List<int> Segments = new();
        public double Size; // Expressed in GB
        public string FileName;
        public int Index;
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

        public List<Title> Titles = new();
        MakeMKVInput input;

        public MakeMKVScraper(MakeMKVInput input) {
            this.input = input;
        }

        public void Scrape(int maxScrapesDebug = -1) {
            Console.WriteLine("Scraping...");

            input.ResetCursor();
            bool checkNext = true;
            for (int i = 0; checkNext; i++)
            {
                if (i == maxScrapesDebug)
                {
                    break;
                }

                // Get the title info
                Title title = ParseTitle(i);

                // this is our first entry so dont try to compare for end of list yet
                if (i == 0)
                {
                    Titles.Add(title);
                    input.ScrollDown(1);
                    continue;
                }

                // Compare to see if this is the end of the list
                if (title != Titles.Last())
                {
                    // Valid title
                    Titles.Add(title);
                    checkNext = true;
                } else
                {
                    checkNext = false;
                }

                input.ScrollDown(1);
            }

            // TODO tell input what the max title length is?
        }

        private Title ParseTitle(int index) {
            //input.SelectTitle(index);
            string data = input.CopyTitleInformation();
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
