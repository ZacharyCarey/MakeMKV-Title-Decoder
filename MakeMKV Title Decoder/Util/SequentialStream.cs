using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.Util {
    public class SequentialStream : Stream {
        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => -1;

        private long ByteCount = 0;
        public override long Position { get => this.ByteCount; set => new NotSupportedException(); }

        private IEnumerator<string> Input;
        private string Buffer = "";
        private int RemainingChars = 0;

        public SequentialStream(IEnumerable<string> input) {
            this.Input = input.GetEnumerator();
            if (this.Input.MoveNext())
            {
                this.Buffer = this.Input.Current;
                this.RemainingChars = this.Buffer.Length;
            }
        }

        public override void Flush() {

        }

        public override int Read(byte[] buffer, int offset, int count) {
            int bytesRead = 0;
            for(int index = offset; count > 0; index++, count--, bytesRead++)
            {
                if (this.RemainingChars == 0)
                {
                    break;
                }

                buffer[index] = (byte)this.Buffer[this.Buffer.Length - this.RemainingChars];
                this.RemainingChars--;
                if (this.RemainingChars == 0)
                {
                    if (this.Input.MoveNext())
                    {
                        this.Buffer = Environment.NewLine + this.Input.Current;
                        this.RemainingChars = this.Buffer.Length;
                    } else
                    {
                        this.Buffer = "";
                        this.RemainingChars = 0;
                    }
                }
            }

            this.ByteCount += bytesRead;

            return bytesRead;
        }

        public override long Seek(long offset, SeekOrigin origin) {
            throw new NotSupportedException();
        }

        public override void SetLength(long value) {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count) {
            throw new NotSupportedException();
        }
    }
}
