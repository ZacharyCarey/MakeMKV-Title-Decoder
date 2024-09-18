using JsonSerializable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.Data
{
	// TODO
	/*public enum PlaylistTrackType
	{
		Source,
		Delay
	}*/

	public class PlaylistTrack : IJsonSerializable
	{
		// Index of the source file in this playlist
		public long SourceFileIndex = -1;

		// The index of the appended file (using the SourceFileIndex)
		// If <0, then this track belongs to the source file
		public long AppendedFileIndex = -1;

		public long TrackIndex = -1;

		/// <summary>
		/// whether or not the track should be copied to the output.
		/// Could also be called enable/disable.
		/// 
		/// If disabled, the track will be used to a create a delay
		/// for the next track
		/// </summary>
		public bool Copy = true;

		/// <summary>
		/// Only allows a depth of 1. I.e. an appended track can't itself have appended tracks
		/// </summary>
		public SerializableList<PlaylistTrack> AppendedTracks = new();

		public void LoadFromJson(JsonData data)
		{
			var obj = (JsonObject)data;

			this.SourceFileIndex = ((JsonInteger)obj["Source File Index"]).Value;
			this.AppendedFileIndex = ((JsonInteger)obj["Appended File Index"]).Value;
			this.TrackIndex = ((JsonInteger)obj["Track Index"]).Value;
			this.Copy = ((JsonBool)obj["Copy"]).Value;
			this.AppendedTracks.LoadFromJson(obj["Appended Tracks"]);
		}

		public JsonData SaveToJson()
		{
			JsonObject obj = new();

			obj["Source File Index"] = new JsonInteger(this.SourceFileIndex);
			obj["Appended File Index"] = new JsonInteger(this.AppendedFileIndex);
			obj["Track Index"] = new JsonInteger(this.TrackIndex);
			obj["Copy"] = new JsonBool(this.Copy);
			obj["Appended Tracks"] = this.AppendedTracks.SaveToJson();

			return obj;
		}
	}
}
