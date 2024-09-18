using JsonSerializable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.Data.Renames
{
	public class TrackRename : IJsonSerializable
	{
		// The index of the track in the StreamIdentity class
		public long ID = -1;

		public string? Name = null;
		public bool? CommentaryFlag = null;
		public bool? DefaultFlag = null;

		public TrackRename() { }

		public TrackRename(TrackRename deepCopy)
		{
			this.ID = deepCopy.ID;
			this.Name = deepCopy.Name;
			this.CommentaryFlag = deepCopy.CommentaryFlag;
			this.DefaultFlag = deepCopy.DefaultFlag;
		}

		public void LoadFromJson(JsonData Data)
		{
			JsonObject obj = (JsonObject)Data;

			this.ID = ((JsonInteger)obj["ID"]).Value;
			this.Name = obj.Load<JsonString>("Name")?.Value ?? null;
			this.CommentaryFlag = obj.Load<JsonBool>("Commentary Flag")?.Value ?? null;
			this.DefaultFlag = obj.Load<JsonBool>("Default Track Flag")?.Value ?? null;
		}

		public JsonData SaveToJson()
		{
			JsonObject obj = new();

			obj["ID"] = new JsonInteger(ID);
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
