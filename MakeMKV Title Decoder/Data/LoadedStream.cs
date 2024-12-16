using FFMpeg_Wrapper.ffprobe;
using libbluray.bdnav.Clpi;
using MakeMKV_Title_Decoder.Data.Renames;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Utils;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MakeMKV_Title_Decoder.Data
{
	public abstract class LoadedStream
	{
		public StreamIdentity Identity { get => RenameData.Identity; }
        public ClipRename RenameData { get; private set; }
        public readonly MediaAnalysis FFProbeInfo;

		public List<LoadedTrack> Tracks { get; }

        Dictionary<LoadedTrack, TrackRename>? TrackMatches = null;

        /// <summary>
        /// Retrieves the path to the source file on the disc.
        /// </summary>
        public string GetFullPath(LoadedDisc disc) {
            return Path.Combine(disc.Root, this.Identity.SourceFile);
        }

        public TimeSpan Duration { get; protected set; }

        protected LoadedStream(string root, string filePath, MediaAnalysis info) {
			var fileSize = DataSize.FromFile(Path.Combine(root, filePath)) ?? new DataSize();
			var ID = new StreamIdentity(filePath, fileSize, info);
            this.FFProbeInfo = info;
            this.RenameData = new ClipRename(ID);
            this.Tracks = info.VideoStreams.Select(x => new LoadedTrack(x, this))
                    .Concat(info.AudioStreams.Select(x => new LoadedTrack(x, this)))
                    .Concat(info.SubtitleStreams.Select(x => new LoadedTrack(x, this)))
                    .ToList();
            int trackUID = 0;
            foreach(var track in this.Tracks)
            {
                track.RenameData.UID = trackUID++;
                this.RenameData.Tracks.Add(track.RenameData);
            }
		}

		public LoadedTrack? GetTrack(long index)
		{
			if (index < 0 || index >= Tracks.Count) return null;
			return Tracks[(int)index];
		}


		internal string? TryMatchRenameData(ClipRename renameData)
		{
            this.TrackMatches = null;

            string? error = this.Identity.Match(renameData.Identity);
            if (error != null) return error;

            // TODO better matching algorithm
            /*
             * If there are more streams on the disc than in the rename data, but all
             * the streams in the rename data do match, ask user if they want to continue
             * and add the new stream in the data. In my experience, some disc drives have
             * issues reading certain streams so it would be nice to be able to fix
             * the issue later when the stream is able to be read.
             */
            // For now, just do strict matching, in order. Not very flexible.
            Dictionary<LoadedTrack, TrackRename>? trackMatches = new();
            if (this.Tracks.Count != renameData.Tracks.Count) throw new Exception("Number of streams does not match.");
            for (int i = 0; i < this.Tracks.Count; i++)
            {
                var track = this.Tracks[i];
                var trackRename = renameData.Tracks[i];
                error = track.TryMatchRenameData(trackRename);
                if (error != null) return error;
                trackMatches[track] = trackRename;
            }

            this.TrackMatches = trackMatches;
            return null;
        }

        internal void LoadRenameData(ClipRename renameData)
        {
            renameData.Identity = this.Identity; // So if there are any changes in the streamID they will be saved
            this.RenameData = renameData;
            foreach (var track in this.Tracks)
            {
                track.LoadRenameData(TrackMatches[track]);
            }
        }
    }
}
