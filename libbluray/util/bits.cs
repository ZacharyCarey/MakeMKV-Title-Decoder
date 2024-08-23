using libbluray.file;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libbluray.util {

    public class BITBUFFER {
        private static readonly UInt32[] i_mask = new UInt32[33] {
            0x00,
            0x01,      0x03,      0x07,      0x0f,
            0x1f,      0x3f,      0x7f,      0xff,
            0x1ff,     0x3ff,     0x7ff,     0xfff,
            0x1fff,    0x3fff,    0x7fff,    0xffff,
            0x1ffff,   0x3ffff,   0x7ffff,   0xfffff,
            0x1fffff,  0x3fffff,  0x7fffff,  0xffffff,
            0x1ffffff, 0x3ffffff, 0x7ffffff, 0xfffffff,
            0x1fffffff,0x3fffffff,0x7fffffff,0xffffffff
        };

        private byte[] source = null;

        internal int p_start;
        internal int p;
        internal int p_end;

        public byte Current => source[p];
        public Span<byte> AsSpan => source.AsSpan(p_start, p_end - p_start);

        /// <summary>
        /// i_count number of available bits
        /// </summary>
        internal int i_left;

        public BITBUFFER() { }
        public BITBUFFER(BITBUFFER other_copy) {
            this.source = other_copy.source;
            this.p_start = other_copy.p_start;
            this.p = other_copy.p;
            this.p_end = other_copy.p_end;
            this.i_left = other_copy.i_left;
        }

        internal void init(byte[] p_data, int startIndex, UInt64 i_data) {
            this.source = p_data;
            this.p_start = startIndex;
            this.p = this.p_start;
            this.p_end = this.p_start + (int)i_data;
            this.i_left = 8;
        }

        internal void init(byte[] p_data, UInt64 i_data) {
            init(p_data, 0, i_data);
        }

        internal void skip(UInt64 i_count) {
            this.p += (int)i_count >> 3;
            this.i_left -= (int)(i_count & 0x07);

            if  (this.i_left <= 0)
            {
                this.p++;
                this.i_left += 8;
            }
        }

        internal UInt32 read(Int32 i_count) {
            int i_shr;
            UInt32 i_result = 0;

            while (i_count > 0)
            {
                if (this.p >= this.p_end)
                {
                    break;
                }

                i_shr = this.i_left - i_count;
                if (i_shr >= 0)
                {
                    // More in the buffer than requested
                    i_result |= (uint)(this.Current >> i_shr) & i_mask[i_count];
                    this.i_left -= i_count;
                    if (this.i_left == 0)
                    {
                        this.p++;
                        this.i_left = 8;
                    }
                    return i_result;
                } else
                {
                    // less in the buffer than requested
                    i_result |= (this.Current & i_mask[this.i_left]) << -i_shr;
                    i_count -= this.i_left;
                    this.p++;
                    this.i_left = 8;
                }
            }

            return i_result;
        }

        internal bool read_bool() {
            UInt32 value = read(1);
            return value != 0;
        }

        public Int64 pos() {
            return 8 * (p - p_start) + 8 - i_left;
        }
        public bool eof() {
            return this.p >= this.p_end;
        }
        public void read_bytes(Span<byte> buf, Int32 i_count) {
            for(int ii = 0; ii < i_count; ii++)
            {
                buf[ii] = (byte)read(8);
            }
        }

        public UInt32 show(int i_count) {
            BITBUFFER temp = new(this);
            return temp.read(i_count);
        }

        public bool is_align(UInt32 mask) {
            Int64 off = pos();
            return (off & mask) == 0;
        }
    }

    public class BITSTREAM {
        const string module = "DBG_FILE";
        const int BF_BUF_SIZE = 1024 * 32;

        BD_FILE_H fp = null;
        byte[] buf = new byte[BF_BUF_SIZE];
        BITBUFFER bb = new();

        /// <summary>
        /// file offset of buffer start (buf[0])
        /// </summary>
        Int64 _pos;

        /// <summary>
        /// size of file
        /// </summary>
        Int64 _end;
        
        /// <summary>
        /// bytes in buf
        /// </summary>
        UInt64 size;

        private Int32 _read() {
            int result = 0;
            Int64 got;

            got = this.fp.read(this.buf.AsSpan());
            if (got  <= 0 || got >= BF_BUF_SIZE)
            {
                Utils.BD_DEBUG(module, "_bs_read(): read error");
                got = 0;
                result = -1;
            }

            this.size = (UInt64)got;
            this.bb.init(this.buf, this.size);

            return result;
        }

        private Int32 _read_at(Int64 off) {
            if (this.fp.seek(off, SeekOrigin.Begin) < 0)
            {
                Utils.BD_DEBUG(LogLevel.Critical, module, "bs_read(): seek failed");
                // No change in state. Call _must_ check return value
                return -1;
            }
            this._pos = off;
            return _read();
        }

        internal Int32 init(BD_FILE_H fp) {
            Int64 size = fp.size();
            this.fp = fp;
            this._pos = 0;
            this._end = (size < 0) ? 0 : size;

            return _read();
        }

        private Int32 _seek(Int64 off, SeekOrigin whence) {
            int result = 0;
            Int64 b;

            switch(whence)
            {
                case SeekOrigin.Current:
                    off = this._pos * 8 + (this.bb.p - this.bb.p_start) * 8 + off;
                    break;
                case SeekOrigin.End:
                    off = this._end * 8 - off;
                    break;
                case SeekOrigin.Begin:
                default:
                    break;
            }
            if (off < 0)
            {
                Utils.BD_DEBUG(LogLevel.Critical, module, "bs_seek(): seek failed (negative offset)");
                return -1;
            }

            b = off >> 3;
            if (b >= this._end)
            {
                Int64 pos;
                if (this._end > BF_BUF_SIZE)
                {
                    pos = this._end - BF_BUF_SIZE;
                } else
                {
                    pos = 0;
                }
                result = _read_at(pos);
                this.bb.p = this.bb.p_end;
            } else if (b < this._pos || b >= (this._pos + BF_BUF_SIZE))
            {
                result = _read_at(b);
            } else
            {
                b -= this._pos;
                this.bb.p = this.bb.p_start + (int)b;
                this.bb.i_left = 8 - (int)(off & 0x07);
            }

            return result;
        }

        internal Int32 seek_byte(Int64 off) {
            return _seek(off << 3, SeekOrigin.Begin);
        }

        internal void skip(UInt64 i_count) {
            int left;
            UInt64 bytes = (i_count + 7) >> 3;

            if ((UInt64)this.bb.p + bytes >= (UInt64)this.bb.p_end)
            {
                this._pos = this._pos + (this.bb.p - this.bb.p_start);
                left = this.bb.i_left;
                this.fp.seek(this._pos, SeekOrigin.Begin);
                this.size = (ulong)this.fp.read(this.buf.AsSpan());
                this.bb.init(this.buf, this.size);
                this.bb.i_left = left;
            }
            this.bb.skip(i_count);
        }

        internal UInt32 read(int i_count) {
            int left;
            int bytes = (i_count + 7) >> 3;

            if (this.bb.p + bytes >= this.bb.p_end)
            {
                this._pos = this._pos + (this.bb.p - this.bb.p_start);
                left = this.bb.i_left;
                this.fp.seek(this._pos, SeekOrigin.Begin);
                this.size = (ulong)this.fp.read(this.buf.AsSpan());
                this.bb.init(this.buf, this.size);
                this.bb.i_left = left;
            }
            return this.bb.read(i_count);
        }

        internal bool read_bool() {
            UInt32 value = read(1);
            return value != 0;
        }

        public Int64 pos() {
            return this._pos * 8 + bb.pos();
        }

        public Int64 end() {
            return this._end * 8;
        }

        public Int64 avail() {
            return end() - pos();
        }

        public void read_bytes(Span<byte> buf) {
            for (int ii = 0; ii < buf.Length; ii++)
            {
                buf[ii] = (byte)read(8);
            }
        }

        public void read_string(out string str, Int32 i_count) {
            byte[] buf = new byte[i_count];
            read_bytes(buf);
            str = Encoding.ASCII.GetString(buf);
        }

        public bool is_align(UInt32 mask) {
            Int64 off = pos();
            return (off & mask) == 0;
        }
    }
}
