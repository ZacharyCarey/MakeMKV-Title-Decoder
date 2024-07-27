using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder {
    public struct Episode {
        // all indices that could potentially be this episode.
        // USUALLY is just one.
        public List<Ref<Title>> Titles = new();

        public Episode() {
        }

        public override string ToString() {
            if (Titles.Count == 0)
            {
                return "null";
            } else if (Titles.Count == 1)
            {
                return Titles.First().Value.SimplifiedFileName;
            } else
            {
                return "{" + string.Join(", ", Titles.Select(x => x.Value.SimplifiedFileName)) + "}";
            }
        }
    }

    internal class SegmentIdentifier {

        /*private class TitleLengthComparer : IComparer<Title> {
            public int Compare(Title x, Title y) {
                return x.Duration.CompareTo(y.Duration);
            }
        }*/

        public Ref<Title> MainFeature; // Movies only
        public List<Episode> MainTitleTracks = new(); // Non-movies only
        public bool IsMovie { get => MainTitleTracks.Count == 0; }
        public HashSet<Ref<Title>> DeselectTitlesIndicies = new();
        public HashSet<Ref<Title>> BonusFeatures = new();

        private List<Title> AllTitles; // The master list of all titles, in order, as scraped.
        private HashSet<Ref<Title>> RemainingTitles; // A working set of indices (referencing AllTitles) of which titles have not been processed

        public SegmentIdentifier(List<Title> allTitles, DvdType type, bool ignoreIncomplete) {
            this.AllTitles = allTitles;
            this.RemainingTitles = new(allTitles.AsPointers()); // Create a copy so we can edit

            this.MainFeature = this.identifyMainFeature();
            this.RemainingTitles.Remove(this.MainFeature); // Dont consider it in any other decisions

            // Print some debug info
            Console.WriteLine($"MainFeature = {MainFeature.Value.SimplifiedFileName}, [{string.Join(", ", MainFeature.Value.Segments)}]");

            // TV and bonus features dont actually want the whole disc in a single file.
            // For that, one episode per file is desired
            if (type != DvdType.Movie)
            {
                DeselectTitlesIndicies.Add(this.MainFeature);
            }


            // Deselect any titles that have the exact same runtime.
            // Hopefully removes playlists that are both the same main feature, but slightly different
            // For instance, in 'Into the Spider-verse' the second playlist has a slightly different end credits but are 99% the same.
            HashSet<Ref<Title>> mainTitleDupes = findTitlesWithMatchingRuntime(this.MainFeature, "Found dupicate main feature = ");
            this.DeselectTitlesIndicies.AddAll(mainTitleDupes);
            this.RemainingTitles.RemoveAll(mainTitleDupes);


            if (type != DvdType.Movie)
            {
                // We want to find all the streams contained in the main feature in the smallest possible file (hopefully individual segments)
                // If we cant find all individual segments, we will just return the "play all" like a movie
                Console.WriteLine("Searching for TV episodes...");

                var mainFeatureEpisodes = findMainFeatureEpisodes();

                if(mainFeatureEpisodes.Episodes == null) {
                    Console.WriteLine("Failed to find enough segments for TV/Bonus features, treating as a movie instead.");
                    type = DvdType.Movie;
                } else
                {
                    // Adjust lists as needed
                    this.MainTitleTracks = mainFeatureEpisodes.Episodes;
                    this.DeselectTitlesIndicies.AddAll(mainFeatureEpisodes.Dupes);
                    this.BonusFeatures.AddAll(mainFeatureEpisodes.BonusFeatures);

                    // Remove both features and dupes from remaining titles, as they are 'processed'
                    this.RemainingTitles.RemoveAll(mainFeatureEpisodes.Dupes);
                    this.RemainingTitles.RemoveAll(mainFeatureEpisodes.BonusFeatures);
                    foreach(Episode episode in mainFeatureEpisodes.Episodes)
                    {
                        this.RemainingTitles.RemoveAll(episode.Titles);
                    }
                }
            }

            if (type == DvdType.Movie)
            {
                // After we found the main feature, we can deselect any other streams/chapters that it uses
                // Sometimes the chapters will all be separate streams that all show up in the list
                // The main feature will be all those streams stitched together wither chapter info, so we just
                // can just not download those.
                var subsetTitles = findSubsetTitles(new HashSet<int>(this.MainFeature.Value.Segments));
                this.DeselectTitlesIndicies.AddAll(subsetTitles);
                this.RemainingTitles.RemoveAll(subsetTitles);
            }

            // Now attempt to find other playlists that can be broken down into smaller chunks.
            // For example, there is usually a "play all" button for bonus features, but we want those to be separate
            // This is done for every remaining item to try and get the smallest possible chunk
            var result = breakdownTitles();
            this.BonusFeatures.AddAll(result.BonusFeatures);
            this.RemainingTitles.RemoveAll(result.BonusFeatures);

            this.DeselectTitlesIndicies.AddAll(result.Deselects);
            this.RemainingTitles.RemoveAll(result.Deselects);

            // Double check any playlists/streams that were deselected, and deselect all streams
            // that whose segments are only contained in deselected stream (done recursivley)
            // For instance, in 'Into the spider-verse' the second main feature gets deselected,
            // but it has a unique credits stream that only it uses, so there is no point in
            // downloading that credit stream separately if we didnt want that feature
            //
            // NOTE to be deselected, segment MUST ASLO be contained in a deselected title.
            // This is to prevent removing things like special features
            /*while (true)
            {
                int? titleIndex = findDanglingTitle();
                if (titleIndex != null)
                {
                    int index = (int)titleIndex;
                    Console.WriteLine($"Removed {allTitles[index].SimplifiedFileName} as its segments were only found in deselected titles [{string.Join(", ", allTitles[index].Segments)}]");
                    DeselectTitlesIndicies.Add(index);
                    this.RemainingTitles.Remove(index);
                } else
                {
                    break;
                }
            }*/

            // Do a final pass on all titles and make some adjustments depending on settings
            foreach (Ref<Title> title in this.AllTitles.AsPointers())
            {
                bool deselect = this.DeselectTitlesIndicies.Contains(title);
                bool isMainFeature = (title.Index == this.MainFeature.Index);
                if (!isMainFeature)
                {
                    // Loop through main episodes to check if considered a main feature
                    foreach (Episode episode in this.MainTitleTracks) // TODO rename to identifier.MainEpisodes?
                    {
                        if (episode.Titles.Contains(title))
                        {
                            isMainFeature = true;
                            break;
                        }
                    }
                }
                bool isBonusFeature = this.BonusFeatures.Contains(title);

                // Filter additional titles
                if ((deselect == false) && (isMainFeature == false) /*&& (isBonusFeature == false)*/ && ignoreIncomplete)
                {
                    bool hasAudio = title.Value.Tracks.Select(x => x.Type).Where(x => x == TrackType.Audio).Any();
                    bool hasVideo = title.Value.Tracks.Select(x => x.Type).Where(x => x == TrackType.Video).Any();
                    if (!(hasAudio && hasVideo))
                    {
                        Console.WriteLine($"Deselected {title.Value.SimplifiedFileName} did not have both audio and video.");
                        deselect = true;
                        this.DeselectTitlesIndicies.Add(title);
                        this.BonusFeatures.Remove(title);
                        this.RemainingTitles.Remove(title);
                    }
                }
            }

            if (this.RemainingTitles.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"WARNING: There were {this.RemainingTitles.Count} titles that were not assigned a category.");
                Console.ResetColor();
            }
        }

        private static bool areAllSegmentsInList(ICollection<int> segments, ICollection<int> checkInList) {
            return segments.Where(checkInList.Contains).Count() == segments.Count;
        }

        static (List<Episode> Episodes, HashSet<Ref<Title>> Deselections, HashSet<Ref<Title>> BonusFeatures)? solveEpisodes(Ref<Title> mainFeature, IEnumerable<Ref<Title>> titles) {
            List<Ref<Title>> sorted = titles.OrderBy(x => x.Value.Segments.Count).ToList();
            HashSet<Ref<Title>> deselections = new();
            HashSet<Ref<Title>> bonusFeatures = new();
            List<Episode>? episodes = solveEpisodes(mainFeature, sorted, 0, deselections, bonusFeatures);
            if (episodes == null)
            {
                return null;
            } else
            {
                return (episodes, deselections, bonusFeatures);
            }
        }

        static List<Episode>? solveEpisodes(Ref<Title> mainFeature, List<Ref<Title>> titles, int index, HashSet<Ref<Title>> deselections, HashSet<Ref<Title>> bonusFeatures) {
            if (index == mainFeature.Value.Segments.Count)
            {
                // Success! Start returning the result
                return new List<Episode>();
            }

            foreach (Ref<Title> title in titles)
            {
                if (segmentsMatch(title.Value.Segments, mainFeature, index))
                {
                    // Special case. If they are exactly the same as the main feature, just keep the main feature
                    if (index == 0 && title.Value.Segments.Count == mainFeature.Value.Segments.Count)
                    {
                        Console.WriteLine($"Deselected {title.Value.SimplifiedFileName} because it has the exact same segments as another feature ({mainFeature.Value.SimplifiedFileName}).");
                        deselections.Add(title);
                        continue;
                    }

                    // Ok, I know how this looks but it's to prevent the same feature from being used twice and I was too lazy to make it more efficient. It's 1 am on a Friday morning while writing this.
                    List<Ref<Title>> newScope = new(titles);
                    newScope.Remove(title);
                    List<Episode>? result = solveEpisodes(mainFeature, newScope, index + title.Value.Segments.Count, deselections, bonusFeatures);
                    if (result == null)
                    {
                        // No successful match found :(
                        continue;
                    }

                    // We found a successful match, but there is still one last step to be performed.
                    Episode episode = new();

                    // First see if there are any titles with the same segments as this one.
                    // An issue appeared when ripping Avatar: The Last Airbender where there was a bonus feature of creator commentary on an entire episode.
                    // that episode was being selected as the "main feature episode" since it had the exact same tun run time and segments as the real episode.
                    // So we look for other episodes of similar length and the same segments, and pick the one that better matches in tracks.
                    // It the case of A:TLA, the commentary only has a single audio/video track, whereas the real one had multiple audios, subtitles, and an attachment.
                    // However in the case of a tie, just return both as the same episode and let the user decide.
                    List<Ref<Title>> realTitle = new(){ title };// Start by assuming it's this one
                    int realScore = getTrackDifferenceCount(mainFeature, title);
                    bool foundDupes = false;
                    foreach (Ref<Title> possibleDupe in titles.Where(x => x.Index != title.Index))
                    {
                        // NOTE: compare time using original title to prevent incremental drifting
                        if (DurationsAlmostSimilar(title, possibleDupe) && possibleDupe.Value.Segments.SequenceEqual(title.Value.Segments))
                        {
                            foundDupes = true;
                            // Ok these two titles likely match, decide which one is 'real'
                            int trackDifferentCount = getTrackDifferenceCount(mainFeature, possibleDupe);
                            if (trackDifferentCount < realScore)
                            {
                                // This title has less incorrect tracks, so it's more likely to be the 'real' episode
                                // What we that to be the 'real' episode is probably just a bonus feature
                                foreach(Ref<Title> bonus in realTitle)
                                {
                                    bonusFeatures.Add(bonus);
                                    Console.WriteLine($"Found bonus feature {bonus.Value.SimplifiedFileName} because it has non-matching tracks with the main feature: [{string.Join(", ", bonus.Value.Tracks.Select(x => x.ToString()))}]");
                                }
                                realTitle.Clear();
                                realTitle.Add(possibleDupe);
                                realScore = trackDifferentCount;
                            } else if (trackDifferentCount > realScore)
                            {
                                // This title has more incorrect tracks, it's definitely a different feature but most likely a bonus feature.
                                bonusFeatures.Add(possibleDupe);
                                Console.WriteLine($"Found bonus feature {possibleDupe.Value.SimplifiedFileName} because it has non-matching tracks with the main feature: [{string.Join(", ", possibleDupe.Value.Tracks.Select(x => x.ToString()))}]");
                            } else
                            {
                                // As far as we can tell, the two titles are the same. Not ideal :(
                                realTitle.Add(possibleDupe);
                            }
                        }
                    }
                    if (foundDupes)
                    {
                        if (realTitle.Count > 1)
                        {
                            Console.WriteLine($"Found duplicate titles that are too similar: [{string.Join(", ", realTitle.Select(x => x.Value.SimplifiedFileName))}]");
                        } else
                        {
                            Console.WriteLine($"Found duplicate titles for episode {realTitle[0].Value.SimplifiedFileName}");
                        }
                    }
                    episode.Titles.AddRange(realTitle);

                    // Continue on the return path for remaining episodes
                    // Put at the front of the list so it's in the correct order
                    result.Insert(0, episode);
                    return result;
                }
            }

            return null;
        }

        private static int getTrackDifferenceCount(Ref<Title> modelTitle, Ref<Title> compareTitle) {
            // TODO compare metadata as well?
            int count = 0;
            for(int i = 0; i < modelTitle.Value.Tracks.Count; i++)
            {
                if (i >= compareTitle.Value.Tracks.Count || modelTitle.Value.Tracks[i] != compareTitle.Value.Tracks[i])
                {
                    count++;
                }
            }

            return count;
        }

        static bool segmentsMatch(List<int> segments, Ref<Title> mainFeature, int index) {
            if (index + segments.Count - 1 > mainFeature.Value.Segments.Count)
            {
                return false;
            }

            for (int i = 0; i < segments.Count; i++)
            {
                if (segments[i] != mainFeature.Value.Segments[index + i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Returns the index to the main feature
        /// </summary>
        private Ref<Title> identifyMainFeature() {
            // Main feature will have the longest run time. In case of a tie, pick the playlist file that contains chapter info
            // NOTE: this is still done for the non-movie types to weed-out the play-all playlist and help
            Ref<Title> mainFeature = this.RemainingTitles.First();
            foreach (Ref<Title> title in this.RemainingTitles.Skip(1))
            {
                int compare = title.Value.Duration.CompareTo(mainFeature.Value.Duration);
                if (DurationsAlmostSimilar(title, mainFeature))
                {
                    if (title.Value.SourceFileExtension == "mpls" && mainFeature.Value.SourceFileExtension != "mpls")
                    {
                        mainFeature = title;
                    }
                } else if (title.Value.Duration > mainFeature.Value.Duration)
                {
                    // This title is longer, store it
                    mainFeature = title;
                }
            }

            return mainFeature;
        }

        /// <summary>
        /// Compares remaining titles and find any that have the same runtime as the given title.
        /// </summary>
        private HashSet<Ref<Title>> findTitlesWithMatchingRuntime(Ref<Title> title, string? debugMessage = null) {
            HashSet<Ref<Title>> result = new();
            foreach (Ref<Title> compare in this.RemainingTitles)
            {
                if (compare.Value.Duration == title.Value.Duration)
                {
                    result.Add(compare);
                    if (debugMessage != null)
                    {
                        Console.WriteLine($"{debugMessage}{compare.Value.SimplifiedFileName}");
                    }
                }
            }

            return result;
        }


        private (List<Episode>? Episodes, HashSet<Ref<Title>> Dupes, HashSet<Ref<Title>> BonusFeatures) findMainFeatureEpisodes() {
            IEnumerable<Ref<Title>> titles = this.RemainingTitles;
            var result = solveEpisodes(MainFeature, titles);
            if (result == null)
            {
                // Failed to find a viable solution
                return (null, new(), new());
            }

            List<Episode> results = result.Value.Episodes;
            HashSet<Ref<Title>> deselections = result.Value.Deselections;
            HashSet<Ref<Title>> bonus = result.Value.BonusFeatures;

            // Print some debug info
            Console.WriteLine("Found solution for episode layout.");
            foreach (Episode episode in results)
            {
                Ref<Title> title = episode.Titles[0];
                Console.WriteLine($"\t[{string.Join(", ", title.Value.Segments)}] @ {episode}");
            }

            HashSet<Ref<Title>> remainingTitles = new(this.RemainingTitles);
            remainingTitles.RemoveAll(deselections);
            remainingTitles.RemoveAll(bonus);
            foreach (Episode episode in results)
            {
                remainingTitles.RemoveAll(episode.Titles);
            }

            // Try and remove any duplicate playlists
            foreach (Ref<Title> title in remainingTitles)
            {
                if (areAllSegmentsInList(title.Value.Segments, this.MainFeature.Value.Segments))
                {
                    Console.WriteLine($"Segments [{string.Join(", ", title.Value.Segments)}] found in main feature, deselting {title.Value.SimplifiedFileName}");
                    deselections.Add(title);
                }
            }

            return (results, deselections, bonus);
        }

        /// <summary>
        /// Finds any titles whose segments are fully contained in the given list of segments, and returns them in a list
        /// </summary>
        private HashSet<Ref<Title>> findSubsetTitles(HashSet<int> segments) {
            HashSet<Ref<Title>> result = new();
            foreach (var title in this.RemainingTitles)
            {
                // If all segments are contained within the list...
                if (areAllSegmentsInList(title.Value.Segments, segments))
                {
                    // This title must be a subset
                    result.Add(title);
                }
            }

            return result;
        }

        /// <summary>
        /// Attempts to breakdown playlists into smaller videos.
        /// Usually done for bonus features, so instead of getting one big "play all" video
        /// you can get the features as individual videos
        /// 
        /// This function is usually called multiple times to get the desired effect
        /// </summary>
        private (HashSet<Ref<Title>> BonusFeatures, HashSet<Ref<Title>> Deselects) breakdownTitles() {
            HashSet<Ref<Title>> bonusFeatures = new();
            HashSet<Ref<Title>> deselections = new();

            HashSet<Ref<Title>> titles = new(this.RemainingTitles);
            while (titles.Count > 0)
            {
                Ref<Title> playlist = titles.First();
                IEnumerable<Ref<Title>> remainingTitles = titles.Where(title => title != playlist);
                var subtitleResults = solveEpisodes(playlist, remainingTitles); // Subtitles as in titles that make up the larger main title
                if (subtitleResults != null)
                {
                    // We managed to divide this title into smaller bits
                    deselections.AddAll(subtitleResults.Value.Deselections);
                    titles.RemoveAll(subtitleResults.Value.Deselections);

                    deselections.Add(playlist);
                    titles.Remove(playlist);

                    // DO NOT remove the subdisions, they are kept to try and subdivide again
                    //bonusFeatures.AddAll(subtitleResults.Value.BonusFeatures);
                    //titles.RemoveAll(bonusFeatures);
                    Console.WriteLine($"Title {playlist.Value.SimplifiedFileName} was subdivided into smaller videos: [{string.Join(", ", playlist.Value.Segments)}]");
                    foreach(Episode episode in subtitleResults.Value.Episodes)
                    {
                        Console.WriteLine($"\t[{string.Join(", ", episode.Titles.First().Value.Segments)}] @ {episode}");
                    }
                } else
                {
                    // This title can't be subdivided anymore than it already is, so it must be a bonus feature
                    bonusFeatures.Add(playlist);
                    titles.Remove(playlist);
                }
            }

            // Now all titles should be marked as deselected or a bonus feature
            // Double check our work
            if (bonusFeatures.Intersect(deselections).Any())
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("WARNING: sets overlapped when breaking down titles.");
                Console.ResetColor();
            }

            HashSet<Ref<Title>> check = new(this.RemainingTitles);
            check.RemoveAll(deselections);
            check.RemoveAll(bonusFeatures);
            if (check.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("WARNING: sets did not color all possible titles.");
                Console.ResetColor();
            }

            return (bonusFeatures, deselections);
        }

        /// <summary>
        /// A "dangling title" is one who's segments are fully contained withing a title that was deselected.
        /// </summary>
        /// <returns></returns>
        /*private int? findDanglingTitle() {
            foreach (int index in this.RemainingTitles)
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
                    Console.WriteLine($"Removed {allTitles[index].SimplifiedFileName} as its segments were only found in deselected titles [{string.Join(", ", allTitles[index].Segments)}]");
                    DeselectTitlesIndicies.Add(index);
                    titles.Remove(index);
                }
            }
        }*/

        private static bool DurationsAlmostSimilar(Ref<Title> left, Ref<Title> right) {
            // If within 1% of the larger runtime, or 1 second minimum, then considered almost the same
            TimeSpan larger;
            TimeSpan smaller;
            if (left.Value.Duration >= right.Value.Duration)
            {
                larger = left.Value.Duration;
                smaller = right.Value.Duration;
            } else
            {
                larger = right.Value.Duration;
                smaller = left.Value.Duration;
            }

            int deltaSeconds = (int)(larger - smaller).TotalSeconds;
            int onePercentSeconds = (int)(larger.TotalSeconds * 0.01);
            int maxDelta = Math.Max(onePercentSeconds, 1); // Minimum 1 second
            return deltaSeconds <= maxDelta;
        }
    }


    public static class IdentifierUtils {
        public static void AddAll<T>(this HashSet<T> set, IEnumerable<T> values) {
            foreach(T value in values)
            {
                set.Add(value);
            }
        }

        public static void RemoveAll<T>(this HashSet<T> set, IEnumerable<T> values) {
            foreach(T value in values)
            {
                set.Remove(value);
            }
        }
    }
}
