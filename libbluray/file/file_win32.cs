using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace libbluray.file {
    public class file_win32 : BD_FILE_H {

        const string module = "DBG_FILE";
        private FileStream file;

        private file_win32(FileStream stream) {
            this.file = stream;
        }

        public static BD_FILE_H? OpenFile(string filename, FileMode mode) {
            FileStream? stream = null;
            try
            {
                stream = File.Open(filename, mode);

                Utils.BD_DEBUG(module, $"Opened WIN32 file \"{filename}\"");
                return new file_win32(stream);
            }catch(Exception ex)
            {
                Utils.BD_DEBUG(LogLevel.Critical, module, $"Error opening file {filename} ({ex.Message})");
                stream?.Close();
                return null;
            }
        }

        public void close() {
            try
            {
                file.Flush();
                file.Close();
                file.Dispose();
            } catch (Exception ex)
            {
                Utils.BD_DEBUG(LogLevel.Critical, module, $"Error closing WIN32 file {file.Name}");
            } finally
            {
                Utils.BD_DEBUG(module, $"Closed WIN32 file {file.Name}");
                this.file = null;
            }
        }

        public int eof() {
            throw new NotImplementedException();
        }

        public long read(Span<byte> buf) {
            if (buf.Length > 0)
            {
                return file.Read(buf);
            }

            Utils.BD_DEBUG(LogLevel.Critical, module, $"Ignoring invalid read of size {buf.Length} ({file.Name})");
            return 0;
        }

        public long seek(Int64 offset, SeekOrigin origin) {
            try
            {
                return file.Seek(offset, origin);
            } catch(Exception ex)
            {
                Utils.BD_DEBUG(LogLevel.Warning, module, $"Failed to seek file {file.Name}");
                return -1;
            }
        }

        public long tell() {
            try
            {
                return file.Position;
            }catch(Exception ex)
            {
                Utils.BD_DEBUG(LogLevel.Warning, module, $"Failed to get file position ({file.Name})");
                return -1;
            }
        }

        public bool flush() {
            try
            {
                file.Flush();
                return true;
            } catch (Exception ex)
            {
                Utils.BD_DEBUG(LogLevel.Warning, module, $"fflush() failed ({file.Name})");
                return false;
            }
        }

        public long write(ReadOnlySpan<byte> buf) {
            if (buf.Length > 0)
            {
                try
                {
                    file.Write(buf);
                    return buf.Length;
                } catch(Exception ex)
                {
                    Utils.BD_DEBUG(LogLevel.Warning, module, $"Failed to write to file \"{file.Name}\": {ex}");
                    return -1;
                }
            }

            if (buf.Length == 0)
            {
                return flush() ? 0 : -1;
            }

            Utils.BD_DEBUG(LogLevel.Critical, module, $"Ignoring invalid write of size {buf.Length} ({file.Name})");
            return 0;
        }

        public Int64 size() {
            try
            {
                return file.Length;
            }catch(Exception ex)
            {
                Utils.BD_DEBUG(LogLevel.Warning, module, $"Failed to read file length \"{file.Name}\": {ex.Message}");
                return -1;
            }
        }
    }
}
