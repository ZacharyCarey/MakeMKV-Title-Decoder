using libbluray.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace libbluray.bdnav.Indx {
    public class Indexes {

        public IndexPlayItem first_play = new();
        public IndexPlayItem top_menu = new();

        public UInt16 num_titles;
        public IndexTitle[] titles = Array.Empty<IndexTitle>();

        public bool Parse(BitStream bits) {
            UInt32 index_len = bits.Read<UInt32>();

            if (!first_play.Parse(bits)
                || !top_menu.Parse(bits))
            {
                return false;
            }

            this.num_titles = bits.Read<UInt16>();
            if (this.num_titles == 0)
            {
                // no "normal" titles - check for first play and top menu
                /*if ((index.first_play.object_type == indx_object_type.hdmv && index.first_play.hdmv.id_ref == 0xFFFF)
                    && (index.top_menu.object_type == indx_object_type.hdmv && index.top_menu.hdmv.id_ref == 0xFFFF))
                {
                    Utils.BD_DEBUG(LogLevel.Critical, "", "empty index");
                    return false;
                }*/
                return true;
            }

            this.titles = new IndexTitle[this.num_titles];
            for (int i = 0; i < this.num_titles; i++)
            {
                this.titles[i] = new();
                if (!this.titles[i].Parse(bits))
                {
                    return false;
                }
            }

            return true;
        }

    }
}
