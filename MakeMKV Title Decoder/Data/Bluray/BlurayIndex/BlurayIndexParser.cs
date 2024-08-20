using MakeMKV_Title_Decoder.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.Data.Bluray.BlurayIndex
{
    public class BlurayIndexParser : ByteParser
    {

        public uint index_start = 0;
        public BlurayIndex BlurayIndex = new();

        public BlurayIndexParser(string path) : base(path)
        {

        }

        public bool Parse()
        {
            try
            {
                parse_header();
                parse_index();

#if DEBUG
                dump();
#endif
                return true;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Bluray Index Exception: {ex.Message}");
                Console.ResetColor();
                return false;
            }
        }

        public void dump()
        {
            Console.WriteLine($"Index parser dump:");
            Console.WriteLine($"  ok:          {true}");
            Console.WriteLine($"  index_start: {index_start}");

            BlurayIndex.dump();
        }

        private void parse_header()
        {
            Seek(0);

            var magic = ReadString(4);
            Console.WriteLine($"File magic 1: {magic}");

            if (magic != "INDX")
            {
                throw new FormatException("Failed to find magic number");
            }

            magic = ReadString(4);
            Console.WriteLine($"File magic 2: {magic}");

            if (magic != "0100" && magic != "0200" && magic != "0300")
            {
                throw new FormatException("Unknown file version.");
            }

            index_start = ReadUInt32();
        }

        private void parse_index()
        {
            Seek((int)index_start);

            Skip(4); // length

            parse_first_playback();
            parse_top_menu();

            uint number_of_titles = ReadUInt16();

            BlurayIndex.titles = new BlurayTitle[number_of_titles];
            for (uint title_index = 0; title_index < number_of_titles; title_index++)
            {
                parse_title(title_index);
            }
        }

        private void parse_title(uint title_index)
        {
            BlurayTitle title = new();

            title.object_type = Read<uint>(2);
            title.access_type = Read<uint>(2);
            Skip(0, 28); // reserved

            if (title.object_type == 0b01)
            {
                title.hdmv_title_playback_type = Read<uint>(2);
                Skip(0, 14); // reserved / alignment
                title.mobj_id_ref = ReadUInt16();
                Skip(4); // reserved
            }
            else if (title.object_type == 0b10)
            {
                title.bd_j_title_playback_type = Read<uint>(2);
                Skip(0, 14); // reserved / alignment
                title.bdjo_file_name = ReadString(5);
                Skip(1); // reserved / alignment
            }
            else
            {
                throw new Exception($"Unknown BlurayTitle.object_type: {title.object_type}");
            }

            BlurayIndex.titles[title_index] = title;
        }

        private void parse_first_playback()
        {
            var fp = new FirstPlayback();

            fp.object_type = Read<uint>(2);

            Skip(0, 30); // reserved

            if (fp.object_type == 0b01)
            {
                fp.hdmv_title_playback_type = Read<uint>(2);
                Skip(0, 14);
                fp.mobj_id_ref = ReadUInt16();
                Skip(4);
            }
            else if (fp.object_type == 0b10)
            {
                fp.bd_j_title_playback_type = Read<uint>(2);
                Skip(0, 14);
                fp.bdjo_file_name = ReadString(5);
                Skip(1);
            }
            else
            {
                throw new Exception($"Unknown BlurayIndex.first_playback.object_type: {fp.object_type}");
            }

            BlurayIndex.first_playback = fp;
        }

        private void parse_top_menu()
        {
            TopMenu tm = new();

            tm.object_type = Read<uint>(2);

            Skip(0, 30);

            if (tm.object_type == 0b01)
            {
                Skip(0, 2 + 14); // 01, alignment
                tm.mobj_id_ref = ReadUInt16();
                Skip(4);
            }
            else if (tm.object_type == 0b10)
            {
                Skip(0, 2 + 14); // 11, alignment
                tm.bdjo_file_name = ReadString(5);
                Skip(1);
            }
            else
            {
                throw new Exception($"Unknown TopMenu.object_type: {tm.object_type}");
            }

            BlurayIndex.top_menu = tm;
        }
    }
}
