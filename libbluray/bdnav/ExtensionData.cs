using libbluray.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libbluray.bdnav
{
    public abstract class ExtensionData
    {

        private const string module = "libbluray.bdnav.ExtensionData";

        public bool Parse(BitStream bits, long start_address)
        {
            int n;

            bits.SeekByte(start_address);
            if (bits.EOF)
            {
                return false;
            }

            long length = bits.Read<uint>(); // length of extension data block
            if (length <= 0) return false;
            bits.Skip(32); // relative start address of extension data
            bits.Skip(24); // padding
            int num_entries = bits.Read<byte>();

            for (n = 0; n < num_entries; n++)
            {
                ushort id1 = bits.Read<ushort>(); // ext data type
                ushort id2 = bits.Read<ushort>(); // ext data version
                long ext_start = bits.Read<uint>();
                long ext_len = bits.Read<uint>();

                long saved_pos = bits.Position;

                bits.SeekByte(start_address + ext_start);
                if (!ParseExtensionData(bits, id1, id2))
                {
                    Utils.BD_DEBUG(LogLevel.Warning, module, $"Failed to parse extension data: {n}");
                }
                bits.Seek(saved_pos);
            }

            return true;
        }

        protected abstract bool ParseExtensionData(BitStream bits, ushort id1, ushort id2);
    }
}
