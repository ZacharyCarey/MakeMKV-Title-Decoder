using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PgcDemuxLib.Data.VTS
{
    public class VTS_TMAPTI
    {
        /// <summary>
        /// Byte index in file
        /// </summary>
        [JsonInclude]
        internal readonly int Address;

        internal VTS_TMAPTI(byte[] file, int addr)
        {
            Address = addr;
        }
    }
}
