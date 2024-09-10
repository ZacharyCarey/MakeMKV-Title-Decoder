using libbluray.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libbluray.bdnav.Clpi {
    public class ClipProgramInfo {
        public byte num_prog;
        public ClipProgram[] progs = null;

        public bool Parse(BitStream bits) {
            Int64 len = bits.Read<UInt32>();
            Int64 pos = bits.Position;

            // Skip the reserved byte
            bits.Skip(8);

            // Then get the number of sequences
            this.num_prog = bits.Read<byte>();

            this.progs = new ClipProgram[this.num_prog];
            for (int i = 0; i < this.num_prog; i++)
            {
                this.progs[i] = new ClipProgram();
                if (!this.progs[i].Parse(bits))
                {
                    return false;
                }
            }

            bits.Seek(pos + len * 8);

            return true;
        }
    }
}
