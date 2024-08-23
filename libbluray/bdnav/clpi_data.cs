using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libbluray.bdnav {
    public class CLPI_STC_SEQ {
        public UInt16 pcr_pid;
        public UInt32 spn_stc_start;
        public UInt32 presentation_start_time;
        public UInt32 presentation_end_time;
    }

    public class CLPI_ATC_SEQ {
        public UInt32 spn_atc_start;
        public byte num_stc_seq;
        public byte offset_stc_id;
        public CLPI_STC_SEQ[] stc_seq = null;
    }

    public class CLPI_SEQ_INFO {
        public byte num_atc_seq;
        public CLPI_ATC_SEQ[] atc_seq = null;
    }

    public class CLPI_TS_TYPE {
        public byte validity;
        public string format_id = ""; // length 5
    }

    public class CLPI_ATC_DELTA {
        public UInt32 delta;
        public string file_id = ""; // length 6
        public string file_code = ""; // length 5
    }

    public struct CLPI_FONT {
        public string file_id = ""; // length 6

        public CLPI_FONT() { }
    }

    public class CLPI_FONT_INFO {
        public byte font_count;
        public CLPI_FONT[] font = null;
    }

    public class CLPI_CLIP_INFO {
        public byte clip_stream_type;
        public byte application_type;
        public bool is_atc_delta;
        public UInt32 ts_recording_rate;
        public UInt32 num_source_packets;
        public CLPI_TS_TYPE ts_type_info = new();
        public byte atc_delta_count;
        public CLPI_ATC_DELTA[] atc_delta = null;
        public CLPI_FONT_INFO font_info = new();      /* Text subtitle stream font files */
    }

    public class CLPI_PROG_STREAM {
        public SimpleStreamType Type = SimpleStreamType.Unknown;

        public UInt16 pid;
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
        public byte dynamic_range_type;
        public ColorSpace color_space = ColorSpace.Unknown;
        public bool hdr_plus_flag;
        public byte[] isrc = new byte[12];     /* International Standard Recording Code (usually empty or all zeroes) */
    }

    public class CLPI_PROG {
        public UInt32 spn_program_sequence_start;
        public UInt16 program_map_pid;
        public byte num_streams;
        public byte num_groups;
        public CLPI_PROG_STREAM[] streams = null;
    }

    public class CLPI_PROG_INFO {
        public byte num_prog;
        public CLPI_PROG[] progs = null;
    }

    public class CLPI_EP_COARSE {
        public int ref_ep_fine_id;
        public int pts_ep;
        public UInt32 spn_ep;
    }

    public class CLPI_EP_FINE {
        public bool is_angle_change_point;
        public byte i_end_position_offset;
        public int pts_ep;
        public int spn_ep;
    }

    public class CLPI_EP_MAP_ENTRY {
        public UInt16 pid;
        public byte ep_stream_type;
        public int num_ep_coarse;
        public int num_ep_fine;
        public UInt32 ep_map_stream_start_addr;
        public CLPI_EP_COARSE[] coarse = null;
        public CLPI_EP_FINE[] fine = null;
    }

    public class CLPI_CPI {
        public byte type;
        // ep_map
        public byte num_stream_pid;
        public CLPI_EP_MAP_ENTRY[] entry = null;
    }

    /// <summary>
    /// Extent start points (profile 5 / version 2.4)
    /// </summary>
    public class CLPI_EXTENT_START {
        public UInt32 num_point;
        public UInt32[] point = null;
    }
}
