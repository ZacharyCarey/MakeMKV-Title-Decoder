using JsonSerializable;
using MakeMKV_Title_Decoder.libs.MkvToolNix.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.Data.Renames
{
	public class DiscIdentity : IJsonSerializable
	{

		public string? Title = null;
		public long? NumberOfSets = null;
		public long? SetNumber = null;

		//public SerializableList<StreamIdentity> Streams = new();

		public void LoadFromMkvToolNix(LoadedDisc loadedDisc)
		{
			var disc = loadedDisc.Data;

			Title = disc.Title;
			NumberOfSets = disc.NumberOfSets;
			SetNumber = disc.SetNumber;

			//Streams.Clear();
			/*foreach(var loadedStream in loadedDisc.Streams)
			{
				var stream = new StreamIdentity();
				stream.LoadFromMkvToolNix(loadedStream);
				Streams.Add(stream);
			}*/
		}

		public void LoadFromJson(JsonData Data)
		{
			Data.LoadJson("Title", out Title);
			Data.LoadJson("Number of Sets", out NumberOfSets);
			Data.LoadJson("Set Number", out  SetNumber);

			//Streams.LoadFromJson(((JsonObject)Data)["Streams"]);
		}

		public JsonData SaveToJson()
		{
			JsonObject obj = new();

			obj["Title"] = Utils.SaveJson(Title);
			obj["Number of Sets"] = Utils.SaveJson(NumberOfSets);
			obj["Set Number"] = Utils.SaveJson(SetNumber);

			//obj["Streams"] = Streams.SaveToJson();

			return obj;
		}
	}
}
