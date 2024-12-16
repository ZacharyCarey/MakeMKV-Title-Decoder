using FFMpeg_Wrapper.ffprobe;
using Iso639;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Utils;

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

		[JsonInclude, JsonConverter(typeof(LanguageJsonConverter))]
		public Language? Language;

		[JsonInclude]
		public TrackIdentity Identity { get; set; }

		/// <summary>
		/// Used to identify this track within the clip (i.e. index)
		/// </summary>
		[JsonInclude]
		public int UID = -1;

		private TrackRename(TrackIdentity id, MediaStream info) {
			this.Identity = id;
			this.Name = id.Title;
			this.CommentaryFlag = id.CommentFlag;
			this.DefaultFlag = id.DefaultFlag ?? false;
			this.Language = id.Language;
		}

        public TrackRename(VideoStream info) : this(new TrackIdentity(info), info) {

		}

		public TrackRename(AudioStream info) : this(new TrackIdentity(info), info) {

		}

		public TrackRename(SubtitleStream info) : this(new TrackIdentity(info), info) {

		}

		[JsonConstructor]
		private TrackRename(string? name, bool? commentaryFlag, bool? defaultFlag, TrackIdentity identity, int uid, Language? language) {
			this.Name = name;
			this.CommentaryFlag = commentaryFlag;
			this.DefaultFlag = defaultFlag;
			this.Identity = identity;
			this.UID = uid;
			this.Language = language;
		}

	}
}
