using libbluray.disc;
using libbluray.file;
using libbluray.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libbluray.bdnav {
    public class BDID_DATA {

        const string module = "DBG_NAV";
        const string BDID_SIG1 = "BDID";

        public string org_id = ""; // length 9
        public string disc_id = ""; // length 33

        internal static BDID_DATA bdid_get(BD_DISC disc) {
            BDID_DATA? bdid = _bdid_get(disc, Path.Combine("CERTIFICATE", "id.bdmv"));

            // If failed, try backup file
            if (bdid == null)
            {
                bdid = _bdid_get(disc, Path.Combine("CERTIFICATE", "BACKUP", "id.bdmv"));
            }

            return bdid;
        }

        private static BDID_DATA? _bdid_get(BD_DISC disc, string path) {
            BD_FILE_H? fp = null;
            BDID_DATA? bdid = null;

            fp = disc.open_path(path);
            if (fp == null)
            {
                return null;
            }

            bdid = _bdid_parse(fp);
            fp.close();
            return bdid;
        }

        private static bool _parse_header(BitStream bs, out UInt32 data_start, out UInt32 extension_data_start) {
            string temp;
            if (!bdmv.parse_header(bs, BDID_SIG1, out temp))
            {
                data_start = 0;
                extension_data_start = 0;
                return false;
            }

            data_start = bs.Read<UInt32>();
            extension_data_start = bs.Read<UInt32>();

            return true;
        }

        private static BDID_DATA? _bdid_parse(BD_FILE_H? fp) {
            BitStream bs = new(fp);
            BDID_DATA? bdid = null;

            UInt32 data_start;
            UInt32 extension_data_start;
            byte[] tmp = new byte[16];

            if (!_parse_header(bs, out data_start, out extension_data_start))
            {
                Utils.BD_DEBUG(LogLevel.Critical, module, "id.bdmv: invalid header");
                return null;
            }

            bs.SeekByte(40);

            bdid = new();
            Span<byte> tmp2 = tmp.AsSpan(0, 4);
            bs.Read(tmp2);
            tmp2.print_hex(out bdid.org_id);

            bs.Read(tmp);
            tmp.print_hex(out bdid.disc_id);

            if (extension_data_start > 0)
            {
                Utils.BD_DEBUG(LogLevel.Critical, module, "id.bdmv: ignoring unknown extension data");
            }

            return bdid;
        }
    }
}
