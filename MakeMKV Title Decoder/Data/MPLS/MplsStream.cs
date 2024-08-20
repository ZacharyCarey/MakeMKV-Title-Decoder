using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.Data.MPLS {

    /// <summary>
    /// Blu-ray specs 5.3.4.5.2.1 table 5-8
    /// </summary>
    public enum StreamType {
        Reserved = 0,
        UsedByPlayItem = 1,
        UsedBySubPathType23456 = 2,
        UsedBySubPathType7 = 3
    }

    // Blu-ray specs 5.4.4.3.2 table 5-16
    public enum StreamCodingType {
        mpeg2_video_primary_secondary = 0x02,
        mpeg4_avc_video_primary_secondary = 0x1b,
        mpegh_hevc_video_primary_secondary = 0x24,
        vc1_video_primary_secondary = 0xea,
        lpcm_audio_primary = 0x80,
        ac3_audio_primary = 0x81,
        dts_audio_primary = 0x82,
        truehd_audio_primary = 0x83,
        eac3_audio_primary = 0x84,
        dts_hd_audio_primary = 0x85,
        dts_hd_xll_audio_primary = 0x86,
        eac3_audio_secondary = 0xa1,
        dts_hd_audio_secondary = 0xa2,
        presentation_graphics_subtitles = 0x90,
        interactive_graphics_menu = 0x91,
        text_subtitles = 0x92,
    }

    public struct MplsStream {

        public StreamType stream_type;
        public StreamCodingType coding_type;
        public uint sub_path_id;
        public uint sub_clip_id;
        public uint pid;
        public uint format;
        public uint rate;
        public uint char_code;

        /// <summary>
        /// bcp47
        /// </summary>
        public string language; 

        public void dump(string type) {
            Console.WriteLine($"        {type} stream dump");
            Console.WriteLine($"          stream_type:                     {(int)stream_type} [{stream_type.ToString()}]");
            Console.WriteLine($"          sub_path_id / sub_clip_id / pid: {sub_path_id} / {sub_clip_id} / {pid:04x}");
            Console.WriteLine($"          coding_type:                     {((int)coding_type):02x} [{coding_type.ToString()}]");
            Console.WriteLine($"          format / rate:                   {format} / {rate}");
            Console.WriteLine($"          char_code / language:            {char_code} / {language}");
        }

    }
}
