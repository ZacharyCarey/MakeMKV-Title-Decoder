using MakeMKV_Title_Decoder.Data;
using MakeMKV_Title_Decoder.Data.Renames;
using MkvToolNix;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.Forms.PlaylistCreator
{
    public class LoadedPlaylist {
        #region Color Management
        private List<Color> _ReusedColors = new();
        private static Color[] _Colors = {
                Color.FromArgb(255, 255, 255),
                Color.FromArgb(0, 255, 0),
                Color.FromArgb(0, 0, 255),
                Color.FromArgb(255, 0, 0),
                Color.FromArgb(255, 255, 0),
                Color.FromArgb(255, 0, 255),
                Color.FromArgb(0, 255, 255)
            };
        private int _ColorIndex = 0;
        private int _ColorMultiplier = 4;

        private Color GetNextColor() {
            if (_ReusedColors.Count > 0)
            {
                Color result = _ReusedColors[0];
                _ReusedColors.RemoveAt(0);
                return result;
            }

            _ColorIndex++;
            if (_ColorIndex >= _Colors.Length)
            {
                _ColorIndex = 0;
                _ColorMultiplier = Math.Max(0, _ColorMultiplier - 1);
            }

            Color baseColor = _Colors[_ColorIndex];
            return Color.FromArgb(
                baseColor.R * _ColorMultiplier / 4,
                baseColor.G * _ColorMultiplier / 4,
                baseColor.B * _ColorMultiplier / 4
            );
        }
        private void ReturnColor(Color color) {
            this._ReusedColors.Add(color);
        }
        private void ResetColors() {
            _ReusedColors.Clear();
            _ColorIndex = 0;
            _ColorMultiplier = 4;
        }
        #endregion

        #region File Management
        private List<AppendedFile> _AllFiles = new();

        // TODO instead of holding references to files, would it be easier
        // just to hold the index references, the same way the rename data works?
        private AppendedFile CreateFile(LoadedStream clip) {
            var result = new AppendedFile(clip, GetNextColor());
            _AllFiles.Add(result);
            return result;
        }

        private AppendedFile GetFileOrCreate(LoadedStream clip) {
            foreach(var file in _AllFiles)
            {
                if (file.Source == clip)
                {
                    return file;
                }
            }

            // File does not exist yet, create a new one
            return CreateFile(clip);
        }

        private bool RemoveFile(AppendedFile clip) {
            AppendedFile file = clip;
            _AllFiles.Remove(file);
            //_FileLookup.Remove(clip);
            this.ReturnColor(file.Color);
            return true;
        }

        private void ResetFiles() {
            _AllFiles.Clear();
        }
        #endregion

        public Playlist RenameData;
        public List<AppendedFile> SourceFiles = new();
        public List<AppendedTrack> SourceTracks = new();
        public string? Name;

        public LoadedPlaylist(Playlist renameData, string? name) {
            this.RenameData = renameData;
            this.Name = name;
        }

        private void AddSourceFile(AppendedFile file) {
            this.SourceFiles.Add(file);
            this.SourceTracks.AddRange(file.Source.Tracks.Select(x => new AppendedTrack(file, x, file.Color)));
        }

        private void AddAppendedFile(AppendedFile sourceFile, AppendedFile file) {
            if (!this.SourceFiles.Contains(sourceFile)) throw new Exception();
            sourceFile.AppendedFiles.Add(file);

            // Best effort to match tracks
            // TODO use better algorithm i.e. try to match up track types
            if (file.Source.Tracks.Count > 0)
            {
                int index = 0;
                AppendedTrack? lastTrack = null;
                foreach (var track in SourceTracks.Where(x => x.Source == sourceFile))
                {
                    lastTrack = track;
                    track.AppendedTracks.Add(new AppendedTrack(file, file.Source.Tracks[index], file.Color));

                    index++;
                    if (index >= file.Source.Tracks.Count)
                    {
                        break;
                    }
                }
                if (index < file.Source.Tracks.Count)
                {
                    // Just throw all remaining tracks into the last track that was found
                    if (lastTrack != null)
                    {
                        lastTrack.AppendedTracks.AddRange(file.Source.Tracks.Skip(index).Select(x => new AppendedTrack(file, x, file.Color)));
                    }
                }
            }
        }

        public AppendedFile AddSourceFile(LoadedStream file) {
            var appendedFile = CreateFile(file);
            AddSourceFile(appendedFile);
            return appendedFile;
        }

        public AppendedFile AddAppendedFile(AppendedFile parent, LoadedStream file) {
            var appendedFile = CreateFile(file);
            this.AddAppendedFile(parent, appendedFile);
            return appendedFile;
        }

        public AppendedTrack AddDelay(AppendedTrack sourceTrack, TrackDelay delayInfo) {
            var track = new AppendedTrack(null, null, Color.White);
            track.Delay = delayInfo;
            sourceTrack.AppendedTracks.Add(track);
            return track;
        }

        public AppendedFile ImportSourceFile(LoadedStream file) {
            var appendedFile = GetFileOrCreate(file);
            AddSourceFile(appendedFile);
            return appendedFile;
        }

        public AppendedFile ImportAppendedFile(AppendedFile parent, LoadedStream file) {
            var appendedFile = GetFileOrCreate(file);
            AddAppendedFile(parent, appendedFile);
            return appendedFile;
        }

        public void DeleteFileAndTracks(AppendedFile file) {
            // Cleanup any appended files (if needed)
            List<AppendedFile> temp = new(file.AppendedFiles);
            foreach(var appended in temp)
            {
                DeleteFileAndTracks(appended);
            }

            // Remove from source files (if needed)
            this.SourceFiles.Remove(file);

            // remove from appended files (if needed)
            foreach(var source in this.SourceFiles)
            {
                source.AppendedFiles.Remove(file);
            }

            // Successfully removed all file references

            // Cleanup any appended tracks
            foreach(var source in this.SourceTracks)
            {
                source.AppendedTracks.RemoveAll(x => x.Source == file);
            }

            // Cleanup any source tracks
            foreach(var source in this.SourceTracks)
            {
                if (source.Source == file)
                {
                    // Just dump the appended tracks in the first available
                    if (source.AppendedTracks.Any())
                    {
                        AppendedTrack? otherSource = this.SourceTracks.Where(x => x.Source != file).FirstOrDefault();
                        if (otherSource == null) throw new Exception("Could not find another source to move appended tracks. This should not happen");

                        otherSource.AppendedTracks.AddRange(source.AppendedTracks);
                    }
                }

                // Remove any referenced delay tracks and swap them to manual delays
                foreach(var appended in source.AppendedTracks)
                {
                    if (appended.Delay != null && appended.Delay.ClipDelay == file)
                    {
                        appended.Delay.ClipDelay = null;
                        appended.Delay.MillisecondDelay = (long)file.Source.Identity.Duration.TotalMilliseconds;
                    }
                }
            }
            this.SourceTracks.RemoveAll(x => x.Source == file);

            // Manage internal state
            this.RemoveFile(file);
        }

        /*public AppendedTrack? GetTrack(MkvTrack track) {
            foreach(var sourceTrack in this.SourceTracks)
            {
                if (sourceTrack.Track == track)
                {
                    return sourceTrack;
                } else
                {
                    foreach(var appendedTrack in sourceTrack.AppendedTracks)
                    {
                        if (appendedTrack.Track == track)
                        {
                            return appendedTrack;
                        }
                    }
                }
            }
            return null;
        }*/

        private void Swap<T>(List<T> listA, int indexA, List<T> listB, int indexB) {
            T temp = listA[indexA];
            listA[indexA] = listB[indexB];
            listB[indexB] = temp;
        }

        public bool MoveTrackUp(AppendedTrack track) {
            for(int sourceIndex = 0; sourceIndex < this.SourceTracks.Count; sourceIndex++)
            {
                AppendedTrack sourceTrack = this.SourceTracks[sourceIndex];
                if (sourceTrack == track)
                {
                    // Attempt to move!
                    if (sourceIndex >= 1)
                    {
                        Swap(this.SourceTracks, sourceIndex - 1, this.SourceTracks, sourceIndex);
                        return true;
                    } else
                    {
                        // We are at the top of the list, it cant be moved!
                        return false;
                    }

                } else
                {
                    // Try to find in sub-tracks
                    for (int trackIndex = 0; trackIndex < sourceTrack.AppendedTracks.Count; trackIndex++)
                    {
                        AppendedTrack subTrack = sourceTrack.AppendedTracks[trackIndex];
                        if (subTrack == track)
                        {
                            // Attempt to move
                            if (trackIndex >= 1)
                            {
                                Swap(sourceTrack.AppendedTracks, trackIndex - 1, sourceTrack.AppendedTracks, trackIndex);
                                return true;
                            } else
                            {
                                // Try to move to a higher source track
                                if (sourceIndex >= 1)
                                {
                                    // Remove from this list and add it to the higher one
                                    sourceTrack.AppendedTracks.Remove(subTrack);
                                    this.SourceTracks[sourceIndex - 1].AppendedTracks.Add(subTrack);
                                    return true;
                                } else
                                {
                                    // We are at the top of the list, it cant be moved!
                                    return false;
                                }
                            }
                        }
                    }
                }
            }

            // Could not find requested track
            return false;
        }

        public bool MoveTrackDown(AppendedTrack track) {
            for (int sourceIndex = 0; sourceIndex < this.SourceTracks.Count; sourceIndex++)
            {
                AppendedTrack sourceTrack = this.SourceTracks[sourceIndex];
                if (sourceTrack == track)
                {
                    // Attempt to move!
                    if (sourceIndex < this.SourceTracks.Count - 1)// Not the last item in the list
                    {
                        Swap(this.SourceTracks, sourceIndex, this.SourceTracks, sourceIndex + 1);
                        return true;
                    } else
                    {
                        // We are at the bottom of the list, it cant be moved!
                        return false;
                    }
                } else
                {
                    // Try searching in sub-tracks
                    for (int trackIndex = 0; trackIndex < sourceTrack.AppendedTracks.Count; trackIndex++)
                    {
                        AppendedTrack subTrack = sourceTrack.AppendedTracks[trackIndex];
                        if (subTrack == track)
                        {
                            // Attempt to move
                            if (trackIndex < sourceTrack.AppendedTracks.Count - 1) // Not the last item in the list
                            {
                                Swap(sourceTrack.AppendedTracks, trackIndex, sourceTrack.AppendedTracks, trackIndex + 1);
                                return true;
                            } else
                            {
                                // Try to move to a lower source track
                                if (sourceIndex < this.SourceTracks.Count - 1) // Not the last source in the list
                                {
                                    // Remove from this list and add it to the beginning of the next one
                                    sourceTrack.AppendedTracks.Remove(subTrack);
                                    this.SourceTracks[sourceIndex + 1].AppendedTracks.Insert(0, subTrack);
                                    return true;
                                } else
                                {
                                    // We are at the bottom of the list, it cant be moved!
                                    return false;
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }

        public void Save(LoadedDisc disc) {
            RenameData.Name = this.Name;
            RenameData.Title = this.Name;
            RenameData.SourceFiles.Clear();
            RenameData.SourceTracks.Clear();

            // Save all the files first
            long streamUID = 0;
            Dictionary<AppendedFile, long> fileLookup = new();
            foreach(var source in this.SourceFiles)
            {
                var sourcePlaylist = new PlaylistSourceFile();
                sourcePlaylist.SourceUID = source.Source.RenameData.UID;
                sourcePlaylist.PlaylistUID = streamUID++;

                fileLookup[source] = sourcePlaylist.PlaylistUID;
                RenameData.SourceFiles.Add(sourcePlaylist);

                foreach(var appended in source.AppendedFiles)
                {
                    var appendedPlaylist = new PlaylistFile();
                    appendedPlaylist.SourceUID = appended.Source.RenameData.UID;
                    appendedPlaylist.PlaylistUID = streamUID++;

                    fileLookup[appended] = appendedPlaylist.PlaylistUID;
                    sourcePlaylist.AppendedFiles.Add(appendedPlaylist);
                }
            }

            // Now go through all the tracks
            List<TrackID> disabledTracks = new();
            TrackID lastEnabledTrack = new TrackID();
            foreach (var sourceTrack in this.SourceTracks)
            {
                PlaylistSourceTrack track = new PlaylistSourceTrack();
                SaveTrack(track, disc, fileLookup, sourceTrack, disabledTracks, ref lastEnabledTrack);
                RenameData.SourceTracks.Add(track);
            }
        }

        private void SaveTrack(PlaylistTrack playlist, LoadedDisc disc, Dictionary<AppendedFile, long> fileLookup, AppendedTrack track, List<TrackID> disabledTracks, ref TrackID lastEnabledTrack) {
            var source = track.Track;

            playlist.Copy = track.Enabled;

            PlaylistSourceTrack? sourcePlaylist = null;
            if (playlist is PlaylistSourceTrack srcTrack)
            {
                sourcePlaylist = srcTrack;
            }
            bool isAppended = (sourcePlaylist == null);

            // Save delay info
            if (track.Delay != null && isAppended) // TODO can't delays be on source tracks too?
            {
                playlist.Delay = new();
                if (track.Delay.ClipDelay != null)
                {
                    playlist.Delay.DelayType = DelayType.Source;
                    playlist.Delay.StreamUID = fileLookup[track.Delay.ClipDelay];
                } else
                {
                    playlist.Delay.DelayType = DelayType.Delay;
                    playlist.Delay.Milliseconds = track.Delay.MillisecondDelay;
                }
            } else
            {
                TrackID trackId = new(); //FindTrackID(disc, track.Source.Source, track.Track);
                trackId.StreamUID = fileLookup[track.Source];
                trackId.TrackUID = track.Track.RenameData.UID;
                playlist.PlaylistSource = trackId;

                if (sourcePlaylist != null) // i.e. is source track
                {
                    // We want to save sub-tracks for accurate reloading later
                    // TODO disable sub-tracks if not enabled
                    //if (track.Enabled)
                    //{
                    // create internal state for appended tracks
                    disabledTracks = new();
                    TrackID lastTrack = trackId;

                    // Save all appended tracks
                    foreach (var appendedTrack in track.AppendedTracks)
                    {
                        PlaylistTrack newTrack = new();
                        SaveTrack(newTrack, disc, fileLookup, appendedTrack, disabledTracks, ref lastTrack);
                        sourcePlaylist.AppendedTracks.Add(newTrack);
                    }
                    //}
                } else
                {
                    if (track.Enabled)
                    {
                        // Update for the next track
                        disabledTracks.Clear();
                        lastEnabledTrack = trackId;
                    } else
                    {
                        // We want to mark how disabled tracks are appended so the file can be properly reloaded 
                        //playlist.AppendedTo = lastEnabledTrack;

                        // Update internal state for next track
                        disabledTracks.Add(trackId);
                    }
                }
            }
        }

		public static LoadedPlaylist? LoadFromRenames(LoadedDisc disc, Playlist playlist)
        {
            LoadedPlaylist result = new(playlist, playlist.Name);

            Dictionary<long, AppendedFile> FileLookup = new();
            Dictionary<long, long> StreamUidLookup = new();

            // Load files first
            foreach(var source in playlist.SourceFiles)
            {
                StreamUidLookup[source.PlaylistUID] = source.SourceUID;
				var sourceFile = result.CreateFile(disc[source.SourceUID]); // TODO error handling
                result.SourceFiles.Add(sourceFile);
                FileLookup[source.PlaylistUID] = sourceFile;

                foreach (var appended in source.AppendedFiles)
                {
                    StreamUidLookup[appended.PlaylistUID] = appended.SourceUID;
                    var appendedFile = result.CreateFile(disc[appended.SourceUID]); // TODO error handling
                    sourceFile.AppendedFiles.Add(appendedFile);
                    FileLookup[appended.PlaylistUID] = appendedFile;
                }
            }

            // Load Tracks
            foreach(var track in playlist.SourceTracks)
            {
                AppendedTrack sourceTrack = CreateTrack(disc, track, FileLookup, StreamUidLookup);
                result.SourceTracks.Add(sourceTrack);

                foreach(var appended in track.AppendedTracks)
                {
                    AppendedTrack appendedTrack = CreateTrack(disc, appended, FileLookup, StreamUidLookup);
                    sourceTrack.AppendedTracks.Add(appendedTrack);
                }
            }

            return result;
        }
        
        private static AppendedTrack CreateTrack(LoadedDisc disc, PlaylistTrack playlist, Dictionary<long, AppendedFile> fileLookup, Dictionary<long, long> streamUidLookup)
        {
            AppendedTrack track;

            if (playlist.Delay == null)
            {
                AppendedFile source = fileLookup[playlist.PlaylistSource.StreamUID];
                LoadedTrack sourceTrack = disc[streamUidLookup[playlist.PlaylistSource.StreamUID], playlist.PlaylistSource.TrackUID];
                track = new AppendedTrack(source, sourceTrack, source.Color);
                track.Enabled = playlist.Copy;
            } else
            {
                track = new AppendedTrack(null, null, Color.White);
                track.Delay = new();

                if (playlist.Delay.DelayType == DelayType.Source)
                {
                    track.Delay.ClipDelay = fileLookup[playlist.Delay.StreamUID];
                } else if (playlist.Delay.DelayType == DelayType.Delay)
                {
                    track.Delay.MillisecondDelay = playlist.Delay.Milliseconds;
                } else
                {
                    throw new Exception("Unknown enum.");
                }
            }

            return track;
        }

		public override string ToString() {
            return Name;
        }
    }
}
