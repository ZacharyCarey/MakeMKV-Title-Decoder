using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder {
    internal class SegmentIdentifier {

        /*private class TitleLengthComparer : IComparer<Title> {
            public int Compare(Title x, Title y) {
                return x.Duration.CompareTo(y.Duration);
            }
        }*/

        public Title MainFeature; // Movies only
        public List<int> MainTitleTracks = new(); // Non-movies only
        public bool IsMovie { get => MainTitleTracks.Count == 0; }
        public HashSet<int> DeselectTitlesIndicies = new();
        public HashSet<int> BonusFeatures = new();

        public SegmentIdentifier(List<Title> allTitles, DvdType type) {
            HashSet<int> titles = new(allTitles.Select(x => x.Index)); // Create a copy so we can edit

            // Main feature will have the longest run time. In case of a tie, pick the playlist file that contains chapter info
            // NOTE: this is still done for the non-movie types to weed-out the play-all playlist and help
            MainFeature = allTitles[0];
            for (int index = 1; index < allTitles.Count; index++)
            {
                int compare = allTitles[index].Duration.CompareTo(MainFeature.Duration);
                if (compare > 0)
                {
                    // This title is longer, store it
                    MainFeature = allTitles[index];
                }
                else if (compare == 0)
                {
                    // This title is the same length, only store if a playlist and the current one is not
                    if (allTitles[index].SourceFileExtension == "mpls" && MainFeature.SourceFileExtension != "mpls")
                    {
                        MainFeature = allTitles[index];
                    }
                }
            }
            titles.Remove(MainFeature.Index);
            Console.WriteLine($"MainFeature = {MainFeature.SimplifiedFileName}");
            Console.WriteLine($"\tSegments = [{string.Join(", ", MainFeature.Segments)}]");
            if (type != DvdType.Movie)
            {
                // TV and bonus features dont actually want the whole thing
                DeselectTitlesIndicies.Add(MainFeature.Index);
            }
            

            // Deselect any titles that have the exact same runtime.
            // Hopefully removes playlists that are both the same main feature, but slightly different
            // For instance, in 'Into the Spider-verse' the second playlist has a slightly different end credits but 99% the same.
            HashSet<int> temp = new(titles);
            foreach(int index in temp)
            {
                if (allTitles[index].Duration == MainFeature.Duration)
                {
                    this.DeselectTitlesIndicies.Add(index);
                    titles.Remove(index);
                    Console.WriteLine($"Found dupicate main feature = {allTitles[index].SimplifiedFileName}");
                }
            }

            if (type != DvdType.Movie)
            {
                // We want to find all the streams contained in the main feature in the smallest possible file (hopefully individual segments)
                // If we cant find all individual segments, we will just return the "play all" like a movie
                Console.WriteLine("Searching for TV episodes...");

                IEnumerable<Title> remainingTitles = titles.Select(index => allTitles[index]);
                HashSet<int> deselections = new();
                List<int>? titleIndex = solveEpisodes(MainFeature, remainingTitles, deselections);
                foreach (int d in deselections)
                {
                    this.DeselectTitlesIndicies.Add(d);
                    titles.Remove(d);
                }
                if (titleIndex != null)
                {
                    Console.WriteLine("Found solution for episode layout.");
                    foreach(int index in titleIndex)
                    {
                        Console.WriteLine($"\t[{string.Join(", ", allTitles[index].Segments)}] @ {allTitles[index].SimplifiedFileName}");
                    }

                    // Adjust lists as needed
                    this.MainTitleTracks = titleIndex;
                    /*foreach(int index in MainTitleTracks)
                    {
                        // No longer
                        titles.Remove(index);
                    }*/

                    // Try and remove any duplicate playlists
                    temp = new(titles);
                    foreach (int index in temp)
                    {
                        if (MainTitleTracks.Contains(index))
                        {
                            // Dont deselect main titles
                            continue;
                        }

                        if (!MainTitleTracks.Contains(index) && areAllSegmentsInList(allTitles[index].Segments, this.MainFeature.Segments))
                        {
                            Console.WriteLine($"Segments [{string.Join(", ", allTitles[index].Segments)}] found in main feature, deselting {allTitles[index].SimplifiedFileName}");
                            DeselectTitlesIndicies.Add(index);
                            titles.Remove(index);
                        }
                    }

                } else {
                    Console.WriteLine("Failed to find enough segments for TV/Bonus features, treating as a movie instead.");

                    type = DvdType.Movie;
                    foreach(int index in this.MainTitleTracks)
                    {
                        DeselectTitlesIndicies.Add(index);
                    }
                    this.MainTitleTracks.Clear();
                    DeselectTitlesIndicies.Remove(MainFeature.Index);
                }
            }

            if (type == DvdType.Movie)
            {
                // After we found the main feature, we can deselect any other streams/chapters that it uses
                // Sometimes the chapters will all be separate streams that all show up in the list
                // The main feature will be all those streams stitched together wither chapter info, so we just
                // can just not download those.
                temp = new(titles);
                foreach (int index in temp)
                {
                    // If all segments are contained within the main feature...
                    if (areAllSegmentsInList(allTitles[index].Segments, MainFeature.Segments))
                    {
                        // Remove title from download list
                        DeselectTitlesIndicies.Add(index);
                        titles.Remove(index);
                    }
                }
            }

            // Now attempt to find other playlists that can be broken down into smaller chunks.
            // For example, there is usually a "play all" button for bonus features, but we want those to be separate
            // This is done for every remaining item to try and get the smallest possible chunk
            bool changed = true;
            while (changed)
            {
                changed = false;

                temp = new(titles);
                foreach (int playlistIndex in temp)
                {
                    if (playlistIndex == MainFeature.Index || MainTitleTracks.Contains(playlistIndex) || DeselectTitlesIndicies.Contains(playlistIndex))
                    {
                        // Skip anything that we already determined to be a part of the main feature
                        continue;
                    }

                    IEnumerable<Title> remainingTitles = titles.Where(index => index != playlistIndex).Select(index => allTitles[index]);
                    HashSet<int> deselections = new(); // usually exact matches only
                    List<int>? subtitlesIndexs = solveEpisodes(allTitles[playlistIndex], remainingTitles, deselections); // Subtitles as in titles that make up the larger main title
                    foreach(int d in deselections)
                    {
                        this.DeselectTitlesIndicies.Add(d);
                        titles.Remove(d);
                    }
                    if (subtitlesIndexs != null)
                    {
                        // We managed to divide this title into smaller bits
                        DeselectTitlesIndicies.Add(playlistIndex);
                        titles.Remove(playlistIndex);
                        BonusFeatures.Remove(playlistIndex); // Incase it was added here by a different playlist
                        changed = true;

                        Console.WriteLine($"Title was subdivided into smaller videos: [{string.Join(", ", allTitles[playlistIndex].Segments)}] @ {allTitles[playlistIndex].SimplifiedFileName}");
                        foreach(int subindex in subtitlesIndexs)
                        {
                            Console.WriteLine($"\t[{string.Join(", ", allTitles[subindex].Segments)}] @ {allTitles[subindex].SimplifiedFileName}");
                            BonusFeatures.Add(subindex); // So we dont accidently remove them later, we do care about them <3
                        }
                    }
                }
            }

            // Double check any playlists/streams that were deselected, and deselect all streams
            // that whose segments are only contained in deselected stream (done recursivley)
            // For instance, in 'Into the spider-verse' the second main feature gets deselected,
            // but it has a unique credits stream that only it uses, so there is no point in
            // downloading that credit stream separately if we didnt want that feature
            //
            // NOTE to be deselected, segment MUST ASLO be contained in a deselected title.
            // This is to prevent removing things like special features
            bool changes = true;
            while (changes)
            {
                temp = new(titles);
                foreach (int index in temp)
                {
                    if (MainTitleTracks.Contains(index) || index == MainFeature.Index || BonusFeatures.Contains(index))
                    {
                        // We want to save these, dont deselect them even if they are dangling
                        continue;
                    }

                    int danglingSegmentCount = 0;
                    foreach (int segment in allTitles[index].Segments)
                    {
                        bool referencedInDeselected = false;
                        foreach (int deselectedIndex in DeselectTitlesIndicies)
                        {
                            if (allTitles[deselectedIndex].Segments.Contains(segment))
                            {
                                referencedInDeselected = true;
                                break;
                            }
                        }

                        bool referencedInKeptTitle = false;
                        foreach (int keptIndex in titles.Where(x => x != index))
                        {
                            if (allTitles[keptIndex].Segments.Contains(segment))
                            {
                                referencedInKeptTitle = true;
                                break;
                            }
                        }

                        if (referencedInDeselected && !referencedInKeptTitle)
                        {
                            danglingSegmentCount++;
                        }
                    }

                    // If ALL segments are dangling i.e. this file was only used in deselected segments
                    if (danglingSegmentCount == allTitles[index].Segments.Count)
                    {
                        Console.WriteLine($"Removed {allTitles[index].SimplifiedFileName} as its segments were only found in deselected titles [{string.Join(", ", allTitles[index].Segments)}]");
                        DeselectTitlesIndicies.Add(index);
                        titles.Remove(index);
                    }
                }

                changes = (temp.Count != titles.Count);
            }
        }

        private static bool areAllSegmentsInList(ICollection<int> segments, ICollection<int> checkInList) {
            return segments.Where(checkInList.Contains).Count() == segments.Count;
        }

        static List<int>? solveEpisodes(Title mainFeature, IEnumerable<Title> titles, HashSet<int> deselections) {
            List<Title> sorted = titles.OrderBy(x => x.Segments.Count).ToList();
            return solveEpisodes(mainFeature, sorted, 0, deselections);
        }

        static List<int>? solveEpisodes(Title mainFeature, List<Title> titles, int index, HashSet<int> deselections) {
            if (index == mainFeature.Segments.Count)
            {
                // Success! Start returning the result
                return new List<int>();
            }

            foreach (Title title in titles)
            {
                if (segmentsMatch(title.Segments, mainFeature, index))
                {
                    // Special case. If they are exactly the same as the main feature, just keep the main feature
                    if (index == 0 && title.Segments.Count == mainFeature.Segments.Count)
                    {
                        Console.WriteLine($"Deselected {title.SimplifiedFileName} because it has the exact same segments as another feature ({mainFeature.SimplifiedFileName}).");
                        deselections.Add(title.Index);
                        continue;
                    }

                    List<int>? result = solveEpisodes(mainFeature, titles, index + title.Segments.Count, deselections);
                    if (result != null)
                    {
                        result.Insert(0, title.Index);
                        return result;
                    }
                }
            }

            return null;
        }

        static bool segmentsMatch(List<int> segments, Title mainFeature, int index) {
            if (index + segments.Count - 1 > mainFeature.Segments.Count)
            {
                return false;
            }

            for (int i = 0; i < segments.Count; i++)
            {
                if (segments[i] != mainFeature.Segments[index + i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
