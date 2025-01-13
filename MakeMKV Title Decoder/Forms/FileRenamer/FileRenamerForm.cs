using MakeMKV_Title_Decoder.Data;
using MakeMKV_Title_Decoder.Forms.TmdbBrowser;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization.Metadata;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;
using MakeMKV_Title_Decoder.Data.Renames;
using FFMpeg_Wrapper.Filters.Video;
using MakeMKV_Title_Decoder.Util;

namespace MakeMKV_Title_Decoder.Forms.FileRenamer
{
    public interface Exportable
    {
        /// <summary>
        /// The user-given name of the collection/playlist
        /// </summary>
        public string Name { get; }

        public OutputName OutputFile { get; }

        public bool IsTranscodable { get; }

        public bool Export(LoadedDisc disc, string outputFolder, string outputFile, IProgress<SimpleProgress>? progress, SimpleProgress? totalProgress);
        public bool ExportTranscoding(LoadedDisc disc, string outputFolder, string outputFile, ScaleResolution resolution, IProgress<SimpleProgress>? progress, SimpleProgress? totalProgress);
    }

    public partial class FileRenamerForm : Form {
        const string IconError = "dialog-error.png";
        const string IconGood = "dialog-ok-apply.png";
        const string IconWarn = "dialog-warning.png";

        const string TmdbPrefix = "tmdbid=";

        private LoadedDisc Disc;

        private bool FirstTmdb = true;
        private string? lastDir = null;

        private FeatureType SelectedType = FeatureType.MainFeature;

        private Dictionary<object, FeatureType> RadioButtonLookup;
        private Dictionary<FeatureType, RadioButton> RadioButtonReverseLookup;

        public FileRenamerForm(LoadedDisc disc) {
            this.Disc = disc;

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
                { this.InterviewsRadioButton, FeatureType.Interviews },
                { this.GalleryRadioButton, FeatureType.Gallery }
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
                { FeatureType.Interviews, this.InterviewsRadioButton },
                { FeatureType.Gallery, this.GalleryRadioButton }
            };

            this.ExportableListBox1.Clear();
            foreach (Exportable exportable in disc.RenameData.Playlists.Concat<Exportable>(disc.RenameData.Collections))
            {
                var item = new ExportableListItem(exportable);
                item.Icon = IconError;
                this.ExportableListBox1.Add(item);
                CheckForErrors(item);
            }

            ExportableListBox1_SelectedIndexChanged(null, null);
            EpisodeRangeCheckBox_CheckedChanged(null, null);
        }

        private void FileRenamerForm_Load(object sender, EventArgs e) {
        }

        private void ExportableListBox1_SelectedIndexChanged(object sender, EventArgs e) {
            var selected = this.ExportableListBox1.SelectedItem;
            var output = selected?.Export?.OutputFile;

            this.OptionsPanel.Enabled = (selected != null);

            ShowOutputName? showName = null;
            if (output != null && output.ShowIndex >= 0) showName = this.Disc.RenameData.ShowOutputNames[(int)output.ShowIndex];
            this.SelectedShowLabel.Text = "Show: " + (showName?.Name ?? "");

            SeasonTextBox.Visible = (showName?.Type == ShowType.TV);
            long temp = output?.Season ?? -1;
            if (temp < 0) SeasonTextBox.Text = "";
            else SeasonTextBox.Text = temp.ToString();

            EpisodeTextBox.Visible = (showName?.Type == ShowType.TV);
            temp = output?.Episode ?? -1;
            if (temp < 0) EpisodeTextBox.Text = "";
            else EpisodeTextBox.Text = temp.ToString();

            this.TmdbBtn.Visible = (showName?.Type == ShowType.TV);

            // TV shows can have the same folders as movies now???
            bool isMovie = true; //((ShowType?)this.TypeComboBox.SelectedItem == ShowType.Movie);
            this.ExtrasRadioButton.Enabled = isMovie;
            this.SpecialsRadioButton.Enabled = isMovie;
            this.ShortsRadioButton.Enabled = isMovie;
            this.ScenesRadioButton.Enabled = isMovie;
            this.FeaturettesRadioButton.Enabled = isMovie;
            this.BehindTheScenesRadioButton.Enabled = isMovie;
            this.DeletedScenesRadioButton.Enabled = isMovie;
            this.InterviewsRadioButton.Enabled = isMovie;
            this.GalleryRadioButton.Enabled = isMovie;

            RadioButtonReverseLookup[output?.Type ?? FeatureType.MainFeature].PerformClick();
            this.ExtraNameTextBox.Text = (output?.ExtraName ?? "");

            ExtraNameTextBox_TextChanged(null, null);

            EpisodeRangeCheckBox.Checked = (output?.MultipleEpisodesRange != null);
            EpisodeRangeTextBox.Text = (output?.MultipleEpisodesRange?.ToString() ?? "");
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

            ExtraNameTextBox_TextChanged(null, null);
        }

        private TmdbID? ParseID() {
            ExportableListItem export;
            if (this.ExportableListBox1.SelectedItem == null) return null;
            if (this.ExportableListBox1.SelectedItem is ExportableListItem item)
            {
                export = item;
            } else
            {
                return null;
            }

            ShowOutputName? showName = null;
            if (export.Export.OutputFile.ShowIndex >= 0)
            {
                showName = this.Disc.RenameData.ShowOutputNames[(int)export.Export.OutputFile.ShowIndex];
            }

            TmdbID? id = null;
            if (showName != null)
            {
                id = new TmdbID(showName.Type, showName.TmdbID);

                long temp;
                if (!string.IsNullOrWhiteSpace(this.SeasonTextBox.Text))
                {
                    if (!long.TryParse(this.SeasonTextBox.Text, out temp))
                    {
                        id = null;
                    } else
                    {
                        id.Season = temp;
                    }
                }
                if (!string.IsNullOrWhiteSpace(this.EpisodeTextBox.Text))
                {
                    if (!long.TryParse(this.EpisodeTextBox.Text, out temp))
                    {
                        id = null;
                    } else
                    {
                        id.Episode = temp;
                    }
                }
            }

            return id;
        }

        private bool IsError(Exportable export) {
            TmdbID? id = export.OutputFile.GetTmdbID(this.Disc.RenameData);
            if (id == null) return true;
            if (export.OutputFile.Type == null) return true;

            if (id.Type == ShowType.TV)
            {
                if (export.OutputFile.Type != null)
                {
                    if (export.OutputFile.Type == FeatureType.MainFeature)
                    {
                        if (id.Season == null || id.Episode == null) return true;
                    } else
                    {
                        if (id.Season == null) return true;
                    }
                }

                if (id.Episode == null && export.OutputFile.MultipleEpisodesRange != null)
                {
                    return true;
                }
            }

            if (string.IsNullOrWhiteSpace(export.OutputFile.GetShowName(this.Disc.RenameData)?.Name)) return true;
            if (export.OutputFile.Type != null && export.OutputFile.Type != FeatureType.MainFeature && string.IsNullOrEmpty(export.OutputFile.ExtraName)) return true;

            return false;
        }

        private void CheckForErrors(ExportableListItem item) {
            var export = item.Export;
            if (export.OutputFile.GetShowName(this.Disc.RenameData) == null)
            {
                item.Icon = IconError;
            } else
            {
                if (IsError(export))
                {
                    item.Icon = IconWarn;
                } else
                {
                    item.Icon = IconGood;
                }
            }

            ExportableListBox1.Invalidate();
        }

        private void ApplyChangesBtn_Click(object sender, EventArgs e) {
            var selectedItem = this.ExportableListBox1.SelectedItem;
            if (selectedItem != null)
            {
                Exportable export = selectedItem.Export;

                // Verify names before saving
                if (this.SelectedType != FeatureType.MainFeature)
                {
                    string? error;
                    if (string.IsNullOrEmpty(this.ExtraNameTextBox.Text))
                    {
                        error = "Name can't be empty.";
                    } else
                    {
                        error = Utils.IsValidFileName(this.ExtraNameTextBox.Text);
                    }

                    if (error != null)
                    {
                        MessageBox.Show("Feature name invalid: " + error, "Invalid name");
                        return;
                    }
                }

                export.OutputFile.Type = SelectedType;
                export.OutputFile.ExtraName = (this.SelectedType == FeatureType.MainFeature) ? null : this.ExtraNameTextBox.Text;

                long temp;
                if (long.TryParse(this.SeasonTextBox.Text, out temp))
                {
                    export.OutputFile.Season = temp;
                } else
                {
                    export.OutputFile.Season = -1;
                }
                if (long.TryParse(this.EpisodeTextBox.Text, out temp))
                {
                    export.OutputFile.Episode = temp;
                } else
                {
                    export.OutputFile.Episode = -1;
                }
                if (this.EpisodeRangeCheckBox.Checked && long.TryParse(this.EpisodeRangeTextBox.Text, out temp))
                {
                    export.OutputFile.MultipleEpisodesRange = temp;
                } else
                {
                    export.OutputFile.MultipleEpisodesRange = null;
                }
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
                case FeatureType.Gallery: return "Gallery";
                default:
                    return null;
            }
        }

        private void Export(params Exportable[] exportables) {
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
            List<(string OutputFolder, string? BonusFolder, string OutputFile, Exportable Export, string Metadata)> exports = new();
            foreach (var export in exportables)
            {
                OutputName output = export.OutputFile;
                try
                {
                    ShowOutputName showName = this.Disc.RenameData.ShowOutputNames[(int)output.ShowIndex];
                    string outputFolder = showName.GetFolderPath(output.Season);
                    string? bonusFolder = output.GetBonusFolder(showName.Type);
                    string outputFile = output.GetFileName(showName);
                    string metadataName = showName.MetadataFileName;
                    exports.Add((outputFolder, bonusFolder, outputFile, export, metadataName));
                } catch (Exception ex)
                {
                    if (MessageBox.Show($"Failed to determine output file for export '{export.Name}'. Continue anyways?", "Error", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning) != DialogResult.Yes)
                    {
                        return;
                    } else
                    {
                        continue;
                    }
                }
            }

            var exporterForm = new ExporterForm(this.Disc, exports.Select(x => x.Export));
            if (exporterForm.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            // Save metadata to output if desired
            bool saveMeta = false;
            if (MessageBox.Show("Save rename metadata?", "Save data?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                saveMeta = true;
            }

            List<string> metaFolders = new();
            List<string> exportErrors = new();

            // Export files
            TaskProgressViewerForm.Run((IProgress<SimpleProgress> progress) =>
            {
                SimpleProgress currentProgress = new();
                currentProgress.TotalMax = (uint)exports.Count * 100;
                progress?.Report(currentProgress);

                for (int i = 0; i < exports.Count; i++)
                {
                    currentProgress.Total = (uint)i * 100;
                    progress?.Report(currentProgress);

                    string outputFolder = exports[i].OutputFolder;
                    string? bonusFolder = exports[i].BonusFolder;
                    string outputFile = exports[i].OutputFile;
                    var export = exports[i].Export;
                    string metadataFileName = exports[i].Metadata;

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
                            string file = Path.Combine(rootFolder, outputFolder, ".metadata", $"{metadataFileName}.json");
                            Directory.CreateDirectory(Path.Combine(rootFolder, outputFolder, ".metadata"));

                            var options = new JsonSerializerOptions { WriteIndented = false, TypeInfoResolver = new DefaultJsonTypeInfoResolver() };
                            using var stream = File.Create(file);
                            JsonSerializer.Serialize(stream, this.Disc.RenameData, options);
                            stream.Flush();
                            stream.Close();
                        } catch (Exception)
                        {
                            MessageBox.Show("Failed to save file.", "Failed", MessageBoxButtons.OK);
                        }
                    }

                    Action<bool, string> checkExportError = (success, name) =>
                    {
                        if (!success)
                        {
                            exportErrors.Add($"Failed to export \"{name}\"");
                        }
                    };

                    if (export.IsTranscodable)
                    {
                        IEnumerable<ScaleResolution?> selectedResolutions;
                        bool rename;
                        if (bonusFolder == null)
                        {
                            // Main feature
                            selectedResolutions = exporterForm.SelectedMainFeatureResolutions;
                            rename = true;
                        } else
                        {
                            // Extras
                            selectedResolutions = [exporterForm.SelectedExtrasResolutions];
                            rename = false;
                        }

                        foreach (ScaleResolution? scale in selectedResolutions)
                        {
                            string multiversionName = outputFile;
                            if (rename)
                            {
                                multiversionName = GetMultiversionFileName(outputFile, scale);
                            }

                            bool result;
                            if (scale == null)
                            {
                                // Original quality
                                result = export.Export(this.Disc, fullPath, multiversionName, progress, new SimpleProgress((uint)i, (uint)exports.Count));
                            } else
                            {
                                // Transcode
                                result = export.ExportTranscoding(this.Disc, fullPath, multiversionName, scale.Value, progress, new SimpleProgress((uint)i, (uint)exports.Count));
                            }
                            checkExportError(result, export.Name);
                        }
                    } else
                    {
                        bool result = export.Export(this.Disc, fullPath, outputFile, progress, new SimpleProgress((uint)i, (uint)exports.Count));
                        checkExportError(result, export.Name);
                    }
                }

                currentProgress.Total = currentProgress.TotalMax;
                progress?.Report(currentProgress);
            });

            if (exportErrors.Count == 0)
            {
                SoundPlayer.Play(SoundPlayer.HappySound);
            } else
            {
                SoundPlayer.Play(SoundPlayer.SadSound);
                foreach (var error in exportErrors)
                {
                    MessageBox.Show(error);
                }
            }
        }

        private void ExportAllBtn_Click(object sender, EventArgs e) {
            List<Exportable> exports = new();
            foreach (var selected in this.ExportableListBox1.AllItems)
            {
                if (selected != null)
                {
                    if (selected.Icon != IconGood)
                    {
                        if (MessageBox.Show($"Export '{selected.Export.Name}' has an error and can't be exported. Continue anyways?", "Error", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning) != DialogResult.Yes)
                        {
                            return;
                        } else
                        {
                            continue;
                        }
                    }

                    exports.Add(selected.Export);
                }
            }

            Export(exports.ToArray());
        }

        private void SeasonTextBox_TextChanged(object sender, EventArgs e) {

        }

        private void EpisodeTextBox_TextChanged(object sender, EventArgs e) {

        }

        private void ExportSelectedBtn_Click(object sender, EventArgs e) {
            var selected = ExportableListBox1.SelectedItem;
            if (selected != null)
            {
                if (selected.Icon != IconGood)
                {
                    MessageBox.Show($"Export '{selected.Export.Name}' has an error and can't be exported.");
                    return;
                }

                Export(selected.Export);
            }
        }

        private void CopyToOthersBtn_Click(object sender, EventArgs e) {
            if (this.ExportableListBox1.SelectedItem != null && this.ExportableListBox1.SelectedItem is ExportableListItem export)
            {
                long showIndex = export.Export.OutputFile.ShowIndex;

                foreach (var item in ExportableListBox1.AllItems.Where(x => x != export))
                {
                    item.Export.OutputFile.ShowIndex = showIndex;
                    CheckForErrors(item);
                }

                ExportableListBox1.Invalidate();
            }
        }

        private void SelectShowBtn_Click(object sender, EventArgs e) {
            if (this.ExportableListBox1.SelectedItem != null && this.ExportableListBox1.SelectedItem is ExportableListItem export)
            {
                var form = new ShowSelector(this.Disc.RenameData, this.Disc);
                var result = form.ShowDialog();
                if (result == DialogResult.OK && form.Result != null)
                {
                    export.Export.OutputFile.ShowIndex = this.Disc.RenameData.ShowOutputNames.IndexOf(form.Result);
                    this.SelectedShowLabel.Text = form.Result.Name;

                    CheckForErrors(export);
                    ExportableListBox1.Invalidate();
                    ExportableListBox1_SelectedIndexChanged(null, null);
                }
            }
        }

        private void TmdbBtn_Click(object sender, EventArgs e) {
            if (this.ExportableListBox1.SelectedItem != null && this.ExportableListBox1.SelectedItem is ExportableListItem export)
            {
                TmdbID? defaultID = export.Export.OutputFile.GetTmdbID(this.Disc.RenameData);

                var form = new TmdbBrowserForm(/*!FirstTmdb*/false, false, false, defaultID);
                //FirstTmdb = false;
                if (form.ShowDialog() == DialogResult.OK)
                {
                    var ID = form.ID;
                    if (ID != null)
                    {
                        this.SeasonTextBox.Text = ID.Season?.ToString() ?? "";
                        this.EpisodeTextBox.Text = ID.Episode?.ToString() ?? "";
                    }
                }
            }
        }

        private void ExtraNameTextBox_TextChanged(object sender, EventArgs e) {
            if (ExtraNameTextBox.Enabled && (Utils.IsValidFileName(this.ExtraNameTextBox.Text) != null))
            {
                this.ExtraNameTextBox.BackColor = Color.LightCoral;
            } else
            {
                this.ExtraNameTextBox.BackColor = SystemColors.Window;
            }
        }

        private static string GetMultiversionFileName(string fileName, ScaleResolution? resolution) {
            return $"{Path.GetFileNameWithoutExtension(fileName)} - {GetMultiversionString(resolution)}{Path.GetExtension(fileName)}";
        }

        private static string GetMultiversionString(ScaleResolution? resolution) {
            switch (resolution)
            {
                case null: return "Original";
                case ScaleResolution.UHD_7680x4320: return "8k";
                case ScaleResolution.UHD_3840x2160: return "4k";
                case ScaleResolution.HD_1920x1080: return "1080p";
                case ScaleResolution.HD_1280x720: return "720p";
                case ScaleResolution.SD_720x480: return "480p";
                default:
                    throw new ArgumentException("Unknown resolution enum.");
            }
        }

        private void EpisodeRangeCheckBox_CheckedChanged(object sender, EventArgs e) {
            EpisodeRangeTextBox.Enabled = EpisodeRangeCheckBox.Checked;
            EpisodeRangeLabel.Enabled = EpisodeRangeCheckBox.Checked;
        }
    }
}
