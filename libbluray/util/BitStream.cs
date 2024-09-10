using libbluray.file;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace libbluray.util {
    public class BitStream {

        private BufferedStream stream;
        private bool streamEOF;

        private byte current;
        private int bitsRemaining;

        public Int64 Position => (this.stream.Position * 8) - this.bitsRemaining;
        public bool EOF => streamEOF && (bitsRemaining == 0);

        public BitStream (Stream stream) {
            this.stream = new BufferedStream(stream);

            this.streamEOF = false;
            ReadNextByte();
        }

        public BitStream(BD_FILE_H file) : this(file.GetStream()) {

        }

        private void ReadNextByte() {
            int val = this.stream.ReadByte();
            if (val < 0)
            {
                this.streamEOF = true;
                this.current = 0;
                this.bitsRemaining = 0;
            } else
            {
                this.current = (byte)val;
                this.bitsRemaining = 8;
                this.streamEOF = false;
            }
        }

        public void Seek(Int64 offset, SeekOrigin origin = SeekOrigin.Begin) {
            Int64 position;
            switch (origin)
            {
                case SeekOrigin.Current:
                    position = this.Position + offset;
                    break;
                case SeekOrigin.Begin:
                    position = offset;
                    break;
                case SeekOrigin.End:
                    position = (stream.Length * 8) + offset;
                    break;
                default:
                    throw new Exception("Unknown origin type");
            }

            this.stream.Seek(position / 8, SeekOrigin.Begin);
            ReadNextByte();
            if (this.bitsRemaining > 0)
            {
                this.bitsRemaining = 8 - (int)(position % 8);
            }
        }

        public void SeekByte(Int64 position, SeekOrigin origin = SeekOrigin.Begin) {
            Seek(position * 8, origin);
        }

        public void Skip(Int64 bits) {
            Seek(bits, SeekOrigin.Current);
        }

        public void SkipBytes(Int64 bytes) {
            Skip(bytes * 8);
        }

        public T Read<T>(int bits) where T : IBinaryInteger<T> {
            T result = T.Zero;

            while (bits > 0)
            {
                if (bitsRemaining == 0)
                {
                    throw new IndexOutOfRangeException("Read past end of file");
                }

                // Will be max of 8
                int bitsToRead = Math.Min(bits, this.bitsRemaining);

                UInt32 mask = (1u << bitsToRead) - 1;
                int shr = this.bitsRemaining - bitsToRead;
                byte data = (byte)((this.current >> shr) & mask);
                result <<= bitsToRead;
                result |= (T)Convert.ChangeType(data, typeof(T));

                bits -= bitsToRead;
                this.bitsRemaining -= bitsToRead;
                if (this.bitsRemaining <= 0) // should never go below 0
                {
                    ReadNextByte();
                }
            }

            return result;
        }

        public T Read<T>() where T : IBinaryInteger<T> {
            T result = T.Zero;
            return this.Read<T>(result.GetByteCount() * 8);
        }

        public bool ReadBool() {
            byte value = Read<byte>(1);
            return value != 0;
        }

        public T ReadEnum<T>(int bits) where T : Enum {
            return (T)Convert.ChangeType(Read<UInt32>(bits), typeof(T));
        }

        public T ReadEnum<T>(int bits, T defaultValue, string? module = null, string? err_message = null) where T : Enum {
            UInt32 result = Read<UInt32>(bits);
            if (Enum.IsDefined(typeof(T), result))
            {
                return (T)Enum.ToObject(typeof(T), result); 
            } else
            {
                if (module != null)
                {
                    string? msg = err_message;
                    if (msg == null)
                    {
                        msg = $"Unknown value for enum '{nameof(T)}'";
                    }
                    Utils.BD_DEBUG(LogLevel.Warning, module, $"{msg}: {result:X2}");
                }
                return defaultValue;
            }
        }

        public void Read(Span<byte> buffer) {
            for(int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = Read<byte>();
            }
        }

        public string ReadString(int bytes) {
            byte[] buffer = new byte[bytes];
            Read(buffer);
            return Encoding.ASCII.GetString(buffer);
        }

        public Int64 Available() {
            return (stream.Length - stream.Position) * 8 + this.bitsRemaining;
        }

        public Int64 AvailableBytes() {
            return Available() / 8;
        }

        public bool IsAligned() {
            return this.bitsRemaining == 8;
        }
    }
}
