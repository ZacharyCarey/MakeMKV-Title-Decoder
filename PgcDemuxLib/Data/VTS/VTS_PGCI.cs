using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PgcDemuxLib.Data.VTS
{
    /// <summary>
    /// https://dvd.sourceforge.net/dvdinfo/ifo_vts.html#pgci
    /// </summary>
    public class VTS_PGCI
    {
        /// <summary>
        /// Byte index in file
        /// </summary>
        [JsonInclude]
        internal readonly int Address;

        public int NumberOfProgramChains => PGCs.Length;

        [JsonInclude]
        PGC[] PGCs;

        public PGC this[int index]
        {
            get => PGCs[index];
        }

        internal VTS_PGCI(byte[] file, int addr)
        {
            Address = addr;

            int numPGCs = file.GetNbytes(addr, 2);
            int lastByte = file.GetNbytes(addr + 4, 4);
            ReadOnlySpan<byte> data = file.AsSpan(addr, lastByte + 1);

            PGCs = new PGC[numPGCs];

            for (int i = 0; i < numPGCs; i++)
            {
                ReadOnlySpan<byte> entry = data.Slice(8 + i * 8, 8);
                int PgcCategory = entry.GetNbytes(0, 4);
                int PgcOffset = entry.GetNbytes(4, 4);
                PGCs[i] = new PGC(data.Slice(PgcOffset), addr + PgcOffset, i, PgcCategory >> 31 & 0b1, 0, PgcCategory >> 24 & 0b1111111);
            }
        }
    }
}
