using JsonSerializable;
using MakeMKV_Title_Decoder.Data;
using MakeMKV_Title_Decoder.Forms.TmdbBrowser;
using MakeMKV_Title_Decoder.MkvToolNix;
using MakeMKV_Title_Decoder.MkvToolNix.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.ComponentModel.Design.ObjectSelectorEditor;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace MakeMKV_Title_Decoder.Forms.FileRenamer {
    public partial class FileRenamerForm : Form {
        const string IconError = "dialog-error.png";
        const string IconGood = "dialog-ok-apply.png";
        const string IconWarn = "dialog-warning.png";

        const string TmdbPrefix = "tmdbid=";

        private MkvToolNixDisc Disc;
        private RenameData2 Renames;

        private string? OutputPath = null;
        private string? OutputFolderName = null;
        private string OutputFullPath = "";

        private bool FirstTmdb = true;
        private string? lastDir = null;

        private FeatureType SelectedType = FeatureType.MainFeature;

        private Dictionary<object, FeatureType> RadioButtonLookup;
        private Dictionary<FeatureType, RadioButton> RadioButtonReverseLookup;

        public FileRenamerForm(MkvToolNixDisc disc, RenameData2 renames) {
            this.Disc = disc;
            this.Renames = renames;

            InitializeComponent();

            RadioButtonLookup = new() {
                { this.MainFeatureRadioButton, FeatureType.MainFeature },
                { this.TrailersRadioButton, FeatureType.Trailers },
                { this.ExtrasRadioButton, FeatureType.Extras },
                { this.SpecialsRadioButton, FeatureType.Specials },
                { this.ShortsRadioButton, FeatureType.Shorts },
                { this.ScenesRadioButton, FeatureType.Scenes },
                { this.FeaturettesRadioButton, FeatureType.Featurettes },
                { this.BehindTheScenesRadioButton, FeatureType.BehindTheScenes },
                { this.DeletedScenesRadioButton, FeatureType.DeletedScenes },
                { this.InterviewsRadioButton, FeatureType.Interviews }
            };

            RadioButtonReverseLookup = new() {
                { FeatureType.MainFeature, this.MainFeatureRadioButton },
                { FeatureType.Trailers, this.TrailersRadioButton },
                { FeatureType.Extras, this.ExtrasRadioButton },
                { FeatureType.Specials, this.SpecialsRadioButton },
                { FeatureType.Shorts, this.ShortsRadioButton },
                { FeatureType.Scenes, this.ScenesRadioButton },
                { FeatureType.Featurettes, this.FeaturettesRadioButton },
                { FeatureType.BehindTheScenes, this.BehindTheScenesRadioButton },
                { FeatureType.DeletedScenes, this.DeletedScenesRadioButton },
                { FeatureType.Interviews, this.InterviewsRadioButton }
            };

            this.PlaylistListBox1.Clear();
            foreach (var playlist in renames.Playlists)
            {
                var item = new PlaylistListItem(playlist);
                item.Icon = IconError;
                this.PlaylistListBox1.Add(item);
                CheckForErrors(item);
            }

            TypeComboBox.Items.Clear();
            foreach (var type in Enum.GetValues<ShowType>())
            {
                TypeComboBox.Items.Add(type);
            }


            TypeComboBox_SelectedIndexChanged(null, null);
            PlaylistListBox1_SelectedIndexChanged(null, null);
        }

        private void FileRenamerForm_Load(object sender, EventArgs e) {
            this.ShowNameTextBox.Text = Utils.GetFileSafeName(this.Disc.Title ?? "");
        }

        private void PlaylistListBox1_SelectedIndexChanged(object sender, EventArgs e) {
            var selected = this.PlaylistListBox1.SelectedItem;
            var output = selected?.Playlist?.OutputFile;

            this.OptionsPanel.Enabled = (selected != null);
            this.MultiVersionCheckBox.Checked = (output?.MultiVersion != null);
            this.MultiVersionTextBox.Text = (output?.MultiVersion ?? "");

            bool isMovie = true; //((ShowType?)this.TypeComboBox.SelectedItem == ShowType.Movie);
            this.ExtrasRadioButton.Enabled = isMovie;
            this.SpecialsRadioButton.Enabled = isMovie;
            this.ShortsRadioButton.Enabled = isMovie;
            this.ScenesRadioButton.Enabled = isMovie;
            this.FeaturettesRadioButton.Enabled = isMovie;
            this.BehindTheScenesRadioButton.Enabled = isMovie;
            this.DeletedScenesRadioButton.Enabled = isMovie;
            this.InterviewsRadioButton.Enabled = isMovie;

            RadioButtonReverseLookup[output?.FeatureType ?? FeatureType.MainFeature].PerformClick();
            this.ExtraNameTextBox.Text = (output?.ExtraName ?? "");
        }

        private void TmdbBtn_Click(object sender, EventArgs e) {
            var form = new TmdbBrowserForm(!FirstTmdb, false, false);
            FirstTmdb = false;
            if (form.ShowDialog() == DialogResult.OK)
            {
                var ID = form.ID;
                if (ID != null)
                {
                    this.TypeComboBox.SelectedItem = ID.Type;
                    this.IdTextBox.Text = ID.ID.ToString();
                    this.SeasonTextBox.Text = ID.Season?.ToString() ?? "";
                    this.EpisodeTextBox.Text = ID.Episode?.ToString() ?? "";

                    UpdateOutputLabel();
                }
            }
        }

        private void TypeComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            var type = (ShowType?)TypeComboBox.SelectedItem;
            this.IdTextBox.Enabled = (type != null);
            this.SeasonTextBox.Enabled = (type == ShowType.TV);
            this.EpisodeTextBox.Enabled = (type == ShowType.TV);

            UpdateOutputLabel();
        }

        private void SelectOutputFolderBtn_Click(object sender, EventArgs e) {
            using (FolderBrowserDialog openFileDialog = new FolderBrowserDialog())
            {
                if (lastDir != null)
                {
                    openFileDialog.InitialDirectory = lastDir;
                }

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string path = openFileDialog.SelectedPath;
                    lastDir = path;
                    string dir = Path.GetFileNameWithoutExtension(path);
                    // Verify it matches selected TMDB ID

                    bool isValid = false;
                    int start = dir.LastIndexOf('[');
                    int end = dir.LastIndexOf(']');
                    if (start >= 0 && end >= 0 && start < end)
                    {
                        string showName = dir.Substring(0, start).TrimEnd();

                        string tmdb = dir.Substring(start + 1, end - start - 1);
                        if (tmdb.StartsWith(TmdbPrefix))
                        {
                            this.ShowNameTextBox.Text = showName;
                            this.IdTextBox.Text = tmdb.Substring(TmdbPrefix.Length);
                            isValid = true;
                        }
                    }

                    if (!isValid)
                    {
                        if (MessageBox.Show("Failed to parse show title/ID. Folder will be renamed as needed.", "Rename?", MessageBoxButtons.OKCancel) != DialogResult.OK)
                        {
                            return;
                        }

                        if (string.IsNullOrWhiteSpace(this.ShowNameTextBox.Text))
                        {
                            MessageBox.Show("Show name can't be empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        long id;
                        if (!long.TryParse(this.IdTextBox.Text, out id))
                        {
                            MessageBox.Show("ID must be a valid number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        try
                        {
                            string parent = Directory.GetParent(path).FullName;
                            dir = $"{this.ShowNameTextBox.Text} [{TmdbPrefix}{id}]";
                            path = Path.Combine(parent, dir);
                            Directory.Move(path, path);

                            this.OutputPath = parent;
                            this.OutputFolderName = dir;
                            this.OutputFullPath = path;
                        } catch (Exception)
                        {
                            MessageBox.Show("Failed to rename folder.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        UpdateOutputLabel();
                    } else
                    {
                        try
                        {
                            this.OutputPath = Directory.GetParent(path).FullName;
                            this.OutputFolderName = dir;
                            this.OutputFullPath = path;
                        } catch (Exception)
                        {
                            MessageBox.Show("Failed to find folder.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        UpdateOutputLabel();
                    }
                }
            }
        }

        private void CreateFolderBtn_Click(object sender, EventArgs e) {
            using (FolderBrowserDialog openFileDialog = new FolderBrowserDialog())
            {
                if (lastDir != null)
                {
                    openFileDialog.InitialDirectory = lastDir;
                }

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string path = openFileDialog.SelectedPath;
                    lastDir = path;

                    if (string.IsNullOrWhiteSpace(this.ShowNameTextBox.Text))
                    {
                        MessageBox.Show("Show name can't be empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    long id;
                    if (!long.TryParse(this.IdTextBox.Text, out id))
                    {
                        MessageBox.Show("ID must be a valid number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    string dir = $"{this.ShowNameTextBox.Text} [{TmdbPrefix}{id}]";
                    try
                    {
                        Directory.CreateDirectory(Path.Combine(path, dir));
                    } catch (Exception)
                    {
                        MessageBox.Show("Failed to create folder.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    this.OutputPath = path;
                    this.OutputFolderName = dir;
                    this.OutputFullPath = Path.Combine(path, dir);

                    UpdateOutputLabel();
                }
            }
        }

        private void ShowNameTextBox_TextChanged_1(object sender, EventArgs e) {
            UpdateOutputLabel();
        }

        private void MultiVersionCheckBox_CheckedChanged(object sender, EventArgs e) {
            this.MultiVersionTextBox.Enabled = MultiVersionCheckBox.Checked;
        }

        private void FeatureTypeRadioButton_CheckedChanged(object sender, EventArgs e) {
            FeatureType type;
            if (sender == null)
            {
                type = FeatureType.MainFeature;
            } else
            {
                type = RadioButtonLookup[sender];
            }
            this.SelectedType = type;

            this.ExtraNameTextBox.Enabled = (type != FeatureType.MainFeature);
        }

        private void UpdateOutputLabel() {
            var ID = ParseID();
            this.OutputFolderName = $"{this.ShowNameTextBox.Text} [{TmdbPrefix}{ID?.ID.ToString() ?? ""}]";
            if (string.IsNullOrWhiteSpace(this.OutputFolderName))
            {
                this.OutputFolderName = null;
            }

            if (this.OutputPath == null)
            {
                this.OutputFullPath = this.OutputFolderName ?? "";
            } else
            {
                if (this.OutputFolderName == null)
                {
                    this.OutputFullPath = this.OutputPath;
                } else
                {
                    this.OutputFullPath = Path.Combine(this.OutputPath, this.OutputFolderName);
                }
            }

            this.OutputFolderLabel.Text = this.OutputFullPath;
        }

        private void ShowNameTextBox_TextChanged(object sender, EventArgs e) {
            // Rename folder
            this.OutputFolderName = ShowNameTextBox.Text;
            if (this.OutputPath == null)
            {
                this.OutputFullPath = this.OutputFolderName;
            } else
            {
                this.OutputFullPath = Path.Combine(this.OutputPath, this.OutputFullPath);
            }
            UpdateOutputLabel();
        }

        private TmdbID? ParseID() {
            TmdbID? id = null;
            ShowType? type = (ShowType?)this.TypeComboBox.SelectedItem;

            long idNum;
            if (long.TryParse(this.IdTextBox.Text, out idNum) && type != null)
            {
                id = new TmdbID(type.Value, idNum);

                if (!string.IsNullOrWhiteSpace(this.SeasonTextBox.Text))
                {
                    if (!long.TryParse(this.SeasonTextBox.Text, out idNum))
                    {
                        id = null;
                    } else
                    {
                        id.Season = idNum;
                    }
                }
                if (!string.IsNullOrWhiteSpace(this.EpisodeTextBox.Text))
                {
                    if (!long.TryParse(this.EpisodeTextBox.Text, out idNum))
                    {
                        id = null;
                    } else
                    {
                        id.Episode = idNum;
                    }
                }
            }

            return id;
        }

        private bool IsError(Playlist playlist) {
            TmdbID? id = playlist.OutputFile.ShowID;
            if (id == null) return true;
            if (id.Type == ShowType.TV)
            {
                if (id.Season == null || id.Episode == null) return true;
            }

            if (playlist.OutputFile.FeatureType == null) return true;
            if (string.IsNullOrWhiteSpace(playlist.OutputFile.ShowName)) return true;
            if (playlist.OutputFile.MultiVersion != null && string.IsNullOrWhiteSpace(playlist.OutputFile.MultiVersion)) return true;
            if (playlist.OutputFile.FeatureType != null && playlist.OutputFile.FeatureType != FeatureType.MainFeature && string.IsNullOrEmpty(playlist.OutputFile.ExtraName)) return true;

            return false;
        }

        private void CheckForErrors(PlaylistListItem item) {
            var playlist = item.Playlist;
            if (playlist.OutputFile.ShowID == null)
            {
                item.Icon = IconError;
            } else
            {
                if (IsError(playlist))
                {
                    item.Icon = IconWarn;
                } else
                {
                    item.Icon = IconGood;
                }
            }

            PlaylistListBox1.Invalidate();
        }

        private void ApplyChangesBtn_Click(object sender, EventArgs e) {
            var selectedItem = this.PlaylistListBox1.SelectedItem;
            if (selectedItem != null)
            {
                Playlist playlist = selectedItem.Playlist;

                playlist.OutputFile.ShowName = this.ShowNameTextBox.Text;
                playlist.OutputFile.MultiVersion = (this.MultiVersionCheckBox.Checked ? this.MultiVersionTextBox.Text : null);
                playlist.OutputFile.ShowID = ParseID();
                playlist.OutputFile.FeatureType = SelectedType;
                playlist.OutputFile.ExtraName = (this.SelectedType == FeatureType.MainFeature) ? null : this.ExtraNameTextBox.Text;

                CheckForErrors(selectedItem);
            }
        }

        private string? GetFolderName(FeatureType type) {
            switch (type)
            {
                case FeatureType.Extras: return "extras";
                case FeatureType.Specials: return "specials";
                case FeatureType.Shorts: return "shorts";
                case FeatureType.Scenes: return "scenes";
                case FeatureType.Featurettes: return "featurettes";
                case FeatureType.BehindTheScenes: return "behind the scenes";
                case FeatureType.DeletedScenes: return "deleted scenes";
                case FeatureType.Interviews: return "interviews";
                case FeatureType.Trailers: return "trailers";
                default:
                    return null;
            }
        }

        private void ExportAllBtn_Click(object sender, EventArgs e) {
            if (this.OutputFolderName == null || this.OutputPath == null)
            {
                MessageBox.Show("Please select a valid output folder");
                return;
            }

            if (!Directory.Exists(this.OutputFullPath))
            {
                // Attempt to create directory
                try
                {
                    Directory.CreateDirectory(this.OutputFullPath);
                } catch (Exception)
                {
                    MessageBox.Show("Failed to create output folder.");
                    return;
                }
            }

            // Get a list of all exported items
            List<Tuple<string, Playlist>> exports = new();
            foreach (var selected in this.PlaylistListBox1.AllItems) {
                if (selected != null)
                {
                    if (selected.Icon != IconGood)
                    {
                        if (MessageBox.Show($"Playlist '{selected.Playlist.Name}' has an error and can't be exported. Continue anyways?", "Error", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning) != DialogResult.Yes)
                        {
                            return;
                        } else
                        {
                            continue;
                        }
                    }

                    string? outputFile = null;
                    OutputName output = selected.Playlist.OutputFile;

                    string versionString = "";
                    if (output.MultiVersion != null)
                    {
                        versionString = $" - {output.MultiVersion}";
                    }

                    if (output.ShowID?.Type == ShowType.Movie)
                    {
                        string? bonusFolder = GetFolderName(output.FeatureType ?? FeatureType.MainFeature);
                        if (bonusFolder != null)
                        {
                            outputFile = Path.Combine(this.OutputFullPath, bonusFolder, $"{output.ExtraName ?? ""}{versionString}.mkv");
                        } else
                        {
                            outputFile = Path.Combine(this.OutputFullPath, $"{output.ShowName ?? ""} [{TmdbPrefix}{output.ShowID?.ID.ToString() ?? ""}]{versionString}.mkv");
                        }
                    } else if (output.ShowID?.Type == ShowType.TV)
                    {
                        outputFile = Path.Combine(this.OutputFullPath, $"Season {output.ShowID.Season}");

                        string? bonusFolder = GetFolderName(output.FeatureType ?? FeatureType.MainFeature);
                        if (bonusFolder != null)
                        {
                            outputFile = Path.Combine(outputFile, bonusFolder, $"{output.ExtraName ?? ""}{versionString}.mkv");
                        } else
                        {
                            outputFile = Path.Combine(outputFile, $"{output.ShowName ?? ""} S{output.ShowID.Season}E{output.ShowID.Episode}{versionString}.mkv");
                        }
                    }

                    if (outputFile == null)
                    {
                        if(MessageBox.Show($"Failed to determine output file for playlisy '{selected.Playlist.Name}'. Continue anyways?", "Error", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning) != DialogResult.Yes)
                        {
                            return;
                        } else
                        {
                            continue;
                        }
                    }

                    exports.Add(new(outputFile, selected.Playlist));
                }
            }

            // Save metadata to output if desired
            if (MessageBox.Show("Save rename metadata?", "Save data?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    string optional = "";
                    if (Disc.NumberOfSets != null || Disc.SetNumber != null)
                    {
                        optional = $" {Disc.SetNumber?.ToString() ?? "_"} of {Disc.NumberOfSets?.ToString() ?? "_"}";
                    }

                    string file = Path.Combine(this.OutputFullPath, ".metadata", $"{Utils.GetFileSafeName($"{Disc.Title}{optional}")}.json");
                    Directory.CreateDirectory(Path.Combine(this.OutputFullPath, ".metadata"));
                    if (File.Exists(file))
                    {
                        File.Delete(file);
                    }
                    Json.Write(this.Renames, file);
                } catch (Exception)
                {
                    MessageBox.Show("Failed to save file.", "Failed", MessageBoxButtons.OK);
                }
            }

            // Export files
            for(int i = 0; i < exports.Count; i++)
            {
                string outputFile = exports[i].Item1;
                var playlist = exports[i].Item2;
                MkvToolNixInterface.MergeAsync(this.Disc, playlist, outputFile, new SimpleProgress((uint)i, (uint)exports.Count));
            }
        }

        private void IdTextBox_TextChanged(object sender, EventArgs e) {
            UpdateOutputLabel();
        }

        private void SeasonTextBox_TextChanged(object sender, EventArgs e) {
            UpdateOutputLabel();
        }

        private void EpisodeTextBox_TextChanged(object sender, EventArgs e) {
            UpdateOutputLabel();
        }

        private void IdTextBox_TextChanged_1(object sender, EventArgs e) {
            UpdateOutputLabel();
        }
    }
}
