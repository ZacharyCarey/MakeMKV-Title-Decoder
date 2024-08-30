using libbluray.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libbluray.bdnav.Clpi {
    public class SequenceInfo {
        public byte num_atc_seq;
        public ClipAtcSequence[] atc_seq = Array.Empty<ClipAtcSequence>();

        public bool Parse(BitStream bits) {
            Int64 len = bits.Read<UInt32>();
            Int64 pos = bits.Position;

            bits.Skip(8); // reserved
            // Get the number of sequences
            num_atc_seq = bits.Read<byte>();

            ClipAtcSequence[] atc_seq = new ClipAtcSequence[this.num_atc_seq];
            this.atc_seq = atc_seq;
            for (int i = 0; i < this.num_atc_seq; i++)
            {
                atc_seq[i] = new();
                atc_seq[i].Parse(bits);
            }

            bits.Seek(pos + len * 8);

            return true;
        }
    }
}
