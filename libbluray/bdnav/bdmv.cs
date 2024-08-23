using libbluray.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libbluray.bdnav {
    internal static class bdmv {

        const string module = "DBG_NAV";

        public static bool parse_extension_data(BITSTREAM bits, Int32 start_address, Func<BITSTREAM, Int32, Int32, object, bool> handler, object handle) {
            return exdata.bdmv_parse_extension_data(bits, start_address, handler, handle);
        }

        public static bool parse_header(BITSTREAM bs, string type, out string version) {
            string tag;
            string ver;

            if (bs.seek_byte(0) < 0)
            {
                Utils.BD_DEBUG(LogLevel.Critical, module, $"bdmv_parse_header({type}): seek failed");
                version = "0";
                return false;
            }

            // Read and verify magic bytes and version code
            if (bs.avail() / 8 < 8)
            {
                Utils.BD_DEBUG(LogLevel.Critical, module, $"bdmv_parse_header({type}): unexpected EOF");
                version = "0";
                return false;
            }

            bs.read_string(out tag, 4);
            bs.read_string(out ver, 4);

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
