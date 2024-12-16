using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libbluray.bdnav.Mpls {
    public enum TSStreamType : byte {
        Unknown = 0,
        MPEG1_VIDEO = 0x01,
        MPEG2_VIDEO = 0x02,
        AVC_VIDEO = 0x1b,
        MVC_VIDEO = 0x20,
        HEVC_VIDEO = 0x24,
        VC1_VIDEO = 0xea,
        MPEG1_AUDIO = 0x03,
        MPEG2_AUDIO = 0x04,
        MPEG2_AAC_AUDIO = 0x0F,
        MPEG4_AAC_AUDIO = 0x11,
        LPCM_AUDIO = 0x80,
        AC3_AUDIO = 0x81,
        AC3_PLUS_AUDIO = 0x84,
        AC3_PLUS_SECONDARY_AUDIO = 0xA1,
        AC3_TRUE_HD_AUDIO = 0x83,
        DTS_AUDIO = 0x82,
        DTS_HD_AUDIO = 0x85,
        DTS_HD_SECONDARY_AUDIO = 0xA2,
        DTS_HD_MASTER_AUDIO = 0x86,
        PRESENTATION_GRAPHICS = 0x90,
        INTERACTIVE_GRAPHICS = 0x91,
        SUBTITLE = 0x92
    }

    public enum TSVideoFormat : byte {
        Unknown = 0,
        VIDEOFORMAT_480i = 1,
        VIDEOFORMAT_576i = 2,
        VIDEOFORMAT_480p = 3,
        VIDEOFORMAT_1080i = 4,
        VIDEOFORMAT_720p = 5,
        VIDEOFORMAT_1080p = 6,
        VIDEOFORMAT_576p = 7,
        VIDEOFORMAT_2160p = 8,
    }

    public enum TSFrameRate : byte {
        Unknown = 0,
        FRAMERATE_23_976 = 1,
        FRAMERATE_24 = 2,
        FRAMERATE_25 = 3,
        FRAMERATE_29_97 = 4,
        FRAMERATE_50 = 6,
        FRAMERATE_59_94 = 7
    }

    public enum TSChannelLayout : byte {
        Unknown = 0,
        CHANNELLAYOUT_MONO = 1,
        CHANNELLAYOUT_STEREO = 3,
        CHANNELLAYOUT_MULTI = 6,
        CHANNELLAYOUT_COMBO = 12
    }

    public enum TSSampleRate : byte {
        Unknown = 0,
        SAMPLERATE_48 = 1,
        SAMPLERATE_96 = 4,
        SAMPLERATE_192 = 5,
        SAMPLERATE_48_192 = 12,
        SAMPLERATE_48_96 = 14
    }

    public enum TSAspectRatio {
        Unknown = 0,
        ASPECT_4_3 = 2,
        ASPECT_16_9 = 3,
        ASPECT_2_21 = 4
    }

    public class TSDescriptor {
        public byte Name;
        public byte[] Value;

        public TSDescriptor(byte name, byte length) {
            Name = name;
            Value = new byte[length];
        }

        public TSDescriptor Clone() {
            TSDescriptor descriptor =
                new TSDescriptor(Name, (byte)Value.Length);
            Value.CopyTo(descriptor.Value, 0);
            return descriptor;
        }
    }
}
