using libbluray.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libbluray.bdnav.Mpls {

    // TODO rename PlayListChapter
    public class PlayListMark {
        const string module = "libbluray.bdnv.Mpls.PlayListMark";

        public byte mark_type;
        public UInt16 play_item_ref;
        public UInt32 time;
        public UInt16 entry_es_pid;
        public UInt32 duration;

        public bool Parse(BitStream bits) {
            bits.Skip(8); // reserved
            this.mark_type = bits.Read<byte>();
            this.play_item_ref = bits.Read<UInt16>();
            this.time = bits.Read<UInt32>(); // TODO mpls_time_to_timestamp(m_bc->get_bits(32)) - play_item.in_time + play_item.relative_in_time;
            this.entry_es_pid = bits.Read<UInt16>();
            this.duration = bits.Read<UInt32>();

            return true;
        }

        public static bool Parse(BitStream bits, out PlayListMark[] result) {
            result = Array.Empty<PlayListMark>();

            Int64 len = bits.Read<UInt32>();

            if (bits.AvailableBytes() < len)
            {
                Utils.BD_DEBUG(LogLevel.Critical, module, "_parse_playlistmark: unexpected EOF");
                return false;
            }

            // Then get the number of marks
            UInt16 mark_count = bits.Read<UInt16>();

            if (bits.AvailableBytes() / 14 < mark_count)
            {
                Utils.BD_DEBUG(LogLevel.Critical, module, "_parse_playlistmark: unextected EOF");
                return false;
            }

            result = new PlayListMark[mark_count];
            for (int i = 0; i < mark_count; i++)
            {
                result[i] = new();
                if (!result[i].Parse(bits))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
