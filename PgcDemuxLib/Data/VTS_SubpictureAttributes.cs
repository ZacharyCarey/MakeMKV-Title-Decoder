using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PgcDemuxLib.Data
{
    public struct VTS_SubpictureAttributes
    {

        [JsonInclude, JsonConverter(typeof(JsonStringEnumConverter))]
        public readonly SubpictureEncoding Encoding;

        [JsonInclude]
        public readonly string? LanguageCode;

        internal VTS_SubpictureAttributes(byte[] file, int addr)
        {
            this.Encoding = Util.ParseEnum<SubpictureEncoding>((file[addr] >> 5) & 0b111);

            bool hasLanguageCode = (file[addr] & 0b11) == 1;
            if (hasLanguageCode)
            {
                LanguageCode = file.GetString(addr + 2, 2);
            }
        }
    }

    public enum SubpictureEncoding
    {
        Rle_2bit = 0
    }
}
