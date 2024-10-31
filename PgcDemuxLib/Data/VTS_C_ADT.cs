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

            //this.NumberOfADTs = file.GetNbytes(addr, 2);
            int numADTs = file.GetNbytes(addr + 4, 4);
            numADTs = (numADTs - 7) / 12;

            // TODO verify if this is needed or not
            // This is how NumberOfVobIDs was calculated in the original program, so verify our number is the same
            /*int check = file.GetNbytes(addr + 4, 4);
            check = (check - 7) / 12;
            if (check != this.NumberOfADTs)
            {
                throw new Exception("Assert failed: NumberOfVobIDs did not match the number of given bytes");
            }*/

            ADTs = new ADT[numADTs];

            //Cells
            for (int nADT = 0; nADT < numADTs; nADT++)
            {
                int offset = 8 + 12 * nADT;
                ADTs[nADT] = new ADT(file, addr + offset);
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
        internal readonly int Address;

        public readonly int VobID;
        public readonly int CellID;
        public readonly int StartSector;
        public readonly int EndSector;

        internal ADT(byte[] file, int addr)
        {
            VobID = file.GetNbytes(addr, 2);
            CellID = file[addr + 2];
            StartSector = file.GetNbytes(addr + 4, 4);
            EndSector = file.GetNbytes(addr + 8, 4);
        }
    }
}
