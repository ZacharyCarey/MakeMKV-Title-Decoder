using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.Data
{
	public enum DelayType
	{
		Source,
		Delay
	}

	public class DelayInfo {
		[JsonInclude, JsonConverter(typeof(JsonStringEnumConverter))]
        public DelayType DelayType = DelayType.Delay;

        /// <summary>
        /// The UID of the stream in the playlist
        /// </summary>
		[JsonInclude]
		public long StreamUID = -1;

		[JsonInclude]
		public long Milliseconds = 0;
    }

	public class PlaylistTrack
	{
        /// <summary>
        /// The source track that this class represents.
        /// The StreamUID is the UID of source/appended files in the playlist,
        /// and the TrackUID is the track UID of that source file.
        /// </summary>
        [JsonInclude]
        public TrackID PlaylistSource = new();

        /// <summary>
        /// whether or not the track should be copied to the output.
        /// Could also be called enable/disable.
        /// 
        /// If disabled, the track will be used to a create a delay
        /// for the next track
        /// </summary>
        [JsonInclude]
        public bool Copy = true;

        [JsonInclude]
        public DelayInfo? Delay = null;
    }

	public class PlaylistSourceTrack : PlaylistTrack
    {
		/// <summary>
		/// Only allows a depth of 1. I.e. an appended track can't itself have appended tracks
		/// </summary>
		[JsonInclude]
		public List<PlaylistTrack> AppendedTracks = new();
	}

}
