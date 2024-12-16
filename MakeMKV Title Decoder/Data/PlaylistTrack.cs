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
        /// whether or not the track should be copied to the output.
        /// Could also be called enable/disable.
        /// 
        /// If disabled, the track will be used to a create a delay
        /// for the next track
        /// </summary>
        [JsonInclude]
        public bool Copy = true;

        //[JsonInclude]
        //public DelayInfo? Delay = null;
    }

}
