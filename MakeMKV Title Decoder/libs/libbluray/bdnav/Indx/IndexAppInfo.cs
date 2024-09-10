using libbluray.bdnav.Mpls;
using libbluray.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libbluray.bdnav.Indx {

    public enum InitialOutputModePreference : UInt32 {
        _2D = 0,
        _3D = 1,

        Unknown = 0xFFFFFFFF
    }

    // TODO is same as other format?
    public enum indx_video_format : UInt32 {
        _format_ignored = 0,
        _480i = 1,
        _576i = 2,
        _480p = 3,
        _1080i = 4,
        _720p = 5,
        _1080p = 6,
        _576p = 7,

        Unknown = 0xFFFFFFFF
    }

    // TODO same as other format?
    public enum indx_frame_rate : UInt32 {
        _reserved1 = 0,
        _23_976 = 1,
        _24 = 2,
        _25 = 3,
        _29_97 = 4,
        _reserved2 = 5,
        _50 = 6,
        _59_94 = 7,

        Unknown = 0xFFFFFFFF
    }

    public class IndexAppInfo {
        const string module = "libbluray.bdnav.Indx.IndexAppInfo";

        /// <summary>
        /// 0 - 2D, 1 - 3D
        /// </summary>
        public InitialOutputModePreference initial_output_mode_preference;
        
        /// <summary>
        /// If stero-scopic content is present
        /// </summary>
        public bool content_exist_flag;
        public UInt32 initial_dynamic_range_type; // length 4
        public indx_video_format video_format; // length 4
        public indx_frame_rate frame_rate; // length 4
        public byte[] user_data = new byte[32];

        public bool Parse(BitStream bits) {
            bits.SeekByte(40);

            UInt32 len = bits.Read<UInt32>();

            if (len != 34)
            {
                Utils.BD_DEBUG(LogLevel.Warning, module, $"index.bdmv app_info length is {len}, expected 34 !");
            }

            bits.Skip(1);
            this.initial_output_mode_preference = bits.ReadEnum(1, InitialOutputModePreference.Unknown, module);
            this.content_exist_flag = bits.ReadBool();
            bits.Skip(1);
            this.initial_dynamic_range_type = bits.Read<byte>(4);
            this.video_format = bits.ReadEnum(4, indx_video_format.Unknown, module);
            this.frame_rate = bits.ReadEnum(4, indx_frame_rate.Unknown, module);

            bits.Read(this.user_data);

            return true;
        }
    }
}
