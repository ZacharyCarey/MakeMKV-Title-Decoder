using JsonSerializable;
using MakeMKV_Title_Decoder.Data.Renames;
using MakeMKV_Title_Decoder.Forms.FileRenamer;
using MakeMKV_Title_Decoder.Forms.TmdbBrowser;
using MakeMKV_Title_Decoder.libs.MkvToolNix.Data;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MakeMKV_Title_Decoder.Data
{

    public class RenameData : IJsonSerializable {
        public const string Version = "1.0";

        public LoadedDisc Disc = new();

        // Playlist data created by the user for how files should be multiplexed / outputted
        public SerializableList<Playlist> Playlists = new();

        public RenameData() {

        }

        public JsonData SaveToJson() {
            JsonObject obj = new();

            obj["version"] = new JsonString(Version);
            obj["Disc"] = this.Disc.SaveToJson();

            JsonArray playlists = new();
            foreach(var playlist in this.Playlists)
            {
                var json = playlist.SaveToJson();
                if (json != null) playlists.Add(json);
            }
            if (playlists.Count() > 0) obj["Playlists"] = playlists;

            return obj;
        }

        public void LoadFromJson(JsonData data) {
            JsonObject obj = (JsonObject)data;

            string version = (JsonString)obj["version"];
            if (version != Version)
            {
                throw new Exception("Unable to read this version of the file. Please upgrade the file to a newer version.");
            }

            this.Disc.LoadFromJson(obj["Disc"]);
            this.Playlists.LoadFromJson(obj["Playlists"] ?? new JsonArray());
        }
    }

    public class OutputName : IJsonSerializable {
        const string TmdbPrefix = "tmdbid=";

        public TmdbID? ShowID;
        public string? ShowName;

        // For multiple versions of the same episode, which version is this one?
        public string? MultiVersion;
        public FeatureType? Type;
        public string? ExtraName;


        public string GetFolderPath() {
            List<string> folders = new();

            folders.Add($"{this.ShowName} [{TmdbPrefix}{ShowID?.ID.ToString() ?? ""}]");

            if (ShowID?.Type == ShowType.Movie)
            {
                // TODO this is bad
            } else if (ShowID?.Type == ShowType.TV)
            {
                folders.Add($"Season {ShowID.Season}");
            } else
            {
                throw new Exception();
            }

            return Path.Combine(folders.ToArray());
        }

        public string? GetBonusFolder() {
            switch (ShowID?.Type)
            {
                case ShowType.Movie:
                case ShowType.TV:
                    return GetFolderName(this.Type ?? FeatureType.MainFeature);
                default:
                    throw new Exception();
            }
        }

        public string GetFileName() {
            string versionString = "";
            if (MultiVersion != null)
            {
                versionString = $" - {MultiVersion}";
            }

            if (ShowID?.Type == ShowType.Movie)
            {
                string? bonusFolder = GetFolderName(this.Type ?? FeatureType.MainFeature);
                if (bonusFolder != null)
                {
                    return $"{ExtraName ?? ""}{versionString}.mkv";
                } else
                {
                    return $"{ShowName ?? ""} [{TmdbPrefix}{ShowID?.ID.ToString() ?? ""}]{versionString}.mkv";
                }
            } else if (ShowID?.Type == ShowType.TV)
            {
                string? bonusFolder = GetFolderName(this.Type ?? FeatureType.MainFeature);
                if (bonusFolder != null)
                {
                    return $"{ExtraName ?? ""}{versionString}.mkv";
                } else
                {
                    return $"{ShowName ?? ""} S{ShowID.Season}E{ShowID.Episode}{versionString}.mkv";
                }
            } else
            {
                throw new Exception();
            }
        }

        private static string? GetFolderName(FeatureType type) {
            switch (type)
            {
                case FeatureType.Extras: return "extras";
                case FeatureType.Specials: return "specials";
                case FeatureType.Shorts: return "shorts";
                case FeatureType.Scenes: return "scenes";
                case FeatureType.Featurettes: return "featurettes";
                case FeatureType.BehindTheScenes: return "behind the scenes";
                case FeatureType.DeletedScenes: return "deleted scenes";
                case FeatureType.Interviews: return "interviews";
                case FeatureType.Trailers: return "trailers";
                default:
                    return null;
            }
        }

        public void LoadFromJson(JsonData Data) {
            JsonObject obj = (JsonObject)Data;

            var data = obj["ID"];
            if (data == null) this.ShowID = null;
            else
            {
                this.ShowID = new(ShowType.Movie, 0);
                this.ShowID.LoadFromJson(data);
            }

            this.ShowName = obj.Load<JsonString>("Show Name")?.Value ?? null;
            this.MultiVersion = obj.Load<JsonString>("Multi-Version")?.Value ?? null;

            FeatureType result;
            if (Enum.TryParse(obj.Load<JsonString>("Feature Type")?.Value ?? null, out result))
            {
                this.Type = result;
            } else
            {
                this.Type = null;
            }

            this.ExtraName = obj.Load<JsonString>("Extra Name")?.Value ?? null;
        }

        public JsonData SaveToJson() {
            JsonObject obj = new();

            if (ShowID != null) obj["ID"] = ShowID.SaveToJson();
            if (this.ShowName != null) obj["Show Name"] = new JsonString(this.ShowName);
            if (this.MultiVersion != null) obj["Multi-Version"] = new JsonString(this.MultiVersion);
            if (this.Type != null) obj["Feature Type"] = new JsonString(this.Type.ToString());
            if (this.ExtraName != null) obj["Extra Name"] = new JsonString(this.ExtraName);

            return obj;
        }

    }
}
