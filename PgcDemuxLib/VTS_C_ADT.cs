using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PgcDemuxLib
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

        public readonly int NumberOfADTs; // TODO remove

        private ADT[] List_ADT;
        public ReadOnlyIndexedProperty<int, ADT> ADTs; // TODO just add an indexer operator

        internal VTS_C_ADT(byte[] file, int addr, IfoParseContext context)
        {
            var ifo = context.ifo;
            ADTs = new((index) => List_ADT[index]);
            this.Address = addr;

            //this.NumberOfADTs = file.GetNbytes(addr, 2);
            this.NumberOfADTs = file.GetNbytes(addr + 4, 4);
            this.NumberOfADTs = (this.NumberOfADTs - 7) / 12;

            // TODO verify if this is needed or not
            // This is how NumberOfVobIDs was calculated in the original program, so verify our number is the same
            /*int check = file.GetNbytes(addr + 4, 4);
            check = (check - 7) / 12;
            if (check != this.NumberOfADTs)
            {
                throw new Exception("Assert failed: NumberOfVobIDs did not match the number of given bytes");
            }*/

            this.List_ADT = new ADT[NumberOfADTs];

            //Cells
            for (int nADT = 0; nADT < this.NumberOfADTs; nADT++)
            {
                int offset = 8 + 12 * nADT;
                List_ADT[nADT] = new ADT(file, addr + offset, context);
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

        internal ADT(byte[] file, int addr, IfoParseContext context)
        {
            this.VobID = file.GetNbytes(addr, 2);
            this.CellID = file[addr + 2];
            this.StartSector = file.GetNbytes(addr + 4, 4);
            this.EndSector = file.GetNbytes(addr + 8, 4);
        }
    }
}
