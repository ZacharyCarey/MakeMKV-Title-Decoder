using libbluray.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libbluray.bdnav.Clpi {
    public class ClipProgram {
        public UInt32 spn_program_sequence_start;
        public UInt16 program_map_pid;
        public byte num_streams;
        public byte num_groups;
        public ClipProgramStream[] streams = Array.Empty<ClipProgramStream>();

        public bool Parse(BitStream bits) {
            this.spn_program_sequence_start = bits.Read<UInt32>();
            this.program_map_pid = bits.Read<UInt16>();
            this.num_streams = bits.Read<byte>();
            this.num_groups = bits.Read<byte>();

            this.streams = new ClipProgramStream[this.num_streams];
            for (int j = 0; j < this.num_streams; j++)
            {
                this.streams[j] = new ClipProgramStream();
                if (!this.streams[j].Parse(bits))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
