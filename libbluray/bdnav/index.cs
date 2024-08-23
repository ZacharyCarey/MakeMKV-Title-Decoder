using libbluray.disc;
using libbluray.file;
using libbluray.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libbluray.bdnav {
    public enum indx_video_format {
        _format_ignored,
        _480i,
        _576i,
        _480p,
        _1080i,
        _720p,
        _1080p,
        _576p,
    }

    public enum indx_frame_rate {
        _reserved1,
        _23_976,
        _24,
        _25,
        _29_97,
        _reserved2,
        _50,
        _59_94,
    }

    public enum indx_object_type {
        hdmv = 1,
        bdj = 2,
    }

    public enum indx_hdmv_playback_type {
        movie = 0,
        interactive = 1,
    }

    public enum indx_bdj_playback_type {
        movie = 2,
        interactive = 3,
    }

    public enum indx_access_type {
        /// <summary>
        /// jump into this title is permitted.  title number may be shown on UI.
        /// </summary>
        permitted = 0,

        /// <summary>
        /// jump into this title is prohibited. title number may be shown on UI.
        /// </summary>
        prohibited = 1,

        /// <summary>
        /// jump into this title is prohibited. title number shall not be shown on UI.
        /// </summary>
        hidden = 3,

        /// <summary>
        /// if set, jump to this title is not allowed
        /// </summary>
        PROHIBITED_MASK = 0x01,

        /// <summary>
        /// if set, title number shall not be displayed on UI
        /// </summary>
        HIDDEN_MASK = 0x02
    }

    public class INDX_APP_INFO {
        /// <summary>
        /// 0 - 2D, 1 - 3D
        /// </summary>
        public bool initial_output_mode_preference;
        public bool content_exist_flag;
        public UInt32 initial_dynamic_range_type; // length 4
        public UInt32 video_format; // length 4
        public UInt32 frame_rate; // length 4
        public byte[] user_data = new byte[32];
    }

    public class INDX_BDJ_OBJ {
        public indx_bdj_playback_type playback_type;
        public string name = ""; // length 6
    }

    public class INDX_HDMV_OBJ {
        public indx_hdmv_playback_type playback_type;
        public UInt16 id_ref;
    }

    public class INDX_PLAY_ITEM {
        public indx_object_type object_type;
        public INDX_BDJ_OBJ bdj = new();
        public INDX_HDMV_OBJ hdmv = new();
    }

    public class INDX_TITLE {
        public indx_object_type object_type;
        public indx_access_type access_type;
        public INDX_BDJ_OBJ bdj = new();
        public INDX_HDMV_OBJ hdmv = new();
    }

    public class INDX_ROOT {
        public INDX_APP_INFO app_info = new();
        public INDX_PLAY_ITEM first_play = new();
        public INDX_PLAY_ITEM top_menu = new();

        public UInt16 num_titles;
        public INDX_TITLE[] titles = null;

        public string indx_version = "";

        /* UHD extension */
        public byte disc_type;
        public bool exist_4k_flag;
        public bool hdrplus_flag;

        /// <summary>
        /// dolby vision
        /// </summary>
        public bool dv_flag;
        public byte hdr_flags;
    }

    public static class indx {

        const string module = "DBG_NAV";
        const string module2 = "DBG_HDMV";

        private static bool _parse_hdmv_obj(BITSTREAM bs, INDX_HDMV_OBJ hdmv) {
            hdmv.playback_type = (indx_hdmv_playback_type)bs.read(2);
            bs.skip(14);
            hdmv.id_ref = (UInt16)bs.read(16);
            bs.skip(32);

            if (!Enum.IsDefined(hdmv.playback_type))
            {
                Utils.BD_DEBUG(LogLevel.Critical, module, $"index.bdmv: invalid HDMV playback type {hdmv.playback_type}");
            }

            return true;
        }

        private static bool _parse_bdj_obj(BITSTREAM bs, INDX_BDJ_OBJ bdj) {
            bdj.playback_type = (indx_bdj_playback_type)bs.read(2);
            bs.skip(14);
            bs.read_string(out bdj.name, 5);
            bs.skip(8);

            if (!Enum.IsDefined(bdj.playback_type))
            {
                Utils.BD_DEBUG(LogLevel.Critical, module, $"index.bdmv: invalid BD-J playback type {bdj.playback_type}");
            }

            return true;
        }

        private static bool _parse_playback_obj(BITSTREAM bs, INDX_PLAY_ITEM obj) {
            obj.object_type = (indx_object_type)bs.read(2);
            bs.skip(30);

            switch(obj.object_type)
            {
                case indx_object_type.hdmv:
                    return _parse_hdmv_obj(bs, obj.hdmv);
                case indx_object_type.bdj:
                    return _parse_bdj_obj(bs, obj.bdj);
            }

            Utils.BD_DEBUG(LogLevel.Critical, module, $"index.bdmv: unknown object type {obj.object_type}");
            return false;
        }

        private static bool _parse_index(BITSTREAM bs, INDX_ROOT index) {
            UInt32 index_len, i;

            index_len = bs.read(32);

            // TODO check if goes to extension data area

            if((bs.end() - bs.pos()) / 8 < (Int64)index_len)
            {
                Utils.BD_DEBUG(LogLevel.Critical, module, $"index.bdmv: invalid index_len {index_len}!");
                return false;
            }

            if (!_parse_playback_obj(bs, index.first_play)
                || !_parse_playback_obj(bs, index.top_menu))
            {
                return false;
            }

            index.num_titles = (UInt16)bs.read(16);
            if (index.num_titles == 0)
            {
                // no "normal" titles - check for first play and top menu
                if ((index.first_play.object_type == indx_object_type.hdmv && index.first_play.hdmv.id_ref == 0xFFFF) 
                    && (index.top_menu.object_type == indx_object_type.hdmv && index.top_menu.hdmv.id_ref == 0xFFFF))
                {
                    Utils.BD_DEBUG(LogLevel.Critical, "", "empty index");
                    return false;
                }
                return true;
            }

            index.titles = new INDX_TITLE[index.num_titles];
            if (bs.avail() / (12 * 8) < index.num_titles)
            {
                Utils.BD_DEBUG(LogLevel.Critical, module2, "index.bdmv: unexpected EOF");
                return false;
            }

            for(i = 0; i < index.num_titles; i++)
            {
                index.titles[i] = new();
                index.titles[i].object_type = (indx_object_type)bs.read(2);
                index.titles[i].access_type = (indx_access_type)bs.read(2);
                bs.skip(28);

                switch(index.titles[i].object_type)
                {
                    case indx_object_type.hdmv:
                        if (!_parse_hdmv_obj(bs, index.titles[i].hdmv))
                        {
                            return false;
                        }
                        break;
                    case indx_object_type.bdj:
                        if (!_parse_bdj_obj(bs, index.titles[i].bdj))
                        {
                            return false;
                        }
                        break;
                    default:
                        Utils.BD_DEBUG(LogLevel.Critical, module, $"index.bdmv: unknown object type {index.titles[i].object_type} (#{i})");
                        return false;
                }
            }

            return true;
        }

        private static bool _parse_app_info(BITSTREAM bs, INDX_APP_INFO app_info) {
            UInt32 len;

            if (bs.seek_byte(40) < 0)
            {
                return false;
            }

            len = bs.read(32);

            if (len != 34)
            {
                Utils.BD_DEBUG(LogLevel.Warning, module, $"index.bdmv app_info length is {len}, expected 34 !");
            }

            bs.skip(1);
            app_info.initial_output_mode_preference = bs.read_bool();
            app_info.content_exist_flag = bs.read_bool();
            bs.skip(1);
            app_info.initial_dynamic_range_type = bs.read(4);
            app_info.video_format = bs.read(4);
            app_info.frame_rate = bs.read(4);

            bs.read_bytes(app_info.user_data);

            return true;
        }

        const string SIG1 = "INDX";

        private static bool _parse_header(BITSTREAM bs, out UInt32 index_start, out UInt32 extension_data_start, out string indx_version) {
            if (!bdmv.parse_header(bs, SIG1, out indx_version))
            {
                index_start = 0;
                extension_data_start = 0;
                return false;
            }

            index_start = bs.read(32);
            extension_data_start = bs.read(32);

            return true;
        }

        private static bool _parse_indx_extension_hevc(BITSTREAM bs, INDX_ROOT index) {
            UInt32 len;
            UInt32 unk0, unk1, unk2, unk3, unk4, unk5;

            len = bs.read(32);
            if (len < 8)
            {
                Utils.BD_DEBUG(LogLevel.Critical, module, $"index.bdmv: unsupported extension 3.1 length ({len})");
                return false;
            }

            index.disc_type = (byte)bs.read(4);
            unk0 = bs.read(3);
            index.exist_4k_flag = bs.read_bool();
            unk1 = bs.read(8);
            unk2 = bs.read(3);
            index.hdrplus_flag = bs.read_bool();
            unk3 = bs.read(1);
            index.dv_flag = bs.read_bool();
            index.hdr_flags = (byte)bs.read(2);
            unk4 = bs.read(8);
            unk5 = bs.read(32);

            Utils.BD_DEBUG(module, $"UHD disc type: {index.disc_type}, 4k: {index.exist_4k_flag}, HDR: {index.hdr_flags}, HDR10+: {index.hdrplus_flag}, Dolby Vision: {index.dv_flag}");

            if (unk0 != 0 || unk1 != 0 || unk2 != 0 || unk3 != 0 || unk4 != 0 || unk5 != 0)
            {
                Utils.BD_DEBUG(LogLevel.Critical, module, $"index.bdmv: unknown data in extension 3.1: 0x{unk0:X2} 0x{unk1:X2} 0x{unk2:X2} 0x{unk3:X2} 0x{unk4:X2} 0x{unk5:X2}");
            }

            return true;
        }

        private static bool _parse_indx_extension(BITSTREAM bits, Int32 id1, Int32 id2, object handle) {
            INDX_ROOT index = (INDX_ROOT)handle;

            if (id1 == 3)
            {
                if (id2 == 1)
                {
                    return _parse_indx_extension_hevc(bits, index);
                }
            }

            Utils.BD_DEBUG(LogLevel.Critical, module, $"_parse_indx_extension(): unknown extension {id1}.{id2}");

            return false;
        }

        private static INDX_ROOT? _indx_parse(BD_FILE_H fp) {
            BITSTREAM bs = new();
            INDX_ROOT? index = null;
            UInt32 indexes_start, extension_data_start;

            if (bs.init(fp) < 0)
            {
                Utils.BD_DEBUG(module, "index.bdmv: read error");
                return null;
            }

            index = new INDX_ROOT();
            if (!_parse_header(bs, out indexes_start, out extension_data_start, out index.indx_version)
                || _parse_app_info(bs, index.app_info) 
             ){
                return null;
            }

            if (bs.seek_byte(indexes_start) < 0)
            {
                return null;
            }

            if (!_parse_index(bs, index))
            {
                return null;
            }

            if (extension_data_start > 0)
            {
                bdmv.parse_extension_data(bs, (int)extension_data_start, _parse_indx_extension, index);
            }

            return index;
        }

        private static INDX_ROOT? _indx_get(BD_DISC disc, string path) {
            BD_FILE_H? fp = null;
            INDX_ROOT? index = null;

            fp = disc.open_path(path);
            if (fp == null)
            {
                return null;
            }

            index = _indx_parse(fp);
            fp.close();
            return index;
        }

        /// <summary>
        /// Parse index.bdmv
        /// </summary>
        public static INDX_ROOT? get(BD_DISC disc) {
            INDX_ROOT? index = _indx_get(disc, Path.Combine("BDMV", "index.bdmv"));
            if (index == null)
            {
                // try backup
                index = _indx_get(disc, Path.Combine("BDMV", "BACKUP", "index.bdmv"));
            }

            return index;
        }
    }
}
