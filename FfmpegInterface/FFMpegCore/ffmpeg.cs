using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace FfmpegInterface.FFMpegCore
{
    public static class ffmpeg
    {
        /// <summary>
        /// Returns the output file name (not including the path) which is the same file name with the .mp4 extension
        /// </summary>
        /// <param name="inputPath"></param>
        /// <param name="outputFolder"></param>
        /// <returns></returns>
        public static FFMpegCommand TranscodeToMP4(string inputPath, string outputPath, SelectedStreams streams, int? crf = null)
        {
            List<string> args = ["-i", $"\"{inputPath}\"", ..streams.Arguments, "-c:v libx264"]; 
            
            if (crf != null)
            {
                if (crf < 0 || crf > 63)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Invalid CRF argument!");
                    Console.ResetColor();
                } else
                {
                    args.Add($"-crf {crf}");
                }
            }
            
            args.AddRange(["-c:a aac", "-c:s copy", "-c:d copy", "-c:t copy", "-f mp4", $"\"{outputPath}\""]);

            return new("ffmpeg.exe", args.ToArray());
        }

        public static FFMpegCommand ExtractFrame(string inputPath, uint frameIndex, string outputPath)
        {
            return new("ffmpeg.exe", $"-i \"{inputPath}\"", "-vf", $"\"select=eq(n\\,{frameIndex})\"", "-vframes 1", $"\"{outputPath}\"", "-y");
        }

    }
}
