using libbluray.bdnav.Clpi;
using libbluray.bdnav.Mpls;
using MakeMKV_Title_Decoder.Data;
using MakeMKV_Title_Decoder.libs.MkvToolNix.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.libs.MkvToolNix
{
    internal interface MkvToolNixData
    {
        bool Parse(IEnumerable<string> std, IProgress<SimpleProgress>? progress = null, object? tag = null);
    }


    // TODO use Utils.CommandLineInterface????
    internal static class MkvToolNixInterface
    {

        private static Process? RunCommandForceArg(string exeName, string args)
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase).Substring(6);

            try
            {
                var startInfo = new ProcessStartInfo();
                startInfo.FileName = Path.Combine(path, "libs", "MkvToolNix", "lib", exeName);
                startInfo.CreateNoWindow = true;
                startInfo.UseShellExecute = false;
                startInfo.RedirectStandardOutput = true;
                startInfo.Arguments = args;

                return Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to read MakeMKV: {ex.Message}", "Failed to read MakeMKV", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        private static Process? RunCommand(string exeName, params string[] args)
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase).Substring(6);

            try
            {
                var startInfo = new ProcessStartInfo();
                startInfo.FileName = Path.Combine(path, "libs", "MkvToolNix", "lib", exeName);
                startInfo.CreateNoWindow = true;
                startInfo.UseShellExecute = false;
                startInfo.RedirectStandardOutput = true;

                foreach (string arg in args)
                {
                    startInfo.ArgumentList.Add(arg);
                }

                return Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to read MakeMKV: {ex.Message}", "Failed to read MakeMKV", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        private static IEnumerable<string> ReadAllStdOut(Process process)
        {
            string? line = process.StandardOutput.ReadLine();
            while (line != null)
            {
                yield return line;
                line = process.StandardOutput.ReadLine();
            }
        }

        // TODO just pass the process as a parameter
        private static bool ParseCommandForceArgs<T>(T data, IProgress<SimpleProgress>? progress, string exeName, string args, object? tag) where T : class, MkvToolNixData
        {
            var process = RunCommandForceArg(exeName, args);
            if (process == null)
            {
                return false;
            }

            try
            {
                bool result = data.Parse(ReadAllStdOut(process), progress, tag);
                process.WaitForExit();
                return result;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        private static bool ParseCommand<T>(T data, IProgress<SimpleProgress>? progress, string exeName, params string[] args) where T : class, MkvToolNixData
        {
            var process = RunCommand(exeName, args);
            if (process == null)
            {
                return false;
            }

            try
            {
                bool result = data.Parse(ReadAllStdOut(process), progress);
                process.WaitForExit();
                return result;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public static MkvInfo? ReadInfo(string filePath)
        {
            return null; //ParseCommand<MkvInfo>("mkvinfo.exe", filePath);
        }

        public static MkvMergeID? Identify(string root, string directory, string fileName)
        {
            MkvMergeID data = new MkvMergeID(root, directory, fileName);
            bool result = ParseCommand(data, null, "mkvmerge.exe", "--identify", "--identification-format", "json", Path.Combine(root, directory, fileName));
            if (result)
            {
                return data;
            }
            else
            {
                return null;
            }
        }

        public static void Merge(LoadedDisc disc, Playlist playlist, string outputFile, IProgress<SimpleProgress>? progress = null, SimpleProgress? totalProgress = null)
        {
            TestData data = new();


            //string args = $"--title \"Test Title\" -o \"{outputFile}\" --track-name 5:\"Director Commentary\" {Path.Combine(disc.RootPath, "BDMV", "STREAM", "00001.m2ts")}";
            string cmd = GetMergeCommand(disc, playlist, outputFile);
            Console.WriteLine(cmd); // TODO put in ParseCommandForceArgs?
            bool result = ParseCommandForceArgs(data, progress, "mkvmerge.exe", cmd, totalProgress);
        }

        public static void MergeAsync(LoadedDisc disc, Playlist playlist, string outputFile, SimpleProgress? totalProgress = null)
        {
            var progressForm = new TaskProgressViewer<Task, SimpleProgress>(
                (progress) =>
                {
                    return Task.Run(() => Merge(disc, playlist, outputFile, progress, totalProgress));
                }
            );
            progressForm.ShowDialog();
        }

        private static string GetMergeCommand(LoadedDisc disc, Playlist playlist, string outputFile)
        {
            List<string> args = new();
            List<MkvAppend> appendedTracks = new();
            List<PlaylistSource> allFiles = new();
            Dictionary<PlaylistID, PlaylistSource> lookup = new();
            FindFiles(playlist, allFiles, lookup);

            args.Add($"-o \"{outputFile}\"");

            foreach(var sourceFile in playlist.SourceFiles.WithIndex())
            {
                args.Add(GetMergeCommand(disc, playlist, new PlaylistID(sourceFile.Index), sourceFile.Value, appendedTracks, false, allFiles, lookup));
            }

            if (playlist.Title != null)
            {
                args.Add($"--title \"{playlist.Title}\"");
            }

            // Determine source track order
            List<PlaylistID> trackOrder = new();
            foreach(var source in playlist.Tracks)
            {
                trackOrder.Add(new PlaylistID(
                    allFiles.IndexOf(lookup[new PlaylistID((int)source.SourceFileIndex, (int)source.AppendedFileIndex)]),
                    (int)FindTrack(lookup, disc, source)?.MkvMergeID
                ));
            }
            args.Add($"--track-order");
            args.Add(string.Join(",", trackOrder.Select(x => $"{x.SourceIndex}:{x.AppendedIndex}")));

            if (appendedTracks.Count > 0)
            {
                args.Add($"--append-to {string.Join(",", appendedTracks)}");
            }

            string fullCommand = string.Join(" ", args);
            return fullCommand;
        }

        private struct PlaylistID : IEquatable<PlaylistID> {
            public int SourceIndex;
            public int AppendedIndex;

            public PlaylistID(int src = -1, int append = -1) {
                SourceIndex = src;
                AppendedIndex = append;
            }

            public static bool operator ==(PlaylistID left, PlaylistID right) {
                return left.Equals(right);
            }

            public static bool operator !=(PlaylistID left, PlaylistID right) {
                return !left.Equals(right);
            }

            public bool Equals(PlaylistID other) {
                return (SourceIndex == other.SourceIndex) && (AppendedIndex == other.AppendedIndex);
            }

            public override bool Equals([NotNullWhen(true)] object? obj) {
                if (obj == null) return false;
                if (obj is PlaylistID other)
                {
                    return Equals(other);
                } else
                {
                    return false;
                }
            } 

            public override int GetHashCode() {
                unchecked
                {
                    int hash = 23;
                    hash = hash * 31 + SourceIndex;
                    hash = hash * 31 + AppendedIndex;
                    return hash;
                }
            }
        }

        private static void FindFiles(Playlist playlist, List<PlaylistSource> allFiles, Dictionary<PlaylistID, PlaylistSource> lookup)
        {
            allFiles.Clear();
            lookup.Clear();
            foreach (var sourcePair in playlist.SourceFiles.WithIndex())
            {
                allFiles.Add(sourcePair.Value);
                lookup[new PlaylistID(sourcePair.Index)] = sourcePair.Value;

                foreach (var appendPair in sourcePair.Value.AppendedFiles.WithIndex())
                {
                    allFiles.Add(appendPair.Value);
                    lookup[new PlaylistID(sourcePair.Index, appendPair.Index)] = appendPair.Value;
                }
            }
        }

        // TODO use the json file option to prevent console issues?
        private static string GetMergeCommand(LoadedDisc disc, Playlist playlist, PlaylistID fileID, PlaylistSource file, List<MkvAppend> appendedTracks, bool isAppended, List<PlaylistSource> allFiles, Dictionary<PlaylistID, PlaylistSource> lookup)
        {
            List<string> args = new();
            List<long> audioTracks = new();
            List<long> videoTracks = new();
            List<long> subtitleTracks = new();

            if (isAppended)
            {
                args.Add("+");
            }

            Action<PlaylistTrack, long> AddTrack = (track, delay) => {
                if (track.Copy)
                {
                    LoadedTrack? source = FindTrack(lookup, disc, track);
                    if (source == null) return; // TODO warning

                    switch(source.Data.Type)
                    {
                        case MkvTrackType.Audio:
                            audioTracks.Add(source.MkvMergeID); break;
                        case MkvTrackType.Video:
                            videoTracks.Add(source.MkvMergeID); break;
                        case MkvTrackType.Subtitles:
                            subtitleTracks.Add(source.MkvMergeID); break;
                        default:
                            return; // TODO warning
                    }

                    if (source.Rename.Name != null) args.Add($"--track-name {source.MkvMergeID}:\"{source.Rename.Name}\"");
                    if (source.Rename.CommentaryFlag != null) args.Add($"--commentary-flag {source.MkvMergeID}:{(source.Rename.CommentaryFlag == true ? 1 : 0)}");
                    if (delay > 0) args.Add($"-y {source.MkvMergeID}:{delay}");
                }
            };

            foreach (var sourcePair in playlist.Tracks.WithIndex())
            {
                PlaylistID sourceID = new PlaylistID((int)sourcePair.Value.SourceFileIndex, (int)sourcePair.Value.AppendedFileIndex);

                if (sourceID == fileID && sourcePair.Value.Copy)
                {
                    AddTrack(sourcePair.Value, 0);
                }

                long totalDelay = 0;
                PlaylistTrack? parentTrack = sourcePair.Value;
                if (sourcePair.Value.Copy == false)
                {
                    LoadedStream? stream = FindSource(disc, lookup[sourceID]);
                    totalDelay = (stream == null) ? 0 : (long)stream.Duration.TotalMilliseconds;
                    parentTrack = null;
                } else
                {
                    foreach(var appendedPair in sourcePair.Value.AppendedTracks.WithIndex())
                    {
                        PlaylistID appendedID = new PlaylistID((int)appendedPair.Value.SourceFileIndex, (int)appendedPair.Value.AppendedFileIndex);

                        if (appendedID == fileID && appendedPair.Value.Copy)
                        {
                            if (appendedPair.Value.Delay == null)
                            {
                                AddTrack(appendedPair.Value, totalDelay);
                                MkvAppend append = new();
                                append.SrcFile = allFiles.IndexOf(file);
                                append.SrcTrack = (int)(FindTrack(lookup, disc, appendedPair.Value)?.MkvMergeID ?? -1);
                                append.DstFile = allFiles.IndexOf(lookup[new PlaylistID((int)parentTrack.SourceFileIndex, (int)parentTrack.AppendedFileIndex)]);
                                append.DstTrack = (int)(FindTrack(lookup, disc, parentTrack)?.MkvMergeID ?? -1);
                                appendedTracks.Add(append);
                            }
                        }

                        if (appendedPair.Value.Copy && appendedPair.Value.Delay == null)
                        {
                            totalDelay = 0;
                            parentTrack = appendedPair.Value;
                        } else
                        {
                            if (appendedPair.Value.Delay == null)
                            {
                                LoadedStream? stream2 = FindSource(disc, lookup[appendedID]);
                                totalDelay += (stream2 == null) ? 0 : (long)stream2.Duration.TotalMilliseconds;
                            } else
                            {
                                var delay = appendedPair.Value.Delay;
                                if (delay.DelayType == DelayType.Source)
                                {
                                    LoadedStream? stream2 = FindSource(disc, lookup[new PlaylistID((int)delay.SourceFileIndex, (int)delay.AppendedFileIndex)]);
                                    totalDelay += (stream2 == null) ? 0 : (long)stream2.Duration.TotalMilliseconds;
                                } else if (delay.DelayType == DelayType.Delay)
                                {
                                    totalDelay += delay.Milliseconds;
                                } else
                                {
                                    throw new Exception();
                                }
                            }
                        }
                    }
                }
            }

            if (audioTracks.Count > 0)
                args.Add($"-a {string.Join(",", audioTracks)}");
            else
                args.Add($"-A");

            if (videoTracks.Count > 0)
                args.Add($"-d {string.Join(",", videoTracks)}");
            else
                args.Add($"-D");

            if (subtitleTracks.Count > 0)
                args.Add($"-s {string.Join(",", subtitleTracks)}");
            else
                args.Add($"-S");

            args.Add(FindSource(disc, file)?.Data.GetFullPath(disc.Data));

            if (!isAppended)
            {
                foreach(var appendedFile in file.AppendedFiles.WithIndex())
                {
                    args.Add(GetMergeCommand(disc, playlist, new PlaylistID(fileID.SourceIndex, appendedFile.Index), appendedFile.Value, appendedTracks, true, allFiles, lookup));
                }
            }

            return string.Join(" ", args);
        }

        private static LoadedStream? FindSource(LoadedDisc disc, PlaylistSource stream)
        {
            return disc.Streams[(int)stream.StreamIndex];
        }

        private static LoadedTrack? FindTrack(Dictionary<PlaylistID, PlaylistSource> lookup, LoadedDisc disc, PlaylistTrack track)
        {
            PlaylistID ID = new((int)track.SourceFileIndex, (int)track.AppendedFileIndex);
            PlaylistSource source = lookup[ID];

            return disc.Streams[(int)source.StreamIndex].Tracks[(int)track.TrackIndex];
        }
    }

    internal struct MkvAppend
    {
        public int SrcFile; // Track that is being appended
        public int SrcTrack;
        public int DstFile; // Track that the source will be appended to
        public int DstTrack;

        public override string ToString()
        {
            return $"{SrcFile}:{SrcTrack}:{DstFile}:{DstTrack}";
        }
    }

    // TODO rename to MultiplexData? MergeData?
    public class TestData : MkvToolNixData
    {

        public List<string> Errors = new();
        public bool FoundSuccess = false;

        public bool Parse(IEnumerable<string> std, IProgress<SimpleProgress>? progress = null, object? tag = null)
        {
            SimpleProgress? baseProgress = null;
            if (tag != null && tag is SimpleProgress p)
            {
                baseProgress = p;
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            foreach (var str in std)
            {
                Console.WriteLine(str);

                if (progress != null && str.StartsWith("Progress: ") && str.EndsWith("%"))
                {
                    string progText = str.Substring("Progress: ".Length).TrimEnd('%');

                    uint prog;
                    if (uint.TryParse(progText, out prog))
                    {
                        if (baseProgress != null)
                        {
                            progress.Report(new SimpleProgress(
                                prog,
                                100,
                                baseProgress.Value.Total * 100 + prog,
                                baseProgress.Value.TotalMax * 100
                            ));
                        }
                        else
                        {
                            progress.Report(new SimpleProgress(prog, 100));
                        }
                    }
                }
                if (str.StartsWith("Error: "))
                {
                    Errors.Add(str.Substring("Error: ".Length));
                }
                if (str.StartsWith("Multiplexing took"))
                {
                    FoundSuccess = true;
                }
            }

            if (Errors.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                foreach (var err in Errors)
                {
                    Console.WriteLine(err);
                }
            }
            else if (FoundSuccess != true)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("No errors detected, but could not tell if operation was successfull.");
            }

            Console.ResetColor();
            return true;
        }
    }
}
