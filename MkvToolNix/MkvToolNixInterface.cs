using CLI_Wrapper;
using MkvToolNix.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Utils;

namespace MkvToolNix {

    // TODO use Utils.CommandLineInterface????
    public static class MkvToolNixInterface {

        internal static CLI? GetCLI() {
            string? path = FileUtils.SearchLocalExeFiles(Path.Combine("lib", "mkvmerge.exe"));
            if (path == null) return null;

            return CLI.RunExeFile(path);
        }

        /// <exception cref="Exception" />
        public static MkvMergeID? Identify(string root, string directory, string fileName) {
            CLI? cli = GetCLI();
            if (cli == null) return null;
            cli.AddArguments("--identify", "--identification-format json", $"\"{Path.Combine(root, directory, fileName)}\"");
            var result = cli.Run();

            if (result.Exception != null || result.ExitCode != 0)
            {
                return null;
            }
            MkvMergeID? data = MkvMergeID.TryParse(result.OutputData);
            if (data != null)
            {
                data.SetFile(root, directory, fileName);
                return data;
            } else
            {
                return null;
            }
        }

        public static Exception? Merge(MergeData playlist, string outputFile, IProgress<SimpleProgress>? progress = null, SimpleProgress? totalProgress = null) {
            //string args = $"--title \"Test Title\" -o \"{outputFile}\" --track-name 5:\"Director Commentary\" {Path.Combine(disc.RootPath, "BDMV", "STREAM", "00001.m2ts")}";
            IEnumerable<string> args = playlist.GetCommand(outputFile);
            JsonArray json = [.. args];

            string tempFilePath = "";
            Stream? tempFile = null;
            try
            {
                tempFilePath = Path.GetTempFileName();
                tempFile = System.IO.File.OpenWrite(tempFilePath);
                JsonSerializer.Serialize(tempFile, json);
                tempFile.Close();
                tempFile = null;
            } catch (Exception ex)
            {
                try
                {
                    if (tempFile != null) tempFile.Close();
                    System.IO.File.Delete(tempFilePath);
                } catch (Exception) { }

                return ex;
            }

            TestData? result = RunMkvMerge(progress, tempFilePath, totalProgress);

            try
            {
                System.IO.File.Delete(tempFilePath);
            } catch (Exception)
            {
            }

            if (result == null || result.Errors.Count > 0 || !result.FoundSuccess)
            {
                return new Exception("Error occured while exporting.");
            }

            return null;
        }

        private static TestData? RunMkvMerge(IProgress<SimpleProgress>? progress, string argFilePath, SimpleProgress? totalProgress) {
            CLI? cli = GetCLI();
            if (cli == null) return null;

            cli.AddArgument($"@\"{argFilePath}\"");
            cli.OutputDataReceived += (object? sender, string line) => TestData.ParseProgress(line, progress, totalProgress);
            var result = cli.Run();

            if (result.Exception != null || result.ExitCode != 0)
            {
                return null;
            }

            return TestData.TryParse(result.OutputData);
        }
    }

    // TODO rename to MultiplexData? MergeData?
    internal class TestData
    {

        public List<string> Errors = new();
        public bool FoundSuccess = false;

        public static void ParseProgress(string line, IProgress<SimpleProgress>? progress, SimpleProgress? baseProgress) {
            if (progress != null && line.StartsWith("Progress: ") && line.EndsWith("%"))
            {
                string progText = line.Substring("Progress: ".Length).TrimEnd('%');

                uint prog;
                if (uint.TryParse(progText, out prog))
                {
                    if (baseProgress != null)
                    {
                        progress.Report(new SimpleProgress(
                            prog,
                            100,
                            baseProgress.Value.Total * 100 + prog,
                            baseProgress.Value.TotalMax * 100
                        ));
                    } else
                    {
                        progress.Report(new SimpleProgress(prog, 100));
                    }
                }
            }
        }

        public static TestData? TryParse(IEnumerable<string> std)
        {
            TestData result = new();

            Console.ForegroundColor = ConsoleColor.Cyan;
            foreach (var str in std)
            {
                Console.WriteLine(str);
                if (str.StartsWith("Error: "))
                {
                    result.Errors.Add(str.Substring("Error: ".Length));
                }
                if (str.StartsWith("Multiplexing took"))
                {
                    result.FoundSuccess = true;
                }
            }

            if (result.Errors.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                foreach (var err in result.Errors)
                {
                    Console.WriteLine(err);
                }
            }
            else if (result.FoundSuccess != true)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("No errors detected, but could not tell if operation was successfull.");
            }

            Console.ResetColor();
            return result;
        }
    }
}
