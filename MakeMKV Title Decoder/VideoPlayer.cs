using LibVLCSharp.Shared;
using LibVLCSharp.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MakeMKV_Title_Decoder {
    public partial class VideoPlayer : UserControl {

        const string TimeSpanFormat = "hh':'mm':'ss";

        public string? LoadedVideoPath { get; private set; }

        private LibVLC? vlc = null;
        public LibVLC? VLC {
            get => vlc;
            set
            {
                LoadVideo(null);
                vlc = value;
                LoadVideo(LoadedVideoPath);
            }
        }

        private VideoView? Viewer = null;
        public VideoView? VlcViewer {
            get => Viewer;
            set
            {
                LoadVideo(null);
                Viewer = value;
                LoadVideo(LoadedVideoPath);
            }
        }

        private VideoPlayer? SyncPlayer = null;
        private bool IsPrimarySync = false;
        public VideoPlayer? Sync {
            get => SyncPlayer;
            set
            {
                if (SyncPlayer != null)
                {
                    SyncPlayer.SyncPlayer = null;
                    SyncPlayer.LoadVideo(null);
                }
                this.SyncPlayer = value;
                this.LoadVideo(null);
                if (value != null)
                {
                    value.SyncPlayer = this;
                    this.IsPrimarySync = true;
                    value.IsPrimarySync = false;
                }
            }
        }

        private bool IsSyncMaster => (this.SyncPlayer != null) && IsPrimarySync;
        private bool IsSyncSlave => (this.SyncPlayer != null) && !IsPrimarySync;

        public VideoPlayer() {
            InitializeComponent();
        }

        // TODO
        /*public VideoPlayer(VideoPlayer sync) {

        }*/

        private void VideoPlayer_Load(object sender, EventArgs e) {

        }

        public void LoadVLC() {
            this.VLC = new LibVLC(enableDebugLogs: false, "--aout=directsound", "--quiet");
        }

        public void LoadVideo(string? path) {
            timer1.Stop();
            if (Viewer != null && Viewer.MediaPlayer != null)
            {
                var mp = Viewer.MediaPlayer;
                mp.Stop();
                Viewer.MediaPlayer = null;
                mp.Dispose();
            }

            bool en = (path != null) && (Viewer != null) && (vlc != null);
            this.VideoScrubTrackBar.Enabled = en;
            this.PlayBtn.Enabled = en;
            this.PauseBtn.Enabled = en;

            this.VideoScrubTrackBar.Value = 0;
            CurrentTimeLabel.Text = new TimeSpan().ToString(TimeSpanFormat);
            TotalTimeLabel.Text = new TimeSpan().ToString(TimeSpanFormat);

            LoadedVideoPath = path;
            if ((path != null) && (Viewer != null) && (vlc != null))
            {
                Media media = new Media(vlc, Path.GetFullPath(path));
                var player = new MediaPlayer(media);
                Viewer.MediaPlayer = player;
                player.Volume = this.VolumeTrackBar.Value;
                if (IsSyncMaster)
                {
                    timer1.Start();
                }
            } else if (path != null)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                if (vlc == null)
                {
                    Console.WriteLine("Tried to load video without first loading VLC.");
                }
                if (Viewer == null)
                {
                    Console.WriteLine("Tried to load video without first setting the viewer.");
                }
                Console.ResetColor();
            }

            this.FileNotFoundLabel.Visible = (path == null) || (!File.Exists(path));
        }

        private void PlayBtn_Click(object sender, EventArgs e) {
            Viewer?.MediaPlayer?.Play();
            this.SyncPlayer?.Viewer?.MediaPlayer?.Play();
        }

        private void PauseBtn_Click(object sender, EventArgs e) {
            Viewer?.MediaPlayer?.Pause();
            this.SyncPlayer?.Viewer?.MediaPlayer?.Pause();
        }

        private void ReloadBtn_Click(object sender, EventArgs e) {
            string? path = this.LoadedVideoPath;
            string? syncPath = this.SyncPlayer?.LoadedVideoPath;

            LoadVideo(null);
            this.SyncPlayer?.LoadVideo(null);

            if (path != null)
            {
                LoadVideo(path);
            }
            if (this.SyncPlayer != null && syncPath != null)
            {
                this.SyncPlayer.LoadVideo(syncPath);
            }

            PlayBtn_Click(sender, e);
        }

        private void VideoScrubTrackBar_Scroll(object sender, EventArgs e) {
            double percent = (float)VideoScrubTrackBar.Value / VideoScrubTrackBar.Maximum;
            var player = Viewer?.MediaPlayer;
            if (player != null)
            {
                double milliseconds = player.Length * percent;
                ScrollToTimestamp(milliseconds);
                if (this.SyncPlayer != null)
                {
                    this.SyncPlayer.ScrollToTimestamp(milliseconds);
                }
            }
        }

        private void VolumeTrackBar_Scroll(object sender, EventArgs e) {
            int value = VolumeTrackBar.Value;
            var player = Viewer?.MediaPlayer;
            if (player != null)
            {
                player.Volume = value;
            }
        }
        private void timer1_Tick(object sender, EventArgs e) {
            bool captured = VideoScrubTrackBar.Capture;
            if (IsSyncMaster)
            {
                captured |= this.SyncPlayer.VideoScrubTrackBar.Capture;
            }

            if (!captured)
            {
                this.VideoScrubTrackBar.Invoke(() =>
                {
                    UpdateTimeLabels(true);
                    if (IsSyncMaster)
                    {
                        this.SyncPlayer.UpdateTimeLabels(true);
                    }
                });
            }
        }

        private void ScrollToTimestamp(double milliseconds) {
            if (Viewer != null && Viewer.MediaPlayer != null)
            {
                double percent = milliseconds / Viewer.MediaPlayer.Length;
                percent = Math.Clamp(percent, 0.0, 1.0);
                Viewer.MediaPlayer.Position = (float)percent;
                UpdateTimeLabels();
            }
        }

        private void UpdateTimeLabels(bool UpdateScrubBar = false) {
            if (this.Viewer != null && this.Viewer.MediaPlayer != null)
            {
                MediaPlayer player = this.Viewer.MediaPlayer;
                float pos = Math.Clamp(player.Position, 0f, 1f);

                if (UpdateScrubBar)
                {
                    this.VideoScrubTrackBar.Value = (int)(pos * this.VideoScrubTrackBar.Maximum);
                }
                this.CurrentTimeLabel.Text = TimeSpan.FromMilliseconds(pos * player.Length).ToString(TimeSpanFormat);
                this.TotalTimeLabel.Text = TimeSpan.FromMilliseconds(player.Length).ToString(TimeSpanFormat);
            }
        }
    }
}
