using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.Util {

    public struct BitIndex {
        public int Byte;
        public int Bit;

        public BitIndex() {
            Byte = 0;
            Bit = 0;
        }

        public BitIndex(int Byte, int Bit = 0) {
            this.Byte = Byte;
            this.Bit = Bit;
        }
    }

    public abstract class ByteParser {

        public string FilePath;
        protected byte[] Bytes;

        public BitIndex Index { get; private set; } = new();

        public ByteParser(string path) {
            this.FilePath = path;
            this.Bytes = File.ReadAllBytes(path);
        }

        protected string ReadString(int length) {
            Debug.Assert(Index.Bit == 0, "Must be byte aligned to read string");
            string result = Encoding.ASCII.GetString(Bytes.AsSpan(Index.Byte, length));
            Skip(length);
            return result;
        }

        protected T Read<T>(int bits) where T : struct, IBinaryInteger<T>, IShiftOperators<uint, int, uint>, IBitwiseOperators<uint, uint, uint>, ISubtractionOperators<uint, uint, uint> {
            if (bits == 0) return default;
            T result = T.AllBitsSet;
            Debug.Assert(bits <= result.GetByteCount() * 8);

            result = T.Zero;

            int bitsUntilAligned = (8 - Index.Bit) % 8;
            T mask;
            if (bits >= bitsUntilAligned)
            {
                // Align with bytes
                mask = (T.One << bitsUntilAligned) - T.One;
                result = ((T)Convert.ChangeType(Bytes[Index.Byte], typeof(T)) >> (8 - Index.Bit - bitsUntilAligned)) & mask;
                bits -= bitsUntilAligned;
                Skip(0, bitsUntilAligned);
                Debug.Assert(this.Index.Bit == 0, "This should not happen.");

                while (bits >= 8)
                {
                    result <<= 8;
                    result |= (T)Convert.ChangeType(Bytes[Index.Byte], typeof(T));
                    Skip(1);
                    bits -= 8;
                }
            }

            mask = (T.One << bits) - T.One;
            result <<= bits;
            result |= ((T)Convert.ChangeType(Bytes[Index.Byte], typeof(T)) >> (8 - Index.Bit - bits)) & mask;
            Skip(0, bits);

            return result;
        }

        protected byte ReadUInt8() {
            Debug.Assert(Index.Bit == 0, "Must be byte aligned to read uint32");
            byte result = Bytes[Index.Byte];
            Skip(1);
            return result;
        }

        protected UInt32 ReadUInt16() {
            Debug.Assert(Index.Bit == 0, "Must be byte aligned to read uint32");
            uint result = BinaryPrimitives.ReadUInt16BigEndian(Bytes.AsSpan(Index.Byte, 2));
            Skip(2);
            return result;
        }

        protected UInt32 ReadUInt32() {
            Debug.Assert(Index.Bit == 0, "Must be byte aligned to read uint32");
            uint result = BinaryPrimitives.ReadUInt32BigEndian(Bytes.AsSpan(Index.Byte, 4));
            Skip(4);
            return result;
        }

        protected bool ReadBool() {
            bool result = ((Bytes[Index.Byte] >> (7 - Index.Bit)) & 0x01) != 0;
            Skip(0, 1);
            return result;
        }

        protected void Seek(int pos, int bit = 0) {
            this.Index = new(pos, bit);
        }

        protected void Seek(BitIndex index) {
            this.Index = index;
        }

        protected void Skip(int bytes, int bits = 0) {
            BitIndex result = this.Index;

            result.Byte += bytes + (bits / 8);
            result.Bit += bits % 8;
            if (result.Bit >= 8)
            {
                result.Byte++;
                result.Bit -= 8;
            }

            this.Index = result;
        }

    }
}
