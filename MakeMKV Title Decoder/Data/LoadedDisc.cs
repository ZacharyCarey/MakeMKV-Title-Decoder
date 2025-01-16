using MakeMKV_Title_Decoder.Data.BluRay;
using MakeMKV_Title_Decoder.Data.Renames;
using PgcDemuxLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Utils;
using MakeMKV_Title_Decoder.Data.DVD;
using FFMpeg_Wrapper.ffprobe;

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

		/// <summary>
		/// Used mostly for DVD's where devices have issues playing
		/// the older MPEG streams, so we force transcoding original files
		/// to fix streaming issues.
		/// </summary>
		public abstract bool ForceTranscoding { get; }

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

				// Was able to match everything, now actually load the rename data
				renameData.DiscID = this.Identity; // So if there are any changes in the discID they will be saved
				this.RenameData = renameData;
				foreach(var stream in this.Streams)
				{
					stream.LoadRenameData(streamMatches[stream]);
				}

                // Extract attachments as needed
                foreach (var attachment in renameData.Attachments)
                {
                    if (!attachment.Extract(this))
                    {
                        throw new Exception("Failed to extract attachment.");
                    }
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

        protected static MediaAnalysis? ParseIdentify(FFProbe ffprobe, string root, string directory, string fileName)
        {
			MediaAnalysis? identification = ffprobe.Analyse(Path.Combine(root, directory, fileName));
            if (identification == null)
            {
				Log.Error($"Failed to identify stream file: {fileName}");
            }
            else
            {
				Log.Info($"Identified stream file: {fileName}");
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

		public LoadedStream? TryGetStreamFromPath(string path, bool ignoreExtension = false)
		{
			foreach (var stream in this.Streams)
			{
				bool sameFile;
				if (ignoreExtension)
				{
					sameFile = (Path.GetFileNameWithoutExtension(stream.Identity.SourceFile) == Path.GetFileNameWithoutExtension(path));
				} else
				{
					sameFile = (stream.Identity.SourceFile == path);
				}

				if (sameFile)
				{
					return stream;
				}
			}
			return null;
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
