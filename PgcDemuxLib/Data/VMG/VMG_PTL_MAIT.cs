using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PgcDemuxLib.Data.VMG
{
    /// <summary>
    /// Supposed to be searched by the country code to get the appropriate list of masks
    /// </summary>
    public class VMG_PTL_MAIT
    {
        [JsonInclude]
        internal readonly int Address;

        [JsonInclude]
        Dictionary<int, PTL_MAIT> ParentalMasks = new();

        public PTL_MAIT this[int CountryCode]
        {
            get => ParentalMasks[CountryCode];
        }

        [JsonIgnore]
        public IEnumerable<int> AvailableCountryCodes => this.ParentalMasks.Keys;

        public int NumberOfCountries => this.ParentalMasks.Keys.Count;

        /// <summary>
        /// Also known as "NTS"
        /// </summary>
        [JsonInclude]
        public readonly int NumberOfTitleSets;

        internal VMG_PTL_MAIT(byte[] file, int addr)
        {
            this.Address = addr;

            int numCountries = file.GetNbytes(addr, 2);
            this.NumberOfTitleSets = file.GetNbytes(addr + 2, 2);
            int lastByte = file.GetNbytes(addr + 4, 4);

            int infoLen = numCountries * 8;
            ReadOnlySpan<byte> data = file.AsSpan(addr + 8, lastByte - 7);

            for(int i = 0; i < numCountries; i++)
            {
                ReadOnlySpan<byte> entry = data.Slice(i * 8, 8);
                int countryCode = entry.GetNbytes(0, 2);
                int maskOffset = entry.GetNbytes(4, 2);
                // subtract from the offset to account for the +8 offset we added earlier
                this.ParentalMasks[countryCode] = new PTL_MAIT(data.Slice(maskOffset - 8, (NumberOfTitleSets + 1) * 16), countryCode, NumberOfTitleSets, addr + maskOffset);
            }
        }

    }

    /// <summary>
    /// Supposed to be searched with the TitleSet number to get the appropriate mask.
    /// NOTE: VMG parental mask is at index=0
    /// </summary>
    public class PTL_MAIT
    {
        [JsonInclude]
        internal readonly int Address;

        [JsonIgnore]
        public readonly int CountryCode;

        [JsonIgnore]
        ushort[][] Levels = new ushort[8][];

        [JsonInclude]
        public ushort[] Level1 => Levels[0];

        [JsonInclude]
        public ushort[] Level2 => Levels[1];

        [JsonInclude]
        public ushort[] Level3 => Levels[2];

        [JsonInclude]
        public ushort[] Level4 => Levels[3];

        [JsonInclude]
        public ushort[] Level5 => Levels[4];

        [JsonInclude]
        public ushort[] Level6 => Levels[5];

        [JsonInclude]
        public ushort[] Level7 => Levels[6];

        [JsonInclude]
        public ushort[] Level8 => Levels[7];

        internal PTL_MAIT(ReadOnlySpan<byte> data, int countryCode, int nts, int globalAddr)
        {
            this.Address = globalAddr;
            
            int entryLen = 2 * (nts + 1);

            for (int level = 0; level < 8; level++) {
                this.Levels[level] = ParseMasks(data.Slice((7 - level) * entryLen, entryLen), nts);
            }
        }

        private static ushort[] ParseMasks(ReadOnlySpan<byte> data, int nts)
        {
            ushort[] masks = new ushort[nts + 1];
            for (int i = 0; i < masks.Length; i++)
            {
                masks[i] = (ushort)data.GetNbytes(i * 2, 2);
            }
            return masks;
        }
    }
}
