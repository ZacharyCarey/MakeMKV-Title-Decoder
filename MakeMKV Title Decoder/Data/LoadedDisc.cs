using MakeMKV_Title_Decoder.Data.BluRay;
using MakeMKV_Title_Decoder.Data.Renames;
using MkvToolNix.Data;
using MkvToolNix;
using PgcDemuxLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Utils;
using MakeMKV_Title_Decoder.Data.DVD;

namespace MakeMKV_Title_Decoder.Data
{

	// TODO probably needs a better name
	public abstract class LoadedDisc
	{
		public DiscIdentity Identity { get => RenameData.DiscID; }
		public List<LoadedStream> Streams { get; }

        public string Root { get; }
        public string? Title { get => Identity.Title; }
        public long? NumberOfSets { get => Identity.NumberOfSets; }
        public long? SetNumber { get => Identity.SetNumber; }
		public abstract bool ForceVlcTrackIndex { get; }
		public abstract bool SupportsDeinterlacing { get; }

		public RenameData RenameData { get; private set; }

        protected LoadedDisc(string root, string? title, long? numSets, long? setNum, List<LoadedStream> streams)
		{
			this.Root = root;
			DiscIdentity discID = new(title, numSets, setNum);
            this.RenameData = new(discID);
			this.Streams = streams;
			int streamUID = 0;
			foreach (var stream in streams)
			{
				stream.RenameData.UID = streamUID++;
				this.RenameData.Clips.Add(stream.RenameData);
			}
        }

		public static LoadedDisc? TryLoadDisc(string rootFolder, IProgress<SimpleProgress>? progress = null)
		{
			try
			{
				if (!Directory.Exists(rootFolder)) throw new Exception("Folder does not exist.");
				
				if (Directory.Exists(Path.Combine(rootFolder, "VIDEO_TS")))
				{
					return DvdDisc.Open(rootFolder, progress);
				} else if (Directory.Exists(Path.Combine(rootFolder, "BDMV")))
				{
					return BlurayDisc.Open(rootFolder, progress);
				} else
				{
					throw new Exception("Unknown disc type.");
				}
			}catch(Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine($"Failed to load disc: {ex.Message}");
				Console.ResetColor();
				return null;
			}
		}

		public bool TryLoadRenameData(RenameData renameData)
		{
			try
			{
				string? error = this.Identity.Match(renameData.DiscID);
				if (error != null) throw new Exception(error);

				// TODO better matching algorithm
				/*
                 * If there are more streams on the disc than in the rename data, but all
                 * the streams in the rename data do match, ask user if they want to continue
                 * and add the new stream in the data. In my experience, some disc drives have
                 * issues reading certain streams so it would be nice to be able to fix
                 * the issue later when the stream is able to be read.
                 */
				// For now, just do strict matching, in order. Not very flexible.
				Dictionary<LoadedStream, ClipRename> streamMatches = new();
                if (this.Streams.Count != renameData.Clips.Count) throw new Exception("Number of streams does not match.");
                for (int i = 0; i < this.Streams.Count; i++)
				{
					var stream = this.Streams[i];
					var clipRename = renameData.Clips[i];
					error = stream.TryMatchRenameData(clipRename);
					if (error != null) throw new Exception(error);
					streamMatches[stream] = clipRename;
				}

				// Extract attachments as needed
				foreach(var attachment in renameData.Attachments)
				{
					if (!attachment.Extract(this))
					{
						throw new Exception("Failed to extract attachment.");
					}
				}

				// Was able to match everything, now actually load the rename data
				renameData.DiscID = this.Identity; // So if there are any changes in the discID they will be saved
				this.RenameData = renameData;
				foreach(var stream in this.Streams)
				{
					stream.LoadRenameData(streamMatches[stream]);
				}

                // Successfully matched rename data, now regenerate any deinterlaced videos as needed
                SimpleProgress currentProgress = new(0, (uint)this.Streams.Count * 2);
                bool failed = TaskProgressViewerForm.Run((IProgress<SimpleProgress> progress) => {
                    bool failure = false;
                    foreach ((int index, var stream) in this.Streams.WithIndex())
                    {
                        currentProgress.Total = (uint)index * 2;
                        bool result = this.GenerateVideo(stream, stream.RenameData.Deinterlaced, progress, currentProgress);
                        if (!result) failure |= true;
                    }
                    return failure;
                });

                if (failed)
                {
                    MessageBox.Show("Failed to deinterlace some videos. Recommend deleting temporary files and reloading the disc.");
                }
            }
            catch (Exception e)
			{
				Console.WriteLine($"Failed to match rename data: {e.Message}");
				MessageBox.Show("Failed to match rename data: " + e.Message, "Failed to load", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}

			return true;
        }

		public abstract List<DiscPlaylist> GetPlaylists();

        protected static MkvMergeID? ParseIdentify(string root, string directory, string fileName)
        {
            MkvMergeID? identification = MkvToolNixInterface.Identify(root, directory, fileName);
            if (identification == null)
            {
                // TODO move to logger??
                // TODO how to handle error?
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Failed to identify stream file: {fileName}");
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine($"Identified stream file: {fileName}");
            }
            return identification;
        }

		public LoadedStream this[long UID]
		{
			get
			{
				foreach(var stream in this.Streams)
				{
					if (stream.RenameData.UID == UID)
					{
						return stream;
					}
				}
				throw new IndexOutOfRangeException();
			}
		}

		public LoadedTrack this[long StreamUID, long TrackUID]
		{
			get
			{
				LoadedStream stream = this[StreamUID];
				foreach (var track in stream.Tracks)
				{
					if (track.RenameData.UID == TrackUID)
					{
						return track;
					}
				}
				throw new IndexOutOfRangeException();
			}
		}

		public LoadedTrack this[TrackID ID]
		{
			get => this[ID.StreamUID, ID.TrackUID];
		}

		public LoadedStream? TryGetStreamFromPath(string path)
		{
			foreach (var stream in this.Streams)
			{
				if (stream.Identity.SourceFile == path)
				{
					return stream;
				}
			}
			return null;
		}

		/// <summary>
		/// Used to regenerate the video later when deinterlacing settings change.
		/// This is only used for DVD where the video is transcoded, and other 
		/// types of media will simply return false if this function is called.
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="progress"></param>
		/// <param name="baseProgress"></param>
		/// <returns></returns>
		protected abstract bool GenerateVideo(LoadedStream stream, bool newDeinterlaceSetting, IProgress<SimpleProgress> progress, SimpleProgress baseProgress);
    
		public bool SetDeinterlace(LoadedStream stream, bool deinterlace, IProgress<SimpleProgress> progress, SimpleProgress currentProgress) {
			bool result = GenerateVideo(stream, deinterlace, progress, currentProgress);
			if (!result) return false;

			//stream.RenameData.Deinterlaced = deinterlace;

			// Regenerate attachments as needed
			bool failed = false;
			foreach(var attachment in this.RenameData.Attachments)
			{
				if (attachment.FilePath == stream.Identity.SourceFile)
				{
					if (!attachment.Extract(this))
					{
						failed |= true;
					}
				}
			}

			if (failed)
			{
				MessageBox.Show("Failed regenerating attachments. Try deleting temporary files and reloading disc.");
			}

			return true;
		}
	}

	public struct DiscPlaylist
	{
		public string Name;
		public List<string> SourceFiles = new();

		public DiscPlaylist(string name)
		{
			this.Name = name;
		}

		public DiscPlaylist DeepCopy()
		{
			DiscPlaylist result = new();
			result.Name = this.Name;
			result.SourceFiles = new List<string>(this.SourceFiles);
			return result;
        }
	}
}
