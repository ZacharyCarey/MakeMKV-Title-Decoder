using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PgcDemuxLib
{
    public class VTS_PTT_SRPT
    {
        /// <summary>
        /// Byte index in file
        /// </summary>
        internal readonly int Address;

        internal VTS_PTT_SRPT(byte[] file, int addr)
        {
            this.Address = addr;

        }

    }
}
