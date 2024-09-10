using libbluray.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libbluray.bdnav.Clpi {
    public class EpCoarse {
        public UInt32 ref_ep_fine_id;
        public UInt16 pts_ep;
        public UInt32 spn_ep;

        public bool Parse(BitStream bits) {
            this.ref_ep_fine_id = bits.Read<UInt32>(18);
            this.pts_ep = bits.Read<UInt16>(14);
            this.spn_ep = bits.Read<UInt32>();

            return true;
        }
    }
}
