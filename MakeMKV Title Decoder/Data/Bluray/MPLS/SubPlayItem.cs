using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.Data.Bluray.MPLS
{
    public struct SubPlayItem
    {

        public string clip_file_name;
        public string codec_id;
        public uint connection_condition;
        public uint sync_playitem_id;
        public uint ref_to_stc_id;
        public bool is_multi_clip_entries;
        public ulong in_time;
        public ulong out_time;
        public ulong sync_start_pts_of_playitem;
        public SubPlayItemClip[] clips;

        public void dump()
        {
            Console.WriteLine($"      sub play item dump");
            Console.WriteLine($"        clip_id / codec_id:    {clip_file_name} / {codec_id}");
            Console.WriteLine($"        conn_con / sync_pi_id: {connection_condition} / {sync_playitem_id}");
            Console.WriteLine($"        ref_to_stc_id / multi: {ref_to_stc_id} / {is_multi_clip_entries}");
            Console.WriteLine($"        in_time / out_time:    {in_time} / {out_time}");
            Console.WriteLine($"        sync_start_pts:        {sync_start_pts_of_playitem}");

            foreach (var clip in clips)
            {
                clip.dump();
            }
        }
    }
}
