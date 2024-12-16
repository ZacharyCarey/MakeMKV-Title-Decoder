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
            Console.WriteLine(msg);
        }
    }
}
