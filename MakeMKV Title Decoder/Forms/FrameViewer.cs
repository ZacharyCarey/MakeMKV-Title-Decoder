﻿using FFMpeg_Wrapper;
using FFMpeg_Wrapper.ffmpeg;
using FFMpeg_Wrapper.ffprobe;
using MakeMKV_Title_Decoder.Data;
using MakeMKV_Title_Decoder.Data.Renames;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;

namespace MakeMKV_Title_Decoder.Forms
{
    public partial class FrameViewer : Form {
        LoadedStream loadedStream;
        LoadedDisc disc;
        uint nFrames;
        string TempFolder = null;
        List<string> FramesPaths = new();
        long currentFrame = 0;

        public FrameViewer(LoadedDisc disc, LoadedStream inputFile) {
            this.disc = disc;
            this.loadedStream = inputFile;
            InitializeComponent();
        }

        private void LoadImage(long index) {
            this.FramePictureBox.ImageLocation = this.FramesPaths[(int)index];
            this.FrameLabel.Text = $"Frame {(index + 1):#,#}/{this.FramesPaths.Count:#,#}";
        }

        private void FrameViewer_Load(object sender, EventArgs e) {
            string filePath = loadedStream.GetFullPath(disc);
            DataSize? fileSize = DataSize.FromFile(filePath);
            if (fileSize == null)
            {
                MessageBox.Show("Failed to find file.");
                this.Close();
                return;
            }
            if (fileSize >= new DataSize(10, Unit.Mega))
            {
                var user = MessageBox.Show($"The file size is {fileSize}, this may take a while to process. Continue?", "Continue?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (user != DialogResult.Yes)
                {
                    this.Close();
                    return;
                }
            }

            string? ffprobePath = FileUtils.GetFFProbeExe();
            string? ffmpegPath = FileUtils.GetFFMpegExe();
            if (ffprobePath == null || ffmpegPath == null)
            {
                MessageBox.Show("Failed to find ffprobe/ffmpeg.");
                this.Close();
                return;
            }

            FFProbe ffprobe = new FFProbe(ffprobePath);
            FFMpeg ffmpeg = new FFMpeg(ffmpegPath);

            uint? frames = ffprobe.GetNumberOfFrames(filePath, true);
            if (frames == null || frames < 0)
            {
                nFrames = 0;
            } else
            {
                nFrames = frames.Value;
            }
            Console.WriteLine($"Detected {nFrames} frames");

            if (nFrames >= 30)
            {
                var result = MessageBox.Show($"There are {nFrames} frames. Are you sure", "Extract frames?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (result != DialogResult.Yes)
                {
                    this.Close();
                    return;
                }
            }

            try
            {
                DirectoryInfo di = Directory.CreateTempSubdirectory();
                if (!di.Exists) throw new DirectoryNotFoundException();
                this.TempFolder = di.FullName;
            } catch (Exception ex)
            {
                MessageBox.Show("Failed to open temp folder: " + ex.Message);
                return;
            }

            long failedFrame = TaskProgressViewerForm.Run<SimpleProgress, long>((IProgress<SimpleProgress> progress) =>
            {
                for (uint i = 0; i < nFrames; i++)
                {
                    string outputFile = Path.Combine(this.TempFolder, $"Frame_{i}.png");
                    string? error = ffmpeg.Snapshot(filePath, i, outputFile).Run();
                    if (error != null)
                    {
                        Log.Error($"FFMpeg snapshot: {error}");
                        return i;
                    }
                    this.FramesPaths.Add(outputFile);
                    progress.Report(new SimpleProgress(i, nFrames));
                }

                return -1;
            });

            if (failedFrame >= 0)
            {
                MessageBox.Show($"Failed to extract Frame #{failedFrame}");
                this.Close();
                return;
            }

            if (this.FramesPaths.Count == 0)
            {
                currentFrame = -1;
            } else
            {
                currentFrame = 0;
                LoadImage(currentFrame);
            }
        }

        private void PrevBtn_Click(object sender, EventArgs e) {
            if (this.currentFrame >= 0)
            {
                currentFrame--;
                if (currentFrame < 0) currentFrame = 0;
                LoadImage(currentFrame);
            }
        }

        private void NextBtn_Click(object sender, EventArgs e) {
            if (this.currentFrame >= 0)
            {
                currentFrame++;
                if (currentFrame >= this.FramesPaths.Count) currentFrame = this.FramesPaths.Count - 1;
                LoadImage(currentFrame);
            }
        }

        private void FrameViewer_FormClosing(object sender, FormClosingEventArgs e) {
            if (TempFolder != null)
            {
                try
                {
                    Directory.Delete(TempFolder, true);
                } catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Failed to delete temp folder: " + ex.Message);
                    Console.ResetColor();
                }
            }
        }

        private void ExportSingleBtn_Click(object sender, EventArgs e) {
            if (this.currentFrame >= 0)
            {
                Attachment? attachment = SingleFrameExtracted.Extract(this.disc, this.loadedStream, (uint)this.currentFrame);
                if (attachment == null)
                {
                    MessageBox.Show("Attachment already exists or failed to extract image.");
                    return;
                } else
                {
                    MessageBox.Show("Successfully extracted image.");
                    this.disc.RenameData.Attachments.Add(attachment);
                }
            }
        }

        private void ExportAllBtn_Click(object sender, EventArgs e) {
            DialogResult result = MessageBox.Show("Are you sure you want to extract ALL frames?", "Extract All?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                bool any = false;
                for (long i = 0; i < this.FramesPaths.Count; i++)
                {
                    Attachment? attachment = SingleFrameExtracted.Extract(this.disc, this.loadedStream, (uint)i);
                    if (attachment != null)
                    {
                        this.disc.RenameData.Attachments.Add(attachment);
                        any = true;
                    }
                }
                if (any)
                {
                    MessageBox.Show("Successfully extracted images.");
                } else
                {
                    MessageBox.Show("Attachments already exists or failed to extract images.");
                }
            }
        }

        protected override bool ProcessDialogKey(Keys keyData) {
            if (Form.ModifierKeys == Keys.None && keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            return base.ProcessDialogKey(keyData);
        }
    }
}
