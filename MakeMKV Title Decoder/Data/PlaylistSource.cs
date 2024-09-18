using JsonSerializable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.Data
{
	public class PlaylistSource : IJsonSerializable
	{

		public long StreamIndex = -1;

		// Can only be a nested depth of 1.
		public SerializableList<PlaylistSource> AppendedFiles = new();

		public LoadedStream? GetStream(LoadedDisc disc)
		{
			return disc.GetStream(StreamIndex);
		}

		public void LoadFromJson(JsonData Data)
		{
			JsonObject obj = (JsonObject)Data;

			this.StreamIndex = ((JsonInteger)obj["Stream Index"]).Value;
			this.AppendedFiles.LoadFromJson(obj["Appended Files"]);
		}

		public JsonData SaveToJson()
		{
			JsonObject obj = new();

			obj["Stream Index"] = new JsonInteger(this.StreamIndex);
			obj["Appended Files"] = this.AppendedFiles.SaveToJson();

			return obj;
		}

	}
}
