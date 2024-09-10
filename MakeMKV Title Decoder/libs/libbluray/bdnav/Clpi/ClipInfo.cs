using libbluray.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libbluray.bdnav.Clpi {

    // TODO
    public class CLPI_ATC_DELTA {
        public UInt32 delta;
        public string file_id = ""; // length 6
        public string file_code = ""; // length 5
    }

    public class ClipInfo {

        const string module = "libbluray.bdnav.Clpi.ClipInfo";

        public byte clip_stream_type;
        public byte application_type;
        public bool is_atc_delta;
        public UInt32 ts_recording_rate;
        public UInt32 num_source_packets;
        public TsTypeInfo ts_type_info = new();
        public byte atc_delta_count;
        public CLPI_ATC_DELTA[] atc_delta = Array.Empty<CLPI_ATC_DELTA>();
        public FontInfo font_info = new();      /* Text subtitle stream font files */

        public bool Parse(BitStream bits) {
            Int64 len = bits.Read<UInt32>();
            Int64 pos = bits.Position;

            bits.Skip(16); // reserved
            this.clip_stream_type = bits.Read<byte>();
            this.application_type = bits.Read<byte>();
            bits.Skip(31); // skip reserved 31 bits
            this.is_atc_delta = bits.ReadBool();
            this.ts_recording_rate = bits.Read<UInt32>();
            this.num_source_packets = bits.Read<UInt32>();

            // reserved 128 bytes
            bits.SkipBytes(128);

            ts_type_info.Parse(bits);

            if (this.is_atc_delta)
            {
                // TODO does not match libbluray vs https://github.com/lw/BluRay/wiki/ClipInfo
                /*bits.Skip(8); // reserved
                this.atc_delta_count = bits.Read<byte>();
                this.atc_delta = new CLPI_ATC_DELTA[this.atc_delta_count];
                for (ii = 0; ii < this.atc_delta_count; ii++)
                {
                    this.atc_delta[ii] = new();
                    this.atc_delta[ii].delta = bits.Read<UInt32>();
                    bits.read_string(out this.atc_delta[ii].file_id, 5);
                    bits.read_string(out this.atc_delta[ii].file_code, 4);
                    bits.skip_bits(8);
                }*/
            }

            if(this.application_type == 6) // Sub TS for a sub-path of Text subtitle
            {
                //this.font_info.Parse(bits);
            }

            bits.Seek(pos + len * 8);

            return true;
        }
    }
}
