using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.Data.Bluray.BlurayIndex
{
    public struct TopMenu
    {

        public uint object_type = 0;
        public uint mobj_id_ref = 0;
        public string bdjo_file_name;

        public TopMenu() { }

        public void dump()
        {
            string str = object_type == 0b01 ? "movie object"
                     : object_type == 0b10 ? "BD-J object"
                     : "reserved";

            Console.WriteLine($"  top_menu:");
            Console.WriteLine($"    object_type:              {object_type} ({str})");

            if (object_type == 0b01)
            {
                Console.WriteLine($"    mobj_id_ref:              0x{mobj_id_ref:x4}");
            }
            else
            {
                Console.WriteLine($"    bjdo_file_name:           {bdjo_file_name}");
            }
        }

    }
}
