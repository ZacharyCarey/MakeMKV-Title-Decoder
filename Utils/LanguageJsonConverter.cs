using Iso639;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Utils {
    //, JsonConverter(typeof(LanguageJsonConverter))
    public class LanguageJsonConverter : JsonConverter<Language> {
        public override Language? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
            string? input = reader.GetString();

            if (input == null) return null;
            Language? result = Language.Database
                .Where(lang =>
                {
                    if (lang.Part1 != null && input.Equals(lang.Part1, StringComparison.OrdinalIgnoreCase)) return true;
                    if (lang.Part2 != null && input.Equals(lang.Part2, StringComparison.OrdinalIgnoreCase)) return true;
                    if (lang.Part2B != null && input.Equals(lang.Part2B, StringComparison.OrdinalIgnoreCase)) return true;
                    if (lang.Part3 != null && input.Equals(lang.Part3, StringComparison.OrdinalIgnoreCase)) return true;
                    return false;
                })
                .FirstOrDefault();

            if (result == null)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Failed to parse language code: '{input}'");
                Console.ResetColor();
            }

            if (result != null && result.Part2 == null)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Found language '{result.Name}' (part3 = {result.Part3}) but an ISO 639-2 code could not be found.");
                Console.ResetColor();
                result = null;
            }

            return result;
        }

        public override void Write(Utf8JsonWriter writer, Language value, JsonSerializerOptions options) {
            string? lang = value.Part2;
            if (lang == null) {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Language does not have a valid ISO 639-2 code. Writing null. (The ISO 639-3 code={value.Part3})");
                Console.ResetColor();
                writer.WriteNullValue();
            } else
            {
                writer.WriteStringValue(lang);
            }
        }
    }
}
