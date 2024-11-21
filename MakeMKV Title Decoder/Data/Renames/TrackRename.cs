using MkvToolNix.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.Data.Renames
{
	public class TrackRename
	{
		[JsonInclude]
		public string? Name;

		[JsonInclude]
		public bool? CommentaryFlag;

		[JsonInclude]
		public bool? DefaultFlag;

		[JsonInclude]
		public TrackIdentity Identity { get; set; }

		/// <summary>
		/// Used to identify this track within the clip (i.e. index)
		/// </summary>
		[JsonInclude]
		public int UID = -1;

        public TrackRename(MkvTrack info) {
			this.Identity = new TrackIdentity(info);

			this.Name = info.Properties?.TrackName;
			this.CommentaryFlag = info.Properties?.FlagCommentary;
			this.DefaultFlag = info.Properties?.DefaultTrack;
		}

		[JsonConstructor]
		private TrackRename(string? name, bool? commentaryFlag, bool? defaultFlag, TrackIdentity identity, int uid) {
			this.Name = name;
			this.CommentaryFlag = commentaryFlag;
			this.DefaultFlag = defaultFlag;
			this.Identity = identity;
			this.UID = uid;
		}

	}
}
