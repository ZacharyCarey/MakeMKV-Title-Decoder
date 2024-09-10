using libbluray.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libbluray.bdnav.Mpls {
    public class StreamAttributes {
        const string module = "libbluray.bdnav.Mpls.StreamAttributes";

        public SimpleStreamType simple_type; // For your convencience :)
        public StreamType coding_type;
        public VideoFormat video_format;
        public AudioFormat audio_format;
        public VideoRate video_rate;
        public AudioRate audio_rate;
        public DynamicRangeType dynamic_range_type;
        public ColorSpace color_space;
        public bool cr_flag;
        public bool hdr_plus_flag;
        public TextCharCode char_code;
        public string lang = ""; // length 4
        /*// Secondary audio specific fields
        public byte sa_num_primary_audio_ref;
        public byte[]? sa_primary_audio_ref;
        // Secondary video specific fields
        public byte sv_num_secondary_audio_ref;
        public byte sv_num_pip_pg_ref;
        public byte[]? sv_secondary_audio_ref;
        public byte[]? sv_pip_pg_ref;*/

        public bool Parse(BitStream bits) {
            if (!bits.IsAligned())
            {
                Utils.BD_DEBUG(LogLevel.Warning, module, "_parse_stream: Stream alignment error");
            }

            Int32 len = bits.Read<byte>();
            Int64 pos = bits.Position;

            this.lang = "";
            this.coding_type = (StreamType)bits.Read<byte>();
            switch (this.coding_type)
            {
                case StreamType.VIDEO_MPEG1:
                case StreamType.VIDEO_MPEG2:
                case StreamType.VIDEO_VC1:
                case StreamType.VIDEO_H264:
                case StreamType.VIDEO_HEVC:
                    this.simple_type = SimpleStreamType.Video;
                    this.video_format = bits.ReadEnum(4, VideoFormat.Unknown, module);
                    this.video_rate = bits.ReadEnum(4, VideoRate.Unknown, module);
                    if (this.coding_type == StreamType.VIDEO_HEVC)
                    {
                        this.dynamic_range_type = bits.ReadEnum(4, DynamicRangeType.Unknown, module);
                        this.color_space = bits.ReadEnum(4, ColorSpace.Unknown, module);
                        this.cr_flag = bits.ReadBool();
                        this.hdr_plus_flag = bits.ReadBool();
                    }
                    break;
                case StreamType.AUDIO_MPEG1:
                case StreamType.AUDIO_MPEG2:
                case StreamType.AUDIO_LPCM:
                case StreamType.AUDIO_AC3:
                case StreamType.AUDIO_DTS:
                case StreamType.AUDIO_TRUHD:
                case StreamType.AUDIO_AC3PLUS:
                case StreamType.AUDIO_DTSHD:
                case StreamType.AUDIO_DTSHD_MASTER:
                case StreamType.AUDIO_Secondary_DolbyDigital:
                case StreamType.AUDIO_Secondary_DTS:
                    this.simple_type = SimpleStreamType.Audio;
                    this.audio_format = bits.ReadEnum(4, AudioFormat.Unknown, module);
                    this.audio_rate = bits.ReadEnum(4, AudioRate.Unknown, module);
                    this.lang = bits.ReadString(3);
                    break;
                case StreamType.SUB_PresentationGraphics:
                case StreamType.SUB_InteractiveGraphics:
                    this.simple_type = SimpleStreamType.Text;
                    this.lang = bits.ReadString(3);
                    break;
                case StreamType.SUB_TEXT:
                    this.simple_type = SimpleStreamType.Text;
                    this.char_code = bits.ReadEnum(8, TextCharCode.Unknown, module);
                    this.lang = bits.ReadString(3);
                    break;
                default:
                    this.simple_type = SimpleStreamType.Unknown;
                    Utils.BD_DEBUG(LogLevel.Critical, module, $"unrecognized coding type {this.coding_type:X2}");
                    break;
            }

            bits.Seek(pos + (len * 8));
            if (bits.EOF)
            {
                return false;
            }

            return true;
        }
    }
}
