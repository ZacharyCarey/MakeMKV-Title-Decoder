using MakeMKV_Title_Decoder.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder {
    class EpisodeSolution {
        public List<Title> Titles;

        public EpisodeSolution(List<Title> titles) {
            this.Titles = titles;
        }

        public override string ToString() {
            return $"[{string.Join(", ", Titles.Select(x => x.SimplifiedFileName))}]";
        }
    }

    internal static class SegmentIdentifier {

        public static Title FindMainFeature(IEnumerable<Title> titles) {
            // Main feature will have the longest run time. In case of a tie, pick the playlist file that contains chapter info
            // NOTE: this is still done for the non-movie types to weed-out the play-all playlist and help
            Title mainFeature = titles.First();
            foreach(Title title in titles.Skip(1))
            {
                if (DurationsAlmostSimilar(title, mainFeature)) {
                    if (title.SourceFileExtension == "mpls" && mainFeature.SourceFileExtension != "mpls")
                    {
                        mainFeature = title;
                    }
                } else if (title.Duration.Value > mainFeature.Duration.Value)
                {
                    // This title is longer, store it
                    mainFeature = title;
                }
            }

            return mainFeature;
        }

        public static List<EpisodeSolution> FindEpisodes(Title mainFeature, IEnumerable<Title> remainingTitles) {
            return FindEpisodes(mainFeature, remainingTitles.Where(x => x != mainFeature), 0).ToList();
        }

        private static IEnumerable<EpisodeSolution> FindEpisodes(Title mainFeature, IEnumerable<Title> remainingTitles, int index) {
            if (index == mainFeature.Segments.Count)
            {
                // Success! Start returning the result
                yield return new EpisodeSolution(new List<Title>());
                yield break;
            }

            foreach(Title title in remainingTitles)
            {
                if (SegmentsMatch(title.Segments, mainFeature.Segments, index))
                {
                    IEnumerable<EpisodeSolution> solutions = FindEpisodes(mainFeature, remainingTitles.Where(x => x != title), index + title.Segments.Count);
                    foreach(var solution in solutions)
                    {
                        // Add our title to the list
                        // Insert at index 0 to maintain the correct order
                        solution.Titles.Insert(0, title);
                        yield return solution;
                    }
                }
            }
        }

        public static bool SegmentsMatch(List<int> segments, List<int> mainSegments, int mainStartIndex) {
            if (mainStartIndex + segments.Count - 1 > mainSegments.Count)
            {
                return false;
            }

            for(int i = 0; i < segments.Count; i++)
            {
                if (segments[i] != mainSegments[mainStartIndex + i])
                {
                    return false;
                }
            }

            return true;
        }

        private static bool DurationsAlmostSimilar(Title left, Title right) {
            // If within 1% of the larger runtime, or 1 second minimum, then considered almost the same
            TimeSpan larger;
            TimeSpan smaller;
            if (left.Duration.Value >= right.Duration.Value)
            {
                larger = left.Duration.Value;
                smaller = right.Duration.Value;
            } else
            {
                larger = right.Duration.Value;
                smaller = left.Duration.Value;
            }

            int deltaSeconds = (int)(larger - smaller).TotalSeconds;
            int onePercentSeconds = (int)(larger.TotalSeconds * 0.01);
            int maxDelta = Math.Max(onePercentSeconds, 1); // Minimum 1 second
            return deltaSeconds <= maxDelta;
        }

        public static bool IsIncomplete(Title title) {
            bool hasAudio = false;
            bool hasVideo = false;
            foreach(Track track in title.Tracks)
            {
                if (track.Type == TrackType.Video)
                {
                    hasVideo = true;
                } else if (track.Type == TrackType.Audio)
                {
                    hasAudio = true;
                }
                if (hasAudio && hasVideo)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
