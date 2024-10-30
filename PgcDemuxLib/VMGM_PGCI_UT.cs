using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PgcDemuxLib
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

        public readonly int NumberOfLanguageUnits; // TODO remove

        private VMGM_LU[] LUs;
        public ReadOnlyIndexedProperty<int, VMGM_LU> LanguageUnits;

        internal VMGM_PGCI_UT(byte[] file, int addr, IfoParseContext context)
        {
            LanguageUnits = new((index) => this.LUs[index]);

            this.Address = addr;

            this.NumberOfLanguageUnits = file.GetNbytes(addr, 2);
            this.LUs = new VMGM_LU[NumberOfLanguageUnits];

            for (int nLU = 0; nLU < this.NumberOfLanguageUnits; nLU++)
            {
                int entryOffset = nLU * 8 + 8;

                int offset = file.GetNbytes(addr + entryOffset + 4, 4);
                this.LUs[nLU] = VMGM_LU.Parse(file, addr + offset, context, nLU);
            }
        }
    }
}
