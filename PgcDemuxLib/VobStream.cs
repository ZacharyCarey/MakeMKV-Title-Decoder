using PgcDemuxLib.Data.VTS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PgcDemuxLib
{
    // NOTE!! Title VOBs (which is what this is used for) start at index "1". i.e. only VTS_xx_1.VOB through VTS_xx_9.VOB
    // are used, and VTS_xx_1.VOB contains position=0
    internal class VobStream : Stream
    {
        int currentVBO;
        FileStream? currentFile;
        VtsIfo ifo;
        long totalLength;
        long position = 0;

        public VobStream(VtsIfo ifo)
        {
            this.ifo = ifo;
            this.totalLength = ifo.VobSize.Skip(1).Sum();
            OpenVOB(1);
        }

        protected override void Dispose(bool disposing)
        {
            currentFile?.Dispose();
        }

        public override bool CanRead => currentFile?.CanRead ?? false;

        public override bool CanSeek => currentFile?.CanSeek ?? false;

        public override bool CanWrite => false;

        public override long Length => totalLength;

        public override long Position
        {
            get => position;
            set => Seek(value, SeekOrigin.Begin);
        }

        public override void Flush()
        {
            currentFile?.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int totalRead = 0;
            while (count > 0 && currentFile != null)
            {
                int read = currentFile.Read(buffer, offset, count);
                if (read < count)
                {
                    OpenNextVOB();
                }
                offset += read;
                count -= read;
                totalRead += read;
            }

            position += totalRead;
            return totalRead;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            if (origin == SeekOrigin.Current)
            {
                offset = position + offset;
            }
            else if (origin == SeekOrigin.End)
            {
                offset = this.Length + offset; // offset will be negative here
            }

            long pos = offset;
            for (int i = 1; i < 10; i++)
            {
                long size = ifo.VobSize[i];
                if (pos < size)
                {
                    OpenVOB(i);
                    currentFile?.Seek(pos, SeekOrigin.Begin);
                    position = offset;
                    return offset;
                }
                else
                {
                    pos -= size;
                }
            }

            position = this.Length;
            currentFile = null;
            return 0;
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Will find the next valid VOB and attempt to open it
        /// </summary>
        private void OpenNextVOB()
        {
            if (currentVBO > 9)
            {
                return;
            }

            for (int i = currentVBO; i <= 10; i++)
            {
                // NOTE: when i=10 we are only calling OpenVOB to set the internal state. VOB=10 will never exist
                OpenVOB(i);
                if (currentFile != null)
                {
                    return;
                }
            }
        }

        private void OpenVOB(int id)
        {
            if (currentFile != null)
            {
                currentFile.Close();
                currentFile.Dispose();
            }

            currentVBO = id;
            if (currentVBO < 1 || currentVBO > 9)
            {
                currentFile = null;
                return;
            }

            if (ifo.VobSize[currentVBO] <= 0)
            {
                currentFile = null;
                return;
            }

            try
            {
                currentFile = File.OpenRead(GetVobPath(currentVBO));
            }
            catch (Exception ex)
            {
                throw new IOException($"Failed to open next VOB file.", ex);
            }
        }

        private string GetVobPath(int id)
        {
            return Path.Combine(ifo.ParentFolder, $"VTS_{ifo.TitleSet:00}_{id:0}.VOB");
        }
    }
}
