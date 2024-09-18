using JsonSerializable;
using MakeMKV_Title_Decoder.libs.MkvToolNix.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.Data.Renames
{
	public class TrackIdentity : IJsonSerializable
	{

		public string? Codec = null;
		public MkvTrackType? Type = null;

		// Track -> Properties ->
		public long? AudioBitsPerSample = null;
		public long? AudioChannels = null;
		public long? AudioSamplingFrequency = null;
		public string? CodecID = null;
		public string? DisplayDimensions = null;
		public long? DisplayUnit = null;
		public bool? FlagHearingImpaired = null;
		public bool? FlagVisualImpaired = null;
		public bool? FlagTextDescriptions = null;
		public bool? FlagOriginal = null;
		public bool? FlagCommentary = null;
		public string? Language = null;
		public long? Number = null;
		public string? PixelDimensions = null;
		public string? TrackName = null;

		public void LoadFromMkvToolNix(LoadedTrack loadedTrack)
		{
			var track = loadedTrack.Data;

			this.Codec = track.Codec;
			this.Type = track.Type;
			this.AudioBitsPerSample = track.Properties?.AudioBitsPerSample;
			this.AudioChannels = track.Properties?.AudioChannels;
			this.AudioSamplingFrequency = track.Properties?.AudioSamplingFrequency;
			this.CodecID = track.Properties?.CodecID;
			this.DisplayDimensions = track.Properties?.DisplayDimensions;
			this.DisplayUnit = track.Properties?.DisplayUnit;
			this.FlagHearingImpaired = track.Properties?.FlagHearingImpaired;
			this.FlagVisualImpaired = track.Properties?.FlagVisualImpaired;
			this.FlagTextDescriptions = track.Properties?.FlagTextDescriptions;
			this.FlagOriginal = track.Properties?.FlagOriginal;
			this.FlagCommentary = track.Properties?.FlagCommentary;
			this.Language = track.Properties?.Language;
			this.Number = track.Properties?.Number;
			this.PixelDimensions = track.Properties?.PixelDimensions;
			this.TrackName = track.Properties?.TrackName;
		}

		public void LoadFromJson(JsonData Data)
		{
			Data.LoadJson("Codec", out this.Codec);
			Data.LoadJson("Type", out this.Type);
			Data.LoadJson("Audio Bits per Sample", out this.AudioBitsPerSample);
			Data.LoadJson("Audio Channels", out this.AudioChannels);
			Data.LoadJson("AudioSamplingFrequency", out this.AudioSamplingFrequency);
			Data.LoadJson("Codec ID", out this.CodecID);
			Data.LoadJson("Display Dimensions", out this.DisplayDimensions);
			Data.LoadJson("Display Unit", out this.DisplayUnit);
			Data.LoadJson("Hearing Impaired", out this.FlagHearingImpaired);
			Data.LoadJson("Visual Impaired", out this.FlagVisualImpaired);
			Data.LoadJson("Original", out this.FlagOriginal);
			Data.LoadJson("Commentary", out this.FlagCommentary);
			Data.LoadJson("Language", out this.Language);
			Data.LoadJson("Number", out this.Number);
			Data.LoadJson("Pixel Dimensions", out this.PixelDimensions);
			Data.LoadJson("Track Name", out this.TrackName);
		}

		public JsonData SaveToJson()
		{
			JsonObject obj = new();

			obj["Codec"] = Utils.SaveJson(Codec);
			obj["Type"] = Utils.SaveJson(Type);
			obj["Audio Bits per Sample"] = Utils.SaveJson(AudioBitsPerSample);
			obj["Audio Channels"] = Utils.SaveJson(AudioChannels);
			obj["AudioSamplingFrequency"] = Utils.SaveJson(AudioSamplingFrequency);
			obj["Codec ID"] = Utils.SaveJson(CodecID);
			obj["Display Dimensions"] = Utils.SaveJson(DisplayDimensions);
			obj["Display Unit"] = Utils.SaveJson(DisplayUnit);
			obj["Hearing Impaired"] = Utils.SaveJson(FlagHearingImpaired);
			obj["Visual Impaired"] = Utils.SaveJson(FlagVisualImpaired);
			obj["Text Descriptions"] = Utils.SaveJson(FlagTextDescriptions);
			obj["Original"] = Utils.SaveJson(FlagOriginal);
			obj["Commentary"] = Utils.SaveJson(FlagCommentary);
			obj["Language"] = Utils.SaveJson(Language);
			obj["Number"] = Utils.SaveJson(Number);
			obj["Pixel Dimensions"] = Utils.SaveJson(PixelDimensions);
			obj["Track Name"] = Utils.SaveJson(TrackName);

			return obj;
		}
	}
}
