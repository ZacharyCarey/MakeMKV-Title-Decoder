using JsonSerializable;
using LibVLCSharp.Shared;
using MakeMKV_Title_Decoder.Data;
using MakeMKV_Title_Decoder.MakeMKV;
using System;
using System.IO;
using System.Linq;
using System.Media;
using System.Reflection;

namespace MakeMKV_Title_Decoder {
    public partial class Form1 : Form, IJsonSerializable {
        string? _outputFolder = null;
        string? OutputFolder {
            get => _outputFolder;
            set
            {
                _outputFolder = value;
                this.LoadedFileLabel.Text = value ?? "N/A";
            }
        }

        MakeMkvInterface? MakeMkv = null;

        const string HappySound = "Success.wav";
        const string SadSound = "Error.wav";

        List<DiscDrive> discDrives = new();

        DiscDrive? _selectedDrive = null;
        DiscDrive? selectedDrive {
            get => _selectedDrive;
            set
            {
                _selectedDrive = value;
                this.DriveBtnPanel.Enabled = (_selectedDrive != null);
            }
        }

        Disc? _loadedDisc = null;
        Disc? loadedDisc {
            get => _loadedDisc;
            set
            {
                _loadedDisc = value;
                this.RenameVideosBtn.Enabled = (_loadedDisc != null);
            }
        }

        public Form1() {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e) {
            ConsoleLog.CreateLogger("Log.txt");
        }

        private void PlaySound(string name) {
            var assembly = Assembly.GetExecutingAssembly();

            /*foreach(string str in assembly.GetManifestResourceNames())
            {
                Console.WriteLine(str);
            }*/
            string file = "MakeMKV_Title_Decoder." + name;
            using (Stream stream = assembly.GetManifestResourceStream(file))
            {
                SoundPlayer sound = new SoundPlayer(stream);
                sound.Play();
            }
        }

        public JsonData SaveToJson() {
            JsonObject data = new();
            data["Output Folder"] = new JsonString(this.OutputFolder);
            data["Disc"] = this.loadedDisc?.SaveToJson() ?? null;
            return data;
        }

        public void LoadFromJson(JsonData data) {
            JsonObject obj = (JsonObject)data;

            JsonData discData = obj["Disc"];
            if (discData == null)
            {
                this.loadedDisc = null;
            } else
            {
                this.loadedDisc = new();
                this.loadedDisc.LoadFromJson(discData);
            }

            // Save output folder for later
            this.OutputFolder = (JsonString)obj["Output Folder"];
            Console.WriteLine($"Found output path: {this.OutputFolder ?? "null"}");
        }

        private bool getMakeMkv(out MakeMkvInterface mkv) {
            if (MakeMkv == null)
            {
                MakeMkv = MakeMkvInterface.FindMakeMkvProcess();
                if (MakeMkv == null)
                {
                    PlaySound(SadSound);
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

        private void RefreshDrivesBtn_Click(object sender, EventArgs e) {
            this.DriveSelectionPanel.Enabled = false;
            MakeMkvInterface mkv;
            if (!getMakeMkv(out mkv))
            {
                return;
            }

            List<DiscDrive>? drives = mkv.ReadDrives();
            if (drives == null)
            {
                PlaySound(SadSound);
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
            this.DriveSelectionPanel.Enabled = true;
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
            this.DriveSelectionPanel.Enabled = false;
            MakeMkvInterface mkv;
            if (!getMakeMkv(out mkv))
            {
                this.DriveSelectionPanel.Enabled = true;
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
                    this.DriveSelectionPanel.Enabled = true;
                    return;
                }
            }
            this.OutputFolder = outputPath;

            UpdateProgressBar(new MakeMkvProgress());

            // Blocking, but will keep application running while in progress
            IProgress<MakeMkvProgress> progress = new Progress<MakeMkvProgress>(UpdateProgressBar);
            bool success = mkv.BackupDisc(this.DrivesComboBox.SelectedIndex, outputPath, progress);

            if (!success)
            {
                PlaySound(SadSound);
                MessageBox.Show("There was an error reading the disc.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DriveSelectionPanel.Enabled = true;
                return;
            }
            PlaySound(HappySound);

            UpdateProgressBar(MakeMkvProgress.Max);

            this.DriveSelectionPanel.Enabled = true;
        }

        private void UpdateProgressBar(MakeMkvProgress progress) {
            CurrentProgressBar.Value = progress.Current;
            TotalProgressBar.Value = progress.Total;
        }

        private void ReadBtn_Click(object sender, EventArgs e) {
            this.DriveSelectionPanel.Enabled = false;
            MakeMkvInterface mkv;
            if (!getMakeMkv(out mkv))
            {
                this.DriveSelectionPanel.Enabled = true;
                return;
            }

            UpdateProgressBar(new MakeMkvProgress());

            // Blocking, but will keep application running while in progress
            IProgress<MakeMkvProgress> progress = new Progress<MakeMkvProgress>(UpdateProgressBar);
            this.loadedDisc = mkv.ReadDisc(this.DrivesComboBox.SelectedIndex, progress);

            UpdateProgressBar(MakeMkvProgress.Max);

            if (this.loadedDisc == null)
            {
                PlaySound(SadSound);
                MessageBox.Show("There was an error reading the disc.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DriveSelectionPanel.Enabled = true;
                return;
            }
            PlaySound(HappySound);

            Console.WriteLine("All data received:");
            Console.WriteLine(this.loadedDisc);

            this.DriveSelectionPanel.Enabled = true;
        }

        private void discToolStripMenuItem1_Click(object sender, EventArgs e) {

        }

        private void saveDiscToolStripMenuItem_Click(object sender, EventArgs e) {
            using (SaveFileDialog dialog = new())
            {
                dialog.Filter = "JSON files (*.json)|*.json";
                dialog.RestoreDirectory = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    var path = dialog.FileName;
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                    try
                    {
                        Json.Write(this, path);
                        //this.OutputFolder = Path.GetDirectoryName(path);
                        Console.WriteLine("Saved JSON file.");
                    } catch (Exception ex)
                    {
                        PlaySound(SadSound);
                        MessageBox.Show("Failed to save JSON: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void loadDiscToolStripMenuItem_Click(object sender, EventArgs e) {
            using (OpenFileDialog dialog = new())
            {
                dialog.Filter = "JSON files (*.json)|*.json";
                dialog.RestoreDirectory = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    using (var stream = dialog.OpenFile())
                    {
                        try
                        {
                            Json.Read(stream, this);
                            if (this.OutputFolder == null)
                            {
                                this.OutputFolder = Path.GetDirectoryName(Path.GetFullPath(dialog.FileName));
                            }
                            Console.WriteLine("Loaded JSON file.");
                        } catch (Exception ex)
                        {
                            PlaySound(SadSound);
                            MessageBox.Show("Failed to read file: " + ex.Message, "Failed to read file.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }
            }
        }

        private void RenameVideosBtn_Click(object sender, EventArgs e) {
            if (OutputFolder == null)
            {
                SelectFolderBtn_Click(null, null);
                if (OutputFolder == null)
                {
                    return;
                }
            }

            // Sanity check
            if (!Directory.Exists(OutputFolder))
            {
                PlaySound(SadSound);
                MessageBox.Show("Failed to find directory: " + OutputFolder, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (this.loadedDisc == null)
            {
                PlaySound(SadSound);
                MessageBox.Show("No disc is currently loaded.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            FileRenamer form = new(this.loadedDisc, OutputFolder, IgnoreIncompleteCheckBox.Checked);
            form.Show();
        }

        private void SelectFolderBtn_Click(object sender, EventArgs e) {
            using (FolderBrowserDialog openFileDialog = new FolderBrowserDialog())
            {
                if (OutputFolder != null)
                {
                    Console.WriteLine($"Using initial path: {OutputFolder}");
                    openFileDialog.InitialDirectory = OutputFolder;
                }

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    OutputFolder = openFileDialog.SelectedPath;
                }
            }
        }
    }
}
