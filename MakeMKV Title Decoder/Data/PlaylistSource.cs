using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.Data
{

	public class PlaylistSourceFile : PlaylistFile
	{
		[JsonInclude]
		public List<PlaylistFile> AppendedFiles = new();
	}

	public class PlaylistFile
	{

		/// <summary>
		/// The UID of the file on the disc
		/// </summary>
        [JsonInclude]
        public long SourceUID = -1;

		/// <summary>
		/// The UID of this particular source file in the playlist. This is because
		/// the same disc source file can be used multiple times in the same playlist.
		/// </summary>
		[JsonInclude]
		public long PlaylistUID = -1;
    }
}
