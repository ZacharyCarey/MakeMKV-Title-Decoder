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

namespace MakeMKV_Title_Decoder.Data.DVD
{
    public class DvdDisc : LoadedDisc
    {
        private List<DiscPlaylist> playlists;

        private DvdDisc(string root, List<LoadedStream> streams, string? title, long? numSets, long? setNum, List<DiscPlaylist> playlists) : base(root, title, numSets, setNum, streams)
        {
            this.playlists = playlists;
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
                    success = dvd.DemuxAllCells(fullDemuxPath, progress);
                } catch (Exception)
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
                if (id != null)
                {
                    TimeSpan duration = new();
                    try
                    {
                        //VTS-{this.TitleSet:00}_Menu_VID-{vobID:X4}_CID-{cellID:X2}.VOB
                        //VTS-{this.TitleSet:00}_VID-{vobID:X4}_CID-{cellID:X2}.VOB
                        string[] args = fileName.Split('_');
                        int n = args.Length - 1;
                        int vts = int.Parse(args[0].Substring(args[0].IndexOf('-') + 1));
                        int vid = int.Parse(args[n - 1].Substring(args[n - 1].IndexOf('-') + 1), System.Globalization.NumberStyles.HexNumber);
                        int cid = int.Parse(args[n].Substring(args[n].IndexOf('-') + 1, 2), System.Globalization.NumberStyles.HexNumber);
                        bool isMenu = (args[1] == "Menu");
                        IEnumerable<PGC> pgcs;
                        if (vts == 0)
                        {
                            pgcs = (dvd.VMG.MenuProgramChainTable?.All?.SelectMany(x => x.All) ?? new List<PGC>().AsEnumerable()).Append(dvd.VMG.FirstPlayPGC);
                        } else
                        {
                            pgcs = (dvd.TitleSets[vts - 1].MenuProgramChainTable?.All?.SelectMany(x => x.All) ?? new List<PGC>().AsEnumerable()).Concat(dvd.TitleSets[vts - 1].TitleProgramChainTable?.All ?? new List<PGC>().AsEnumerable());
                        }
                        IEnumerable<CellInfo> cells = pgcs.SelectMany(x => x.CellInfo);
                        duration = cells.Where(cell => cell.VobID == vid && cell.CellID == cid).First().Duration;
                    }catch(Exception)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Failed to read DVD cell duration.");
                        Console.ResetColor();
                    }
                    streams.Add(new DvdStream(root, Path.Combine(demuxDir, fileName), id, duration));
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

            return new DvdDisc(root, streams, $"DVD-{new DirectoryInfo(root).Name}", dvd.VMG.NumberOfVolumes, dvd.VMG.VolumeNumber, playlists);
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
            string fileName = $"VTS-{vts:00}{(isMenu ? "_Menu" : "")}_VID-{vid:X4}_CID-{cid:X2}.VOB";
            return Path.Combine(demuxDir, fileName);
        }

        public override List<DiscPlaylist> GetPlaylists()
        {
            return this.playlists.Select(x => x.DeepCopy()).ToList();
        }
    }
}
