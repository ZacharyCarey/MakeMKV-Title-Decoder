using MkvToolNix.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.Data.Renames
{
	public class ClipRename
	{
		[JsonInclude]
		public string? Name { get; set; }

		[JsonInclude]
		public StreamIdentity Identity { get; set; }

		[JsonInclude]
		public List<TrackRename> Tracks { get; }

		/// <summary>
		/// Used to identify the clip within the disc (i.e. index)
		/// </summary>
		[JsonInclude]
		public int UID = -1;

		public ClipRename(StreamIdentity identification) { 
			//try
			//{
			//	this.Name = Path.GetFileNameWithoutExtension(identification.SourceFile);
			//} catch(Exception)
			//{
			//	this.Name = "null";
			//}
			this.Identity = identification;
			this.Tracks = new();
		}

		[JsonConstructor]
		private ClipRename(string? name, StreamIdentity identity, List<TrackRename> tracks, int uid) { 
			this.Name = name;
			this.Identity = identity;
			this.Tracks = tracks;
			this.UID = uid;
		}

	}
}
