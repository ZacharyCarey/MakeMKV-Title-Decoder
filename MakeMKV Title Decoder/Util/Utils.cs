using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder {

    public static partial class Utils {

        public static void Append<T>(this StringBuilder sb, int tabs, string name, T? value) {
            if (value != null)
            {
                sb.Append('\t', tabs);
                sb.Append(name);
                sb.AppendLine(value.ToString());
            }
        }

        public static void Append<T>(this StringBuilder sb, int tabs, string name, T? value, Func<T, string> tostring) where T : struct{
            if (value != null)
            {
                sb.Append('\t', tabs);
                sb.Append(name);
                sb.AppendLine(tostring((T)value));
            }
        }

        public static void Append<T>(this StringBuilder sb, int tabs, string name, List<T>? value) {
            if (value != null)
            {
                sb.Append('\t', tabs);
                sb.Append(name);
                sb.Append('[');
                sb.Append(string.Join(", ", value));
                sb.AppendLine("]");
            }
        }

        public static string GetFileSafeName(string name) {
            char[] fileChars = Path.GetInvalidFileNameChars();
            char[] pathChars = Path.GetInvalidPathChars();
            for(int i = 0; i < name.Length; i++)
            {
                char c = name[i];
                if (fileChars.Contains(c) || pathChars.Contains(c))
                {
                    string prefix = "";
                    string posfix = "";
                    if (i > 0)
                    {
                        prefix = name.Substring(0, i);
                    }
                    if (i < name.Length)
                    {
                        posfix = name.Substring(i + 1);
                    }

                    name = prefix + " " + posfix;
                }
            }

            return name;
        }

    }
}
