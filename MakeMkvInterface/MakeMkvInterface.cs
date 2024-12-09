using CLI_Wrapper;
using MakeMKV_Title_Decoder.libs.MakeMKV.Data;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace MakeMKV_Title_Decoder.libs.MakeMKV
{
    /// <summary>
    /// https://www.makemkv.com/developers/usage.txt
    /// </summary>
    public class MakeMkvInterface
    {
        string exePath;

        private MakeMkvInterface(string MakeMkvExePath)
        {
            this.exePath = MakeMkvExePath;
        }

        public static MakeMkvInterface? FindMakeMkvProcess()
        {
            string? exe = FileUtils.SearchProgramFiles("MakeMKV", "makemkvcon64.exe").FirstOrDefault();
            if (exe == null)
            {
                return null;
            }

            return new MakeMkvInterface(exe);
        }

        private static MakeMkvMessage? ParseOutput(string line) {
            MakeMkvMessage msg;
            if (MakeMkvMessage.TryParse(line, null, out msg))
            {
                return msg; 
            } else
            {
                return null;
            }
        }

        private static IEnumerable<MakeMkvMessage> ParseOutput(IEnumerable<string> line) {
            return line
            .Select(line =>
                {
                    MakeMkvMessage msg;
                    MakeMkvMessage? result = null;
                    if (MakeMkvMessage.TryParse(line, null, out msg))
                    {
                        result = msg;
                    }
                    return result;
                })
            .Where(msg => msg != null)
            .Select(msg => msg.Value)
            .Where(msg => msg.Type != MakeMkvMessageType.ProgressCurrent 
                && msg.Type != MakeMkvMessageType.ProgressTotal
                && msg.Type != MakeMkvMessageType.ProgressValues
            );
        }

        private static void ReportProgressCallback(string line, IProgress<MakeMkvProgress>? progress = null) {
            MakeMkvMessage msg;
            if (MakeMkvMessage.TryParse(line, null, out msg))
            {
                if (msg.Type == MakeMkvMessageType.ProgressValues)
                {
                    if (msg.Arguments.Count == 3)
                    {
                        progress?.Report(new MakeMkvProgress((int)(long)msg[0], (int)(long)msg[1]));
                    }
                }
            }
        }

        /// <exception cref="Exception"></exception>
        public List<DiscDrive>? ReadDrives()
        {
            var result = CLI.RunExeFile(exePath)
                .AddArguments("--robot", "info", "disc")
                .Run();
           if (result.Exception != null || result.ExitCode != 0)
            {
                return null;
            }

            // Decode
            List<DiscDrive> drives = new();
            foreach (MakeMkvMessage msg in ParseOutput(result.OutputData).Where(x => x.Type == MakeMkvMessageType.Drive))
            {
                if (msg.Arguments.Count != 7) throw new FormatException("Unexpected number of args when parsing drive.");

                DiscDrive info = new();
                info.Index = (uint)(long)msg[0];
                info.Visible = (uint)(long)msg[1];
                info.Enabled = (uint)(long)msg[2];
                info.Flags = (uint)(long)msg[3];
                info.DriveName = (string)msg[4];
                info.DiscName = (string)msg[5];
                info.DriveLetter = (string)msg[6];

                if (info.IsValid)
                {
                    drives.Add(info);
                    Console.WriteLine($"Found drive {info.DriveName} ({info.DiscName})");
                }
            }

            return drives;
        }

        /// <exception cref="Exception"></exception>
        public Disc? ReadDisc(int driveIndex, IProgress<MakeMkvProgress>? progress = null)
        {
            var cli = CLI.RunExeFile(exePath)
                .AddArguments("--robot", "--progress=-stdout", "info", $"disc:{driveIndex}");
            cli.OutputDataReceived += (object? sender, string str) => ReportProgressCallback(str, progress);

            var cliResult = cli.Run();
            if (cliResult.Exception != null || cliResult.ExitCode != 0)
            {
                return null;
            }

            Disc result = new();
            foreach (var msg in ParseOutput(cliResult.OutputData))
            {
                if (msg.Type == MakeMkvMessageType.Message)
                {
                    Console.WriteLine(msg[3]);
                }
                else if (msg.Type == MakeMkvMessageType.DiscInfo)
                {
                    result.ParseMakeMkv(msg);
                }
                else if (msg.Type == MakeMkvMessageType.TitleInfo)
                {
                    Title.ParseMakeMkv(result, msg);
                }
                else if (msg.Type == MakeMkvMessageType.StreamInfo)
                {
                    Track.ParseMakeMkv(result, msg);
                }
            }
            return result;
        }

        /// <exception cref="Exception"></exception>
        public async Task<Disc?> ReadDiscAsync(int driveIndex, IProgress<MakeMkvProgress>? progress = null)
        {
            var task = Task.Run(() => ReadDisc(driveIndex, progress));
            await task;
            return task.Result;
        }

        /// <exception cref="Exception"></exception>
        public void BackupDisc(int driveIndex, string outputFolder, IProgress<MakeMkvProgress>? progress = null)
        {
            var cli = CLI.RunExeFile(exePath)
                .AddArguments("--robot", "--progress=-stdout", "--decrypt", "--noscan", "mkv", $"disc:{driveIndex}", "all", $"\"{Path.GetFullPath(outputFolder)}\"");
            cli.OutputDataReceived += (object? sender, string str) => ReportProgressCallback(str, progress);

            var result = cli.Run();
            if (result.Exception != null || result.ExitCode != 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Failed to backup disc.");
                Console.ResetColor();
                return;
            }

            foreach (var msg in ParseOutput(result.OutputData))
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(msg);
                Console.ResetColor();
            }
        }

        /// <exception cref="Exception"></exception>
        public async Task BackupDiscAsync(int driveIndex, string outputFolder, IProgress<MakeMkvProgress>? progress = null)
        {
            await Task.Run(() => BackupDisc(driveIndex, outputFolder, progress));
        }

    }
}
