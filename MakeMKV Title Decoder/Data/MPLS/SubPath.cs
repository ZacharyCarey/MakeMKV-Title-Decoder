using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.Data.MPLS {

    public enum SubPathType {
        reserved1 = 0,
        reserved2 = 1,
        primary_audio_of_browsable_slideshow = 2,
        interactive_graphics_presentation_menu = 3,
        text_subtitle_presentation = 4,
        out_of_mux_synchronous_elementary_streams = 5,
        out_of_mux_asynchronous_picture_in_picture = 6,
        in_mux_synchronous_picture_in_picture = 7,
    }

    public struct SubPath {

        public SubPathType type;
        public bool is_repeat_sub_path;
        public SubPlayItem[] items;

        public void dump() {
            Console.WriteLine("    sub path dump");
            Console.WriteLine($"      type / is_repeat:        {(int)type} [{type.ToString()}] / {is_repeat_sub_path}");
        
            foreach(var item in items)
            {
                item.dump();
            }
        }

    }
}
