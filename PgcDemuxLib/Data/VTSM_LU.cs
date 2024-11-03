using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PgcDemuxLib.Data
{
    public class VTSM_LU
    {
        /// <summary>
        /// Byte index in file
        /// </summary>
        [JsonInclude]
        internal readonly int Address;

        public int NumberOfProgramChains => PGCs.Length; // TODO remove

        [JsonInclude]
        PGC[] PGCs;

        [JsonIgnore]
        public IEnumerable<PGC> All => PGCs;

        [JsonInclude, JsonConverter(typeof(JsonStringEnumConverter))]
        public readonly MenuExistenceFlag MenuFlags;

        /// <summary>
        /// ISO-639
        /// </summary>
        [JsonInclude]
        public readonly string LanguageCode;

        public PGC this[int index]
        {
            get => PGCs[index];
        }

        // TODO get rid of nLU at some point
        internal VTSM_LU(ReadOnlySpan<byte> data, int globalAddr,int nLU, ref int pgcID, string languageCode, int menuFlags)
        {
            Address = globalAddr;

            this.LanguageCode = languageCode;
            this.MenuFlags = (MenuExistenceFlag)menuFlags;

            int numProgramChains = data.GetNbytes(0, 2);
            int lastByte = data.GetNbytes(4, 4);
            data = data.Slice(0, lastByte + 1);

            PGCs = new PGC[numProgramChains];
            for (int j = 0; j < numProgramChains; j++)
            {
                ReadOnlySpan<byte> entry = data.Slice(8 + j * 8);

                int pgcCategory = entry.GetNbytes(0, 4);
                int offsetPtr = entry.GetNbytes(4, 4);

                PGC pgc = new PGC(data.Slice(offsetPtr), globalAddr + offsetPtr, pgcID, (pgcCategory >> 31) & 1, (pgcCategory >> 24) & 0b1111, -1);
                pgcID++;
                PGCs[j] = pgc;
            }
        }
    }

    [Flags]
    public enum MenuExistenceFlag
    {
        RootOrTitle = 0x80,
        Subpicture = 0x40,
        Audio = 0x20,
        Angle = 0x10,
        Chapters = 0x08,
        None = 0x0
    }
}
