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
            return Language.FromPart2(input);
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
