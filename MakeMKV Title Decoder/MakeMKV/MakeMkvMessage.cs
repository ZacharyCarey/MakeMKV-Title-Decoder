using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.MakeMKV {

    public enum MakeMkvMessageType {
        Message,
        ProgressCurrent,
        ProgressTotal,
        ProgressValues,
        Drive,
        TitleCount,
        DiscInfo,
        TitleInfo,
        StreamInfo
    }

    public struct MakeMkvMessage {
        public MakeMkvMessageType Type;
        public List<object> Arguments;

        public MakeMkvMessage(MakeMkvMessageType type) {
            this.Type = type;
            this.Arguments = new();
        }

        public object this[int index] {
            get => Arguments[index];
        }


        /// <exception cref="FormatException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static MakeMkvMessage Parse(string input) {
            input = input.Trim();
            int divider = input.IndexOf(':');
            string key = input.Substring(0, divider);
            string value = input.Substring(divider + 1);
            MakeMkvMessage result = new(ParseMessageType(key));

            int i = 0;
            if (value[i] == '"') result.Arguments.Add(ParseString(ref i, value));
            else result.Arguments.Add(ParseNumber(ref i, value));

            while(i < value.Length)
            {
                if (value[i] != ',') 
                    throw new FormatException("Expected separator.");
                i++;
                if (i >= value.Length) 
                    throw new FormatException("Unexpected end of string.");

                if (value[i] == '"') result.Arguments.Add(ParseString(ref i, value));
                else result.Arguments.Add(ParseNumber(ref i, value));
            }

            return result;
        }

        /// <exception cref="FormatException"></exception>
        private static long ParseNumber(ref int i, string value) {
            char c = value[i];
            long n;
            if (c == '0')
            {
                i++;
                return 0;
            } else if ( c >= '1' && c <= '9')
            {
                n = (c - '0');
                i++;
            } else
            {
                throw new FormatException("Excpected a number");
            }

            for(; i < value.Length; i++)
            {
                c = value[i];
                if (c >= '0' && c <= '9')
                {
                    n = (n * 10) + (c - '0');
                } else
                {
                    // Found end of number
                    return n;
                }
            }

            // End of string
            return n;
        }

        /// <exception cref="FormatException"></exception>
        private static string ParseString(ref int i, string value) {
            if (value[i] != '"') throw new FormatException("Expected string");

            int startIndex = i + 1;
            for(i++; i < value.Length; i++)
            {
                char c = value[i];
                if (c == '"')
                {
                    string result = value.Substring(startIndex, i - startIndex);
                    i++;
                    return result;
                }
            }

            throw new FormatException("Unexpected end of string.");
        }

        /// <exception cref="FormatException"></exception>
        private static MakeMkvMessageType ParseMessageType(string str) {
            switch(str)
            {
                case "MSG": return MakeMkvMessageType.Message;
                case "PRGC": return MakeMkvMessageType.ProgressCurrent;
                case "PRGT": return MakeMkvMessageType.ProgressTotal;
                case "PRGV": return MakeMkvMessageType.ProgressValues;
                case "DRV": return MakeMkvMessageType.Drive;
                case "TCOUNT": return MakeMkvMessageType.TitleCount;
                case "CINFO": return MakeMkvMessageType.DiscInfo;
                case "TINFO": return MakeMkvMessageType.TitleInfo;
                case "SINFO": return MakeMkvMessageType.StreamInfo;
                default:
                    throw new FormatException("Unknown message type.");
            }
        }

        private static string GetShortenedMessageType(MakeMkvMessageType type) {
            switch (type)
            {
                case MakeMkvMessageType.Message: return "MSG";
                case MakeMkvMessageType.ProgressCurrent: return "PRGC";
                case MakeMkvMessageType.ProgressTotal: return "PRGT";
                case MakeMkvMessageType.ProgressValues: return "PRGV";
                case MakeMkvMessageType.Drive: return "DRV";
                case MakeMkvMessageType.TitleCount: return "TCOUNT";
                case MakeMkvMessageType.DiscInfo: return "CINFO";
                case MakeMkvMessageType.TitleInfo: return "TINFO";
                case MakeMkvMessageType.StreamInfo: return "SINFO";
                default:
                    throw new Exception("Unknown message type.");
            }
        }

        public static bool operator ==(MakeMkvMessage left, MakeMkvMessage right) {
            return (left.Type == right.Type) && left.Arguments.SequenceEqual(right.Arguments);
        }

        public static bool operator !=(MakeMkvMessage left, MakeMkvMessage right) {
            return !(left == right);
        }

        public override bool Equals(object? obj) {
            if (obj == null)
            {
                return false;
            } else if (obj is MakeMkvMessage other)
            {
                return this == other;
            } else
            {
                return false;
            }
        }

        public override int GetHashCode() {
            return ((int)this.Type + this.Arguments.Count).GetHashCode();
        }

        public override string ToString() {
            return $"{GetShortenedMessageType(this.Type)}:{string.Join(", ", this.Arguments.Select(x =>
            {
                if (x is string str)
                {
                    return $"\"{str}\"";
                } else
                {
                    return x.ToString();
                }
            }))}";
        }
    }
}
