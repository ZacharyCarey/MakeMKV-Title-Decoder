using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.Data
{
	public class TrackID
	{
		[JsonInclude]
		public long StreamUID;

		[JsonInclude]
		public long TrackUID;
	}
}
