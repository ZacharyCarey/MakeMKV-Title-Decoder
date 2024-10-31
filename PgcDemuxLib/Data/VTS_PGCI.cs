using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PgcDemuxLib.Data
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

        public int NumberOfProgramChains => PGCs.Length;

        [JsonInclude]
        PGC[] PGCs;

        public PGC this[int index]
        {
            get => this.PGCs[index];
        }

        internal VTS_PGCI(byte[] file, int addr)
        {
            Address = addr;

            int numPGCs = file.GetNbytes(Address, 2);
            PGCs = new PGC[numPGCs];

            // TODO Create struct to store the PGC category
            for (int i = 0; i < numPGCs; i++)
            {
                int offset = i * 8 + 8;

                int PgcCategory = file.GetNbytes(offset, 4); // TODO parse PgcCategory

                int PgcAddr = file.GetNbytes(Address + offset + 4, 4) + Address;
                PGCs[i] = new PGC(file, PgcAddr, i);
            }
        }
    }
}
