using libbluray.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libbluray.bdnav.Mpls
{
    public class AppInfo {
        const string module = "libbluray.bdnav.Mpls.AppInfo";

        public byte playback_type;
        public UInt16 playback_count;
        public UserOperationMask uo_mask = new();
        public bool random_access_flag;
        public bool audio_mix_flag;
        public bool lossless_bypass_flag;
        public bool mvc_base_view_r_flag;
        public bool sdr_conversion_notification_flag;

        public bool Parse(BitStream bits) {
            if (!bits.IsAligned())
            {
                Utils.BD_DEBUG(LogLevel.Warning, module, "_parse_appinfo: alignment error");
            }

            Int64 len = bits.Read<UInt32>();
            if (len < bits.AvailableBytes())
            {
                Utils.BD_DEBUG(LogLevel.Critical, module, "_parse_appinfo: unexpected end of file");
                return false;
            }

            bits.Skip(8);
            this.playback_type = bits.Read<byte>();
            if (this.playback_type == 2 || this.playback_type == 3) 
            {
                this.playback_count = bits.Read<UInt16>();
            } else
            {
                bits.Skip(16); // reserved
            }
            this.uo_mask.parse(bits);
            this.random_access_flag = bits.ReadBool();
            this.audio_mix_flag = bits.ReadBool();
            this.lossless_bypass_flag = bits.ReadBool();
            this.mvc_base_view_r_flag = bits.ReadBool();
            this.sdr_conversion_notification_flag = bits.ReadBool();
            bits.Skip(11);

            return true;
        }
    }
}
