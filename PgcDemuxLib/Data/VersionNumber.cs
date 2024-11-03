using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PgcDemuxLib.Data
{
    public struct VersionNumber
    {
        [JsonInclude]
        public readonly int Major;

        [JsonInclude]
        public readonly int Minor;

        internal VersionNumber(byte[] file, int addr)
        {
            int val = file[addr + 1];
            Major = val >> 4 & 0xF;
            Minor = val & 0xF;
        }
    }
}
