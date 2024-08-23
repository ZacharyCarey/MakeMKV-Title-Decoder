using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libbluray.bdnav {
    public enum StreamType : uint {
        VIDEO_MPEG1          = 0x01,
        VIDEO_MPEG2          = 0x02,
        AUDIO_MPEG1          = 0x03,
        AUDIO_MPEG2          = 0x04,
        AUDIO_LPCM           = 0x80,
        AUDIO_AC3            = 0x81,
        AUDIO_DTS            = 0x82,
        AUDIO_TRUHD          = 0x83,
        AUDIO_AC3PLUS        = 0x84,
        AUDIO_DTSHD          = 0x85,
        AUDIO_DTSHD_MASTER   = 0x86,
        VIDEO_VC1            = 0xea,
        VIDEO_H264           = 0x1b,
        VIDEO_HEVC           = 0x24,
        SUB_PG               = 0x90,
        SUB_IG               = 0x91,
        SUB_TEXT             = 0x92,

        Unknown = 0xFFFFFFFF
    }

    public enum VideoFormat : uint {
        _480I = 1, // ITU-R BT.601-5
        _576I = 2, // ITU-R BT.601-4
        _480P = 3, // SMPTE 293M
        _1080I = 4, // SMPTE 274M
        _720P = 5, // SMPTE 296M
        _1080P = 6, // SMPTE 274M
        _576P = 7, // ITU-R BT.1358
        _2160P = 8,

        Unknown = 0xFFFFFFFF
    }

    public enum VideoRate : uint {
        _24000_1001 = 1, // 23.976
        _24 = 2, 
        _25 = 3, 
        _30000_1001 = 4, // 29.97
        _50 = 6, 
        _60000_1001 = 7, // 59.94

        Unknown = 0xFFFFFFFF
    }

    public enum AspectRatio : uint {
        _4_3 = 2,
        _16_9 = 3,

        Unknown = 0xFFFFFFFF
    }

    public enum ColorSpace : uint {
        _BT_709 = 1,
        _BT_2020 = 2,

        Unknown = 0xFFFFFFFF
    }

    public enum AudioFormat : uint{
        MONO = 1, 
        STEREO = 3, 
        MULTI_CHAN = 6, 
        COMBO = 12, // Stereo ac3/dts, multi mlp/dts-hd

        Unknown = 0xFFFFFFFF
    }

    public enum AudioRate : uint {
        _48 = 1, 
        _96 = 4, 
        _192 = 5, 
        _192_COMBO = 12, // 48 or 96 ac3/dts, 192 mpl/dts-hd
        _96_COMBO = 14, // 48 ac3/dts, 96 mpl/dts-hd

        Unknown = 0xFFFFFFFF
    }

    public enum TextCharCode : uint {
        UTF8 = 0x01, 
        UTF16BE = 0x02, 
        SHIFT_JIS = 0x03, 
        EUC_KR = 0x04, 
        GB18030_20001 = 0x05, 
        CN_GB = 0x06, 
        BIG5 = 0x07,

        Unknown = 0xFFFFFFFF
    }

    public enum SimpleStreamType : uint {
        Audio = 1,
        Video = 2,
        Text = 3,

        Unknown = 0xFFFFFFFF
    }
}
