using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.Util {
    public abstract class CommandLineInterface
    {

        protected string ExePath { get; }
        public abstract string ProgramName { get; }

        protected CommandLineInterface(string exePath) {
            this.ExePath = exePath;
        }

        protected static IEnumerable<string> SearchProgramFiles(string folderName, string? exeName) {
            Environment.SpecialFolder[] searchLocations = {
                Environment.SpecialFolder.CommonProgramFiles,
                Environment.SpecialFolder.CommonProgramFilesX86,
                Environment.SpecialFolder.ProgramFiles,
                Environment.SpecialFolder.ProgramFilesX86
            };

            HashSet<string> searchedPaths = new();

            foreach(var specialFolderType in searchLocations)
            {
                string? folder = null;
                try
                {
                    folder = Environment.GetFolderPath(specialFolderType);
                    if (searchedPaths.Contains(folder))
                    {
                        continue;
                    }
                    searchedPaths.Add(folder);

                    if (!Directory.Exists(folder))
                    {
                        continue;
                    }

                    folder = Path.Combine(folder, folderName);
                    if (!Directory.Exists(folder))
                    {
                        continue;
                    }

                    if (exeName != null)
                    {
                        folder = Path.Combine(folder, exeName);
                        if (!File.Exists(folder))
                        {
                            continue;
                        }
                    }

                    //yield return folder;
                } catch (Exception) {
                    //folder = null;
                    continue;
                }

                yield return folder;
            }
        }

        private Process StartProcess(params string[] args) {
            var startInfo = new ProcessStartInfo();
            startInfo.FileName = this.ExePath;
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;

            foreach (string arg in args)
            {
                startInfo.ArgumentList.Add(arg);
            }

            var process = Process.Start(startInfo);
            if (process == null)
            {
                throw new Exception("Failed to start process.");
            }
            return process;
        }

        protected void RunCommand(params string[] args) {
            var process = StartProcess(args);
            process.WaitForExit();
        }

        protected IEnumerable<MessageType> RunCommand<MessageType>(params string[] args)
            where MessageType : IParsable<MessageType> 
        {
            var process = StartProcess(args);

            while(true)
            {
                string? line;
                try
                {
                    line = process.StandardOutput.ReadLine();
                }catch(Exception ex)
                {
                    process.Close();
                    throw new Exception("Failed to read input.", ex);
                }

                if (line == null) break; // End of program/stream

                MessageType message;
                try
                {
                    message = MessageType.Parse(line, null);
                } catch(Exception ex)
                {
                    process.Close();
                    throw new Exception("Failed to parse message.", ex);
                }

                yield return message;
            }

            try
            {
                process.WaitForExit();
            } catch (Exception) {
                process.Close();
            }
        }
    }
}
