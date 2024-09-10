using libbluray.bdnav.Clpi;
using libbluray.bdnav.Mpls;
using MakeMKV_Title_Decoder.Data;
using MakeMKV_Title_Decoder.MkvToolNix.Data;
using MakeMKV_Title_Decoder.MkvToolNix.MkvToolNix;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.MkvToolNix
{
    internal interface MkvToolNixData {
        bool Parse(IEnumerable<string> std, IProgress<SimpleProgress>? progress = null, object? tag = null);
    }


    // TODO use Utils.CommandLineInterface????
    internal static class MkvToolNixInterface {

        private static Process? RunCommandForceArg(string exeName, string args) {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase).Substring(6);

            try
            {
                var startInfo = new ProcessStartInfo();
                startInfo.FileName = Path.Combine(path, "lib", "mkvtoolnix", exeName);
                startInfo.CreateNoWindow = true;
                startInfo.UseShellExecute = false;
                startInfo.RedirectStandardOutput = true;
                startInfo.Arguments = args;

                return Process.Start(startInfo);
            } catch (Exception ex)
            {
                MessageBox.Show($"Failed to read MakeMKV: {ex.Message}", "Failed to read MakeMKV", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        private static Process? RunCommand(string exeName, params string[] args) {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase).Substring(6);

            try
            {
                var startInfo = new ProcessStartInfo();
                startInfo.FileName = Path.Combine(path, "lib", "mkvtoolnix", exeName);
                startInfo.CreateNoWindow = true;
                startInfo.UseShellExecute = false;
                startInfo.RedirectStandardOutput = true;

                foreach (string arg in args)
                {
                    startInfo.ArgumentList.Add(arg);
                }

                return Process.Start(startInfo);
            } catch (Exception ex)
            {
                MessageBox.Show($"Failed to read MakeMKV: {ex.Message}", "Failed to read MakeMKV", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        private static IEnumerable<string> ReadAllStdOut(Process process) {
            string? line = process.StandardOutput.ReadLine();
            while(line != null)
            {
                yield return line;
                line = process.StandardOutput.ReadLine();
            }
        }

        // TODO just pass the process as a parameter
        private static bool ParseCommandForceArgs<T>(T data, IProgress<SimpleProgress>? progress, string exeName, string args, object? tag) where T : class, MkvToolNixData {
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
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        private static bool ParseCommand<T>(T data, IProgress<SimpleProgress>? progress, string exeName, params string[] args) where T : class, MkvToolNixData {
            var process = RunCommand(exeName, args);
            if(process == null)
            {
                return false;
            }

            try
            {
                bool result = data.Parse(ReadAllStdOut(process), progress);
                process.WaitForExit();
                return result;
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public static MkvInfo? ReadInfo(string filePath) {
            return null; //ParseCommand<MkvInfo>("mkvinfo.exe", filePath);
        }

        public static MkvMergeID? Identify(string root, string directory, string fileName) {
            MkvMergeID data = new MkvMergeID(root, directory, fileName);
            bool result = ParseCommand<MkvMergeID>(data, null, "mkvmerge.exe", "--identify", "--identification-format", "json", Path.Combine(root, directory, fileName));
            if (result)
            {
                return data;
            } else
            {
                return null;
            }
        }

        public static void Merge(MkvToolNixDisc disc, Playlist playlist, string outputFile, IProgress<SimpleProgress>? progress = null, SimpleProgress? totalProgress = null) {
            TestData data = new();


            //string args = $"--title \"Test Title\" -o \"{outputFile}\" --track-name 5:\"Director Commentary\" {Path.Combine(disc.RootPath, "BDMV", "STREAM", "00001.m2ts")}";
            string cmd = GetMergeCommand(disc, playlist, outputFile);
            Console.WriteLine(cmd); // TODO put in ParseCommandForceArgs?
            bool result = ParseCommandForceArgs<TestData>(data, progress, "mkvmerge.exe", cmd, totalProgress);
        }

        public static void MergeAsync(MkvToolNixDisc disc, Playlist playlist, string outputFile, SimpleProgress? totalProgress = null) {
            var progressForm = new TaskProgressViewer<Task, SimpleProgress>(
                (IProgress<SimpleProgress> progress) =>
                {
                    return Task.Run(() => Merge(disc, playlist, outputFile, progress, totalProgress));
                }
            );
            progressForm.ShowDialog();
        }

        private static string GetMergeCommand(MkvToolNixDisc disc, Playlist playlist, string outputFile) {
            List<string> args = new();
            List<MkvAppend> appendedTracks = new();
            List<PlaylistFile> allFiles = FindFiles(playlist).ToList();
            IEnumerable<string> files = playlist.Files.Select(x => GetMergeCommand(disc, playlist, x, appendedTracks, false, allFiles));

            args.Add($"-o \"{outputFile}\"");
            args.AddRange(files);

            if (playlist.Title != null)
            {
                args.Add($"--title \"{playlist.Title}\"");
            }

            if (playlist.TrackOrder != null)
            {
                args.Add($"--track-order");
                args.Add(string.Join(",", playlist.TrackOrder.Select(x => $"{x.FileIndex}:{x.TrackIndex}")));
            }

            if (appendedTracks.Count > 0)
            {
                args.Add($"--append-to {string.Join(",", appendedTracks)}");
            }

            string fullCommand = string.Join(" ", args);
            return fullCommand;
        }

        private static IEnumerable<PlaylistFile> FindFiles(Playlist playlist) {
            foreach(var file in playlist.Files)
            {
                yield return file;
                foreach(var append in file.AppendedFiles)
                {
                    yield return append;
                }
            }
        }

        // TODO use the json file option to prevent console issues?
        private static string GetMergeCommand(MkvToolNixDisc disc, Playlist playlist, PlaylistFile file, List<MkvAppend> appendedTracks, bool isAppended, List<PlaylistFile> allFiles) {
            List<string> args = new();
            List<long> audioTracks = new();
            List<long> videoTracks = new();
            List<long> subtitleTracks = new();

            if (isAppended)
            {
                args.Add("+");
            }

            foreach(PlaylistTrack track in file.Tracks)
            {
                if (track.Copy ?? false)
                {
                    MkvTrack? source = FindTrack(disc, file, track);
                    if (source == null) continue;// TODO warning

                    switch (source.Type)
                    {
                        case MkvTrackType.Audio:
                            audioTracks.Add(source.ID);
                            break;
                        case MkvTrackType.Video:
                            videoTracks.Add(source.ID);
                            break;
                        case MkvTrackType.Subtitles:
                            subtitleTracks.Add(source.ID);
                            break;
                        default:
                            continue; // TODO warning
                    }

                    if (track.Name != null) args.Add($"--track-name {source.ID}:\"{track.Name}\"");
                    if (track.Commentary != null) args.Add($"--commentary-flag {source.ID}:{(track.Commentary == true ? 1 : 0)}");
                    if (track.Sync.Count > 0)
                    {
                        TimeSpan delay = new();
                        foreach(var sync in track.Sync)
                        {
                            //MkvTrack? syncTrack = FindTrack(disc, playlist.Files[sync.FileID.Value], playlist.Files[sync.FileID.Value].Tracks[sync.TrackID.Value]);
                            MkvMergeID syncFile = FindSource(disc, allFiles[sync.FileIndex.Value]);
                            if (syncFile != null && syncFile.Container?.Properties?.Duration != null)
                            {
                                delay += syncFile.Container.Properties.Duration.Value;
                            } else
                            {
                                // Try using libbluray
                                // TODO prevent parsing multiple times
                                ClpiFile? clipFile = ClpiFile.Parse(Path.Combine(disc.RootPath, "BDMV", "CLIPINF", $"{Path.GetFileNameWithoutExtension(syncFile.FileName)}.clpi"));
                                if (clipFile == null)
                                {
                                    // TODO adjust clpi parsing to ONLY parse durations??
                                    throw new Exception("Failed to calculate duration");
                                }
                                //TimeSpan clipLength = clipFile.sequence.atc_seq.SelectMany<ClipStcSequence,TimeSpan>(x => x.stc_seq.Select(y => y.Length).Aggregate((a, b) => a + b));
                                TimeSpan clipLength = clipFile.sequence.atc_seq.SelectMany(x => x.stc_seq.Select(y => y.Length)).Aggregate((a, b) => a + b);
                                //Console.WriteLine("clipLength: " + clipLength.TotalMilliseconds + " ms");
                                delay += clipLength;
                            }
                        }

                        args.Add($"-y {source.ID}:{(long)delay.TotalMilliseconds}");
                    }

                    if (track.AppendedTo != null)
                    {
                        MkvAppend append = new();
                        append.SrcFile = allFiles.IndexOf(file);
                        append.SrcTrack = (int)track.ID.Value;
                        append.DstFile = track.AppendedTo.FileIndex.Value;
                        append.DstTrack = track.AppendedTo.TrackIndex.Value;
                        appendedTracks.Add(append);
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

            args.Add(Path.Combine(disc.RootPath, file.Source));

            if (!isAppended)
            {
                args.AddRange(file.AppendedFiles.Select(x => GetMergeCommand(disc, playlist, x, appendedTracks, true, allFiles)));
            }

            return string.Join(" ", args);
        }

        private static MkvMergeID? FindSource(MkvToolNixDisc disc, PlaylistFile file) {
            // TODO store results for faster lookup and/or better searching
            string filePath = file.Source;
            foreach (MkvMergeID stream in disc.Streams)
            {
                if (stream.GetRelativePath() == filePath)
                {
                    return stream;
                }
            }

            return null;
        }

        private static MkvTrack? FindTrack(MkvToolNixDisc disc, PlaylistFile file, PlaylistTrack playlist) {
            // TODO store results for faster lookup and/or better searching
            string filePath = file.Source;
            foreach(MkvMergeID stream in disc.Streams)
            {
                if (stream.GetRelativePath() == filePath)
                {
                    foreach(MkvTrack track in stream.Tracks)
                    {
                        if (track.ID == playlist.ID)
                        {
                            return track;
                        }
                    }
                    break;
                }
            }

            return null;
        }
    }

    internal struct MkvAppend {
        public int SrcFile; // Track that is being appended
        public int SrcTrack;
        public int DstFile; // Track that the source will be appended to
        public int DstTrack;

        public override string ToString() {
            return $"{SrcFile}:{SrcTrack}:{DstFile}:{DstTrack}";
        }
    }

    // TODO rename to MultiplexData? MergeData?
    public class TestData : MkvToolNixData {

        public List<string> Errors = new();
        public bool FoundSuccess = false;

        public bool Parse(IEnumerable<string> std, IProgress<SimpleProgress>? progress = null, object? tag = null) {
            SimpleProgress? baseProgress = null;
            if (tag != null && tag is SimpleProgress p)
            {
                baseProgress = p;
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            foreach(var str in std)
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
                                (baseProgress.Value.Total * 100) + prog,
                                baseProgress.Value.TotalMax * 100
                            ));
                        } else
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
                    this.FoundSuccess = true;
                }
            }

            if (this.Errors.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                foreach(var err in Errors)
                {
                    Console.WriteLine(err);
                }
            } else if (this.FoundSuccess != true)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("No errors detected, but could not tell if operation was successfull.");
            }

            Console.ResetColor();
            return true;
        }
    }
}
