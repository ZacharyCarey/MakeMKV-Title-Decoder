using MakeMKV_Title_Decoder.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder {
    public class NamedTitle {
        public string Name;
        public Title Title;

        public NamedTitle (string name, Title title) {
            this.Name = name;
            this.Title = title;
        }

        public override string ToString() {
            return Name;
        }
    }

    internal class VideoRenamerStateMachine {
        const bool verbose = true;
        const ConsoleColor consoleColor = ConsoleColor.Magenta;
        const ConsoleColor verboseColor = ConsoleColor.DarkMagenta;
        Disc disc;
        HashSet<Title> remainingTitles;
        Queue<Title> titlesToRename = new();
        Title? currentTitle = null;
        List<EpisodeSolution> currentEpisodes = new();
        public bool HasEpisodes { get => currentEpisodes.Count > 0; }
        bool ignoreIncomplete;

        public HashSet<Title> RenamedTitles = new();
        public HashSet<Title> DeletedTitles = new();

        public VideoRenamerStateMachine(Disc disc, bool ignoreIncompleteVideos) {
            this.disc = disc;
            this.remainingTitles = new(disc.Titles);
            this.ignoreIncomplete = ignoreIncompleteVideos;

            Title mainFeature = SegmentIdentifier.FindMainFeature(this.remainingTitles);
            print($"Found main feature={mainFeature.SimplifiedFileName}");
            titlesToRename.Enqueue(mainFeature);
        }
        
        /// <summary>
        /// Gets the next video to present to the user.
        /// 'ApplyChoice' must be called between choices
        /// </summary>
        /// <returns></returns>
        public Title? NextTitle() {
            Debug.Assert(currentTitle == null, "Please call 'ApplyChoice' before calling 'NextTitle'.");
            currentEpisodes = new();
            while (titlesToRename.Count > 0 || this.remainingTitles.Count > 0)
            {
                string message;
                if (titlesToRename.Count == 0)
                {
                    if (this.remainingTitles.Count == 0)
                    {
                        print("There are no remaining titles to process.");
                        return null;
                    }

                    currentTitle = SegmentIdentifier.FindMainFeature(remainingTitles);
                    message = $"Got next title from remaining titles: {currentTitle.SimplifiedFileName}";
                } else
                {
                    this.currentTitle = titlesToRename.Dequeue();
                    message = $"Got the next title from the queue: {currentTitle.SimplifiedFileName}";
                }

                this.remainingTitles.Remove(currentTitle);
                if (RenamedTitles.Contains(this.currentTitle) || DeletedTitles.Contains(this.currentTitle))
                {
                    print($"Skipped {this.currentTitle.SimplifiedFileName} because it was already processed.");
                    this.currentTitle = null;
                    continue;
                }
                if (this.ignoreIncomplete && SegmentIdentifier.IsIncomplete(this.currentTitle))
                {
                    print($"Deleted {this.currentTitle.SimplifiedFileName} because it was incomplete (missing audio or video).");
                    this.DeletedTitles.Add(this.currentTitle);
                    this.currentTitle = null;
                    continue;
                }
                print(message);

                this.currentEpisodes = SegmentIdentifier.FindEpisodes(currentTitle, this.remainingTitles);
                if (this.currentEpisodes.Count == 0)
                {
                    print("Unable to breakout episode any further.");
                } else if (this.currentEpisodes.Count == 1)
                {
                    print($"Found a solution to episodes: {this.currentEpisodes[0]}");
                } else
                {
                    print($"Found multiple solutions to episodes:");
                    foreach (var solution in this.currentEpisodes)
                    {
                        print($"\t{solution}");
                    }
                }

                verboseCurrentState();

                return currentTitle;
            }

            print("There are no remaining titles to process.");
            return null;
        }

        /// <summary>
        /// Applies the user choice to the current title.
        /// 'NextTitle' must be called between choices
        /// 
        /// This function keeps the current episode with the given options
        /// Returns an object to add to the "SameAs" list.
        /// </summary>
        public NamedTitle? ApplyChoice(string userName, bool isBonusFeature, bool deleteEpisodes) {
            Debug.Assert(currentTitle != null, "No video is selected. Please call 'NextTitle' first.");

            NamedTitle namedTitle = new(userName, this.currentTitle);
            this.currentTitle.UserName = userName;
            this.RenamedTitles.Add(this.currentTitle);
            print($"Marked {this.currentTitle.SimplifiedFileName} to keep. name={this.currentTitle.UserName}");

            if (deleteEpisodes)
            {
                foreach(var solution in this.currentEpisodes)
                {
                    foreach(var title in solution.Titles)
                    {
                        // Sanity check to not remove files that have already been marked to keep
                        if (!this.RenamedTitles.Contains(title))
                        {
                            this.DeletedTitles.Add(title);
                            this.remainingTitles.Remove(title);
                            print($"Marked {title.SimplifiedFileName} for deletion.");
                        }
                    }
                }
            }

            verboseCurrentState();

            this.currentTitle = null;
            this.currentEpisodes.Clear();
            return namedTitle;
        }

        /// <summary>
        /// Applies the user choice to the current title.
        /// 'NextTitle' must be called between choices
        /// 
        /// This function deletes the current episode with the given options
        /// Returns an object to add to the "SameAs" list.
        /// </summary>
        public NamedTitle? ApplyChoice(bool deleteEpisodes) {
            Debug.Assert(currentTitle != null, "No video is selected. Please call 'NextTitle' first.");

            this.DeletedTitles.Add(this.currentTitle);
            print($"Marked {this.currentTitle.SimplifiedFileName} for deletion.");

            if (deleteEpisodes)
            {
                foreach(var solution in this.currentEpisodes)
                {
                    foreach(var title in solution.Titles)
                    {
                        // Sanity check to not remove files that have already been marked to keep
                        if (!this.RenamedTitles.Contains(title))
                        {
                            this.DeletedTitles.Add(title);
                            this.remainingTitles.Remove(title);
                            print($"Marked {title.SimplifiedFileName} for deletion");
                        }
                    }
                }
            }

            verboseCurrentState();

            this.currentTitle = null;
            this.currentEpisodes.Clear();
            return null;
        }

        /// <summary>
        /// Applies the user choice to the current title.
        /// 'NextTitle' must be called between choices
        /// 
        /// This function will delete the current episode in preference of using it's episodes instead
        /// Returns an object to add to the "SameAs" list.
        /// </summary>
        public NamedTitle? ApplyChoice() {
            Debug.Assert(currentTitle != null, "No video is selected. Please call 'NextTitle' first.");
            Debug.Assert(currentEpisodes != null, "Current title cannot be broken apart.");

            this.DeletedTitles.Add(this.currentTitle);
            print($"Marked {this.currentTitle.SimplifiedFileName} for deletion");

            foreach(var solution in this.currentEpisodes)
            {
                foreach(var title in solution.Titles)
                {
                    this.titlesToRename.Enqueue(title);
                    print($"Added {title.SimplifiedFileName} to the process queue.");
                }
            }

            verboseCurrentState();

            this.currentTitle = null;
            this.currentEpisodes.Clear();
            return null;
        }

        /// <summary>
        /// Applies the user choice to the current title.
        /// 'NextTitle' must be called between choices
        /// 
        /// This function will set this title as the same as a different title
        /// Returns an object to add to the "SameAs" list.
        /// </summary>
        public NamedTitle? ApplyChoice(NamedTitle sameAsTitle, string rootFolder) {
            Debug.Assert(currentTitle != null, "No video is selected. Please call 'NextTitle' first.");

            VideoComparer form = new(rootFolder, sameAsTitle, this.currentTitle);
            form.ShowDialog();

            if (form.PreferVideo1)
            {
                // Keep the old title, delete the new one
                this.DeletedTitles.Add(this.currentTitle);
                print($"Marked {this.currentTitle.SimplifiedFileName} for deletion.");
            } else
            {
                // Replace the old title with the new title
                print($"Marked {sameAsTitle.Title.SimplifiedFileName} (name={sameAsTitle.Title.UserName}) for deletion.");
                sameAsTitle.Title.UserName = null;
                this.DeletedTitles.Add(sameAsTitle.Title);
                this.RenamedTitles.Remove(sameAsTitle.Title);

                this.currentTitle.UserName = sameAsTitle.Name;
                this.RenamedTitles.Add(this.currentTitle);
                print($"Marked {this.currentTitle.SimplifiedFileName} to keep name={this.currentTitle.UserName}");
            }

            verboseCurrentState();

            this.currentTitle = null;
            this.currentEpisodes.Clear();
            return null;
        }

        private void print(string str, ConsoleColor color = consoleColor) {
            Console.ForegroundColor = color;
            Console.WriteLine(str);
            Console.ResetColor();
        }

        private void verboseCurrentState() {
            Console.ForegroundColor = verboseColor;
            Console.WriteLine("Current state: ");
            Console.WriteLine($"\tRemaining: [{string.Join(", ", this.remainingTitles.Select(x => x.SimplifiedFileName))}]");
            Console.WriteLine($"\tProcess queue: [{string.Join(", ", this.titlesToRename.Select(x => x.SimplifiedFileName))}]");
            Console.WriteLine($"\tRenamed: [{string.Join(", ", this.RenamedTitles.Select(x => x.SimplifiedFileName))}]");
            Console.WriteLine($"\tDeleted: [{string.Join(", ", this.DeletedTitles.Select(x => x.SimplifiedFileName))}]");
            Console.ResetColor();
        }
    }
}
