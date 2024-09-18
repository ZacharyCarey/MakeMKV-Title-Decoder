using JsonSerializable;
using MakeMKV_Title_Decoder.Data.Renames;
using MakeMKV_Title_Decoder.libs.MkvToolNix.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.Data
{
	public class LoadedTrack : IJsonSerializable
	{
		public const double MinimumSimilarityScore = 0.99;

		public TrackIdentity Identity { get; private set; } = new();
		public TrackRename Rename = new();
		public MkvTrack Data { get; private set; }
		public long MkvMergeID { get => Data.ID; }

		private LoadedTrack() { }

		public LoadedTrack(MkvTrack track)
		{
			this.Identity = new();
			this.Data = track;
		}

		public LoadedTrack(TrackIdentity trackID, MkvTrack trackData)
		{
			this.Identity = trackID;
			this.Data = trackData;
		}


		public static LoadedTrack? TryLoadTrack(MkvToolNixDisc disc, MkvMergeID clip, MkvTrack track)
		{
			var loadedTrack = new LoadedTrack(track);
			loadedTrack.Identity.LoadFromMkvToolNix(track); // TODO should this just be in constructor?
			return loadedTrack;
		}

		public bool TryLoadRenameData(MkvTrack track)
		{
			// TODO shares code with TryLoadTrack....
			this.Data = track;
			return true;
		}

		public double CalculateSimilarityScore()
		{
			double score = 0;

			score += Score(Identity.Codec, Data.Codec);
			score += Score(Identity.Type, Data.Type, (a, b) => a == b);
			score += Score(Identity.AudioBitsPerSample, Data.Properties?.AudioBitsPerSample);
			score += Score(Identity.AudioChannels, Data.Properties?.AudioChannels);
			score += Score(Identity.AudioSamplingFrequency, Data.Properties?.AudioSamplingFrequency);
			score += Score(Identity.CodecID, Data.Properties?.CodecID);
			score += Score(Identity.DisplayDimensions, Data.Properties?.DisplayDimensions);
			score += Score(Identity.DisplayUnit, Data.Properties?.DisplayUnit);
			score += Score(Identity.FlagHearingImpaired, Data.Properties?.FlagHearingImpaired);
			score += Score(Identity.FlagVisualImpaired, Data.Properties?.FlagHearingImpaired);
			score += Score(Identity.FlagTextDescriptions, Data.Properties?.FlagTextDescriptions);
			score += Score(Identity.FlagOriginal, Data.Properties?.FlagOriginal);
			score += Score(Identity.FlagCommentary, Data.Properties?.FlagCommentary);
			score += Score(Identity.Language, Data.Properties?.Language);
			score += Score(Identity.Number, Data.Properties?.Number);
			score += Score(Identity.PixelDimensions, Data.Properties?.PixelDimensions);
			score += Score(Identity.TrackName, Data.Properties?.TrackName);

			return score / 17.0;
		}

		private double Score<T>(T? a, T? b) where T : struct, IEquatable<T>
		{
			bool equal = false;
			if (!a.HasValue && !b.HasValue)
			{
				equal = true;
			} else if (a.HasValue && b.HasValue)
			{
				equal = a.Value.Equals(b.Value);
			}

			return equal ? 1.0 : 0.0;
		}

		private double Score<T>(T? a, T? b, Func<T, T, bool> compare) where T : struct
		{
			bool equal = false;
			if (!a.HasValue && !b.HasValue)
			{
				equal = true;
			} else if (a.HasValue && b.HasValue)
			{
				equal = compare(a.Value, b.Value);
			}

			return equal ? 1.0 : 0.0;
		}

		private double Score(string? original, string? testing)
		{
			if (original == null && testing == null)
			{
				return 1.0;
			} else if (original != null && testing != null)
			{
				int strDist = Utils.LevenshteinDistance(original, testing);
				double dist = strDist; //Math.Abs(strDist - original.Length);
				if (original.Length > 0)
				{
					dist /= original.Length;
				}
				double score = -4.0 * Math.Pow(dist, 2) + 1.0;
				return Math.Clamp(score, 0.0, 1.0);
            } else
			{
				return 0.0;
			}
		}
	
		internal bool MatchData(MkvTrack track)
		{
			this.Data = track;
			double score = CalculateSimilarityScore();
			if (score < MinimumSimilarityScore)
			{
				return false;
			}

			return true;
		}

		public void LoadFromJson(JsonData data)
		{
			JsonObject obj = (JsonObject)data;

			this.Identity.LoadFromJson(obj["Identity"]);
			this.Rename.LoadFromJson(obj["Rename"]);
		}

		public JsonData SaveToJson()
		{
			JsonObject obj = new();

			obj["Identity"] = this.Identity.SaveToJson();
			obj["Rename"] = this.Rename.SaveToJson();

			return obj;
		}

		// TODO hack
		public static LoadedTrack LoadJson(JsonData data)
		{
			LoadedTrack track = new();
			track.LoadFromJson(data);
			return track;
		}
	}
}
