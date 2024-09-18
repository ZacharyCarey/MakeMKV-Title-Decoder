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
		public string? Name = null;
		public bool? CommentaryFlag = null;
		public bool? DefaultFlag = null;

		public TrackRename() { }

		/*public TrackRename(TrackRename deepCopy)
		{
			this.Name = deepCopy.Name;
			this.CommentaryFlag = deepCopy.CommentaryFlag;
			this.DefaultFlag = deepCopy.DefaultFlag;
		}*/

		public void LoadFromJson(JsonData Data)
		{
			JsonObject obj = (JsonObject)Data;

			this.Name = obj.Load<JsonString>("Name")?.Value ?? null;
			this.CommentaryFlag = obj.Load<JsonBool>("Commentary Flag")?.Value ?? null;
			this.DefaultFlag = obj.Load<JsonBool>("Default Track Flag")?.Value ?? null;
		}

		public JsonData SaveToJson()
		{
			JsonObject obj = new();

			if (this.Name != null) obj["Name"] = new JsonString(this.Name);
			if (this.CommentaryFlag != null) obj["Commentary Flag"] = new JsonBool(this.CommentaryFlag.Value);
			if (this.DefaultFlag != null) obj["Default Track Flag"] = new JsonBool(this.DefaultFlag.Value);

			return obj;
		}

	}
}
