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
        SegmentIdentifier identifier;
        Queue<Title> titlesToRename = new();
        List<EpisodeSolution> episodes = new();

        public FileRenamer(Disc data, string folder) {
            this.disc = data;
            this.folder = folder;
            vlc = new LibVLC(enableDebugLogs: false);
            InitializeComponent();

            identifier = new(data);
            titlesToRename.Enqueue(identifier.FindMainFeature());
        }

        private void FileRenamer_Load(object sender, EventArgs e) {

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
            this.VideoSrubTrackBar.Enabled = en;
            this.PlayBtn.Enabled = en;
            this.PauseBtn.Enabled = en;

            this.VideoSrubTrackBar.Value = 0;

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
            }
        }

        private void ResetUserInputPanel() {
            UserInputPanel.Enabled = false;
            KeepRadioBtn.Checked = true;
            EpisodeRadioRadioBtn.Checked = true;
            DeleteThisFile.Checked = true;
            DeleteEpisodesCheckBox.Checked = false;
            NameTextBox.Text = "";
        }

        private void LoadNextTitle() {
            UserInputPanel.Enabled = true;
            this.NameTextBox.Text = titlesToRename.Peek().Name ?? "";
            episodes = identifier.FindEpisodes(titlesToRename.Peek());
        }

        private void NextBtn_Click(object sender, EventArgs e) {
            loadVideo(this.VideoViewVLC1, null);
            UserInputPanel.Enabled = false;
            if (titlesToRename.Count > 0)
            {
                HandleVideoLogic();
                titlesToRename.Dequeue();
                ResetUserInputPanel();
                if (titlesToRename.Count > 0)
                {
                    LoadNextTitle();
                    this.titleInfo1.LoadTitle(titlesToRename.Peek());
                    string? file = titlesToRename.Peek().OutputFileName;
                    if (file != null)
                    {
                        loadVideo(this.VideoViewVLC1, Path.Combine(this.folder, file));
                        PlayBtn_Click(null, null);
                    }
                } else
                {
                    // TODO rename all files
                }
            }
        }

        private void VideoSrubTrackBar_Scroll(object sender, EventArgs e) {
            //Console.WriteLine("Scroll");
            scrub((float)VideoSrubTrackBar.Value / VideoSrubTrackBar.Maximum);
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
            if (titlesToRename.Count > 0)
            {
                this.titleInfo1.LoadTitle(titlesToRename.Peek());
                string? file = titlesToRename.Peek().OutputFileName;
                if (file != null)
                {
                    loadVideo(this.VideoViewVLC1, Path.Combine(this.folder, file));
                    PlayBtn_Click(null, null);
                }
            }
        }

        private void VideoSrubTrackBar_MouseCaptureChanged(object sender, EventArgs e) {

        }

        private void timer1_Tick(object sender, EventArgs e) {
            //Console.WriteLine($"Captured: {VideoSrubTrackBar.Capture}");
            MediaPlayer? player = this.VideoViewVLC1.MediaPlayer;
            if (player != null && !VideoSrubTrackBar.Capture)
            {
                this.VideoSrubTrackBar.Invoke(() =>
                {
                    this.VideoSrubTrackBar.Value = (int)(player.Position * this.VideoSrubTrackBar.Maximum);
                });
            }
        }

        private void DeleteRadioBtn_CheckedChanged(object sender, EventArgs e) {
            this.DeleteOptionsPanel.Enabled = this.DeleteRadioBtn.Checked;
        }

        private void KeepRadioBtn_CheckedChanged(object sender, EventArgs e) {
            this.KeepOptionsPanel.Enabled = KeepRadioBtn.Checked;
        }

        private void HandleVideoLogic() {

        }

        private void HandleVideoLogicKeep() {

        }

        private void HandleVideoLogicDelete() {

        }

        private void HandleVideoLogicBreakApart() {

        }
    }
}
