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
    internal class MakeMKVScraper : IJsonSerializable {

        const bool verbose = true;
        public List<Title> Titles = new();
        MakeMKVInput input;

        public MakeMKVScraper(MakeMKVInput input) {
            this.input = input;
        }

        public void Scrape(int maxScrapes = -1, int finalTitleTrackCount = -1) {
            Console.WriteLine("Scraping...");

            input.Reset();
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
                    if ((maxScrapes < 0 || finalTitleTrackCount < 0) && (data == lastData))
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
                    if (data.StartsWith("Title information"))
                    {
                        // Found the next title while searching tracks
                        if (verbose) Console.WriteLine("Found title for next track, finishing up last track now.");
                        break;
                    } else
                    {
                        Track track = Track.Parse(data);
                        lastTitle.Tracks.Add(track);
                    }

                    if (Titles.Count + 1 == maxScrapes && lastTitle.Tracks.Count == finalTitleTrackCount)
                    {
                        break;
                    }
                }

                if (lastData == null)
                {
                    // We found the last title
                    if (verbose) Console.WriteLine("\tFound the end of the list while searching for tracks, finishing up last track now.");
                    Titles.Add(lastTitle);
                    Console.WriteLine($"Found title: {lastTitle}");
                    break;
                }

                // Add to master list of titles
                Titles.Add(lastTitle);
                Console.WriteLine($"Found title: {lastTitle}");

                if (Titles.Count == maxScrapes)
                {
                    break;
                }

                // Parse the next title and repeat to parse the tracks
                lastTitle = ParseTitle(Titles.Count, lastData);
            }

            input.SetTitles(Titles, false);
            Console.WriteLine("Finished scraping.");
        }

        private Title ParseTitle(int index, string? data = null) {
            if (data == null)
            {
                data = input.CopyTitleInformation();
            }
            return Title.Parse(data);
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
