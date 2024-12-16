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
using Iso639;
using PgcDemuxLib.Data;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Globalization;
using FFMpeg_Wrapper.ffmpeg;
using FFMpeg_Wrapper.ffprobe;
using FFMpeg_Wrapper.Codecs;

namespace PgcDemuxLib
{
    public struct CellID {
        public int VTS;
        public bool IsMenu;
        public int CID;
        public int VID;
    }

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
            this.Folder = folder;

            folder = Path.Combine(folder, "VIDEO_TS");
            if (!Directory.Exists(folder))
            {
                throw new DirectoryNotFoundException($"Failed to find directory: {folder}");
            }

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
        /*
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
        */

        public static bool TryParseSourceFileName(string name, out CellID result) {
            result = new();

            if (!name.StartsWith("VTS-")) return false;
            int index = "VTS-".Length;

            if (!int.TryParse(name.Substring(index, 2), out result.VTS)) return false;
            if (result.VTS < 0 || result.VTS > 99) return false;
            index += 2;

            if (name.Substring(index, "_Menu".Length) == "_Menu")
            {
                result.IsMenu = true;
                index += "_Menu".Length;
            }

            if (name.Substring(index, "_VID-".Length) != "_VID-") return false;
            index += "_VID-".Length;

            if (!int.TryParse(name.Substring(index, 4), NumberStyles.HexNumber, null, out result.VID)) return false;
            index += 4;

            if (name.Substring(index, "_CID-".Length) != "_CID-") return false;
            index += "_CID-".Length;

            if (!int.TryParse(name.Substring(index, 2), NumberStyles.HexNumber, null, out result.CID)) return false;
            index += 2;

            if (name[index] != '.') return false;

            return true;
        }

        public bool DemuxSourceFiles(IProgress<SimpleProgress>? progress) {
            List<CellID> cells = new();

            if (this.VMG.MenuCellAddressTable != null)
            {
                foreach(ADT adt in this.VMG.MenuCellAddressTable.All.Distinct(new CellEqualityComparer()))
                {
                    cells.Add(new() {
                        VTS = 0,
                        IsMenu = true,
                        VID = adt.VobID,
                        CID = adt.CellID
                    });
                }
            }
            foreach (var vts in this.TitleSets)
            {
                if (vts.MenuCellAddressTable != null)
                {
                    foreach(ADT adt in vts.MenuCellAddressTable.All.Distinct(new CellEqualityComparer()))
                    {
                        cells.Add(new() {
                            VTS = vts.TitleSet,
                            IsMenu = true,
                            VID = adt.VobID,
                            CID = adt.CellID
                        });
                    }
                }
                if (vts.TitleSetCellAddressTable != null)
                {
                    foreach(ADT adt in vts.TitleSetCellAddressTable.All.Distinct(new CellEqualityComparer()))
                    {
                        cells.Add(new() {
                            VTS = vts.TitleSet,
                            IsMenu = false,
                            VID = adt.VobID,
                            CID = adt.CellID
                        });
                    }
                }
            }

            // Prepare folders
            string demuxFolder = Path.Combine(this.Folder, "demux");
            try
            {
                var result = Directory.CreateDirectory(demuxFolder);
                if (!result.Exists) throw new Exception();
            }catch(Exception)
            {
                Log.Error("Failed to create demux folder!");
                return false;
            }

            string? logsFolder = Path.Combine(demuxFolder, "logs");
            try
            {
                var result = Directory.CreateDirectory(logsFolder);
                if (!result.Exists) throw new Exception();
            } catch (Exception)
            {
                Log.Warn("Failed to create ffmpeg logs folder!");
                logsFolder = null;
            }

            string? ffprobeEXE = FileUtils.GetFFProbeExe();
            string? ffmpegEXE = FileUtils.GetFFMpegExe();
            if (ffprobeEXE == null || ffmpegEXE == null)
            {
                Log.Error("FATAL ERROR: Failed to find local .exe files.");
                return false;
            }

            FFProbe ffprobe = new FFProbe(ffprobeEXE);
            FFMpeg ffmpeg = new FFMpeg(ffmpegEXE);

            SimpleProgress currentProgress = new(0, (uint)cells.Count * 2);
            for (int i = 0; i < cells.Count; i++)
            {
                CellID cell = cells[i];
                currentProgress.Total = (uint)i * 2;
                progress?.Report(currentProgress);

                string? file = DemuxSourceFile(ffmpeg, ffprobe, demuxFolder, cell, logsFolder, currentProgress, progress);
                if (file == null)
                {
                    Log.Error($"Failed to demux source file: VTS={cell.VTS} IsMenu={cell.IsMenu} VID={cell.VID} CID={cell.CID}");
                    return false;
                }
            }

            return true;
        }

        private string? DemuxSourceFile(FFMpeg ffmpeg, FFProbe ffprobe, string outputFolder, CellID cell, string? logsFolder, SimpleProgress maxProgress, IProgress<SimpleProgress>? progress = null) {
            IfoBase ifo;
            if (cell.VTS == 0)
            {
                ifo = this.VMG;
            } else
            {
                if (cell.VTS < 0 || (cell.VTS - 1) >= this.TitleSets.Count)
                {
                    Log.Error($"Invalid VTS number: {cell.VTS}");
                    return null; // failure
                }
                ifo = this.TitleSets[cell.VTS - 1];
            }

            // Extract the .VOB from the DVD
            DemuxResult demuxResult;
            string vobFileName = $"VTS-{cell.VTS:00}{(cell.IsMenu ? "_Menu" : "")}_VID-{cell.VID:X4}_CID-{cell.CID:X2}.VOB";
            if (cell.IsMenu)
            {
                demuxResult = ifo.DemuxMenuCell(outputFolder, vobFileName, cell.VID, cell.CID, progress, maxProgress);
            } else
            {
                demuxResult = ifo.DemuxTitleCell(outputFolder, vobFileName, cell.VID, cell.CID, progress, maxProgress);
            }

            if (demuxResult.Successful == false)
            {
                Log.Error($"Failed to demux source VOB file: \"{vobFileName}\"");
                return null;
            }
            maxProgress.Total++;
            progress?.Report(maxProgress);

            try
            {
                // Get ffprobe of source file for later remuxing
                MediaAnalysis? info = ffprobe.Analyse(Path.Combine(outputFolder, vobFileName));
                if (info == null) throw new Exception($"Failed to analyze file: \"{vobFileName}\"");

                // Get the correct stream order from the DVD
                ReadOnlyArray<VTS_AudioAttributes> audioAttribs;
                if (cell.VTS == 0)
                {
                    audioAttribs = this.VMG.MenuAudioAttributes;
                } else
                {
                    VtsIfo vts = this.TitleSets[cell.VTS - 1];
                    audioAttribs = (cell.IsMenu ? vts.MenuAudioAttributes : vts.TitleSetAudioAttributes);
                }

                // Remux into an mp4 with the streams in the correct order
                string remuxFile = Path.Combine(outputFolder, $"{Path.GetFileNameWithoutExtension(vobFileName)}.mp4");
                var args = ffmpeg.Transcode(remuxFile)
                    .AddVideoStreams(info, null, null);

                // Audio streams must be added in a particular order
                foreach ((int index, int audioStreamID) in demuxResult.AudioStreamIDs.Order().WithIndex())
                {
                    var audioAttrib = audioAttribs[index];
                    Language? lang = null;
                    if (audioAttrib.LanguageCode != null)
                    {
                        lang = Language.FromPart1(audioAttrib.LanguageCode);
                    }

                    args.AddAudioStream(
                        info.AudioStreams[demuxResult.AudioStreamIDs.IndexOf(audioStreamID)],
                        new AudioStreamOptions()
                            .SetLanguage(lang),
                        null);
                }

                // Add any remaining streams we want to copy
                args.AddSubtitleStreams(info.SubtitleStreams, null, null);

                // Run ffmpeg command
                var cmd = args
                    .NotifyOnProgress((double percent) =>
                    {
                        maxProgress.Current = (uint)percent;
                        maxProgress.CurrentMax = 100;
                        progress?.Report(maxProgress);
                    })
                    .SetOverwrite(false); // In case something went wrong, throw an error if the output already exists

                if (logsFolder != null)
                {
                    cmd.SetLogPath(Path.Combine(logsFolder, $"FFMpeg_log_{Path.GetFileNameWithoutExtension(vobFileName)}.txt"));
                }

                string? error = cmd.Run();

                maxProgress.Total++;
                progress?.Report(maxProgress);

                if (error != null) throw new Exception(error);

                return remuxFile;
            }catch(Exception e)
            {
                Log.Error(e.Message);
                return null;
            } finally
            {
                try
                {
                    File.Delete(Path.Combine(outputFolder, vobFileName));
                } catch (Exception) { }
            }
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

        private class CellEqualityComparer : IEqualityComparer<ADT> {
            public bool Equals(ADT? x, ADT? y) {
                if (x != null && y != null)
                {
                    return (x.VobID == y.VobID) && (x.CellID == y.CellID);
                } else
                {
                    if ((x == null) && (y == null)) return true; 
                    else return false;
                }
            }

            public int GetHashCode([DisallowNull] ADT obj) {
                return HashCode.Combine(obj.VobID, obj.CellID);
            }
        }
    }
}
