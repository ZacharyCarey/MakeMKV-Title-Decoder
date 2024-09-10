using libbluray.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libbluray.bdnav.Mpls {
    public class SubPlayItem {
        const string module = "libbluray.bdnav.Mpls.SubPlayItem";

        public byte connection_condition;
        public bool is_multi_clip;
        public UInt32 in_time;
        public UInt32 out_time;
        public UInt16 sync_play_item_id;
        public UInt32 sync_pts;
        public byte clip_count;
        public MPLS_CLIP[] clip = Array.Empty<MPLS_CLIP>();

        public bool Parse(BitStream bits) {
            if (!bits.IsAligned())
            {
                Utils.BD_DEBUG(LogLevel.Critical, module, "_parse_subplayitem: alignment error");
            }

            // playitem length
            Int32 len = bits.Read<UInt16>();
            Int64 pos = bits.Position;

            if (len < 24)
            {
                Utils.BD_DEBUG(LogLevel.Critical, module, $"_parse_subplayitem: invalid length {len}");
                return false;
            }

            if (bits.AvailableBytes() < len)
            {
                Utils.BD_DEBUG(LogLevel.Critical, module, "_parse_subplayitem: unexpected EOF");
                return false;
            }

            // primary clip identifier
            MPLS_CLIP firstClip = new();
            firstClip.clip_id = bits.ReadString(5);
            firstClip.codec_id = bits.ReadString(4);
            if ((firstClip.codec_id != "M2TS") && (firstClip.codec_id != "FMTS"))
            {
                Utils.BD_DEBUG(LogLevel.Warning, module, $"Incorrect CodecIdentifier ({firstClip.codec_id})");
            }

            bits.Skip(27);

            this.connection_condition = bits.Read<byte>(4);

            if (this.connection_condition != 0X01
                && this.connection_condition != 0X05
                && this.connection_condition != 0X06)
            {
                Utils.BD_DEBUG(LogLevel.Warning, module, $"Unexpected connection condition {this.connection_condition:X2}");
            }
            this.is_multi_clip = bits.ReadBool();
            firstClip.stc_id = bits.Read<byte>();
            this.in_time = bits.Read<UInt32>();
            this.out_time = bits.Read<UInt32>();
            this.sync_play_item_id = bits.Read<UInt16>();
            this.sync_pts = bits.Read<UInt32>();
            this.clip_count = 1;
            if (this.is_multi_clip)
            {
                this.clip_count = bits.Read<byte>();
                if (this.clip_count < 1)
                {
                    this.clip_count = 1;
                }
                bits.Skip(8); // reserved
            }
            this.clip = new MPLS_CLIP[this.clip_count];
            this.clip[0] = firstClip;
            for (int i = 1; i < this.clip_count; i++)
            {
                // TODO MPLS_CLIP.parse()???
                // Primary clip identifer
                this.clip[i] = new();
                this.clip[i].clip_id = bits.ReadString(5);
                this.clip[i].codec_id = bits.ReadString(4);

                if ((this.clip[i].codec_id != "M2TS") && (this.clip[i].codec_id != "FMTS"))
                {
                    Utils.BD_DEBUG(LogLevel.Warning, module, $"Incorrect CodecIdentifier ({this.clip[i].codec_id})");
                }
                this.clip[i].stc_id = bits.Read<byte>();
            }

            // Seek to end of subpath
            bits.Seek(pos + len * 8);
            if (bits.EOF)
            {
                return false;
            }

            return true;
        }
    }
}
