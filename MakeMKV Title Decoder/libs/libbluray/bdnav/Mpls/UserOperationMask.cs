using libbluray.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace libbluray.bdnav.Mpls
{

    public enum UO_MASK_TYPE
    {
        MENU_CALL_INDEX = 0,
        TITLE_SEARCH_INDEX = 1
    }

    public class UserOperationMask
    {
        const string module = "libbluray.bdnav.Mpls.UserOperationMask";
        public static UserOperationMask Empty => new UserOperationMask();

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

        public UserOperationMask combine(UserOperationMask other)
        {
            UserOperationMask result = new();

            result.menu_call = menu_call | other.menu_call;
            result.title_search = title_search | other.title_search;
            result.chapter_search = chapter_search | other.chapter_search;
            result.time_search = time_search | other.time_search;
            result.skip_to_next_point = skip_to_next_point | other.skip_to_next_point;
            result.skip_to_prev_point = skip_to_prev_point | other.skip_to_prev_point;
            result.play_firstplay = play_firstplay | other.play_firstplay;
            result.stop = stop | other.stop;
            result.pause_on = pause_on | other.pause_on;
            result.pause_off = pause_off | other.pause_off;
            result.still_off = still_off | other.still_off;
            result.forward = forward | other.forward;
            result.backward = backward | other.backward;
            result.resume = resume | other.resume;
            result.move_up = move_up | other.move_up;
            result.move_down = move_down | other.move_down;
            result.move_left = move_left | other.move_left;
            result.move_right = move_right | other.move_right;
            result.select = select | other.select;
            result.activate = activate | other.activate;
            result.select_and_activate = select_and_activate | other.select_and_activate;
            result.primary_audio_change = primary_audio_change | other.primary_audio_change;
            result.reserved0 = reserved0 | other.reserved0;
            result.angle_change = angle_change | other.angle_change;
            result.popup_on = popup_on | other.popup_on;
            result.popup_off = popup_off | other.popup_off;
            result.pg_enable_disable = pg_enable_disable | other.pg_enable_disable;
            result.pg_change = pg_change | other.pg_change;
            result.secondary_video_enable_disable = secondary_video_enable_disable | other.secondary_video_enable_disable;
            result.secondary_video_change = secondary_video_change | other.secondary_video_change;
            result.secondary_audio_enable_disable = secondary_audio_enable_disable | other.secondary_audio_enable_disable;
            result.secondary_audio_change = secondary_audio_change | other.secondary_audio_change;
            result.reserved1 = reserved1 | other.reserved1;
            result.pip_pg_change = pip_pg_change | other.pip_pg_change;

            return result;
        }

        public bool parse(BitStream stream)
        {
            if (!stream.IsAligned())
            {
                Utils.BD_DEBUG(LogLevel.Warning, module, "Expected bytes to be aligned.");
            }

            menu_call = stream.ReadBool();
            title_search = stream.ReadBool();
            chapter_search = stream.ReadBool();
            time_search = stream.ReadBool();
            skip_to_next_point = stream.ReadBool();
            skip_to_prev_point = stream.ReadBool();
            play_firstplay = stream.ReadBool();
            stop = stream.ReadBool();
            pause_on = stream.ReadBool();
            pause_off = stream.ReadBool();
            still_off = stream.ReadBool();
            forward = stream.ReadBool();
            backward = stream.ReadBool();
            resume = stream.ReadBool();
            move_up = stream.ReadBool();
            move_down = stream.ReadBool();
            move_left = stream.ReadBool();
            move_right = stream.ReadBool();
            select = stream.ReadBool();
            activate = stream.ReadBool();
            select_and_activate = stream.ReadBool();
            primary_audio_change = stream.ReadBool();
            stream.Skip(1);
            angle_change = stream.ReadBool();
            popup_on = stream.ReadBool();
            popup_off = stream.ReadBool();
            pg_enable_disable = stream.ReadBool();
            pg_change = stream.ReadBool();
            secondary_video_enable_disable = stream.ReadBool();
            secondary_video_change = stream.ReadBool();
            secondary_audio_enable_disable = stream.ReadBool();
            secondary_audio_change = stream.ReadBool();
            stream.Skip(1);
            pg_change = stream.ReadBool();
            stream.Skip(30);

            return true;
        }
    }
}
