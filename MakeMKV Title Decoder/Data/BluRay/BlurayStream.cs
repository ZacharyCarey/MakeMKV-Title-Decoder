using FfmpegInterface.FFProbeCore;
using libbluray.bdnav.Clpi;
using MkvToolNix.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.Data.BluRay
{
    public class BlurayStream : LoadedStream
    {

        internal BlurayStream(string root, string filePath, MkvMergeID data) : base(root, filePath, data)
        {
            // First try to directly load the length as indicated on the disc
            TimeSpan? duration = GetDurationFromCLPI(root, filePath);
            if (duration == null)
            {
                // If failed to load from disc, as a last resort get the length from ffprobe
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Failed to load duration from disc, using FFProbe instead.");
                Console.ResetColor();

                var info = ffprobe.Analyse(Path.Combine(root, filePath));
                if (info != null)
                {
                    duration = info.Duration;
                }
            }

            if (duration == null)
            {
                this.Duration = TimeSpan.Zero;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Failed to load duration!");
                Console.ResetColor();
            }
        }

        private static TimeSpan? GetDurationFromCLPI(string root, string filePath)
        {
            ClpiFile? clpiFile = ClpiFile.Parse(Path.Combine(root, "BDMV", "CLIPINF", $"{Path.GetFileNameWithoutExtension(filePath)}.clpi"));
            if (clpiFile == null)
            {
                return null;
            }

            return clpiFile.sequence.atc_seq.SelectMany(x => x.stc_seq.Select(y => y.Length)).Aggregate((a, b) => a + b);            
        }
    }
}
