using libbluray.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace libbluray.bdnav.Clpi {
    public class ClipExtension : ExtensionData {

        const string module = "lubbluray.bdnav.Clpi.ClipExtension";

        public ClipExtentStart extent_start = new(); /* extent start points (.ssif interleaving) */
        public ClipProgramInfo program_ss = new();
        public ClipCPI cpi_ss = new();

        protected override bool ParseExtensionData(BitStream bits, ushort id1, ushort id2) {
            if (id1 == 1)
            {
                if (id2 == 2)
                {
                    // LPCM down mix coefficient
                    //_parse_lpcm_down_mix_coeff(bits, &cl->lpcm_down_mix_coeff);
                    return false;
                }
            }

            if (id1 == 2)
            {
                if (id2 == 4)
                {
                    // Extent start point
                    return this.extent_start.Parse(bits);
                }
                if (id2 == 5)
                {
                    // ProgramInfo SS
                    return this.program_ss.Parse(bits);
                }
                if (id2 == 6)
                {
                    // CPI SS
                    this.cpi_ss.Parse(bits);
                }
            }

            Utils.BD_DEBUG(LogLevel.Critical, module, $"_parse_clpi_extension(): unhandled extension {id1}.{id2}");
            return false;
        }
    }
}
