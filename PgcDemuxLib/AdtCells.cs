using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PgcDemuxLib
{
    internal class ADT_CELL
    {
        public int VobID;
        public int CellID;
        public int StartSector;
        public int EndSector;
        public int Size;
        public TimeSpan dwDuration;
    }

    internal class ADT_VID
    {
        public int VobID;
        public int Size;
        public int NumberOfCells;
        public TimeSpan dwDuration;
    }
}
