using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.Data
{

	public class PlaylistFile
	{

		/// <summary>
		/// The UID of the file on the disc
		/// </summary>
        [JsonInclude]
        public long SourceUID = -1;
    }
}
