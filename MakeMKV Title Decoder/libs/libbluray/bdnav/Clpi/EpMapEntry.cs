using libbluray.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libbluray.bdnav.Clpi {
    public class EpMapEntry {
        public UInt16 pid;
        public byte ep_stream_type;
        public UInt16 num_ep_coarse;
        public UInt32 num_ep_fine;
        public Int64 ep_map_stream_start_addr;
        public EpCoarse[] coarse = Array.Empty<EpCoarse>();
        public EpFine[] fine = Array.Empty<EpFine>();

        public bool Parse(BitStream bits, Int64 ep_map_pos) {
            this.pid = bits.Read<UInt16>();
            bits.Skip(10);
            this.ep_stream_type = bits.Read<byte>(4);
            this.num_ep_coarse = bits.Read<UInt16>();
            this.num_ep_fine = bits.Read<UInt32>(18);
            this.ep_map_stream_start_addr = bits.Read<Int64>() * 8 + ep_map_pos;

            return true;
        }

        public bool ParseMapStream(BitStream bits) {
            bits.Seek(this.ep_map_stream_start_addr);
            UInt32 fine_start = bits.Read<UInt32>();

            coarse = new EpCoarse[this.num_ep_coarse];
            for (int i = 0; i < this.num_ep_coarse; i++)
            {
                this.coarse[i] = new EpCoarse();
                this.coarse[i].Parse(bits);
            }

            bits.Seek(this.ep_map_stream_start_addr + fine_start * 8);

            fine = new EpFine[this.num_ep_fine];
            for (int i = 0; i < this.num_ep_fine; i++)
            {
                this.fine[i] = new EpFine();
                this.fine[i].Parse(bits);
            }

            return true;
        }
    }
}
