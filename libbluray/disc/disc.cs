using libbluray.bdnav;
using libbluray.bdnav.Clpi;
using libbluray.bdnav.Mpls;
using libbluray.file;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace libbluray.disc {

    public class BD_DISC {

        const string module = "DBG_FILE";

        /// <summary>
        /// disc filesystem root (if disc is mounted)
        /// </summary>
        public string DiscPath;

        private ClpiFile[] ClipFiles = Array.Empty<ClpiFile>();
        private META_ROOT? MetaFiles = null;
        //private PlayList[] PlaylistFiles = Array.Empty<PlayList>();


        public BD_DISC(string path) {
            this.DiscPath = path;
        }

        public static Task LoadBackupFolder<T>(string path, IProgress<T> progress) where T : TaskProgress, new() {
            // TODO
            //BD_DISC? disc = open(path);
            var file = file_win32.OpenFile(Path.Combine(path, "BDMV", "CLIPINF", "00339.clpi"), FileMode.Open);
            ClpiFile? clip = ClpiFile.Parse(file);
            if (clip != null)
            {
                Console.WriteLine(clip.type_indicator);
            }

            return Task.Delay(100);
        }

        public static BD_DISC? open(string device_path) {
            BD_DISC p = new(device_path);
            if (p == null)
            {
                return null;
            }

            // check if disc root directory can be opened. If not, treat it as device/image file.
            BD_DIR_H? dp_img = (device_path != null) ? dir_win32.Open(device_path) : null;
            if (dp_img == null)
            {
                
            } else
            {
                dp_img.close();
                Utils.BD_DEBUG(module, $"{device_path} does not seem to be image file or device node");
            }

            return p;
        }

        public string root() {
            return this.DiscPath;
        }

        private BD_DIR_H? _bdrom_open_dir(string dir) {
            BD_DIR_H? dp = null;
            string path = Path.Combine(this.DiscPath, dir);
            dp = dir_win32.Open(path);
            return dp;
        }

        /// <summary>
        /// Open VFS file (relative to disc root)
        /// </summary>
        internal BD_FILE_H? open_file(string dir, string file) {
            string path = Path.Combine(dir, file);
            BD_FILE_H? fp = open_path(path);
            return fp;
        }

        /// <summary>
        /// Open VFS file (relative to disc root)
        /// </summary>
        internal BD_FILE_H? open_path(string path) {
            BD_FILE_H? fp = null;

            return file_win32.OpenFile(Path.Combine(this.DiscPath, path), FileMode.Open);
        }

        /// <summary>
        /// Open VFS directory (relative to disc root)
        /// </summary>
        internal BD_DIR_H? open_dir(string dir) {
            BD_DIR_H? dp_rom = this._bdrom_open_dir(dir);
            BD_DIR_H? dp_ovl = null;// _overlay_open_dir(dir);

            if (dp_ovl == null)
            {
                if (dp_rom == null)
                {
                    Utils.BD_DEBUG(LogLevel.Warning, module, $"error opening dir {dir}");
                }
                return dp_rom;
            }
            if (dp_rom == null)
            {
                return dp_ovl;
            }

            return null; // _combine_dirs(dp_ovl, dp_rom);
        }

        /// <summary>
        /// Read VFS file
        /// </summary>
        internal UInt64 read_file(string dir, string file, out byte[] data) {
            BD_FILE_H? fp = null;
            Int64 size;

            data = null;

            if (dir != null)
            {
                fp = this.open_file(dir, file);
            } else
            {
                fp = this.open_path(file);
            }
            if (fp == null)
            {
                return 0;
            }

            size = fp.size();
            if (size > 0)
            {
                data = new byte[size];
                Int64 got = fp.read(data);
                if (got != size)
                {
                    Utils.BD_DEBUG(LogLevel.Critical, module, $"Error reading file {file} from {dir}");
                    data = Array.Empty<byte>();
                    size = 0;
                }
            } else
            {
                size = 0;
            }

            fp.close();
            return (UInt64)size;
        }
    }
}
