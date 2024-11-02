using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PgcDemuxLib.Data
{
    /// <summary>
    /// https://dvd.sourceforge.net/dvdinfo/ifo.html#c_adt
    /// </summary>
    public class VTS_C_ADT
    {
        /// <summary>
        /// Byte index in file
        /// </summary>
        [JsonInclude]
        internal readonly int Address;

        public int NumberOfADTs => ADTs.Length;

        [JsonInclude]
        private ADT[] ADTs;
        
        public ADT this[int index]
        {
            get => ADTs[index];
        }

        internal VTS_C_ADT(byte[] file, int addr)
        {
            Address = addr;

            int numADTs = file.GetNbytes(addr, 2);
            int lastByte = file.GetNbytes(addr + 4, 4);
            ReadOnlySpan<byte> data = file.AsSpan(addr, lastByte + 1);

            // TODO verify if this is needed or not
            int numEntries = (data.Length - 8) / 12;
            if (numADTs > numEntries)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"WARNING: VTS_C_ADT has {numADTs} ADTs, but there are enough bytes for {numEntries} entries. Data may be truncated.");
                Console.ResetColor();
                numADTs = numEntries;
            }

            ADTs = new ADT[numADTs];
            //Cells
            for (int nADT = 0; nADT < numADTs; nADT++)
            {
                ADTs[nADT] = new ADT(data.Slice(8 + 12 * nADT, 12), addr + 8 + 12 * nADT);
            }
        }
    }

    /// <summary>
    /// https://dvd.sourceforge.net/dvdinfo/ifo.html#c_adt
    /// </summary>
    public class ADT
    {
        /// <summary>
        /// Byte index in file
        /// </summary>
        [JsonInclude]
        internal readonly int Address;

        [JsonInclude]
        public readonly int VobID;

        [JsonInclude]
        public readonly int CellID;

        [JsonInclude]
        public readonly int StartSector;

        [JsonInclude]
        public readonly int EndSector;

        internal ADT(ReadOnlySpan<byte> data, int globalAddr)
        {
            this.Address = globalAddr;
            VobID = data.GetNbytes(0, 2);
            CellID = data[2];
            StartSector = data.GetNbytes(4, 4);
            EndSector = data.GetNbytes(8, 4);
        }
    }
}
