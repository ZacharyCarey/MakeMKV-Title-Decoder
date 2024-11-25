using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
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

        /// <summary>
        /// Returns the index and value of every invalid character.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static IEnumerable<(int Index, char Value)> GetInvalidFileNameChars(string fileName) {
            char[] invalidChars = { '<', '>', ':', '/', '\\', '|', '?', '*', '\"' };
            for (int i = 0; i < fileName.Length; i++)
            {
                char c = fileName[i];
                bool isValid = true;
                if (!char.IsAscii(c))
                {
                    isValid = false;
                } else if ((int)c < 32)
                {
                    isValid = false;
                } else if (invalidChars.Contains(c))
                {
                    isValid = false;
                } else if (i == fileName.Length - 1 && (c == '.' || c == ' '))
                {
                    isValid = false;
                }

                if (!isValid)
                {
                    yield return (i, c);
                }
            }
        }

        /// <summary>
        /// Returns null if the name is valid.
        /// Returns the error message if the name is not valid.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string? IsValidFileName(string fileName)
        {
            int dummy;
            if (fileName.StartsWith("COM") && int.TryParse(fileName[3..], out dummy))
            {
                return "File named a COM port.";
            }
            if (fileName.StartsWith("LPT") && int.TryParse(fileName[3..], out dummy))
            {
                return "File names a LPT port.";
            }
            if (fileName.Where(c => c != '.').Any() == false) // Filename is all periods
            {
                return "File name can not be all periods.";
            }

            string[] dosNames = { "CON", "PRN", "AUX", "NUL" };
            if (dosNames.Contains(fileName))
            {
                return "File name can not be a protected DOS name";
            }

            foreach ((int index, char value) in GetInvalidFileNameChars(fileName))
            {
                return $"Invalid character '{value}' at position {index}";
            }

            return null;
        }

        public static string GetFileSafeName(string name) {
            char[] mutable = name.ToCharArray();
            foreach ((int index, char value) in GetInvalidFileNameChars(name))
            {
                mutable[index] = '_';
            }
            name = new string(mutable);

            if (IsValidFileName(name) != null)
            {
                name = name + "_";
                if (IsValidFileName(name) != null)
                {
                    throw new Exception("Failed to create safe file name for unknown reasons.");
                }
            }

            return name;
        }

    }
}
