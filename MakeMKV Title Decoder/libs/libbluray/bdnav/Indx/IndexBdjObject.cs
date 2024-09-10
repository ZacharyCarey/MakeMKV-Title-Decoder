using libbluray.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libbluray.bdnav.Indx {

    public enum indx_bdj_playback_type : UInt32 {
        movie = 2,
        interactive = 3,

        Unknown = 0xFFFFFFFF
    }

    public class IndexBdjObject {
        const string module = "libbluray.bdnav.Indx.IndexBdjObject";

        public indx_bdj_playback_type playback_type;
        public string name = ""; // length 6

        public bool Parse(BitStream bits) {
            this.playback_type = bits.ReadEnum(2, indx_bdj_playback_type.Unknown, module);
            this.name = bits.ReadString(5);
            bits.Skip(8);

            return true;
        }
    }
}
