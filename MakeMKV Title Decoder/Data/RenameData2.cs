using JsonSerializable;
using MakeMKV_Title_Decoder.MkvToolNix.Data;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.Data {

    public class RenameData2 : IJsonSerializable {
        public const string Version = "1.0";
        public SerializableDictionary<UserGivenName> UserGivenNames = new();

        public RenameData2() {

        }

        private UserGivenName? TryGetUserNames(MkvMergeID file) {
            string key = Path.Combine(file.FileDirectory, file.FileName);
            if (this.UserGivenNames.ContainsKey(key))
            {
                return this.UserGivenNames[key];
            } else
            {
                return null;
            }
        }

        public void SetUserGivenName(MkvMergeID file, string? NewName) {
            string key = Path.Combine(file.FileDirectory, file.FileName);
            UserGivenName? obj = TryGetUserNames(file);
            if (NewName != null)
            {
                if (obj == null)
                {
                    obj = new UserGivenName();
                    this.UserGivenNames[key] = obj;
                }
                obj.Name = NewName;
            } else
            {
                if (obj != null)
                {
                    obj.Name = null;
                    if (obj.Reduce())
                    {
                        this.UserGivenNames.Remove(key);
                    }
                }
            }
        }

        public string? GetUserGivenName(MkvMergeID file) {
            return TryGetUserNames(file)?.Name;
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

    public class UserGivenName : IJsonSerializable {
        public string? Name = null;

        public bool Reduce() {
            return this.Name == null;
        }

        public void LoadFromJson(JsonData Data) {
            JsonObject obj = (JsonObject)Data;

            this.Name = (JsonString)obj["Name"];
        }

        public JsonData SaveToJson() {
            JsonObject obj = new();

            if (this.Name != null) obj["Name"] = new JsonString(this.Name);

            return obj;
        }
    }
}
