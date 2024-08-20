using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.Data.MPLS {
    public struct SubPlayItemClip {

        public string clip_file_name;
        public string codec_id;
        public uint ref_to_stc_id;

        public void dump() {
            Console.WriteLine($"        sub play item clip dump");
            Console.WriteLine($"          clip_id / codec_id:  {clip_file_name} / {codec_id}");
            Console.WriteLine($"        ref_to_stc_id:         {ref_to_stc_id}");
        }

    }
}
