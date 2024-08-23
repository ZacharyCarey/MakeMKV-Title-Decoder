using libbluray.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace libbluray.bdnav {

    public enum UO_MASK_TYPE {
        MENU_CALL_INDEX = 0,
        TITLE_SEARCH_INDEX = 1
    }

    public class BD_UO_MASK {
        public static BD_UO_MASK Empty => new BD_UO_MASK();

        public bool menu_call = false;
        public bool title_search = false;
        public bool chapter_search = false;
        public bool time_search = false;
        public bool skip_to_next_point = false;
        public bool skip_to_prev_point = false;
        public bool play_firstplay = false;
        public bool stop = false;
        public bool pause_on = false;
        public bool pause_off = false;
        public bool still_off = false;
        public bool forward = false;
        public bool backward = false;
        public bool resume = false;
        public bool move_up = false;
        public bool move_down = false;
        public bool move_left = false;
        public bool move_right = false;
        public bool select = false;
        public bool activate = false;
        public bool select_and_activate = false;
        public bool primary_audio_change = false;
        public bool reserved0 = false;
        public bool angle_change = false;
        public bool popup_on = false;
        public bool popup_off = false;
        public bool pg_enable_disable = false;
        public bool pg_change = false;
        public bool secondary_video_enable_disable = false;
        public bool secondary_video_change = false;
        public bool secondary_audio_enable_disable = false;
        public bool secondary_audio_change = false;
        public bool reserved1 = false;
        public bool pip_pg_change = false;

        public BD_UO_MASK combine(BD_UO_MASK other) {
            BD_UO_MASK result = new();
            // TODO this should probably just be a struct..... make sure that refs to this arent needed
            result.menu_call = this.menu_call | other.menu_call;
            result.title_search = this.title_search | other.title_search;
            result.chapter_search = this.chapter_search | other.chapter_search;
            result.time_search = this.time_search | other.time_search;
            result.skip_to_next_point = this.skip_to_next_point | other.skip_to_next_point;
            result.skip_to_prev_point = this.skip_to_prev_point | other.skip_to_prev_point;
            result.play_firstplay = this.play_firstplay | other.play_firstplay;
            result.stop = this.stop | other.stop;
            result.pause_on = this.pause_on | other.pause_on;
            result.pause_off = this.pause_off | other.pause_off;
            result.still_off = this.still_off | other.still_off;
            result.forward = this.forward | other.forward;
            result.backward = this.backward | other.backward;
            result.resume = this.resume | other.resume;
            result.move_up = this.move_up | other.move_up;
            result.move_down = this.move_down | other.move_down;
            result.move_left = this.move_left | other.move_left;
            result.move_right = this.move_right | other.move_right;
            result.select = this.select | other.select;
            result.activate = this.activate | other.activate;
            result.select_and_activate = this.select_and_activate | other.select_and_activate;
            result.primary_audio_change = this.primary_audio_change | other.primary_audio_change;
            result.reserved0 = this.reserved0 | other.reserved0;
            result.angle_change = this.angle_change | other.angle_change;
            result.popup_on = this.popup_on | other.popup_on;
            result.popup_off = this.popup_off | other.popup_off;
            result.pg_enable_disable = this.pg_enable_disable | other.pg_enable_disable;
            result.pg_change = this.pg_change | other.pg_change;
            result.secondary_video_enable_disable = this.secondary_video_enable_disable | other.secondary_video_enable_disable;
            result.secondary_video_change = this.secondary_video_change | other.secondary_video_change;
            result.secondary_audio_enable_disable = this.secondary_audio_enable_disable | other.secondary_audio_enable_disable;
            result.secondary_audio_change = this.secondary_audio_change | other.secondary_audio_change;
            result.reserved1 = this.reserved1 | other.reserved1;
            result.pip_pg_change = this.pip_pg_change | other.pip_pg_change;

            return result;
        }

        public static bool parse(byte[] buf, BD_UO_MASK uo) {
            BITBUFFER bb = new();
            bb.init(buf, 8);

            uo.menu_call = bb.read_bool();
            uo.title_search = bb.read_bool();
            uo.chapter_search = bb.read_bool();
            uo.time_search = bb.read_bool();
            uo.skip_to_next_point = bb.read_bool();
            uo.skip_to_prev_point = bb.read_bool();
            uo.play_firstplay = bb.read_bool();
            uo.stop = bb.read_bool();
            uo.pause_on = bb.read_bool();
            uo.pause_off = bb.read_bool();
            uo.still_off = bb.read_bool();
            uo.forward = bb.read_bool();
            uo.backward = bb.read_bool();
            uo.resume = bb.read_bool();
            uo.move_up = bb.read_bool();
            uo.move_down = bb.read_bool();
            uo.move_left = bb.read_bool();
            uo.move_right = bb.read_bool();
            uo.select = bb.read_bool();
            uo.activate = bb.read_bool();
            uo.select_and_activate = bb.read_bool();
            uo.primary_audio_change = bb.read_bool();
            bb.skip(1);
            uo.angle_change = bb.read_bool();
            uo.popup_on = bb.read_bool();
            uo.popup_off = bb.read_bool();
            uo.pg_enable_disable = bb.read_bool();
            uo.pg_change = bb.read_bool();
            uo.secondary_video_enable_disable = bb.read_bool();
            uo.secondary_video_change = bb.read_bool();
            uo.secondary_audio_enable_disable = bb.read_bool();
            uo.secondary_audio_change = bb.read_bool();
            bb.skip(1);
            uo.pg_change = bb.read_bool();
            bb.skip(30);

            return true;
        }
    }
}
