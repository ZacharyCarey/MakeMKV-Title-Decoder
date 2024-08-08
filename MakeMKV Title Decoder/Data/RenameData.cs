using JsonSerializable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.Data {
    public class RenameData : IJsonSerializable {
        public const string Version = "1.1";
        public string OutputFolder = "";
        public SerializableDictionary<JsonString> Renamed = new();
        public SerializableList<JsonString> Deleted = new();
        public SerializableList<JsonString> Errors = new();

        public RenameData() {

        }

        public JsonData SaveToJson() {
            JsonObject obj = new();

            obj["version"] = new JsonString(Version);
            obj["Output Folder"] = new JsonString(OutputFolder);
            obj["Renamed"] = Renamed.SaveToJson();
            obj["Deleted"] = Deleted.SaveToJson();
            obj["Errors"] = Errors.SaveToJson();

            return obj;
        }

        public void LoadFromJson(JsonData data) {
            JsonObject obj = (JsonObject)data;

            string version = (JsonString)obj["version"];
            if (version != Version)
            {
                throw new Exception("Unable to read this version of the file. Please upgrade the file to a newer version.");
            }

            this.OutputFolder = (JsonString)obj["Output Folder"];
            this.Renamed.LoadFromJson(obj["Renamed"]);
            this.Deleted.LoadFromJson(obj["Deleted"]);
            this.Errors.LoadFromJson(obj["Errors"]);
        }
    }
}
