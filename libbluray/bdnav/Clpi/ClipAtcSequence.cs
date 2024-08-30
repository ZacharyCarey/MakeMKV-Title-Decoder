using libbluray.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libbluray.bdnav.Clpi {
    public class ClipAtcSequence {
        public UInt32 spn_atc_start;
        public byte num_stc_seq;
        public byte offset_stc_id;
        public ClipStcSequence[] stc_seq = null;

        public bool Parse(BitStream bits) {
            this.spn_atc_start = bits.Read<UInt32>();
            this.num_stc_seq = bits.Read<byte>();
            this.offset_stc_id = bits.Read<byte>();

            ClipStcSequence[] stc_seq = new ClipStcSequence[this.num_stc_seq];
            this.stc_seq = stc_seq;
            for (int j = 0; j < this.num_stc_seq; j++)
            {
                stc_seq[j] = new();
                if (!stc_seq[j].Parse(bits))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
