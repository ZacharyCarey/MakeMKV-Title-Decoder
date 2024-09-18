using MakeMKV_Title_Decoder.Data.Renames;
using MakeMKV_Title_Decoder.libs.MkvToolNix.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.Data
{
	public class LoadedTrack
	{
		public const double MinimumSimilarityScore = 0.95;

		public TrackIdentity Identity { get; private set; }
		public TrackRename Rename = new();
		public MkvTrack Data { get; private set; }
		public long MkvMergeIndex { get; set; }

		public LoadedTrack(MkvTrack track)
		{
			this.Identity = new();
			this.Data = track;
			this.MkvMergeIndex = track.ID;
		}

		public LoadedTrack(TrackIdentity trackID, MkvTrack trackData)
		{
			this.Identity = trackID;
			this.Data = trackData;
			this.MkvMergeIndex = trackData.ID;
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
				double dist = Math.Abs(strDist - original.Length);
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
	}
}
