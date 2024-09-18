using JsonSerializable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.Data
{
	public class Playlist : IJsonSerializable
	{

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
		public SerializableList<PlaylistSource> SourceFiles = new();

		public SerializableList<PlaylistTrack> Tracks = new();

		public void LoadFromJson(JsonData Data)
		{
			JsonObject obj = (JsonObject)Data;

			this.Title = obj.Load<JsonString>("Title")?.Value ?? null;
			this.Name = obj.Load<JsonString>("Name")?.Value ?? null;
			this.OutputFile.LoadFromJson(obj["Output File"]);
			this.SourceFiles.LoadFromJson(obj["Source Files"]);
			this.Tracks.LoadFromJson(obj["Tracks"]);
		}

		public JsonData SaveToJson()
		{
			JsonObject obj = new();

			if (this.Title != null) obj["Title"] = new JsonString(this.Title);
			if (this.Name != null) obj["Name"] = new JsonString(this.Name);

			obj["Output File"] = this.OutputFile.SaveToJson();
			obj["Source Files"] = this.SourceFiles.SaveToJson();
			obj["Tracks"] = this.Tracks.SaveToJson();

			return obj;
		}
	}
}
