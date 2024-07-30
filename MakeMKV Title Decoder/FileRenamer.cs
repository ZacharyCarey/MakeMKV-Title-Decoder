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

namespace MakeMKV_Title_Decoder {
    public partial class FileRenamer : Form {
        Disc disc;
        string folder;
        LibVLC vlc;
        int videoIndex = -1;

        public FileRenamer(Disc data, string folder) {
            this.disc = data;
            this.folder = folder;
            vlc = new LibVLC(enableDebugLogs: false);
            InitializeComponent();
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

        private void NextBtn_Click(object sender, EventArgs e) {
            loadVideo(this.VideoViewVLC1, null);
            if (this.videoIndex < disc.Titles.Count) this.videoIndex++;
            if (this.videoIndex < disc.Titles.Count)
            {
                this.titleInfo1.LoadTitle(disc.Titles[this.videoIndex]);
                string? file = this.disc.Titles[this.videoIndex].OutputFileName;
                if (file != null)
                {
                    loadVideo(this.VideoViewVLC1, Path.Combine(this.folder, file));
                    PlayBtn_Click(null, null);
                }
            }
        }

        private void VideoSrubTrackBar_Scroll(object sender, EventArgs e) {
            Console.WriteLine("Scroll");
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
            if (this.videoIndex < disc.Titles.Count)
            {
                this.titleInfo1.LoadTitle(disc.Titles[this.videoIndex]);
                string? file = this.disc.Titles[this.videoIndex].OutputFileName;
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

    }
}
