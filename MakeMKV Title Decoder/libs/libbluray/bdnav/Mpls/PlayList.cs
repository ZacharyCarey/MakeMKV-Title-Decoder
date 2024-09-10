using libbluray.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libbluray.bdnav.Mpls {
    public class PlayList {
        const string module = "libbluray.bdnav.Mpls.PlayList";

        public UInt16 mark_count;
        public PlayItem[] play_item = Array.Empty<PlayItem>();
        public SubPath[] sub_path = Array.Empty<SubPath>();

        public bool Parse(BitStream bits) {
            // playlist length
            Int64 len = bits.Read<UInt32>();

            if (bits.AvailableBytes() < len)
            {
                Utils.BD_DEBUG(LogLevel.Critical, module, "_parse_playlist: unexpected end of file");
                return false;
            }

            // Skip reserved bytes
            bits.Skip(16);

            Int32 list_count = bits.Read<UInt16>();
            Int32 sub_count = bits.Read<UInt16>();

            if (list_count > 0)
            {
                this.play_item = new PlayItem[list_count];
                for (int i = 0; i < list_count; i++)
                {
                    this.play_item[i] = new PlayItem();
                    if (this.play_item[i].Parse(bits))
                    {
                        Utils.BD_DEBUG(LogLevel.Critical, module, "error parsing play list item");
                        return false;
                    }
                }
            }

            if (sub_count > 0)
            {
                this.sub_path = new SubPath[sub_count];
                for (int i = 0; i < sub_count; i++)
                {
                    this.sub_path[i] = new SubPath();
                    if (!this.sub_path[i].Parse(bits))
                    {
                        Utils.BD_DEBUG(LogLevel.Critical, module, "error parsing subpath");
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
