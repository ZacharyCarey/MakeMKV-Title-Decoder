using libbluray.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libbluray.bdnav.Mpls {
    public class PlayItem {
        const string module = "libbluray.bdnav.Mpls.PlayItem";

        public bool is_multi_angle;
        public byte connection_condition;
        public UInt32 in_time;
        public UInt32 out_time;
        public UserOperationMask uo_mask = new();
        public bool random_access_flag;
        public byte still_mode;
        public UInt16 still_time;
        public byte angle_count;
        public bool is_different_audio;
        public bool is_seamless_angle;
        public MPLS_CLIP[] clip = Array.Empty<MPLS_CLIP>();
        public StnTable stn = new();

        public bool Parse(BitStream bits) {
            if (!bits.IsAligned())
            {
                Utils.BD_DEBUG(LogLevel.Warning, module, "_parse_playitem: stream alignment error");
            }

            // playitem length
            Int32 len = bits.Read<UInt16>();
            Int64 pos = bits.Position;

            if (len < 18)
            {
                Utils.BD_DEBUG(LogLevel.Critical, module, $"_parse_playitem: invalid length {len}");
                return false;
            }
            if (bits.Available() / 8 < len)
            {
                Utils.BD_DEBUG(LogLevel.Critical, module, "_parse_playitem: unexpected EOF");
                return false;
            }

            // Primary clip identifier
            MPLS_CLIP firstClip = new();
            firstClip.clip_id = bits.ReadString(5);
            firstClip.codec_id = bits.ReadString(4);
            if (firstClip.codec_id != "M2TS" && firstClip.codec_id != "FMTS")
            {
                Utils.BD_DEBUG(LogLevel.Critical, module, $"Incorrect CodecIdentifier ({firstClip.codec_id})");
                return false;
            }

            // Skip reserved 11 bits
            bits.Skip(11);

            this.is_multi_angle = bits.ReadBool();

            this.connection_condition = bits.Read<byte>(4);
            if (this.connection_condition != 0x01
                && this.connection_condition != 0x05
                && this.connection_condition != 0x06
            )
            {
                Utils.BD_DEBUG(LogLevel.Warning, module, $"Unexpected connection condition {this.connection_condition:X2}");
            }

            firstClip.stc_id = bits.Read<byte>(8);
            this.in_time = bits.Read<UInt32>();
            this.out_time = bits.Read<UInt32>();

            this.uo_mask.parse(bits);
            this.random_access_flag = bits.ReadBool();
            bits.Skip(7);
            this.still_mode = bits.Read<byte>();
            if (this.still_mode == 0x01)
            {
                this.still_time = bits.Read<UInt16>();
            } else
            {
                bits.Skip(16);
            }

            this.angle_count = 1;
            if (this.is_multi_angle)
            {
                this.angle_count = bits.Read<byte>();
                if (this.angle_count < 1)
                {
                    this.angle_count = 1;
                }
                bits.Skip(6);
                this.is_different_audio = bits.ReadBool();
                this.is_seamless_angle = bits.ReadBool();
            }

            this.clip = new MPLS_CLIP[this.angle_count];
            this.clip[0] = firstClip;
            for (int i = 1; i < this.angle_count; i++)
            {
                // TODO create MPLS_CLIP.parse
                this.clip[i] = new();
                this.clip[i].clip_id = bits.ReadString(5);

                this.clip[i].codec_id = bits.ReadString(4);
                if ((this.clip[i].codec_id != "M2TS") && (this.clip[i].codec_id != "FMTS"))
                {
                    Utils.BD_DEBUG(LogLevel.Critical, module, $"Incorrect CodecIdentifier ({this.clip[i].codec_id})");
                }
                this.clip[i].stc_id = bits.Read<byte>();
            }

            if (!this.stn.Parse(bits))
            {
                return false;
            }

            // Seek past any unused items
            bits.Seek(pos + len * 8);
            if (bits.EOF)
            {
                return false;
            }

            return true;
        }
    }
}
