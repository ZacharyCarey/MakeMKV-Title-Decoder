using JsonSerializable;
using libbluray.bdnav.Clpi;
using MakeMKV_Title_Decoder.Data.Renames;
using MakeMKV_Title_Decoder.libs.MkvToolNix.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.Data
{
	public class LoadedStream : IJsonSerializable
	{
		public StreamIdentity Identity { get; private set; } = new();
		public ClipRename Rename = new();
		public MkvMergeID Data { get; private set; }

		public TimeSpan Duration; // Loaded from CLPI file, if possible

		public List<LoadedTrack> Tracks { get; private set; } = new List<LoadedTrack>();

		private LoadedStream() { }

		private LoadedStream(MkvMergeID clip, TimeSpan duration)
		{
			this.Data = clip;
			this.Identity = new();
			this.Duration = duration;

			foreach(var track in clip.Tracks)
			{
				this.Tracks.Add(new LoadedTrack(track));
			}
		}

		private LoadedStream(StreamIdentity identity, MkvMergeID clip, TimeSpan duration)
		{
			this.Identity = identity;
			this.Data = clip;
			this.Duration = duration;
		}

		public LoadedTrack? GetTrack(long index)
		{
			if (index < 0 || index >= Tracks.Count) return null;
			return Tracks[(int)index];
		}

		public static LoadedStream? TryLoadStream(MkvToolNixDisc disc, MkvMergeID clip)
		{
			TimeSpan? timespan = LoadDurationFromClpiFile(disc, clip);
			if (timespan == null)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("Failed to read stream duration: " + clip.FileName);
				Console.ResetColor();
				return null;
			}

			Dictionary<MkvTrackType, List<LoadedTrack>> sortedByType = new();
			List<LoadedTrack> tracks = new List<LoadedTrack>();
			foreach(var track in clip.Tracks)
			{
				LoadedTrack? loadedTrack = LoadedTrack.TryLoadTrack(disc, clip, track);
				if (loadedTrack == null)
				{
					return null;
				}
				tracks.Add(loadedTrack);

				List<LoadedTrack> typeList;
				if (!sortedByType.TryGetValue(loadedTrack.Data.Type, out typeList))
				{
					typeList = new();
					sortedByType[loadedTrack.Data.Type] = typeList;
				}
				typeList.Add(loadedTrack);
			}

			// Set defaults if needed
			foreach(var track in tracks)
			{
				if (track.Data.Properties?.DefaultTrack == null)
				{
					if (track.Data.Type == MkvTrackType.Audio || track.Data.Type == MkvTrackType.Video)
					{
						track.Rename.DefaultFlag = sortedByType[track.Data.Type].IndexOf(track) == 0;
					} else
					{
						track.Rename.DefaultFlag = false;
					}
				}
			}

			var stream = new LoadedStream(clip, timespan.Value);
			stream.Identity.LoadFromMkvToolNix(stream); // TODO should this just be in constructor?
			stream.Tracks = tracks;
			return stream;
		}

		public bool TryLoadRenameData(MkvToolNixDisc disc, MkvMergeID clip)
		{
			if (this.Tracks.Count != clip.Tracks.Count) return false;

			// TODO shares code with TryLoadStream....
			TimeSpan? timespan = LoadDurationFromClpiFile(disc, clip);
			if (timespan == null)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("Failed to read stream duration: " + clip.FileName);
				Console.ResetColor();
				return false;
			}

			for (int i = 0; i < this.Tracks.Count; i++)
			{
				LoadedTrack loadedTrack = this.Tracks[i];
				bool result = loadedTrack.TryLoadRenameData(clip.Tracks[i]);
				if (result == false)
				{
					return false;
				}
			}

			this.Data = clip;
			this.Duration = timespan.Value;

			return true;
		}

		// TODO remove if not needed
		/*
				internal static LoadedStream? TryLoadStream(StreamIdentity identity, MkvToolNixDisc disc, MkvMergeID clip)
				{
					if (identity.Tracks.Count != clip.Tracks.Count) return null;

					TimeSpan? timespan = LoadDurationFromClpiFile(disc, clip);
					if (timespan == null)
					{
						Console.ForegroundColor = ConsoleColor.Red;
						Console.WriteLine("Failed to read stream duration: " + clip.FileName);
						Console.ResetColor();
						return null;
					}

					if ((timespan.Value - identity.Duration.Value).Duration() >= new TimeSpan(0, 0, 1))
					{
						Console.ForegroundColor = ConsoleColor.Red;
						Console.WriteLine("Durations did not match: " + clip.FileName);
						Console.ResetColor();
						return null;
					}

					// TODO check other parameters stored in StreamIdentity

					List<LoadedTrack> tracks = new();
					for (int i = 0; i < identity.Tracks.Count; i++)
					{
						LoadedTrack loadedTrack = new(identity.Tracks[i], clip.Tracks[i]);
						double score = loadedTrack.CalculateSimilarityScore();
						if (score < 0.99)
						{
							return null;
						}
						tracks.Add(loadedTrack);
					}

					LoadedStream result = new LoadedStream(identity, clip, timespan.Value);
					result.Tracks = tracks;
					return result;
				}*/

		private static TimeSpan? LoadDurationFromClpiFile(MkvToolNixDisc disc, MkvMergeID clip)
		{
			// TODO combine MkvToolNixDisc and libbluray into one common data structure
			// that can be loaded all at once and merged, including this data

			// Try using libbluray
			// TODO prevent parsing multiple times
			ClpiFile? clipFile = ClpiFile.Parse(Path.Combine(disc.RootPath, "BDMV", "CLIPINF", $"{Path.GetFileNameWithoutExtension(clip.FileName)}.clpi"));
			if (clipFile == null)
			{
				// TODO adjust clpi parsing to ONLY parse durations??
				return null;
			}
			//TimeSpan clipLength = clipFile.sequence.atc_seq.SelectMany<ClipStcSequence,TimeSpan>(x => x.stc_seq.Select(y => y.Length).Aggregate((a, b) => a + b));
			TimeSpan clipLength = clipFile.sequence.atc_seq.SelectMany(x => x.stc_seq.Select(y => y.Length)).Aggregate((a, b) => a + b);
			return clipLength;
		}

		internal bool MatchData(MkvMergeID clip)
		{
			this.Data = clip;
			if (this.Tracks.Count != clip.Tracks.Count) return false;

			if ((Duration - Identity.Duration.Value).Duration() >= new TimeSpan(0, 0, 1))
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("Durations did not match: " + clip.FileName);
				Console.ResetColor();
				return false;
			}

			// TODO check other parameters stored in StreamIdentity


			for (int i = 0; i < clip.Tracks.Count; i++)
			{
				bool result = this.Tracks[i].MatchData(clip.Tracks[i]);
				if (result == false)
				{
					return false;
				}
			}

			return true;
		}


		public void LoadFromJson(JsonData Data)
		{
			JsonObject obj = (JsonObject)Data;

			this.Identity.LoadFromJson(obj["Identity"]);
			this.Rename.LoadFromJson(obj["Rename"]);

			TimeSpan? temp;
			Data.LoadJson("Duration", out temp);
			if (temp == null) throw new Exception();
			else this.Duration = temp.Value;

			this.Tracks.Clear();
			foreach(var data in (JsonArray)obj["Tracks"])
			{
				LoadedTrack track = LoadedTrack.LoadJson(data);
				this.Tracks.Add(track);
			}
		}

		public JsonData SaveToJson()
		{
			JsonObject obj = new();

			obj["Identity"] = this.Identity.SaveToJson();
			obj["Rename"] = this.Rename.SaveToJson();
			obj["Duration"] = Utils.SaveJson(this.Duration);

			JsonArray tracks = new();
			foreach(var track in this.Tracks)
			{
				tracks.Add(track.SaveToJson());
			}
			obj["Tracks"] = tracks;

			return obj;
		}

		// TODO hack
		public static LoadedStream LoadJson(JsonData Data)
		{
			LoadedStream stream = new();
			stream.LoadFromJson(Data);
			return stream;
		}
	}
}
