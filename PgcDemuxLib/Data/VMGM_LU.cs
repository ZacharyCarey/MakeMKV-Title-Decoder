using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PgcDemuxLib.Data
{
    public class VMGM_LU
    {
        /// <summary>
        /// Byte index in file
        /// </summary>
        internal readonly int Address;

        public int NumberOfProgramChains => PGCs.Length; // TODO remove
        internal readonly int nIniPGCinLU; // TODO can this be removed?? Just reference the table in this struct??

        [JsonInclude]
        PGC[] PGCs;
        
        public PGC this[int index]
        {
            get => PGCs[index];
        }

        // TODO get rid of nLU at some point
        internal VMGM_LU(Ifo ifo, byte[] file, int addr, int nLU, ref int pgcID)
        {
            Address = addr;

            int numProgramChains = file.GetNbytes(addr, 2);
            PGCs = new PGC[numProgramChains];
            nIniPGCinLU = ifo.MenuPGCs.Count;

            for (int j = 0; j < numProgramChains; j++)
            {
                int entryOffset = 8 + 8 * j;

                int offset = file.GetNbytes(addr + entryOffset + 4, 4);
                PGC pgc = new PGC(file, addr + offset, pgcID);
                pgcID++;
                PGCs[j] = pgc;

                int nAbsPGC = ifo.MenuPGCs.Count; //j + ifo.MenuPGCs.Count;
                ifo.m_nLU_MPGC[pgc] = nLU;
                ifo.MenuPGCs.Add(pgc);
            }
        }
    }
}
