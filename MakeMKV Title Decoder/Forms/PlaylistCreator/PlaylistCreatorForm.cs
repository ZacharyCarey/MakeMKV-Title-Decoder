using libbluray.bdnav.Mpls;
using MakeMKV_Title_Decoder.Controls;
using MakeMKV_Title_Decoder.Data;
using MakeMKV_Title_Decoder.Data.Renames;
using MakeMKV_Title_Decoder.Forms.FileRenamer;
using MakeMKV_Title_Decoder.Forms.PlaylistCreator;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MakeMKV_Title_Decoder
{
    public partial class PlaylistCreatorForm : Form {

        const int IndentedTrackPadding = 25;
        const string EnableIconKey = "dialog-ok-apply.png";
        const string DisableIconKey = "dialog-cancel.png";
        const string UnsavedIconKey = "document-save-as.png";
        const string WarningIconKey = "dialog-warning.png";

        public Color ErrorColor { get; set; } = Color.LightCoral;

        private class SourceListItem {
            public LoadedStream Clip;
            public string Name;

            public SourceListItem(LoadedStream clip, string name) {
                this.Clip = clip;
                this.Name = name;
            }

            public override string ToString() {
                return Name;
            }
        }

        LoadedDisc Disc;

        public PlaylistCreatorForm(LoadedDisc disc) {
            this.Disc = disc;

            InitializeComponent();

            SourceList.Items.Clear();
            foreach (var source in disc.Streams)
            {
                string? name = source.RenameData.Name;
                if (name != null)
                {
                    this.SourceList.Items.Add(new SourceListItem(source, name));
                }
            }
            SourceList_SelectedIndexChanged(null, null);

            foreach (var playlist in disc.RenameData.Playlists)
            {
                this.PlaylistsListBox.Add(playlist);
                CheckErrors(playlist);
            }
            this.PlaylistsListBox.Invalidate();
            PlaylistsListBox_SelectedIndexChanged(null, null);
        }

        private void PlaylistCreatorForm_Load(object sender, EventArgs e) {

        }

        private void PlaylistCreatorForm_FormClosing(object sender, FormClosingEventArgs e) {
            bool unsavedChanges = false;
            foreach (var item in PlaylistsListBox.AllItems)
            {
                if (item.Icons.Contains(UnsavedIconKey))
                {
                    unsavedChanges = true;
                    break;
                }
            }

            if (unsavedChanges && MessageBox.Show("Exit without saving?", "Unsaved changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning) != DialogResult.Yes)
            {
                e.Cancel = true;
            }
        }

        private void SourceList_SelectedIndexChanged(object sender, EventArgs e) {
            SourceListItem? selectedItem = (SourceListItem?)this.SourceList.SelectedItem;
            this.SourcePropertiesPanel.Enabled = (selectedItem != null);
        }

        private void AddPlaylistSourceFile(Playlist selectedPlaylist, LoadedStream clip) {
            PlaylistFile file = new PlaylistFile();
            file.SourceUID = clip.RenameData.UID;

            selectedPlaylist.SourceFiles.Add(file);
            if (selectedPlaylist.SourceFiles.Count == 1)
            {
                selectedPlaylist.SourceTracks.Clear();
                LoadedStream? firstFile = this.Disc[selectedPlaylist.SourceFiles[0].SourceUID];
                if (firstFile != null && selectedPlaylist.SourceTracks.Count != firstFile.Tracks.Count)
                {
                    // Regenerate settings
                    foreach (var track in firstFile.Tracks)
                    {
                        selectedPlaylist.SourceTracks.Add(new PlaylistTrack());
                    }
                }
            }
        }

        private void SourceApplyButton_Click(object sender, EventArgs e) {
            if (this.PlaylistsListBox.SelectedItem != null)
            {
                if (this.SourceList.SelectedItems.Count > 1)
                {
                    Playlist selectedPlaylist = this.PlaylistsListBox.SelectedItem.Playlist;
                    for (int i = 0; i < this.SourceList.SelectedItems.Count; i++)
                    {
                        SourceListItem? selectedItem = (SourceListItem?)this.SourceList.SelectedItems[i];
                        if (selectedItem != null)
                        {
                            AddPlaylistSourceFile(selectedPlaylist, selectedItem.Clip);
                        }
                    }
                    UpdatePlaylistUI();
                } else
                {
                    Playlist selectedPlaylist = this.PlaylistsListBox.SelectedItem.Playlist;
                    SourceListItem? selectedItem = (SourceListItem?)this.SourceList.SelectedItem;
                    if (selectedItem != null)
                    {
                        AddPlaylistSourceFile(selectedPlaylist, selectedItem.Clip);
                        UpdatePlaylistUI();
                    }
                }
            }
        }

        private void UpdatePlaylistUI() {
            bool errors = false;
            Playlist? selectedPlaylist = this.PlaylistsListBox.SelectedItem?.Playlist;

            this.PlaylistFilesList.Clear();
            if (selectedPlaylist != null)
            {
                foreach (var sourceFile in selectedPlaylist.SourceFiles)
                {
                    CreateClipEntry(sourceFile);
                }
            }

            this.PlaylistTrackOrder.Clear();
            if (selectedPlaylist != null)
            {
                LoadedStream? stream = null;
                if (selectedPlaylist != null && selectedPlaylist.SourceFiles.Count > 0) stream = this.Disc[selectedPlaylist.SourceFiles[0].SourceUID];
                if (stream != null)
                {
                    foreach ((int index, (var sourceTrack, var loadedTrack)) in selectedPlaylist.SourceTracks.Zip(stream.Tracks).WithIndex())
                    {
                        // Check for errors
                        bool trackErrors = false;
                        foreach (var otherStream in selectedPlaylist.SourceFiles.Select(x => this.Disc[x.SourceUID]))
                        {
                            if (index >= otherStream.Tracks.Count) continue; //???
                            trackErrors = !TracksCompatible(loadedTrack.Identity, otherStream.Tracks[index].Identity);
                            if (trackErrors) break;
                        }
                        errors |= trackErrors;

                        // Add to GUI
                        TrackListData item = this.PlaylistTrackOrder.Add(loadedTrack, null, null, null, trackErrors ? this.ErrorColor : null, null, sourceTrack.Copy, sourceTrack.PictureInPicture);
                        item.Tag = sourceTrack;
                    }
                }
            }

            if (selectedPlaylist != null)
            {
                if (errors) ErrorIcon(selectedPlaylist);
                else ClearErrorIcon(selectedPlaylist);
            }
        }

        private void CheckErrors(Playlist playlist) {
            bool errors = false;

            IEnumerable<LoadedTrack> sourceTracks;
            if (playlist.SourceFiles.Count > 0) sourceTracks = this.Disc[playlist.SourceFiles[0].SourceUID].Tracks;
            else sourceTracks = Enumerable.Empty<LoadedTrack>();

            foreach (var file in playlist.SourceFiles)
            {
                LoadedStream stream = this.Disc[file.SourceUID];
                if (stream.Tracks.Count != sourceTracks.Count()) errors = true;
                if (errors) break;
                foreach ((var track, LoadedTrack sourceTrack) in stream.Tracks.Zip(sourceTracks))
                {
                    bool compatible = TracksCompatible(track.Identity, sourceTrack.Identity);
                    errors |= !compatible;
                    if (errors) break;
                }
            }

            if (errors) ErrorIcon(playlist);
            else ClearErrorIcon(playlist);
        }

        private static bool TracksCompatible(TrackIdentity a, TrackIdentity b) {
            if (a.TrackType != b.TrackType) return false;
            if (a.TrackType == TrackType.Video)
            {
                return true;
            } else if (a.TrackType == TrackType.Audio)
            {
                return true;
            } else if (a.TrackType == TrackType.Subtitle)
            {
                return true;
            } else
            {
                throw new ArgumentException("Unknown enum type");
            }

            return true;
        }

        private void CreateClipEntry(PlaylistFile clip) {
            LoadedStream stream = this.Disc[clip.SourceUID];

            // Clip name
            PropertyData item = new();
            item.Text = stream.RenameData.Name ?? "";
            item.IconColor = null;
            item.IsSubItem = false;
            item.Tag = clip;

            // Container
            PropertyData sub1 = new();
            sub1.Text = stream.Identity.ContainerType?.ToString() ?? "";
            item.SubItems.Add(sub1);

            // File size
            PropertyData sub2 = new();
            sub2.Text = stream.Identity.FileSize.ToString();
            item.SubItems.Add(sub2);

            // Directory
            PropertyData sub3 = new();
            sub3.Text = Path.Combine(this.Disc.Root, stream.Identity.SourceFile);
            item.SubItems.Add(sub3);

            this.PlaylistFilesList.Add(item, sub1, sub2, sub3);
        }

        private void PlaylistsListBox_SelectedIndexChanged(object sender, EventArgs e) {
            PlaylistPropertiesPanel.Enabled = (this.PlaylistsListBox.SelectedItem != null);
            this.PlaylistNameTextBox.Text = this.PlaylistsListBox.SelectedItem?.Playlist?.Name ?? "";
            UpdatePlaylistUI();
        }

        private void ImportPlaylists_Click(object sender, EventArgs e) {
            foreach (var discPlaylist in this.Disc.GetPlaylists())
            {
                Playlist? playlist = Playlist.Import(this.Disc, discPlaylist);
                if (playlist == null)
                {
                    // TODO error
                    continue;
                }

                // If no valid source files (including ignored files) were found,
                // dont even bother to import the playlist
                if (playlist.SourceFiles.Count != 0)
                {
                    this.Disc.RenameData.Playlists.Add(playlist);
                    this.PlaylistsListBox.Add(playlist);
                    string? newName = this.Disc[playlist.SourceFiles[0].SourceUID].RenameData.Name;
                    if (newName != null) playlist.Name = newName;
                    playlist.Title = playlist.Name;

                    CheckErrors(playlist);
                }
            }

            this.PlaylistsListBox.Invalidate();
        }

        private void NewPlaylistButton_Click(object sender, EventArgs e) {
            var playlist = new Playlist();
            this.Disc.RenameData.Playlists.Add(playlist);
            this.PlaylistsListBox.Add(playlist);
        }

        private void PlaylistSourceDeleteBtn_Click(object sender, EventArgs e) {
            if (PlaylistFilesList.SelectedItems.Count > 0)
            {
                PlaylistFile? selectedItem = (PlaylistFile?)((PropertyData?)PlaylistFilesList.SelectedItems[0].Tag)?.Tag;
                Playlist? selectedPlaylist = this.PlaylistsListBox.SelectedItem?.Playlist;
                if (selectedItem != null && selectedPlaylist != null)
                {
                    if (selectedPlaylist.SourceFiles.IndexOf(selectedItem) == 0)
                    {
                        selectedPlaylist.SourceTracks.Clear();
                    }

                    selectedPlaylist.SourceFiles.Remove(selectedItem);

                    // Check if tracks need to be refreshed
                    LoadedStream? firstFile = null;
                    if (selectedPlaylist.SourceFiles.Count > 0) firstFile = this.Disc[selectedPlaylist.SourceFiles[0].SourceUID];
                    if (firstFile != null && selectedPlaylist.SourceTracks.Count != firstFile.Tracks.Count)
                    {
                        // Regenerate settings
                        foreach (var track in firstFile.Tracks)
                        {
                            selectedPlaylist.SourceTracks.Add(new PlaylistTrack());
                        }
                    }

                    UpdatePlaylistUI();
                }
            }
        }

        private void EnableTrackBtn_Click(object sender, EventArgs e) {
            Playlist? selectedPlaylist = this.PlaylistsListBox.SelectedItem?.Playlist;
            PlaylistTrack? selectedTrack = (PlaylistTrack?)PlaylistTrackOrder.SelectedItem?.Tag;
            LoadedTrack? selected = PlaylistTrackOrder.SelectedItem?.Track;
            if (selectedPlaylist != null && selectedTrack != null)
            {
                selectedTrack.Copy = true;
                UpdatePlaylistUI();
                this.PlaylistTrackOrder.Select(selected);
            }
        }

        private void DisableTrackBtn_Click(object sender, EventArgs e) {
            Playlist? selectedPlaylist = this.PlaylistsListBox.SelectedItem?.Playlist;
            PlaylistTrack? selectedTrack = (PlaylistTrack?)PlaylistTrackOrder.SelectedItem?.Tag;
            LoadedTrack? selected = PlaylistTrackOrder.SelectedItem?.Track;
            if (selectedPlaylist != null && selectedTrack != null)
            {
                selectedTrack.Copy = false;
                UpdatePlaylistUI();
                this.PlaylistTrackOrder.Select(selected);
            }
        }

        private void FileUpBtn_Click(object sender, EventArgs e) {
            Playlist? selectedPlaylist = this.PlaylistsListBox.SelectedItem?.Playlist;
            PlaylistFile? selectedFile = (PlaylistFile?)((PropertyData?)PlaylistFilesList.SelectedItems[0].Tag)?.Tag;
            if (selectedPlaylist != null && selectedFile != null)
            {
                int index = selectedPlaylist.SourceFiles.IndexOf(selectedFile);
                if (index > 0)
                {
                    // Move file up
                    PlaylistFile temp = selectedPlaylist.SourceFiles[index - 1];
                    selectedPlaylist.SourceFiles[index - 1] = selectedFile;
                    selectedPlaylist.SourceFiles[index] = temp;

                    if (index == 1)
                    {
                        // We moved into the source file position, check stream count
                        LoadedStream stream = this.Disc[selectedFile.SourceUID];
                        if (selectedPlaylist.SourceTracks.Count != stream.Tracks.Count)
                        {
                            selectedPlaylist.SourceTracks.Clear();
                            foreach (var track in stream.Tracks)
                            {
                                selectedPlaylist.SourceTracks.Add(new PlaylistTrack());
                            }
                        }
                    }

                    // Update UI
                    UpdatePlaylistUI();
                    this.PlaylistFilesList.SelectedIndex = index - 1;
                }
            }
        }

        private void DownFileBtn_Click(object sender, EventArgs e) {
            Playlist? selectedPlaylist = this.PlaylistsListBox.SelectedItem?.Playlist;
            PlaylistFile? selectedFile = (PlaylistFile?)((PropertyData?)PlaylistFilesList.SelectedItems[0].Tag)?.Tag;
            if (selectedPlaylist != null && selectedFile != null)
            {
                int index = selectedPlaylist.SourceFiles.IndexOf(selectedFile);
                if (index < (selectedPlaylist.SourceFiles.Count - 1))
                {
                    // Move file down
                    PlaylistFile temp = selectedPlaylist.SourceFiles[index + 1];
                    selectedPlaylist.SourceFiles[index + 1] = selectedFile;
                    selectedPlaylist.SourceFiles[index] = temp;

                    if (index + 1 == (selectedPlaylist.SourceFiles.Count - 1))
                    {
                        // We moved into the source file position, check stream count
                        LoadedStream stream = this.Disc[selectedFile.SourceUID];
                        if (selectedPlaylist.SourceTracks.Count != stream.Tracks.Count)
                        {
                            selectedPlaylist.SourceTracks.Clear();
                            foreach (var track in stream.Tracks)
                            {
                                selectedPlaylist.SourceTracks.Add(new PlaylistTrack());
                            }
                        }
                    }

                    // Update UI
                    UpdatePlaylistUI();
                    this.PlaylistFilesList.SelectedIndex = index + 1;
                }
            }
        }

        private void ErrorIcon(Playlist playlist) {
            LoadedPlaylistListItem? item = PlaylistsListBox.FindItem(playlist);
            if (item != null)
            {
                if (!item.Icons.Contains(WarningIconKey))
                {
                    if (item.Icons.Count > 0) item.Icons.Insert(0, WarningIconKey);
                    else item.Icons.Add(WarningIconKey);

                    this.PlaylistsListBox.Invalidate();
                }
            }
        }

        private void ClearErrorIcon(Playlist playlist) {
            LoadedPlaylistListItem? item = PlaylistsListBox.FindItem(playlist);
            if (item != null)
            {
                item.Icons.Remove(WarningIconKey);
                this.PlaylistsListBox.Invalidate();
            }
        }

        private bool HasErrors(Playlist playlist) {
            LoadedPlaylistListItem? item = PlaylistsListBox.FindItem(playlist);
            return item?.Icons?.Contains(WarningIconKey) ?? false;
        }

        private void ApplyPlaylistButton_Click(object sender, EventArgs e) {
            if (PlaylistsListBox.SelectedItems.Count > 0)
            {
                if (MessageBox.Show("Can not be undone. Are you sure?", "Rename?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning) != DialogResult.Yes)
                {
                    return;
                }
            }

            if (PlaylistsListBox.SelectedItems.Count > 1)
            {
                List<LoadedPlaylistListItem> itemsToRename = new();
                foreach (var obj in PlaylistsListBox.SelectedItems)
                {
                    if (obj != null && obj is LoadedPlaylistListItem item)
                    {
                        itemsToRename.Add(item);
                    }
                }

                string baseName = this.PlaylistNameTextBox.Text.TrimEnd();
                for (int i = 0; i < itemsToRename.Count; i++) {
                    var item = itemsToRename[i];
                    item.Playlist.Name = $"{baseName} {(i + 1)}";
                }
                this.PlaylistsListBox.Invalidate();
            } else
            {
                LoadedPlaylistListItem? selectedItem = PlaylistsListBox.SelectedItem;
                if (selectedItem != null)
                {
                    selectedItem.Playlist.Name = this.PlaylistNameTextBox.Text;
                    this.PlaylistsListBox.Invalidate();
                }
            }
        }

        private void DeletePlaylistButton_Click(object sender, EventArgs e) {
            if (PlaylistsListBox.SelectedItems.Count > 0)
            {
                if (MessageBox.Show("Can not be undone. Are you sure?", "Delete?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning) != DialogResult.Yes)
                {
                    return;
                }
            }

            if (PlaylistsListBox.SelectedItems.Count > 1)
            {
                List<LoadedPlaylistListItem> itemsToDelete = new();
                foreach (var obj in PlaylistsListBox.SelectedItems)
                {
                    if (obj != null && obj is LoadedPlaylistListItem item)
                    {
                        itemsToDelete.Add(item);
                    }
                }
                foreach(var item in itemsToDelete)
                {
                    this.PlaylistsListBox.Remove(item);
                    this.Disc.RenameData.Playlists.Remove(item.Playlist);
                }
            } else
            {
                LoadedPlaylistListItem? selectedItem = PlaylistsListBox.SelectedItem;
                if (selectedItem != null)
                {
                    this.PlaylistsListBox.Remove(selectedItem);
                    this.Disc.RenameData.Playlists.Remove(selectedItem.Playlist);
                }
            }
        }

        private void PiPBtn_Click(object sender, EventArgs e) {
            Playlist? selectedPlaylist = this.PlaylistsListBox.SelectedItem?.Playlist;
            PlaylistTrack? selectedTrack = (PlaylistTrack?)PlaylistTrackOrder.SelectedItem?.Tag;
            LoadedTrack? selected = PlaylistTrackOrder.SelectedItem?.Track;
            if (selectedPlaylist != null && selectedTrack != null)
            {
                selectedTrack.PictureInPicture = !selectedTrack.PictureInPicture;
                UpdatePlaylistUI();
                this.PlaylistTrackOrder.Select(selected);
            }
        }
    }
}
