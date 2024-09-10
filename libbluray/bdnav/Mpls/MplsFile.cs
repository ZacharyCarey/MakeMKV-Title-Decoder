using libbluray.disc;
using libbluray.file;
using libbluray.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libbluray.bdnav.Mpls
{
    public class MplsFile {

        const string module = "libbluray.bdnav.Mpls.MplsFile";
        const string SIG1 = "MPLS";

        /// <summary>
        /// 'MPLS'
        /// </summary>
        public string type_indicator = SIG1;

        /// <summary>
        /// version
        /// </summary>
        public string type_indicator2;
        public UInt32 list_pos;
        public UInt32 mark_pos;
        public UInt32 ext_pos;

        public AppInfo app_info = new();
        public PlayList playlist = new();

        public PlayListMark[] play_mark = Array.Empty<PlayListMark>();

        public PlaylistExtension extension = new();

        public bool Parse(BitStream bits) {
            if (!bdmv.parse_header(bits, this.type_indicator, out this.type_indicator2))
            {
                return false;
            }

            this.list_pos = bits.Read<UInt32>();
            this.mark_pos = bits.Read<UInt32>();
            this.ext_pos = bits.Read<UInt32>();

            // Skip 160 reserved bits
            bits.Skip(160);

            this.app_info.Parse(bits);

            bits.SeekByte(this.list_pos);
            if (!this.playlist.Parse(bits))
            {
                return false;
            }

            bits.SeekByte(this.mark_pos);
            if (!PlayListMark.Parse(bits, out this.play_mark))//_parse_playlistmark(bits, pl)
            {
                return false;
            }

            if (!extension.Parse(bits, ext_pos))
            {
                Utils.BD_DEBUG(LogLevel.Warning, module, "Failed to parse Mpls extension data");
            }
            
            return true;
        }

        public static MplsFile? Parse(BD_FILE_H file) {
            BitStream bits = new(file);
            MplsFile mpls = new();
            if (!mpls.Parse(bits))
            {
                return null;
            }
            return mpls;
        }

        public static MplsFile? Parse(string path) {
            BD_FILE_H? file = file_win32.OpenFile(path, FileMode.Open);
            if (file == null)
            {
                Utils.BD_DEBUG(LogLevel.Critical, module, $"Failed to open {path}");
                return null;
            }

            MplsFile? result = Parse(file);

            file.close();

            return result;
        }

        public static MplsFile? Get(BD_DISC disc, string filename) {
            MplsFile? result = null;
            BD_FILE_H? file = disc.open_file(Path.Combine("BDMV", "PLAYLIST"), filename);
            if (file != null)
            {
                result = Parse(file);
            }

            // if failed, try backup file
            if (result == null)
            {
                file = disc.open_file(Path.Combine("BDMV", "BACKUP", "PLAYLIST"), filename);
                if (file != null)
                {
                    result = Parse(file);
                }
            }

            return result;
        }
    }
}
