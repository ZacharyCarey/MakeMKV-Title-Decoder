using Instances;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Utils;

namespace FfmpegInterface.FFProbeCore
{
    public static class ffprobe
    {

        public static IMediaAnalysis? Analyse(string filePath)
        {

            ProcessArguments args = GetProcessArguments("-loglevel error", "-print_format json", "-show_format -sexagesimal", "-show_streams", "-show_chapters", $"\"{filePath}\"");
            IProcessResult result = args.StartAndWaitForExit();
            if (result.ExitCode != 0) return null;

            return ParseOutput(result);
        }

        public static uint? GetNumberOfFrames(string inputFile)
        {
            IMediaAnalysis? analysis = Analyse(inputFile);
            return analysis?.PrimaryVideoStream?.NumberOfFrames;
        }

        private static ProcessArguments GetProcessArguments(params string[] args)
        {
            string? exePath = CommandLineInterface.SearchLocalExeFiles(Path.Combine("lib", "ffprobe.exe"));
            if (exePath == null) throw new Exception("Failed to licate ffprobe executable.");

            var startInfo = new ProcessStartInfo(exePath, string.Join(' ', args));
            return new ProcessArguments(startInfo);
        }

        private static IMediaAnalysis? ParseOutput(IProcessResult instance)
        {
            string json = string.Join("", instance.OutputData);
            var analysis = JsonSerializer.Deserialize<FFProbeAnalysis>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (analysis?.Format == null)
            {
                return null;
            }

            analysis.ErrorData = instance.ErrorData;
            return new MediaAnalysis(analysis);
        }
    }
}
