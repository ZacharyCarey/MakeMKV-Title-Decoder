using libbluray.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libbluray.bdnav.Clpi {

    public struct CLPI_FONT {
        public string file_id = ""; // length 6

        public CLPI_FONT() { }
    }

    public class FontInfo {
        public byte font_count;
        public CLPI_FONT[] font = Array.Empty<CLPI_FONT>();

        public bool Parse(BitStream bits) {
            bits.Skip(8);
            this.font_count = bits.Read<byte>();
            if (this.font_count > 0)
            {
                this.font = new CLPI_FONT[this.font_count];
                for (int i = 0; i < this.font_count; i++)
                {
                    this.font[i].file_id = bits.ReadString(5);
                    bits.Skip(8);
                }
            }

            return true;
        }
    }
}
