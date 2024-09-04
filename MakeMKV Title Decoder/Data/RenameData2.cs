using JsonSerializable;
using MakeMKV_Title_Decoder.MkvToolNix.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.Data {
    public class RenameData2 : IJsonSerializable {
        public const string Version = "1.0";
        public SerializableDictionary<JsonString> UserGivenNames = new();

        public RenameData2() {

        }

        public void SetUserGivenName(MkvMergeID file, string? NewName) {
            string key = Path.Combine(file.FileDirectory, file.FileName);
            if (NewName == null)
            {
                this.UserGivenNames.Remove(key);
            } else
            {
                this.UserGivenNames[key] = new JsonString(NewName);
            }
        }

        public string? GetUserGivenName(MkvMergeID file) {
            string key = Path.Combine(file.FileDirectory, file.FileName);
            if (UserGivenNames.ContainsKey(key))
            {
                return UserGivenNames[key];
            } else
            {
                return null;
            }
        }

        public JsonData SaveToJson() {
            JsonObject obj = new();

            obj["version"] = new JsonString(Version);
            obj["User Names"] = UserGivenNames.SaveToJson();

            return obj;
        }

        public void LoadFromJson(JsonData data) {
            JsonObject obj = (JsonObject)data;

            string version = (JsonString)obj["version"];
            if (version != Version)
            {
                throw new Exception("Unable to read this version of the file. Please upgrade the file to a newer version.");
            }

            this.UserGivenNames.LoadFromJson(obj["User Names"]);
        }
    }
}
