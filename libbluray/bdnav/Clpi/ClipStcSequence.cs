using libbluray.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libbluray.bdnav.Clpi {
    public class ClipStcSequence {
        public UInt16 pcr_pid;
        public UInt32 spn_stc_start;
        public UInt32 presentation_start_time;
        public UInt32 presentation_end_time;
        public TimeSpan Length => TimeSpan.FromMilliseconds((presentation_end_time - presentation_start_time) / 45);

        public bool Parse(BitStream bits) {
            this.pcr_pid = bits.Read<UInt16>();
            this.spn_stc_start = bits.Read<UInt32>();
            this.presentation_start_time = bits.Read<UInt32>();
            this.presentation_end_time = bits.Read<UInt32>();
            return true;
        }
    }
}
