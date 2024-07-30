using JsonSerializable;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder {

    public struct ValueWithIndex<T> {
        public int Index;
        public T Value;

        public ValueWithIndex(int index, T value) {
            Index = index;
            Value = value;
        }
    }

    public static class Utils {
        #region LINQ
        public static IEnumerable<ValueWithIndex<T>> WithIndex<T>(this IEnumerable<T> values) {
            int index = 0;
            foreach(T value in values)
            {
                yield return new ValueWithIndex<T>(index, value);
                index++;
            }
        }
        #endregion

        #region LoadFromJson
        public static void LoadFromJson(this JsonData? data, out string? result) {
            if (data == null)
            {
                result = null;
                return;
            }

            result = (JsonString)data;
        }
        public static void LoadFromJson(this JsonData? data, out int? result) {
            if (data == null)
            {
                result = null;
                return;
            }

            result = (int)(JsonInteger)data;
        }
        public static void LoadSerializableFromJson<T>(this JsonData? data, out T? result) where T : struct, IJsonSerializable {
            if (data == null)
            {
                result = null;
                return;
            }

            result = new T();
            result.Value.LoadFromJson(data);
        }
        public static void LoadFromJson<T>(this JsonData? data, out T? result) where T : struct, IParsable<T> {
            if (data == null)
            {
                result = null;
                return;
            }

            result = T.Parse((JsonString)data, CultureInfo.CurrentCulture);
        }
        public static void LoadFromJson<T>(this JsonData? data, out T? result, Func<string, T> parser) where T : struct {
            if (data == null)
            {
                result = null;
                return;
            }

            result = parser((JsonString)data);
        }
        public static void LoadFromJson<T>(this JsonData? data, out T? result, Func<JsonData, T> parser) where T : struct {
            if (data == null)
            {
                result = null;
                return;
            }

            result = parser(data);
        }
        public static void LoadFromJson<T>(this JsonData? data, out List<T>? result, Func<JsonData, T> parser) {
            if (data == null)
            {
                result = null;
                return;
            }

            result = new List<T>(((JsonArray)data).Select(parser));
        }
        #endregion

        #region SaveToJson
        public static void SaveToJson(this JsonObject obj, string key, int? value) {
            if (value != null)
            {
                obj[key] = new JsonInteger((long)value);
            }
        }
        public static void SaveToJson<T>(this JsonObject obj, string key, IEnumerable<T>? value) where T : JsonData {
            if (value != null)
            {
                obj[key] = new JsonArray(value);
            }
        }
        public static void SaveToJson(this JsonObject obj, string key, IJsonSerializable? value) {
            if (value != null)
            {
                obj[key] = value.SaveToJson();
            }
        }
        public static void SaveToJson<T>(this JsonObject obj, string key, T? value) {
            if (value != null)
            {
                obj[key] = new JsonString(value.ToString());
            }
        }
        public static void SaveToJson<T>(this JsonObject obj, string key, T? value, Func<T, string> tostring) where T : struct {
            if (value != null)
            {
                obj[key] = new JsonString(tostring((T)value));
            }
        }
        public static void SaveToJson<T>(this JsonObject obj, string key, T? value, Func<T, JsonData> tojson) where T : struct {
            if (value != null)
            {
                obj[key] = tojson((T)value);
            }
        }
        #endregion

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
    }
}
