using MakeMKV_Title_Decoder.MkvToolNix.Data;
using MakeMKV_Title_Decoder.MkvToolNix.MkvToolNix;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.MkvToolNix
{
    internal interface MkvToolNixData<TSelf> where TSelf : class {
        abstract static TSelf? Parse(IEnumerable<string> std);
    }

    internal static class MkvToolNixInterface {

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

        private static T? ParseCommand<T>(string exeName, params string[] args) where T : class, MkvToolNixData<T> {
            var process = RunCommand(exeName, args);
            if(process == null)
            {
                return null;
            }

            try
            {
                T? result = T.Parse(ReadAllStdOut(process));
                process.WaitForExit();
                return result;
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public static MkvInfo? ReadInfo(string filePath) {
            return null; //ParseCommand<MkvInfo>("mkvinfo.exe", filePath);
        }

        public static MkvMergeID? Identify(string filePath) {
            return ParseCommand<MkvMergeID>("mkvmerge.exe", "--identify", "--identification-format", "json", filePath);
        }
    }
}
