using libbluray.disc;
using libbluray.file;
using libbluray.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace libbluray.bdnav.Indx {

    public class IndxFile {
        const string SIG1 = "INDX";

        public string indx_version = "";
        public IndexAppInfo app_info = new();
        public Indexes indexes = new();
        public IndexExtensionData extension = new();

        private bool ParseHeader(BitStream bs, out UInt32 index_start, out UInt32 extension_data_start, out string indx_version) {
            if (!bdmv.parse_header(bs, SIG1, out indx_version))
            {
                index_start = 0;
                extension_data_start = 0;
                return false;
            }

            index_start = bs.Read<UInt32>();
            extension_data_start = bs.Read<UInt32>();

            return true;
        }

        public bool Parse(BitStream bits) {
            UInt32 indexes_start, extension_data_start;

            if (!ParseHeader(bits, out indexes_start, out extension_data_start, out this.indx_version)
                || !this.app_info.Parse(bits)
             )
            {
                return false;
            }

            bits.SeekByte(indexes_start);
            if (!indexes.Parse(bits))
            {
                return false;
            }

            if (extension_data_start > 0)
            {
                extension.Parse(bits, extension_data_start);
            }

            return true;
        }

        public static IndxFile? Parse(BD_FILE_H fp) {
            BitStream bs = new(fp);
            IndxFile index = new IndxFile();

            if (index.Parse(bs))
            {
                return index;
            }

            return null;
        }

        public static IndxFile? _indx_get(BD_DISC disc, string path) {
            BD_FILE_H? fp = null;
            IndxFile? index = null;

            fp = disc.open_path(path);
            if (fp == null)
            {
                return null;
            }

            index = IndxFile.Parse(fp);
            fp.close();
            return index;
        }

        /// <summary>
        /// Parse index.bdmv
        /// </summary>
        public static IndxFile? get(BD_DISC disc) {
            IndxFile? index = _indx_get(disc, Path.Combine("BDMV", "index.bdmv"));
            if (index == null)
            {
                // try backup
                index = _indx_get(disc, Path.Combine("BDMV", "BACKUP", "index.bdmv"));
            }

            return index;
        }
    }
}
