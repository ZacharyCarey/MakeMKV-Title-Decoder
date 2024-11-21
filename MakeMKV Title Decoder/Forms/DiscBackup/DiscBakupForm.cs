using MakeMKV_Title_Decoder.libs.MakeMKV;
using MakeMKV_Title_Decoder.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MakeMKV_Title_Decoder.Forms.DiscBackup {
    public partial class DiscBakupForm : Form {

        MakeMkvInterface? MakeMkv = null;

        // TODO is this needed, or can we just use ComboBox.SelectedItem???
        List<DiscDrive> discDrives = new();

        DiscDrive? _selectedDrive = null;
        DiscDrive? selectedDrive {
            get => _selectedDrive;
            set
            {
                _selectedDrive = value;
                this.DownloadBtn.Enabled = (_selectedDrive != null);
            }
        }

        public DiscBakupForm() {
            InitializeComponent();
        }

        private void RefreshDrivesBtn_Click(object sender, EventArgs e) {
            this.Enabled = false;
            MakeMkvInterface mkv;
            if (!getMakeMkv(out mkv))
            {
                return;
            }

            List<DiscDrive>? drives = mkv.ReadDrives();
            if (drives == null)
            {
                SoundPlayer.Play(SoundPlayer.SadSound);
                MessageBox.Show("Failed to read drives from MakeMKV", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            this.discDrives = drives;

            DrivesComboBox.Items.Clear();
            foreach (DiscDrive drive in drives)
            {
                DrivesComboBox.Items.Add(drive);
            }
            if (drives.Count > 0)
            {
                DrivesComboBox.SelectedIndex = 0;
            }
            this.Enabled = true;
        }

        private bool getMakeMkv(out MakeMkvInterface mkv) {
            if (MakeMkv == null)
            {
                MakeMkv = MakeMkvInterface.FindMakeMkvProcess();
                if (MakeMkv == null)
                {
                    SoundPlayer.Play(SoundPlayer.SadSound);
                    MessageBox.Show("Failed to find MakeMKV.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    mkv = null;
                    return false;
                } else
                {
                    mkv = MakeMkv;
                    return true;
                }
            } else
            {
                mkv = MakeMkv;
                return true;
            }
        }

        private void DrivesComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            int index = DrivesComboBox.SelectedIndex;
            DiscDrive? drive = (DiscDrive?)DrivesComboBox.SelectedItem;
            this.selectedDrive = drive;

            string discName = "N/A";
            if (drive != null && drive.Value.HasDisc)
            {
                discName = drive.Value.DiscName;
            }
            this.DiscNameLabel.Text = discName;

            this.DownloadBtn.Enabled = (drive != null);
        }

        private void DownloadBtn_Click(object sender, EventArgs e) {
            // TODO create backup instead of ripping
            /*this.Enabled = false;
            MakeMkvInterface mkv;
            if (!getMakeMkv(out mkv))
            {
                this.Enabled = true;
                return;
            }

            string outputPath = this.OutputFolder;
            if (outputPath != null)
            {
                outputPath = Path.GetFullPath(outputPath);
            } else
            {
                outputPath = "C:\\";
            }
            using (FolderBrowserDialog openFileDialog = new FolderBrowserDialog())
            {
                Console.WriteLine($"Using initial path: {outputPath}");
                openFileDialog.InitialDirectory = outputPath;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    outputPath = openFileDialog.SelectedPath;
                } else
                {
                    this.Enabled = true;
                    return;
                }
            }
            this.OutputFolder = outputPath;

            UpdateProgressBar(new MakeMkvProgress());

            // Blocking, but will keep application running while in progress
            try
            {
                var progressForm = new TaskProgressViewer<Task, MakeMkvProgress>(
                    (IProgress<MakeMkvProgress> progress) =>
                    {
                        return mkv.BackupDiscAsync(this.DrivesComboBox.SelectedIndex, outputPath, progress);
                    }
                );
                progressForm.ShowDialog();
            } catch (Exception ex)
            {
                SoundPlayer.Play(SoundPlayer.SadSound);
                MessageBox.Show($"There was an error reading the disc.: {ex.Message}", "Failed to read MakeMKV", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Enabled = true;
                return;
            }

            SoundPlayer.Play(SoundPlayer.HappySound);

            // Try and save metadata with the rip
            string metadataFolder = Path.Combine(outputPath, ".metadata");
            try
            {
                Directory.CreateDirectory(metadataFolder);
            } catch (Exception)
            {
                MessageBox.Show("Failed to create folder.");
                this.Enabled = true;
                return;
            }

            try
            {
                Json.Write(this, Path.Combine(metadataFolder, "DiscScrape.json"));
                Console.WriteLine("Saved 'DiscScrape.json'");
            } catch (Exception ex)
            {
                Console.WriteLine("Failed to save 'DiscScrape.json'");
                MessageBox.Show("Failed to write DiscScrapes.json: " + ex.Message);
            } finally
            {
                this.Enabled = true;
            }*/
        }

        private void DiscBakupForm_Load(object sender, EventArgs e) {

        }

        private void UpdateProgressBar(MakeMkvProgress progress) {
            //CurrentProgressBar.Value = progress.Current;
            //TotalProgressBar.Value = progress.Total;
        }
    }
}
