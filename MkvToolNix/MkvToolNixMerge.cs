using MkvToolNix.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace MkvToolNix
{
    public class MergeData
    {
        /// <summary>
        /// Title that shows in VLC when played (i.e. title saved in file)
        /// </summary>
        public string? Title = null;
        private List<SourceFile> SourceFiles = new();

        public SourceFile AddSourceFile(string filePath)
        {
            SourceFile file = new(filePath);
            this.SourceFiles.Add(file);
            return file;
        }

        internal IEnumerable<string> GetCommand(string outputPath)
        {
            if (Title != null)
            {
                yield return "--title";
                yield return $"\"{this.Title}\"";
            }

            yield return "--output";
            yield return $"\"{outputPath}\"";

            foreach (var sourceFile in SourceFiles)
            {
                foreach (string arg in sourceFile.GetCommand())
                {
                    yield return arg;
                }
            }

            // Determine file index
            Dictionary<File, int> fileIndexLookup = new();
            int index = 0;
            foreach(var sourceFile in this.SourceFiles)
            {
                fileIndexLookup[sourceFile] = index++;
                foreach(var appendedFile in sourceFile.AppendedFiles)
                {
                    fileIndexLookup[appendedFile] = index++;
                }
            }

            // Determine track order
            List<(int Index, string Command)> trackOrder = new();
            List<string> appendTo = new();
            foreach(var sourceFile in this.SourceFiles)
            {
                sourceFile.GetTrackOrder(fileIndexLookup, trackOrder, appendTo);
            }

            trackOrder.Sort((x, y) => x.Index.CompareTo(y.Index));

            int lastIndex = -1;
            foreach(var pair in trackOrder)
            {
                if (pair.Index == lastIndex)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"WARNING: MkvMerge track order index repeated, source tracks may be in wrong order.");
                    Console.ResetColor();
                }
                lastIndex = pair.Index;
            }

            if (trackOrder.Count > 0)
            {
                yield return "--track-order";
                yield return string.Join(",", trackOrder.Select(x => x.Command));
            }

            if (appendTo.Count > 0)
            {
                yield return "--append-to";
                yield return string.Join(",", appendTo);
            }
        }
    } 

    public abstract class File
    {
        public readonly string Path;
        internal abstract IEnumerable<Track> AllTracks { get; }

        protected File(string filePath)
        {
            this.Path = System.IO.Path.GetFullPath(filePath);
        }
    }

    public class SourceFile : File
    {
        internal List<AppendedFile> AppendedFiles = new();
        private List<SourceTrack> Tracks = new();
        internal override IEnumerable<Track> AllTracks => Tracks;

        internal SourceFile(string filePath) : base(filePath)
        {

        }

        public AppendedFile AddAppendedFile(string filePath)
        {
            AppendedFile file = new(filePath);
            AppendedFiles.Add(file);
            return file;
        }

        public SourceTrack AddTrack(long ID, MkvTrackType type, int index = -1)
        {
            SourceTrack track = new(ID, type, index);
            this.Tracks.Add(track);
            return track;
        }

        internal IEnumerable<string> GetCommand()
        {
            List<SourceTrack> audioTracks = this.Tracks.Where(x => x.CopyToOutput && x.TrackType == MkvTrackType.Audio).ToList();
            List<SourceTrack> videoTracks = this.Tracks.Where(x => x.CopyToOutput && x.TrackType == MkvTrackType.Video).ToList();
            List<SourceTrack> subtitleTracks = this.Tracks.Where(x => x.CopyToOutput && x.TrackType == MkvTrackType.Subtitles).ToList();

            if (audioTracks.Count > 0)
            {
                yield return "--audio-tracks";
                yield return string.Join(",", audioTracks.Select(x => x.ID));
            }
            else
            {
                yield return "--no-audio";
            }

            if (videoTracks.Count > 0)
            {
                yield return "--video-tracks";
                yield return string.Join(",", videoTracks.Select(x => x.ID));
            }
            else
            {
                yield return "--no-video";
            }

            if (subtitleTracks.Count > 0)
            {
                yield return "--subtitle-tracks";
                yield return string.Join(",", subtitleTracks.Select(x => x.ID));
            }
            else
            {
                yield return "--no-subtitles";
            }

            // Get all track flags
            foreach(var command in this.Tracks.SelectMany(x => x.GetCommand()))
            {
                yield return command;
            }

            yield return $"\"{this.Path}\"";

            foreach(var appendedFile in this.AppendedFiles)
            {
                foreach(var command in appendedFile.GetCommands())
                {
                    yield return command;
                }
                yield return "+";
                yield return $"\"{appendedFile.Path}\"";
            }
        }
    
        internal void GetTrackOrder(Dictionary<File, int> fileIndexLookup, List<(int Index, string Command)> trackOrder, List<string> appendTo)
        {
            int myIndex = fileIndexLookup[this];
            foreach(var track in this.Tracks.Where(x => x.CopyToOutput))
            {
                if (track.Index >= 0)
                {
                    trackOrder.Add((track.Index, $"{myIndex}:{track.ID}"));
                }
            }

            foreach(var appended in this.AppendedFiles)
            {
                appended.GetTrackOrder(fileIndexLookup, appendTo);
            }
        }
    }

    public class AppendedFile : File
    {
        private List<AppendedTrack> Tracks = new();
        internal override IEnumerable<Track> AllTracks => Tracks;

        internal AppendedFile(string filePath) : base(filePath)
        {
  
        }

        public AppendedTrack AddTrack(long ID, MkvTrackType type, Track AppendedTo)
        {
            AppendedTrack track = new(ID, type, AppendedTo);
            this.Tracks.Add(track);
            return track;
        }

        internal void GetTrackOrder(Dictionary<File, int> fileIndexLookup, List<string> appendTo)
        {
            int myIndex = fileIndexLookup[this];
            foreach(var track in this.Tracks.Where(x => x.CopyToOutput))
            {
                appendTo.Add(track.GetTrackOrder(fileIndexLookup, myIndex));
            }
        }

        internal IEnumerable<string> GetCommands()
        {
            foreach(var track in this.Tracks)
            {
                foreach(string cmd in track.GetCommand())
                {
                    yield return cmd;
                }
            }
        }
    }

    public abstract class Track
    {
        /// <summary>
        /// The ID as provided by <see cref="MkvToolNixInterface.Identify(string, string, string)"/>
        /// </summary>
        internal readonly long ID;

        internal MkvTrackType TrackType;

        /// <summary>
        /// When false, this track will not be copied to the output file, are therefore not exist.
        /// </summary>
        public bool CopyToOutput = true;

        /// <summary>
        /// The delay before the track is played, in milliseconds
        /// </summary>
        public long? DelayMS = null;

        protected Track(long id, MkvTrackType type)
        {
            this.ID = id;
            this.TrackType = type;
            if (type == MkvTrackType.Unknown) throw new Exception("Invalid track type.");
        }

        protected IEnumerable<string> GetDelayCommand()
        {
            if (DelayMS == null) yield break;

             yield return "--sync";
             yield return $"{this.ID}:{this.DelayMS}";
        }
    }

    public class SourceTrack : Track
    {
        public string? Name = null;

        /// <summary>
        /// The language this track is in.
        /// Both ISO 639-2 and ISO 639-1 country codes are allowed. Country codes will be converted
        /// to language codes automatically.
        /// </summary>
        public string? Language = null;

        /// <summary>
        /// Whether or not this track should be considered for playback.
        /// This flag may be a legacy of MP4, and can most likely be ignored.
        /// </summary>
        public bool? EnableFlag = null;

        /// <summary>
        /// This track contains commentary
        /// </summary>
        public bool? CommentaryFlag = null;

        /// <summary>
        /// If the user does not explicitly select a track during playback, the player should select a track
        /// that has the "default flag" set, taking preferences such as their preferred language into account.
        /// </summary>
        public bool? DefaultTrackFlag = null;

        /// <summary>
        /// Usually used for tracks containing onscreen text or foreign-language dialogue
        /// </summary>
        public bool? ForcedDisplayFlag = null;

        /// <summary>
        /// This track is suitable for users with hearing impairments
        /// </summary>
        public bool? HearingImpairedFlag = null;

        /// <summary>
        /// This track contains textual descriptions of video content suitable for playback via text-to-speech
        /// </summary>
        public bool? VisualImpairedFlag = null;

        /// <summary>
        /// This track is in the content's original language (i.e. not a translation)
        /// </summary>
        public bool? OriginalFlag = null;

        /// <summary>
        /// The index of ordering for source tracks
        /// </summary>
        public readonly int Index;

        internal SourceTrack(long id, MkvTrackType type, int index) : base(id, type)
        {
            this.Index = index;
        }

        internal IEnumerable<string> GetCommand()
        {
            if (!CopyToOutput) yield break;

            if (this.Name != null)
            {
                yield return "--track-name";
                yield return $"{this.ID}:\"{this.Name}\"";
            }

            if (this.CommentaryFlag != null)
            {
                yield return "--commentary-flag";
                yield return $"{this.ID}:{(this.CommentaryFlag == true ? "1" : "0")}";
            }

            bool isDefault = false;
            if (this.DefaultTrackFlag != null)
            {
                isDefault = this.DefaultTrackFlag.Value;
            }
            yield return "--default-track-flag";
            yield return $"{this.ID}:{(isDefault ? "1" : "0")}";

            foreach(string cmd in GetDelayCommand())
            {
                yield return cmd;
            }

            if (this.ForcedDisplayFlag != null)
            {
                yield return "--forced-display-flag";
                yield return $"{this.ID}:{(this.ForcedDisplayFlag == true ? "1" : "0")}";
            }

            if (this.HearingImpairedFlag != null)
            {
                yield return "--hearing-impaired-flag";
                yield return $"{this.ID}:{(this.HearingImpairedFlag == true ? "1" : "0")}";
            }

            if (this.VisualImpairedFlag != null)
            {
                yield return "--text-descriptions-flag";
                yield return $"{this.ID}:{(this.VisualImpairedFlag == true ? "1" : "0")}";
            }

            if (this.OriginalFlag != null)
            {
                yield return "--original-flag";
                yield return $"{this.ID}:{(this.OriginalFlag == true ? "1" : "0")}";
            }

            if (this.Language != null)
            {
                yield return "--language";
                yield return $"{this.ID}:{this.Language}";
            }
        }
    }

    public class AppendedTrack : Track
    {
        internal readonly Track AppendedTo;

        internal AppendedTrack(long id, MkvTrackType type, Track appendedTo) : base(id, type)
        {
            this.AppendedTo = appendedTo;
        }

        internal string GetTrackOrder(Dictionary<File, int> fileIndexLookup, int sourceFileIndex)
        {
            long dstFileIndex = -1;
            foreach(var pair in fileIndexLookup)
            {
                var file = pair.Key;
                if (file.AllTracks.Contains(this.AppendedTo))
                {
                    dstFileIndex = pair.Value;
                    break;
                }
            }

            if (dstFileIndex < 0) throw new Exception("Failed to find appened track's source file.");

            long dstTrackIndex = AppendedTo.ID;
            return $"{sourceFileIndex}:{this.ID}:{dstFileIndex}:{dstTrackIndex}";
        }

        internal IEnumerable<string> GetCommand()
        {
            if (!CopyToOutput) yield break;

            foreach (string cmd in GetDelayCommand())
            {
                yield return cmd;
            }
        }
    }
}
