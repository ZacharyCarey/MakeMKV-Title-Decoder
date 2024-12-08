using PgcDemuxLib.Data;
using PgcDemuxLib.Data.VTS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace PgcDemuxLib
{
    public abstract class IfoBase
    {
        // This data is used for demuxing later
        internal List<ADT_CELL>? SortedTitleCells = null;
        internal List<ADT_CELL>? SortedMenuCells = null;
        internal List<ADT_VID>? CombinedTitleVobCells = null;
        internal List<ADT_VID>? CombinedMenuVobCells = null;
        internal long[] VobSize = new long[10];

        /// <summary>
        /// VMG returns 0.
        /// </summary>
        public abstract int TitleSet { get; protected set; }
        public abstract string ParentFolder { get; protected set; }

        public abstract VTS_PGCI? TitleProgramChainTable { get; protected set; }
        public abstract VTS_C_ADT? TitleSetCellAddressTable { get; protected set; }


        public abstract VTSM_PGCI_UT? MenuProgramChainTable { get; protected set; }
        public abstract VTS_C_ADT? MenuCellAddressTable { get; protected set; }

        /// <summary>
        /// Get the name of the VOB with the given ID.
        /// For the VMG, there is only one VOB (id=0), VIDEO_TS.VOB.
        /// For the VTS, there are up to 9 files, VTS_xx_n.VOB
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        internal abstract string GetVobPath(int id);

        protected void OrganizeCells()
        {
            IEnumerable<ADT> titleCells = (this.TitleSetCellAddressTable != null) ? this.TitleSetCellAddressTable.All : Enumerable.Empty<ADT>();
            IEnumerable<PGC> titlePGC = (this.TitleProgramChainTable != null) ? this.TitleProgramChainTable.All : Enumerable.Empty<PGC>();
            SortedTitleCells = GetCells(titleCells, titlePGC);
            CombinedTitleVobCells = GetVidCells(SortedTitleCells);

            IEnumerable<ADT> menuCells = (this.MenuCellAddressTable != null) ? this.MenuCellAddressTable.All : Enumerable.Empty<ADT>();
            IEnumerable<PGC> menuPGC = (this.MenuProgramChainTable != null) ? this.MenuProgramChainTable.All.SelectMany(languageUnit => languageUnit.All) : Enumerable.Empty<PGC>();
            SortedMenuCells = GetCells(menuCells, menuPGC);
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
                foreach (var cell in cells)
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
            foreach (var cell in cells)
            {
                bool found = false;
                foreach (var pgc in AllPGCs)
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
        internal static List<ADT_VID> GetVidCells(List<ADT_CELL>? sortedCells)
        {
            List<ADT_VID> vidCells = new();
            foreach (var cell in sortedCells)
            {
                // Check if we already have this cell
                ADT_VID? newCell = null;
                foreach (var vidCell in vidCells)
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

        /// <summary>
        /// Returns the file name of the generated file if successful, null if failed
        /// </summary>
        public DemuxResult DemuxMenuCell(string outputFolder, int vobID, int cellID, IProgress<SimpleProgress>? progress = null, SimpleProgress? maxProgress = null)
        {
            IfoOptions options = new IfoOptions();
            options.Angle = 1;
            options.DomainType = DemuxingDomain.Menus;
            options.ExportVOB = true;
            options.Mode = DemuxingMode.CID;
            options.VID = vobID;
            options.CID = cellID;
            options.CombinedVobName = $"VTS-{this.TitleSet:00}_Menu_VID-{vobID:X4}_CID-{cellID:X2}.VOB";

            PgcDemux demux = new PgcDemux(this, options, outputFolder);
            bool result = demux.Demux(outputFolder, progress, maxProgress); ;
            demux.Close();
            return result ? new DemuxResult(options.CombinedVobName, demux.StreamOrder) : new DemuxResult();
        }

        /// <summary>
        /// Returns the file name of the generated file if successful, null if failed
        /// </summary>
        public DemuxResult DemuxTitleCell(string outputFolder, int vobID, int cellID, IProgress<SimpleProgress>? progress = null, SimpleProgress? maxProgress = null)
        {
            IfoOptions options = new IfoOptions();
            options.Angle = 1;
            options.DomainType = DemuxingDomain.Titles;
            options.ExportVOB = true;
            options.Mode = DemuxingMode.CID;
            options.VID = vobID;
            options.CID = cellID;
            options.CombinedVobName = $"VTS-{this.TitleSet:00}_VID-{vobID:X4}_CID-{cellID:X2}.VOB";
            options.CustomVOB = new();
            options.CustomVOB.PatchLbaNumber = true;
            options.CustomVOB.OnlyFirstIFrame = false;
            options.CustomVOB.SplitVOB = false;
            options.CustomVOB.WriteAudioPacks = true;
            options.CustomVOB.WriteSubPacks = true;
            options.CustomVOB.WriteNavPacks = true;
            options.CustomVOB.WriteVideoPacks = true;

            PgcDemux demux = new PgcDemux(this, options, outputFolder);
            bool result = demux.Demux(outputFolder, progress, maxProgress);
            demux.Close();
            return result ? new DemuxResult(options.CombinedVobName, demux.StreamOrder) : new DemuxResult();
        }
    }

    public struct DemuxResult {
        public bool Successful;
        public string OutputFileName;
        public List<int> AudioStreamIDs;

        public DemuxResult() {
            Successful = false;
            OutputFileName = "";
            AudioStreamIDs = new();
        }

        internal DemuxResult(string outputFileName, List<int> audioStreamIDs) {
            this.Successful = true;
            this.OutputFileName = outputFileName;
            this.AudioStreamIDs = audioStreamIDs;
        }
    }
}
