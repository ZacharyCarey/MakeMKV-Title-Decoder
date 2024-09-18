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

		public string? Name { get; set; } = null;

		public ClipRename() { }

		/*public ClipRename(ClipRename deepCopy)
		{
			this.ID = deepCopy.ID;
			this.Name = deepCopy.Name;

			foreach(var track in deepCopy.TrackRenames)
			{
				this.TrackRenames.Add(new TrackRename(track));
			}
		}*/

		public void LoadFromJson(JsonData Data)
		{
			JsonObject obj = (JsonObject)Data;

			this.Name = obj.Load<JsonString>("Name")?.Value ?? null;
		}

		public JsonData SaveToJson()
		{
			JsonObject obj = new();

			if (this.Name != null) obj["Name"] = new JsonString(this.Name);

			return obj;
		}
	}
}
