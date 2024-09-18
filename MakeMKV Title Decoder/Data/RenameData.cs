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

    public class Playlist : IJsonSerializable {

        /// <summary>
        /// Title that shows in VLC when played (i.e. title saved in file)
        /// </summary>
        public string? Title = null;

        /// <summary>
        /// The user given name of the playlist for renaming purposes
        /// </summary>
        public string? Name = null;

        /// <summary>
        /// How the output file should be renamed based on season, episode, type, etc
        /// </summary>
        public OutputName OutputFile = new();

        /// <summary>
        /// Source files added to the playlist
        /// </summary>
        public SerializableList<PlaylistFile> Files = new();

        // TODO should TrackID here be the filename, since index may not be known/needed until multiplexing?
        /// <summary>
        /// The order of the source file tracks as they should be organized in the mkv file
        /// </summary>
        public SerializableList<TrackID> TrackOrder = new(); 

        public void LoadFromJson(JsonData Data) {
            JsonObject obj = (JsonObject)Data;

            this.Title = obj.Load<JsonString>("Title")?.Value ?? null;
            this.Name = obj.Load<JsonString>("Name")?.Value ?? null;

            var data = obj["Output Name"];
            this.OutputFile = new();
            if (data != null)
            {
                this.OutputFile.LoadFromJson(data);
            }

            this.Files.LoadFromJson(obj["Files"] ?? new JsonArray());
            this.TrackOrder.LoadFromJson(obj["Track Order"] ?? new JsonArray());
        }

        public JsonData SaveToJson() {
            JsonObject obj = new();

            if (this.Title != null) obj["Title"] = new JsonString(this.Title);
            if (this.Name != null) obj["Name"] = new JsonString(this.Name);

            JsonData data = this.OutputFile.SaveToJson();
            if (data != null) obj["Output File"] = data;

            JsonArray arr = new();
            foreach(var item in this.Files)
            {
                data = item.SaveToJson();
                if (data != null) arr.Add(data);
            }
            if(arr.Any()) obj["Files"] = arr;

            arr = new();
            foreach(var item in this.TrackOrder)
            {
                data = item.SaveToJson();
                if (data != null) arr.Add(data);
            }
            if(arr.Any()) obj["Track Order"] = arr;

            if (obj.Items.Count() > 0) return obj;
            else return null;
        }
    }

    public class PlaylistFile : IJsonSerializable {

        public string? Source;
        public SerializableList<PlaylistTrack> Tracks = new();

        public SerializableList<PlaylistFile> AppendedFiles = new();

        public void LoadFromJson(JsonData Data) {
            JsonObject obj = (JsonObject)Data;

            this.Source = obj.Load<JsonString>("Source")?.Value ?? null;
            this.Tracks.LoadFromJson(obj["Tracks"] ?? new JsonArray());
            this.AppendedFiles.LoadFromJson(obj["Appended Files"] ?? new JsonArray());
        }

        public JsonData SaveToJson() {
            JsonObject obj = new();

            if (Source != null) obj["Source"] = new JsonString(Source);

            JsonArray arr = new();
            foreach(var track in this.Tracks)
            {
                JsonData? data = track.SaveToJson();
                if (data != null) arr.Add(data);
            }
            if(arr.Any()) obj["Tracks"] = arr;

            arr = new();
            foreach(var file in this.AppendedFiles)
            {
                JsonData? data = file.SaveToJson();
                if (data != null) arr.Add(data);
            }
            if (arr.Any()) obj["Appended Files"] = arr;

            return obj;
        }
    }

    public class PlaylistTrack : IJsonSerializable {

        /// <summary>
        /// Primary method of identifying the track
        /// </summary>
        public long? UID;

        /// <summary>
        /// Secondary method of identifying the track
        /// </summary>
        public string? Codec;

        /// <summary>
        /// Third method of identifying the track, as provided by MkvToolNix
        /// </summary>
        public long? ID;

        /// <summary>
        /// whether or not the track should be copied to the output.
        /// Could also be called enable/disable
        /// </summary>
        public bool? Copy;

        /// <summary>
        /// If the previous track is missing/disabled/empty, this is used to
        /// point to all of those tracks so the appropriate empty time can be
        /// calculated (by parsing cpli files)
        /// </summary>
        public SerializableList<TrackID> Sync = new();

        /// <summary>
        /// The user-given name to apply to the track
        /// </summary>
        public string? Name;

        /// <summary>
        /// If the track has the commentary flag
        /// </summary>
        public bool? Commentary;

        /// <summary>
        /// The previous track that this track is appended to. There can be missing/disabled
        /// tracks between this and the previous track so long as the 'sync' attribute is set
        /// </summary>
        public TrackID? AppendedTo = null; 

        public void LoadFromJson(JsonData Data) {
            JsonObject obj = (JsonObject)Data;

            this.UID = obj.Load<JsonInteger>("UID")?.Value ?? null;
            this.Codec = obj.Load<JsonString>("Codec")?.Value ?? null;
            this.ID = obj.Load<JsonInteger>("ID")?.Value ?? null;
            this.Copy = obj.Load<JsonBool>("Copy")?.Value ?? null;
            this.Sync.LoadFromJson(obj["Sync"] ?? new JsonArray());
            this.Name = obj.Load<JsonString>("Name")?.Value ?? null;
            this.Commentary = obj.Load<JsonBool>("Commentary")?.Value ?? null;

            JsonData? data = obj["Append To"];
            if (data == null) AppendedTo = null;
            else
            {
                this.AppendedTo = new();
                this.AppendedTo.LoadFromJson(data);
            }
        }

        public JsonData SaveToJson() {
            JsonObject obj = new();
            
            if (this.UID != null) obj["UID"] = new JsonInteger(this.UID.Value);
            if (this.Codec != null) obj["Codec"] = new JsonString(this.Codec);
            if (this.ID != null) obj["ID"] = new JsonInteger(this.ID.Value);
            if (this.Copy != null) obj["Copy"] = new JsonBool(this.Copy.Value);

            JsonArray arr = new();
            foreach(var sync in this.Sync)
            {
                JsonData? d = sync.SaveToJson();
                if (d != null) arr.Add(d);
            }
            if(arr.Any()) obj["Sync"] = arr;

            if (this.Name != null) obj["Name"] = new JsonString(this.Name);
            if (this.Commentary != null) obj["Commentary"] = new JsonBool(this.Commentary.Value);

            JsonData? data = this.AppendedTo?.SaveToJson();
            if (data != null) obj["Append To"] = data;

            if (obj.Items.Count() == 0) return null;
            else return obj;
        }
    }

    public class TrackID : IJsonSerializable {
        public int? StreamIndex;
        public int? TrackIndex; // TODO Want this to be called "TrackID" but can't because that matches the class name

        public void LoadFromJson(JsonData Data) {
            JsonObject obj = (JsonObject)Data;

            this.StreamIndex = (int?)(obj.Load<JsonInteger>("File Index")?.Value ?? null);
            this.TrackIndex = (int?)(obj.Load<JsonInteger>("Track Index")?.Value ?? null);
        }

        public JsonData SaveToJson() {
            JsonObject obj = new();

            if (this.StreamIndex != null) obj["File Index"] = new JsonInteger(this.StreamIndex.Value);
            if (this.TrackIndex != null) obj["Track Index"] = new JsonInteger(this.TrackIndex.Value);

            if (obj.Items.Count() == 0) return null;
            else return obj;
        }
    }

    public class OutputName : IJsonSerializable {

        public TmdbID? ShowID;
        public string? ShowName;

        // For multiple versions of the same episode, which version is this one?
        public string? MultiVersion;
        public FeatureType? FeatureType;
        public string? ExtraName;


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
                this.FeatureType = result;
            } else
            {
                this.FeatureType = null;
            }

            this.ExtraName = obj.Load<JsonString>("Extra Name")?.Value ?? null;
        }

        public JsonData SaveToJson() {
            JsonObject obj = new();

            if (ShowID != null) obj["ID"] = ShowID.SaveToJson();
            if (this.ShowName != null) obj["Show Name"] = new JsonString(this.ShowName);
            if (this.MultiVersion != null) obj["Multi-Version"] = new JsonString(this.MultiVersion);
            if (this.FeatureType != null) obj["Feature Type"] = new JsonString(this.FeatureType.ToString());
            if (this.ExtraName != null) obj["Extra Name"] = new JsonString(this.ExtraName);

            if (obj.Items.Count() == 0) return null;
            else return obj;
        }

    }
}
