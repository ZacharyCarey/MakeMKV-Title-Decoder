using libbluray.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libbluray.bdnav {
    internal static class bdmv {

        const string module = "DBG_NAV";

        public static bool parse_header(BitStream bs, string type, out string version) {
            string tag;
            string ver;

            bs.Seek(0);

            // Read and verify magic bytes and version code
            if (bs.AvailableBytes() < 8)
            {
                Utils.BD_DEBUG(LogLevel.Critical, module, $"bdmv_parse_header({type}): unexpected EOF");
                version = "0";
                return false;
            }

            tag = bs.ReadString(4);
            ver = bs.ReadString(4);

            if (tag != type)
            {
                Utils.BD_DEBUG(LogLevel.Critical, module, $"bdmv_parse_header({type}): invalid signature {tag}");
                version = "0";
                return false;
            }

            switch(ver)
            {
                case "0100":
                case "0200":
                case "0240":
                case "0300":
                    version = ver;
                    return true;
                default:
                    Utils.BD_DEBUG(LogLevel.Critical, module, $"bdmv_parse_header({type}): unsupported file version {ver}");
                    version = "0";
                    return false;
            }
        }

    }
}
