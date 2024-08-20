using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.Data.Bluray.BlurayIndex
{
    public struct FirstPlayback
    {

        public uint object_type = 0;
        public uint hdmv_title_playback_type = 0;
        public uint mobj_id_ref = 0;
        public uint bd_j_title_playback_type = 0;
        public string bdjo_file_name;

        public FirstPlayback() { }

        public void dump()
        {
            string str = object_type == 0b01 ? "movie object"
                : object_type == 0b10 ? "BD-J object"
                     : "reserved";

            Console.WriteLine($"  first_playback:");
            Console.WriteLine($"    object_type:              {object_type} ({str})");

            if (object_type == 0b01)
            {
                str = hdmv_title_playback_type == 0b00 ? "movie title"
                       : hdmv_title_playback_type == 0b01 ? "interactive title"
                       : "reserved";

                Console.WriteLine($"    hdmv_title_playback_type: {hdmv_title_playback_type} ({str})");
                Console.WriteLine($"    mobj_id_ref:              0x{mobj_id_ref:x4}");
            }
            else
            {
                str = bd_j_title_playback_type == 0b10 ? "movie title"
                       : bd_j_title_playback_type == 0b11 ? "interactive title"
                       : "reserved";

                Console.WriteLine($"    bd_j_title_playback_type: {bd_j_title_playback_type} ({str})");
                Console.WriteLine($"    bjdo_file_name:           {bdjo_file_name}");
            }
        }

    }
}
