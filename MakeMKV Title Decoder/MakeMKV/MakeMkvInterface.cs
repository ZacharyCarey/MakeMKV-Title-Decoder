using MakeMKV_Title_Decoder.Data;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.MakeMKV
{
    /// <summary>
    /// https://www.makemkv.com/developers/usage.txt
    /// </summary>
    internal class MakeMkvInterface
    {
        string exePath;

        private MakeMkvInterface(string MakeMkvExePath)
        {
            exePath = MakeMkvExePath;
        }

        public static MakeMkvInterface? FindMakeMkvProcess()
        {
            string? exeLoc = SearchDefaultInstallLocation();
            Console.WriteLine($"Default Install Loc: {exeLoc}");
            if (exeLoc == null)
            {
                return null;
            }

            return new MakeMkvInterface(exeLoc);
        }

        private static string? SearchDefaultInstallLocation()
        {
            const string basePath = "C:\\Program Files (x86)\\MakeMKV\\";
            string path = basePath;
            if (!Directory.Exists(path))
            {
                return null;
            }

            path = Path.Combine(path, "makemkvcon.exe");
            if (!File.Exists(path))
            {
                return null;
            }

            return path;
        }

        private Process? RunCommand(params string[] args) {
            try
            {
                var startInfo = new ProcessStartInfo();
                startInfo.FileName = this.exePath;
                startInfo.CreateNoWindow = true;
                startInfo.UseShellExecute = false;
                startInfo.RedirectStandardOutput = true;

                startInfo.ArgumentList.Add("--robot");
                foreach(string arg in args)
                {
                    startInfo.ArgumentList.Add(arg);
                }

                return Process.Start(startInfo);
            } catch (Exception ex)
            {
                MessageBox.Show($"Failed to read MakeMKV: {ex.Message}", "Failed to read MakeMKV", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        /// <exception cref="FormatException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="InvalidCastException"></exception>
        private async IAsyncEnumerable<MakeMkvMessage> ParseOutput(Process process, IProgress<MakeMkvProgress>? progress = null) {
            for (string? line = await process.StandardOutput.ReadLineAsync(); line != null; line = await process.StandardOutput.ReadLineAsync())
            {
                MakeMkvMessage msg = MakeMkvMessage.Parse(line);
                if (msg.Type == MakeMkvMessageType.ProgressCurrent || msg.Type == MakeMkvMessageType.ProgressTotal)
                {
                    continue;
                } else if (msg.Type == MakeMkvMessageType.ProgressValues)
                {
                    if (msg.Arguments.Count != 3) throw new FormatException("Only expected 2 arguments.");
                    progress?.Report(new MakeMkvProgress((int)(long)msg[0], (int)(long)msg[1]));
                } else
                {
                    yield return msg;
                }
            }
        }

        public List<DiscDrive>? ReadDrives() {
            try
            {
                var process = RunCommand("info", "disc");
                if (process == null)
                {
                    return null;
                }

                Task<List<MakeMkvMessage>> task = new(() =>
                {
                    return ParseOutput(process).ToBlockingEnumerable().ToList();
                });
                task.Start();
                task.Wait();
                process.WaitForExit();

                if (!task.IsCompletedSuccessfully)
                {
                    return null;
                }

                // Decode
                List<DiscDrive> drives = new();
                foreach(MakeMkvMessage msg in task.Result.Where(x => x.Type == MakeMkvMessageType.Drive))
                {
                    if (msg.Arguments.Count != 7) throw new FormatException("Unexpected number of args when parsing drive.");

                    DiscDrive info = new();
                    info.Index = (UInt32)(long)msg[0];
                    info.Visible = (UInt32)(long)msg[1];
                    info.Enabled = (UInt32)(long)msg[2];
                    info.Flags = (UInt32)(long)msg[3];
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
            }catch(Exception ex)
            {
                MessageBox.Show($"Failed to read MakeMKV: {ex.Message}", "Failed to read MakeMKV", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        } 

        public Disc? ReadDisc(int driveIndex, IProgress<MakeMkvProgress>? progress = null) {
            var process = RunCommand("--progress=-stdout", "info", $"disc:{driveIndex}");
            if (process == null)
            {
                return null;
            }

            Task<Disc> task = new(() => {
                Disc result = new();
                foreach(var msg in ParseOutput(process, progress).ToBlockingEnumerable())
                {
                    if (msg.Type == MakeMkvMessageType.Message)
                    {
                        Console.WriteLine(msg[3]);
                    } else if (msg.Type == MakeMkvMessageType.DiscInfo)
                    {
                        result.ParseMakeMkv(msg);
                    } else if (msg.Type == MakeMkvMessageType.TitleInfo)
                    {
                        Title.ParseMakeMkv(result, msg);
                    } else if (msg.Type == MakeMkvMessageType.StreamInfo)
                    {
                        Track.ParseMakeMkv(result, msg);
                    }
                }
                return result;
            });

            task.Start();
            while (!task.IsCompleted && !process.HasExited)
            {
                Application.DoEvents();
            }
            task.Wait();
            process.WaitForExit();

            if (!task.IsCompletedSuccessfully) return null;
            return task.Result;
        }

        public bool BackupDisc(int driveIndex, string outputFolder, IProgress<MakeMkvProgress>? progress = null) {
            var process = RunCommand("--progress=-stdout", "--decrypt", "--noscan", "mkv", $"disc:{driveIndex}", "all", Path.GetFullPath(outputFolder));
            if (process == null)
            {
                return false;
            }

            Task task = new(() => {
                foreach (var msg in ParseOutput(process, progress).ToBlockingEnumerable())
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine(msg);
                    Console.ResetColor();
                }
            });

            task.Start();
            while (!task.IsCompleted && !process.HasExited)
            {
                Application.DoEvents();
            }
            task.Wait();
            process.WaitForExit();

            return task.IsCompletedSuccessfully;
        }
    }
}
