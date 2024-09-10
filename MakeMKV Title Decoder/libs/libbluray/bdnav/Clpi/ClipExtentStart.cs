using libbluray.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libbluray.bdnav.Clpi {
    /// <summary>
    /// Extent start points (profile 5 / version 2.4)
    /// </summary>
    public class ClipExtentStart {
        public UInt32 num_point;
        public UInt32[] point = Array.Empty<UInt32>();

        public bool Parse(BitStream bits) {
            bits.Skip(32); // length
            this.num_point = bits.Read<UInt32>();

            this.point = new UInt32[this.num_point];
            for (UInt32 i = 0; i < this.num_point; i++)
            {
                this.point[i] = bits.Read<UInt32>();
            }

            return true;
        }
    }
}
