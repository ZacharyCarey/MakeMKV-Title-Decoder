using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PgcDemuxLib
{
    /// <summary>
    /// https://dvd.sourceforge.net/dvdinfo/ifo_vts.html#pgci
    /// </summary>
    public class VTS_PGCI
    {
        /// <summary>
        /// Byte index in file
        /// </summary>
        internal readonly int Address;

        // TODO remove
        internal readonly int NumberOfProgramChains;

        PGC[] PGC_List;
        public readonly ReadOnlyIndexedProperty<int, PGC> PGCs;

        internal VTS_PGCI(IfoParseContext context, byte[] file, int addr)
        {
            //TODO REMOVE
            var m_iVTS_PGCI = addr;
            ref int m_nPGCs = ref this.NumberOfProgramChains;
            //END TODO

            this.PGCs = new((index) => PGC_List[index]);

            this.Address = addr;

            m_nPGCs = file.GetNbytes(m_iVTS_PGCI, 2);
            this.PGC_List = new PGC[m_nPGCs];
            
            // TODO Create struct to store the PGC category
            for (int i = 0; i < m_nPGCs; i++)
            {
                int offset = i * 8 + 8; 

                int PgcCategory = file.GetNbytes(offset, 4); // TODO parse PgcCategory

                int PgcAddr = file.GetNbytes(this.Address + offset + 4, 4) + this.Address;
                this.PGC_List[i] = PGC.Parse(file, PgcAddr, context);
            }
        }
    }
}
