using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libbluray.bdnav {
    public class MPLS_STREAM {
        public SimpleStreamType simple_type;
        public byte stream_type;
        public StreamType coding_type;
        public UInt16 pid;
        public byte subpath_id;
        public byte subclip_id;
        public VideoFormat video_format;
        public AudioFormat audio_format;
        public VideoRate video_rate;
        public AudioRate audio_rate;
        public byte dynamic_range_type;
        public ColorSpace color_space;
        public bool cr_flag;
        public bool hdr_plus_flag;
        public TextCharCode char_code;
        public string lang = ""; // length 4
        // Secondary audio specific fields
        public byte sa_num_primary_audio_ref;
        public byte[]? sa_primary_audio_ref;
        // Secondary video specific fields
        public byte sv_num_secondary_audio_ref;
        public byte sv_num_pip_pg_ref;
        public byte[]? sv_secondary_audio_ref;
        public byte[]? sv_pip_pg_ref;
    }

    public class MPLS_STN {
        public byte num_video;
        public byte num_audio;
        public byte num_pg;
        public byte num_ig;
        public byte num_secondary_audio;
        public byte num_secondary_video;
        public byte num_pip_pg;
        public byte num_dv;
        public MPLS_STREAM[]? video = null;
        public MPLS_STREAM[]? audio = null;

        /// <summary>
        /// Presentation graphic streams
        /// </summary>
        public MPLS_STREAM[]? pg = null;

        /// <summary>
        /// Interactive graphic streams
        /// </summary>
        public MPLS_STREAM[]? ig = null;
        public MPLS_STREAM[]? secondary_audio = null;
        public MPLS_STREAM[]? secondary_video = null;

        /// <summary>
        /// Dolby vision enhancement layer
        /// </summary>
        public MPLS_STREAM[]? dv = null;
    }

    public class MPLS_CLIP {
        public string clip_id; // length 6
        public string codec_id; // length 5
        public byte stc_id;
    }

    public class MPLS_PI {
        public bool is_multi_angle;
        public byte connection_condition;
        public UInt32 in_time;
        public UInt32 out_time;
        public BD_UO_MASK uo_mask = new();
        public bool random_access_flag;
        public byte still_mode;
        public UInt16 still_time;
        public byte angle_count;
        public bool is_different_audio;
        public bool is_seamless_angle;
        public MPLS_CLIP[] clip = null;
        public MPLS_STN stn = new();
    }

    public class MPLS_PLM {
        public byte mark_type;
        public UInt16 play_item_ref;
        public UInt32 time;
        public UInt16 entry_es_pid;
        public UInt32 duration;
    }

    public class MPLS_AI {
        public byte playback_type;
        public UInt16 playback_count;
        public BD_UO_MASK uo_mask = new();
        public bool random_access_flag;
        public bool audio_mix_flag;
        public bool lossless_bypass_flag;
        public bool mvc_base_view_r_flag;
        public bool sdr_conversion_notification_flag;
    }

    public class MPLS_SUB_PI {
        public byte connection_condition;
        public bool is_multi_clip;
        public UInt32 in_time;
        public UInt32 out_time;
        public UInt16 sync_play_item_id;
        public UInt32 sync_pts;
        public byte clip_count;
        public MPLS_CLIP[] clip = null;
    }

    public enum mpls_sub_path_type {
/*        /// <summary>
        /// Primary audio of the Browsable slideshow
        /// </summary>
        mpls_sub_path_        = 2,*/

        /// <summary>
        /// Interactive Graphics presentation menu
        /// </summary>
        ig_menu = 3,

        /// <summary>
        /// Text Subtitle
        /// </summary>
        textst = 4,

/*        /// <summary>
        /// Out-of-mux Synchronous elementary streams
        /// </summary>
        mpls_sub_path_ = 5,*/

        /// <summary>
        /// Out-of-mux Asynchronous Picture-in-Picture presentation
        /// </summary>
        async_pip = 6,

        /// <summary>
        /// In-mux Synchronous Picture-in-Picture presentation
        /// </summary>
        sync_pip = 7,

        /// <summary>
        /// SS Video
        /// </summary>
        ss_video = 8,

        /// <summary>
        /// Dolby Vision Enhancement Layer
        /// </summary>
        dv_el = 10
    }

    public class MPLS_SUB {
        /// <summary>
        /// enum mpls_sub_path_type
        /// </summary>
        public byte type;
        public bool is_repeat;
        public byte sub_playitem_count;
        public MPLS_SUB_PI[]? sub_play_item = null;
    }

    public enum mpls_pip_scaling {
        /// <summary>
        /// unscaled
        /// </summary>
        none = 1,

        /// <summary>
        /// 1:2
        /// </summary>
        half = 2,

        /// <summary>
        /// 1:4
        /// </summary>
        quarter = 3,

        /// <summary>
        /// 3:2
        /// </summary>
        one_half = 4,

        /// <summary>
        /// scale to main video size
        /// </summary>
        fullscreen = 5, 
    }

    public class MPLS_PIP_DATA {
        /// <summary>
        /// start timestamp (clip time) when the block is valid
        /// </summary>
        public UInt32 time;       
        public UInt16 xpos;
        public UInt16 ypos;

        /// <summary>
        /// mpls_pip_scaling. Note: PSR14 may override this !
        /// </summary>
        public byte scale_factor;
    }

    public enum mpls_pip_timeline {
        /// <summary>
        /// timeline refers to main path
        /// </summary>
        sync_mainpath = 1,

        /// <summary>
        /// timeline refers to sub-path time
        /// </summary>
        async_subpath = 2,

        /// <summary>
        /// timeline refers to main path
        /// </summary>
        async_mainpath = 3,
    }

    public class MPLS_PIP_METADATA {
        /// <summary>
        /// clip id for secondary_video_ref (STN)
        /// </summary>
        public UInt16 clip_ref;

        /// <summary>
        ///  secondary video stream id (STN)
        /// </summary>
        public byte secondary_video_ref;

        /// <summary>
        /// mpls_pip_timeline
        /// </summary>
        public byte timeline_type;

        /// <summary>
        /// use luma keying
        /// </summary>
        public bool luma_key_flag;

        /// <summary>
        /// luma key (secondary video pixels with Y <= this value are transparent)
        /// </summary>
        public byte upper_limit_luma_key;

        /// <summary>
        /// show synchronous PiP when playing trick speed
        /// </summary>
        public bool trick_play_flag;    

        public UInt16 data_count;
        public MPLS_PIP_DATA[]? data = null;
    }

    /// <summary>
    /// They are stored as GBR, we would like to show them as RGB
    /// </summary>
    public enum mpls_static_primaries {
        primary_green,
        primary_blue,
        primary_red
    }

    public class MPLS_STATIC_METADATA {
        public byte dynamic_range_type;
        public UInt16[] display_primaries_x = new UInt16[3];
        public UInt16[] display_primaries_y = new UInt16[3];
        public UInt16 white_point_x;
        public UInt16 white_point_y;
        public UInt16 max_display_mastering_luminance;
        public UInt16 min_display_mastering_luminance;
        public UInt16 max_CLL;
        public UInt16 max_FALL;
    }

    public class MPLS_PL {
        /// <summary>
        /// 'MPLS'
        /// </summary>
        public string type_indicator;

        /// <summary>
        /// version
        /// </summary>
        public string type_indicator2; 
        public UInt32 list_pos;
        public UInt32 mark_pos;
        public UInt32 ext_pos;
        public MPLS_AI app_info = new();
        public UInt16 list_count;
        public UInt16 sub_count;
        public UInt16 mark_count;
        public MPLS_PI[]? play_item = null;
        public MPLS_SUB[]? sub_path = null;
        public MPLS_PLM[]? play_mark = null;

        // extension data (profile 5, version 2.4)
        public UInt16 ext_sub_count;

        /// <summary>
        /// sub path entries extension
        /// </summary>
        public MPLS_SUB[]? ext_sub_path = null;

        // extension data (Picture-In-Picture metadata)
        public UInt16 ext_pip_data_count;

        /// <summary>
        /// pip metadata extension
        /// </summary>
        public MPLS_PIP_METADATA[]? ext_pip_data = null;  

        // extension data (Static Metadata)
        public byte ext_static_metadata_count;
        public MPLS_STATIC_METADATA[]? ext_static_metadata = null;
    }
}
