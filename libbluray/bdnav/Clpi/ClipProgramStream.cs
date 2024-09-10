using libbluray.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libbluray.bdnav.Clpi {
    public class ClipProgramStream {
        public UInt16 pid;
        public ClipStreamAttributes attributes = new();

        public bool Parse(BitStream bits) {
            this.pid = bits.Read<UInt16>();
            if (!this.attributes.Parse(bits))
            {
                return false;
            }

            return true;
        }
    }
}
