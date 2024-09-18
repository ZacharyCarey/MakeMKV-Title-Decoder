using JsonSerializable;
using MakeMKV_Title_Decoder.Data.Renames;
using MakeMKV_Title_Decoder.libs.MkvToolNix.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.Data
{

	// TODO probably needs a better name
	public class LoadedDisc : IJsonSerializable
	{
		public DiscIdentity Identity { get; private set; } = new();
		public MkvToolNixDisc Data { get; private set; }
		public List<LoadedStream> Streams { get; private set; } = new List<LoadedStream>();

		public LoadedDisc()
		{

		}

		private LoadedDisc(MkvToolNixDisc disc)
		{
			this.Data = disc;
			this.Identity = new();
		}

		public LoadedStream? GetStream(long index)
		{
			if (index < 0 || index >= Streams.Count) return null;
			return Streams[(int)index];
		}

		public LoadedTrack? GetTrack(TrackID id)
		{
			if (id.StreamIndex == null || id.TrackIndex == null) return null;

			LoadedStream? stream = GetStream((long)id.StreamIndex);
			if (stream == null) return null;

			return stream.GetTrack((long)id.TrackIndex);
		}

		/*public void Save(RenameData renameData)
		{
			Dictionary<LoadedStream, long> streamLookup = new();
			Dictionary<LoadedTrack, TrackID> trackLookup = new();

			// Save ID info
			renameData.DiscIdentity = new();
			renameData.DiscIdentity.LoadFromMkvToolNix(this);
			foreach (var stream in Streams)
			{
				var streamID = new StreamIdentity();
				streamID.LoadFromMkvToolNix(stream);

				int streamIndex = renameData.DiscIdentity.Streams.Count;
				streamLookup[stream] = streamIndex;
				renameData.DiscIdentity.Streams.Add(streamID);

				foreach(var track in stream.Tracks)
				{
					var trackID = new TrackIdentity();
					trackID.LoadFromMkvToolNix(track);

					int trackIndex = stream.Tracks.Count;
					var ID = new TrackID();
					ID.StreamIndex = streamIndex;
					ID.TrackIndex = trackIndex;

					trackLookup[track] = ID;
					streamID.Tracks.Add(trackID);
				}
			}

			// Save rename info
			renameData.ClipRenames.Clear();
			foreach(var stream in Streams)
			{
				renameData.ClipRenames.Add(stream.Rename);
				stream.Rename.ID = streamLookup[stream];

				stream.Rename.TrackRenames.Clear();
				foreach(var track in stream.Tracks)
				{
					stream.Rename.TrackRenames.Add(track.Rename);
					track.Rename.ID = (long)trackLookup[track].TrackIndex;
				}
			}
		}*/

		public static LoadedDisc? TryLoadDisc(MkvToolNixDisc disc)
		{
			List<LoadedStream> streams = new();
			foreach (var stream in disc.Streams)
			{
				LoadedStream? loadedStream = LoadedStream.TryLoadStream(disc, stream);
				if (loadedStream == null)
				{
					return null;
				}
				streams.Add(loadedStream);
			}

			LoadedDisc result = new(disc);
			result.Identity.LoadFromMkvToolNix(result); // TODO should this just be in constructor?
			result.Streams = streams;
			return result;
		}

		public bool TryLoadRenameData(MkvToolNixDisc disc)
		{
			if (this.Streams.Count != disc.Streams.Length) return false;

			// TODO shares code with TryLoadDisc....
			for (int i = 0; i < this.Streams.Count; i++)
			{
				LoadedStream loadedStream = this.Streams[i];
				bool result = loadedStream.TryLoadRenameData(disc, disc.Streams[i]);

				if (result == false)
				{
					return false;
				}
			}
			this.Data = disc;

			// Attempt to match data to disc
			if (!this.MatchData(disc))
			{
				return false;
			} else
			{
				return true;
			}
		}

		internal bool MatchData(MkvToolNixDisc disc)
		{
			this.Data = disc;
			if (disc.Streams.Length != this.Streams.Count) return false;

			try
			{
				for (int i = 0; i < disc.Streams.Length; i++)
				{
					bool result = this.Streams[i].MatchData(disc.Streams[i]);
					if (result == false)
					{
						return false;
					}
				}
			}catch(Exception ex)
			{
				return false;
			}

			return true;
		}

		public void LoadFromJson(JsonData data)
		{
			var obj = (JsonObject)data;

			this.Identity.LoadFromJson(obj["Identity"]);

			this.Streams.Clear();
			foreach(var Data in (JsonArray)obj["Streams"])
			{
				LoadedStream stream = LoadedStream.LoadJson(Data);
				this.Streams.Add(stream);
			}
		}

		public JsonData SaveToJson()
		{
			var obj = new JsonObject();

			obj["Identity"] = this.Identity.SaveToJson();

			JsonArray streams = new();
			foreach(var stream in this.Streams)
			{
				streams.Add(stream.SaveToJson());
			}
			obj["Streams"] = streams;

			return obj;
		}
	}

}
