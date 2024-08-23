using libbluray.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libbluray.bdnav {
    public static class exdata {

        public static bool bdmv_parse_extension_data(BITSTREAM bits, Int32 start_address, Func<BITSTREAM, Int32, Int32, object, bool> handler, object handle) {
            Int64 length;
            Int32 num_entries, n;

            if (start_address < 1) return false;
            if (start_address > bits.end() - 12) return false;

            if (bits.seek_byte(start_address) < 0)
            {
                return false;
            }

            length = bits.read(32); // length of extension data block
            if (length < 1) return false;
            bits.skip(32); // relative start address of extension data
            bits.skip(24); // padding
            num_entries = (Int32)bits.read(8);

            if (start_address > bits.end() - 12 - num_entries * 12) return false;

            for(n = 0; n < num_entries; n++)
            {
                UInt16 id1 = (UInt16)bits.read(16);
                UInt16 id2 = (UInt16)bits.read(16);
                Int64 ext_start = bits.read(32);
                Int64 ext_len = bits.read(32);

                Int64 saved_pos = bits.pos() >> 3;

                if (ext_start + start_address + ext_len > bits.end()) return false;

                if (bits.seek_byte(start_address + ext_start) >= 0)
                {
                    handler(bits, id1, id2, handle);
                }

                if (bits.seek_byte(saved_pos) < 0)
                {
                    return false;
                }
            }

            return true;
        }

    }
}
