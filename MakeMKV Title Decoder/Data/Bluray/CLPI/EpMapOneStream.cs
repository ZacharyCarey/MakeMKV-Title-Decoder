using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.Data.Bluray.CLPI
{

    public struct CoarsePoint
    {
        public ulong ref_to_fine_id = 0;
        public ulong pts = 0;
        public ulong spn = 0;

        public CoarsePoint() { }
    }

    public struct FinePoint
    {
        public ulong pts = 0;
        public ulong spn = 0;

        public FinePoint() { }
    }

    public struct Point
    {
        public ulong pts = 0;
        public TimeSpan PTS { get => TimeSpan.FromTicks((long)pts / 100); }

        public ulong spn = 0;

        public Point() { }
    }

    public struct EpMapOneStream
    {

        public uint pid = 0;
        public uint type = 0;
        public uint num_coarse_points = 0;
        public uint num_fine_points = 0;
        public uint address = 0;

        public CoarsePoint[] coarse_points;
        public FinePoint[] fine_points;
        public Point[] points;

        public EpMapOneStream() { }

        public void dump(uint index)
        {
            Console.WriteLine($"  EP map for one stream[{index}]:");
            Console.WriteLine($"    PID:               {pid}");
            Console.WriteLine($"    type:              {type}");
            Console.WriteLine($"    num_coarse_points: {num_coarse_points}");
            Console.WriteLine($"    num_fine_points:   {num_fine_points}");
            Console.WriteLine($"    address:           {address}");

            index = 0;
            foreach (var coarse_point in coarse_points)
            {
                Console.WriteLine($"    coarse point[{index++}]:");
                Console.WriteLine($"      ref_to_fine_id: {coarse_point.ref_to_fine_id}");
                Console.WriteLine($"      PTS:            {coarse_point.pts}");
                Console.WriteLine($"      SPN:            {coarse_point.spn}");
            }

            index = 0;
            foreach (var fine_point in fine_points)
            {
                Console.WriteLine($"    fine point[{index++}]:");
                Console.WriteLine($"      PTS: {fine_point.pts}");
                Console.WriteLine($"      SPN: {fine_point.spn}");
            }

            index = 0;
            foreach (var point in points)
            {
                Console.WriteLine($"    calculated point[{index++}]:");
                Console.WriteLine($"      PTS: {point.pts}");
                Console.WriteLine($"      SPN: {point.spn}");
            }
        }

    }
}
