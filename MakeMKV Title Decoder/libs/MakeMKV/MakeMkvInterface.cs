using MakeMKV_Title_Decoder.libs.MakeMKV.Data;
using MakeMKV_Title_Decoder.Util;
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
    internal class MakeMkvInterface : CommandLineInterface
    {
        public override string ProgramName => "MakeMKV";

        private MakeMkvInterface(string MakeMkvExePath) : base(MakeMkvExePath)
        {
        }

        public static MakeMkvInterface? FindMakeMkvProcess()
        {
            string ExeLoc = SearchProgramFiles("MakeMKV", "makemkvcon64.exe").FirstOrDefault("");
            if (ExeLoc.Length == 0)
            {
                return null;
            }

            return new MakeMkvInterface(ExeLoc);
        }

        private static IEnumerable<MakeMkvMessage> ReportProgress(IEnumerable<MakeMkvMessage> messages, IProgress<MakeMkvProgress>? progress = null)
        {
            foreach (var msg in messages)
            {
                if (msg.Type == MakeMkvMessageType.ProgressCurrent || msg.Type == MakeMkvMessageType.ProgressTotal)
                {
                    continue;
                }
                else if (msg.Type == MakeMkvMessageType.ProgressValues)
                {
                    if (msg.Arguments.Count != 3) throw new FormatException("Only expected 3 arguments.");
                    progress?.Report(new MakeMkvProgress((int)(long)msg[0], (int)(long)msg[1]));
                }
                else
                {
                    yield return msg;
                }
            }
        }

        /// <exception cref="Exception"></exception>
        public List<DiscDrive>? ReadDrives()
        {
            var result = RunCommand<MakeMkvMessage>("--robot", "info", "disc");

            // Decode
            List<DiscDrive> drives = new();
            foreach (MakeMkvMessage msg in result.Where(x => x.Type == MakeMkvMessageType.Drive))
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
            var process = ReportProgress(RunCommand<MakeMkvMessage>("--robot", "--progress=-stdout", "info", $"disc:{driveIndex}"), progress);

            Disc result = new();
            foreach (var msg in process)
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
            var process = ReportProgress(
                RunCommand<MakeMkvMessage>(
                    "--robot",
                    "--progress=-stdout",
                    "--decrypt",
                    "--noscan",
                    "mkv",
                    $"disc:{driveIndex}",
                    "all",
                    Path.GetFullPath(outputFolder)
                ),
                progress
            );

            foreach (var msg in process)
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
