using libbluray.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libbluray.bdnav.Clpi {
    public class EpFine {
        public bool is_angle_change_point;
        public byte i_end_position_offset;
        public UInt16 pts_ep;
        public UInt32 spn_ep;

        public bool Parse(BitStream bits) {
            this.is_angle_change_point = bits.ReadBool();
            this.i_end_position_offset = bits.Read<byte>(3);
            this.pts_ep = bits.Read<UInt16>(11);
            this.spn_ep = bits.Read<UInt32>(17);

            return true;
        }
    }
}
