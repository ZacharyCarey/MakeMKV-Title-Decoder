using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.Data.Bluray.MPLS
{
    public struct PlayItem
    {

        public string clip_id;
        public string codec_id;
        public uint connection_condition;
        public uint stc_id;

        /// <summary>
        /// nanoseconds
        /// </summary>
        public ulong in_time;
        public ulong out_time;
        public ulong relative_in_time;
        public bool is_multi_angle;
        public STN stn;

        public void dump()
        {
            Console.WriteLine($"    play item dump");
            Console.WriteLine($"      clip_id / codec_id:      {clip_id} / {codec_id}");
            Console.WriteLine($"      connection_condition:    {connection_condition}");
            Console.WriteLine($"      is_multi_angle / stc_id: {is_multi_angle} / {stc_id}");
            Console.WriteLine($"      in_time / out_time:      {in_time} / {out_time}");
            Console.WriteLine($"      relative_in_time / end:  {relative_in_time} / {relative_in_time + out_time - in_time}");

            stn.dump();
        }
    }
}
