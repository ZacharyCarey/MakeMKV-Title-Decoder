using libbluray.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libbluray.bdnav.Clpi {
    public class TsTypeInfo {
        const string module = "libbluray.bdnav.Clpi.TsTypeInfo";
        const Int64 Length = 256;

        //public byte validity;
        //public string format_id = ""; // length 5

        public bool Parse(BitStream bits) {
            Int64 pos = bits.Position;
            bool result = true;

            // TODO not sure how to properly read
            /*UInt16 len = bits.Read<UInt16>();
            if (len != 0)
            {
                this.validity = bits.Read<byte>();
                this.format_id = bits.ReadString(4);
                // Seek past the stuff we dont know anything about
            }

            // font info
            if (this.application_type == 6) //Sub TS for a sub-path of Text subtitle
            {
                CLPI_FONT_INFO fi = this.font_info;
                bits.skip_bits(8);
                fi.font_count = bits.Read<byte>();
                if (fi.font_count > 0)
                {
                    fi.font = new CLPI_FONT[fi.font_count];
                    for (ii = 0; ii < fi.font_count; ii++)
                    {
                        bits.read_string(out fi.font[ii].file_id, 5);
                        bits.skip_bits(8);
                    }
                }
            }*/

            bits.Seek(pos + Length);
            return result;
        }
    }
}
