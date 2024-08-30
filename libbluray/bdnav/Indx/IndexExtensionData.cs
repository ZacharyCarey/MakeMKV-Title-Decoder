using libbluray.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace libbluray.bdnav.Indx {
    public class IndexExtensionData : ExtensionData {
        const string module = "libbluray.bdnav.Indx.IndexExtensionData";

        /* UHD extension */
        public byte disc_type;
        public bool exist_4k_flag;
        public bool hdrplus_flag;

        /// <summary>
        /// dolby vision
        /// </summary>
        public bool dv_flag;
        public byte hdr_flags;

        protected override bool ParseExtensionData(BitStream bits, ushort id1, ushort id2) {
            if (id1 == 3)
            {
                if (id2 == 1)
                {
                    return ParseExtensionHEVC(bits);
                }
            }

            Utils.BD_DEBUG(LogLevel.Critical, module, $"_parse_indx_extension(): unknown extension {id1}.{id2}");

            return true;
        }

        private bool ParseExtensionHEVC(BitStream bits) {
            UInt32 unk0, unk1, unk2, unk3, unk4, unk5;

            UInt32 len = bits.Read<UInt32>(32);
            if (len < 8)
            {
                Utils.BD_DEBUG(LogLevel.Critical, module, $"index.bdmv: unsupported extension 3.1 length ({len})");
                return false;
            }

            this.disc_type = bits.Read<byte>(4);
            unk0 = bits.Read<byte>(3);
            this.exist_4k_flag = bits.ReadBool();
            unk1 = bits.Read<byte>();
            unk2 = bits.Read<byte>(3);
            this.hdrplus_flag = bits.ReadBool();
            unk3 = bits.Read<byte>(1);
            this.dv_flag = bits.ReadBool();
            this.hdr_flags = bits.Read<byte>(2);
            unk4 = bits.Read<byte>(8);
            unk5 = bits.Read<UInt32>();

            Utils.BD_DEBUG(module, $"UHD disc type: {this.disc_type}, 4k: {this.exist_4k_flag}, HDR: {this.hdr_flags}, HDR10+: {this.hdrplus_flag}, Dolby Vision: {this.dv_flag}");
            if ((unk0 | unk1 | unk2 | unk3 | unk4 | unk5) != 0)
            {
                Utils.BD_DEBUG(LogLevel.Critical, module, $"index.bdmv: unknown data in extension 3.1: 0x{unk0:X1} 0x{unk1:X1} 0x{unk2:X1} 0x{unk3:X1} 0x{unk4:X1} 0x{unk5:X4}");
            }

            return true;
        }
    }
}
