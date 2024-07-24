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
        //public bool HasMainFeature = false;
        public List<int> MainTitleTracks = new(); // Non-movies only
        public bool IsMovie { get => MainTitleTracks.Count == 0; }
        public HashSet<int> DeselectTitlesIndicies = new();

        public SegmentIdentifier(List<Title> allTitles, DvdType type) {
            HashSet<int> titles = new(allTitles.Select(x => x.Index)); // Create a copy so we can edit

            if (type == DvdType.Movie)
            {
                //HasMainFeature = true;
            }

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
            Console.WriteLine($"MainFeature = {MainFeature.FileName}");
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
                    Console.WriteLine($"Found dupicate main feature = {allTitles[index].FileName}");
                }
            }

            if (type != DvdType.Movie)
            {
                // We want to find all the streams contained in the main feature in the smallest possible file (hopefully individual segments)
                // If we cant find all individual segments, we will just return the "play all" like a movie
                Console.WriteLine("Searching for TV episodes...");

                IEnumerable<Title> remainingTitles = titles.Select(index => allTitles[index]);
                List<int>? titleIndex = solveEpisodes(MainFeature, remainingTitles);
                if (titleIndex != null)
                {
                    Console.WriteLine("Found solution for episode layout.");
                    foreach(int index in titleIndex)
                    {
                        Console.WriteLine($"\t[{string.Join(", ", allTitles[index].Segments)}] @ {allTitles[index].FileName}");
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
                            Console.WriteLine($"Segments [{string.Join(", ", allTitles[index].Segments)}] found in main feature, deselting {allTitles[index].FileName}");
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
                            DeselectTitlesIndicies.Add(index);
                            titles.Remove(index);
                        }
                    }

                    changes = (temp.Count != titles.Count);
                }
            }
        }

        private static bool areAllSegmentsInList(ICollection<int> segments, ICollection<int> checkInList) {
            return segments.Where(checkInList.Contains).Count() == segments.Count;
        }

        /*struct IndexWithValue<T> {
            public int Index;
            public T Value;

            public IndexWithValue(int index, T value) {
                Index = index;
                Value = value;
            }
        }*/

        static List<int>? solveEpisodes(Title mainFeature, IEnumerable<Title> titles) {
            //List<IndexWithValue<Title>> withIndex = titles.Select(t => new IndexWithValue<Title>())
            List<Title> sorted = titles.OrderBy(x => x.Segments.Count).ToList();
            return solveEpisodes(mainFeature, sorted, 0);
        }

        static List<int>? solveEpisodes(Title mainFeature, List<Title> titles, int index) {
            if (index == mainFeature.Segments.Count)
            {
                // Success! Start returning the result
                return new List<int>();
            }

            foreach (Title title in titles)
            {
                if (segmentsMatch(title.Segments, mainFeature, index))
                {
                    List<int>? result = solveEpisodes(mainFeature, titles, index + title.Segments.Count);
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
