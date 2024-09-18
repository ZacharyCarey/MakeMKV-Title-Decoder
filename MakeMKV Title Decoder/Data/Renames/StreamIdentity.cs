using JsonSerializable;
using MakeMKV_Title_Decoder.libs.MkvToolNix.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.Data.Renames
{
	public class StreamIdentity : IJsonSerializable
	{

		public string? FileName = null;

		public TimeSpan? Duration = null;

		public DataSize? FileSize = null;

		//public SerializableList<TrackIdentity> Tracks = new();

		public void LoadFromMkvToolNix(LoadedStream loadedClip)
		{
			var clip = loadedClip.Data;

			this.FileName = clip.GetRelativePath();
			this.Duration = loadedClip.Duration;
			this.FileSize = clip.FileSize;

			/*this.Tracks.Clear();
			foreach(var loadedTrack in loadedClip.Tracks)
			{
				var track = new TrackIdentity();
				track.LoadFromMkvToolNix(loadedTrack.Data);
				this.Tracks.Add(track);
			}*/
		}

		public void LoadFromJson(JsonData Data)
		{
			Data.LoadJson("File Name", out FileName);
			Data.LoadJson("Duration", out Duration);

			// TODO simplify
			JsonData data = ((JsonObject)Data)["File Size"];
			if (data != null && data is JsonString str && str.Value != null)
			{
				this.FileSize = DataSize.Parse(str.Value);
			} else
			{
				this.FileSize = null;
			}

			//Tracks.LoadFromJson(((JsonObject)Data)["Tracks"]);
		}

		public JsonData SaveToJson()
		{
			JsonObject obj = new();

			obj["File Name"] = Utils.SaveJson(FileName);
			obj["Duration"] = Utils.SaveJson(Duration);
			obj["File Size"] = Utils.SaveJson(FileSize);
			//obj["Tracks"] = Tracks.SaveToJson();

			return obj;
		}
	}
}
