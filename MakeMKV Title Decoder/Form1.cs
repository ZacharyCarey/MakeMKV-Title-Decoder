using JsonSerializable;
using libbluray.disc;
using LibVLCSharp.Shared;
using MakeMKV_Title_Decoder.Data;
using MakeMKV_Title_Decoder.Forms.DiscBackup;
using MakeMKV_Title_Decoder.Forms.FileRenamer;
using MakeMKV_Title_Decoder.Forms.TmdbBrowser;
using MakeMKV_Title_Decoder.libs.MakeMKV;
using MakeMKV_Title_Decoder.libs.MakeMKV.Data;
using MakeMKV_Title_Decoder.libs.MkvToolNix;
using MakeMKV_Title_Decoder.libs.MkvToolNix.Data;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MakeMKV_Title_Decoder
{
    public partial class Form1 : Form {

        MkvToolNixDisc? disc = null;
        RenameData renames = new();

        public Form1() {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e) {
            ConsoleLog.CreateLogger("Log.txt");
        }

        private void testToolStripMenuItem_Click(object sender, EventArgs e) {

            using (FolderBrowserDialog openFileDialog = new FolderBrowserDialog())
            {
                openFileDialog.InitialDirectory = "F:\\Video\\backup";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var playlist = new Playlist {
                        Title = "RWBY Test",
                        Files = new() {
                            new PlaylistFile() {
                                Source = "BDMV\\STREAM\\00004.m2ts",
                                Tracks = new() {
                                    new PlaylistTrack() {
                                        ID = 0,
                                        Copy = true,
                                        Sync = new(){ },
                                        Name = "Wassup",
                                        Commentary = false
                                    },
                                    new PlaylistTrack() {
                                        ID = 1,
                                        Copy = true,
                                        Sync = new(){ },
                                        Name = "IM IMMUNE",
                                        Commentary = false
                                    },
                                    new PlaylistTrack() {
                                        ID = 2,
                                        Copy = true,
                                        Sync = new(){ },
                                        Commentary = false
                                    },
                                    new PlaylistTrack() {
                                        ID = 3,
                                        Copy = false,
                                        Sync = new(){ },
                                        Commentary = false
                                    }
                                }
                            },
                            new PlaylistFile() {
                                Source = "BDMV\\STREAM\\00015.m2ts",
                                Tracks = new() {
                                    new PlaylistTrack() {
                                        ID = 0,
                                        Copy = true,
                                        Sync = new(){ },
                                        Commentary = false,
                                        AppendedTo = new() {
                                            FileIndex = 0,
                                            TrackIndex = 0
                                        }
                                    },
                                    new PlaylistTrack() {
                                        ID = 1,
                                        Copy = true,
                                        Sync = new(){ },
                                        Commentary = false,
                                        AppendedTo = new() {
                                            FileIndex = 0,
                                            TrackIndex = 1
                                        }
                                    },
                                    new PlaylistTrack() {
                                        ID = 2,
                                        Copy = false,
                                        Sync = new(){ },
                                        Commentary = false
                                    },
                                    new PlaylistTrack() {
                                        ID = 3,
                                        Copy = false,
                                        Sync = new(){ },
                                        Commentary = false
                                    }
                                },
                            },
                            new PlaylistFile() {
                                Source = "BDMV\\STREAM\\00004.m2ts",
                                Tracks = new() {
                                    new PlaylistTrack() {
                                        ID = 0,
                                        Copy = true,
                                        Sync = new(){ },
                                        Commentary = false,
                                        AppendedTo = new() {
                                            FileIndex = 1,
                                            TrackIndex = 0
                                        }
                                    },
                                    new PlaylistTrack() {
                                        ID = 1,
                                        Copy = true,
                                        Sync = new(){ },
                                        Commentary = false,
                                        AppendedTo = new() {
                                            FileIndex = 1,
                                            TrackIndex = 1
                                        }
                                    },
                                    new PlaylistTrack() {
                                        ID = 2,
                                        Copy = true,
                                        Sync = new(){
                                            new TrackID() {
                                                FileIndex = 1,
                                                TrackIndex = 2
                                            }
                                        },
                                        Commentary = false,
                                        AppendedTo = new() {
                                            FileIndex = 0,
                                            TrackIndex = 2
                                        }
                                    },
                                    new PlaylistTrack() {
                                        ID = 3,
                                        Copy = false,
                                        Sync = new(){ },
                                        Commentary = false
                                    }
                                }
                            }
                        }
                    };

                    MkvToolNixInterface.MergeAsync(this.disc, playlist, Path.Combine(openFileDialog.SelectedPath, "Test.mkv"));
                }
            }
        }

        private void viewInfoToolStripMenuItem_Click(object sender, EventArgs e) {
            using (FolderBrowserDialog openFileDialog = new FolderBrowserDialog())
            {
                openFileDialog.InitialDirectory = "F:\\Video\\backup";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    MkvToolNixDisc? disc = null;
                    try
                    {
                        disc = MkvToolNixDisc.OpenAsync(openFileDialog.SelectedPath);
                    } catch (Exception ex)
                    {
                        MessageBox.Show($"There was an error reading the disc.: {ex.Message}", "Failed to read MkvToolNix", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (disc == null)
                    {
                        MessageBox.Show($"Failed to parse disc data.", "Failed to read MkvToolNix", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    new ViewInfoForm(disc).ShowDialog();
                }
            }
        }

        private void loadDiscToolStripMenuItem1_Click(object sender, EventArgs e) {
            using (FolderBrowserDialog openFileDialog = new FolderBrowserDialog())
            {
                openFileDialog.InitialDirectory = "F:\\Video\\backup";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {

                    try
                    {
                        this.disc = MkvToolNixDisc.OpenAsync(openFileDialog.SelectedPath);
                        this.renames = new();
                    } catch (Exception ex)
                    {
                        MessageBox.Show($"There was an error reading the disc.: {ex.Message}", "Failed to read MkvToolNix", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (disc == null)
                    {
                        MessageBox.Show($"Failed to parse disc data.", "Failed to read MkvToolNix", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e) {
            using (SaveFileDialog dialog = new())
            {
                dialog.Filter = "JSON files (*.json)|*.json";
                dialog.RestoreDirectory = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    var path = dialog.FileName;
                    try
                    {
                        File.Delete(path);
                    } catch (Exception) { }

                    try
                    {
                        Json.Write(this.renames, path);
                        Console.WriteLine("Saved JSON file.");
                    } catch (Exception ex)
                    {
                        MessageBox.Show("Failed to save JSON: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void loadToolStripMenuItem1_Click(object sender, EventArgs e) {
            using (OpenFileDialog dialog = new())
            {
                dialog.Filter = "JSON files (*.json)|*.json";
                dialog.RestoreDirectory = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    using (var stream = dialog.OpenFile())
                    {
                        RenameData data = new();

                        try
                        {
                            Json.Read(stream, data);
                            Console.WriteLine("Loaded JSON file.");
                            this.renames = data;
                        } catch (Exception ex)
                        {
                            MessageBox.Show("Failed to read file: " + ex.Message, "Failed to read file.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }
            }
        }

        private void clipRenamerToolStripMenuItem1_Click(object sender, EventArgs e) {
            if (disc == null)
            {
                MessageBox.Show("Please load a disc first.");
                return;
            }
            new ClipRenamerForm(renames, disc).ShowDialog();
        }

        private void playlistCreatorToolStripMenuItem_Click(object sender, EventArgs e) {
            if (this.disc != null)
            {
                new PlaylistCreatorForm(this.disc, this.renames).ShowDialog();
            }
        }

        private void fileRenamerToolStripMenuItem_Click(object sender, EventArgs e) {
            if (this.disc != null)
            {
                new FileRenamerForm(this.disc, this.renames).ShowDialog();
            }
        }

        private void backupToolStripMenuItem_Click(object sender, EventArgs e) {
            new DiscBakupForm().Show();
        }
    }
}
