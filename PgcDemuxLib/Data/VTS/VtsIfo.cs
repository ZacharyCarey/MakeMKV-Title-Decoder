using PgcDemuxLib.Data.VMG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace PgcDemuxLib.Data.VTS
{
    public class VtsIfo
    {
        [JsonInclude]
        public readonly string ParentFolder;

        [JsonInclude]
        public readonly string FileName;

        [JsonInclude]
        public readonly int TitleSet;

        [JsonInclude]
        public readonly VersionNumber Version;

        [JsonInclude]
        public readonly VTS_PTT_SRPT TitleAndChapterInfoTable;

        [JsonInclude]
        public readonly VTS_PGCI TitleProgramChainTable;

        [JsonInclude]
        public readonly VTSM_PGCI_UT? MenuProgramChainTable;

        [JsonInclude]
        public readonly VTS_TMAPTI TimeMap;

        [JsonInclude]
        public readonly VTS_C_ADT? MenuCellAddressTable;

        [JsonInclude]
        public readonly VTS_C_ADT MenuVobuAddressMap;

        [JsonInclude]
        public readonly VTS_C_ADT? TitleSetCellAddressTable;

        [JsonInclude]
        public readonly VTS_C_ADT TitleSetVobuAddressMap;

        [JsonInclude]
        public readonly VTS_VideoAttributes MenuVideoAttributes;

        [JsonInclude]
        public readonly ReadOnlyArray<VTS_AudioAttributes> MenuAudioAttributes;

        [JsonInclude]
        public readonly VTS_SubpictureAttributes? MenuSubpictureAttributes;

        [JsonInclude]
        public readonly VTS_VideoAttributes TitleSetVideoAttributes;

        [JsonInclude]
        public readonly ReadOnlyArray<VTS_AudioAttributes> TitleSetAudioAttributes;

        [JsonInclude]
        public readonly ReadOnlyArray<VTS_SubpictureAttributes> TitleSetSubpictureAttributes;

        // This data is used for demuxing later
        internal List<ADT_CELL> SortedTitleCells;
        internal List<ADT_CELL> SortedMenuCells;
        internal List<ADT_VID> CombinedTitleVobCells;
        internal List<ADT_VID> CombinedMenuVobCells;
        internal long[] VobSize = new long[10];

        internal VtsIfo(string folder, string fileName)
        {
            this.ParentFolder = folder;
            FileName = fileName;

            if (fileName[..4].ToUpper() != "VTS_") throw new Exception("Invalid file name.");
            if (fileName[^6..].ToUpper() != "_0.IFO") throw new Exception("Invalid file name.");
            this.TitleSet = int.Parse(fileName[4..^6]);

            byte[] file = File.ReadAllBytes(Path.Combine(folder, fileName));

            string header = file.GetString(0, 12);
            if (header != "DVDVIDEO-VTS")
            {
                throw new Exception("Invalid file format.");
            }

            Version = new VersionNumber(file, 0x20);

            int addr = PgcDemux.SECTOR_SIZE * file.GetNbytes(0xC8, 4);
            Util.AssertValidAddress(addr, "VTS_PTT_SRPT");
            TitleAndChapterInfoTable = new VTS_PTT_SRPT(file, addr);

            addr = PgcDemux.SECTOR_SIZE * file.GetNbytes(0xCC, 4);
            Util.AssertValidAddress(addr, "VTS_PGCI");
            TitleProgramChainTable = new VTS_PGCI(file, addr);

            addr = PgcDemux.SECTOR_SIZE * file.GetNbytes(0xD0, 4);
            MenuProgramChainTable = (addr == 0) ? null : new VTSM_PGCI_UT(file, addr);

            addr = PgcDemux.SECTOR_SIZE * file.GetNbytes(0xD4, 4);
            Util.AssertValidAddress(addr, "VTS_TMAPTI");
            TimeMap = new VTS_TMAPTI(file, addr);

            addr = PgcDemux.SECTOR_SIZE * file.GetNbytes(0xD8, 4);
            MenuCellAddressTable = (addr == 0) ? null : new VTS_C_ADT(file, addr);

            addr = PgcDemux.SECTOR_SIZE * file.GetNbytes(0xDC, 4);
            Util.AssertValidAddress(addr, "VTSM_VOBU_ADMAP");
            MenuVobuAddressMap = new VTS_C_ADT(file, addr);

            addr = PgcDemux.SECTOR_SIZE * file.GetNbytes(0xE0, 4);
            TitleSetCellAddressTable = (addr == 0) ? null : new VTS_C_ADT(file, addr);

            addr = PgcDemux.SECTOR_SIZE * file.GetNbytes(0xE4, 4);
            Util.AssertValidAddress(addr, "VTS_VOBU_ADMAP");
            TitleSetVobuAddressMap = new VTS_C_ADT(file, addr);

            this.MenuVideoAttributes = new VTS_VideoAttributes(file, 0x100);

            int numAudioStreams = file.GetNbytes(0x102, 2);
            this.MenuAudioAttributes = new ReadOnlyArray<VTS_AudioAttributes>(numAudioStreams);
            for (int i = 0; i < numAudioStreams; i++)
            {
                this.MenuAudioAttributes[i] = new VTS_AudioAttributes(file, 0x104 + 8 * i);
            }

            int numSubpictureAttributes = file.GetNbytes(0x154, 2);
            if (numSubpictureAttributes > 1)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("More than 1 VMGM_VOBS was listed. Only the first one is being read.");
                Console.ResetColor();
                numSubpictureAttributes = 1;
            }
            if (numSubpictureAttributes >= 1)
            {
                this.MenuSubpictureAttributes = new VTS_SubpictureAttributes(file, 0x156);
            }
            else
            {
                this.MenuSubpictureAttributes = null;
            }

            TitleSetVideoAttributes = new VTS_VideoAttributes(file, 0x200);

            numAudioStreams = file.GetNbytes(0x202, 2);
            this.TitleSetAudioAttributes = new ReadOnlyArray<VTS_AudioAttributes>(numAudioStreams);
            for (int i = 0; i < numAudioStreams; i++)
            {
                this.TitleSetAudioAttributes[i] = new VTS_AudioAttributes(file, 0x204 + 8 * i);
            }

            numSubpictureAttributes = file.GetNbytes(0x254, 2);
            this.TitleSetSubpictureAttributes = new ReadOnlyArray<VTS_SubpictureAttributes>(numSubpictureAttributes);
            for (int i = 0; i < numSubpictureAttributes; i++)
            {
                this.TitleSetSubpictureAttributes[i] = new VTS_SubpictureAttributes(file, 0x256 + i * 6);
            }

            // Process cells for easier demuxing later
            SortedTitleCells = GetCells(this.TitleSetCellAddressTable.All, TitleProgramChainTable.All);
            IEnumerable<PGC> menuPGCs = MenuProgramChainTable.All.SelectMany(languageUnit => languageUnit.All);
            SortedMenuCells = GetCells(this.MenuCellAddressTable.All, menuPGCs);
            CombinedTitleVobCells = GetVidCells(SortedTitleCells);
            CombinedMenuVobCells = GetVidCells(SortedMenuCells);

            for (int k = 0; k < 10; k++)
            {
                string vobName = $"VTS_{this.TitleSet:00}_{k:0}.VOB";
                
                try
                {
                    VobSize[k] = (new FileInfo(Path.Combine(this.ParentFolder, vobName)).Length);
                }
                catch (Exception)
                {
                    continue;
                }
            }
        }

        // TODO put in relevent data structure???
        public bool DemuxTitle(string outputFolder, int pgcIndex, int angle = 1)
        {
            //string fileName = $"VTS-{this.TitleSet}_PGC-{pgcIndex}_Angle-{angleIndex}.vob";
            //return Demux(Path.Combine(outputFolder, fileName), this.TitleProgramChainTable[pgcIndex], this.SortedTitleCells, angleIndex, options);
            IfoOptions options = new IfoOptions();
            options.Angle = angle;
            options.DomainType = DemuxingDomain.Titles;
            options.ExportVOB = true;
            options.Mode = DemuxingMode.PGC;
            options.PGC = pgcIndex;
            
            PgcDemux demux = new PgcDemux(this, options);
            return demux.Demux(outputFolder);
        }
/*
        private bool Demux(string outputPath, PGC data, List<ADT_CELL> sortedCells, int selectedAngle, DemuxOptions options)
        {
            // TODO temp!
            IfoOptions demuxOptions = new();
            demuxOptions.ExtractVideo = false;
            demuxOptions.ExtractAudio = false;
            demuxOptions.ExportVOB = true;
            demuxOptions.CustomVOB = null;
            demuxOptions.ExtractSubtitles = false;
            demuxOptions.GenerateCellTimes = false;
            demuxOptions.IncludeEndTime = false;
            demuxOptions.DomainType = DemuxingDomain.Titles;
            demuxOptions.Angle = selectedAngle;

            if (selectedAngle < 0 || selectedAngle >= data.NumberOfAngles)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid angle.");
                Console.ResetColor();
                return false;
            }

            // TODO video streams
            Stream? videoStream = null;
            //using Stream output = File.Create(outputPath);
            //if (options.ExportVideoStreams) videoStream = File.Create()

            IEnumerable<ADT_CELL> cells = GetPgcCells(data, sortedCells, selectedAngle);
            int nTotalSectors = cells.Sum(x => x.Size);

            using VobStream vobReader = new VobStream(this.ParentFolder, this.TitleSet);
            byte[] buffer = new byte[2050];

            PgcDemux2 demux = new(demuxOptions, buffer);
            foreach (var cell in FilterByAngle(data.CellInfo, selectedAngle))
            {
                long startSector = cell.FirstVobuStartSector;
                long endSector = cell.LastVobuEndSector;
                long sectors = 0;
                int numVob = 9;
                for (int vob = 1; vob < 10; vob++)
                {
                    //long numSectors = VobSize[vob] / PgcDemux.SECTOR_SIZE;
                    //if (sectors + numSectors <= startSector)
                    //{
                    //    sectors += numSectors;
                    //} else
                    //{
                    //    numVob = vob;
                    //    break;
                    //}

                }

                vobReader.Seek(startSector * PgcDemux.SECTOR_SIZE, SeekOrigin.Begin);

                bool validCell = true;
                for (long i = 0; i < (endSector - startSector + 1); i++)
                {
                    if (vobReader.Read(buffer, 0, PgcDemux.SECTOR_SIZE) != PgcDemux.SECTOR_SIZE)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Input error: reached end of VOB too early.");
                        Console.ResetColor();
                        return false;
                    }

                    if (Util.IsSynch(buffer) == false)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Error reading input VOB: Unsynchronized");
                        Console.ResetColor();
                        return false;
                    }
                    if (Util.IsNav(buffer))
                    {
                        validCell = (buffer[0x420] == (byte)(cell.VobID % 256) &&
                            buffer[0x41F] == (byte)(cell.VobID / 256) &&
                            buffer[0x422] == (byte)cell.CellID);
                    }

                    if (validCell)
                    {
                        if (!demux.ProcessPack(outputPath, true))
                        {
                            return false;
                        } 
                    }
                }
            }

            // TODO
            if (demuxOptions.GenerateCellTimes)
            {
                //csAux = Path.Combine(m_csOutputPath, "Celltimes.txt");
                //fout = File.Open(csAux, FileMode.Create);
                //for (nCell = 0, nCurrAngle = 0; nCell < ifo.TitleProgramChainTable[nPGC].NumberOfCells && m_bInProcess == true; nCell++)
                //{
                //    var cellInfo = ifo.TitleProgramChainTable[nPGC].CellInfo[nCell];
                //    dwCellDuration = cellInfo.Duration;

                //    //			0101=First; 1001=Middle ;	1101=Last
                //    if (cellInfo.IsFirstAngle)
                //        nCurrAngle = 1;
                //    else if ((cellInfo.IsMiddleAngle || cellInfo.IsLastAngle) && nCurrAngle != 0)
                //        nCurrAngle++;
                //    if (cellInfo.IsNormal || (nAng + 1) == nCurrAngle)
                //    {
                //        nFrames += Util.DurationInFrames(dwCellDuration);
                //        if (nCell != (ifo.TitleProgramChainTable[nPGC].NumberOfCells - 1) || m_bCheckEndTime)
                //        {
                //            var writer = new StreamWriter(fout);
                //            writer.Write($"{nFrames}\n");
                //            writer.Flush();
                //        }
                //    }

                //    if (cellInfo.IsLastAngle) nCurrAngle = 0;
                //}
                //fout.Close();
            }

            return true;
        }
*/
        /// <summary>
        /// The selected angle number is 1 indexed. That is, the first angle would be "angle=1", the second "angle=2" and so on.
        /// Some cells without angles will likely be listed as "angle=0".
        /// Giving an angle of 0 or less will return all cells
        /// </summary>
        /// <param name="pgc"></param>
        /// <param name="sortedCells"></param>
        /// <returns></returns>
        private static IEnumerable<ADT_CELL> GetPgcCells(PGC pgc, List<ADT_CELL> sortedCells, int selectedAngle = 0)
        {
            bool sortByAngle = (selectedAngle > 0);
            foreach(var cell in FilterByAngle(pgc.CellInfo, selectedAngle))
            {
                foreach(var adt in sortedCells)
                {
                    if (adt.VobID == cell.VobID && adt.CellID == cell.CellID)
                    {
                        yield return adt;
                    }
                }
            }
        }

        private static IEnumerable<(CellInfo Cell, int Angle)> GetCellAngle(IEnumerable<CellInfo> cells)
        {
            int currentAngle = 0;
            foreach (var cell in cells)
            {
                if (cell.IsFirstAngle)
                {
                    currentAngle = 1;
                }
                else if ((cell.IsMiddleAngle || cell.IsLastAngle) && currentAngle != 0)
                {
                    currentAngle++;
                }

                yield return (cell, currentAngle);

                if (cell.IsLastAngle)
                {
                    currentAngle = 0;
                }
            }
        }

        /// <summary>
        /// The selected angle number is 1 indexed. That is, the first angle would be "angle=1", the second "angle=2" and so on.
        /// Some cells without angles will likely be listed as "angle=0".
        /// Giving an angle of 0 or less will return all cells
        /// </summary>
        /// <param name="cells"></param>
        /// <param name="selectedAngle"></param>
        /// <returns></returns>
        private static IEnumerable<CellInfo> FilterByAngle(IEnumerable<CellInfo> cells, int selectedAngle)
        {
            if (selectedAngle <= 0)
            {
                return cells;
            } else
            {
                return GetCellAngle(cells)
                    .Where(x => x.Cell.IsNormal || (x.Angle == selectedAngle))
                    .Select(x => x.Cell);
            }
        }

        /// <summary>
        /// Given the raw cells from the DVD, returns a list of "combined" cells
        /// that are sorted by their VobID and CellID.
        /// 
        /// By "combined" I mean cells that have the sam VobID and CellID are
        /// combined into a single struct with the needed sector and duration info.
        /// </summary>
        /// <param name="AllCells"></param>
        /// <param name="AllPGCs"></param>
        /// <returns></returns>
        internal static List<ADT_CELL> GetCells(IEnumerable<ADT> AllCells, IEnumerable<PGC> AllPGCs)
        {
            // Collect all relevant cells
            List<ADT_CELL> cells = new();
            foreach (var adt in AllCells)
            {
                // Check if this cell has already been added
                ADT_CELL? newCell = null;
                foreach(var cell in cells)  
                {
                    if (adt.CellID == cell.CellID && adt.VobID == cell.VobID)
                    {
                        newCell = cell;
                        break;
                    }
                }

                // If cell hasnt been been added yet, create a new one
                if (newCell == null)
                {
                    newCell = new();
                    newCell.VobID = adt.VobID;
                    newCell.CellID = adt.CellID;
                    newCell.Size = 0;
                    newCell.StartSector = 0x7fffffff;
                    newCell.EndSector = 0;
                    //InsertTitleCell(newCell);
                    cells.Add(newCell);
                }

                // Adjust sectors as needed
                int startSector = adt.StartSector;
                int endSector = adt.EndSector;
                if (startSector < newCell.StartSector) newCell.StartSector = startSector;
                if (endSector > newCell.EndSector) newCell.EndSector = endSector;
                newCell.Size += endSector - startSector + 1;
            }

            // Put cells in order first by VobID, then CellID
            cells.Sort((x, y) =>
            {
                // NOTE: return <0 means x < y
                //       return >0 means x > y
                //       return  0 means x = y

                // First sort by VobID
                int comp = (x.VobID - y.VobID);
                if (comp == 0)
                {
                    // VobID is the same, so now sort by CellId
                    comp = (x.CellID - y.CellID);
                }

                // If comp != 0, then returning it will return the correct sorting by VobID
                return comp;
            });

            // Calculate duration info
            foreach(var cell in cells)
            {
                bool found = false;
                foreach(var pgc in AllPGCs)
                {
                    foreach (var pgcCell in pgc.CellInfo)
                    {
                        if (pgcCell.VobID == cell.VobID && pgcCell.CellID == cell.CellID)
                        {
                            found = true;
                            cell.dwDuration = pgcCell.Duration;
                        }
                    }
                }

                if (!found)
                {
                    cell.dwDuration = TimeSpan.Zero;
                }
            }

            return cells;
        }

        /// <summary>
        /// Further processes combined cells by combining cells with the same VobID.
        /// </summary>
        /// <param name="sortedCells"></param>
        /// <returns></returns>
        internal static List<ADT_VID> GetVidCells(List<ADT_CELL> sortedCells)
        {
            List<ADT_VID> vidCells = new();
            foreach(var cell in sortedCells)
            {
                // Check if we already have this cell
                ADT_VID? newCell = null;
                foreach(var vidCell in vidCells)
                {
                    if (cell.VobID == vidCell.VobID)
                    {
                        newCell = vidCell;
                        break;
                    }
                }
                if (newCell == null)
                {
                    newCell = new();
                    newCell.VobID = cell.VobID;
                    newCell.Size = 0;
                    newCell.NumberOfCells = 0;
                    newCell.dwDuration = TimeSpan.Zero;
                    vidCells.Add(newCell);
                }

                newCell.Size += cell.Size;
                newCell.NumberOfCells++;
                newCell.dwDuration += cell.dwDuration;
            }

            return vidCells;
        }
    }
}
