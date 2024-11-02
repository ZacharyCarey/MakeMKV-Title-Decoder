using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PgcDemuxLib.Data.VMG;
using PgcDemuxLib.Data.VTS;

namespace PgcDemuxLib.Data
{

    internal class ADT_CELL
    {
        public int VobID;
        public int CellID;
        public int StartSector;
        public int EndSector;
        public int Size;
        public TimeSpan dwDuration;
    }

    internal class ADT_VID
    {
        public int VobID;
        public int iSize;
        public int NumberOfCells;
        public TimeSpan dwDuration;
    }

    /// <summary>
    /// https://dvd.sourceforge.net/dvdinfo/ifo.html
    /// </summary>
    public class IfoOld
    {

        public string ParentFolder { get; internal set; }
        public string FileName { get; internal set; }
        public bool IsVideoManager { get; internal set; }

        #region VMG Specific data
        public TT_SRPT? TableOfTitles { get; internal set; }
        #endregion

        #region VTS specific data
        public VTS_PTT_SRPT? TitlesTable { get; internal set; }
        public VTS_PGCI? TitleProgramChainTable { get; internal set; }
        public VTS_TMAPTI? TimeMap { get; internal set; }
        public VTS_C_ADT? TitleSetCellAddressTable { get; internal set; }
        //public VTS_VOBU_ADMAP? TitleSetVobuAddressMap { get; internal set; }
        public VTS_VideoAttributes? VtsVideoAttributes { get; internal set; }
        #endregion

        public VTSM_PGCI_UT? MenuProgramChainTable { get; internal set; }
        public VTS_C_ADT MenuCellAddressTable { get; internal set; }
        //public VTS_VOBU_ADMAP MenuVobuAddressMap { get; internal set; }

        #region Internal Use
        /// <summary>
        /// All Menu PGCs from all LanguageUnits concatenated into one list
        /// </summary>
        internal List<PGC> MenuPGCs = new();

        /// <summary>
        /// With a given PGC from a LanguageUnit, get the language unit index
        /// </summary>
        internal Dictionary<PGC, int> m_nLU_MPGC = new(); // TODO I hate this

        internal List<ADT_CELL> m_AADT_Cell_list = new();
        internal List<ADT_CELL> m_MADT_Cell_list = new();
        internal List<ADT_VID> m_AADT_Vid_list = new();
        internal List<ADT_VID> m_MADT_Vid_list = new();
        internal int m_nVobFiles;
        internal long[] m_i64VOBSize = new long[10];
        #endregion

        public IfoOld(string parentFolder, string fileName, VmgIfo ifo)
        {
            IsVideoManager = true;
            TitlesTable = null;
            TitleProgramChainTable = null;
            MenuProgramChainTable = ifo.MenuProgramChainTable;
            TimeMap = null;
            MenuCellAddressTable = ifo.MenuCellAddressTable;
            //MenuVobuAddressMap = ifo.MenuVobuAddressMap;
            TitleSetCellAddressTable = null;
            //TitleSetVobuAddressMap = null;
            VtsVideoAttributes = null;
            TableOfTitles = ifo.TitleInfoTable;

            Init(parentFolder, fileName);
        }

        public IfoOld(string parentFolder, string fileName, VtsIfo ifo)
        {
            IsVideoManager = false;
            TitlesTable = ifo.TitleAndChapterInfoTable;
            TitleProgramChainTable = ifo.TitleProgramChainTable;
            MenuProgramChainTable = ifo.MenuProgramChainTable;
            TimeMap = ifo.TimeMap;
            MenuCellAddressTable = ifo.MenuCellAddressTable;
            //MenuVobuAddressMap = ifo.MenuVobuAddressMap;
            TitleSetCellAddressTable = ifo.TitleSetCellAddressTable;
            //TitleSetVobuAddressMap = ifo.TitleSetVobuAddressMap;
            VtsVideoAttributes = ifo.TitleSetVideoAttributes;
            TableOfTitles = null;

            Init(parentFolder, fileName);
        }

        private void Init(string parentFolder, string fileName)
        {
            ParentFolder = parentFolder;
            FileName = fileName;

            if (MenuProgramChainTable != null)
            {
                for (int nLU = 0; nLU < MenuProgramChainTable.NumberOfLanguageUnits; nLU++)
                {
                    var lu = MenuProgramChainTable[nLU];
                    for (int nPGC = 0; nPGC < lu.NumberOfProgramChains; nPGC++)
                    {
                        var pgc = lu[nPGC];
                        this.m_nLU_MPGC[pgc] = nLU;
                        this.MenuPGCs.Add(pgc);
                    }
                }
            }

            // Title cells
            if (TitleSetCellAddressTable != null)
            {
                for (int i = 0; i < TitleSetCellAddressTable.NumberOfADTs; i++)
                {
                    var adt = TitleSetCellAddressTable[i];

                    int iArraysize = m_AADT_Cell_list.Count;
                    ADT_CELL? kk = null;
                    for (int k = 0; k < iArraysize; k++)
                    {
                        if (adt.CellID == m_AADT_Cell_list[k].CellID &&
                            adt.VobID == m_AADT_Cell_list[k].VobID)
                        {
                            kk = m_AADT_Cell_list[k];
                            break;
                        }
                    }
                    if (kk == null)
                    {
                        ADT_CELL myADT_Cell = new();
                        myADT_Cell.VobID = adt.VobID;
                        myADT_Cell.CellID = adt.CellID;
                        myADT_Cell.Size = 0;
                        myADT_Cell.StartSector = 0x7fffffff;
                        myADT_Cell.EndSector = 0;
                        kk = InsertCell(myADT_Cell, DemuxingDomain.Titles);
                        //			m_AADT_Cell_list.SetAtGrow(iArraysize,myADT_Cell);
                        //			kk=iArraysize;
                    }
                    int iIniSec = adt.StartSector;
                    int iEndSec = adt.EndSector;
                    if (iIniSec < kk.StartSector) kk.StartSector = iIniSec;
                    if (iEndSec > kk.EndSector) kk.EndSector = iEndSec;
                    //int iSize = (iEndSec - iIniSec + 1);
                    kk.Size += iEndSec - iIniSec + 1;
                }
            }

            // Menu cells
            if (MenuCellAddressTable != null)
            {
                for (int i = 0; i < MenuCellAddressTable.NumberOfADTs; i++)
                {
                    var adt = MenuCellAddressTable[i];

                    int iArraySize = m_MADT_Cell_list.Count;
                    ADT_CELL? kk = null;
                    for (int k = 0; k < iArraySize; k++)
                    {
                        if (adt.CellID == m_MADT_Cell_list[k].CellID &&
                            adt.VobID == m_MADT_Cell_list[k].VobID)
                        {
                            kk = m_MADT_Cell_list[k];
                            break;
                        }
                    }
                    if (kk == null)
                    {
                        ADT_CELL myADT_Cell = new();
                        myADT_Cell.VobID = adt.VobID;
                        myADT_Cell.CellID = adt.CellID;
                        myADT_Cell.Size = 0;
                        myADT_Cell.StartSector = 0x7fffffff;
                        myADT_Cell.EndSector = 0;
                        kk = InsertCell(myADT_Cell, DemuxingDomain.Menus);
                        // m_MADT_Cell_list.SetAtGrow(iArraysize, myADT_Cell);
                        // kk = iArraysize
                    }
                    int iIniSec = adt.StartSector;
                    int iEndSec = adt.EndSector;
                    if (iIniSec < kk.StartSector) kk.StartSector = iIniSec;
                    if (iEndSec > kk.EndSector) kk.EndSector = iEndSec;
                    //int iSize = (iEndSec - iIniSec + 1)
                    kk.Size += iEndSec - iIniSec + 1;
                }
            }

            FillDurations();

            // VIDs in Titles
            for (int i = 0; i < m_AADT_Cell_list.Count; i++)
            {
                int VidADT = m_AADT_Cell_list[i].VobID;

                int nVIDs = m_AADT_Vid_list.Count;
                ADT_VID? kk = null;
                for (int k = 0; k < nVIDs; k++)
                {
                    if (VidADT == m_AADT_Vid_list[k].VobID)
                    {
                        kk = m_AADT_Vid_list[k];
                        break;
                    }
                }
                if (kk == null)
                {
                    ADT_VID myADT_Vid = new();
                    myADT_Vid.VobID = VidADT;
                    myADT_Vid.iSize = 0;
                    myADT_Vid.NumberOfCells = 0;
                    myADT_Vid.dwDuration = new TimeSpan();
                    m_AADT_Vid_list.Add(myADT_Vid);
                    kk = myADT_Vid;
                }
                kk.iSize += m_AADT_Cell_list[i].Size;
                kk.NumberOfCells++;
                kk.dwDuration += m_AADT_Cell_list[i].dwDuration;
            }

            // VIDs in Menus
            for (int i = 0; i < m_MADT_Cell_list.Count; i++)
            {
                int VidADT = m_MADT_Cell_list[i].VobID;

                int nVIDs = m_MADT_Vid_list.Count;
                ADT_VID? kk = null;
                for (int k = 0; k < nVIDs; k++)
                {
                    if (VidADT == m_MADT_Vid_list[k].VobID)
                    {
                        kk = m_MADT_Vid_list[k];
                        break;
                    }
                }
                if (kk == null)
                {
                    ADT_VID myADT_Vid = new();
                    myADT_Vid.VobID = VidADT;
                    myADT_Vid.iSize = 0;
                    myADT_Vid.NumberOfCells = 0;
                    myADT_Vid.dwDuration = new TimeSpan();
                    m_MADT_Vid_list.Add(myADT_Vid);
                    kk = myADT_Vid;
                }
                kk.iSize += m_MADT_Cell_list[i].Size;
                kk.NumberOfCells++;
                kk.dwDuration += m_MADT_Cell_list[i].dwDuration;
            }

            // Fill VOB file size
            if (IsVideoManager)
            {
                m_nVobFiles = 0;

                for (int k = 0; k < 10; k++)
                    m_i64VOBSize[k] = 0;

                string temp = fileName[..^3];
                temp += "VOB";
                try
                {
                    m_i64VOBSize[0] = new FileInfo(Path.Combine(parentFolder, temp)).Length;
                }
                catch (Exception)
                {
                    m_i64VOBSize[0] = 0;
                }
            }
            else
            {
                for (int k = 0; k < 10; k++)
                {
                    string temp2 = fileName[..^5];
                    string temp = $"{k}.VOB";
                    temp = temp2 + temp;

                    try
                    {
                        m_i64VOBSize[k] = new FileInfo(Path.Combine(parentFolder, temp)).Length;
                        m_nVobFiles = k;
                    }
                    catch (Exception)
                    {
                        m_i64VOBSize[k] = 0;
                    }
                }
            }
        }

        private ADT_CELL InsertCell(ADT_CELL myADT_Cell, DemuxingDomain iDomain)
        {
            int iArraysize, i, ii;
            bool bIsHigher;

            if (iDomain == DemuxingDomain.Titles)
            {
                iArraysize = m_AADT_Cell_list.Count;
                ii = iArraysize;
                for (i = 0, bIsHigher = true; i < iArraysize && bIsHigher; i++)
                {
                    if (myADT_Cell.VobID < m_AADT_Cell_list[i].VobID) { ii = i; bIsHigher = false; }
                    else if (myADT_Cell.VobID > m_AADT_Cell_list[i].VobID) bIsHigher = true;
                    else
                    {
                        if (myADT_Cell.CellID < m_AADT_Cell_list[i].CellID) { ii = i; bIsHigher = false; }
                        else if (myADT_Cell.CellID > m_AADT_Cell_list[i].CellID) bIsHigher = true;
                    }

                }
                m_AADT_Cell_list.Insert(ii, myADT_Cell);
            }
            if (iDomain == DemuxingDomain.Menus)
            {
                iArraysize = m_MADT_Cell_list.Count;
                ii = iArraysize;
                for (i = 0, bIsHigher = true; i < iArraysize && bIsHigher; i++)
                {
                    if (myADT_Cell.VobID < m_MADT_Cell_list[i].VobID) { ii = i; bIsHigher = false; }
                    else if (myADT_Cell.VobID > m_MADT_Cell_list[i].VobID) bIsHigher = true;
                    else
                    {
                        if (myADT_Cell.CellID < m_MADT_Cell_list[i].CellID) { ii = i; bIsHigher = false; }
                        else if (myADT_Cell.CellID > m_MADT_Cell_list[i].CellID) bIsHigher = true;
                    }

                }
                //		if (i>0 && bIsHigher) i--;
                m_MADT_Cell_list.Insert(ii, myADT_Cell);
            }
            return myADT_Cell; //ii;
        }

        private void FillDurations()
        {
            int iArraysize;
            int i, j, k;
            int VIDa, CIDa, VIDb, CIDb;
            bool bFound;


            iArraysize = m_AADT_Cell_list.Count;

            for (i = 0; i < iArraysize; i++)
            {
                VIDb = m_AADT_Cell_list[i].VobID;
                CIDb = m_AADT_Cell_list[i].CellID;
                for (j = 0, bFound = false; j < (TitleProgramChainTable?.NumberOfProgramChains ?? 0) && !bFound; j++)
                {
                    for (k = 0; k < TitleProgramChainTable[j].NumberOfCells; k++)
                    {
                        var cellInfo = TitleProgramChainTable[j].CellInfo[k];
                        VIDa = cellInfo.VobID;
                        CIDa = cellInfo.CellID;
                        if (VIDa == VIDb && CIDa == CIDb)
                        {
                            bFound = true;
                            m_AADT_Cell_list[i].dwDuration = cellInfo.Duration;
                        }
                    }
                }
                if (!bFound)
                {
                    if (VtsVideoAttributes == null || VtsVideoAttributes?.Format == VideoFormat.NTSC) // NTSC
                        m_AADT_Cell_list[i].dwDuration = Util.ParseDuration(0xC0); // TODO parse static value
                    else // PAL
                        m_AADT_Cell_list[i].dwDuration = Util.ParseDuration(0x40); // TODO parse static value
                }
            }

            iArraysize = m_MADT_Cell_list.Count;

            for (i = 0; i < iArraysize; i++)
            {
                VIDb = m_MADT_Cell_list[i].VobID;
                CIDb = m_MADT_Cell_list[i].CellID;
                for (j = 0, bFound = false; j < MenuPGCs.Count && !bFound; j++)
                {
                    for (k = 0; k < MenuPGCs[j].NumberOfCells; k++)
                    {
                        var cellInfo = MenuPGCs[j].CellInfo[k];
                        VIDa = cellInfo.VobID;
                        CIDa = cellInfo.CellID;
                        if (VIDa == VIDb && CIDa == CIDb)
                        {
                            bFound = true;
                            m_MADT_Cell_list[i].dwDuration = cellInfo.Duration;
                        }
                    }
                }
                if (!bFound)
                {
                    if (VtsVideoAttributes == null || VtsVideoAttributes?.Format == VideoFormat.NTSC) // NTSC
                        m_MADT_Cell_list[i].dwDuration = Util.ParseDuration(0xC0); // TODO parse static value
                    else // PAL
                        m_MADT_Cell_list[i].dwDuration = Util.ParseDuration(0x40); // TODO parse static value
                }
            }

        }
    }
}
