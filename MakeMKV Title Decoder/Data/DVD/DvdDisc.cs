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

        public override bool ForceVlcTrackIndex => true;
        public override bool ForceTranscoding => true;

        private DvdDisc(Dvd dvd, string root, List<LoadedStream> streams, string? title, long? numSets, long? setNum, List<DiscPlaylist> playlists) : base(root, title, numSets, setNum, streams)
        {
            this.playlists = playlists;
            this.dvd = dvd;
        }

        public static DvdDisc? Open(string root, IProgress<SimpleProgress>? progress = null)
        {
            string? ffprobeEXE = FileUtils.GetFFProbeExe();
            if (ffprobeEXE == null) return null;
            FFProbe ffprobe = new(ffprobeEXE);

            //string streamDir = "VIDEO_TS";
            string demuxDir = Path.Combine(root, "demux");
            //SimpleProgress currentProgress = new();

            Dvd? dvd = Dvd.ParseFolder(root);
            if (dvd == null) return null;

            if (!Directory.Exists(demuxDir))
            {
                if (MessageBox.Show("The stream files must be extracted from the DVD. This will create large temporary files. Continue?", "Extract files?", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
                {
                    return null;
                }

                bool success;
                try
                {
                    success = dvd.DemuxSourceFiles(progress);
                } catch (Exception e)
                {
                    success = false;
                }

                if (!success)
                {
                    Directory.Delete(demuxDir, true);
                    return null;
                }
            }

            string[] streamFilePaths = Directory.GetFiles(demuxDir);
            SimpleProgress currentProgress = new();
            currentProgress.TotalMax = (uint)streamFilePaths.Length;
            progress?.Report(currentProgress);

            List<LoadedStream> streams = new();
            for (int i = 0; i < streamFilePaths.Length; i++)
            {
                string fileName = Path.GetFileName(streamFilePaths[i]);
                MediaAnalysis? id = ParseIdentify(ffprobe, root, demuxDir, fileName);
                if (id == null) return null;

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
                    Log.Warn("Failed to read DVD cell duration.");
                }
                DvdStream stream = new DvdStream(root, Path.Combine(demuxDir, fileName), id, duration);
                stream.RenameData.Deinterlaced = stream.Tracks.Select(track => track.Identity.Interlaced).Any(x => x == true);
                streams.Add(stream);
                
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
