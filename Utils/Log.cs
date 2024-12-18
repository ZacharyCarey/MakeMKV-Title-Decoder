using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils {
    public static class Log {
        public static void Info(string msg) {
            Console.WriteLine(msg);
        }
        public static void Warn(string msg) {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(msg);
            Console.ResetColor();
        }

        public static void Error(string msg) {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(msg);
            Console.ResetColor();
        }

        public static void ClearLogs() {
            try
            {
                string path = Path.GetFullPath("logs");
                Directory.Delete("logs", true);
            } catch (Exception) { }
        }

        private static string? CreateLogDirectory(string fullPath) {
            string path = Path.GetFullPath(fullPath);
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(fullPath);
                    Info("Created log directory at " + path);
                }
                return path;
            } catch (Exception)
            {
                Error("Failed to create log directory at " + path);
                return null;
            }
        }

        public static string? GetLogDirectory() {
            return CreateLogDirectory("logs");
        }

        public static string? GetLogDirectory(params string[] directory) {
            return CreateLogDirectory(Path.Combine("logs", Path.Combine(directory)));
        }
    }
}
