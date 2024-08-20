using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.Data.Bluray.MPLS
{
    public struct Playlist
    {

        public uint list_count;
        public uint sub_count;
        public PlayItem[] items;
        public SubPath[] sub_paths;

        /// <summary>
        /// nanoseconds
        /// </summary>
        public ulong duration;

        public TimeSpan Duration
        {
            get => TimeSpan.FromTicks((long)(duration / 100));
        }

        public void dump()
        {
            Console.WriteLine("  playlist dump");
            Console.WriteLine($"    list_count / sub_count: {list_count} / {sub_count}");
            Console.WriteLine($"    duration:               {Duration:c}");

            foreach (var item in items)
            {
                item.dump();
            }

            foreach (var path in sub_paths)
            {
                path.dump();
            }
        }

    }
}
