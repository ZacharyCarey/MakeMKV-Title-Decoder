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

    
}
