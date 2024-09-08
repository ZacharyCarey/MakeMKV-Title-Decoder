using MakeMKV_Title_Decoder.Data;
using MakeMKV_Title_Decoder.MkvToolNix.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.Forms.PlaylistCreator {
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
        //private Dictionary<MkvMergeID, AppendedFile> _FileLookup = new();

        /*private AppendedFile? GetFile(MkvMergeID clip) {
            AppendedFile result;
            if (!_FileLookup.TryGetValue(clip, out result))
            {
                return null;
            } else
            {
                return result;
            }
        }*/

        private AppendedFile CreateFile(MkvMergeID clip) {
            var result = new AppendedFile(clip, GetNextColor());
            _AllFiles.Add(result);
            //_FileLookup[clip] = result;
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
            //if (_FileLookup.TryGetValue(clip, out file))
            //{
                _AllFiles.Remove(file);
                //_FileLookup.Remove(clip);
                this.ReturnColor(file.Color);
                return true;
            //}

            //return false;
        }

        private void ResetFiles() {
            _AllFiles.Clear();
            //_FileLookup.Clear();
        }
        #endregion

        public PlaylistOld RenameData;
        public AppendedFile? PrimarySource = null;
        public IEnumerable<AppendedFile> AppendedFiles => _AllFiles.Where(x => x != this.PrimarySource);
        public List<AppendedTrack> SourceTracks = new();
        public string Name;

        public LoadedPlaylist(PlaylistOld renameData, MkvMergeID? file, string name) {
            this.RenameData = renameData;
            if (file != null) this.PrimarySource = CreateFile(file);//GetFileOrCreate(file);
            this.Name = name;
        }

        private AppendedFile AddSourceFile(AppendedFile file) {
            if (this.PrimarySource == null)
            {
                this.PrimarySource = file;
                // Add primary tracks
                foreach (var track in file.Source.Tracks)
                {
                    AppendedTrack sourceTrack = new AppendedTrack(file, track, file.Color);
                    this.SourceTracks.Add(sourceTrack);
                }
            } else
            {
                // Best effort to match tracks
                // TODO use better algorithm i.e. try to match up track types
                for (int i = 0; i < file.Source.Tracks.Count; i++)
                {
                    AppendedTrack appendTrack = new AppendedTrack(file, file.Source.Tracks[i], file.Color);
                    if (i < this.SourceTracks.Count)
                    {
                        this.SourceTracks[i].AppendedTracks.Add(appendTrack);
                    } else
                    {
                        // Dump all extra tracks into the last track.
                        this.SourceTracks.Last().AppendedTracks.Add(appendTrack);
                    }
                }
            }
            return file;
        }

        public AppendedFile AddSourceFile(MkvMergeID file) {
            var appendedFile = CreateFile(file);
            return AddSourceFile(appendedFile);
        }

        public AppendedFile ImportSourceFile(MkvMergeID file) {
            var appendedFile = GetFileOrCreate(file);
            return AddSourceFile(appendedFile);
        }

        public void DeleteSourceFileAndTracks(AppendedFile file) {
            if (file == this.PrimarySource)
            {
                // Delete everything
                ResetColors();
                ResetFiles();
                SourceTracks.Clear();
            } else
            {
                // Delete relevent tracks
                foreach(var sourceTrack in this.SourceTracks)
                {
                    sourceTrack.AppendedTracks.RemoveAll(x => x.Source == file);
                }

                // Delete from files
                this.RemoveFile(file);
            }
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

        public int MoveTrackUp(AppendedTrack track) {
            for(int sourceIndex = 0; sourceIndex < this.SourceTracks.Count; sourceIndex++)
            {
                AppendedTrack sourceTrack = this.SourceTracks[sourceIndex];
                for (int trackIndex = 0; trackIndex < sourceTrack.AppendedTracks.Count; trackIndex++)
                {
                    AppendedTrack subTrack = sourceTrack.AppendedTracks[trackIndex];
                    if (subTrack == track)
                    {
                        // Attempt to move
                        if (trackIndex >= 1)
                        {
                            Swap(sourceTrack.AppendedTracks, trackIndex - 1, sourceTrack.AppendedTracks, trackIndex);
                            return -1;
                        } else
                        {
                            // Try to move to a higher source track
                            if (sourceIndex >= 1)
                            {
                                // Remove from this list and add it to the higher one
                                sourceTrack.AppendedTracks.Remove(subTrack);
                                this.SourceTracks[sourceIndex - 1].AppendedTracks.Add(subTrack);
                                return -1;
                            } else
                            {
                                // We are at the top of the list, it cant be moved!
                                return 0;
                            }
                        }
                    }
                }
            }

            return 0;
        }

        public int MoveTrackDown(AppendedTrack track) {
            for (int sourceIndex = 0; sourceIndex < this.SourceTracks.Count; sourceIndex++)
            {
                AppendedTrack sourceTrack = this.SourceTracks[sourceIndex];
                for (int trackIndex = 0; trackIndex < sourceTrack.AppendedTracks.Count; trackIndex++)
                {
                    AppendedTrack subTrack = sourceTrack.AppendedTracks[trackIndex];
                    if (subTrack == track)
                    {
                        // Attempt to move
                        if (trackIndex < sourceTrack.AppendedTracks.Count - 1) // Not the last item in the list
                        {
                            Swap(sourceTrack.AppendedTracks, trackIndex, sourceTrack.AppendedTracks, trackIndex + 1);
                            return 1;
                        } else
                        {
                            // Try to move to a lower source track
                            if (sourceIndex < this.SourceTracks.Count - 1) // Not the last source in the list
                            {
                                // Remove from this list and add it to the beginning of the next one
                                sourceTrack.AppendedTracks.Remove(subTrack);
                                this.SourceTracks[sourceIndex + 1].AppendedTracks.Insert(0, subTrack);
                                return 1;
                            } else
                            {
                                // We are at the bottom of the list, it cant be moved!
                                return 0;
                            }
                        }
                    }
                }
            }

            return 0;
        }

        public void Save() {
            RenameData.Name = this.Name;
            RenameData.PrimarySource = this.PrimarySource?.Source?.GetRelativePath();
            RenameData.Tracks.Clear();
            foreach(AppendedTrack sourceTrack in this.SourceTracks)
            {
                PlaylistTrackOld source = new PlaylistTrackOld();
                RenameData.Tracks.Add(source);

                source.Source = sourceTrack.Source.Source.GetRelativePath();
                source.UID = sourceTrack.Track?.Properties?.Number ?? -1;
                source.Codec = sourceTrack.Track?.Codec ?? "";
                source.ID = sourceTrack.Track.ID;
                source.Enabled = sourceTrack.Enabled;

                foreach(AppendedTrack subTrack in sourceTrack.AppendedTracks)
                {
                    PlaylistTrackOld track = new PlaylistTrackOld();
                    source.AppendedTracks.Add(track);

                    track.Source = subTrack.Source.Source.GetRelativePath();
                    track.UID = subTrack.Track?.Properties?.Number ?? -1;
                    track.Codec = subTrack.Track?.Codec ?? "";
                    track.ID = subTrack.Track.ID;
                    track.Enabled = subTrack.Enabled;
                }
            }
        }

        public static LoadedPlaylist? LoadFromRenames(MkvToolNixDisc disc, PlaylistOld playlist) {
            LoadedPlaylist? result = null;
            // Try to find primary source
            if (playlist.PrimarySource == null) return null;
            foreach (var clip in disc.Streams)
            {
                if (clip.GetRelativePath() == playlist.PrimarySource)
                {
                    result = new LoadedPlaylist(playlist, clip, playlist.Name ?? "?");
                    break;
                }
            }

            if (result != null)
            {
                foreach (var primaryTrack in playlist.Tracks)
                {
                    var track = LoadTrack(disc, result, primaryTrack, true);
                    if (track != null && track.Source.Source.GetRelativePath() == result.PrimarySource.Source.GetRelativePath())
                    {
                        result.SourceTracks.Add(track);
                    } else
                    {
                        // TODO error reporting
                    }
                }
            }
            return result;
        }

        private static AppendedTrack? LoadTrack(MkvToolNixDisc disc, LoadedPlaylist playlist, PlaylistTrackOld playlistTrack, bool isPrimary) {
            MkvMergeID? sourceFile = null;
            MkvTrack? sourceTrack = null;
            Color color;

            // Find source file
            if (playlistTrack.Source == null) return null;
            foreach (var clip in disc.Streams)
            {
                if (clip != null && clip.GetRelativePath() == playlistTrack.Source)
                {
                    sourceFile = clip;
                    break;
                }
            }
            if (sourceFile == null) return null;

            // Find source track, first by matching UID
            if (playlistTrack.UID == null) return null;
            List<MkvTrack> matchingTracks = sourceFile.Tracks.Where(x => x.Properties?.Number == playlistTrack.UID).ToList();
            if (matchingTracks.Count > 1)
            {
                // Multiple matches, try to match codec name next
                if (playlistTrack.Codec == null) return null;
                matchingTracks = matchingTracks.Where(x => x.Codec == playlistTrack.Codec).ToList();
                if (matchingTracks.Count > 1)
                {
                    // Multiple matches, try to match using MkvToolNix track ID
                    if (playlistTrack.ID == null) return null;
                    matchingTracks = matchingTracks.Where(x => x.ID == playlistTrack.ID).ToList();
                }
            }
            if (matchingTracks.Count >= 1)
            {
                // We tried our best to match, just take the first result if there are more than 1
                sourceTrack = matchingTracks[0];
            }
            if (sourceTrack == null) return null;

            // Get the color based on the selected source file
            var file = playlist.GetFileOrCreate(sourceFile);
            color = file.Color;

            // Finally create the track
            var result = new AppendedTrack(file, sourceTrack, color);
            result.Enabled = playlistTrack.Enabled ?? true;

            // Now load any appended tracks, if needed
            if (isPrimary)
            {
                foreach (var track in playlistTrack.AppendedTracks)
                {
                    if (track != null)
                    {
                        var loadedTrack = LoadTrack(disc, playlist, track, false);
                        if (loadedTrack != null)
                        {
                            // TODO report loading error
                            result.AppendedTracks.Add(loadedTrack);
                        }
                    }
                }
            }

            return result;
        }

        public override string ToString() {
            return Name;
        }
    }
}
