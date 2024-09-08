using JsonSerializable;
using libbluray.disc;
using LibVLCSharp.Shared;
using MakeMKV_Title_Decoder.Data;
using MakeMKV_Title_Decoder.MakeMKV;
using MakeMKV_Title_Decoder.MakeMKV.Data;
using MakeMKV_Title_Decoder.MkvToolNix;
using MakeMKV_Title_Decoder.MkvToolNix.Data;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Reflection;

namespace MakeMKV_Title_Decoder
{
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

        MkvToolNixDisc? disc = null;
        RenameData2 renames = new();

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

        private async void DownloadBtn_Click(object sender, EventArgs e) {
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
                PlaySound(SadSound);
                MessageBox.Show($"There was an error reading the disc.: {ex.Message}", "Failed to read MakeMKV", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DriveSelectionPanel.Enabled = true;
                return;
            }

            PlaySound(HappySound);

            // Try and save metadata with the rip
            string metadataFolder = Path.Combine(outputPath, ".metadata");
            try
            {
                Directory.CreateDirectory(metadataFolder);
            } catch (Exception)
            {
                MessageBox.Show("Failed to create folder.");
                this.DriveSelectionPanel.Enabled = true;
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
                this.DriveSelectionPanel.Enabled = true;
            }
        }

        private void UpdateProgressBar(MakeMkvProgress progress) {
            //CurrentProgressBar.Value = progress.Current;
            //TotalProgressBar.Value = progress.Total;
        }

        private void ReadBtn_Click(object sender, EventArgs e) {
            this.DriveSelectionPanel.Enabled = false;
            MakeMkvInterface mkv;
            if (!getMakeMkv(out mkv))
            {
                this.DriveSelectionPanel.Enabled = true;
                return;
            }

            this.loadedDisc = mkv.ReadDisc(this.DrivesComboBox.SelectedIndex, null);

            if (this.loadedDisc == null)
            {
                PlaySound(SadSound);
                MessageBox.Show("There was an error reading the disc.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DriveSelectionPanel.Enabled = true;
                return;
            }
            PlaySound(HappySound);

            //Console.WriteLine("All data received:");
            //Console.WriteLine(this.loadedDisc);
            Console.WriteLine("Disc read successful.");

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
                    try
                    {
                        File.Delete(path);
                    } catch (Exception) { }

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

            FileRenamer form = new(this.loadedDisc, OutputFolder, IgnoreIncompleteCheckBox.Checked, this);
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

        private void loadToolStripMenuItem_Click(object sender, EventArgs e) {
            using (OpenFileDialog dialog = new())
            {
                dialog.Filter = "JSON files (*.json)|*.json";
                dialog.RestoreDirectory = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    using (var stream = dialog.OpenFile())
                    {
                        RenameData renameData = new();

                        try
                        {
                            Json.Read(stream, renameData);
                            this.OutputFolder = renameData.OutputFolder;
                            Console.WriteLine("Loaded JSON file.");
                        } catch (Exception ex)
                        {
                            PlaySound(SadSound);
                            MessageBox.Show("Failed to read file: " + ex.Message, "Failed to read file.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        FileRenamer renamer = new(renameData);
                        renamer.Show();
                    }
                }
            }
        }

        private void testToolStripMenuItem_Click(object sender, EventArgs e) {

            /*using (FolderBrowserDialog openFileDialog = new FolderBrowserDialog())
            {
                openFileDialog.InitialDirectory = "F:\\Video\\backup";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //BlurayBackup? backup = BlurayBackup.FromBackupFolder(openFileDialog.SelectedPath);
                    //var viewer = new TaskProgressViewer<Task, SimpleProgress>(x => BD_DISC.LoadBackupFolder(openFileDialog.SelectedPath, x));
                    //viewer.ShowDialog();
                    //var data = MkvToolNixInterface.Identify(Path.Combine(openFileDialog.SelectedPath, "BDMV", "STREAM", "00339.m2ts"));

                }
            }*/
            //new ClipRenamer().ShowDialog();
            
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

                    new ViewInfo(disc).ShowDialog();
                }
            }
        }

        #region V2
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
                        RenameData2 data = new();

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
            new ClipRenamer(renames, disc).ShowDialog();
        }
        #endregion

        private void playlistCreatorToolStripMenuItem_Click(object sender, EventArgs e) {
            if (this.disc != null)
            {
                new PlaylistCreatorForm(this.disc, this.renames).ShowDialog();
            }
        }
    }
}
