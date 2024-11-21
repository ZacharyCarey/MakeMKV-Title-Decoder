using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libbluray.file {
    internal class dir_win32 : BD_DIR_H {

        string path;
        string[] entries;
        int index = 0;

        private dir_win32(string path) {
            this.path = path;
            this.entries = Directory.GetFileSystemEntries(path);
        }

        /// <summary>
        /// Close directory stream
        /// </summary>
        public void close() {

        }

        /// <summary>
        /// Read next directory entry
        /// </summary>
        /// <param name="entry">BD_DIRENT where to store directory entry data</param>
        /// <returns>0 on success, 1 on EOF, <0 on error</returns>
        public bool read(out string? entry) {
            if (index < entries.Length)
            {
                entry = entries[index];
                index++;
                return true;
            } else
            {
                entry = null;
                return false;
            }
        }

        /// <summary>
        /// Prototype for a function that returns BD_DIR_H implementation.
        /// </summary>
        /// <param name="dirname">name of the directory to open</param>
        /// <returns>BD_DIR_H object, NULL on error</returns>
        public static BD_DIR_H? Open(string dirname) {
            if (Directory.Exists(dirname))
            {
                return new dir_win32(dirname);
            } else
            {
                return null;
            }
        }

    }
}
