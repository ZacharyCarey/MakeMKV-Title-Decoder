using MakeMKV_Title_Decoder.Data;
using MakeMKV_Title_Decoder.MkvToolNix.Data;
using MakeMKV_Title_Decoder.MkvToolNix.MkvToolNix;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.MkvToolNix
{
    internal interface MkvToolNixData {
        bool Parse(IEnumerable<string> std, IProgress<SimpleProgress>? progress = null);
    }


    // TODO use Utils.CommandLineInterface????
    internal static class MkvToolNixInterface {

        private static Process? RunCommandForceArg(string exeName, string args) {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase).Substring(6);

            try
            {
                var startInfo = new ProcessStartInfo();
                startInfo.FileName = Path.Combine(path, "lib", "mkvtoolnix", exeName);
                startInfo.CreateNoWindow = true;
                startInfo.UseShellExecute = false;
                startInfo.RedirectStandardOutput = true;
                startInfo.Arguments = args;

                return Process.Start(startInfo);
            } catch (Exception ex)
            {
                MessageBox.Show($"Failed to read MakeMKV: {ex.Message}", "Failed to read MakeMKV", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        private static Process? RunCommand(string exeName, params string[] args) {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase).Substring(6);

            try
            {
                var startInfo = new ProcessStartInfo();
                startInfo.FileName = Path.Combine(path, "lib", "mkvtoolnix", exeName);
                startInfo.CreateNoWindow = true;
                startInfo.UseShellExecute = false;
                startInfo.RedirectStandardOutput = true;

                foreach (string arg in args)
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

        private static IEnumerable<string> ReadAllStdOut(Process process) {
            string? line = process.StandardOutput.ReadLine();
            while(line != null)
            {
                yield return line;
                line = process.StandardOutput.ReadLine();
            }
        }

        // TODO just pass the process as a parameter
        private static bool ParseCommandForceArgs<T>(T data, IProgress<SimpleProgress>? progress, string exeName, string args) where T : class, MkvToolNixData {
            var process = RunCommandForceArg(exeName, args);
            if (process == null)
            {
                return false;
            }

            try
            {
                bool result = data.Parse(ReadAllStdOut(process), progress);
                process.WaitForExit();
                return result;
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        private static bool ParseCommand<T>(T data, IProgress<SimpleProgress>? progress, string exeName, params string[] args) where T : class, MkvToolNixData {
            var process = RunCommand(exeName, args);
            if(process == null)
            {
                return false;
            }

            try
            {
                bool result = data.Parse(ReadAllStdOut(process), progress);
                process.WaitForExit();
                return result;
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public static MkvInfo? ReadInfo(string filePath) {
            return null; //ParseCommand<MkvInfo>("mkvinfo.exe", filePath);
        }

        public static MkvMergeID? Identify(string root, string directory, string fileName) {
            MkvMergeID data = new MkvMergeID(root, directory, fileName);
            bool result = ParseCommand<MkvMergeID>(data, null, "mkvmerge.exe", "--identify", "--identification-format", "json", Path.Combine(root, directory, fileName));
            if (result)
            {
                return data;
            } else
            {
                return null;
            }
        }

        public static void Merge(MkvToolNixDisc disc, Playlist playlist, string outputFile, IProgress<SimpleProgress>? progress = null) {
            TestData data = new();

            // TRailer only
            /*bool result = ParseCommand<TestData>(
                data, 
                progress,
                "mkvmerge.exe", 
                "-o", outputFile, 
                "--title", "Test Title", 
                Path.Combine(disc.RootPath, "BDMV", "STREAM", "00338.m2ts")
            );*/

            // Main feature + credits
            string args = $"--title \"Test Title\" -o \"{outputFile}\" --track-name 5:\"Director Commentary\" {Path.Combine(disc.RootPath, "BDMV", "STREAM", "00001.m2ts")}";
            bool result = ParseCommandForceArgs<TestData>(data, progress, "mkvmerge.exe", args);
            
        }

        public static void MergeAsync(MkvToolNixDisc disc, Playlist playlist, string outputFile) {
            var progressForm = new TaskProgressViewer<Task, SimpleProgress>(
                (IProgress<SimpleProgress> progress) =>
                {
                    return Task.Run(() => Merge(disc, playlist, outputFile, progress));
                }
            );
            progressForm.ShowDialog();
        }
    }

    public class TestData : MkvToolNixData {
        public bool Parse(IEnumerable<string> std, IProgress<SimpleProgress>? progress = null) {
            Console.ForegroundColor = ConsoleColor.Cyan;
            foreach(var str in std)
            {
                Console.WriteLine(str);

                if (progress != null && str.StartsWith("Progress: ") && str.EndsWith("%"))
                {
                    string progText = str.Substring("Progress: ".Length).TrimEnd('%');

                    uint prog;
                    if (uint.TryParse(progText, out prog))
                    {
                        progress.Report(new SimpleProgress(prog, 100));
                    }
                }
            }
            Console.ResetColor();
            return true;
        }
    }
}
