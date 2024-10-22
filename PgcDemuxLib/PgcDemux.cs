using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PgcDemuxLib {
    internal class ADT_CELL_LIST {
        public int VID, CID;
        public int iSize;
        public int iIniSec, iEndSec;
        public UInt64 dwDuration;
    }

    internal class ADT_VID_LIST {
        public int VobID;
        public int iSize;
        public int NumberOfCells;
        public UInt64 dwDuration;
    }

    internal enum AudioType {
        UNK = 0,
        WAV = 1,
        AC3 = 2,
        DTS = 3,
        MP1 = 4,
        MP2 = 5,
        DDS = 6
    }

    public enum DemuxingMode {
        PGC = 0,
        VID = 1,
        CID = 2
    }

    public enum DemuxingDomain {
        Titles = 1,
        Menus = 0
    }

    public class PgcDemux {

        const string PGCDEMUX_VERSION = "1.2.0.5";
        const int MAXLENGTH = 20 * 1024 * 1024;
        const int MAX_PGC = 32768;
        const int MAX_LU = 100;
        const int MAX_MPGC = 32768;
        const int MODUPDATE = 100;
        const int MAXLOOKFORAUDIO = 10000; // Max number of explored packs in audio delay check

        private static byte[] pcmheader = new byte[44] {
            0x52, 0x49, 0x46, 0x46, 0x00, 0x00, 0x00, 0x00, 0x57, 0x41, 0x56, 0x45, 0x66, 0x6D, 0x74, 0x20,
            0x10, 0x00, 0x00, 0x00, 0x01, 0x00, 0x02, 0x00, 0x80, 0xBB, 0x00, 0x00, 0x70, 0x17, 0x00, 0x00,
            0x04, 0x00, 0x10, 0x00, 0x64, 0x61, 0x74, 0x61, 0x00, 0x00, 0x00, 0x00
        };

        byte[] m_pIFO = null;
        byte[] m_buffer = new byte[2050];

        bool m_bInProcess, m_bAbort, m_bCLI;

        bool m_bCheckIFrame;
        bool m_bCheckVideoPack;
        bool m_bCheckAudioPack;
        bool m_bCheckNavPack;
        bool m_bCheckSubPack;
        bool m_bCheckLBA;

        bool m_bCheckAud;
        bool m_bCheckSub;
        bool m_bCheckVid;
        bool m_bCheckVob;
        bool m_bCheckVob2;
        bool m_bCheckLog;
        bool m_bCheckCellt;
        bool m_bCheckEndTime;

        string m_csInputIFO;
        string m_csInputPath;
        //string m_csOutputPath;
        int NumberOfPGCsInTitles, NumberOfPGCsInMenus;
        int m_iIFOlen;
        public int m_nSelPGC;
        public int m_nSelAng;
        public int m_nVid, m_nCid;
        public DemuxingMode m_iMode;
        Int64 m_i64OutputLBA;
        int m_nVobout, m_nVidout, m_nCidout;
        int m_nCurrVid;     // Used to write in different VOB files, one per VID
        public DemuxingDomain m_iDomain;
        int NumberOfVideoPacks, NumberOfAudioPacks, NumberOfSubPacks, NumberOfNavPacks, NumberOfPadPacks, NumberOfUnknownPacks;
        bool m_bVMGM;
        int TotalNumberOfFrames;
        bool bNewCell;
        int m_nLastVid, m_nLastCid;

        internal List<ADT_CELL_LIST> CellsInTitles = new();
        internal List<ADT_CELL_LIST> CellsInMenus = new();
        internal List<ADT_VID_LIST> VobIDsInTitles = new();
        internal List<ADT_VID_LIST> VobIDsInMenus = new();

        int m_iVTS_PTT_SRPT, m_iVTS_PGCI, m_iVTS_C_ADT;
        int m_iVTS_VOBU_ADMAP, m_iVTS_TMAPTI;
        int m_iVTSM_PGCI, m_iVTSM_C_ADT, m_iVTSM_VOBU_ADMAP;
        int[] m_iVTS_PGC = new int[MAX_PGC];
        int[] m_C_PBKT = new int[MAX_PGC];
        int[] m_C_POST = new int[MAX_PGC];
        int[] NumberOfCells = new int[MAX_PGC];
        int[] m_nAngles = new int[MAX_PGC];

        int[] m_iVTSM_LU = new int[MAX_LU];
        int[] m_nIniPGCinLU = new int[MAX_LU];
        int[] m_nPGCinLU = new int[MAX_LU];
        int[] m_iMENU_PGC = new int[MAX_MPGC];
        int[] m_M_C_PBKT = new int[MAX_MPGC];
        int[] m_M_C_POST = new int[MAX_MPGC];
        int[] NumberOfMenuCells = new int[MAX_MPGC];
        int[] m_nLU_MPGC = new int[MAX_MPGC];

        UInt64[] m_dwDuration = new UInt64[MAX_PGC];
        UInt64[] m_dwMDuration = new UInt64[MAX_MPGC];

        Int64[] m_i64VOBSize = new Int64[10];
        int m_nVobFiles;

        Stream[] fsub = new Stream[32];
        Stream[] faud = new Stream[8];
        Stream fvid = null;
        Stream fvob = null;
        AudioType[] m_audfmt = new AudioType[8];
        string[] m_csAudname = new string[8];
        int[] m_iFirstSubPTS = new int[32];
        int[] m_iFirstAudPTS = new int[8];
        int m_iFirstVidPTS, m_iFirstNavPTS0;
        int[] m_iAudIndex = new int[8];
        int m_iSubPTS, m_iAudPTS, m_iVidPTS, m_iNavPTS0_old, m_iNavPTS0, m_iNavPTS1_old, m_iNavPTS1;
        int m_iOffsetPTS;

        int nLU, nAbsPGC;
        int m_nLUs;

        // Only in PCM
        int[] nbitspersample = new int[8];
        int[] nchannels = new int[8];
        int[] fsample = new int[8];

        public PgcDemux(string ifo_path, IfoOptions options) {
            int i, k;

            m_pIFO = null;
            for (i = 0; i < 32; i++) fsub[i] = null;
            for (i = 0; i < 8; i++) faud[i] = null;
            fvob = fvid = null;

            m_bInProcess = false;
            m_bCLI = false;
            m_bAbort = false;
            m_bVMGM = false;

            m_bCheckAud = m_bCheckSub = m_bCheckLog = m_bCheckCellt = true;
            m_bCheckVid = m_bCheckVob = m_bCheckVob2 = m_bCheckEndTime = false;

            m_bCheckIFrame = false;
            m_bCheckLBA = m_bCheckVideoPack = m_bCheckAudioPack = m_bCheckNavPack = m_bCheckSubPack = true;

            // Parse options
            string csPar, csPar2;
            string csAux, csAux1, csAux2;

            m_nSelPGC = m_nSelAng = 0;
            m_iMode = DemuxingMode.PGC;
            m_iDomain = DemuxingDomain.Titles;

            if (options.Mode == DemuxingMode.PGC)
            {
                m_iMode = DemuxingMode.PGC;
                if (options.PGC < 1 || options.PGC > 255)
                {
                    Console.WriteLine($"Invalid PGC '{options.PGC}'. Value must be between 1 and 255 inclusive. Defaulting to 1.");
                    m_nSelPGC = 1;
                } else
                {
                    m_nSelPGC = options.PGC;
                }
                m_nSelPGC--; // internally from 0 to nPGCs-1.
            } else if (options.Mode == DemuxingMode.VID)
            {
                m_iMode = DemuxingMode.VID;
                if (options.VID < 1 || options.VID > 32768)
                {
                    Console.WriteLine($"Invalid VID '{options.VID}'. Value must be between 1 and 32768 inclusive. Defaulting to 1.");
                    m_nVid = 1;
                } else
                {
                    m_nVid = options.VID;
                }
            } else if (options.Mode == DemuxingMode.CID)
            {
                m_iMode = DemuxingMode.CID;
                m_nVid = options.VID;
                m_nCid = options.CID;
                if (m_nVid < 1 || m_nVid > 32768)
                {
                    Console.WriteLine($"Invalid VID '{m_nVid}'. Value must be between 1 and 32768 inclusive. Defaulting to 1.");
                    m_nVid = 1;
                }
                if (m_nCid < 1 || m_nCid > 255)
                {
                    Console.WriteLine($"Invalid CID '{m_nCid}'. Value must be between 1 and 255 inclusive. Defaulting to 1.");
                    m_nCid = 1;
                }
            } else
            {
                throw new Exception($"Unknown program mode '{options.Mode}'");
            }

            if (options.Angle < 1 || options.Angle > 9)
            {
                Console.WriteLine($"Invalid angle '{options.Angle}'. Value must be between 1 and 9 inclusive. Defaulting to 1.");
                m_nSelAng = 1;
            } else
            {
                m_nSelAng = options.Angle;
            }
            m_nSelAng--; // internally from 0 to nAngs-1.

            m_bCheckVob = (options.CustomVOB != null);
            if (options.CustomVOB != null)
            {
                m_bCheckVob2 = options.CustomVOB.SplitVOB;
                m_bCheckVideoPack = options.CustomVOB.WriteVideoPacks;
                m_bCheckAudioPack = options.CustomVOB.WriteAudioPacks;
                m_bCheckNavPack = options.CustomVOB.WriteNavPacks;
                m_bCheckSubPack = options.CustomVOB.WriteSubPacks;
                m_bCheckIFrame = options.CustomVOB.OnlyFirstIFrame;
                m_bCheckLBA = options.CustomVOB.PatchLbaNumber;
            }

            m_bCheckVid = options.ExtractVideo;
            m_bCheckVob = options.ExportVOB;
            m_bCheckAud = options.ExtractAudio;
            m_bCheckSub = options.ExtractSubtitles;
            m_bCheckLog = options.GenerateLog;
            m_bCheckCellt = options.GenerateCellTimes;
            m_bCheckEndTime = options.IncludeEndTime;
            m_iDomain = options.DomainType;

            m_csInputIFO = ifo_path;

            m_csInputPath = Path.GetDirectoryName(m_csInputIFO);

            m_csInputIFO = m_csInputIFO.ToUpper();
            //m_csOutputPath = m_csOutputPath.ToUpper();
            m_csInputPath = m_csInputPath.ToUpper();

            csAux = Path.GetFileName(m_csInputIFO); //m_csInputIFO.Right(m_csInputIFO.GetLength() - m_csInputIFO.ReverseFind('\\') - 1);
            csAux1 = csAux.Substring(0, 4);
            csAux = m_csInputIFO.Substring(m_csInputIFO.Length - 6);
            csAux2 = m_csInputIFO.Substring(m_csInputIFO.Length - 12);
            if ((csAux != "_0.IFO" || csAux1 != "VTS_") && csAux2 != "VIDEO_TS.IFO")
            {
                throw new Exception("Invalid input file!");
            }

            if (csAux2 == "VIDEO_TS.IFO")
            {
                m_bVMGM = true;
                m_iDomain = DemuxingDomain.Menus;
            } else m_bVMGM = false;

            ReadIFO();
        }

        public static PgcDemux? TryOpenFile(string input, IfoOptions options) {
            try
            {
                return new PgcDemux(input, options);
            } catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public bool Demux(string output_folder) {
            if (m_iMode == DemuxingMode.PGC)
            {
                // Check if PGC exists in done in PgcDemux
                if (m_iDomain == DemuxingDomain.Titles)
                    return Demux(output_folder, m_nSelPGC, m_nSelAng, null);
                else
                    return MDemux(output_folder, m_nSelPGC, null);
            } else if (m_iMode == DemuxingMode.VID)
            {
                // Look for nSelVid
                int nSelVid = -1;
                if (m_iDomain == DemuxingDomain.Titles)
                {
                    for (int k = 0; k < VobIDsInTitles.Count && nSelVid == -1; k++)
                        if (VobIDsInTitles[k].VobID == m_nVid)
                            nSelVid = k;
                } else
                {
                    for (int k = 0; k < VobIDsInMenus.Count && nSelVid == -1; k++)
                        if (VobIDsInMenus[k].VobID == m_nVid)
                            nSelVid = k;
                }
                if (nSelVid == -1)
                {
                    Console.WriteLine("Selected Vid not found!");
                    return false;
                }
                if (m_iDomain == DemuxingDomain.Titles)
                    return VIDDemux(output_folder, nSelVid, null);
                else
                    return VIDMDemux(output_folder, nSelVid, null);
            } else if (m_iMode == DemuxingMode.CID)
            {
                // Look for nSelVid
                int nSelCid = -1;
                if (m_iDomain == DemuxingDomain.Titles)
                {
                    for (int k = 0; k < CellsInTitles.Count && nSelCid == -1; k++)
                        if (CellsInTitles[k].VID == m_nVid && CellsInTitles[k].CID == m_nCid)
                            nSelCid = k;
                } else
                {
                    for (int k = 0; k < CellsInMenus.Count && nSelCid == -1; k++)
                        if (CellsInMenus[k].VID == m_nVid && CellsInMenus[k].CID == m_nCid)
                            nSelCid = k;
                }
                if (nSelCid == -1)
                {
                    Console.WriteLine("Selected Vid/Cid not found!");
                    return false;
                }
                if (m_iDomain == DemuxingDomain.Titles)
                    return CIDDemux(output_folder, nSelCid, null);
                else
                    return CIDMDemux(output_folder, nSelCid, null);
            } else
            {
                throw new Exception($"Unknown Program Mode {m_iMode}");
            }
        }

        private void ReadIFO() {
            string csAux, csAux2;
            int i, j, k, nCell, nVIDs;
            int kk = 0;
            ADT_CELL_LIST myADT_Cell;
            ADT_VID_LIST myADT_Vid;
            int nTotADT, nADT, VidADT, CidADT;
            int iArraysize;
            bool bAlready, bEndAngle;
            Stream in_;
            int iIniSec, iEndSec;

            FileInfo statbuf;
            int iSize, iCat;
            int iIFOSize;

            statbuf = new FileInfo(m_csInputIFO);
            iIFOSize = (int)statbuf.Length;

            if (iIFOSize > MAXLENGTH)
            {
                throw new Exception($"IFO too big {m_csInputIFO}");
            }

            try
            {
                in_ = File.OpenRead(m_csInputIFO);
            } catch (Exception)
            {
                in_ = null;
            }
            if (in_ == null)
            {
                throw new Exception($"Unable to open {m_csInputIFO}");
            }

            m_pIFO = new byte[iIFOSize + 2048];

            // Read IFO

            m_iIFOlen = in_.Read(m_pIFO, 0, Math.Min(MAXLENGTH, m_pIFO.Length));

            CellsInTitles.Clear();
            CellsInMenus.Clear();
            VobIDsInTitles.Clear();
            VobIDsInMenus.Clear();


            // Get Title Cells
            if (m_bVMGM)
            {
                m_iVTS_PTT_SRPT = 0;
                m_iVTS_PGCI = 0;
                m_iVTSM_PGCI = 2048 * Util.GetNbytes(4, m_pIFO.AsSpan(0xC8));
                m_iVTS_TMAPTI = 0;
                m_iVTSM_C_ADT = 2048 * Util.GetNbytes(4, m_pIFO.AsSpan(0xD8));
                m_iVTSM_VOBU_ADMAP = 2048 * Util.GetNbytes(4, m_pIFO.AsSpan(0xDC));
                m_iVTS_C_ADT = 0;
                m_iVTS_VOBU_ADMAP = 0;
            } else
            {
                m_iVTS_PTT_SRPT = 2048 * Util.GetNbytes(4, m_pIFO.AsSpan(0xC8));
                m_iVTS_PGCI = 2048 * Util.GetNbytes(4, m_pIFO.AsSpan(0xCC));
                m_iVTSM_PGCI = 2048 * Util.GetNbytes(4, m_pIFO.AsSpan(0xD0));
                m_iVTS_TMAPTI = 2048 * Util.GetNbytes(4, m_pIFO.AsSpan(0xD4));
                m_iVTSM_C_ADT = 2048 * Util.GetNbytes(4, m_pIFO.AsSpan(0xD8));
                m_iVTSM_VOBU_ADMAP = 2048 * Util.GetNbytes(4, m_pIFO.AsSpan(0xDC));
                m_iVTS_C_ADT = 2048 * Util.GetNbytes(4, m_pIFO.AsSpan(0xE0));
                m_iVTS_VOBU_ADMAP = 2048 * Util.GetNbytes(4, m_pIFO.AsSpan(0xE4));
            }
            if (m_bVMGM)
                NumberOfPGCsInTitles = 0;
            else
                NumberOfPGCsInTitles = Util.GetNbytes(2, m_pIFO.AsSpan(m_iVTS_PGCI));


            // Title PGCs	
            if (NumberOfPGCsInTitles > MAX_PGC)
            {
                throw new Exception($"ERROR: Max PGCs limit ({MAX_PGC}) has been reached.");
            }
            for (k = 0; k < NumberOfPGCsInTitles; k++)
            {
                m_iVTS_PGC[k] = Util.GetNbytes(4, m_pIFO.AsSpan(m_iVTS_PGCI + 0x04 + (k + 1) * 8)) + m_iVTS_PGCI;
                m_dwDuration[k] = (ulong)Util.GetNbytes(4, m_pIFO.AsSpan(m_iVTS_PGC[k] + 4));

                m_C_PBKT[k] = Util.GetNbytes(2, m_pIFO.AsSpan(m_iVTS_PGC[k] + 0xE8));
                if (m_C_PBKT[k] != 0) m_C_PBKT[k] += m_iVTS_PGC[k];

                m_C_POST[k] = Util.GetNbytes(2, m_pIFO.AsSpan(m_iVTS_PGC[k] + 0xEA));
                if (m_C_POST[k] != 0) m_C_POST[k] += m_iVTS_PGC[k];

                NumberOfCells[k] = m_pIFO[m_iVTS_PGC[k] + 3];


                m_nAngles[k] = 1;

                for (nCell = 0, bEndAngle = false; nCell < NumberOfCells[k] && bEndAngle == false; nCell++)
                {
                    iCat = Util.GetNbytes(1, m_pIFO.AsSpan(m_C_PBKT[k] + 24 * nCell));
                    iCat = iCat & 0xF0;
                    //			0101=First; 1001=Middle ;	1101=Last
                    if (iCat == 0x50)
                        m_nAngles[k] = 1;
                    else if (iCat == 0x90)
                        m_nAngles[k]++;
                    else if (iCat == 0xD0)
                    {
                        m_nAngles[k]++;
                        bEndAngle = true;
                    }
                }
            }


            // Menu PGCs
            if (m_iVTSM_PGCI == 0)
                m_nLUs = 0;
            else
                m_nLUs = Util.GetNbytes(2, m_pIFO.AsSpan(m_iVTSM_PGCI));

            NumberOfPGCsInMenus = 0;
            if (m_nLUs > MAX_LU)
            {
                throw new Exception($"ERROR: Max LUs limit ({MAX_LU}) has been reached.");
            }

            for (nLU = 0; nLU < m_nLUs; nLU++)
            {
                m_iVTSM_LU[nLU] = Util.GetNbytes(4, m_pIFO.AsSpan(m_iVTSM_PGCI + 0x04 + (nLU + 1) * 8)) + m_iVTSM_PGCI;
                m_nPGCinLU[nLU] = Util.GetNbytes(2, m_pIFO.AsSpan(m_iVTSM_LU[nLU]));
                m_nIniPGCinLU[nLU] = NumberOfPGCsInMenus;

                for (j = 0; j < m_nPGCinLU[nLU]; j++)
                {
                    if ((NumberOfPGCsInMenus + m_nPGCinLU[nLU]) > MAX_MPGC)
                    {
                        throw new Exception($"ERROR: Max MPGCs limit ({MAX_MPGC}) has been reached.");
                    }
                    nAbsPGC = j + NumberOfPGCsInMenus;
                    m_nLU_MPGC[nAbsPGC] = nLU;
                    m_iMENU_PGC[nAbsPGC] = Util.GetNbytes(4, m_pIFO.AsSpan(m_iVTSM_LU[nLU] + 0x04 + (j + 1) * 8)) + m_iVTSM_LU[nLU];

                    m_M_C_PBKT[nAbsPGC] = Util.GetNbytes(2, m_pIFO.AsSpan(m_iMENU_PGC[nAbsPGC] + 0xE8));
                    if (m_M_C_PBKT[nAbsPGC] != 0) m_M_C_PBKT[nAbsPGC] += m_iMENU_PGC[nAbsPGC];
                    m_M_C_POST[nAbsPGC] = Util.GetNbytes(2, m_pIFO.AsSpan(m_iMENU_PGC[nAbsPGC] + 0xEA));
                    if (m_M_C_POST[nAbsPGC] != 0) m_M_C_POST[nAbsPGC] += m_iMENU_PGC[nAbsPGC];

                    NumberOfMenuCells[nAbsPGC] = m_pIFO[m_iMENU_PGC[nAbsPGC] + 3];

                    if ((m_M_C_PBKT[nAbsPGC] == 0 || m_M_C_POST[nAbsPGC] == 0) && NumberOfMenuCells[nAbsPGC] != 0)
                    // There is something wrong...
                    {
                        NumberOfMenuCells[nAbsPGC] = 0;
                        throw new Exception($"ERROR: There is something wrong in number of cells in LU {nLU:00}, Menu PGC {j:00}.");
                    }
                    m_dwMDuration[nAbsPGC] = (ulong)Util.GetNbytes(4, m_pIFO.AsSpan(m_iMENU_PGC[nAbsPGC] + 4));

                } // For PGCs
                NumberOfPGCsInMenus += m_nPGCinLU[nLU];
            }


            ///////////// VTS_C_ADT  ///////////////////////
            if (m_iVTS_C_ADT == 0) nTotADT = 0;
            else
            {
                nTotADT = Util.GetNbytes(4, m_pIFO.AsSpan(m_iVTS_C_ADT + 4));
                nTotADT = (nTotADT - 7) / 12;
            }

            //Cells
            for (nADT = 0; nADT < nTotADT; nADT++)
            {
                VidADT = Util.GetNbytes(2, m_pIFO.AsSpan(m_iVTS_C_ADT + 8 + 12 * nADT));
                CidADT = m_pIFO[m_iVTS_C_ADT + 8 + 12 * nADT + 2];

                iArraysize = CellsInTitles.Count;
                for (k = 0, bAlready = false; k < iArraysize; k++)
                {
                    if (CidADT == CellsInTitles[k].CID &&
                        VidADT == CellsInTitles[k].VID)
                    {
                        bAlready = true;
                        kk = k;
                    }
                }
                if (!bAlready)
                {
                    myADT_Cell = new();
                    myADT_Cell.CID = CidADT;
                    myADT_Cell.VID = VidADT;
                    myADT_Cell.iSize = 0;
                    myADT_Cell.iIniSec = 0x7fffffff;
                    myADT_Cell.iEndSec = 0;
                    kk = InsertCell(myADT_Cell, DemuxingDomain.Titles);
                    //			m_AADT_Cell_list.SetAtGrow(iArraysize,myADT_Cell);
                    //			kk=iArraysize;
                }
                iIniSec = Util.GetNbytes(4, m_pIFO.AsSpan(m_iVTS_C_ADT + 8 + 12 * nADT + 4));
                iEndSec = Util.GetNbytes(4, m_pIFO.AsSpan(m_iVTS_C_ADT + 8 + 12 * nADT + 8));
                if (iIniSec < CellsInTitles[kk].iIniSec) CellsInTitles[kk].iIniSec = iIniSec;
                if (iEndSec > CellsInTitles[kk].iEndSec) CellsInTitles[kk].iEndSec = iEndSec;
                iSize = (iEndSec - iIniSec + 1);
                CellsInTitles[kk].iSize += (iEndSec - iIniSec + 1);
            }

            ///////////// VTSM_C_ADT  ///////////////////////
            if (m_iVTSM_C_ADT == 0) nTotADT = 0;
            else
            {
                nTotADT = Util.GetNbytes(4, m_pIFO.AsSpan(m_iVTSM_C_ADT + 4));
                nTotADT = (nTotADT - 7) / 12;
            }

            // Cells
            for (nADT = 0; nADT < nTotADT; nADT++)
            {
                VidADT = Util.GetNbytes(2, m_pIFO.AsSpan(m_iVTSM_C_ADT + 8 + 12 * nADT));
                CidADT = m_pIFO[m_iVTSM_C_ADT + 8 + 12 * nADT + 2];

                iArraysize = CellsInMenus.Count;
                for (k = 0, bAlready = false; k < iArraysize; k++)
                {
                    if (CidADT == CellsInMenus[k].CID &&
                        VidADT == CellsInMenus[k].VID)
                    {
                        bAlready = true;
                        kk = k;
                    }
                }
                if (!bAlready)
                {
                    myADT_Cell = new();
                    myADT_Cell.CID = CidADT;
                    myADT_Cell.VID = VidADT;
                    myADT_Cell.iSize = 0;
                    myADT_Cell.iIniSec = 0x7fffffff;
                    myADT_Cell.iEndSec = 0;
                    kk = InsertCell(myADT_Cell, DemuxingDomain.Menus);
                    //			m_MADT_Cell_list.SetAtGrow(iArraysize,myADT_Cell);
                    //			kk=iArraysize;
                }
                iIniSec = Util.GetNbytes(4, m_pIFO.AsSpan(m_iVTSM_C_ADT + 8 + 12 * nADT + 4));
                iEndSec = Util.GetNbytes(4, m_pIFO.AsSpan(m_iVTSM_C_ADT + 8 + 12 * nADT + 8));
                if (iIniSec < CellsInMenus[kk].iIniSec) CellsInMenus[kk].iIniSec = iIniSec;
                if (iEndSec > CellsInMenus[kk].iEndSec) CellsInMenus[kk].iEndSec = iEndSec;
                iSize = (iEndSec - iIniSec + 1);
                CellsInMenus[kk].iSize += (iEndSec - iIniSec + 1);
            }

            FillDurations();

            //////////////////////////////////////////////////////////////	
            /////////////   VIDs
            // VIDs in Titles
            iArraysize = CellsInTitles.Count;
            for (i = 0; i < iArraysize; i++)
            {
                VidADT = CellsInTitles[i].VID;

                nVIDs = VobIDsInTitles.Count;
                for (k = 0, bAlready = false; k < nVIDs; k++)
                {
                    if (VidADT == VobIDsInTitles[k].VobID)
                    {
                        bAlready = true;
                        kk = k;
                    }
                }
                if (!bAlready)
                {
                    myADT_Vid = new();
                    myADT_Vid.VobID = VidADT;
                    myADT_Vid.iSize = 0;
                    myADT_Vid.NumberOfCells = 0;
                    myADT_Vid.dwDuration = 0;
                    VobIDsInTitles.Add(myADT_Vid);
                    kk = nVIDs;
                }
                VobIDsInTitles[kk].iSize += CellsInTitles[i].iSize;
                VobIDsInTitles[kk].NumberOfCells++;
                VobIDsInTitles[kk].dwDuration = Util.AddDuration(CellsInTitles[i].dwDuration, VobIDsInTitles[kk].dwDuration);
            }

            // VIDs in Menus
            iArraysize = CellsInMenus.Count;
            for (i = 0; i < iArraysize; i++)
            {
                VidADT = CellsInMenus[i].VID;

                nVIDs = VobIDsInMenus.Count;
                for (k = 0, bAlready = false; k < nVIDs; k++)
                {
                    if (VidADT == VobIDsInMenus[k].VobID)
                    {
                        bAlready = true;
                        kk = k;
                    }
                }
                if (!bAlready)
                {
                    myADT_Vid = new();
                    myADT_Vid.VobID = VidADT;
                    myADT_Vid.iSize = 0;
                    myADT_Vid.NumberOfCells = 0;
                    myADT_Vid.dwDuration = 0;
                    VobIDsInMenus.Add(myADT_Vid);
                    kk = nVIDs;
                }
                VobIDsInMenus[kk].iSize += CellsInMenus[i].iSize;
                VobIDsInMenus[kk].NumberOfCells++;
                VobIDsInMenus[kk].dwDuration = Util.AddDuration(CellsInMenus[i].dwDuration, VobIDsInMenus[kk].dwDuration);
            }

            // Fill VOB file size
            if (m_bVMGM)
            {
                m_nVobFiles = 0;

                for (k = 0; k < 10; k++)
                    m_i64VOBSize[k] = 0;

                csAux2 = m_csInputIFO.Substring(0, m_csInputIFO.Length - 3);
                csAux = csAux2 + "VOB";
                statbuf = new FileInfo(csAux);
                m_i64VOBSize[0] = statbuf.Length;
            } else
            {
                for (k = 0; k < 10; k++)
                {
                    csAux2 = m_csInputIFO.Substring(0, m_csInputIFO.Length - 5);
                    csAux = $"{k}.VOB";
                    csAux = csAux2 + csAux;
                    if (File.Exists(csAux))
                    {
                        statbuf = new FileInfo(csAux);
                        m_i64VOBSize[k] = statbuf.Length;
                        m_nVobFiles = k;
                    } else
                        m_i64VOBSize[k] = 0;
                }
            }
        }

        int InsertCell(ADT_CELL_LIST myADT_Cell, DemuxingDomain iDomain) {
            int iArraysize, i;
            int ii = -1;
            bool bIsHigher;

            if (iDomain == DemuxingDomain.Titles)
            {
                iArraysize = CellsInTitles.Count;
                ii = iArraysize;
                for (i = 0, bIsHigher = true; i < iArraysize && bIsHigher; i++)
                {
                    if (myADT_Cell.VID < CellsInTitles[i].VID) { ii = i; bIsHigher = false; } else if (myADT_Cell.VID > CellsInTitles[i].VID) bIsHigher = true;
                    else
                    {
                        if (myADT_Cell.CID < CellsInTitles[i].CID) { ii = i; bIsHigher = false; } else if (myADT_Cell.CID > CellsInTitles[i].CID) bIsHigher = true;
                    }

                }
                CellsInTitles.Insert(ii, myADT_Cell);
            }
            if (iDomain == DemuxingDomain.Menus)
            {
                iArraysize = CellsInMenus.Count;
                ii = iArraysize;
                for (i = 0, bIsHigher = true; i < iArraysize && bIsHigher; i++)
                {
                    if (myADT_Cell.VID < CellsInMenus[i].VID) { ii = i; bIsHigher = false; } else if (myADT_Cell.VID > CellsInMenus[i].VID) bIsHigher = true;
                    else
                    {
                        if (myADT_Cell.CID < CellsInMenus[i].CID) { ii = i; bIsHigher = false; } else if (myADT_Cell.CID > CellsInMenus[i].CID) bIsHigher = true;
                    }

                }
                //		if (i>0 && bIsHigher) i--;
                CellsInMenus.Insert(ii, myADT_Cell);
            }
            return ii;
        }

        void FillDurations() {
            int iArraysize;
            int i, j, k;
            int VIDa, CIDa, VIDb, CIDb;
            bool bFound;
            int iVideoAttr, iFormat;


            iArraysize = CellsInTitles.Count;

            for (i = 0; i < iArraysize; i++)
            {
                VIDb = CellsInTitles[i].VID;
                CIDb = CellsInTitles[i].CID;
                for (j = 0, bFound = false; j < NumberOfPGCsInTitles && !bFound; j++)
                {
                    for (k = 0; k < NumberOfCells[j]; k++)
                    {
                        VIDa = Util.GetNbytes(2, m_pIFO.AsSpan(m_C_POST[j] + k * 4));
                        CIDa = m_pIFO[m_C_POST[j] + k * 4 + 3];
                        if (VIDa == VIDb && CIDa == CIDb)
                        {
                            bFound = true;
                            CellsInTitles[i].dwDuration = (ulong)Util.GetNbytes(4, m_pIFO.AsSpan(m_C_PBKT[j] + 0x18 * k + 4));
                        }
                    }
                }
                if (!bFound)
                {
                    iVideoAttr = m_pIFO[0x200] * 256 + m_pIFO[0x201];
                    iFormat = (iVideoAttr & 0x1000) >> 12;
                    if (iFormat == 0) // NTSC
                        CellsInTitles[i].dwDuration = 0xC0;
                    else // PAL
                        CellsInTitles[i].dwDuration = 0x40;
                }
            }

            iArraysize = CellsInMenus.Count;

            for (i = 0; i < iArraysize; i++)
            {
                VIDb = CellsInMenus[i].VID;
                CIDb = CellsInMenus[i].CID;
                for (j = 0, bFound = false; j < NumberOfPGCsInMenus && !bFound; j++)
                {
                    for (k = 0; k < NumberOfMenuCells[j]; k++)
                    {
                        VIDa = Util.GetNbytes(2, m_pIFO.AsSpan(m_M_C_POST[j] + k * 4));
                        CIDa = m_pIFO[m_M_C_POST[j] + k * 4 + 3];
                        if (VIDa == VIDb && CIDa == CIDb)
                        {
                            bFound = true;
                            CellsInMenus[i].dwDuration = (ulong)Util.GetNbytes(4, m_pIFO.AsSpan(m_M_C_PBKT[j] + 0x18 * k + 4));
                        }
                    }
                }
                if (!bFound)
                {
                    iVideoAttr = m_pIFO[0x100] * 256 + m_pIFO[0x101];
                    iFormat = (iVideoAttr & 0x1000) >> 12;
                    if (iFormat == 0) // NTSC
                        CellsInMenus[i].dwDuration = 0xC0;
                    else // PAL
                        CellsInMenus[i].dwDuration = 0x40;
                }
            }

        }

        bool Demux(string m_csOutputPath, int nPGC, int nAng, object? pDlg) {
            int nTotalSectors;
            int nSector, nCell;
            int k, iArraysize;
            int CID, VID;
            Int64 i64IniSec, i64EndSec;
            Int64 i64sectors;
            int nVobin = 0;
            string csAux, csAux2;
            Stream in_;
            Stream fout;
            Int64 i64;
            bool bMyCell;
            bool iRet;
            ulong dwCellDuration;
            int nFrames;
            int nCurrAngle, iCat;

            if (nPGC >= NumberOfPGCsInTitles)
            {
                Console.WriteLine("Error: PGC does not exist");
                m_bInProcess = false;
                return false;
            }

            IniDemuxGlobalVars();
            if (OpenVideoFile(m_csOutputPath) == false) return false;
            m_bInProcess = true;

            // Calculate  the total number of sectors
            nTotalSectors = 0;
            iArraysize = CellsInTitles.Count;
            for (nCell = nCurrAngle = 0; nCell < NumberOfCells[nPGC]; nCell++)
            {
                VID = Util.GetNbytes(2, m_pIFO.AsSpan(m_C_POST[nPGC] + 4 * nCell));
                CID = m_pIFO[m_C_POST[nPGC] + 3 + 4 * nCell];

                iCat = m_pIFO[m_C_PBKT[nPGC] + 24 * nCell];
                iCat = iCat & 0xF0;
                //		0101=First; 1001=Middle ;	1101=Last
                if (iCat == 0x50)
                    nCurrAngle = 1;
                else if ((iCat == 0x90 || iCat == 0xD0) && nCurrAngle != 0)
                    nCurrAngle++;
                if (iCat == 0 || (nAng + 1) == nCurrAngle)
                {
                    for (k = 0; k < iArraysize; k++)
                    {
                        if (CID == CellsInTitles[k].CID &&
                            VID == CellsInTitles[k].VID)
                        {
                            nTotalSectors += CellsInTitles[k].iSize;
                        }
                    }
                }
                if (iCat == 0xD0) nCurrAngle = 0;
            }

            nSector = 0;
            iRet = true;
            for (nCell = nCurrAngle = 0; nCell < NumberOfCells[nPGC] && m_bInProcess == true; nCell++)
            {
                iCat = m_pIFO[m_C_PBKT[nPGC] + 24 * nCell];
                iCat = iCat & 0xF0;
                //		0101=First; 1001=Middle ;	1101=Last
                if (iCat == 0x50)
                    nCurrAngle = 1;
                else if ((iCat == 0x90 || iCat == 0xD0) && nCurrAngle != 0)
                    nCurrAngle++;
                if (iCat == 0 || (nAng + 1) == nCurrAngle)
                {

                    VID = Util.GetNbytes(2, m_pIFO.AsSpan(m_C_POST[nPGC] + 4 * nCell));
                    CID = m_pIFO[m_C_POST[nPGC] + 3 + 4 * nCell];

                    i64IniSec = Util.GetNbytes(4, m_pIFO.AsSpan(m_C_PBKT[nPGC] + nCell * 24 + 8));
                    i64EndSec = Util.GetNbytes(4, m_pIFO.AsSpan(m_C_PBKT[nPGC] + nCell * 24 + 0x14));
                    for (k = 1, i64sectors = 0; k < 10; k++)
                    {
                        i64sectors += (m_i64VOBSize[k] / 2048);
                        if (i64IniSec < i64sectors)
                        {
                            i64sectors -= (m_i64VOBSize[k] / 2048);
                            nVobin = k;
                            k = 20;
                        }
                    }
                    csAux2 = m_csInputIFO.Substring(0, m_csInputIFO.Length - 5);
                    csAux = $"{nVobin}.VOB";
                    csAux = csAux2 + csAux;
                    try
                    {
                        in_ = File.OpenRead(csAux);
                    } catch (Exception)
                    {
                        in_ = null;
                    }
                    if (in_ == null)
                    {
                        Console.WriteLine("Error opening input VOB: " + csAux);
                        m_bInProcess = false;
                        iRet = false;
                    }
                    if (m_bInProcess) in_.Seek((long)((i64IniSec - i64sectors) * 2048), SeekOrigin.Begin);

                    for (i64 = 0, bMyCell = true; i64 < (i64EndSec - i64IniSec + 1) && m_bInProcess == true; i64++)
                    {
                        //readpack
                        if ((i64 % MODUPDATE) == 0) UpdateProgress(pDlg, (int)((100 * nSector) / nTotalSectors));
                        if (Util.readbuffer(m_buffer, in_) != 2048)
                        {
                            if (in_ != null) in_.Close();
                            nVobin++;
                            csAux2 = m_csInputIFO.Substring(0, m_csInputIFO.Length - 5);
                            csAux = $"{nVobin}.VOB";
                            csAux = csAux2 + csAux;
                            in_ = File.OpenRead(csAux);
                            if (Util.readbuffer(m_buffer, in_) != 2048)
                            {
                                Console.WriteLine("Input error: Reached end of VOB too early");
                                m_bInProcess = false;
                                iRet = false;
                            }
                        }

                        if (m_bInProcess == true)
                        {
                            if (Util.IsSynch(m_buffer) != true)
                            {
                                Console.WriteLine("Error reading input VOB: Unsynchronized");
                                m_bInProcess = false;
                                iRet = false;
                            }
                            if (Util.IsNav(m_buffer))
                            {
                                if (m_buffer[0x420] == (byte)(VID % 256) &&
                                    m_buffer[0x41F] == (byte)(VID / 256) &&
                                    m_buffer[0x422] == (byte)CID)
                                    bMyCell = true;
                                else
                                    bMyCell = false;
                            }

                            if (bMyCell)
                            {
                                nSector++;
                                iRet = ProcessPack(m_csOutputPath, true);
                            }

                        }
                    } // For readpacks
                    if (in_ != null) in_.Close();
                    in_ = null;
                }  // if (iCat==0 || (nAng+1) == nCurrAngle)
                if (iCat == 0xD0) nCurrAngle = 0;
            }   // For Cells 

            CloseAndNull();
            nFrames = 0;

            if (m_bCheckCellt && m_bInProcess == true)
            {
                csAux = Path.Combine(m_csOutputPath, "Celltimes.txt");
                fout = File.Open(csAux, FileMode.Create);
                for (nCell = 0, nCurrAngle = 0; nCell < NumberOfCells[nPGC] && m_bInProcess == true; nCell++)
                {
                    dwCellDuration = (ulong)Util.GetNbytes(4, m_pIFO.AsSpan(m_C_PBKT[nPGC] + 24 * nCell + 4));

                    iCat = m_pIFO[m_C_PBKT[nPGC] + 24 * nCell];
                    iCat = iCat & 0xF0;
                    //			0101=First; 1001=Middle ;	1101=Last
                    if (iCat == 0x50)
                        nCurrAngle = 1;
                    else if ((iCat == 0x90 || iCat == 0xD0) && nCurrAngle != 0)
                        nCurrAngle++;
                    if (iCat == 0 || (nAng + 1) == nCurrAngle)
                    {
                        nFrames += Util.DurationInFrames(dwCellDuration);
                        if (nCell != (NumberOfCells[nPGC] - 1) || m_bCheckEndTime)
                        {
                            var writer = new StreamWriter(fout, Encoding.ASCII);
                            writer.Write($"{nFrames}\n");
                            writer.Flush();
                        }
                    }

                    if (iCat == 0xD0) nCurrAngle = 0;
                }
                fout.Close();
            }

            TotalNumberOfFrames = nFrames;

            if (m_bCheckLog && m_bInProcess == true) OutputLog(nPGC, nAng, DemuxingDomain.Titles, Console.Out);

            return iRet;
        }

        void IniDemuxGlobalVars() {
            int k;
            string csAux;

            // clear PTS
            for (k = 0; k < 32; k++)
                m_iFirstSubPTS[k] = 0;
            for (k = 0; k < 8; k++)
            {
                m_iFirstAudPTS[k] = 0;
                nchannels[k] = -1;
                nbitspersample[k] = -1;
                fsample[k] = -1;
            }
            m_iFirstVidPTS = 0;
            m_iFirstNavPTS0 = 0;
            m_iNavPTS0_old = m_iNavPTS0 = 0;
            m_iNavPTS1_old = m_iNavPTS1 = 0;

            NumberOfNavPacks = NumberOfVideoPacks = NumberOfAudioPacks = NumberOfSubPacks = NumberOfUnknownPacks = NumberOfPadPacks = 0;
            m_i64OutputLBA = 0;
            m_nVobout = m_nVidout = m_nCidout = 0;
            m_nLastVid = 0;
            m_nLastCid = 0;

            m_nCurrVid = 0;
            m_iOffsetPTS = 0;
            bNewCell = false;
        }

        bool OpenVideoFile(string m_csOutputPath) {
            string csAux;

            if (m_bCheckVid)
            {
                csAux = Path.Combine(m_csOutputPath, "VideoFile.m2v");
                try
                {
                    fvid = File.Open(csAux, FileMode.Create);
                } catch (Exception)
                {
                    fvid = null;
                }
                if (fvid == null) return false;
            }

            return true;
        }

        void UpdateProgress(object? pDlg, int nPerc) {
            // TODO dialog progress bar
        }

        private static int nPack = 0;
        private static int nFirstRef = 0;

        bool ProcessPack(string m_csOutputPath, bool bWrite) {
            int sID;
            bool bFirstAud;
            int nBytesOffset;

            if (bWrite && m_bCheckVob)
            {
                if (Util.IsNav(m_buffer))
                {
                    if (m_bCheckLBA) Util.ModifyLBA(m_buffer, m_i64OutputLBA);
                    m_nVidout = (int)Util.GetNbytes(2, m_buffer.AsSpan(0x41f));
                    m_nCidout = (int)m_buffer[0x422];
                    nFirstRef = (int)Util.GetNbytes(4, m_buffer.AsSpan(0x413));
                    nPack = 0;

                    bNewCell = false;
                    if (m_nVidout != m_nLastVid || m_nCidout != m_nLastCid)
                    {
                        bNewCell = true;
                        m_nLastVid = m_nVidout;
                        m_nLastCid = m_nCidout;
                    }
                } else
                    nPack++;
                if ((Util.IsNav(m_buffer) && m_bCheckNavPack) ||
                     (Util.IsAudio(m_buffer) && m_bCheckAudioPack) ||
                     (Util.IsSubs(m_buffer) && m_bCheckSubPack))
                    WritePack(m_csOutputPath, m_buffer);
                else if (Util.IsVideo(m_buffer) && m_bCheckVideoPack)
                {
                    if (!m_bCheckIFrame)
                        WritePack(m_csOutputPath, m_buffer);
                    else
                    {
                        //				if (nFirstRef == nPack)  
                        //					if ( ! PatchEndOfSequence(m_buffer))
                        //						WritePack (Pad_pack);
                        if (bNewCell && nFirstRef >= nPack) WritePack(m_csOutputPath, m_buffer);
                    }
                }

            }
            if (Util.IsNav(m_buffer))
            {
                // do nothing
                NumberOfNavPacks++;
                m_iNavPTS0 = (int)Util.GetNbytes(4, m_buffer.AsSpan(0x39));
                m_iNavPTS1 = (int)Util.GetNbytes(4, m_buffer.AsSpan(0x3d));
                if (m_iFirstNavPTS0 == 0) m_iFirstNavPTS0 = m_iNavPTS0;
                if (m_iNavPTS1_old > m_iNavPTS0)
                {
                    // Discontinuity, so add the offset 
                    m_iOffsetPTS += (m_iNavPTS1_old - m_iNavPTS0);
                }
                m_iNavPTS0_old = m_iNavPTS0;
                m_iNavPTS1_old = m_iNavPTS1;
            } else if (Util.IsVideo(m_buffer))
            {
                NumberOfVideoPacks++;
                if ((m_buffer[0x15] & 0x80) != 0)
                {
                    m_iVidPTS = Util.readpts(m_buffer.AsSpan(0x17));
                    if (m_iFirstVidPTS == 0) m_iFirstVidPTS = m_iVidPTS;
                }
                if (bWrite && m_bCheckVid) demuxvideo(m_buffer);
            } else if (Util.IsAudio(m_buffer))
            {
                NumberOfAudioPacks++;
                nBytesOffset = 0;

                sID = Util.getAudId(m_buffer) & 0x07;

                bFirstAud = false;

                if ((m_buffer[0x15] & 0x80) != 0)
                {
                    if (m_iFirstAudPTS[sID] == 0)
                    {
                        bFirstAud = true;
                        m_iAudPTS = Util.readpts(m_buffer.AsSpan(0x17));
                        m_iFirstAudPTS[sID] = m_iAudPTS;
                        //				m_iAudIndex[sID]=m_buffer[0x17+m_buffer[0x16]];
                        m_iAudIndex[sID] = Util.getAudId(m_buffer);
                    }
                }
                if (bFirstAud)
                {
                    nBytesOffset = GetAudHeader(m_buffer);
                    if (nBytesOffset < 0)
                        // This pack does not have an Audio Frame Header, so its PTS is  not valid.
                        m_iFirstAudPTS[sID] = 0;
                }

                if (bWrite && m_bCheckAud && m_iFirstAudPTS[sID] != 0)
                {
                    demuxaudio(m_csOutputPath, m_buffer, nBytesOffset);
                }
            } else if (Util.IsSubs(m_buffer))
            {
                NumberOfSubPacks++;
                sID = m_buffer[0x17 + m_buffer[0x16]] & 0x1F;

                if ((m_buffer[0x15] & 0x80) != 0)
                {
                    m_iSubPTS = Util.readpts(m_buffer.AsSpan(0x17));
                    if (m_iFirstSubPTS[sID] == 0)
                        m_iFirstSubPTS[sID] = m_iSubPTS;
                }
                if (bWrite && m_bCheckSub) demuxsubs(m_csOutputPath, m_buffer);
            } else if (Util.IsPad(m_buffer))
            {
                NumberOfPadPacks++;
            } else
            {
                NumberOfUnknownPacks++;
            }
            return true;
        }

        void WritePack(string m_csOutputPath, ReadOnlySpan<byte> buffer) {
            string csAux;

            if (m_bInProcess == true)
            {
                if (m_bCheckVob2)
                {
                    if (fvob == null || m_nVidout != m_nCurrVid)
                    {
                        m_nCurrVid = m_nVidout;
                        if (fvob != null) fvob.Close();
                        if (m_iDomain == DemuxingDomain.Titles)
                            csAux = $"VTS_01_1_{m_nVidout:000}.VOB";
                        else
                            csAux = $"VTS_01_0_{m_nVidout:000}.VOB";
                        csAux = Path.Combine(m_csOutputPath, csAux);
                        fvob = File.Open(csAux, FileMode.Create);
                    }
                } else
                {
                    if (fvob == null || ((m_i64OutputLBA) % (512 * 1024 - 1)) == 0)
                    {
                        if (fvob != null) fvob.Close();
                        if (m_iDomain == DemuxingDomain.Titles)
                        {
                            m_nVobout++;
                            csAux = $"VTS_01_{m_nVobout}.VOB";
                        } else
                            csAux = "VTS_01_0.VOB";

                        csAux = Path.Combine(m_csOutputPath, csAux);
                        fvob = File.Open(csAux, FileMode.Create);
                    }
                }

                if (fvob != null) Util.writebuffer(buffer, fvob, 2048);
                m_i64OutputLBA++;
            }

        }

        void CloseAndNull() {
            int i;
            uint byterate, nblockalign;

            FileInfo statbuf;
            Int64 i64size;


            if (fvob != null)
            {
                fvob.Close();
                fvob = null;
            }
            if (fvid != null)
            {
                fvid.Close();
                fvid = null;
            }
            for (i = 0; i < 32; i++)
                if (fsub[i] != null)
                {
                    fsub[i].Close();
                    fsub[i] = null;
                }
            for (i = 0; i < 8; i++)
            {
                if (faud[i] != null)
                {
                    if (m_audfmt[i] == AudioType.WAV)
                    {
                        i64size = 0;
                        faud[i].Close();

                        statbuf = new FileInfo(m_csAudname[i]);
                        i64size = statbuf.Length;

                        if (i64size >= 8) i64size -= 8;

                        faud[i] = File.Open(m_csAudname[i], FileMode.Open, FileAccess.ReadWrite, FileShare.None);

                        faud[i].Seek(4, SeekOrigin.Begin);
                        faud[i].WriteByte((byte)(i64size % 256));
                        faud[i].WriteByte((byte)((i64size >> 8) % 256));
                        faud[i].WriteByte((byte)((i64size >> 16) % 256));
                        faud[i].WriteByte((byte)((i64size >> 24) % 256));

                        //				# of channels (2 bytes!!)
                        faud[i].Seek(22, SeekOrigin.Begin);
                        faud[i].WriteByte((byte)(nchannels[i] % 256));

                        //				Sample rate ( 48k / 96k in DVD)
                        faud[i].Seek(24, SeekOrigin.Begin);
                        faud[i].WriteByte((byte)(fsample[i] % 256));
                        faud[i].WriteByte((byte)((fsample[i] >> 8) % 256));
                        faud[i].WriteByte((byte)((fsample[i] >> 16) % 256));
                        faud[i].WriteByte((byte)((fsample[i] >> 24) % 256));

                        //				Byte rate ( 4 bytes)== SampleRate * NumChannels * BitsPerSample/8
                        //                    6000* NumChannels * BitsPerSample
                        byterate = (uint)((fsample[i] / 8) * nchannels[i] * nbitspersample[i]);
                        faud[i].Seek(28, SeekOrigin.Begin);
                        faud[i].WriteByte((byte)(byterate % 256));
                        faud[i].WriteByte((byte)((byterate >> 8) % 256));
                        faud[i].WriteByte((byte)((byterate >> 16) % 256));
                        faud[i].WriteByte((byte)((byterate >> 24) % 256));


                        //				Block align ( 2 bytes)== NumChannels * BitsPerSample/8
                        nblockalign = (uint)(nbitspersample[i] * nchannels[i] / 8);
                        faud[i].Seek(32, SeekOrigin.Begin);
                        faud[i].WriteByte((byte)(nblockalign % 256));
                        faud[i].WriteByte((byte)((nblockalign >> 8) % 256));

                        //				Bits per sample ( 2 bytes)
                        faud[i].Seek(32, SeekOrigin.Begin);
                        faud[i].WriteByte((byte)(nbitspersample[i] % 256));

                        if (i64size >= 36) i64size -= 36;
                        faud[i].Seek(40, SeekOrigin.Begin);
                        //				fseek(faud[i],54,SEEK_SET);
                        faud[i].WriteByte((byte)(i64size % 256));
                        faud[i].WriteByte((byte)((i64size >> 8) % 256));
                        faud[i].WriteByte((byte)((i64size >> 16) % 256));
                        faud[i].WriteByte((byte)((i64size >> 24) % 256));
                    }
                    faud[i].Close();
                    faud[i] = null;
                }
            }
        }

        void demuxvideo(ReadOnlySpan<byte> buffer) {

            int start, nbytes;

            start = 0x17 + buffer[0x16];
            nbytes = buffer[0x12] * 256 + buffer[0x13] + 0x14;

            Util.writebuffer(buffer.Slice(start), fvid, nbytes - start);

        }

        int GetAudHeader(ReadOnlySpan<byte> buffer)
        // Returns the number of bytes from audio start until first header
        // If no header found  returns -1
        {
            int i, start, nbytes;
            byte streamID;
            int firstheader, nHeaders;
            bool bFound = false;

            start = 0x17 + buffer[0x16];
            nbytes = buffer[0x12] * 256 + buffer[0x13] + 0x14;
            if (Util.IsAudMpeg(buffer))
                streamID = buffer[0x11];
            else
            {
                streamID = buffer[start];
                start += 4;
            }

            firstheader = 0;

            // Check if PCM
            if (streamID >= 0xa0 && streamID <= 0xa7) return 0;
            if (streamID >= 0x80 && streamID <= 0x8f)
            {
                // Stream is AC3 or DTS...
                nHeaders = buffer[start - 3];
                if (nHeaders != 0)
                {
                    bFound = true;
                    firstheader = buffer[start - 2] * 256 + buffer[start - 1] - 1;
                } else
                    bFound = false;
            } else if (streamID >= 0xc0 && streamID <= 0xc7)
            {
                // Stream is MPEG ...
                for (i = start, bFound = false; i < (nbytes - 1) && bFound == false; i++)
                {
                    //			if ( buffer[start+i] == 0xFF && (buffer[start+1+i] & 0xF0 )== 0xF0 )
                    if (buffer[i] == 0xFF && (buffer[i + 1] & 0xF0) == 0xF0)
                    {
                        bFound = true;
                        firstheader = i - start;
                    }
                }
            }

            if ((start + firstheader) >= nbytes) bFound = false;

            if (bFound)
                return firstheader;
            else
                return -1;

        }

        void demuxaudio(string m_csOutputPath, ReadOnlySpan<byte> buffer, int nBytesOffset) {
            int start, nbytes, i, j;
            int nbit, ncha;
            byte streamID;
            byte[] mybuffer = new byte[2050];

            start = 0x17 + buffer[0x16];
            nbytes = buffer[0x12] * 256 + buffer[0x13] + 0x14;
            if (Util.IsAudMpeg(buffer))
                streamID = buffer[0x11];
            else
            {
                streamID = buffer[start];
                start += 4;
            }

            // Open File descriptor if it isn't open
            if (check_aud_open(m_csOutputPath, streamID) == false)
                return;

            // Check if PCM
            if (streamID >= 0xa0 && streamID <= 0xa7)
            {
                start += 3;

                if (nchannels[streamID & 0x7] == -1)
                    nchannels[streamID & 0x7] = (buffer[0x17 + buffer[0x16] + 5] & 0x7) + 1;

                nbit = (buffer[0x17 + buffer[0x16] + 5] >> 6) & 0x3;

                if (nbit == 0) nbit = 16;
                else if (nbit == 1) nbit = 20;
                else if (nbit == 2) nbit = 24;
                else nbit = 0;

                if (nbitspersample[streamID & 0x7] == -1)
                    nbitspersample[streamID & 0x7] = nbit;
                if (nbitspersample[streamID & 0x7] != nbit)
                    nbit = nbitspersample[streamID & 0x7];

                if (fsample[streamID & 0x7] == -1)
                {
                    fsample[streamID & 0x7] = (buffer[0x17 + buffer[0x16] + 5] >> 4) & 0x3;
                    if (fsample[streamID & 0x7] == 0) fsample[streamID & 0x7] = 48000;
                    else fsample[streamID & 0x7] = 96000;
                }

                ncha = nchannels[streamID & 0x7];
                if (nbit == 24)
                {
                    for (j = start; j < (nbytes - 6 * ncha + 1); j += (6 * ncha))
                    {
                        for (i = 0; i < 2 * ncha; i++)
                        {
                            mybuffer[j + 3 * i + 2] = buffer[j + 2 * i];
                            mybuffer[j + 3 * i + 1] = buffer[j + 2 * i + 1];
                            mybuffer[j + 3 * i] = buffer[j + 4 * ncha + i];
                        }
                    }

                } else if (nbit == 16)
                {
                    for (i = start; i < (nbytes - 1); i += 2)
                    {
                        mybuffer[i] = buffer[i + 1];
                        mybuffer[i + 1] = buffer[i];
                    }
                } else if (nbit == 20)
                {
                    for (j = start; j < (nbytes - 5 * ncha + 1); j += (5 * ncha))
                    {
                        for (i = 0; i < ncha; i++)
                        {
                            mybuffer[j + 5 * i + 0] = (byte)((hi_nib(buffer[j + 4 * ncha + i]) << 4) + hi_nib(buffer[j + 4 * i + 1]));
                            mybuffer[j + 5 * i + 1] = (byte)((lo_nib(buffer[j + 4 * i + 1]) << 4) + hi_nib(buffer[j + 4 * i + 0]));
                            mybuffer[j + 5 * i + 2] = (byte)((lo_nib(buffer[j + 4 * i + 0]) << 4) + lo_nib(buffer[j + 4 * ncha + i]));
                            mybuffer[j + 5 * i + 3] = buffer[j + 4 * i + 3];
                            mybuffer[j + 5 * i + 4] = buffer[j + 4 * i + 2];
                        }
                    }
                }

                if ((nbit == 16 && ((nbytes - start) % 2) != 0) ||
                    (nbit == 24 && ((nbytes - start) % (6 * ncha)) != 0) ||
                    (nbit == 20 && ((nbytes - start) % (5 * ncha)) != 0))

                    Console.WriteLine("Error: Uncompleted PCM sample");

                // if PCM do not take into account nBytesOffset
                Util.writebuffer(mybuffer.AsSpan(start), faud[streamID & 0x7], nbytes - start);
            } else
            {
                // Very easy, no process at all, but take into account nBytesOffset...
                start += nBytesOffset;
                Util.writebuffer(buffer.Slice(start), faud[streamID & 0x7], nbytes - start);

            }

        }

        bool check_aud_open(string m_csOutputPath, byte i) {
            string csAux;
            byte ii;
            /*
            0x80-0x87: ac3  --> ac3
            0x88-0x8f: dts  --> dts
            0x90-0x97: sdds --> dds
            0x98-0x9f: unknown
            0xa0-0xa7: lpcm  -->wav
            0xa8-0xaf: unknown
            0xb0-0xbf: unknown
            0xc0-0xc8: mpeg1 --> mpa
            0xc8-0xcf: unknown 
            0xd0-0xd7: mpeg2 --> mpb
            0xd8-0xdf: unknown 
            ---------------------------------------------
            SDSS   AC3   DTS   LPCM   MPEG-1   MPEG-2

             90    80    88     A0     C0       D0
             91    81    89     A1     C1       D1
             92    82    8A     A2     C2       D2
             93    83    8B     A3     C3       D3
             94    84    8C     A4     C4       D4
             95    85    8D     A5     C5       D5
             96    86    8E     A6     C6       D6
             97    87    8F     A7     C7       D7
            ---------------------------------------------
            */

            ii = i;

            if (ii < 0x80) return false;

            i = (byte)(i & 0x7);

            if (faud[i] == null)
            {
                if (ii >= 0x80 && ii <= 0x87)
                {
                    csAux = $"AudioFile_{(i + 0x80):X2}X.ac3";
                    m_audfmt[i] = AudioType.AC3;
                } else if (ii >= 0x88 && ii <= 0x8f)
                {
                    csAux = $"AudioFile_{(i + 0x88):X2}.dts";
                    m_audfmt[i] = AudioType.DTS;
                } else if (ii >= 0x90 && ii <= 0x97)
                {
                    csAux = $"AudioFile_{(i + 0x90):X2}.dds";
                    m_audfmt[i] = AudioType.DDS;
                } else if (ii >= 0xa0 && ii <= 0xa7)
                {
                    csAux = $"AudioFile_{(i + 0xa0):X2}.wav";
                    m_audfmt[i] = AudioType.WAV;
                } else if (ii >= 0xc0 && ii <= 0xc7)
                {
                    csAux = $"AudioFile_{(i + 0xc0):X2}.mpa";
                    m_audfmt[i] = AudioType.MP1;
                } else if (ii >= 0xd0 && ii <= 0xd7)
                {
                    csAux = $"AudioFile_{(i + 0xd0):X2}.mpa";
                    m_audfmt[i] = AudioType.MP2;
                } else
                {
                    csAux = $"AudioFile_{(ii):X2}.unk";
                    m_audfmt[i] = AudioType.UNK;
                }

                csAux = Path.Combine(m_csOutputPath, csAux);
                m_csAudname[i] = csAux;

                try
                {
                    faud[i] = File.Open(csAux, FileMode.Create);
                } catch (Exception)
                {
                    faud[i] = null;
                }
                if (faud[i] == null)
                {
                    Console.WriteLine("Error opening output audio file: " + csAux);
                    m_bInProcess = false;
                    return false;
                }

                if (m_audfmt[i] == AudioType.WAV)
                {
                    faud[i].Write(pcmheader);
                }

                return true;
            } else
                return true;
        }

        void demuxsubs(string m_csOutputPath, ReadOnlySpan<byte> buffer) {
            int start, nbytes;
            byte streamID;
            int k;
            byte[] mybuff = new byte[10];
            int iPTS;

            start = 0x17 + buffer[0x16];
            nbytes = buffer[0x12] * 256 + buffer[0x13] + 0x14;
            streamID = buffer[start];

            if (check_sub_open(m_csOutputPath, streamID) == false)
                return;
            if ((buffer[0x16] == 0) || (m_buffer[0x15] & 0x80) != 0x80)
                Util.writebuffer(buffer.Slice(start + 1), fsub[streamID & 0x1F], nbytes - start - 1);
            else
            {
                // fill 10 characters
                for (k = 0; k < 10; k++)
                    mybuff[k] = 0;

                iPTS = m_iSubPTS - m_iFirstNavPTS0 + m_iOffsetPTS;

                mybuff[0] = 0x53;
                mybuff[1] = 0x50;
                mybuff[2] = (byte)(iPTS % 256);
                mybuff[3] = (byte)((iPTS >> 8) % 256);
                mybuff[4] = (byte)((iPTS >> 16) % 256);
                mybuff[5] = (byte)((iPTS >> 24) % 256);

                Util.writebuffer(mybuff, fsub[streamID & 0x1F], 10);
                Util.writebuffer(buffer.Slice(start + 1), fsub[streamID & 0x1F], nbytes - start - 1);
            }
        }

        bool check_sub_open(string m_csOutputPath, byte i) {
            string csAux;

            i -= 0x20;

            if (i > 31) return false;

            if (fsub[i] == null)
            {
                csAux = $"Subpictures_{(i + 0x20):X2}.sup";
                csAux = Path.Combine(m_csOutputPath, csAux);

                try
                {
                    fsub[i] = File.Open(csAux, FileMode.Create);
                } catch (Exception)
                {
                    fsub[i] = null;
                }
                if (fsub[i] == null)
                {
                    Console.WriteLine("Error opening output subs file:" + csAux);
                    m_bInProcess = false;
                    return false;
                } else return true;
            } else
                return true;
        }

        byte hi_nib(byte a) {
            return (byte)((a >> 4) & 0x0F);
        }

        byte lo_nib(byte a) {
            return (byte)(a & 0x0F);
        }

        bool MDemux(string m_csOutputPath, int nPGC, object? pDlg) {
            int nTotalSectors;
            int nSector, nCell;
            int k, iArraysize;
            int CID, VID;
            Int64 i64IniSec, i64EndSec;
            string csAux, csAux2;
            Stream in_, fout;
            Int64 i64;
            bool bMyCell;
            bool iRet;
            ulong dwCellDuration;
            int nFrames;


            if (nPGC >= NumberOfPGCsInMenus)
            {
                Console.WriteLine("Error: PGC does not exist");
                m_bInProcess = false;
                return false;
            }

            IniDemuxGlobalVars();
            if (OpenVideoFile(m_csOutputPath) == false) return false;
            m_bInProcess = true;

            // Calculate  the total number of sectors
            nTotalSectors = 0;
            iArraysize = CellsInMenus.Count;
            for (nCell = 0; nCell < NumberOfMenuCells[nPGC]; nCell++)
            {
                VID = Util.GetNbytes(2, m_pIFO.AsSpan(m_M_C_POST[nPGC] + 4 * nCell));
                CID = m_pIFO[m_M_C_POST[nPGC] + 3 + 4 * nCell];
                for (k = 0; k < iArraysize; k++)
                {
                    if (CID == CellsInMenus[k].CID &&
                        VID == CellsInMenus[k].VID)
                    {
                        nTotalSectors += CellsInMenus[k].iSize;
                    }
                }
            }

            nSector = 0;
            iRet = true;

            for (nCell = 0; nCell < NumberOfMenuCells[nPGC] && m_bInProcess == true; nCell++)
            {
                VID = Util.GetNbytes(2, m_pIFO.AsSpan(m_M_C_POST[nPGC] + 4 * nCell));
                CID = m_pIFO[m_M_C_POST[nPGC] + 3 + 4 * nCell];

                i64IniSec = Util.GetNbytes(4, m_pIFO.AsSpan(m_M_C_PBKT[nPGC] + nCell * 24 + 8));
                i64EndSec = Util.GetNbytes(4, m_pIFO.AsSpan(m_M_C_PBKT[nPGC] + nCell * 24 + 0x14));

                if (m_bVMGM)
                {
                    csAux2 = m_csInputIFO.Substring(0, m_csInputIFO.Length - 3);
                    csAux = csAux2 + "VOB";
                } else
                {
                    csAux2 = m_csInputIFO.Substring(0, m_csInputIFO.Length - 5);
                    csAux = csAux2 + "0.VOB";
                }
                try
                {
                    in_ = File.OpenRead(csAux);
                } catch (Exception)
                {
                    in_ = null;
                }
                if (in_ == null)
                {
                    Console.WriteLine("Error opening input VOB: " + csAux);
                    m_bInProcess = false;
                    iRet = false;
                }
                if (m_bInProcess) in_.Seek((long)((i64IniSec) * 2048), SeekOrigin.Begin);

                for (i64 = 0, bMyCell = true; i64 < (i64EndSec - i64IniSec + 1) && m_bInProcess == true; i64++)
                {
                    //readpack
                    if ((i64 % MODUPDATE) == 0) UpdateProgress(pDlg, (int)((100 * nSector) / nTotalSectors));
                    if (Util.readbuffer(m_buffer, in_) != 2048)
                    {
                        if (in_ != null) in_.Close();
                        Console.WriteLine("Input error: Reached end of VOB too early");
                        m_bInProcess = false;
                        iRet = false;
                    }

                    if (m_bInProcess == true)
                    {
                        if (Util.IsSynch(m_buffer) != true)
                        {
                            Console.WriteLine("Error reading input VOB: Unsynchronized");
                            m_bInProcess = false;
                            iRet = false;
                        }
                        if (Util.IsNav(m_buffer))
                        {
                            if (m_buffer[0x420] == (byte)(VID % 256) &&
                                m_buffer[0x41F] == (byte)(VID / 256) &&
                                m_buffer[0x422] == (byte)CID)
                                bMyCell = true;
                            else
                                bMyCell = false;
                        }

                        if (bMyCell)
                        {
                            nSector++;
                            iRet = ProcessPack(m_csOutputPath, true);
                        }
                    }
                } // For readpacks
                if (in_ != null) in_.Close();
                in_ = null;
            }   // For Cells 

            CloseAndNull();

            nFrames = 0;

            if (m_bCheckCellt && m_bInProcess == true)
            {
                csAux = m_csOutputPath + '\\' + "Celltimes.txt";
                fout = File.Open(csAux, FileMode.Create); ;
                for (nCell = 0; nCell < NumberOfMenuCells[nPGC] && m_bInProcess == true; nCell++)
                {
                    dwCellDuration = (ulong)Util.GetNbytes(4, m_pIFO.AsSpan(m_M_C_PBKT[nPGC] + 24 * nCell + 4));
                    nFrames += Util.DurationInFrames(dwCellDuration);
                    if (nCell != (NumberOfMenuCells[nPGC] - 1) || m_bCheckEndTime)
                    {
                        var writer = new StreamWriter(fout);
                        writer.Write($"{nFrames}\n");
                        writer.Flush();
                    }
                }
                fout.Close();
            }

            TotalNumberOfFrames = nFrames;

            if (m_bCheckLog && m_bInProcess == true) OutputLog(nPGC, 1, DemuxingDomain.Menus, Console.Out);

            return iRet;
        }

        bool VIDDemux(string output_folder, int nVid, object? pDlg) {
            int nTotalSectors;
            int nSector, nCell;
            int k, iArraysize;
            int CID, VID, nDemuxedVID;
            Int64 i64IniSec, i64EndSec;
            Int64 i64sectors;
            int nVobin = 0;
            string csAux, csAux2;
            Stream in_, fout;
            Int64 i64;
            bool bMyCell;
            bool iRet;
            int nFrames;
            int nLastCell;

            if (nVid >= VobIDsInTitles.Count)
            {
                Console.WriteLine("Error: Selected Vid does not exist");
                m_bInProcess = false;
                return false;
            }

            IniDemuxGlobalVars();
            if (OpenVideoFile(output_folder) == false) return false;
            m_bInProcess = true;

            // Calculate  the total number of sectors
            nTotalSectors = VobIDsInTitles[nVid].iSize;
            nSector = 0;
            iRet = true;
            nDemuxedVID = VobIDsInTitles[nVid].VobID;

            iArraysize = CellsInTitles.Count;
            for (nCell = 0; nCell < iArraysize && m_bInProcess == true; nCell++)
            {
                VID = CellsInTitles[nCell].VID;
                CID = CellsInTitles[nCell].CID;

                if (VID == nDemuxedVID)
                {
                    i64IniSec = CellsInTitles[nCell].iIniSec;
                    i64EndSec = CellsInTitles[nCell].iEndSec;
                    for (k = 1, i64sectors = 0; k < 10; k++)
                    {
                        i64sectors += (m_i64VOBSize[k] / 2048);
                        if (i64IniSec < i64sectors)
                        {
                            i64sectors -= (m_i64VOBSize[k] / 2048);
                            nVobin = k;
                            k = 20;
                        }
                    }
                    csAux2 = m_csInputIFO.Substring(0, m_csInputIFO.Length - 5);
                    csAux = $"{nVobin}.VOB";
                    csAux = csAux2 + csAux;
                    try
                    {
                        in_ = File.OpenRead(csAux);
                    } catch (Exception)
                    {
                        in_ = null;
                    }
                    if (in_ == null)
                    {
                        Console.WriteLine("Error opening input VOB: " + csAux);
                        m_bInProcess = false;
                        iRet = false;
                    }
                    if (m_bInProcess) in_.Seek((long)((i64IniSec - i64sectors) * 2048), SeekOrigin.Begin);

                    for (i64 = 0, bMyCell = true; i64 < (i64EndSec - i64IniSec + 1) && m_bInProcess == true; i64++)
                    {
                        //readpack
                        if ((i64 % MODUPDATE) == 0) UpdateProgress(pDlg, (int)((100 * nSector) / nTotalSectors));
                        if (Util.readbuffer(m_buffer, in_) != 2048)
                        {
                            if (in_ != null) in_.Close();
                            nVobin++;
                            csAux2 = m_csInputIFO.Substring(0, m_csInputIFO.Length - 5);
                            csAux = $"{nVobin}.VOB";
                            csAux = csAux2 + csAux;
                            in_ = File.OpenRead(csAux);
                            if (Util.readbuffer(m_buffer, in_) != 2048)
                            {
                                Console.WriteLine("Input error: Reached end of VOB too early");
                                m_bInProcess = false;
                                iRet = false;
                            }
                        }

                        if (m_bInProcess == true)
                        {
                            if (Util.IsSynch(m_buffer) != true)
                            {
                                Console.WriteLine("Error reading input VOB: Unsynchronized");
                                m_bInProcess = false;
                                iRet = false;
                            }
                            if (Util.IsNav(m_buffer))
                            {
                                if (m_buffer[0x420] == (byte)(VID % 256) &&
                                    m_buffer[0x41F] == (byte)(VID / 256) &&
                                    m_buffer[0x422] == (byte)CID)
                                    bMyCell = true;
                                else
                                    bMyCell = false;
                            }

                            if (bMyCell)
                            {
                                nSector++;
                                iRet = ProcessPack(output_folder, true);
                            }
                        }
                    } // For readpacks
                    if (in_ != null) in_.Close();
                    in_ = null;
                }  // if (VID== DemuxedVID)
            }   // For Cells 

            CloseAndNull();
            nFrames = 0;

            if (m_bCheckCellt && m_bInProcess == true)
            {
                csAux = Path.Combine(output_folder, "Celltimes.txt");
                fout = File.Open(csAux, FileMode.Create);

                nDemuxedVID = VobIDsInTitles[nVid].VobID;

                iArraysize = CellsInTitles.Count;
                for (nCell = nLastCell = 0; nCell < iArraysize && m_bInProcess == true; nCell++)
                {
                    VID = CellsInTitles[nCell].VID;
                    if (VID == nDemuxedVID)
                        nLastCell = nCell;
                }

                for (nCell = 0; nCell < iArraysize && m_bInProcess == true; nCell++)
                {
                    VID = CellsInTitles[nCell].VID;

                    if (VID == nDemuxedVID)
                    {
                        nFrames += Util.DurationInFrames(CellsInTitles[nCell].dwDuration);
                        if (nCell != nLastCell || m_bCheckEndTime)
                        {
                            var writer = new StreamWriter(fout);
                            writer.Write($"{nFrames}\n");
                            writer.Flush();
                        }
                    }
                }
                fout.Close();
            }

            TotalNumberOfFrames = nFrames;

            if (m_bCheckLog && m_bInProcess == true) OutputLog(nVid, 1, DemuxingDomain.Titles, Console.Out);

            return iRet;
        }

        bool VIDMDemux(string output_folder, int nVid, object? pDlg) {
            int nTotalSectors;
            int nSector, nCell;
            int iArraysize;
            int CID, VID, nDemuxedVID;
            Int64 i64IniSec, i64EndSec;
            string csAux, csAux2;
            Stream in_, fout;
            Int64 i64;
            bool bMyCell;
            bool iRet;
            int nFrames;
            int nLastCell;

            if (nVid >= VobIDsInMenus.Count)
            {
                Console.WriteLine("Error: Selected Vid does not exist");
                m_bInProcess = false;
                return false;
            }

            IniDemuxGlobalVars();
            if (OpenVideoFile(output_folder) == false) return false;
            m_bInProcess = true;

            // Calculate  the total number of sectors
            nTotalSectors = VobIDsInMenus[nVid].iSize;
            nSector = 0;
            iRet = true;
            nDemuxedVID = VobIDsInMenus[nVid].VobID;

            iArraysize = CellsInMenus.Count;
            for (nCell = 0; nCell < iArraysize && m_bInProcess == true; nCell++)
            {
                VID = CellsInMenus[nCell].VID;
                CID = CellsInMenus[nCell].CID;

                if (VID == nDemuxedVID)
                {
                    i64IniSec = CellsInMenus[nCell].iIniSec;
                    i64EndSec = CellsInMenus[nCell].iEndSec;
                    if (m_bVMGM)
                    {
                        csAux2 = m_csInputIFO.Substring(0, m_csInputIFO.Length - 3);
                        csAux = csAux2 + "VOB";
                    } else
                    {
                        csAux2 = m_csInputIFO.Substring(0, m_csInputIFO.Length - 5);
                        csAux = csAux2 + "0.VOB";
                    }
                    try
                    {
                        in_ = File.OpenRead(csAux);
                    } catch (Exception)
                    {
                        in_ = null;
                    }
                    if (in_ == null)
                    {
                        Console.WriteLine("Error opening input VOB: " + csAux);
                        m_bInProcess = false;
                        iRet = false;
                    }
                    if (m_bInProcess) in_.Seek((long)((i64IniSec) * 2048), SeekOrigin.Begin);

                    for (i64 = 0, bMyCell = true; i64 < (i64EndSec - i64IniSec + 1) && m_bInProcess == true; i64++)
                    {
                        //readpack
                        if ((i64 % MODUPDATE) == 0) UpdateProgress(pDlg, (int)((100 * nSector) / nTotalSectors));
                        if (Util.readbuffer(m_buffer, in_) != 2048)
                        {
                            if (in_ != null) in_.Close();
                            Console.WriteLine("Input error: Reached end of VOB too early");
                            m_bInProcess = false;
                            iRet = false;
                        }

                        if (m_bInProcess == true)
                        {
                            if (Util.IsSynch(m_buffer) != true)
                            {
                                Console.WriteLine("Error reading input VOB: Unsynchronized");
                                m_bInProcess = false;
                                iRet = false;
                            }
                            if (Util.IsNav(m_buffer))
                            {
                                if (m_buffer[0x420] == (byte)(VID % 256) &&
                                    m_buffer[0x41F] == (byte)(VID / 256) &&
                                    m_buffer[0x422] == (byte)CID)
                                    bMyCell = true;
                                else
                                    bMyCell = false;
                            }

                            if (bMyCell)
                            {
                                nSector++;
                                iRet = ProcessPack(output_folder, true);
                            }
                        }
                    } // For readpacks
                    if (in_ != null) in_.Close();
                    in_ = null;
                } // If (VID==DemuxedVID)
            }   // For Cells 

            CloseAndNull();

            nFrames = 0;

            if (m_bCheckCellt && m_bInProcess == true)
            {
                csAux = Path.Combine(output_folder, "Celltimes.txt");
                fout = File.Open(csAux, FileMode.Create);

                nDemuxedVID = VobIDsInMenus[nVid].VobID;

                iArraysize = CellsInMenus.Count;

                for (nCell = nLastCell = 0; nCell < iArraysize && m_bInProcess == true; nCell++)
                {
                    VID = CellsInMenus[nCell].VID;
                    if (VID == nDemuxedVID) nLastCell = nCell;
                }


                for (nCell = 0; nCell < iArraysize && m_bInProcess == true; nCell++)
                {
                    VID = CellsInMenus[nCell].VID;

                    if (VID == nDemuxedVID)
                    {
                        nFrames += Util.DurationInFrames(CellsInMenus[nCell].dwDuration);
                        if (nCell != nLastCell || m_bCheckEndTime)
                        {
                            var writer = new StreamWriter(fout);
                            writer.WriteLine($"{nFrames}\n");
                            writer.Flush();
                        }
                    }
                }
                fout.Close();
            }

            TotalNumberOfFrames = nFrames;

            if (m_bCheckLog && m_bInProcess == true) OutputLog(nVid, 1, DemuxingDomain.Menus, Console.Out);

            return iRet;
        }

        bool CIDDemux(string output_folder, int nCell, object? pDlg) {
            int nTotalSectors;
            int nSector;
            int k;
            int CID, VID;
            Int64 i64IniSec, i64EndSec;
            Int64 i64sectors;
            int nVobin = 0;
            string csAux, csAux2;
            Stream in_, fout;
            Int64 i64;
            bool bMyCell;
            bool iRet;
            int nFrames;

            if (nCell >= CellsInTitles.Count)
            {
                Console.WriteLine("Error: Selected Cell does not exist");
                m_bInProcess = false;
                return false;
            }

            IniDemuxGlobalVars();
            if (OpenVideoFile(output_folder) == false) return false;
            m_bInProcess = true;

            // Calculate  the total number of sectors
            nTotalSectors = CellsInTitles[nCell].iSize;
            nSector = 0;
            iRet = true;

            VID = CellsInTitles[nCell].VID;
            CID = CellsInTitles[nCell].CID;

            i64IniSec = CellsInTitles[nCell].iIniSec;
            i64EndSec = CellsInTitles[nCell].iEndSec;
            for (k = 1, i64sectors = 0; k < 10; k++)
            {
                i64sectors += (m_i64VOBSize[k] / 2048);
                if (i64IniSec < i64sectors)
                {
                    i64sectors -= (m_i64VOBSize[k] / 2048);
                    nVobin = k;
                    k = 20;
                }
            }
            csAux2 = m_csInputIFO.Substring(0, m_csInputIFO.Length - 5);
            csAux = $"{nVobin}.VOB";
            csAux = csAux2 + csAux;
            try
            {
                in_ = File.OpenRead(csAux);
            } catch (Exception)
            {
                in_ = null;
            }
            if (in_ == null)
            {
                Console.WriteLine("Error opening input VOB: " + csAux);
                m_bInProcess = false;
                iRet = false;
            }
            if (m_bInProcess) in_.Seek((long)((i64IniSec - i64sectors) * 2048), SeekOrigin.Begin);

            for (i64 = 0, bMyCell = true; i64 < (i64EndSec - i64IniSec + 1) && m_bInProcess == true; i64++)
            {
                //readpack
                if ((i64 % MODUPDATE) == 0) UpdateProgress(pDlg, (int)((100 * nSector) / nTotalSectors));
                if (Util.readbuffer(m_buffer, in_) != 2048)
                {
                    if (in_ != null) in_.Close();
                    nVobin++;
                    csAux2 = m_csInputIFO.Substring(0, m_csInputIFO.Length - 5);
                    csAux = $"{nVobin}.VOB";
                    csAux = csAux2 + csAux;
                    in_ = File.OpenRead(csAux);
                    if (Util.readbuffer(m_buffer, in_) != 2048)
                    {
                        Console.WriteLine("Input error: Reached end of VOB too early");
                        m_bInProcess = false;
                        iRet = false;
                    }
                }

                if (m_bInProcess == true)
                {
                    if (Util.IsSynch(m_buffer) != true)
                    {
                        Console.WriteLine("Error reading input VOB: Unsynchronized");
                        m_bInProcess = false;
                        iRet = false;
                    }
                    if (Util.IsNav(m_buffer))
                    {
                        if (m_buffer[0x420] == (byte)(VID % 256) &&
                            m_buffer[0x41F] == (byte)(VID / 256) &&
                            m_buffer[0x422] == (byte)CID)
                            bMyCell = true;
                        else
                            bMyCell = false;
                    }

                    if (bMyCell)
                    {
                        nSector++;
                        iRet = ProcessPack(output_folder, true);
                    }
                }
            } // For readpacks
            if (in_ != null) in_.Close();
            in_ = null;

            CloseAndNull();

            nFrames = 0;

            if (m_bCheckCellt && m_bInProcess == true)
            {
                csAux = Path.Combine(output_folder, "Celltimes.txt");
                fout = File.Open(csAux, FileMode.Create);
                nFrames = Util.DurationInFrames(CellsInTitles[nCell].dwDuration);
                if (m_bCheckEndTime)
                {
                    var writer = new StreamWriter(fout);
                    writer.WriteLine($"{nFrames}\n");
                    writer.Flush();
                }
                fout.Close();
            }

            TotalNumberOfFrames = nFrames;

            if (m_bCheckLog && m_bInProcess == true) OutputLog(nCell, 1, DemuxingDomain.Titles, Console.Out);

            return iRet;
        }

        bool CIDMDemux(string output_folder, int nCell, object? pDlg) {
            int nTotalSectors;
            int nSector;
            int CID, VID;
            Int64 i64IniSec, i64EndSec;
            string csAux, csAux2;
            Stream in_, fout;
            Int64 i64;
            bool bMyCell;
            bool iRet;
            int nFrames;

            if (nCell >= CellsInMenus.Count)
            {
                Console.WriteLine("Error: Selected Cell does not exist");
                m_bInProcess = false;
                return false;
            }

            IniDemuxGlobalVars();
            if (OpenVideoFile(output_folder) == false) return false;
            m_bInProcess = true;

            // Calculate  the total number of sectors
            nTotalSectors = CellsInMenus[nCell].iSize;
            nSector = 0;
            iRet = true;

            VID = CellsInMenus[nCell].VID;
            CID = CellsInMenus[nCell].CID;

            i64IniSec = CellsInMenus[nCell].iIniSec;
            i64EndSec = CellsInMenus[nCell].iEndSec;
            if (m_bVMGM)
            {
                csAux2 = m_csInputIFO.Substring(0, m_csInputIFO.Length - 3);
                csAux = csAux2 + "VOB";
            } else
            {
                csAux2 = m_csInputIFO.Substring(0, m_csInputIFO.Length - 5);
                csAux = csAux2 + "0.VOB";
            }
            in_ = File.OpenRead(csAux);
            if (in_ == null)
            {
                Console.WriteLine("Error opening input VOB: " + csAux);
                m_bInProcess = false;
                iRet = false;
            }
            if (m_bInProcess) in_.Seek((long)((i64IniSec) * 2048), SeekOrigin.Begin);

            for (i64 = 0, bMyCell = true; i64 < (i64EndSec - i64IniSec + 1) && m_bInProcess == true; i64++)
            {
                //readpack
                if ((i64 % MODUPDATE) == 0) UpdateProgress(pDlg, (int)((100 * nSector) / nTotalSectors));
                if (Util.readbuffer(m_buffer, in_) != 2048)
                {
                    if (in_ != null) in_.Close();
                    Console.WriteLine("Input error: Reached end of VOB too early");
                    m_bInProcess = false;
                    iRet = false;
                }
                if (m_bInProcess == true)
                {
                    if (Util.IsSynch(m_buffer) != true)
                    {
                        Console.WriteLine("Error reading input VOB: Unsynchronized");
                        m_bInProcess = false;
                        iRet = false;
                    }
                    if (Util.IsNav(m_buffer))
                    {
                        if (m_buffer[0x420] == (byte)(VID % 256) &&
                            m_buffer[0x41F] == (byte)(VID / 256) &&
                            m_buffer[0x422] == (byte)CID)
                            bMyCell = true;
                        else
                            bMyCell = false;
                    }

                    if (bMyCell)
                    {
                        nSector++;
                        iRet = ProcessPack(output_folder, true);
                    }
                }
            } // For readpacks
            if (in_ != null) in_.Close();
            in_ = null;

            CloseAndNull();

            nFrames = 0;

            if (m_bCheckCellt && m_bInProcess == true)
            {
                csAux = Path.Combine(output_folder, "Celltimes.txt");
                fout = File.Open(csAux, FileMode.Create);
                nFrames = Util.DurationInFrames(CellsInMenus[nCell].dwDuration);
                if (m_bCheckEndTime)
                {
                    var writer = new StreamWriter(fout);
                    writer.WriteLine($"{nFrames}\n");
                    writer.Flush();
                }
                fout.Close();
            }

            TotalNumberOfFrames = nFrames;

            if (m_bCheckLog && m_bInProcess == true) OutputLog(nCell, 1, DemuxingDomain.Menus, Console.Out);

            return iRet;
        }

        public bool GetAudioDelay(string output_folder, DemuxingMode iMode, int nSelection) {
            int VID = 0, CID = 0;
            int k, nCell;
            Int64 i64IniSec, i64EndSec;
            Int64 i64sectors;
            int nVobin = 0;
            string csAux, csAux2;
            Stream in_;
            Int64 i64;
            bool bMyCell;
            bool iRet;

            IniDemuxGlobalVars();

            if (iMode == DemuxingMode.PGC)
            {
                if (nSelection >= NumberOfPGCsInTitles)
                {
                    Console.WriteLine("Error: PGC does not exist");
                    return false;
                }
                nCell = 0;
                VID = Util.GetNbytes(2, m_pIFO.AsSpan(m_C_POST[nSelection] + 4 * nCell));
                CID = m_pIFO[m_C_POST[nSelection] + 3 + 4 * nCell];
            } else if (iMode == DemuxingMode.VID)
            {
                if (nSelection >= VobIDsInTitles.Count)
                {
                    Console.WriteLine("Error: VID does not exist");
                    return false;
                }
                VID = VobIDsInTitles[nSelection].VobID;
                CID = -1;
                for (k = 0; k < CellsInTitles.Count && CID == -1; k++)
                {
                    if (VID == CellsInTitles[k].VID)
                        CID = CellsInTitles[k].CID;
                }

            } else if (iMode == DemuxingMode.CID)
            {
                if (nSelection >= CellsInTitles.Count)
                {
                    Console.WriteLine("Error: CID does not exist");
                    return false;
                }
                VID = CellsInTitles[nSelection].VID;
                CID = CellsInTitles[nSelection].CID;
            }

            for (k = 0, nCell = -1; k < CellsInTitles.Count && nCell == -1; k++)
            {
                if (VID == CellsInTitles[k].VID &&
                    CID == CellsInTitles[k].CID)
                    nCell = k;
            }

            if (nCell < 0)
            {
                Console.WriteLine("Error: VID/CID not found!.");
                return false;
            }
            //
            // Now we have VID; CID; and the index in Cell Array "nCell".
            // So we are going to open the VOB and read the delays using ProcessPack(false)
            i64IniSec = CellsInTitles[nCell].iIniSec;
            i64EndSec = CellsInTitles[nCell].iEndSec;

            iRet = true;
            for (k = 1, i64sectors = 0; k < 10; k++)
            {
                i64sectors += (m_i64VOBSize[k] / 2048);
                if (i64IniSec < i64sectors)
                {
                    i64sectors -= (m_i64VOBSize[k] / 2048);
                    nVobin = k;
                    k = 20;
                }
            }
            csAux2 = m_csInputIFO.Substring(0, m_csInputIFO.Length - 5);
            csAux = $"{nVobin}.VOB";
            csAux = csAux2 + csAux;
            in_ = File.OpenRead(csAux);
            if (in_ == null)
            {
                Console.WriteLine("Error opening input VOB: " + csAux);
                iRet = false;
            }
            if (iRet == true) in_.Seek((long)((i64IniSec - i64sectors) * 2048), SeekOrigin.Begin);

            for (i64 = 0, bMyCell = true; iRet == true && i64 < (i64EndSec - i64IniSec + 1) && i64 < MAXLOOKFORAUDIO; i64++)
            {
                //readpack
                if (Util.readbuffer(m_buffer, in_) != 2048)
                {
                    if (in_ != null) in_.Close();
                    nVobin++;
                    csAux2 = m_csInputIFO.Substring(0, m_csInputIFO.Length - 5);
                    csAux = $"{nVobin}.VOB";
                    csAux = csAux2 + csAux;
                    in_ = File.OpenRead(csAux);
                    if (Util.readbuffer(m_buffer, in_) != 2048)
                    {
                        Console.WriteLine("Input error: Reached end of VOB too early");
                        iRet = false;
                    }
                }

                if (iRet == true)
                {
                    if (Util.IsSynch(m_buffer) != true)
                    {
                        Console.WriteLine("Error reading input VOB: Unsynchronized");
                        iRet = false;
                    }
                    if ((iRet == true) && Util.IsNav(m_buffer))
                    {
                        if (m_buffer[0x420] == (byte)(VID % 256) &&
                            m_buffer[0x41F] == (byte)(VID / 256) &&
                            m_buffer[0x422] == (byte)CID)
                            bMyCell = true;
                        else
                            bMyCell = false;
                    }

                    if (iRet == true && bMyCell)
                    {
                        iRet = ProcessPack(output_folder, false);
                    }
                }
            } // For readpacks
            if (in_ != null) in_.Close();
            in_ = null;

            return iRet;
        }

        bool GetMAudioDelay(string output_folder, DemuxingMode iMode, int nSelection) {
            int VID = 0, CID = 0;
            int k, nCell;
            Int64 i64IniSec, i64EndSec;
            string csAux, csAux2;
            Stream in_;
            Int64 i64;
            bool bMyCell;
            bool iRet;

            IniDemuxGlobalVars();

            if (iMode == DemuxingMode.PGC)
            {
                if (nSelection >= NumberOfPGCsInMenus)
                {
                    Console.WriteLine("Error: PGC does not exist");
                    return false;
                }
                nCell = 0;
                VID = Util.GetNbytes(2, m_pIFO.AsSpan(m_M_C_POST[nSelection] + 4 * nCell));
                CID = m_pIFO[m_M_C_POST[nSelection] + 3 + 4 * nCell];
            } else if (iMode == DemuxingMode.VID)
            {
                if (nSelection >= VobIDsInMenus.Count)
                {
                    Console.WriteLine("Error: VID does not exist");
                    return false;
                }
                VID = VobIDsInMenus[nSelection].VobID;
                CID = -1;
                for (k = 0; k < CellsInMenus.Count && CID == -1; k++)
                {
                    if (VID == CellsInMenus[k].VID)
                        CID = CellsInMenus[k].CID;
                }

            } else if (iMode == DemuxingMode.CID)
            {
                if (nSelection >= CellsInMenus.Count)
                {
                    Console.WriteLine("Error: CID does not exist");
                    return false;
                }
                VID = CellsInMenus[nSelection].VID;
                CID = CellsInMenus[nSelection].CID;
            }

            for (k = 0, nCell = -1; k < CellsInMenus.Count && nCell == -1; k++)
            {
                if (VID == CellsInMenus[k].VID &&
                    CID == CellsInMenus[k].CID)
                    nCell = k;
            }

            if (nCell < 0)
            {
                Console.WriteLine("Error: VID/CID not found!.");
                return false;
            }
            //
            // Now we have VID; CID; and the index in Cell Array "nCell".
            // So we are going to open the VOB and read the delays using ProcessPack(false)
            i64IniSec = CellsInMenus[nCell].iIniSec;
            i64EndSec = CellsInMenus[nCell].iEndSec;

            iRet = true;

            if (m_bVMGM)
            {
                csAux2 = m_csInputIFO.Substring(0, m_csInputIFO.Length - 3);
                csAux = csAux2 + "VOB";
            } else
            {
                csAux2 = m_csInputIFO.Substring(0, m_csInputIFO.Length - 5);
                csAux = csAux2 + "0.VOB";
            }
            try
            {
                in_ = File.OpenRead(csAux);
            } catch (Exception)
            {
                in_ = null;
            }
            if (in_ == null)
            {
                Console.WriteLine("Error opening input VOB: " + csAux);
                iRet = false;
            }
            if (iRet == true) in_.Seek((long)((i64IniSec) * 2048), SeekOrigin.Begin);

            for (i64 = 0, bMyCell = true; iRet == true && i64 < (i64EndSec - i64IniSec + 1) && i64 < MAXLOOKFORAUDIO; i64++)
            {
                //readpack
                if (Util.readbuffer(m_buffer, in_) != 2048)
                {
                    Console.WriteLine("Input error: Reached end of VOB too early");
                    iRet = false;
                }

                if (iRet == true)
                {
                    if (Util.IsSynch(m_buffer) != true)
                    {
                        Console.WriteLine("Error reading input VOB: Unsynchronized");
                        iRet = false;
                    }
                    if ((iRet == true) && Util.IsNav(m_buffer))
                    {
                        if (m_buffer[0x420] == (byte)(VID % 256) &&
                            m_buffer[0x41F] == (byte)(VID / 256) &&
                            m_buffer[0x422] == (byte)CID)
                            bMyCell = true;
                        else
                            bMyCell = false;
                    }

                    if (iRet == true && bMyCell)
                    {
                        iRet = ProcessPack(output_folder, false);
                    }
                }
            } // For readpacks
            if (in_ != null) in_.Close();
            in_ = null;

            return iRet;
        }

        void OutputLog(string outputPath, int nItem, int nAng, DemuxingDomain iDomain) {
            string csFilePath = Path.Combine(outputPath, "LogFile.txt");
            Stream file;
            try
            {
                file = File.Open(csFilePath, FileMode.Create);
            } catch (Exception)
            {
                Console.WriteLine("Failed to open log file!");
                return;
            }
            var writer = new StreamWriter(file);
            OutputLog(nItem, nAng, iDomain, writer);
            writer.Flush();
            file.Flush();
            file.Close();
        }

        void OutputLog(int nItem, int nAng, DemuxingDomain iDomain, TextWriter stream) {
            string csFilePath, csAux, csAux1, csAux2;
            int k;
            int AudDelay;

            stream.WriteLine("[General]");
            stream.WriteLine($"Total Number of PGCs   in Titles={NumberOfPGCsInTitles}");
            stream.WriteLine($"Total Number of PGCs   in  Menus={NumberOfPGCsInMenus}");
            stream.WriteLine($"Total Number of VobIDs in Titles={VobIDsInTitles.Count}");
            stream.WriteLine($"Total Number of VobIDs in  Menus={VobIDsInMenus.Count}");
            stream.WriteLine($"Total Number of Cells  in Titles={CellsInTitles.Count}");
            stream.WriteLine($"Total Number of Cells  in  Menus={CellsInMenus.Count}");

            switch (m_iMode)
            {
                case DemuxingMode.PGC: csAux = "by PGC"; break;
                case DemuxingMode.VID: csAux = "by VOB Id"; break;
                case DemuxingMode.CID: csAux = "Single Cell"; break;
                default: csAux = "[unknown]"; break;
            }
            stream.WriteLine($"Demuxing   Mode={csAux}");

            switch (iDomain)
            {
                case DemuxingDomain.Titles: csAux = "Titles"; break;
                case DemuxingDomain.Menus: csAux = "Menus"; break;
                default: csAux = "[unknown]"; break;
            }
            stream.WriteLine($"Demuxing Domain={csAux}");
            stream.WriteLine($"Total Number of Frames={TotalNumberOfFrames}");

            if (m_iMode == DemuxingMode.PGC)
            {
                stream.WriteLine($"Selected PGC={(nItem + 1)}");
                stream.WriteLine($"Number of Cells in Selected PGC={(iDomain == DemuxingDomain.Titles ? NumberOfCells : NumberOfMenuCells)[nItem]}");
                stream.WriteLine($"Selected VOBID=None");
                stream.WriteLine($"Number of Cells in Selected VOB=None");
            } else if (m_iMode == DemuxingMode.VID)
            {
                List<ADT_VID_LIST> vid_list = (iDomain == DemuxingDomain.Titles ? VobIDsInTitles : VobIDsInMenus);
                stream.WriteLine($"Selected VOBID={vid_list[nItem].VobID}");
                stream.WriteLine($"Number of Cells in Selected VOB={vid_list[nItem].NumberOfCells}");
                stream.WriteLine($"Selected PGC=None");
                stream.WriteLine($"Number of Cells in Selected PGC=None");
            } else if (m_iMode == DemuxingMode.CID)
            {
                stream.WriteLine($"Selected VOBID=None");
                stream.WriteLine($"Number of Cells in Selected VOB=None");
                stream.WriteLine($"Selected PGC=None");
                stream.WriteLine($"Number of Cells in Selected PGC=None");
            } else
            {
                stream.WriteLine("Unknown program mode: " + m_iMode.ToString());
            }

            stream.WriteLine();
            stream.WriteLine("[Demux]");
            stream.WriteLine($"Number of Video Packs={NumberOfVideoPacks}");
            stream.WriteLine($"Number of Audio Packs={NumberOfAudioPacks}");
            stream.WriteLine($"Number of Subs  Packs={NumberOfSubPacks}");
            stream.WriteLine($"Number of Nav   Packs={NumberOfNavPacks}");
            stream.WriteLine($"Number of Pad   Packs={NumberOfPadPacks}");
            stream.WriteLine($"Number of Unkn  Packs={NumberOfUnknownPacks}");

            stream.WriteLine();
            stream.WriteLine("[Audio Streams]");
            for (k = 0; k < 8; k++)
            {
                stream.WriteLine($"Audio_{(k + 1)}={(m_iFirstAudPTS[k] != 0 ? $"0x{m_iAudIndex[k]:X2}" : "None")}");
            }

            stream.WriteLine();
            stream.WriteLine("[Audio Delays]");
            for (k = 0; k < 8; k++)
            {
                if (m_iFirstAudPTS[k] != 0)
                {
                    //			AudDelay=m_iFirstAudPTS[k]-m_iFirstVidPTS;
                    AudDelay = m_iFirstAudPTS[k] - m_iFirstNavPTS0;

                    if (AudDelay < 0)
                        AudDelay -= 44;
                    else
                        AudDelay += 44;
                    AudDelay /= 90;
                    stream.WriteLine($"Audio_{(k + 1)}={AudDelay}");
                }
            }

            stream.WriteLine();
            stream.WriteLine("[Subs Streams]");
            for (k = 0; k < 32; k++)
            {
                stream.WriteLine($"Subs_{(k + 1):00}={(m_iFirstSubPTS[k] != 0 ? $"0x{(k + 0x20):X2}" : "None")}");
            }
        }
    }
}
