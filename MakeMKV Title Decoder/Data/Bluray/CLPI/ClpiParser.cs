using MakeMKV_Title_Decoder.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.Data.Bluray.CLPI
{
    public class ClpiParser : ByteParser
    {

        const string FILE_MAGIC = "HDMV";
        const string FILE_MAGIC_2A = "0200";
        const string FILE_MAGIC_2B = "0100";
        const string FILE_MAGIC_2C = "0300";

        uint m_sequence_info_start;
        uint m_program_info_start;
        uint m_characteristic_point_info_start;

        public ClpiProgram[] m_programs;
        public EpMapOneStream[] m_ep_map;

        public ClpiParser(string path) : base(path)
        {

        }

        public bool Parse()
        {
            try
            {
                parse_header();
                parse_program_info();
                parse_characteristic_point_info();

#if DEBUG
                dump();
#endif
                return true;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"CLPI Exception: {ex.Message}");
                Console.ResetColor();
                return false;
            }
        }

        public void dump()
        {
            Console.WriteLine($"Parser dump:");
            Console.WriteLine($"  sequence_info_start:             {m_sequence_info_start}");
            Console.WriteLine($"  program_info_start:              {m_program_info_start}");
            Console.WriteLine($"  characteristic_point_info_start: {m_characteristic_point_info_start}");

            foreach (var programs in m_programs)
            {
                programs.dump();
            }

            Console.WriteLine("Characteristic point info dump:");

            uint index = 0;
            foreach (var ep_map in m_ep_map)
            {
                ep_map.dump(index);
            }
        }

        private void parse_header()
        {
            Seek(0);

            string magic = ReadString(4);
            Console.WriteLine($"File magic 1: {magic}");
            if (magic != FILE_MAGIC)
            {
                throw new Exception("Magic number 1 did not match.");
            }

            magic = ReadString(4);
            Console.WriteLine($"File magic 2: {magic}");
            if (magic != FILE_MAGIC_2A && magic != FILE_MAGIC_2B && magic != FILE_MAGIC_2C)
            {
                throw new Exception("Unknown CLPI version");
            }

            m_sequence_info_start = ReadUInt32();
            m_program_info_start = ReadUInt32();
            m_characteristic_point_info_start = ReadUInt32();
        }

        private void parse_program_info()
        {
            Seek((int)m_program_info_start);

            Skip(5); // x4 length, x1 reserved
            uint num_program_streams = ReadUInt8();

            Console.WriteLine($"num_program_streams: {num_program_streams}");

            m_programs = new ClpiProgram[num_program_streams];
            for (uint program_index = 0; program_index < num_program_streams; program_index++)
            {
                ClpiProgram program = new();
                m_programs[program_index] = program;

                program.spn_program_sequence_start = ReadUInt32();
                program.program_map_pid = ReadUInt16();
                program.num_streams = ReadUInt8();
                program.num_groups = ReadUInt8();

                program.program_streams = new ProgramStream[program.num_streams];
                for (uint stream_index = 0; stream_index < program.num_streams; stream_index++)
                {
                    parse_program_stream(program, stream_index);
                }
            }
        }

        private void parse_program_stream(ClpiProgram program, uint stream_index)
        {
            ProgramStream stream = new();
            program.program_streams[stream_index] = stream;

            stream.pid = ReadUInt16();

            Debug.Assert(Index.Bit == 0, "Bit position not divisible by 8.");

            uint length_in_bytes = ReadUInt8();
            BitIndex position_in_bits = Index;

            stream.coding_type = ReadUInt8();

            string language = "";

            switch (stream.coding_type)
            {
                case 0x01:
                case 0x02:
                case 0xea:
                case 0x1b:
                    stream.format = Read<uint>(4);
                    stream.rate = Read<uint>(4);
                    stream.aspect = Read<uint>(4);
                    Skip(0, 2);
                    stream.oc_flag = Read<uint>(1);
                    Skip(0, 1);
                    break;
                case 0x03:
                case 0x04:
                case 0x80:
                case 0x81:
                case 0x82:
                case 0x83:
                case 0x84:
                case 0x85:
                case 0x86:
                case 0xa1:
                case 0xa2:
                    stream.format = Read<uint>(4);
                    stream.rate = Read<uint>(4);
                    language = ReadString(3);
                    break;
                case 0x90:
                case 0x91:
                case 0xa0:
                    language = ReadString(3);
                    break;
                case 0x92:
                    stream.char_code = ReadUInt8();
                    language = ReadString(3);
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Unknown coding type {stream.coding_type}");
                    Console.ResetColor();
                    break;
            }

            stream.language = language;

            position_in_bits.Byte += (int)length_in_bytes;
            Seek(position_in_bits);
        }

        private void parse_characteristic_point_info()
        {
            Seek((int)m_characteristic_point_info_start);

            var length = ReadUInt32();
            if (length < 2)
            {
                return;
            }

            Skip(0, 12);
            var cpi_type = Read<uint>(4);
            if (cpi_type == 1) // 1 == EP_map
            {
                parse_ep_map();
            }
        }

        private void parse_ep_map()
        {
            var ep_map_start = Index;

            Skip(1); // reserved

            var num_stream_pid_entries = ReadUInt8();
            m_ep_map = new EpMapOneStream[num_stream_pid_entries];
            for (uint index = 0; index < num_stream_pid_entries; index++)
            {
                var entry = new EpMapOneStream();

                entry.pid = ReadUInt16();
                Skip(0, 10); // reserved
                entry.type = Read<uint>(4);
                entry.num_coarse_points = Read<uint>(16);
                entry.num_fine_points = Read<uint>(18);
                entry.address = ReadUInt32();

                m_ep_map[index] = new();
            }

            for (int index = 0; index < m_ep_map.Length; index++)
            {
                BitIndex addr = ep_map_start;
                addr.Byte += (int)m_ep_map[index].address;
                Seek(addr);
                parse_ep_map_for_one_stream_pid(ref m_ep_map[index]);
            }
        }

        private void parse_ep_map_for_one_stream_pid(ref EpMapOneStream map)
        {
            if (map.num_coarse_points == 0)
            {
                map.coarse_points = new CoarsePoint[0];
                map.fine_points = new FinePoint[0];
                map.points = new Point[0];

                return;
            }

            var start = Index;
            var fine_table_start = ReadUInt32();

            map.coarse_points = new CoarsePoint[map.num_coarse_points];
            for (uint index = 0; index < map.num_coarse_points; index++)
            {
                var coarse_point = new CoarsePoint();

                coarse_point.ref_to_fine_id = Read<uint>(18);
                coarse_point.pts = Read<uint>(14);
                coarse_point.spn = ReadUInt32();

                map.coarse_points[index] = coarse_point;
            }

            start.Byte += (int)fine_table_start;
            Seek(start);

            uint current_coarse_point = 0;
            uint next_coarse_point = current_coarse_point + 1;
            uint coarse_end = 0;

            for (uint index = 0; index < map.num_fine_points; index++)
            {
                Skip(1 + 3); // is_angle_change_point, l_end_position_offset

                var fine_point = new FinePoint();
                fine_point.pts = Read<uint>(11);
                fine_point.spn = Read<uint>(17);
                map.fine_points[index] = fine_point;

                if (next_coarse_point < coarse_end && index >= map.coarse_points[next_coarse_point].ref_to_fine_id)
                {
                    current_coarse_point++;
                    next_coarse_point++;
                }

                var point = new Point();
                point.pts = ((map.coarse_points[current_coarse_point].pts & ~0x01ul) << 19) + (fine_point.pts << 9);
                point.pts = point.pts * 100000 / 9; // mpeg
                point.spn = (map.coarse_points[current_coarse_point].spn & ~0x1fffful) + fine_point.spn;

                map.points[index] = point;
            }
        }
    }
}
