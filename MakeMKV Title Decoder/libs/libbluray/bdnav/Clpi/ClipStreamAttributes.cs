using libbluray.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libbluray.bdnav.Clpi {

    // TODO is this same as Mpls StreamAttribute / parses the same??
    public class ClipStreamAttributes {
        const string module = "lubbluray.bdnav.Clpi.ClipStreamAttributes";

        public SimpleStreamType Type = SimpleStreamType.Unknown;
        public StreamType coding_type = StreamType.Unknown;

        public VideoFormat video_format = VideoFormat.Unknown;
        public AudioFormat audio_format = AudioFormat.Unknown;

        public VideoRate video_rate = VideoRate.Unknown;
        public AudioRate audio_rate = AudioRate.Unknown;

        public AspectRatio aspect = AspectRatio.Unknown;
        public bool oc_flag;
        public TextCharCode char_code;
        public string lang = ""; // length 4
        public bool cr_flag;
        public DynamicRangeType dynamic_range_type;
        public ColorSpace color_space = ColorSpace.Unknown;
        public bool hdr_plus_flag;
        public byte[] isrc = new byte[12];     /* International Standard Recording Code (usually empty or all zeroes) */

        public bool Parse(BitStream bits) {
            if (!bits.IsAligned())
            {
                Utils.BD_DEBUG(LogLevel.Critical, module, "_parse_stream_attr(): Stream alignment error");
            }

            Int64 len = bits.Read<byte>();
            Int64 pos = bits.Position;

            this.lang = "";
            this.isrc = new byte[12];
            this.coding_type = bits.ReadEnum(8, StreamType.Unknown, module);
            switch (this.coding_type)
            {
                case StreamType.VIDEO_MPEG1:
                case StreamType.VIDEO_MPEG2:
                case StreamType.VIDEO_VC1:
                case StreamType.VIDEO_H264:
                case (StreamType)0x20: // TODO unknown type
                case StreamType.VIDEO_HEVC:
                    this.Type = SimpleStreamType.Video;
                    this.video_format = bits.ReadEnum(4, VideoFormat.Unknown, module);
                    this.video_rate = bits.ReadEnum(4, VideoRate.Unknown, module);
                    this.aspect = bits.ReadEnum(4, AspectRatio.Unknown, module);
                    bits.Skip(2);
                    this.oc_flag = bits.ReadBool();
                    if (this.coding_type == StreamType.VIDEO_HEVC)
                    {
                        this.cr_flag = bits.ReadBool();
                        this.dynamic_range_type = bits.ReadEnum(4, DynamicRangeType.Unknown, module);
                        this.color_space = bits.ReadEnum(4, ColorSpace.Unknown, module);
                        this.hdr_plus_flag = bits.ReadBool();
                        bits.Skip(7);
                    } else
                    {
                        bits.Skip(17);
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
                case (StreamType)0xA1: // TODO unknown type
                case (StreamType)0xA2: // TODO unknown type
                    this.Type = SimpleStreamType.Audio;
                    this.audio_format = bits.ReadEnum(4, AudioFormat.Unknown, module);
                    this.audio_rate = bits.ReadEnum(4, AudioRate.Unknown, module);
                    this.lang = bits.ReadString(3);
                    break;

                case StreamType.SUB_PresentationGraphics:
                case StreamType.SUB_InteractiveGraphics:
                case (StreamType)0xA0: // TODO unknown type
                    this.Type = SimpleStreamType.Text;
                    this.lang = bits.ReadString(3);
                    break;

                case StreamType.SUB_TEXT:
                    this.Type = SimpleStreamType.Text;
                    this.char_code = bits.ReadEnum(8, TextCharCode.Unknown, module);
                    this.lang = bits.ReadString(3);
                    break;

                default:
                    Utils.BD_DEBUG(LogLevel.Critical, module, $"_parse_stream_attr(): unrecognized coding type {(uint)this.coding_type:X2}");
                    break;
            }

            // TODO not present in https://github.com/lw/BluRay/wiki/StreamCodingInfo
            bits.Read(this.isrc);

            // Skip over any padding
            bits.Seek(pos + len * 8);
            return true;
        }
    }
}
