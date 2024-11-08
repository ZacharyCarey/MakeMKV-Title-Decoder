﻿using PgcDemuxLib.Data;
using PgcDemuxLib.Data.VTS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            if (TitleProgramChainTable != null && TitleSetCellAddressTable != null)
            { 
                SortedTitleCells = GetCells(this.TitleSetCellAddressTable.All, TitleProgramChainTable.All);
                CombinedTitleVobCells = GetVidCells(SortedTitleCells);
            }

            if (MenuProgramChainTable != null && MenuCellAddressTable != null)
            {
                IEnumerable<PGC> menuPGCs = MenuProgramChainTable.All.SelectMany(languageUnit => languageUnit.All);
                SortedMenuCells = GetCells(this.MenuCellAddressTable.All, menuPGCs);
                CombinedMenuVobCells = GetVidCells(SortedMenuCells);
            }

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
        internal static List<ADT_VID> GetVidCells(List<ADT_CELL> sortedCells)
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

    }
}
