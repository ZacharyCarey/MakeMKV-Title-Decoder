using libbluray.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libbluray.bdnav.Indx {

    public enum indx_hdmv_playback_type : UInt32 {
        movie = 0,
        interactive = 1,

        Unknown = 0xFFFFFFFF
    }

    public class IndexHdmvObject {
        public indx_hdmv_playback_type playback_type;
        public UInt16 id_ref;

        public bool Parse(BitStream bits) {
            this.playback_type = bits.ReadEnum(2, indx_hdmv_playback_type.Unknown);
            this.id_ref = bits.Read<UInt16>();
            bits.Skip(32);

            return true;
        }
    }
}
