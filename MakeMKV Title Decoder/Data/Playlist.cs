using FFMpeg_Wrapper.Codecs;
using FFMpeg_Wrapper.ffmpeg;
using FFMpeg_Wrapper.ffprobe;
using FFMpeg_Wrapper.Filters;
using FFMpeg_Wrapper.Filters.Video;
using libbluray.bdnav.Mpls;
using MakeMKV_Title_Decoder.Data.Renames;
using MakeMKV_Title_Decoder.Forms.FileRenamer;
using MakeMKV_Title_Decoder.libs.MakeMKV.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;
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
		public string Name = "New Playlist";

		/// <summary>
		/// How the output file should be renamed based on season, episode, type, etc
		/// </summary>
		[JsonInclude]
		public OutputName OutputFile { get; private set; } = new();

		/// <summary>
		/// Source files added to the playlist
		/// </summary>
		[JsonInclude]
		public List<PlaylistFile> SourceFiles = new();

		[JsonInclude]
		public List<PlaylistTrack> SourceTracks = new();

		[JsonIgnore]
        string Exportable.Name => this.Name ?? "N/A";

		[JsonIgnore]
		bool Exportable.IsTranscodable => true;

        public static Playlist? Import(LoadedDisc disc, DiscPlaylist importPlaylist)
		{
			Playlist playlist = new();
			playlist.Name = importPlaylist.Name;
			playlist.Title = importPlaylist.Name;

			foreach(var streamPath in importPlaylist.SourceFiles)
			{
				LoadedStream? stream = disc.TryGetStreamFromPath(streamPath, true);
				if (stream == null) return null;
				if (string.IsNullOrWhiteSpace(stream.RenameData.Name)) continue; // Dont import clips that haven't been renamed. This helps skip clips that are blank or not wanted.

				bool first = (playlist.SourceFiles.Count == 0);
				if (first)
				{
					foreach(var track in stream.Tracks)
					{
						PlaylistTrack sourceTrack = new PlaylistTrack();
						sourceTrack.Copy = true;
						playlist.SourceTracks.Add(sourceTrack);
					}
				}

				PlaylistFile file = new PlaylistFile();
				file.SourceUID = stream.RenameData.UID;
				playlist.SourceFiles.Add(file);
			}

			return playlist;
		} 

		private FFMpegCliArgs GetMergeData(FFMpeg ffmpeg, LoadedDisc disc, OutputFile outputFile, ScaleResolution? resolution = null)
		{
			if (this.Title != null) outputFile.Title = this.Title;
			var args = ffmpeg.Transcode(outputFile);

			// Adds all the source files into a concat arg so that they are combined into a single file
			int concatInputIndex = args.AddInput(
				new ConcatArguments()
					.AddFiles(this.SourceFiles.Select(file => disc[file.SourceUID].FFProbeInfo))
			);

			int nextFilterName = 0;
			LoadedStream sourceFile = disc[this.SourceFiles[0].SourceUID];

			// Find the "main" video in case there is a PiP track
			// We only look for disabled tracks since the overly will copy the stream in the transcoding process
			LoadedTrack? pipMainVideo = null;
			foreach((PlaylistTrack track, LoadedTrack loadedTrack) in this.SourceTracks.Zip(sourceFile.Tracks))
			{
				if ((track.Copy == false) && (track.PictureInPicture == false) && (loadedTrack.Identity.TrackType == Renames.TrackType.Video) && (loadedTrack.RenameData.DefaultFlag == true))
				{
					pipMainVideo = loadedTrack;
					break;
				}
			}

			if (pipMainVideo == null) {
				// Failed to find a default video track, fallback to any non-default video track that is being copied
				foreach ((PlaylistTrack track, LoadedTrack loadedTrack) in this.SourceTracks.Zip(sourceFile.Tracks))
				{
					if ((track.Copy == false) && (track.PictureInPicture == false) && (loadedTrack.Identity.TrackType == Renames.TrackType.Video))
					{
						pipMainVideo = loadedTrack;
						break;
					}
				}
			}

			// Add all the streams in the correct order, applying relevent options
            foreach ((PlaylistTrack track, LoadedTrack loadedTrack) in this.SourceTracks.Zip(sourceFile.Tracks))
			{
				if (track.Copy) {
					TrackRename rename = loadedTrack.RenameData;
                    if (loadedTrack.FFProbeInfo is VideoStream videoStream)
					{
						VideoStreamOptions options;
						FilterArguments? filter = null;
						string? namedVideoInput = null;

						bool failedPIP = false;
						if (track.PictureInPicture && pipMainVideo == null) {
							failedPIP = true;
							Log.Warn("Picture in Picture was selected, but no valid 'main video' was found. Stream will be copied as normal.");
						}


						if (track.PictureInPicture == false || failedPIP) {
							if (sourceFile.RenameData.Deinterlaced || (resolution != null))
							{
								filter = new();
								args.AddInputFilter(filter);
								if (sourceFile.RenameData.Deinterlaced) filter.AddFilter(Filters.Bwdif);
								if (resolution != null) filter.AddFilter(Filters.Scale(resolution.Value));

								filter.AddInput(concatInputIndex, loadedTrack.Identity.Index);
								filter.AddOutput($"video{nextFilterName}");

								namedVideoInput = $"video{nextFilterName}";
								nextFilterName++;
							}
						} else if(track.PictureInPicture == true && pipMainVideo != null) {
							int? mainVideoSize = pipMainVideo.Identity.Height;
							if (resolution != null)
							{
								mainVideoSize = (int)resolution; // Gets the vertical resolution
							}

							// Create a filter for the main video
							string? mainVideoName = null;
							if (sourceFile.RenameData.Deinterlaced || (resolution != null))
							{
								mainVideoName = $"video{nextFilterName}";
								nextFilterName++;

								filter = new();
								args.AddInputFilter(filter);
								if (sourceFile.RenameData.Deinterlaced) filter.AddFilter(Filters.Bwdif);
								if (resolution != null) filter.AddFilter(Filters.Scale(resolution.Value));

								filter.AddInput(concatInputIndex, pipMainVideo.Identity.Index);
								filter.AddOutput(mainVideoName);
							}


							// Create a filter for deinterlacing/scaling the overlay video
							string? overlayVideoName = null;
							if (sourceFile.RenameData.Deinterlaced || mainVideoSize != null)
							{
								overlayVideoName = $"video{nextFilterName}";
								nextFilterName++;

								filter = new();
								args.AddInputFilter(filter);
								if (sourceFile.RenameData.Deinterlaced) filter.AddFilter(Filters.Bwdif);
								if (mainVideoSize != null) filter.AddFilter(Filters.Scale(-1, mainVideoSize.Value / 3)); // Scale overlay to desired height, keeping the original aspect ratio

								filter.AddInput(concatInputIndex, loadedTrack.Identity.Index);
								filter.AddOutput(overlayVideoName);
							}


							// Create a filter for overlaying the video
							namedVideoInput = $"video{nextFilterName}";
							nextFilterName++;

							filter = new();
							args.AddInputFilter(filter);
							filter.AddFilter(Filters.Overlay);
							
							// Main video input
							if (mainVideoName == null)
							{
								filter.AddInput(concatInputIndex, pipMainVideo.Identity.Index);
							} else
							{
								filter.AddInput(mainVideoName);
							}

							// Scaled overlay input
							if (overlayVideoName == null)
							{
								filter.AddInput(concatInputIndex, loadedTrack.Identity.Index);
							} else
							{
								filter.AddInput(overlayVideoName);
							}

							// Finished output
							filter.AddOutput(namedVideoInput);
						}

						if (namedVideoInput == null)
						{
							// Add stream directly from the input file
							options = new(concatInputIndex, loadedTrack.Identity.Index);
						} else
						{
							// Add stream using the named stream
                            options = new(namedVideoInput);
						}

                        ApplyStreamOptions(options, disc, sourceFile, rename);
						if (disc.ForceTranscoding || filter != null)
						{
							options.SetCodec(Codecs.LibSvtAV1.SetPreset(5).SetCRF(20));
						}
						args.AddStream(options);
					} else 
					{
						// Simply copy other streams
						StreamOptions options = new StreamOptions(concatInputIndex, loadedTrack.Identity.Index);
						ApplyStreamOptions(options, disc, sourceFile, rename);
						args.AddStream(options);
					}
				}
			}

			return args;
		}

		private static void ApplyStreamOptions(StreamOptions options, LoadedDisc disc, LoadedStream sourceFile, TrackRename rename) {
            if (rename.Name != null) options.SetName(rename.Name);
            if (rename.DefaultFlag != null) options.SetFlag(StreamFlag.Default, rename.DefaultFlag.Value);
            if (rename.CommentaryFlag != null) options.SetFlag(StreamFlag.Commentary, rename.CommentaryFlag.Value);
            if (rename.Language != null) options.SetLanguage(rename.Language);
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
            string? ffmpegPath = FileUtils.GetFFMpegExe();
            if (ffmpegPath == null) return false;

            FFMpeg ffmpeg = new FFMpeg();
            OutputFile file = new OutputFile(Path.Combine(outputFolder, outputFile));
            FFMpegCliArgs args = this.GetMergeData(ffmpeg, disc, file);
            return Export(disc, args, Path.Combine(outputFolder, outputFile), 0, false, progress, totalProgress);
        }

		bool Exportable.ExportTranscoding(LoadedDisc disc, string outputFolder, string outputFile, ScaleResolution resolution, IProgress<SimpleProgress>? progress, SimpleProgress? totalProgress) {
			string? ffmpegPath = FileUtils.GetFFMpegExe();
			if (ffmpegPath == null) return false;

			FFMpeg ffmpeg = new FFMpeg();
			OutputFile file = new OutputFile(Path.Combine(outputFolder, outputFile));
			FFMpegCliArgs args = this.GetMergeData(ffmpeg, disc, file, resolution);
			return Export(disc, args, Path.Combine(outputFolder, outputFile), GetVerticalResolution(resolution), true, progress, totalProgress);
		}

		private bool Export(LoadedDisc disc, FFMpegCliArgs args, string outputFilePath, int outputResolution, bool isTranscoded, IProgress<SimpleProgress>? progress, SimpleProgress? totalProgress) {
            SimpleProgress currentProgress;
			if (totalProgress.HasValue) currentProgress = totalProgress.Value;
			else currentProgress = new();
            currentProgress.CurrentMax = 100;

			string? logPath = Log.GetLogDirectory("ffmpeg");
			if (logPath != null)
			{
				logPath = Path.Combine(logPath, $"ffmpeg_{Path.GetFileNameWithoutExtension(outputFilePath)}.txt");
				args.SetLogPath(logPath);
			}

			Stopwatch timer = new();
			timer.Start();
			string? error = args
				.NotifyOnProgress((double percent) =>
				{
					currentProgress.Current = (uint)percent;
					progress?.Report(currentProgress);
				})
				//.SetLogPath()
				.SetOverwrite(true)
				.Run();
			timer.Stop();

			if (error != null)
			{
				Log.Error($"FFMpeg error: {error}");
				return false;
			}

			PrintDevStats(disc, outputFilePath, timer.Elapsed, outputResolution, isTranscoded);
			return true;
		}

		private static int GetVerticalResolution(ScaleResolution resolution) {
			switch(resolution)
			{
				case ScaleResolution.UHD_7680x4320: return 4320;
				case ScaleResolution.UHD_3840x2160: return 2160;
				case ScaleResolution.HD_1920x1080: return 1080;
				case ScaleResolution.HD_1280x720: return 720;
				case ScaleResolution.SD_720x480: return 480;
				default: return 0;
			}
		}

		private void PrintDevStats(LoadedDisc disc, string outputFilePath, TimeSpan transcodeTime, int outputResolution, bool isTranscoded) {
			TimeSpan totalInputDuration = new();
			DataSize totalInputSize = new();
			int inputResolution = 0;
			foreach (var input in this.SourceFiles)
			{
				LoadedStream file = disc[input.SourceUID];
				totalInputDuration += file.Duration;
				totalInputSize += file.Identity.FileSize;
				if (inputResolution <= 0)
				{
					inputResolution = file.Tracks
						.Select(track => track.Identity.Height ?? -1)
						.Where(height => height > 0)
						.FirstOrDefault(0);
				}
			}

			if (outputResolution <= 0)
			{
				outputResolution = inputResolution;
			}

			DataSize outputFileSize = DataSize.FromFile(outputFilePath) ?? new DataSize();

            Console.ForegroundColor = ConsoleColor.Cyan;
			Console.WriteLine($"{Path.GetFileName(outputFilePath)}{(isTranscoded ? "" : " (Copy)")}: [{inputResolution}p {outputResolution}p {totalInputDuration.TotalMinutes} {transcodeTime.TotalMinutes} {totalInputSize.AsGB()} {outputFileSize.AsGB()}]");
			Console.ResetColor();
		}
    }
}
