using JsonSerializable;
using MakeMKV_Title_Decoder.Data;
using MakeMKV_Title_Decoder.Forms.TmdbBrowser;
using MakeMKV_Title_Decoder.libs.MkvToolNix;
using MakeMKV_Title_Decoder.libs.MkvToolNix.Data;
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

namespace MakeMKV_Title_Decoder.Forms.FileRenamer
{
    public partial class FileRenamerForm : Form {
        const string IconError = "dialog-error.png";
        const string IconGood = "dialog-ok-apply.png";
        const string IconWarn = "dialog-warning.png";

        const string TmdbPrefix = "tmdbid=";

        private RenameData Renames;

        private bool FirstTmdb = true;
        private string? lastDir = null;

        private FeatureType SelectedType = FeatureType.MainFeature;

        private Dictionary<object, FeatureType> RadioButtonLookup;
        private Dictionary<FeatureType, RadioButton> RadioButtonReverseLookup;

        public FileRenamerForm(RenameData renames) {
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
            this.ShowNameTextBox.Text = Utils.GetFileSafeName(this.Renames.Disc.Identity.Title ?? "");
        }

        private void PlaylistListBox1_SelectedIndexChanged(object sender, EventArgs e) {
            var selected = this.PlaylistListBox1.SelectedItem;
            var output = selected?.Playlist?.OutputFile;

            this.OptionsPanel.Enabled = (selected != null);
            this.MultiVersionCheckBox.Checked = (output?.MultiVersion != null);
            this.MultiVersionTextBox.Text = (output?.MultiVersion ?? "");

            TypeComboBox.SelectedItem = output?.ShowID?.Type;
            IdTextBox.Text = output?.ShowID?.ID.ToString() ?? "";
            SeasonTextBox.Text = output?.ShowID?.Season?.ToString() ?? "";
            EpisodeTextBox.Text = output?.ShowID?.Episode?.ToString() ?? "";
            ShowNameTextBox.Text = output?.ShowName ?? "";

            bool isMovie = true; //((ShowType?)this.TypeComboBox.SelectedItem == ShowType.Movie);
            this.ExtrasRadioButton.Enabled = isMovie;
            this.SpecialsRadioButton.Enabled = isMovie;
            this.ShortsRadioButton.Enabled = isMovie;
            this.ScenesRadioButton.Enabled = isMovie;
            this.FeaturettesRadioButton.Enabled = isMovie;
            this.BehindTheScenesRadioButton.Enabled = isMovie;
            this.DeletedScenesRadioButton.Enabled = isMovie;
            this.InterviewsRadioButton.Enabled = isMovie;

            RadioButtonReverseLookup[output?.Type ?? FeatureType.MainFeature].PerformClick();
            this.ExtraNameTextBox.Text = (output?.ExtraName ?? "");

            UpdateOutputLabel();
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
            this.OutputFolderLabel.Text = $"{this.ShowNameTextBox.Text} [{TmdbPrefix}{ID?.ID.ToString() ?? ""}]";
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

            if (playlist.OutputFile.Type == null) return true;
            if (string.IsNullOrWhiteSpace(playlist.OutputFile.ShowName)) return true;
            if (playlist.OutputFile.MultiVersion != null && string.IsNullOrWhiteSpace(playlist.OutputFile.MultiVersion)) return true;
            if (playlist.OutputFile.Type != null && playlist.OutputFile.Type != FeatureType.MainFeature && string.IsNullOrEmpty(playlist.OutputFile.ExtraName)) return true;

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
                playlist.OutputFile.Type = SelectedType;
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

        private void Export(params Playlist[] playlists) {
            string rootFolder;
            using (FolderBrowserDialog browser = new())
            {
                if (lastDir != null)
                {
                    browser.InitialDirectory = lastDir;
                }

                if (browser.ShowDialog() == DialogResult.OK)
                {
                    rootFolder = browser.SelectedPath;
                } else
                {
                    return;
                }
            }

            // Get a list of all exported items
            List<Tuple<string, string?, string, Playlist>> exports = new();
            foreach (var playlist in playlists)
            {
                OutputName output = playlist.OutputFile;
                try
                {
                    string outputFolder = output.GetFolderPath();
                    string? bonusFolder = output.GetBonusFolder();
                    string outputFile = output.GetFileName();
                    exports.Add(new(outputFolder, bonusFolder, outputFile, playlist));
                } catch(Exception ex)
                {
                    if (MessageBox.Show($"Failed to determine output file for playlist '{playlist.Name}'. Continue anyways?", "Error", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning) != DialogResult.Yes)
                    {
                        return;
                    } else
                    {
                        continue;
                    }
                }
            }

            // Save metadata to output if desired
            bool saveMeta = false;
            if (MessageBox.Show("Save rename metadata?", "Save data?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                saveMeta = true;
            }

            List<string> metaFolders = new();

            // Export files
            for (int i = 0; i < exports.Count; i++)
            {
                string outputFolder = exports[i].Item1;
                string? bonusFolder = exports[i].Item2;
                string outputFile = exports[i].Item3;
                var playlist = exports[i].Item4;

                string fullPath;
                if (bonusFolder == null)
                {
                    fullPath = Path.Combine(rootFolder, outputFolder);
                } else
                {
                    fullPath = Path.Combine(rootFolder, outputFolder, bonusFolder);
                }

                if (!Directory.Exists(fullPath))
                {
                    // Attempt to create directory
                    try
                    {
                        Directory.CreateDirectory(fullPath);
                    } catch (Exception)
                    {
                        MessageBox.Show("Failed to create output folder.");
                        continue;
                    }
                }

                if (saveMeta && !metaFolders.Contains(outputFolder))
                {
                    metaFolders.Add(outputFolder);

                    try
                    {
                        string optional = "";
                        if (Renames.Disc.Data.NumberOfSets != null || Renames.Disc.Data.SetNumber != null)
                        {
                            optional = $" {Renames.Disc.Data.SetNumber?.ToString() ?? "_"} of {Renames.Disc.Data.NumberOfSets?.ToString() ?? "_"}";
                        }

                        string file = Path.Combine(rootFolder, outputFolder, ".metadata", $"{Utils.GetFileSafeName($"{Renames.Disc.Identity.Title}{optional}")}.json");
                        Directory.CreateDirectory(Path.Combine(rootFolder, outputFolder, ".metadata"));
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

                MkvToolNixInterface.MergeAsync(Renames.Disc, playlist, Path.Combine(fullPath, outputFile), new SimpleProgress((uint)i, (uint)exports.Count));
            }
        }

        private void ExportAllBtn_Click(object sender, EventArgs e) {
            List<Playlist> exports = new();
            foreach (var selected in this.PlaylistListBox1.AllItems)
            {
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

                    exports.Add(selected.Playlist);
                }
            }

            Export(exports.ToArray());
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

        private void ExportSelectedBtn_Click(object sender, EventArgs e) {
            var selected = PlaylistListBox1.SelectedItem;
            if (selected != null)
            {
                if (selected.Icon != IconGood)
                {
                    MessageBox.Show($"Playlist '{selected.Playlist.Name}' has an error and can't be exported.");
                    return;
                }
                Export(selected.Playlist);
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            var selected = PlaylistListBox1.SelectedItem;
            if (selected != null)
            {
                foreach (var item in PlaylistListBox1.AllItems)
                {
                    if (item == selected) continue;

                    var ID = selected.Playlist.OutputFile.ShowID;
                    if (ID != null) ID = new TmdbID(ID);
                    item.Playlist.OutputFile.ShowID = ID;

                    var name = selected.Playlist.OutputFile.ShowName;
                    item.Playlist.OutputFile.ShowName = name;

                    CheckForErrors(item);
                }

                PlaylistListBox1.Invalidate();
            }
        }
    }
}
