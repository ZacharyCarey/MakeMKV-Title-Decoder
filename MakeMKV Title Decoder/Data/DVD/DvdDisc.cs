using MkvToolNix.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
using PgcDemuxLib;
using PgcDemuxLib.Data;
using System.Runtime.CompilerServices;
using PgcDemuxLib.Data.VMG;
using FFMpeg_Wrapper.ffprobe;
using FFMpeg_Wrapper;
using System.Diagnostics;
using FFMpeg_Wrapper.Codecs;
using FFMpeg_Wrapper.Filters;
using PgcDemuxLib.Data.VTS;
using Iso639;

namespace MakeMKV_Title_Decoder.Data.DVD
{
    public class DvdDisc : LoadedDisc
    {
        private List<DiscPlaylist> playlists;
        private Dvd dvd;

        public override bool SupportsDeinterlacing => true;

        private DvdDisc(Dvd dvd, string root, List<LoadedStream> streams, string? title, long? numSets, long? setNum, List<DiscPlaylist> playlists) : base(root, title, numSets, setNum, streams)
        {
            this.playlists = playlists;
            this.dvd = dvd;
        }

        public override bool ForceVlcTrackIndex => true;

        public static DvdDisc? Open(string root, IProgress<SimpleProgress>? progress = null)
        {
            string streamDir = "VIDEO_TS";
            string demuxDir = Path.Combine(streamDir, "demux");
            SimpleProgress currentProgress = new();

            Dvd? dvd = Dvd.ParseFolder(root);
            if (dvd == null) return null;

            string fullDemuxPath = Path.Combine(root, demuxDir);
            if (!Directory.Exists(fullDemuxPath))
            {
                if (MessageBox.Show("The stream files must be extracted from the DVD. This will create large temporary files. Continue?", "Extract files?", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
                {
                    return null;
                }

                try
                {
                    Directory.CreateDirectory(fullDemuxPath);
                }catch(Exception)
                {
                    return null;
                }

                bool success;
                try
                {
                    success = DemuxAllCells(dvd, fullDemuxPath, progress);
                } catch (Exception e)
                {
                    success = false;
                }

                if (!success)
                {
                    Directory.Delete(fullDemuxPath, true);
                    return null;
                }
            }

            string[] streamFilePaths = Directory.GetFiles(fullDemuxPath);
            currentProgress.TotalMax = (uint)streamFilePaths.Length;
            progress?.Report(currentProgress);

            List<LoadedStream> streams = new();
            for (int i = 0; i < streamFilePaths.Length; i++)
            {
                string fileName = Path.GetFileName(streamFilePaths[i]);
                MkvMergeID? id = ParseIdentify(root, demuxDir, fileName);
                bool deinterlaced = false;

                string name = Path.GetFileNameWithoutExtension(fileName);
                if (name.EndsWith(".dint"))
                {
                    deinterlaced = true;
                    fileName = name[..^5] + Path.GetExtension(fileName);
                }

                if (id != null)
                {
                    TimeSpan duration = new();
                    try
                    {
                        //VTS-{this.TitleSet:00}_Menu_VID-{vobID:X4}_CID-{cellID:X2}.VOB
                        //VTS-{this.TitleSet:00}_VID-{vobID:X4}_CID-{cellID:X2}.VOB
                        CellID cellID;
                        if (!Dvd.TryParseSourceFileName(fileName, out cellID)) throw new Exception();

                        IEnumerable<PGC> pgcs;
                        if (cellID.VTS == 0)
                        {
                            pgcs = (dvd.VMG.MenuProgramChainTable?.All?.SelectMany(x => x.All) ?? new List<PGC>().AsEnumerable()).Append(dvd.VMG.FirstPlayPGC);
                        } else
                        {
                            pgcs = (dvd.TitleSets[cellID.VTS - 1].MenuProgramChainTable?.All?.SelectMany(x => x.All) ?? new List<PGC>().AsEnumerable()).Concat(dvd.TitleSets[cellID.VTS - 1].TitleProgramChainTable?.All ?? new List<PGC>().AsEnumerable());
                        }
                        IEnumerable<CellInfo> cells = pgcs.SelectMany(x => x.CellInfo);
                        duration = cells.Where(cell => cell.VobID == cellID.VID && cell.CellID == cellID.CID).First().Duration;
                    }catch(Exception)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Failed to read DVD cell duration.");
                        Console.ResetColor();
                    }
                    DvdStream stream = new DvdStream(root, Path.Combine(demuxDir, fileName), id, duration);
                    stream.RenameData.Deinterlaced = deinterlaced;
                    streams.Add(stream);
                }
                currentProgress.Total++;
                progress?.Report(currentProgress);
            }

            List<DiscPlaylist> playlists = new();
            if (dvd.VMG.FirstPlayPGC != null)
            {
                playlists.AddRange(GetPlaylistsFromPGC(dvd.VMG.FirstPlayPGC, "First Play PGC", false, dvd.VMG.TitleSet, demuxDir));
            }
            if (dvd.VMG.MenuProgramChainTable != null)
            {
                foreach(var pgc in dvd.VMG.MenuProgramChainTable.All.SelectMany(x => x.All))
                {
                    playlists.AddRange(GetPlaylistsFromPGC(pgc, $"VTS{dvd.VMG.TitleSet:00} Menu PGC{pgc.ID:00}", true, dvd.VMG.TitleSet, demuxDir));
                }
            }

            foreach(var vts in dvd.TitleSets)
            {
                if (vts.MenuProgramChainTable != null)
                {
                    foreach (var pgc in vts.MenuProgramChainTable.All.SelectMany(x => x.All))
                    {
                        playlists.AddRange(GetPlaylistsFromPGC(pgc, $"VTS{vts.TitleSet:00} Menu PGC{pgc.ID:00}", true, vts.TitleSet, demuxDir));
                    }
                }
                if (vts.TitleProgramChainTable != null)
                {
                    foreach (var pgc in vts.TitleProgramChainTable.All)
                    {
                        bool isOnlyAngle = (pgc.NumberOfAngles == 1);
                        for (int i = 1; i <= pgc.NumberOfAngles; i++)
                        {
                            playlists.AddRange(GetPlaylistsFromPGC(pgc, $"VTS{vts.TitleSet:00} PGC{pgc.ID:00}", false, vts.TitleSet, demuxDir));
                        }
                    }
                }
            }

            return new DvdDisc(dvd, root, streams, $"DVD-{new DirectoryInfo(root).Name}", dvd.VMG.NumberOfVolumes, dvd.VMG.VolumeNumber, playlists);
        }

        private static bool DemuxAllCells(Dvd dvd, string demuxFolder, IProgress<SimpleProgress>? progress = null) {
            // Get a count of the number of cells we need to extract
            List<string> vobFiles = dvd.GetSourceFiles().ToList();
            SimpleProgress currentProgress = new();
            currentProgress.TotalMax = (uint)vobFiles.Count;
            currentProgress.TotalMax *= 2; // One for demuxing the mpg stream, a second one for transcoding to mp4
            progress?.Report(currentProgress);

            bool? autoDeinterlace = null;
            foreach((int index, var vobFile) in vobFiles.WithIndex())
            {
                currentProgress.Total = (uint)index * 2;
                string? generatedFile = GenerateSourceFile(dvd, vobFile, ref autoDeinterlace, progress, currentProgress);
                if (generatedFile == null)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// if deinterlace is null, the user will be prompted if they want to deinterlace now. 
        /// Giving a value, false or true, will not ask the user and simply apply those settings
        /// </summary>
        /// <returns></returns>
        private static string? GenerateSourceFile(Dvd dvd, string sourceName, ref bool? deinterlace, IProgress<SimpleProgress>? progress, SimpleProgress currentProgress) {
            Action<string> PrintWarn = (string error) =>
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(error);
                Console.ResetColor();
            };
            Action<string> PrintError = (string error) =>
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(error);
                Console.ResetColor();
            };

            // Prepare folders
            string demuxDir = Path.Combine(dvd.Folder, "VIDEO_TS", "demux");
            string? logDir = Path.Combine(demuxDir, "logs");
            try
            {
                var result = Directory.CreateDirectory(logDir);
                if (!result.Exists) throw new Exception();
            } catch (Exception)
            {
                PrintWarn("Failed to create ffmpeg logs folder!");
                logDir = null;
            }

            // Find local exe files
            string? ffprobeEXE = FileUtils.GetFFProbeExe();
            string? ffmpegEXE = FileUtils.GetFFMpegExe();
            if (ffprobeEXE == null || ffmpegEXE == null)
            {
                PrintError("FATAL ERROR: Failed to find local .exe files.");
                return null;
            }

            FFProbe ffprobe = new FFProbe(ffprobeEXE);
            FFMpeg ffmpeg = new FFMpeg(ffmpegEXE);

            // Extract the .VOB file from the DVD
            DemuxResult demuxResult = dvd.DemuxSourceFile(demuxDir, sourceName, progress, currentProgress);
            if (demuxResult.Successful == false)
            {
                PrintError($"Failed to demux source VOB file: \"{sourceName}\"");
                return null;
            }
            currentProgress.Total++;
            progress?.Report(currentProgress);

            // Attempt to auto-detect interlaced video
            bool deinterlaceSetting = false;
            MediaAnalysis? info = ffprobe.Analyse(Path.Combine(demuxDir, sourceName));
            if (info == null)
            {
                PrintError($"Failed to analyze file: \"{sourceName}\"");
                return null;
            }
            if (!string.IsNullOrWhiteSpace(info.PrimaryVideoStream?.FieldOrder))
            {
                bool isInterlaced = false;
                switch (info.PrimaryVideoStream.FieldOrder)
                {
                    case "tt":
                    case "bb":
                    case "tb":
                    case "bt":
                        isInterlaced = true;
                        break;
                    default:
                        break;
                }

                if (isInterlaced)
                {
                    if (deinterlace == null)
                    {
                        var user = MessageBox.Show("A stream was detected to be interlaced. Would you like to deinterlace the entire disc? (On streams that is can be detected only)", "Deinterlace?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (user == DialogResult.Yes)
                        {
                            deinterlace = true;
                        } else
                        {
                            deinterlace = false;
                        }
                    }

                    if (deinterlace == true)
                    {
                        deinterlaceSetting = true;
                    }
                }
            }

            // Transcode the source file to MP4 to resolve streaming errors (a lot of devices seem to struggle with mpeg these days)
            // Also fixes interlacing issues
            string? transcodedFile = TranscodeToMP4(ffmpeg, info, demuxResult, dvd, sourceName, demuxDir, logDir, deinterlaceSetting, PrintError, progress, currentProgress);
            if (transcodedFile == null)
            {
                PrintError($"Failed to transcode VOB file to mp4: {sourceName}");
                return null;
            }
            currentProgress.Total++;
            progress?.Report(currentProgress);

            // Cleanup
            try
            {
                File.Delete(Path.Combine(demuxDir, sourceName));
            } catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Failed to delete temporary file: " + sourceName);
                Console.ResetColor();
            }

            return transcodedFile;
        }

        private static string? TranscodeToMP4(FFMpeg ffmpeg, MediaAnalysis info, DemuxResult demuxResult, Dvd dvd, string vobFile, string demuxFolder, string? logsFolder, bool deinterlace, Action<string> PrintError, IProgress<SimpleProgress>? progress, SimpleProgress currentProgress) {
            // Get info from the DVD about this file
            CellID cellID;
            if (!Dvd.TryParseSourceFileName(vobFile, out cellID))
            {
                PrintError("Failed to parse source folder name.");
                return null;
            }

            // Get the corresponding IFO file and audio attributes for that file
            ReadOnlyArray<VTS_AudioAttributes> audioAttribs;
            if (cellID.VTS == 0)
            {
                audioAttribs = dvd.VMG.MenuAudioAttributes;
            } else
            {
                VtsIfo ifo = dvd.TitleSets[cellID.VTS - 1];
                audioAttribs = (cellID.IsMenu ? ifo.MenuAudioAttributes : ifo.TitleSetAudioAttributes);
            }

            // Transcode the raw source file to mp4, deinterlacing if needed
            string outputFile = Path.Combine(demuxFolder, $"{Path.GetFileNameWithoutExtension(vobFile)}{(deinterlace ? ".dint" : "")}.mp4");

            // Add video options with optional deinterlacing
            VideoStreamOptions videoOptions = new() {
                Codec = Codecs.Libx264.SetCRF(16)
            };
            if (deinterlace) videoOptions.Filter = Filters.Bwdif;
            var ffmpegArgs = ffmpeg.Transcode(outputFile)
                .AddVideoStreams(info, videoOptions);

            // Audio streams must be added in a particular order
            foreach ((int index, int audioStreamID) in demuxResult.AudioStreamIDs.Order().WithIndex())
            {
                var audioAttrib = audioAttribs[index];
                Language? lang = null;
                if (audioAttrib.LanguageCode != null)
                {
                    lang = Language.FromPart1(audioAttrib.LanguageCode);
                }

                ffmpegArgs.AddAudioStream(
                    info.AudioStreams[demuxResult.AudioStreamIDs.IndexOf(audioStreamID)],
                    new AudioStreamOptions() {
                        Codec = Codecs.AAC,
                        Language = lang
                    }
                );
            }

            // Add any remaining streams we want to copy
            ffmpegArgs.AddSubtitleStreams(info.SubtitleStreams, null);

            // Run the FFMpeg command
            var cmd = ffmpegArgs
                .NotifyOnProgress((double percent) => {
                    currentProgress.Current = (uint)percent;
                    currentProgress.CurrentMax = 100;
                    progress?.Report(currentProgress);
                })
                .SetOverwrite(false); // In case something went wrong, throw an error if the output already exists

            if (logsFolder != null)
            {
                cmd.SetLogPath(Path.Combine(logsFolder, $"FFMpeg_log_{Path.GetFileNameWithoutExtension(outputFile)}.txt"));
            }

            return cmd.Run() ? outputFile : null;
        }

        // Used to re-transcode the source file if deinterlace settings are changed.
        protected override bool GenerateVideo(LoadedStream stream, bool newDeinterlaceSetting, IProgress<SimpleProgress> progress, SimpleProgress baseProgress) {
            string newFile = Path.Combine(
                Path.GetDirectoryName(stream.Identity.SourceFile),
                $"{Path.GetFileNameWithoutExtension(stream.Identity.SourceFile)}{(newDeinterlaceSetting ? ".dint" : "")}{Path.GetExtension(stream.Identity.SourceFile)}"
            );

            if (File.Exists(Path.Combine(this.Root, newFile)))
            {
                // The correct file already exists, just update the settings as a precaution
                stream.RenameData.Deinterlaced = newDeinterlaceSetting;
                return true;
            }

            // Delete the old file
            try
            {
                File.Delete(stream.GetFullPath(this));
            } catch (Exception)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Failed to delete orginal file: " + stream.GetFullPath(this));
                Console.ResetColor();
            }

            // Generate the new file
            bool? temp = newDeinterlaceSetting;
            string? result = GenerateSourceFile(this.dvd, Path.GetFileName(stream.Identity.SourceFile), ref temp, progress, baseProgress);
            if (result == null) return false;

            if (result != Path.Combine(this.Root, newFile))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Assertion error: New generated file name did not match expected.");
                Console.WriteLine($"Expected \"{Path.Combine(this.Root, newFile)}\" but got \"{result}\" instead");
                Console.ResetColor();
            }

            // Set variable and return success
            stream.RenameData.Deinterlaced = newDeinterlaceSetting;
            return true;
        }

        private static IEnumerable<DiscPlaylist> GetPlaylistsFromPGC(PGC pgc, string baseName, bool isMenu, int vts, string demuxDir)
        {
            if (pgc.NumberOfAngles > 1)
            {
                baseName += " Angle-";

                int angle = 0;
                DiscPlaylist[] playlists = new DiscPlaylist[pgc.NumberOfAngles];
                for(int i = 0; i < pgc.NumberOfAngles; i++)
                {
                    playlists[i] = new DiscPlaylist(baseName + (i + 1).ToString());
                }
                foreach (var cellInfo in pgc.CellInfo)
                {
                    if (cellInfo.IsFirstAngle)
                    {
                        angle = 1;
                    } else if (cellInfo.IsMiddleAngle)
                    {
                        angle++;
                    } else if (cellInfo.IsLastAngle)
                    {
                        angle++;
                    }

                    if (angle == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Invalid cell angle!");
                        Console.ResetColor();
                    } else
                    {
                        playlists[angle - 1].SourceFiles.Add(GetSourceFile(demuxDir, vts, cellInfo.VobID, cellInfo.CellID, isMenu));
                    }

                    if (cellInfo.IsLastAngle)
                    {
                        angle = 0;
                    }
                }
                foreach(var playlist in playlists)
                {
                    yield return playlist;
                }
            } else
            {
                DiscPlaylist playlist = new(baseName);
                foreach (var cellInfo in pgc.CellInfo)
                {
                    playlist.SourceFiles.Add(GetSourceFile(demuxDir, vts, cellInfo.VobID, cellInfo.CellID, isMenu));
                }
                yield return playlist;
            }
        }

        private static string GetSourceFile(string demuxDir, int vts, int vid, int cid, bool isMenu)
        {
            string fileName = $"VTS-{vts:00}{(isMenu ? "_Menu" : "")}_VID-{vid:X4}_CID-{cid:X2}.mp4";
            return Path.Combine(demuxDir, fileName);
        }

        public override List<DiscPlaylist> GetPlaylists()
        {
            return this.playlists.Select(x => x.DeepCopy()).ToList();
        }
    }
}
