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
    public class VTSM_PGCI_UT
    {
        /// <summary>
        /// Byte index in file
        /// </summary>
        [JsonInclude]
        internal readonly int Address;

        public int NumberOfLanguageUnits => LanguageUnits.Length;

        [JsonInclude]
        private VTSM_LU[] LanguageUnits;

        [JsonIgnore]
        public IEnumerable<VTSM_LU> All => LanguageUnits;

        public VTSM_LU this[int index]
        {
            get => LanguageUnits[index];
        }

        internal VTSM_PGCI_UT(byte[] file, int addr)
        {
            Address = addr;

            int numLanguageUnits = file.GetNbytes(addr, 2);
            int lastByte = file.GetNbytes(addr + 4, 4);
            ReadOnlySpan<byte> data = file.AsSpan(addr, lastByte + 1);

            LanguageUnits = new VTSM_LU[numLanguageUnits];
            for (int nLU = 0; nLU < numLanguageUnits; nLU++)
            {
                ReadOnlySpan<byte> entry = data.Slice(nLU * 8 + 8);

                string languageCode = entry.GetString(0, 2);
                int menuFlags = entry[3];

                int offsetPtr = entry.GetNbytes(4, 4);
                int pgcID = 0;
                LanguageUnits[nLU] = new VTSM_LU(data.Slice(offsetPtr), addr + offsetPtr, ref pgcID, languageCode, menuFlags);
            }
        }
    }

}
