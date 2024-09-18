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
	public class LoadedStream
	{
		public ClipRename Rename = new();
		public MkvMergeID Data { get; private set; }
		public StreamIdentity Identity { get; private set; }
		public TimeSpan Duration; // Loaded from CLPI file, if possible

		public List<LoadedTrack> Tracks { get; private set; } = new List<LoadedTrack>();

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

			return new LoadedStream(clip, timespan.Value); ;
		}

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
		}

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
	}
}
