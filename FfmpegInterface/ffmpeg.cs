using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace FfmpegInterface
{
    public class ffmpeg : CommandLineInterface
    {
        public override string ProgramName => "ffmpeg";

        public ffmpeg() : base(SearchLocalExeFiles(Path.Combine("lib", "ffmpeg.exe")))
        {

        }

        /// <summary>
        /// Extracts a single frame from the video as a PNG.
        /// </summary>
        /// <param name="inputPath">The path to the video file to read from.</param>
        /// <param name="frame">The timestamp of the frame to extract.</param>
        /// <param name="outputPath">The path to save the PNG to. Make sure the extension is ".png"</param>
        public void ExtractFrame(string inputPath, TimeSpan frame, string outputPath)
        {
            base.RunCommand("-i", inputPath, "-ss", frame.ToString(), outputPath);
        }
    }
}
