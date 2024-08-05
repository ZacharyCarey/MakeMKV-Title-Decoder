using JsonSerializable;
using LibVLCSharp.Shared;
using LibVLCSharp.WinForms;
using MakeMKV_Title_Decoder.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Title = MakeMKV_Title_Decoder.Data.Title;

namespace MakeMKV_Title_Decoder {
    public partial class FileRenamer : Form {
        Disc disc;
        string folder;
        Folder bonusFolder;
        LibVLC vlc;
        VideoRenamerStateMachine state;
        Title? currentTitle;
        const string TimeSpanFormat = "hh':'mm':'ss";
        JsonData optionalMetadata;
        RenameData renameData = new();

        public FileRenamer(Disc data, string folder, bool ignoreIncompleteVideos, IJsonSerializable optionalMetadata) {
            this.disc = data;
            this.folder = folder;
            this.bonusFolder = new Folder();
            bonusFolder.Name = "Bonus Features";
            this.optionalMetadata = optionalMetadata.SaveToJson();
            vlc = new LibVLC(enableDebugLogs: false, "--aout=directsound", "--quiet");
            InitializeComponent();

            state = new(data, ignoreIncompleteVideos);
        }

        private void FileRenamer_Load(object sender, EventArgs e) {
            ResetUserInputPanel();
        }

        private void FileRenamer_FormClosing(object sender, FormClosingEventArgs e) {
            loadVideo(this.VideoViewVLC1, null);
        }

        private void loadVideo(VideoView viewer, string? path) {
            if (viewer.MediaPlayer != null)
            {
                var mp = viewer.MediaPlayer;
                mp.Stop();
                viewer.MediaPlayer = null;
                mp.Dispose();
            }

            bool en = (path != null);
            this.VideoScrubTrackBar.Enabled = en;
            this.PlayBtn.Enabled = en;
            this.PauseBtn.Enabled = en;

            this.VideoScrubTrackBar.Value = 0;
            CurrentTimeLabel.Text = new TimeSpan().ToString(TimeSpanFormat);
            TotalTimeLabel.Text = new TimeSpan().ToString(TimeSpanFormat);

            if (path != null)
            {
                Media media = new Media(vlc, Path.GetFullPath(path));
                var player = new MediaPlayer(media);
                viewer.MediaPlayer = player;
                player.Volume = this.VolumeTrackBar.Value;
            }
        }

        private void setVolume(int value) {
            var player = VideoViewVLC1.MediaPlayer;
            if (player != null)
            {
                player.Volume = value;
            }
        }

        private void scrub(float value) {
            var player = VideoViewVLC1.MediaPlayer;
            if (player != null)
            {
                player.Position = value;
                this.CurrentTimeLabel.Text = TimeSpan.FromMilliseconds(player.Position * player.Length).ToString(TimeSpanFormat);
                this.TotalTimeLabel.Text = TimeSpan.FromMilliseconds(player.Length).ToString(TimeSpanFormat);
            }
        }

        private void ResetUserInputPanel() {
            UserInputPanel.Enabled = false;
            KeepRadioBtn.Checked = true;
            EpisodeRadioRadioBtn.Checked = true;
            DeleteThisFile.Checked = true;
            DeleteEpisodesCheckBox.Checked = false;
            this.UniqueNameLabel.Visible = false;
            this.InvalidNameLabel.Visible = false;
            NameTextBox.Text = "";
            this.EpisodeComboBox.SelectedIndex = -1;
            this.FolderLabel.Text = bonusFolder.ToString();
            this.FileNotFoundLabel.Visible = false;
        }

        private void NextBtn_Click(object sender, EventArgs e) {
            loadVideo(this.VideoViewVLC1, null);
            UserInputPanel.Enabled = false;
            bool success = submitUserChoice();
            if (!success)
            {
                UserInputPanel.Enabled = true;
                return;
            }
            ResetUserInputPanel();

            currentTitle = state.NextTitle();
            this.titleInfo1.LoadTitle(currentTitle);
            this.NameTextBox.Text = "";
            UserInputPanel.Enabled = (currentTitle != null);

            this.BreakApartRadioBtn.Enabled = state.HasEpisodes;
            this.DeleteEpisodesCheckBox.Enabled = state.HasEpisodes;
            this.DeleteAllChapters.Enabled = state.HasEpisodes;

            if (currentTitle?.OutputFileName != null)
            {
                string fullPath = Path.Combine(this.folder, currentTitle.OutputFileName);
                loadVideo(this.VideoViewVLC1, fullPath);
                PlayBtn_Click(null, null);

                if (!File.Exists(currentTitle.OutputFileName))
                {
                    this.FileNotFoundLabel.Text = $"Could not find file: {fullPath}";
                    this.FileNotFoundLabel.Visible = true;
                }
            }

            if (currentTitle == null)
            {
                RenameFiles();
                this.Close();
            }
        }

        private void VideoSrubTrackBar_Scroll(object sender, EventArgs e) {
            //Console.WriteLine("Scroll");
            scrub((float)VideoScrubTrackBar.Value / VideoScrubTrackBar.Maximum);
        }

        private void VolumeTrackBar_Scroll(object sender, EventArgs e) {
            setVolume(VolumeTrackBar.Value);
        }

        private void PlayBtn_Click(object sender, EventArgs e) {
            VideoViewVLC1.MediaPlayer?.Play();
            //VideoViewVLC2.MediaPlayer?.Play();
        }

        private void PauseBtn_Click(object sender, EventArgs e) {
            VideoViewVLC1.MediaPlayer?.Pause();
            //VideoViewVLC2.MediaPlayer?.Pause();
        }

        private void ReloadBtn_Click(object sender, EventArgs e) {
            loadVideo(this.VideoViewVLC1, null);
            if (this.currentTitle?.OutputFileName != null)
            {
                this.titleInfo1.LoadTitle(this.currentTitle);
                loadVideo(this.VideoViewVLC1, Path.Combine(this.folder, this.currentTitle.OutputFileName));
                PlayBtn_Click(null, null);
            }
        }

        private void VideoSrubTrackBar_MouseCaptureChanged(object sender, EventArgs e) {

        }

        private void timer1_Tick(object sender, EventArgs e) {
            //Console.WriteLine($"Captured: {VideoSrubTrackBar.Capture}");
            MediaPlayer? player = this.VideoViewVLC1.MediaPlayer;
            if (player != null && !VideoScrubTrackBar.Capture)
            {
                this.VideoScrubTrackBar.Invoke(() =>
                {
                    this.VideoScrubTrackBar.Value = (int)(player.Position * this.VideoScrubTrackBar.Maximum);
                    this.CurrentTimeLabel.Text = TimeSpan.FromMilliseconds(player.Position * player.Length).ToString(TimeSpanFormat);
                    this.TotalTimeLabel.Text = TimeSpan.FromMilliseconds(player.Length).ToString(TimeSpanFormat);
                });
            }
        }

        private void DeleteRadioBtn_CheckedChanged(object sender, EventArgs e) {
            this.DeleteOptionsPanel.Enabled = this.DeleteRadioBtn.Checked;
        }

        private void KeepRadioBtn_CheckedChanged(object sender, EventArgs e) {
            this.KeepOptionsPanel.Enabled = KeepRadioBtn.Checked;
        }

        private void SameAsRadioBtn_CheckedChanged(object sender, EventArgs e) {
            EpisodeComboBox.Enabled = SameAsRadioBtn.Checked;
        }

        private bool submitUserChoice() {
            if (currentTitle == null)
            {
                return true;
            }

            NamedTitle? newTitle;
            if (KeepRadioBtn.Checked)
            {
                if (!uniqueName(this.NameTextBox.Text))
                {
                    MessageBox.Show("Please create a unique name, or select a different option.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                } else if (!isValidFileName(this.NameTextBox.Text))
                {
                    MessageBox.Show("Invalid file name. File probably contains special characters not allowed by the OS.");
                    return false;
                }

                bool isBonus = this.BonusFeatureRadioBtn.Checked;
                if (isBonus)
                {
                    if (this.currentTitle.Folder == null)
                    {
                        this.currentTitle.Folder = bonusFolder;
                    }
                } else
                {
                    this.currentTitle.Folder = null;
                }

                newTitle = state.ApplyChoice(this.NameTextBox.Text, isBonus, this.DeleteEpisodesCheckBox.Checked);
            } else if (DeleteRadioBtn.Checked)
            {
                newTitle = state.ApplyChoice(this.DeleteAllChapters.Checked);
            } else if (BreakApartRadioBtn.Checked)
            {
                newTitle = state.ApplyChoice();
            } else /*if (SameAsRadioBtn.Checked)*/
            {
                NamedTitle? title = (NamedTitle?)EpisodeComboBox.SelectedItem;
                if (title == null)
                {
                    MessageBox.Show("Please select an episode to match to.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }
                newTitle = state.ApplyChoice(title, this.folder);
            }

            if (newTitle != null)
            {
                this.EpisodeComboBox.Items.Add(newTitle);
            }

            return true;
        }

        private void NameTextBox_TextChanged(object sender, EventArgs e) {
            this.UniqueNameLabel.Visible = !uniqueName(NameTextBox.Text);
            this.InvalidNameLabel.Visible = !isValidFileName(NameTextBox.Text);
        }

        private bool uniqueName(string name) {
            foreach (var item in this.EpisodeComboBox.Items)
            {
                if (((NamedTitle)item).Name.ToLower() == name.ToLower())
                {
                    return false;
                }
            }
            return true;
        }

        private bool isValidFileName(string name) {
            char[] fileChars = Path.GetInvalidFileNameChars();
            char[] pathChars = Path.GetInvalidPathChars();
            foreach (char c in name)
            {
                if (fileChars.Contains(c) || pathChars.Contains(c))
                {
                    return false;
                }
            }
            return true;
        }

        private void RenameFiles() {
            List<string> failedFiles = new();
            renameData = new();
            foreach (Title title in disc.Titles)
            {
                try
                {
                    if (state.RenamedTitles.Contains(title))
                    {
                        string folderName = "";
                        if (title.Folder != null)
                        {
                            if (!title.Folder.IsValidFolder)
                            {
                                title.Folder = bonusFolder;
                            }
                            folderName = title.Folder.ToString();
                        }
                        string newName = Path.Combine(folderName, title.UserName) + ".mkv";
                        Console.WriteLine($"Renamed {title.SimplifiedFileName} => {newName}");
                        renameData.Renames[title.OutputFileName] = new JsonString(newName);

                        if (!string.IsNullOrWhiteSpace(folderName))
                        {
                            folderName = Path.Combine(this.folder, folderName);
                            Directory.CreateDirectory(folderName);
                        } else
                        {
                            folderName = this.folder;
                        }

                        File.Move(Path.Combine(this.folder, title.OutputFileName), Path.Combine(folderName, title.UserName + ".mkv"));
                    } else if (state.DeletedTitles.Contains(title))
                    {
                        string fullPath = Path.Combine(this.folder, title.OutputFileName);
                        if (File.Exists(fullPath))
                        {
                            Console.WriteLine($"Deleted {title.SimplifiedFileName}");
                            File.Delete(fullPath);
                        }
                    } else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Could not find {title.SimplifiedFileName}");
                        Console.ResetColor();
                    }
                } catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Failed to rename file {title.SimplifiedFileName}: " + ex.Message);
                    Console.ResetColor();
                    failedFiles.Add(title.SimplifiedFileName);
                }
            }
            if (failedFiles.Count > 0)
            {
                MessageBox.Show($"Failed to rename files: {string.Join(", ", failedFiles)}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (MessageBox.Show("Save renamed files metadata?", "Save?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                string metadataFolder = Path.Combine(this.folder, ".metadata");
                try
                {
                    Directory.CreateDirectory(metadataFolder);
                } catch(Exception)
                {
                    MessageBox.Show("Failed to create folder.");
                    return;
                }

                try
                {
                    Json.Write(renameData, Path.Combine(metadataFolder, "FileRenames.json"));
                    Console.WriteLine("Saved 'FileRenames.json'");
                } catch (Exception ex)
                {
                    Console.WriteLine("Failed to save 'FileRenames.json'");
                    MessageBox.Show("Failed to write FileRenames.json: " + ex.Message);
                }

                try
                {
                    Json.Write(optionalMetadata, Path.Combine(metadataFolder, "DiscScrape.json"));
                    Console.WriteLine("Saved 'DiscScrape.json'");
                } catch(Exception ex)
                {
                    Console.WriteLine("Failed to save 'DiscScrape.json'");
                    MessageBox.Show("Failed to write DiscScrapes.json: " + ex.Message);
                }
            }
        }

        private void NewFolderBtn_Click(object sender, EventArgs e) {
            if (this.currentTitle == null) return;

            FolderManager folderManager = new FolderManager(bonusFolder);
            folderManager.ShowDialog();
            if (folderManager.SelectedFolder != null)
            {
                this.currentTitle.Folder = folderManager.SelectedFolder;
            }

            this.FolderLabel.Text = (this.currentTitle.Folder ?? bonusFolder).ToString();
        }

        private void BonusFeatureRadioBtn_CheckedChanged(object sender, EventArgs e) {
            bool state = BonusFeatureRadioBtn.Checked;
            FolderLabelLabel.Enabled = state;
            FolderLabel.Enabled = state;
            NewFolderBtn.Enabled = state;
        }

        private struct RenameData : IJsonSerializable {
            public const string Version = "1.0";
            public SerializableDictionary<JsonString> Renames = new();

            public RenameData() {

            }

            public JsonData SaveToJson() {
                JsonObject obj = new();

                obj["version"] = new JsonString(Version);
                obj["File Names"] = Renames.SaveToJson();

                return obj;
            }

            public void LoadFromJson(JsonData data) {
                throw new NotImplementedException();
            }
        }
    }
}
