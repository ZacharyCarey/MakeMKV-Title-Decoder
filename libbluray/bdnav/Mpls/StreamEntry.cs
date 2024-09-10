using libbluray.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libbluray.bdnav.Mpls {
    public class StreamEntry {
        const string module = "libbluray.bdnav.Mpls.StreamEntry";

        public byte stream_type;

        /// <summary>
        /// Contains the PID of this stream inside of the 'MPEG-2 Transport Stream' 
        /// corresponding to the clip referenced by the PlayItem which this stream belongs 
        /// to or referenced by the 'SubPlayItem' specified above (subclip_id?)
        /// </summary>
        public UInt16 PID;

        /// <summary>
        /// Contains the index of a 'SubPath' inside the 'PlayList' to which this stream belongs to
        /// </summary>
        public byte subpath_id;

        /// <summary>
        /// Contains the index of a 'SubPlayItem?' inside the 'SubPath' referenced above (subpath_id)
        /// </summary>
        public byte subclip_id;

        public StreamAttributes Attributes = new();

        public bool Parse(BitStream bits) {
            if (!bits.IsAligned())
            {
                Utils.BD_DEBUG(LogLevel.Warning, module, "_parse_stream: Stream alignment error");
            }

            Int32 len = bits.Read<byte>();
            Int64 pos = bits.Position;

            this.stream_type = bits.Read<byte>();
            switch (this.stream_type)
            {
                case 1:
                    // a stream of the clip used by the PlayItem
                    this.PID = bits.Read<UInt16>();
                    break;
                case 2:
                    // a stream of the clip used by a SubPath (SubPathType set to 2, 3, 4, 5, 6, 8 or 9),
                    this.subpath_id = bits.Read<byte>();
                    this.subclip_id = bits.Read<byte>();
                    this.PID = bits.Read<UInt16>();
                    break;
                case 3:
                case 4:
                    // a stream of the clip used by a SubPath (SubPathType set to 7/10),
                    this.subpath_id = bits.Read<byte>();
                    this.PID = bits.Read<UInt16>();
                    break;
                default:
                    Utils.BD_DEBUG(LogLevel.Critical, module, $"unrecognized stream type {this.stream_type:X2}");
                    break;
            }

            bits.Seek(pos + (len * 8));
            if (bits.EOF)
            {
                return false;
            }

            return this.Attributes.Parse(bits);
        }
    }
}
