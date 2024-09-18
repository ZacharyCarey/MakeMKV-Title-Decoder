using JsonSerializable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.Data
{
	public class TrackID : IJsonSerializable
	{
		public int? StreamIndex;
		public int? TrackIndex; // TODO Want this to be called "TrackID" but can't because that matches the class name

		public void LoadFromJson(JsonData Data)
		{
			JsonObject obj = (JsonObject)Data;

			this.StreamIndex = (int?)(obj.Load<JsonInteger>("File Index")?.Value ?? null);
			this.TrackIndex = (int?)(obj.Load<JsonInteger>("Track Index")?.Value ?? null);
		}

		public JsonData SaveToJson()
		{
			JsonObject obj = new();

			if (this.StreamIndex != null) obj["File Index"] = new JsonInteger(this.StreamIndex.Value);
			if (this.TrackIndex != null) obj["Track Index"] = new JsonInteger(this.TrackIndex.Value);

			if (obj.Items.Count() == 0) return null;
			else return obj;
		}
	}
}
