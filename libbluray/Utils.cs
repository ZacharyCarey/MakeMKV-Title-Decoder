using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libbluray {

    public enum LogLevel {
        Info,
        Warning,
        Critical
    }

    public static class Utils {

        public static void BD_DEBUG(string module, string msg) {
            BD_DEBUG(LogLevel.Info, module, msg);
        }

        public static void BD_DEBUG(LogLevel level, string module, string msg) {
            switch (level)
            {
                case LogLevel.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogLevel.Critical:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
            }
            Console.WriteLine($"[{module}] {msg}");
            Console.ResetColor();
        }

        public static string print_hex(this byte[] buf, out string result) {
            return print_hex(out result, buf);
        }

        public static string print_hex(this Span<byte> buf, out string result) {
            return print_hex(out result, buf);
        }

        public static string print_hex(this ReadOnlySpan<byte> buf, out string result) {
            return print_hex(out result, buf);
        }

        public static string print_hex(out string result, ReadOnlySpan<byte> buf) {
            const string nibble = "0123456789abcdef";
            char[] arr = new char[buf.Length * 2];
            int zz;
            for(zz = 0; zz < buf.Length; zz++)
            {
                arr[zz * 2] = nibble[(buf[zz] >> 4) & 0x0F];
                arr[zz * 2 + 1] = nibble[buf[zz] & 0x0F];
            }

            result = new string(arr);
            return result;
        }

    }
}
