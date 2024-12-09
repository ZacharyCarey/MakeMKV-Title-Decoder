using FFMpeg_Wrapper.ffprobe;
using FFMpeg_Wrapper;
using PgcDemuxLib.Data.VMG;
using PgcDemuxLib.Data.VTS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using Utils;
using FFMpeg_Wrapper.Codecs;
using Iso639;

namespace PgcDemuxLib
{
    public class Dvd
    {
        internal const int SECTOR_SIZE = 2048;

        [JsonInclude]
        public readonly string Folder;

        [JsonInclude]
        public readonly VmgIfo VMG;

        [JsonInclude]
        public readonly ReadOnlyArray<VtsIfo> TitleSets;

        private Dvd(string folder)
        {
            folder = Path.Combine(folder, "VIDEO_TS");
            if (!Directory.Exists(folder))
            {
                throw new DirectoryNotFoundException($"Failed to find directory: {folder}");
            }
            this.Folder = folder;

            this.VMG = new VmgIfo(folder, "VIDEO_TS.IFO");

            this.TitleSets = new ReadOnlyArray<VtsIfo>(VMG.NumberOfTitleSets);
            for (int i = 0; i < this.VMG.NumberOfTitleSets; i++)
            {
                this.TitleSets[i] = new VtsIfo(folder, $"VTS_{(i + 1):00}_0.IFO");
            }
        }

        public static Dvd? ParseFolder(string folderPath)
        {
            try
            {
                return new Dvd(folderPath);
            } catch(Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Failed to parse DVD:");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                Console.ResetColor();
                return null;
            }
        }

        public bool DemuxAllPgc(string outputFolder)
        {
            foreach(var vts in this.TitleSets)
            {
                if (vts?.MenuProgramChainTable != null) {
                    foreach ((var menu, int index) in vts.MenuProgramChainTable.All.SelectMany(languageUnit => languageUnit.All).WithIndex())
                    {
                        if (menu.NumberOfCells > 0)
                        {
                            if(!vts.DemuxMenuPgc(outputFolder, index))
                            {
                                return false;
                            }
                        }
                    }
                }

                if (vts?.TitleProgramChainTable != null)
                {
                    foreach((var title, int index) in vts.TitleProgramChainTable.All.WithIndex())
                    {
                        if (title.NumberOfCells > 0)
                        {
                            for (int angle = 0; angle < title.NumberOfAngles; angle++)
                            {
                                if (!vts.DemuxTitlePgc(outputFolder, index, angle))
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
            }

            return true;
        }

        public bool DemuxAllCells(string outputFolder, IProgress<SimpleProgress>? progress = null)
        {
            string? ffprobeEXE = FileUtils.SearchLocalExeFiles("ffprobe.exe");
            string? ffmpegEXE = FileUtils.SearchLocalExeFiles("ffmpeg.exe");
            if (ffprobeEXE == null || ffmpegEXE == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("FATAL ERROR: Failed to find local .exe files.");
                Console.ResetColor();
                return false;
            }

            FFProbe ffprobe = new FFProbe(ffprobeEXE);
            FFMpeg ffmpeg = new FFMpeg(ffmpegEXE);

            string? logFolder = Path.Combine(outputFolder, "logs");
            try
            {
                var result = Directory.CreateDirectory(logFolder);
                if (!result.Exists) throw new Exception();
            } catch(Exception)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Failed to create ffmpeg logs folder!");
                Console.ResetColor();
                logFolder = null;
            }

            // Get a count of the number of cells we need to extract
            SimpleProgress currentProgress = new();
            currentProgress.TotalMax = 0;
            if (this.VMG.MenuCellAddressTable != null)
            {
                currentProgress.TotalMax += (uint)this.VMG.MenuCellAddressTable.All.Count();
            }
            foreach(var vts in this.TitleSets)
            {
                if (vts.MenuCellAddressTable != null)
                {
                    currentProgress.TotalMax += (uint)vts.MenuCellAddressTable.All.Count();
                }
                if (vts.TitleSetCellAddressTable != null)
                {
                    currentProgress.TotalMax += (uint)vts.TitleSetCellAddressTable.All.Count();
                }
            }
            currentProgress.TotalMax *= 2; // One for demuxing the mpg stream, a second one for transcoding to mp4
            progress?.Report(currentProgress);

            if (this.VMG.MenuCellAddressTable != null)
            {
                foreach (var cell in this.VMG.MenuCellAddressTable.All)
                {
                    // Demux mpg stream
                    DemuxResult demux = this.VMG.DemuxMenuCell(outputFolder, cell.VobID, cell.CellID, progress, currentProgress);
                    if (!demux.Successful)
                    {
                        return false;
                    }
                    currentProgress.Total++;
                    progress?.Report(currentProgress);

                    // transcode to mp4
                    string outputFile = Path.Combine(outputFolder, $"{Path.GetFileNameWithoutExtension(demux.OutputFileName)}.mp4");
                    string? logFile = null;
                    if (logFolder != null) logFile = Path.Combine(logFolder, $"FFMpeg_log_{Path.GetFileNameWithoutExtension(demux.OutputFileName)}.txt");
                    TranscodeToMP4(ffprobe, ffmpeg, Path.Combine(outputFolder, demux.OutputFileName), outputFile, logFile, demux, this.VMG.MenuAudioAttributes, progress, currentProgress);
                    currentProgress.Total++;
                    progress?.Report(currentProgress);

                    // Cleanup
                    try
                    {
                        File.Delete(Path.Combine(outputFolder, demux.OutputFileName));
                    } catch(Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Failed to delete temporary file: " + demux.OutputFileName);
                        Console.ResetColor();
                    }
                }
            }

            foreach (var vts in this.TitleSets)
            {
                if (vts.MenuCellAddressTable != null)
                {
                    foreach (var cell in vts.MenuCellAddressTable.All)
                    {
                        // Demux mpg stream
                        DemuxResult demux = vts.DemuxMenuCell(outputFolder, cell.VobID, cell.CellID, progress, currentProgress);
                        if (!demux.Successful)
                        {
                            return false;
                        }
                        currentProgress.Total++;
                        progress?.Report(currentProgress);

                        // transcode to mp4
                        string outputFile = Path.Combine(outputFolder, $"{Path.GetFileNameWithoutExtension(demux.OutputFileName)}.mp4");
                        string? logFile = null;
                        if (logFolder != null) logFile = Path.Combine(logFolder, $"FFMpeg_log_{Path.GetFileNameWithoutExtension(demux.OutputFileName)}.txt");
                        TranscodeToMP4(ffprobe, ffmpeg, Path.Combine(outputFolder, demux.OutputFileName), outputFile, logFile, demux, vts.MenuAudioAttributes, progress, currentProgress);
                        currentProgress.Total++;
                        progress?.Report(currentProgress);

                        // Cleanup
                        try
                        {
                            File.Delete(Path.Combine(outputFolder, demux.OutputFileName));
                        } catch (Exception ex)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("Failed to delete temporary file: " + demux.OutputFileName);
                            Console.ResetColor();
                        }
                    }
                }

                if (vts.TitleSetCellAddressTable != null)
                {
                    foreach (var cell in vts.TitleSetCellAddressTable.All)
                    {
                        // Demux mpg stream
                        DemuxResult demux = vts.DemuxTitleCell(outputFolder, cell.VobID, cell.CellID, progress, currentProgress);
                        if (!demux.Successful)
                        {
                            return false;
                        }
                        currentProgress.Total++;
                        progress?.Report(currentProgress);

                        // transcode to mp4
                        string outputFile = Path.Combine(outputFolder, $"{Path.GetFileNameWithoutExtension(demux.OutputFileName)}.mp4");
                        string? logFile = null;
                        if (logFolder != null) logFile = Path.Combine(logFolder, $"FFMpeg_log_{Path.GetFileNameWithoutExtension(demux.OutputFileName)}.txt");
                        TranscodeToMP4(ffprobe, ffmpeg, Path.Combine(outputFolder, demux.OutputFileName), outputFile, logFile, demux, vts.TitleSetAudioAttributes, progress, currentProgress);
                        currentProgress.Total++;
                        progress?.Report(currentProgress);

                        // Cleanup
                        try
                        {
                            File.Delete(Path.Combine(outputFolder, demux.OutputFileName));
                        } catch (Exception ex)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("Failed to delete temporary file: " + demux.OutputFileName);
                            Console.ResetColor();
                        }
                    }
                }
            }

            return true;
        }

        private bool TranscodeToMP4(FFProbe ffprobe, FFMpeg ffmpeg, string inputFile, string outputFile, string? logFile, DemuxResult demux, ReadOnlyArray<Data.VTS_AudioAttributes> audioAttribs, IProgress<SimpleProgress>? progress, SimpleProgress currentProgress) {
            // Determine video length, used to determine transcode progress
            var info = ffprobe.Analyse(inputFile);
            if (info == null) return false;

            var ffmpegArgs = ffmpeg.Transcode(outputFile)
                .AddVideoStreams(info, new VideoStreamOptions()
                    .SetCodec(
                        Codecs.Libx264.SetCRF(16)
                    ));

            // Add audio streams in a particular order
            foreach((int audioStreamID, int index) in demux.AudioStreamIDs.Order().WithIndex())
            {
                var audioAttrib = audioAttribs[index];
                Language? lang = null;
                if (audioAttrib.LanguageCode != null)
                {
                    lang = Language.FromPart1(audioAttrib.LanguageCode);
                }

                ffmpegArgs.AddAudioStream(
                    info.AudioStreams[demux.AudioStreamIDs.IndexOf(audioStreamID)],
                    new AudioStreamOptions()
                        .SetCodec(Codecs.AAC)
                        .SetLanguage(lang)
                );
            }

            ffmpegArgs.AddSubtitleStreams(info.SubtitleStreams, null);

            var cmd = ffmpegArgs
                .NotifyOnProgress(
                    (double percent) => {
                        currentProgress.Current = (uint)percent;
                        currentProgress.CurrentMax = 100;
                        progress?.Report(currentProgress);
                    })
                .SetOverwrite(false);

            if (logFile != null) cmd.SetLogPath(logFile);

            return cmd.Run();
        }

        public bool SaveToFile(string filePath)
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true, TypeInfoResolver = new DefaultJsonTypeInfoResolver() };
                using var stream = File.Create(filePath);
                JsonSerializer.Serialize(stream, this, options);
                stream.Flush();
                stream.Close();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
