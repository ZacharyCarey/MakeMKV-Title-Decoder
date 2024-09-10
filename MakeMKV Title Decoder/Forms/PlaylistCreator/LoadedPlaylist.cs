using MakeMKV_Title_Decoder.Data;
using MakeMKV_Title_Decoder.libs.MkvToolNix.Data;
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

        private AppendedFile CreateFile(MkvMergeID clip) {
            var result = new AppendedFile(clip, GetNextColor());
            _AllFiles.Add(result);
            return result;
        }

        private AppendedFile GetFileOrCreate(MkvMergeID clip) {
            var path = clip.GetRelativePath();
            foreach(var file in _AllFiles)
            {
                if (file.Source.GetRelativePath() == path)
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
        //public IEnumerable<AppendedFile> AppendedFiles => _AllFiles.Where(x => x != this.PrimarySource);
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

        public AppendedFile AddSourceFile(MkvMergeID file) {
            var appendedFile = CreateFile(file);
            AddSourceFile(appendedFile);
            return appendedFile;
        }

        public AppendedFile AddAppendedFile(AppendedFile parent, MkvMergeID file) {
            var appendedFile = CreateFile(file);
            this.AddAppendedFile(parent, appendedFile);
            return appendedFile;
        }

        public AppendedFile ImportSourceFile(MkvMergeID file) {
            var appendedFile = GetFileOrCreate(file);
            AddSourceFile(appendedFile);
            return appendedFile;
        }

        public AppendedFile ImportAppendedFile(AppendedFile parent, MkvMergeID file) {
            var appendedFile = GetFileOrCreate(file);
            AddAppendedFile(parent, appendedFile);
            return appendedFile;
        }

        public void DeleteFileAndTracks(AppendedFile file) {
            // Cleanup any appended files (if needed)
            foreach(var appended in file.AppendedFiles)
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

        public void Save(RenameData renames) {
            RenameData.Name = this.Name;
            RenameData.Files.Clear();
            RenameData.TrackOrder.Clear();

            // Save all the files first
            Dictionary<AppendedFile, PlaylistFile> lookup = new();
            Dictionary<AppendedFile, int> fileIndexLookup = new();
            int lastIndex = 0;
            foreach(var source in this.SourceFiles)
            {
                var sourcePlaylist = new PlaylistFile();
                lookup[source] = sourcePlaylist;
                RenameData.Files.Add(sourcePlaylist);
                fileIndexLookup[source] = lastIndex++;

                sourcePlaylist.Source = source.Source.GetRelativePath();

                foreach(var appended in source.AppendedFiles)
                {
                    var appendedPlaylist = new PlaylistFile();
                    lookup[appended] = appendedPlaylist;
                    sourcePlaylist.AppendedFiles.Add(appendedPlaylist);
                    fileIndexLookup[appended] = lastIndex++;

                    appendedPlaylist.Source = appended.Source.GetRelativePath();
                }
            }

            // Now go through all the tracks
            List<TrackID> disabledTracks = new();
            TrackID lastEnabledTrack = new TrackID();
            foreach (var sourceTrack in this.SourceTracks)
            {
                SaveTrack(lookup, fileIndexLookup, renames, sourceTrack, false, disabledTracks, ref lastEnabledTrack);
            }
        }

        private void SaveTrack(Dictionary<AppendedFile, PlaylistFile> lookup, Dictionary<AppendedFile, int> fileIndexLookup, RenameData renames, AppendedTrack track, bool isAppended, List<TrackID> disabledTracks, ref TrackID lastEnabledTrack) {
            var source = track.Track;

            TrackID trackId = new();
            trackId.FileIndex = fileIndexLookup[track.Source];
            trackId.TrackIndex = (int)track.Track.ID;
            if (!isAppended)
            {
                RenameData.TrackOrder.Add(trackId);
            }

            // This will hold all of the track data
            var trackRename = renames.GetClipRename(track.Source.Source)?.GetTrackRename(source);
            var playlist = new PlaylistTrack() {
                UID = source.Properties?.Number,
                Codec = source.Codec,
                ID = source.ID,
                Copy = track.Enabled,
                Name = trackRename?.Name,
                Commentary = trackRename?.CommentaryFlag
            };

            // Add to the source file that owns it
            lookup[track.Source].Tracks.Add(playlist);

            if (!isAppended)
            {
                // We want to save sub-tracks for accurate reloading later
                //if (track.Enabled)
                //{
                    // create internal state for appended tracks
                    disabledTracks = new();
                    TrackID lastTrack = trackId;

                    // Save all appended tracks
                    foreach (var appendedTrack in track.AppendedTracks)
                    {
                        SaveTrack(lookup, fileIndexLookup, renames, appendedTrack, true, disabledTracks, ref lastTrack);
                    }
                //}
            } else
            {
                if (track.Enabled)
                {
                    // Mark which track it is appended to
                    playlist.AppendedTo = lastEnabledTrack;

                    // Update sync info as needed
                    playlist.Sync.Clear();
                    playlist.Sync.AddRange(disabledTracks);

                    // Update for the next track
                    disabledTracks.Clear();
                    lastEnabledTrack = trackId;
                } else
                {
                    // We want to mark how disabled tracks are appended so the file can be properly reloaded 
                    playlist.AppendedTo = lastEnabledTrack;

                    // Update internal state for next track
                    disabledTracks.Add(trackId);
                }
            }
        }

        private static MkvMergeID? FindFile(MkvToolNixDisc disc, string relativePath) {
            foreach(var stream in disc.Streams)
            {
                if (stream.GetRelativePath() == relativePath)
                {
                    return stream;
                }
            }
            return null;
        }

        private static MkvTrack? FindTrack(MkvMergeID stream, PlaylistTrack filter) {
            // TODO better matching algorithm
            List<MkvTrack> matchingTracks = stream.Tracks.Where(x => x.Properties?.Number == filter.UID).ToList();
            if (matchingTracks.Count > 1)
            {
                // Multiple matches, try to match codec name next
                if (filter.Codec == null) return null;
                matchingTracks = matchingTracks.Where(x => x.Codec == filter.Codec).ToList();
                if (matchingTracks.Count > 1)
                {
                    // Multiple matches, try to match using MkvToolNix track ID
                    if (filter.ID == null) return null;
                    matchingTracks = matchingTracks.Where(x => x.ID == filter.ID).ToList();
                }
            }
            if (matchingTracks.Count >= 1)
            {
                // We tried our best to match, just take the first result if there are more than 1
                return matchingTracks[0];
            }
            return null;
        }

        public static LoadedPlaylist? LoadFromRenames(MkvToolNixDisc disc, Playlist playlist) {
            LoadedPlaylist result = new(playlist, playlist.Name);

            List<PlaylistFile> allFiles = new();
            Dictionary<PlaylistFile, AppendedFile> lookup = new();
            Dictionary<AppendedFile, List<AppendedTrack>> trackLookup = new();
            Dictionary<PlaylistTrack, AppendedTrack> breadcrumb = new();
            foreach(var sourceFile in playlist.Files)
            {
                var file = result.CreateFile(FindFile(disc, sourceFile.Source)); // TODO error handling
                lookup[sourceFile] = file;
                trackLookup[file] = new();
                result.SourceFiles.Add(file);
                allFiles.Add(sourceFile);
                foreach(var appendedFile in sourceFile.AppendedFiles)
                {
                    var appended = result.CreateFile(FindFile(disc, appendedFile.Source)); // TODO error handling
                    lookup[appendedFile] = appended;
                    trackLookup[appended] = new();
                    file.AppendedFiles.Add(appended);
                    allFiles.Add(appendedFile);
                }
            }

            // Now attempt to add the tracks
            foreach(var playlistSourceFile in playlist.Files)
            {
                foreach(var playlistSourceTrack in playlistSourceFile.Tracks)
                {
                    LoadTrack(result, disc, lookup, trackLookup, breadcrumb, allFiles, playlistSourceFile, playlistSourceTrack, true);
                }

                foreach(var playlistAppendedFile in playlistSourceFile.AppendedFiles)
                {
                    foreach (var playlistAppendedTrack in playlistAppendedFile.Tracks)
                    {
                        LoadTrack(result, disc, lookup, trackLookup, breadcrumb, allFiles, playlistAppendedFile, playlistAppendedTrack, false);
                    }
                }
            }

            if (playlist.TrackOrder.Any())
            {
                List<AppendedTrack> newSourceTracks = new();
                foreach(var order in playlist.TrackOrder)
                {
                    PlaylistFile playlistFile = allFiles[(int)order.FileIndex];
                    AppendedTrack track = trackLookup[lookup[playlistFile]][(int)order.TrackIndex];
                    if (!result.SourceTracks.Contains(track)) throw new Exception();
                }
            }

            // TODO check for other tracks from source that were not listed to be imported in playlist

            return result;
        }

        private static void LoadTrack(LoadedPlaylist result, MkvToolNixDisc disc, Dictionary<PlaylistFile, AppendedFile> lookup, Dictionary<AppendedFile, List<AppendedTrack>> trackLookup, Dictionary<PlaylistTrack, AppendedTrack>  breadcrumb, List<PlaylistFile> allFiles, PlaylistFile playlistFile, PlaylistTrack playlistTrack, bool isSource) {
            var sourceFile = lookup[playlistFile];
            var track = new AppendedTrack(sourceFile, FindTrack(sourceFile.Source, playlistTrack), sourceFile.Color); // TODO error handling
            track.Enabled = playlistTrack.Copy ?? true;
            trackLookup[sourceFile].Add(track);
            
            if (isSource)
            {
                if (playlistTrack.AppendedTo != null) throw new Exception(); // TODO error handling
                result.SourceTracks.Add(track);
                breadcrumb[playlistTrack] = track;
            } else
            {
                TrackID appendedID = playlistTrack.AppendedTo;
                PlaylistFile appendedFile = allFiles[(int)appendedID.FileIndex];
                AppendedTrack appendedTrack = breadcrumb[appendedFile.Tracks[(int)appendedID.TrackIndex]]; //trackLookup[lookup[appendedFile]][(int)appendedID.TrackIndex];

                appendedTrack.AppendedTracks.Add(track);
                if (playlistTrack.Copy ?? true) breadcrumb[playlistTrack] = track;

                if (!appendedTrack.Enabled) track.Enabled = false;
            }
        }

        public override string ToString() {
            return Name;
        }
    }
}
