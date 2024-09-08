using libbluray.bdnav.Mpls;
using MakeMKV_Title_Decoder.Data;
using MakeMKV_Title_Decoder.Forms.PlaylistCreator;
using MakeMKV_Title_Decoder.MkvToolNix.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MakeMKV_Title_Decoder {
    public partial class PlaylistCreatorForm : Form {

        const int IndentedTrackPadding = 25;
        const string EnableIconKey = "dialog-ok-apply.png";
        const string DisableIconKey = "dialog-cancel.png";
        const string UnsavedIconKey = "document-save-as.png";
        const string WarningIconKey = "dialog-warning.png";

        public Color ErrorColor { get; set; } = Color.LightCoral;

        private class SourceListItem {
            public MkvMergeID Clip;
            public string Name;

            public SourceListItem(MkvMergeID clip, string name) {
                this.Clip = clip;
                this.Name = name;
            }

            public override string ToString() {
                return Name;
            }
        }

        MkvToolNixDisc Disc;
        RenameData2 Renames;

        public PlaylistCreatorForm(MkvToolNixDisc disc, RenameData2 renames) {
            this.Disc = disc;
            this.Renames = renames;

            InitializeComponent();

            SourceList.Items.Clear();
            foreach (var source in disc.Streams)
            {
                string? name = renames.GetClipRename(source)?.Name;
                if (name != null)
                {
                    this.SourceList.Items.Add(new SourceListItem(source, name));
                }
            }
            SourceList_SelectedIndexChanged(null, null);

            foreach (var playlist in renames.Playlists)
            {
                var loadedPlaylist = LoadedPlaylist.LoadFromRenames(disc, playlist);
                if (loadedPlaylist != null)
                {
                    this.PlaylistsListBox.Add(loadedPlaylist);
                } else
                {
                    // TODO handle error
                }
            }
            PlaylistsListBox_SelectedIndexChanged(null, null);
        }

        private void PlaylistCreatorForm_Load(object sender, EventArgs e) {

        }

        private void PlaylistCreatorForm_FormClosing(object sender, FormClosingEventArgs e) {
            bool unsavedChanges = false;
            foreach(var item in PlaylistsListBox.AllItems)
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

        private void SourceApplyButton_Click(object sender, EventArgs e) {
            if (this.PlaylistsListBox.SelectedItem != null)
            {
                LoadedPlaylist selectedPlaylist = this.PlaylistsListBox.SelectedItem.Playlist;
                SourceListItem? selectedItem = (SourceListItem?)this.SourceList.SelectedItem;
                if (selectedItem != null)
                {
                    selectedPlaylist.AddSourceFile(selectedItem.Clip);
                    UpdatePlaylistUI();
                    UnsavedChangesIcon(selectedPlaylist);
                }
            }
        }

        private void CheckErrors(LoadedPlaylist playlist) {
            bool errors = false;

            foreach (var sourceTrack in playlist.SourceTracks)
            {
                foreach (var appendedTrack in sourceTrack.AppendedTracks)
                {
                    bool compatible = appendedTrack.IsCompatableWith(sourceTrack);
                    errors |= !compatible;
                }
            }

            if (errors) ErrorIcon(playlist);
            else ClearErrorIcon(playlist);
        }

        private void UpdatePlaylistUI() {
            bool errors = false;
            LoadedPlaylist? selectedPlaylist = this.PlaylistsListBox.SelectedItem?.Playlist;

            this.PlaylistFilesList.Items.Clear();
            if (selectedPlaylist != null)
            {
                if (selectedPlaylist.PrimarySource != null)
                {
                    this.PlaylistFilesList.Items.Add(CreateClipEntry(selectedPlaylist.PrimarySource));
                } else
                {
                    // Ensure state matches UI
                    // selectedPlaylist.RemoveAllAppendedTracks;
                }

                foreach (var file in selectedPlaylist.AppendedFiles)
                {
                    this.PlaylistFilesList.Items.Add(CreateClipEntry(file, IndentedTrackPadding));
                }
            }

            this.PlaylistTrackOrder.Clear();
            if (selectedPlaylist != null)
            {
                foreach (var sourceTrack in selectedPlaylist.SourceTracks)
                {
                    var item = this.PlaylistTrackOrder.Add(
                        sourceTrack.Source.Source,
                        sourceTrack.Track,
                        this.Renames,
                        sourceTrack.Color
                    );
                    item.Tag = sourceTrack;
                    foreach (var appendedTrack in sourceTrack.AppendedTracks)
                    {
                        bool compatible = appendedTrack.IsCompatableWith(sourceTrack);
                        item = this.PlaylistTrackOrder.Add(
                            appendedTrack.Source.Source,
                            appendedTrack.Track,
                            this.Renames,
                            appendedTrack.Color,
                            IndentedTrackPadding,
                            appendedTrack.Enabled ? EnableIconKey : DisableIconKey,
                            compatible ? null : this.ErrorColor
                        );
                        item.Tag = appendedTrack;
                        errors |= !compatible;
                    }
                }
            }

            if (selectedPlaylist != null)
            {
                if (errors) ErrorIcon(selectedPlaylist);
                else ClearErrorIcon(selectedPlaylist);
            }
        }

        private PropertyItem CreateClipEntry(AppendedFile clip, int padding = 0) {
            // Clip name
            PropertyItem item = new();
            item.Text = this.Renames.GetClipRename(clip.Source)?.Name ?? "";
            item.IconColor = clip.Color;
            item.Padding = padding;
            item.Tag = clip;

            // Container
            PropertySubItem sub1 = new(item);
            sub1.Text = clip.Source.Container?.Type ?? "";
            item.SubItems.Add(sub1);

            // File size
            PropertySubItem sub2 = new(item);
            sub2.Text = clip.Source.FileSize.ToString();
            item.SubItems.Add(sub2);

            // Directory
            PropertySubItem sub3 = new(item);
            sub3.Text = clip.Source.GetFullPath(this.Disc);
            item.SubItems.Add(sub3);

            return item;
        }

        private void PlaylistsListBox_SelectedIndexChanged(object sender, EventArgs e) {
            PlaylistPropertiesPanel.Enabled = (this.PlaylistsListBox.SelectedItem != null);
            this.PlaylistNameTextBox.Text = this.PlaylistsListBox.SelectedItem?.Playlist?.Name ?? "";
            UpdatePlaylistUI();
        }

        private void ImportPlaylists_Click(object sender, EventArgs e) {
            foreach (var playlist in this.Disc.Playlists)
            {
                Playlist renamePlaylist = new();
                LoadedPlaylist loadedPlaylist = new(renamePlaylist, null, playlist.FileName);

                foreach (var sourceFile in playlist.Container?.Properties?.PlaylistFiles ?? new List<string>())
                {
                    // Try to find file
                    MkvMergeID? file = null;
                    foreach (var source in this.Disc.Streams)
                    {
                        if (source.GetRelativePath() == sourceFile)
                        {
                            file = source;
                            break;
                        }
                    }
                    if (file == null)
                    {
                        // TODO error
                        continue;
                    }

                    if (Renames.GetClipRename(file) != null)
                    {
                        // Only add files the user has renames i.e. dont add ignored files
                        loadedPlaylist.ImportSourceFile(file);
                    }
                }

                // If no valid source files (including ignored files) were found,
                // dont even bother to import the playlist
                if (loadedPlaylist.PrimarySource != null)
                {
                    this.Renames.Playlists.Add(renamePlaylist);
                    this.PlaylistsListBox.Add(loadedPlaylist);

                    // If only one file was found, just give it the same name as the source
                    if (!loadedPlaylist.AppendedFiles.Any())
                    {
                        var rename = Renames.GetClipRename(loadedPlaylist.PrimarySource.Source);
                        if (rename != null && rename.Name != null) loadedPlaylist.Name = rename.Name;
                    }
                    loadedPlaylist.Save();

                    CheckErrors(loadedPlaylist);
                }
            }

            this.PlaylistsListBox.Invalidate();
        }

        private void NewPlaylistButton_Click(object sender, EventArgs e) {
            var playlist = new Playlist();
            this.Renames.Playlists.Add(playlist);
            this.PlaylistsListBox.Add(new LoadedPlaylist(playlist, null, "Empty Playlist"));
        }

        private void PlaylistSourceDeleteBtn_Click(object sender, EventArgs e) {
            if (PlaylistFilesList.SelectedItems.Count > 0)
            {
                AppendedFile? selectedItem = (AppendedFile?)((PropertyData?)PlaylistFilesList.SelectedItems[0].Tag)?.Tag;
                LoadedPlaylist? selectedPlaylist = this.PlaylistsListBox.SelectedItem?.Playlist;
                if (selectedItem != null && selectedPlaylist != null)
                {
                    if (MessageBox.Show("Deleting this file will delete all related tracks. Are you sure?", "Warning", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        selectedPlaylist.DeleteSourceFileAndTracks(selectedItem);
                        UpdatePlaylistUI();
                        UnsavedChangesIcon(selectedPlaylist);
                    }
                }
            }
        }

        private void EnableTrackBtn_Click(object sender, EventArgs e) {
            LoadedPlaylist? selectedPlaylist = this.PlaylistsListBox.SelectedItem?.Playlist;
            AppendedTrack? selectedTrack = (AppendedTrack?)PlaylistTrackOrder.SelectedItem?.Tag;
            if (selectedPlaylist != null && selectedTrack != null)
            {
                selectedTrack.Enabled = true;
                UpdatePlaylistUI();
                this.PlaylistTrackOrder.Select(selectedTrack.Track);
                UnsavedChangesIcon(selectedPlaylist);
            }
        }

        private void DisableTrackBtn_Click(object sender, EventArgs e) {
            LoadedPlaylist? selectedPlaylist = this.PlaylistsListBox.SelectedItem?.Playlist;
            AppendedTrack? selectedTrack = (AppendedTrack?)PlaylistTrackOrder.SelectedItem?.Tag;
            if (selectedPlaylist != null && selectedTrack != null)
            {

                selectedTrack.Enabled = false;
                UpdatePlaylistUI();
                this.PlaylistTrackOrder.Select(selectedTrack.Track);
                UnsavedChangesIcon(selectedPlaylist);
            }
        }

        private void TrackUpBtn_Click(object sender, EventArgs e) {
            LoadedPlaylist? selectedPlaylist = this.PlaylistsListBox.SelectedItem?.Playlist;
            var selectedIndex = PlaylistTrackOrder.SelectedIndex;
            AppendedTrack? selectedTrack = (AppendedTrack?)PlaylistTrackOrder.SelectedItem?.Tag;
            if (selectedPlaylist != null && selectedTrack != null)
            {
                var moved = selectedPlaylist.MoveTrackUp(selectedTrack);
                if (moved != 0)
                {
                    UpdatePlaylistUI();
                    this.PlaylistTrackOrder.SelectedIndex = selectedIndex + moved;
                    UnsavedChangesIcon(selectedPlaylist);
                }
            }
        }

        private void DownTrackBtn_Click(object sender, EventArgs e) {
            LoadedPlaylist? selectedPlaylist = this.PlaylistsListBox.SelectedItem?.Playlist;
            var selectedIndex = PlaylistTrackOrder.SelectedIndex;
            AppendedTrack? selectedTrack = (AppendedTrack?)PlaylistTrackOrder.SelectedItem?.Tag;
            if (selectedPlaylist != null && selectedTrack != null)
            {
                var moved = selectedPlaylist.MoveTrackDown(selectedTrack);
                if (moved != 0)
                {
                    UpdatePlaylistUI();
                    this.PlaylistTrackOrder.SelectedIndex = selectedIndex + moved;
                    UnsavedChangesIcon(selectedPlaylist);
                }
            }
        }

        private void UnsavedChangesIcon(LoadedPlaylist playlist) {
            LoadedPlaylistListItem? item = PlaylistsListBox.FindItem(playlist);
            if (item != null)
            {
                if (!item.Icons.Contains(UnsavedIconKey))
                {
                    item.Icons.Add(UnsavedIconKey);
                    this.PlaylistsListBox.Invalidate();
                }
            }
        }

        private void ClearUnsavedChangesIcon(LoadedPlaylist playlist) {
            LoadedPlaylistListItem? item = PlaylistsListBox.FindItem(playlist);
            if (item != null)
            {
                item.Icons.Remove(UnsavedIconKey);
                this.PlaylistsListBox.Invalidate();
            }
        }

        private void ErrorIcon(LoadedPlaylist playlist) {
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

        private void ClearErrorIcon(LoadedPlaylist playlist) {
            LoadedPlaylistListItem? item = PlaylistsListBox.FindItem(playlist);
            if (item != null)
            {
                item.Icons.Remove(WarningIconKey);
                this.PlaylistsListBox.Invalidate();
            }
        }

        private bool HasErrors(LoadedPlaylist playlist) {
            LoadedPlaylistListItem? item = PlaylistsListBox.FindItem(playlist);
            return item?.Icons?.Contains(WarningIconKey) ?? false;
        }

        private void ApplyPlaylistButton_Click(object sender, EventArgs e) {
            LoadedPlaylistListItem? selectedItem = PlaylistsListBox.SelectedItem;
            if (selectedItem != null)
            {
                if (HasErrors(selectedItem.Playlist))
                {
                    MessageBox.Show("Please resolve errors to save.", "Errors", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                selectedItem.Playlist.Name = this.PlaylistNameTextBox.Text;
                selectedItem.Playlist.Save();
                ClearUnsavedChangesIcon(selectedItem.Playlist);
                this.PlaylistsListBox.Invalidate();
            }
        }

        private void DeletePlaylistButton_Click(object sender, EventArgs e) {
            LoadedPlaylistListItem? selectedItem = PlaylistsListBox.SelectedItem;
            if (selectedItem != null)
            {
                if (MessageBox.Show("Can not be undone. Are you sure?", "Delete?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    this.PlaylistsListBox.Remove(selectedItem);
                    this.Renames.Playlists.Remove(selectedItem.Playlist.RenameData);
                }
            }
        }
    }
}
