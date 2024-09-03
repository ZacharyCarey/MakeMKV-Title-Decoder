using JsonSerializable;
using MakeMKV_Title_Decoder.MkvToolNix.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.MkvToolNix {
    internal static class MkvToolNixUtils {

        public static void LoadFromJson(this List<string> list, JsonData data) {
            list.Clear();
            if (data != null)
            {
                var arr = (JsonArray)data;
                foreach (var item in arr)
                {
                    if (item == null)
                    {
                        list.Add(null);
                    } else if (item is JsonString str)
                    {
                        list.Add(str);
                    } else
                    {
                        throw new InvalidCastException();
                    }
                }
            }
        }

        public static void LoadFromJson(this List<long> list, JsonData data) {
            list.Clear();
            if (data != null)
            {
                var arr = (JsonArray)data;
                foreach (var item in arr)
                {
                    if (item is JsonInteger num)
                    {
                        list.Add(num);
                    } else
                    {
                        throw new InvalidCastException();
                    }
                }
            }
        }

        public static void LoadFromJson<T>(this List<T> list, JsonData data) where T : IJsonSerializable, new() {
            list.Clear();
            if (data != null)
            {
                var arr = (JsonArray)data;
                foreach(var item in arr)
                {
                    T result = new T();
                    result.LoadFromJson(item);
                    list.Add(result);
                }
            }
        }

        public static T? ParseOptional<T>(JsonData data) where T : JsonData {
            if (data == null)
            {
                return (T?)null;
            } else if (data is T)
            {
                return (T)data;
            } else
            {
                throw new InvalidCastException();
            }
        }

        public static T? ParseOptionalObject<T>(JsonData data) where T : class, IJsonSerializable, new() {
            if (data == null)
            {
                return (T?)null;
            } else
            {
                T result = new();
                result.LoadFromJson(data);
                return result;
            }
        }

        public static T? ParseEnum<T>(JsonData data, T defaultValue) where T : struct, Enum {
            if (data == null)
            {
                return null;
            }

            long val = (JsonInteger)data;
            if (Enum.IsDefined(typeof(T), (int)val))
            {
                return (T)Enum.ToObject(typeof(T), val);
            } else
            {
                return defaultValue;
            }
        }

        public static AacIsSbr? ParseEnumName(JsonData data, AacIsSbr defaultValue) {
            if (data == null)
            {
                return null;
            }

            switch ((string)(JsonString)data)
            {
                case "true": return AacIsSbr.True;
                case "false": return AacIsSbr.False;
                case "unknown": return AacIsSbr.Unknown;
                default:
                    return defaultValue;
            }
        }

        public static MkvTrackType ParseEnumName(JsonData data, MkvTrackType defaultValue) {
            if (data == null)
            {
                return defaultValue;
            }

            switch((string)(JsonString)data)
            {
                case "video": return MkvTrackType.Video;
                case "audio": return MkvTrackType.Audio;
                case "subtitles": return MkvTrackType.Subtitles;
                default:
                    return defaultValue;
            }
        }
    }
}
