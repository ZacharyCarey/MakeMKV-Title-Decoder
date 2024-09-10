using libbluray.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libbluray.bdnav.Mpls {
    public class SubPath {
        const string module = "libbluray.bdnav.Mpls.SubPath";

        /// <summary>
        /// enum mpls_sub_path_type
        /// </summary>
        public byte type;
        public bool is_repeat;
        public byte sub_playitem_count;
        public SubPlayItem[] sub_play_item = Array.Empty<SubPlayItem>();

        public bool Parse(BitStream bits) {
            if (!bits.IsAligned())
            {
                Utils.BD_DEBUG(LogLevel.Warning, module, "_parse_subpath: alignment error");
            }

            // PlayItem Length
            UInt32 len = bits.Read<UInt32>();
            Int64 pos = bits.Position;

            bits.Skip(8);
            this.type = bits.Read<byte>();
            bits.Skip(15);
            this.is_repeat = bits.ReadBool();
            bits.Skip(8);
            this.sub_playitem_count = bits.Read<byte>();

            if (this.sub_playitem_count > 0)
            {
                this.sub_play_item = new SubPlayItem[this.sub_playitem_count];
                for (int i = 0; i < this.sub_playitem_count; i++)
                {
                    this.sub_play_item[i] = new SubPlayItem();
                    if (!this.sub_play_item[i].Parse(bits))
                    {
                        Utils.BD_DEBUG(LogLevel.Critical, module, "error parsing sub play item");
                        return false;
                    }
                }
            }

            // Seek to end of subpath
            bits.Seek(pos + len * 8);
            if (bits.EOF)
            {
                return false;
            }

            return true;
        }
    }
}
