using MakeMKV_Title_Decoder.Data.Renames;
using MkvToolNix.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Principal;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.Data
{
	public class LoadedTrack
	{
		public TrackIdentity Identity => RenameData.Identity;
		public TrackRename RenameData { get; private set; }
        public LoadedStream SourceFile { get; }

        /// <summary>
        /// Identifies this track within the clip for MkvToolNix. ID is unique to this clip but not other clips.
        /// </summary>
        [JsonIgnore]
        public long MkvToolNixID;

        public LoadedTrack(MkvTrack track, LoadedStream source)
		{
			this.RenameData = new TrackRename(track);
            this.MkvToolNixID = track.ID;
            this.SourceFile = source;
        }

        internal string? TryMatchRenameData(TrackRename renameData)
        {
            // TODO better matching algorithm
            /*
             * If there are more streams on the disc than in the rename data, but all
             * the streams in the rename data do match, ask user if they want to continue
             * and add the new stream in the data. In my experience, some disc drives have
             * issues reading certain streams so it would be nice to be able to fix
             * the issue later when the stream is able to be read.
             */
            // For now, just do strict matching, in order. Not very flexible.
            string? error = this.Identity.Match(renameData.Identity);
            if (error != null) return error;

            return null;
        }

        internal void LoadRenameData(TrackRename renameData)
        {
            renameData.Identity = this.Identity; // So if there are any changes in the streamID they will be saved
            this.RenameData = renameData;
        }
    }
}
