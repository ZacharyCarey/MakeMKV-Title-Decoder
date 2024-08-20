﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.Data.Bluray.MPLS
{
    public struct Header
    {

        public string type_indicator1;
        public string type_indicator2;
        public uint playlist_pos;
        public uint chapter_pos;
        public uint ext_pos;

        public void dump()
        {
            Console.WriteLine($"  header dump");
            Console.WriteLine($"    type_indicator1 & 2:          {type_indicator1} / {type_indicator2}");
            Console.WriteLine($"    playlist / chapter / ext pos: {playlist_pos} / {chapter_pos} / {ext_pos}");
        }
    }
}