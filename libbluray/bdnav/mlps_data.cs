using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libbluray.bdnav.Mpls;

// TODO everything in this file should be moved into it's own respective file
namespace libbluray.bdnav
{

    public class MPLS_CLIP {
        /// <summary>
        /// Clip Information file name
        /// </summary>
        public string clip_id; // length 5

        /// <summary>
        /// Clip Codes ID (i.e. M2TS or FMTS)
        /// </summary>
        public string codec_id; // length 4

        /// <summary>
        /// Reference to STC ID
        /// </summary>
        public byte stc_id;
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

}
