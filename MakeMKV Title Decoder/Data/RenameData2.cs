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
        public SerializableDictionary<ClipRename> ClipRenames = new();

        public RenameData2() {

        }

        public ClipRename? GetClipRename(MkvMergeID file, bool createNewIfNeeded = false) {
            string key = Path.Combine(file.FileDirectory, file.FileName);
            if (this.ClipRenames.ContainsKey(key))
            {
                return this.ClipRenames[key];
            } else
            {
                if (createNewIfNeeded)
                {
                    ClipRename result = new();
                    this.ClipRenames[key] = result;
                    return result;
                } else
                {
                    return null;
                }
            }
        }

        public JsonData SaveToJson() {
            JsonObject obj = new();

            obj["version"] = new JsonString(Version);

            JsonObject clips = new();
            foreach (var pair in this.ClipRenames)
            {
                var json = pair.Value.SaveToJson();
                if (json != null) clips[pair.Key] = json;
            }
            if (clips.Items.Count() > 0) obj["User Names"] = clips;

            return obj;
        }

        public void LoadFromJson(JsonData data) {
            JsonObject obj = (JsonObject)data;

            string version = (JsonString)obj["version"];
            if (version != Version)
            {
                throw new Exception("Unable to read this version of the file. Please upgrade the file to a newer version.");
            }

            this.ClipRenames.LoadFromJson(obj["User Names"] ?? new JsonObject());
        }
    }

    public class ClipRename : IJsonSerializable {
        public string? Name { get; set; } = null;
        public SerializableDictionary<TrackRename> TrackRenames = new();

        public void SetName(string? NewName) {
            this.Name = NewName;
        }

        public TrackRename? GetTrackRename(MkvTrack track, bool createNewIfNeeded = false) {
            string key = track.ID.ToString();
            if (this.TrackRenames.ContainsKey(key))
            {
                return this.TrackRenames[key];
            } else
            {
                if (createNewIfNeeded)
                {
                    TrackRename result = new();
                    this.TrackRenames[key] = result;
                    return result;
                } else
                {
                    return null;
                }
            }
        }


        public void LoadFromJson(JsonData Data) {
            JsonObject obj = (JsonObject)Data;

            this.Name = (JsonString?)obj["Name"];
            this.TrackRenames.LoadFromJson(obj["Tracks"] ?? new JsonObject());
        }

        public JsonData SaveToJson() {
            JsonObject obj = new();

            if (this.Name != null) obj["Name"] = new JsonString(this.Name);

            JsonObject tracks = new();
            foreach(var pair in this.TrackRenames)
            {
                var json = pair.Value.SaveToJson();
                if (json != null) tracks[pair.Key] = json;
            }
            if (tracks.Items.Count() > 0) obj["Tracks"] = tracks;

            if (obj.Items.Count() == 0)
            {
                return null;
            } else
            {
                return obj;
            }
        }
    }

    public class TrackRename : IJsonSerializable {

        public string? Name = null;
        public bool? CommentaryFlag = null;
        public bool? DefaultFlag = null;

        public void SetName(string? NewName) {
            this.Name = NewName;
        }

        public void SetCommentaryFlag(bool? val) {
            this.CommentaryFlag = val;
        }

        public void SetDefaultFlag(bool? val) {
            this.DefaultFlag = val;
        }

        public void LoadFromJson(JsonData Data) {
            JsonObject obj = (JsonObject)Data;

            this.Name = (JsonString?)obj["Name"];
            this.CommentaryFlag = (JsonBool?)obj["Commentary Flag"];
            this.DefaultFlag = (JsonBool?)obj["Default Track Flag"];
        }

        public JsonData SaveToJson() {
            JsonObject obj = new();

            if (this.Name != null) obj["Name"] = new JsonString(this.Name);
            if (this.CommentaryFlag != null) obj["Commentary Flag"] = new JsonBool(this.CommentaryFlag.Value);
            if (this.DefaultFlag != null) obj["Default Track Flag"] = new JsonBool(this.DefaultFlag.Value);

            if (obj.Items.Count() == 0)
            {
                return null;
            } else
            {
                return obj;
            }
        }

    }
}
