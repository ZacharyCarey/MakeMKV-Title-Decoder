using libbluray.bdnav.Clpi;
using libbluray.bdnav.Mpls;
using libbluray.disc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libbluray.bdnav {
    public static class navigation_utils {
        const int CONNECT_NON_SEAMLESS = 0;
        const int CONNECT_SEAMLESS = 1;

        const int TITLES_ALL = 0;
        const int TITLES_FILTER_DUP_TITLE = 1;
        const int TITLES_FILTER_DUP_CLIP = 2;
        const int TITLES_RELEVANT = (TITLES_FILTER_DUP_TITLE | TITLES_FILTER_DUP_CLIP);
    }

    public class NAV_MARK {
        public Int32 number;
        public Int32 mark_type;
        public UInt32 clip_ref;
        public UInt32 clip_pkt;
        public UInt32 clip_time;

        // Title relative metrics
        public UInt32 title_pkt;
        public UInt32 title_time;
        public UInt32 duration;
    }

    public class NAV_MARK_LIST {
        public UInt32 count;
        public NAV_MARK? mark = null;
    }

    public class NAV_CLIP {
        public string name = ""; // length 11
        public UInt32 clip_id;
        public UInt32 reference;
        public UInt32 start_pkt;
        public UInt32 end_pkt;
        public byte connection;
        public byte angle;

        public UInt32 duration;

        public UInt32 in_time;
        public UInt32 out_time;

        // Title relative metrics
        public UInt32 title_pkt;
        public UInt32 title_time;

        public NAV_TITLE? title = null;

        public UInt32 stc_spn;  /* start packet of clip STC sequence */

        public byte still_mode;
        public UInt16 still_time;

        public ClpiFile? cl = null;
    }

    public class NAV_CLIP_LIST {
        public UInt32 count;
        public NAV_CLIP? clip = null;
    }

    public class NAV_SUB_PATH {
        public byte type;
        public NAV_CLIP_LIST clip_list = new();
    }

    public class NAV_TITLE {
        public BD_DISC? disc = null;
        public string name = ""; // length 11
        public byte angle_count;
        public byte angle;
        public NAV_CLIP_LIST clip_list = new();
        public NAV_MARK_LIST chap_list = new();
        public NAV_MARK_LIST mark_list = new();

        public UInt32 sub_path_count;
        public NAV_SUB_PATH? sub_path = null;

        public UInt32 packets;
        public UInt32 duration;

        //public PlayList? pl = null;
    }

    public class NAV_TITLE_INFO {
        public string name = ""; // length 11
        public UInt32 mpls_id;
        public UInt32 duration;
        public UInt32 reference;
    }

    public class NAV_TITLE_LIST {
        public UInt32 count;
        public NAV_TITLE_INFO? title_info = null;
        public UInt32 main_title_idx;
    }
}
