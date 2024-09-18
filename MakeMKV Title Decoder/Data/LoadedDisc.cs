using MakeMKV_Title_Decoder.Data.Renames;
using MakeMKV_Title_Decoder.libs.MkvToolNix.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.Data
{

	// TODO probably needs a better name
	public class LoadedDisc
	{
		public DiscIdentity Identity { get; private set; }
		public MkvToolNixDisc Disc { get; private set; }
		public List<LoadedStream> Streams { get; private set; } = new List<LoadedStream>();

		private LoadedDisc(MkvToolNixDisc disc)
		{
			this.Disc = disc;
			this.Identity = new();
		}

		private LoadedDisc(DiscIdentity identity, MkvToolNixDisc disc)
		{
			this.Disc = disc;
			this.Identity = identity;
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

		public void Save(RenameData renameData)
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
		}

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
			result.Streams = streams;
			return result;
		}

		public static LoadedDisc? TryLoadRenameData(MkvToolNixDisc disc, RenameData rename)
		{
			var ID = rename.DiscIdentity;
			if (ID.Streams.Count != disc.Streams.Length) return null; 

			// TODO check other parameters in DiscID

			List<LoadedStream> streams = new();
			for (int i = 0; i < ID.Streams.Count; i++)
			{
				LoadedStream? stream = LoadedStream.TryLoadStream(ID.Streams[i], disc, disc.Streams[i]);
				if (stream == null)
				{
					return null;
				}
				streams.Add(stream);
			}

			LoadedDisc result = new(ID, disc);
			result.Streams = streams;

			// Load rename data
			foreach (ClipRename clipRename in rename.ClipRenames)
			{
				var streamID = streams[(int)clipRename.ID];
				streamID.Rename = new ClipRename(clipRename);

				foreach(TrackRename trackRename in streamID.Rename.TrackRenames)
				{
					var trackID = streamID.Tracks[(int)trackRename.ID];
					trackID.Rename = trackRename;
				}
			}

			return result;
		}
	}

}
