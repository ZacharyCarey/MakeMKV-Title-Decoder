using FFMpeg_Wrapper.ffprobe;
using Iso639;
using libbluray.bdnav.Clpi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace MakeMKV_Title_Decoder.Data.BluRay
{
    public class BlurayStream : LoadedStream
    {
        internal BlurayStream(string root, string filePath, MediaAnalysis data) : base(root, filePath, data)
        {
            TimeSpan? duration;
            (Language? Language, int PID)[] trackData;
            GetDataFromCLPI(root, filePath, out duration, out trackData);

            // First try to directly load the length as indicated on the disc
            if (duration == null)
            {
                // If failed to load from disc, as a last resort get the length from ffprobe
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Failed to load duration from disc, using FFProbe instead.");
                Console.ResetColor();

                FFProbe ffprobe = new(FileUtils.GetFFProbeExe());
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
            } else
            {
                this.Duration = duration.Value;
            }

            foreach ((Language? lang, int pid) in trackData)
            {
                LoadedTrack? track = this.Tracks.Where(x => x.Identity.ID == pid).FirstOrDefault();
                if (track == null)
                {
                    Log.Warn($"Failed to find track with PID={pid}");
                    continue;
                }

                track.Identity.Language = lang;
                track.RenameData.Language = lang;
            }
        }

        public override int GetVlcID(LoadedTrack track) {
            return track.Identity.ID ?? -1;
        }

        private static void GetDataFromCLPI(string root, string filePath, out TimeSpan? duration, out (Language? language, int PID)[] trackData)
        {
            ClpiFile? clpiFile = ClpiFile.Parse(Path.Combine(root, "BDMV", "CLIPINF", $"{Path.GetFileNameWithoutExtension(filePath)}.clpi"));
            if (clpiFile == null)
            {
                duration = null;
                trackData = Array.Empty<(Language?, int)>();
                return;
            }

            duration = clpiFile.sequence.atc_seq.SelectMany(x => x.stc_seq.Select(y => y.Length)).Aggregate((a, b) => a + b);

            if (clpiFile.program.progs.Length > 1) throw new Exception("More than one program is not supported.");
            trackData = clpiFile.program.progs[0].streams.OrderBy(x => x.pid).Select(stream => (TryParseLang(stream.attributes.lang), (int)stream.pid)).ToArray();
        }

        private static Language? TryParseLang(string lang) {
            return LanguageJsonConverter.TryParseLanguage(lang);
        }
    }
}
