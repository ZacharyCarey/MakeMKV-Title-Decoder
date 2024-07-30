using LibVLCSharp.Shared;
using LibVLCSharp.WinForms;
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
    public partial class VideoComparer : Form {
        LibVLC vlc;
        Title vid1;
        Title vid2;
        VideoView shorterVideo;
        VideoView longerVideo;

        public bool PreferVideo1 = false;


        public VideoComparer(NamedTitle vid1, Title vid2) {
            this.vid1 = vid1.Title;
            this.vid2 = vid2;
            vlc = new LibVLC(enableDebugLogs: false, "--aout=directsound", "--quiet");
            InitializeComponent();
            shorterVideo = this.VideoViewVLC1;
            longerVideo = this.VideoViewVLC2;
        }

        private void VideoComparer_Load(object sender, EventArgs e) {
            this.EpisodeNameLabel.Text = vid1.UserName;
            this.titleInfo1.LoadTitle(this.vid1);
            this.titleInfo2.LoadTitle(this.vid2);
        }

        private void VideoComparer_FormClosing(object sender, FormClosingEventArgs e) {
            loadVideo(this.VideoViewVLC1, null, null);
            loadVideo(this.VideoViewVLC2, null, null);
        }

        private void PlayBtn_Click(object sender, EventArgs e) {
            VideoViewVLC1.MediaPlayer?.Play();
            VideoViewVLC2.MediaPlayer?.Play();
        }

        private void PauseBtn_Click(object sender, EventArgs e) {
            VideoViewVLC1.MediaPlayer?.Pause();
            VideoViewVLC2.MediaPlayer?.Pause();
        }

        private void ReloadBtn_Click(object sender, EventArgs e) {
            loadVideo(this.VideoViewVLC1, this.VolumeTrackBar, vid1.OutputFileName);
            loadVideo(this.VideoViewVLC2, this.VolumeTrackBar2, vid2.OutputFileName);

            long length1 = VideoViewVLC1.MediaPlayer.Length;
            long length2 = VideoViewVLC2.MediaPlayer.Length;
            if (length1 >= length2)
            {
                this.longerVideo = VideoViewVLC1;
                this.shorterVideo = VideoViewVLC2;
            } else
            {
                this.longerVideo = VideoViewVLC2;
                this.shorterVideo = VideoViewVLC1;
            }

            PlayBtn_Click(null, null);
        }

        private void VideoSrubTrackBar_Scroll(object sender, EventArgs e) {
            if (shorterVideo.MediaPlayer != null && longerVideo.MediaPlayer != null)
            {
                double percent = (double)VideoScrubTrackBar.Value / VideoScrubTrackBar.Maximum;

                double longerTime = longerVideo.MediaPlayer.Length * percent;
                double shortPercent = longerTime / shorterVideo.MediaPlayer.Length;

                percent = Math.Min(Math.Max(percent, 0.0), 1.0);
                shortPercent = Math.Min(Math.Max(shortPercent, 0.0), 1.0);

                longerVideo.MediaPlayer.Position = (float)percent;
                shorterVideo.MediaPlayer.Position = (float)shortPercent;
            }
        }

        private void VolumeTrackBar_Scroll(object sender, EventArgs e) {
            setVolume(VideoViewVLC1, VolumeTrackBar.Value);
        }

        private void VolumeTrackBar2_Scroll(object sender, EventArgs e) {
            setVolume(VideoViewVLC2, VolumeTrackBar2.Value);
        }

        private void loadVideo(VideoView viewer, TrackBar volume, string? path) {
            if (viewer.MediaPlayer != null)
            {
                var mp = viewer.MediaPlayer;
                mp.Stop();
                viewer.MediaPlayer = null;
                mp.Dispose();
            }

            this.VideoScrubTrackBar.Value = 0;

            if (path != null)
            {
                Media media = new Media(vlc, Path.GetFullPath(path));
                var player = new MediaPlayer(media);
                viewer.MediaPlayer = player;
                player.Volume = volume.Value;
            }
        }

        private void setVolume(VideoView viewer, int value) {
            var player = viewer.MediaPlayer;
            if (player != null)
            {
                player.Volume = value;
            }
        }

        private void timer1_Tick(object sender, EventArgs e) {
            //MediaPlayer? player1 = this.VideoViewVLC1.MediaPlayer;
            //MediaPlayer? player2 = this.VideoViewVLC2.MediaPlayer;
            if (!VideoScrubTrackBar.Capture && longerVideo.MediaPlayer != null)
            {
                double pos = longerVideo.MediaPlayer.Position;
                this.VideoScrubTrackBar.Invoke(() =>
                {
                    this.VideoScrubTrackBar.Value = (int)(pos * this.VideoScrubTrackBar.Maximum);
                });
            }
        }

        private void LeftBtn_Click(object sender, EventArgs e) {
            PreferVideo1 = true;
            this.Close();
        }

        private void RightBtn_Click(object sender, EventArgs e) {
            PreferVideo1 = false;
            this.Close();
        }
    }
}
