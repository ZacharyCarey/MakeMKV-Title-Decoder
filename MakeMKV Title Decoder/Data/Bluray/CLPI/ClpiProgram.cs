using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.Data.Bluray.CLPI
{
    public class ClpiProgram
    {

        public uint spn_program_sequence_start = 0;
        public uint program_map_pid = 0;
        public uint num_streams = 0;
        public uint num_groups = 0;
        public ProgramStream[] program_streams;

        public ClpiProgram() { }

        public void dump()
        {
            Console.WriteLine($"Program dump:");
            Console.WriteLine($"  spn_program_sequence_start: {spn_program_sequence_start}");
            Console.WriteLine($"  program_map_pid:            {program_map_pid}");
            Console.WriteLine($"  num_streams:                {num_streams}");
            Console.WriteLine($"  num_groups:                 {num_groups}");

            foreach (var program_stream in program_streams)
            {
                program_stream.dump();
            }
        }

    }
}
