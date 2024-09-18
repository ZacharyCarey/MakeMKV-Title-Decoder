using JsonSerializable;
using MakeMKV_Title_Decoder.libs.MkvToolNix.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.Data.Renames
{
	public class ClipRename : IJsonSerializable
	{
		// The index of the clip in the DiscIdentity class
		public long ID = -1;

		public string? Name { get; set; } = null;
		public SerializableList<TrackRename> TrackRenames = new();

		public ClipRename() { }

		public ClipRename(ClipRename deepCopy)
		{
			this.ID = deepCopy.ID;
			this.Name = deepCopy.Name;

			foreach(var track in deepCopy.TrackRenames)
			{
				this.TrackRenames.Add(new TrackRename(track));
			}
		}

		public void LoadFromJson(JsonData Data)
		{
			JsonObject obj = (JsonObject)Data;

			this.ID = ((JsonInteger)obj["ID"]).Value;
			this.Name = obj.Load<JsonString>("Name")?.Value ?? null;
			this.TrackRenames.LoadFromJson(obj["Tracks"] ?? new JsonObject());
		}

		public JsonData SaveToJson()
		{
			JsonObject obj = new();

			obj["ID"] = new JsonInteger(this.ID);

			if (this.Name != null) obj["Name"] = new JsonString(this.Name);

			JsonArray tracks = new();
			foreach (var track in this.TrackRenames)
			{
				var json = track.SaveToJson();
				if (json != null) tracks.Add(json);
			}
			if (tracks.Count() > 0) obj["Tracks"] = tracks;

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
