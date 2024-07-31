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
        LibVLC vlc;
        VideoRenamerStateMachine state;
        Title? currentTitle;
        const string TimeSpanFormat = "hh':'mm':'ss";

        public FileRenamer(Disc data, string folder, bool ignoreIncompleteVideos) {
            this.disc = data;
            this.folder = folder;
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
                loadVideo(this.VideoViewVLC1, Path.Combine(this.folder, currentTitle.OutputFileName));
                PlayBtn_Click(null, null);
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
                newTitle = state.ApplyChoice(this.NameTextBox.Text, this.BonusFeatureRadioBtn.Checked, this.DeleteEpisodesCheckBox.Checked);
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
                if (((NamedTitle)item).Name == name)
                {
                    return false;
                }
            }
            return true;
        }

        private bool isValidFileName(string name) {
            char[] fileChars = Path.GetInvalidFileNameChars();
            char[] pathChars = Path.GetInvalidPathChars();
            foreach(char c in name)
            {
                if (fileChars.Contains(c) || pathChars.Contains(c))
                {
                    return false;
                }
            }
            return true;
        }

        private void RenameFiles() {
            if (state.BonusFeatures.Count > 0)
            {
                Directory.CreateDirectory(Path.Combine(this.folder, "Bonus Features"));
            }

            List<string> failedFiles = new();
            foreach (Title title in disc.Titles)
            {
                try
                {
                    if (state.RenamedTitles.Contains(title))
                    {
                        Console.WriteLine($"Renamed {title.SimplifiedFileName} => {title.UserName}.mkv");
                        File.Move(Path.Combine(this.folder, title.OutputFileName), Path.Combine(this.folder, title.UserName + ".mkv"));
                    } else if (state.BonusFeatures.Contains(title)) {
                        Console.WriteLine($"Renamed {title.SimplifiedFileName} => Bonus Features/{title.UserName}.mkv");
                        File.Move(Path.Combine(this.folder, title.OutputFileName), Path.Combine(this.folder, "Bonus Features", title.UserName + ".mkv"));
                    } else if (state.DeletedTitles.Contains(title))
                    {
                        Console.WriteLine($"Deleted {title.SimplifiedFileName}");
                        File.Delete(Path.Combine(this.folder, title.OutputFileName));
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
        }
    }
}
