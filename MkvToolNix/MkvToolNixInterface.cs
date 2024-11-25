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

namespace MkvToolNix
{
    internal interface MkvToolNixData<TSelf>
    {
        abstract static TSelf? TryParse(IEnumerable<string> std, IProgress<SimpleProgress>? progress = null, object? tag = null);
    }


    // TODO use Utils.CommandLineInterface????
    public static class MkvToolNixInterface
    {

        /// <exception cref="Exception" />
        private static Process? RunCommandForceArg(string exeName, string args)
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase).Substring(6);

            var startInfo = new ProcessStartInfo();
            startInfo.FileName = Path.Combine(path, "lib", exeName);
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.Arguments = args;

            return Process.Start(startInfo);
        }

        /// <exception cref="Exception" />
        private static Process? RunCommand(string exeName, params string[] args)
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase).Substring(6);

            var startInfo = new ProcessStartInfo();
            startInfo.FileName = Path.Combine(path, "lib", exeName);
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;

            foreach (string arg in args)
            {
                startInfo.ArgumentList.Add(arg);
            }

            return Process.Start(startInfo);
        }

        private static IEnumerable<string> ReadAllStdOut(Process process)
        {
            string? line = process.StandardOutput.ReadLine();
            while (line != null)
            {
                yield return line;
                line = process.StandardOutput.ReadLine();
            }
        }

        // TODO just pass the process as a parameter
        /// <exception cref="Exception" />
        private static T? ParseCommandForceArgs<T>( IProgress<SimpleProgress>? progress, string exeName, string args, object? tag) where T : class, MkvToolNixData<T>
        {
            var process = RunCommandForceArg(exeName, args);
            if (process == null)
            {
                return null;
            }

            T? result = T.TryParse(ReadAllStdOut(process), progress, tag);
            process.WaitForExit();
            return result;
        }

        /// <exception cref="Exception" />
        private static T? ParseCommand<T>(IProgress<SimpleProgress>? progress, string exeName, params string[] args) where T : class, MkvToolNixData<T>
        {
            var process = RunCommand(exeName, args);
            if (process == null)
            {
                return null;
            }

            T? result = T.TryParse(ReadAllStdOut(process), progress);
            process.WaitForExit();
            return result;
        }

        /// <exception cref="Exception" />
        public static MkvMergeID? Identify(string root, string directory, string fileName)
        {
            MkvMergeID? result = ParseCommand<MkvMergeID>(null, "mkvmerge.exe", "--identify", "--identification-format", "json", Path.Combine(root, directory, fileName));
            if (result != null)
            {
                result.SetFile(root, directory, fileName);
                return result;
            }
            else
            {
                return null;
            }
        }

        public static Exception? Merge(MergeData playlist, string outputFile, IProgress<SimpleProgress>? progress = null, SimpleProgress? totalProgress = null)
        {
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
            } catch(Exception ex)
            {
                try
                {
                    if (tempFile != null) tempFile.Close();
                    System.IO.File.Delete(tempFilePath);
                }
                catch (Exception) { }

                return ex;
            }

            TestData? result = ParseCommandForceArgs<TestData>(progress, "mkvmerge.exe", $"@{tempFilePath}", totalProgress);

            try
            {
                System.IO.File.Delete(tempFilePath);
            }catch(Exception) { 
            }

            return null;
        }

        
    }

    // TODO rename to MultiplexData? MergeData?
    public class TestData : MkvToolNixData<TestData>
    {

        public List<string> Errors = new();
        public bool FoundSuccess = false;

        public static TestData? TryParse(IEnumerable<string> std, IProgress<SimpleProgress>? progress = null, object? tag = null)
        {
            TestData result = new();

            SimpleProgress? baseProgress = null;
            if (tag != null && tag is SimpleProgress p)
            {
                baseProgress = p;
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            foreach (var str in std)
            {
                Console.WriteLine(str);

                if (progress != null && str.StartsWith("Progress: ") && str.EndsWith("%"))
                {
                    string progText = str.Substring("Progress: ".Length).TrimEnd('%');

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
                        }
                        else
                        {
                            progress.Report(new SimpleProgress(prog, 100));
                        }
                    }
                }
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
