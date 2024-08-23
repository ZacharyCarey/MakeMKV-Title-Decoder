using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libbluray.file {

    public interface BD_DIR_H {

        /// <summary>
        /// Close directory stream
        /// </summary>
        public void close();

        /// <summary>
        /// Read next directory entry
        /// </summary>
        /// <param name="entry">BD_DIRENT where to store directory entry data</param>
        /// <returns>0 on success, 1 on EOF, <0 on error</returns>
        public bool read(out string? entry);

        /// <summary>
        /// Prototype for a function that returns BD_DIR_H implementation.
        /// </summary>
        /// <param name="dirname">name of the directory to open</param>
        /// <returns>BD_DIR_H object, NULL on error</returns>
        public static abstract BD_DIR_H? Open(string dirname);
    }

    public interface BD_FILE_H {

        public void close();

        /// <summary>
        ///  Reposition file offset
        ///
        ///  - SEEK_SET: seek to 'offset' bytes from file start
        ///  - SEEK_CUR: seek 'offset' bytes from current position
        ///  - SEEK_END: seek 'offset' bytes from file end
        /// </summary>
        /// <param name="offset">byte offset</param>
        /// <param name="origin">SEEK_SET, SEEK_CUR or SEEK_END</param>
        /// <returns>current file offset, < 0 on error</returns>
        public Int64 seek(Int64 offset, SeekOrigin origin);

        /// <summary>
        /// Get current read or write position
        /// </summary>
        /// <returns>current file offset, < 0 on error</returns>
        public Int64 tell();

        /// <summary>
        /// Check for end of file
        /// - optional, currently not used
        /// </summary>
        /// <returns>1 on EOF, < 0 on error, 0 if not EOF</returns>
        public Int32 eof();

        /// <summary>
        /// Read from file
        /// </summary>
        /// <param name="buf">buffer where to store the data</param>
        /// <returns>number of bytes read, 0 on EOF, < 0 on error</returns>
        public Int64 read(Span<byte> buf);

        /// <summary>
        /// Write to file
        /// Writing 0 bytes can be used to flush previous writes and check for errors.
        /// </summary>
        /// <param name="buf">data to be written</param>
        /// <returns>number of bytes written, < 0 on error</returns>
        public Int64 write(ReadOnlySpan<byte> buf);

        public bool flush();

        public Int64 size();

        /// <summary>
        /// Prototype for a function that returns BD_FILE_H implementation.
        /// </summary>
        /// <param name="filename">name of the file to open</param>
        /// <param name="mode">string starting with "r" for reading or "w" for writing</param>
        /// <returns>BD_FILE_H object, NULL on error</returns>
        public static abstract BD_FILE_H? OpenFile(string filename, FileMode mode);
    }
}
