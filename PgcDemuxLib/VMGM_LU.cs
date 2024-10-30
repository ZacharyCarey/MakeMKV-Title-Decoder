using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PgcDemuxLib
{
    public class VMGM_LU
    {
        /// <summary>
        /// Byte index in file
        /// </summary>
        internal readonly int Address;

        public readonly int NumberOfProgramChains; // TODO remove
        internal readonly int nIniPGCinLU; // TODO can this be removed?? Just reference the table in this struct??

        PGC[] List_PGCs;
        public ReadOnlyIndexedProperty<int, PGC> PGCs;

        // TODO get rid of nLU at some point
        internal VMGM_LU(IfoParseContext context, byte[] file, int addr, int nLU)
        {
            var ifo = context.ifo;
            PGCs = new((index) => List_PGCs[index]);

            this.Address = addr;

            this.NumberOfProgramChains = file.GetNbytes(addr, 2);
            this.List_PGCs = new PGC[this.NumberOfProgramChains];
            nIniPGCinLU = ifo.m_nMPGCs;

            for (int j = 0; j < this.NumberOfProgramChains; j++)
            {
                int entryOffset = 8 + 8 * j;

                int offset = file.GetNbytes(addr + entryOffset + 4, 4);
                PGC pgc = PGC.Parse(file, addr + offset, context);
                List_PGCs[j] = pgc;

                int nAbsPGC = ifo.MenuPGCs.Count; //j + ifo.m_nMPGCs;
                ifo.m_nLU_MPGC[pgc] = nLU;
                ifo.MenuPGCs.Add(pgc);

                if ((ifo.m_M_C_PBKT[nAbsPGC] == 0 || ifo.m_M_C_POST[nAbsPGC] == 0) && ifo.m_nMCells[nAbsPGC] != 0)
                // There is something wrong...
                {
                    throw new Exception($"ERROR: There is something wrong in number of cells in LU {nLU:00}, Menu PGC {j:00}.");
                }
            }
        }

        internal static VMGM_LU Parse(byte[] file, int addr, IfoParseContext context, int nLU)
        {
            VMGM_LU result;
            if (!context.VMGM_LU.TryGetValue(addr, out result))
            {
                result = new VMGM_LU(context, file, addr, nLU);
                context.VMGM_LU[addr] = result;
            }
            return result;
        }
    }
}
