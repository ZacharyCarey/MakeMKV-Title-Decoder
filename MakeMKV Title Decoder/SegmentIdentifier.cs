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

        public Title MainFeature;
        public HashSet<int> DeselectTitlesIndicies = new();

        public SegmentIdentifier(List<Title> allTitles) {
            HashSet<int> titles = new(allTitles.Select(x => x.Index)); // Create a copy so we can edit

            // Main feature will have the longest run time. In case of a tie, pick the playlist file that contains chapter info
            MainFeature = allTitles[0];
            for(int index = 1; index < allTitles.Count; index++)
            {
                int compare = allTitles[index].Duration.CompareTo(MainFeature.Duration);
                if (compare > 0)
                {
                    // This title is longer, store it
                    MainFeature = allTitles[index];
                } else if (compare == 0)
                {
                    // This title is the same length, only store if a playlist and the current one is not
                    if (allTitles[index].SourceFileExtension == "mpls" && MainFeature.SourceFileExtension != "mpls")
                    {
                        MainFeature = allTitles[index];
                    }
                }
            }
            titles.Remove(MainFeature.Index);

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
                }
            }

            // TODO this is only true for movies
            // After we found the main feature, we can deselect any other streams/chapters that is uses
            // Sometimes the chapters will all be separate streams that all show up in the list
            // The main feature will be all those streams stiched together wither chapter info, so we just
            // can just not download those.
            temp = new(titles);
            foreach(int index in temp)
            {
                // If all segments are contained within the main feature...
                if (allTitles[index].Segments.Where(MainFeature.Segments.Contains).Count() == allTitles[index].Segments.Count)
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
                foreach(int index in temp)
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
}
