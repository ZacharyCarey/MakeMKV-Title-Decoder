using libbluray.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libbluray.bdnav.Clpi {
    public class ClipCPI {
        public byte type;
        // ep_map
        public byte num_stream_pid;
        public EpMapEntry[] entry = Array.Empty<EpMapEntry>();

        public bool Parse(BitStream bits) {
            Int64 len = bits.Read<UInt32>();
            if (len == 0)
            {
                return true;
            }

            bits.Skip(12);
            this.type = bits.Read<byte>(4);
            Int64 ep_map_pos = bits.Position;

            // EP Map starts here
            bits.Skip(8);
            this.num_stream_pid = bits.Read<byte>();

            this.entry = new EpMapEntry[this.num_stream_pid];
            for (int i = 0; i < this.num_stream_pid; i++)
            {
                this.entry[i] = new EpMapEntry();
                this.entry[i].Parse(bits, ep_map_pos);
            }
            for (int i = 0; i < this.num_stream_pid; i++)
            {
                if (!this.entry[i].ParseMapStream(bits))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
