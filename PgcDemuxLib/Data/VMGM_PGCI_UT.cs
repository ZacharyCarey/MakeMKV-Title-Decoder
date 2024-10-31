using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PgcDemuxLib.Data
{
    /// <summary>
    /// https://dvd.sourceforge.net/dvdinfo/ifo_vmg.html#pgciut
    /// </summary>
    public class VMGM_PGCI_UT
    {
        /// <summary>
        /// Byte index in file
        /// </summary>
        internal readonly int Address;

        public int NumberOfLanguageUnits => LanguageUnits.Length;

        [JsonInclude]
        private VMGM_LU[] LanguageUnits;

        public VMGM_LU this[int index]
        {
            get => LanguageUnits[index];
        }

        internal VMGM_PGCI_UT(byte[] file, int addr, Ifo ifo)
        {
            Address = addr;

            int numLanguageUnits = file.GetNbytes(addr, 2);
            LanguageUnits = new VMGM_LU[numLanguageUnits];

            for (int nLU = 0; nLU < numLanguageUnits; nLU++)
            {
                int entryOffset = nLU * 8 + 8;

                int offset = file.GetNbytes(addr + entryOffset + 4, 4);
                int pgcID = 0;
                LanguageUnits[nLU] = new VMGM_LU(ifo, file, addr + offset, nLU, ref pgcID);
            }
        }
    }
}
