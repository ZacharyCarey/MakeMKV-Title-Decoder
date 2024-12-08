using libbluray.bdnav.Mpls;
using MakeMKV_Title_Decoder.Forms.FileRenamer;
using MkvToolNix;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Utils;

namespace MakeMKV_Title_Decoder.Data
{
	public class Playlist : Exportable
	{

		/// <summary>
		/// Title that shows in VLC when played (i.e. title saved in file)
		/// </summary>
		[JsonInclude]
		public string? Title = null;

		/// <summary>
		/// The user given name of the playlist for renaming purposes
		/// </summary>
		[JsonInclude]
		public string? Name = null;

		/// <summary>
		/// How the output file should be renamed based on season, episode, type, etc
		/// </summary>
		[JsonInclude]
		public OutputName OutputFile { get; private set; } = new();

		/// <summary>
		/// Source files added to the playlist
		/// </summary>
		[JsonInclude]
		public List<PlaylistSourceFile> SourceFiles = new();

		[JsonInclude]
		public List<PlaylistSourceTrack> SourceTracks = new();

		[JsonIgnore]
        string Exportable.Name => this.Name ?? "N/A";

        public static Playlist? Import(LoadedDisc disc, DiscPlaylist importPlaylist)
		{
			int streamUID = 0;

			Playlist playlist = new();
			playlist.Name = importPlaylist.Name;
			playlist.Title = importPlaylist.Name;

			foreach(var streamPath in importPlaylist.SourceFiles)
			{
				LoadedStream? stream = disc.TryGetStreamFromPath(streamPath);
				if (stream == null) return null;
				if (string.IsNullOrWhiteSpace(stream.RenameData.Name)) continue; // Dont import clips that haven't been renamed. This helps skip clips that are blank or not wanted.

				bool first = (playlist.SourceFiles.Count == 0);
				if (first)
				{
					PlaylistSourceFile sourceFile = new PlaylistSourceFile();
                    sourceFile.SourceUID = stream.RenameData.UID;
                    sourceFile.PlaylistUID = streamUID++;
					playlist.SourceFiles.Add(sourceFile);

					foreach(var track in stream.Tracks)
					{
						PlaylistSourceTrack sourceTrack = new PlaylistSourceTrack();
						sourceTrack.PlaylistSource = new();
						sourceTrack.PlaylistSource.StreamUID = sourceFile.PlaylistUID;
						sourceTrack.PlaylistSource.TrackUID = track.RenameData.UID;
						playlist.SourceTracks.Add(sourceTrack);
					}
				} else
				{
					PlaylistFile file = new PlaylistFile();
					file.SourceUID = stream.RenameData.UID;
					file.PlaylistUID = streamUID++;
					playlist.SourceFiles[0].AppendedFiles.Add(file);

					foreach((int index, var track) in stream.Tracks.WithIndex())
					{
						PlaylistTrack playlistTrack = new PlaylistTrack();
						playlistTrack.PlaylistSource = new();
						playlistTrack.PlaylistSource.StreamUID = file.PlaylistUID;
						playlistTrack.PlaylistSource.TrackUID = track.RenameData.UID;

						int i = index;
						if (i >= playlist.SourceTracks.Count)
						{
							i = playlist.SourceTracks.Count - 1;
						}
						playlist.SourceTracks[i].AppendedTracks.Add(playlistTrack);
					}
				}
			}

			return playlist;
		} 

		private MergeData GetMergeData(LoadedDisc disc)
		{
			var mergeData = new MergeData();
			mergeData.Title = this.Title;

			Dictionary<long, MkvToolNix.SourceFile> SourceFileLookup = new();
			Dictionary<long, MkvToolNix.AppendedFile> AppendedFileLookup = new();
			Dictionary<long, PlaylistFile> playlistFiles = new();
			foreach (var sourceFile in this.SourceFiles)
			{
				string relativeFilePath = disc[sourceFile.SourceUID].Identity.SourceFile;
				string fullFilePath = Path.Combine(disc.Root, relativeFilePath);
				MkvToolNix.SourceFile source = mergeData.AddSourceFile(fullFilePath);
				SourceFileLookup[sourceFile.PlaylistUID] = source;
				playlistFiles[sourceFile.PlaylistUID] = sourceFile;

				foreach(var appendedFile in sourceFile.AppendedFiles)
				{
					relativeFilePath = disc[appendedFile.SourceUID].Identity.SourceFile;
					fullFilePath = Path.Combine(disc.Root, relativeFilePath);
					MkvToolNix.AppendedFile appended = source.AddAppendedFile(fullFilePath);
					AppendedFileLookup[appendedFile.PlaylistUID] = appended;
					playlistFiles[appendedFile.PlaylistUID] = appendedFile;
				}
			}

			long sourceDelay = 0;
			foreach((int index, var playlistSourceTrack) in this.SourceTracks.WithIndex())
			{
				if (playlistSourceTrack.Delay != null)
				{
					sourceDelay += CalculateDelay(playlistSourceTrack.Delay, disc, playlistFiles);
				}
				else
				{
					var playlistFile = playlistFiles[playlistSourceTrack.PlaylistSource.StreamUID];
					var loadedTrack = disc[playlistFile.SourceUID, playlistSourceTrack.PlaylistSource.TrackUID];

					MkvToolNix.SourceFile sourceFile = SourceFileLookup[playlistSourceTrack.PlaylistSource.StreamUID];
					var sourceTrack = sourceFile.AddTrack(loadedTrack.MkvToolNixID, loadedTrack.Identity.Type ?? MkvToolNix.Data.MkvTrackType.Unknown, index);

					sourceTrack.CommentaryFlag = loadedTrack.RenameData.CommentaryFlag ?? loadedTrack.Identity.FlagCommentary;
					sourceTrack.CopyToOutput = playlistSourceTrack.Copy;
					sourceTrack.DefaultTrackFlag = loadedTrack.RenameData.DefaultFlag ?? loadedTrack.Identity.FlagCommentary;
					sourceTrack.EnableFlag = null;
					sourceTrack.ForcedDisplayFlag = null;
					sourceTrack.HearingImpairedFlag = loadedTrack.Identity.FlagHearingImpaired;
					sourceTrack.Language = loadedTrack.RenameData.Language ?? loadedTrack.Identity.Language;
					sourceTrack.Name = loadedTrack.RenameData.Name ?? loadedTrack.Identity.TrackName;
					sourceTrack.OriginalFlag = loadedTrack.Identity.FlagOriginal;
					sourceTrack.VisualImpairedFlag = loadedTrack.Identity.FlagVisualImpaired;

                    if (sourceDelay > 0) sourceTrack.DelayMS = sourceDelay;
					sourceDelay = 0;

					MkvToolNix.Track lastTrack = sourceTrack;
					long appendedDelay = 0;
					foreach (var playlistAppendedTrack in playlistSourceTrack.AppendedTracks)
					{
						if (playlistAppendedTrack.Delay != null)
						{
							appendedDelay += CalculateDelay(playlistAppendedTrack.Delay, disc, playlistFiles);
                        } else {
							playlistFile = playlistFiles[playlistAppendedTrack.PlaylistSource.StreamUID];
							loadedTrack = disc[playlistFile.SourceUID, playlistAppendedTrack.PlaylistSource.TrackUID];

							MkvToolNix.AppendedFile appendedFile = AppendedFileLookup[playlistAppendedTrack.PlaylistSource.StreamUID];
							var appendedTrack = appendedFile.AddTrack(loadedTrack.MkvToolNixID, loadedTrack.Identity.Type ?? MkvToolNix.Data.MkvTrackType.Unknown, lastTrack);

							appendedTrack.CopyToOutput = playlistAppendedTrack.Copy;
							if (playlistAppendedTrack.Copy == false)
							{
								DelayInfo delayInfo = new DelayInfo();
								delayInfo.DelayType = DelayType.Source;
								delayInfo.StreamUID = playlistAppendedTrack.PlaylistSource.StreamUID;
								appendedDelay += CalculateDelay(delayInfo, disc, playlistFiles);
							} else
							{
								if (appendedDelay > 0)
								{
									appendedTrack.DelayMS = appendedDelay;
									appendedDelay = 0;
								}
                            }

                            lastTrack = appendedTrack;
						}
					}
				}
			}

			return mergeData;
		}

		private static long CalculateDelay(DelayInfo delay, LoadedDisc disc, Dictionary<long, PlaylistFile> playlistFiles)
		{
            if (delay.DelayType == DelayType.Delay)
            {
				return delay.Milliseconds;
            }
            else if (delay.DelayType == DelayType.Source)
            {
				return (long)disc[playlistFiles[delay.StreamUID].SourceUID].Duration.TotalMilliseconds;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Failed to parse delay!");
                Console.ResetColor();
				return 0;
            }
        }

        bool Exportable.Export(LoadedDisc disc, string outputFolder, string outputFile, IProgress<SimpleProgress>? progress, SimpleProgress? totalProgress)
        {
            return MkvToolNix.MkvToolNixInterface.Merge(this.GetMergeData(disc), Path.Combine(outputFolder, outputFile), progress, totalProgress) == null;
        }
    }
}
