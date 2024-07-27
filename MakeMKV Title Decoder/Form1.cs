using JsonSerializable;
using System;
using System.IO;
using System.Linq;
using System.Media;
using System.Reflection;

namespace MakeMKV_Title_Decoder {
    public partial class Form1 : Form, IJsonSerializable {
        string? OutputFolder = null;
        MakeMKVInput? input = null;
        MakeMKVScraper? scraper = null;
        SegmentIdentifier? identifier = null;

        const string HappySound = "Success.wav";
        const string SadSound = "Error.wav";

        public Form1() {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e) {
            ConsoleLog.CreateLogger("Log.txt");
        }

        private DvdType getDvdType() {
            if (this.MovieRadioBtn.Checked)
            {
                return DvdType.Movie;
            } else if (this.TvRadioBtn.Checked)
            {
                return DvdType.TV;
            } else if (this.BonusFeaturesRadioBtn.Checked)
            {
                return DvdType.BonusFeatures;
            }

            throw new Exception("Unknown DVD type selected.");
        }

        private void ScrapeBtn_Click(object sender, EventArgs e) {
            Console.Clear();

            try
            {
                // Give time for mouse to stop moving
                Console.WriteLine("Waiting for mouse to stop moving...");
                Thread.Sleep(3000);

                if (input == null)
                {
                    input = new MakeMKVInput(true);
                }
                scraper = new MakeMKVScraper(input);

                // Save output folder for later
                this.OutputFolder = input.ReadOutputFolder();
                Console.WriteLine($"Found output path: {this.OutputFolder ?? "null"}");

                // scrape all titles
                input.FocusMKV();
                scraper.Scrape();
            } catch (Exception err)
            {
                PlaySound(SadSound);
                MessageBox.Show("An error occured.", err.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            IdentifyData();
        }

        private void IdentifyData() {
            try
            {
                if (scraper == null)
                {
                    throw new Exception("Scraper was not found.");
                }

                Console.WriteLine($"Found {scraper.Titles.Count} titles.");

                // identify which to keep
                identifier = new SegmentIdentifier(scraper.Titles, getDvdType());

                foreach (Title title in scraper.Titles.Reverse<Title>()) // Reverse since our cursor will be at the bottom
                {
                    // Determined by identifier
                    bool deselect = identifier.DeselectTitlesIndicies.Contains(title.Index);
                    bool isMainFeature = (title.Index == identifier.MainFeature.Index);
                    if (!isMainFeature)
                    {
                        // Loop through main episodes to check if considered a main feature
                        foreach (Episode episode in identifier.MainTitleTracks) // TODO rename to identifier.MainEpisodes?
                        {
                            if (episode.Indices.Contains(title.Index))
                            {
                                isMainFeature = true;
                                break;
                            }
                        }
                    }
                    bool isBonusFeature = identifier.BonusFeatures.Contains(title.Index);

                    // Filter additional titles
                    if ((deselect == false) && (isMainFeature == false) /*&& (isBonusFeature == false)*/ && this.IgnoreIncompleteCheckBox.Checked)
                    {
                        bool hasAudio = title.Tracks.Contains(TrackType.Audio);
                        bool hasVideo = title.Tracks.Contains(TrackType.Video);
                        if (!(hasAudio && hasVideo))
                        {
                            deselect = true;
                            Console.WriteLine($"Deselected {title.SimplifiedFileName} did not have both audio and video.");
                        }
                    }

                    // Do in bottom-up order
                    if ((deselect == false) && isMainFeature && this.IncludeAttachmentsCheckBox.Checked)
                    {
                        input?.ToggleAttachment(title);
                    }

                    if (deselect)
                    {
                        input?.ToggleTitleSelection(title);
                    }
                }

                if (this.CollapseCheckBox.Checked)
                {
                    input?.CollapseAll();
                }

                Refocus();

                // Print data
                Console.WriteLine($"The main feature is {identifier.MainFeature.SimplifiedFileName}");
                if (!identifier.IsMovie)
                {
                    for (int i = 0; i < identifier.MainTitleTracks.Count; i++)
                    {
                        int episode = i + 1;
                        Console.WriteLine($"\tEpisode {episode:D2} is file {identifier.MainTitleTracks[i].ToString(this.scraper.Titles)}");
                    }
                }
                Console.WriteLine("Possible bonus features:");
                IEnumerable<Title> bonusFeatures = identifier.BonusFeatures.Select(i => scraper.Titles[i]);
                foreach (Title bonus in bonusFeatures.OrderBy(x => x.SimplifiedFileName))
                {
                    Console.WriteLine($"\t{bonus.SimplifiedFileName}");
                }

                if (input != null)
                {
                    // Don't play the sound if we loaded from a file
                    PlaySound(HappySound);
                }
                /*int n = scraper.Titles[0].SourceFileDuplicateNumber;
                PlaylistCreater file = new();
                file.Add("Test1.txt");
                file.Add("Folder1/Test2.txt");
                file.Add("C:\\Users\\Zach\\source\\repos\\MakeMKV Title Decoder\\MakeMKV Title Decoder\\obj\\test2.txt");
                file.Add("C:\\Users\\Zach\\source\\repos\\MakeMKV Title Decoder\\MakeMKV Title Decoder\\bin\\Debug\\net8.0-windows\\Folder2\\test.txt");
                file.Save("test.m3u8");*/
            } catch (Exception err)
            {
                if (input != null)
                {
                    // Dont play the sound if we loaded from a file
                    PlaySound(SadSound);
                }
                MessageBox.Show("An error occured.", err.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Refocus() {
            // Regain focus of window
            this.WindowState = FormWindowState.Minimized;
            this.Show();
            this.WindowState = FormWindowState.Normal;
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

        private void RenameBtn_Click(object sender, EventArgs e) {
            if (this.OutputFolder == null)
            {
                MessageBox.Show("No output folder was found, unable to rename files.", "No output folder", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string folder = Path.GetFullPath(this.OutputFolder);
            /*using (FolderBrowserDialog openFileDialog = new FolderBrowserDialog())
            {
                string path = this.OutputFolder;
                if (path != null)
                {
                    path = Path.GetFullPath(path);
                }
                else
                {
                    path = "C:\\";
                }
                Console.WriteLine($"Using initial path: {path}");
                openFileDialog.InitialDirectory = path;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    folder = openFileDialog.SelectedPath;
                }
            }*/

            if (folder != null && identifier != null)
            {
                List<string> failedRenames = new();
                if (identifier.IsMovie)
                {
                    try
                    {
                        string oldPath = Path.Combine(folder, identifier.MainFeature.FileName);
                        string newPath = Path.Combine(folder, "MainFeature.mkv");
                        File.Move(oldPath, newPath);
                        Console.WriteLine($"Renamed from {oldPath} to {newPath}");
                    } catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to rename file {identifier.MainFeature.SimplifiedFileName}");
                        failedRenames.Add(identifier.MainFeature.SimplifiedFileName);
                    }
                } else
                {

                    for (int i = 0; i < identifier.MainTitleTracks.Count; i++)
                    {
                        Episode episode = identifier.MainTitleTracks[i];
                        foreach (int titleIndex in episode.Indices)
                        {
                            Title title = scraper.Titles[titleIndex];
                            try
                            {
                                string oldPath = Path.Combine(folder, title.FileName);
                                string newPath = Path.Combine(folder, $"Episode_{(i + 1):D2}_{title.SimplifiedFileName}.mkv");
                                File.Move(oldPath, newPath);
                                Console.WriteLine($"Renamed from {oldPath} to {newPath}");
                            } catch (Exception ex)
                            {
                                Console.WriteLine($"Failed to rename file {title.SimplifiedFileName}");
                                failedRenames.Add(title.SimplifiedFileName);

                            }
                        }
                    }

                }

                for (int i = 0; i < identifier.BonusFeatures.Count; i++)
                {
                    Title bonusFeature = scraper.Titles[i];
                    try
                    {
                        string oldPath = Path.Combine(folder, bonusFeature.FileName);
                        string newPath = Path.Combine(folder, $"BonusFeature_{(i + 1):D2}.mkv");
                        File.Move(oldPath, newPath);
                        Console.WriteLine($"Renamed from {oldPath} to {newPath}");
                    } catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to rename file {bonusFeature.SimplifiedFileName}");
                        failedRenames.Add(bonusFeature.SimplifiedFileName);

                    }
                }

                if (failedRenames.Count > 0)
                {
                    MessageBox.Show("Failed to rename file(s): " + string.Join(", ", failedRenames), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void IncludeAttachmentsCheckBox_CheckedChanged(object sender, EventArgs e) {

        }

        private void debugDumpToolStripMenuItem_Click(object sender, EventArgs e) {
            // Give time for mouse to stop moving
            Console.WriteLine("Waiting for mouse to stop moving...");
            Thread.Sleep(3000);

            if (input == null)
            {
                input = new MakeMKVInput(true);
            }
            scraper = new MakeMKVScraper(input);

            // scrape all titles
            input.FocusMKV();
            scraper.Scrape();

            // Save data to file
            using (FileStream stream = File.OpenWrite("DataDump.txt"))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                foreach (Title title in scraper.Titles)
                {
                    writer.WriteLine(title.ToString());
                }
            }

            if (this.CollapseCheckBox.Checked)
            {
                input.CollapseAll();
            }

            Refocus();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e) {
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
                    Json.Write(this, path);
                }
            }
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e) {
            LoadFromFile(true);
        }

        private void loadNoInputToolStripMenuItem_Click(object sender, EventArgs e) {
            LoadFromFile(false);
        }

        private void LoadFromFile(bool useInput) {
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
                        } catch (Exception ex)
                        {
                            MessageBox.Show("Failed to read file: " + ex.Message, "Failed to read file.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }

                    if (useInput)
                    {
                        // Give time for mouse to stop moving
                        Console.WriteLine("Waiting for mouse to stop moving...");
                        Thread.Sleep(3000);

                        if (this.input == null)
                        {
                            this.input = new MakeMKVInput(true);
                        }
                        input.SetTitles(scraper.Titles, true);
                        input.FocusMKV();
                        this.IdentifyData();
                    } else
                    {
                        MakeMKVInput? oldInput = this.input;
                        this.input = null;
                        this.IdentifyData();
                        this.input = oldInput;
                    }

                    
                }
            }
        }

        public JsonData SaveToJson() {
            JsonObject data = new();
            data["Output Folder"] = new JsonString(this.OutputFolder);
            data["Titles"] = this.scraper.SaveToJson();
            return data;
        }

        public void LoadFromJson(JsonData data) {
            JsonObject obj = (JsonObject)data;

            //input = null; // Indicated in other places that we have data, but it was laoded from a file so dont do any inputs
            scraper = new MakeMKVScraper(null);
            scraper.LoadFromJson(obj["Titles"]);

            // Save output folder for later
            this.OutputFolder = (JsonString)obj["Output Folder"];
            Console.WriteLine($"Found output path: {this.OutputFolder ?? "null"}");
        }

        private void checkAllToolStripMenuItem_Click(object sender, EventArgs e) {
            if (input == null || scraper == null)
            {
                MessageBox.Show("No input detected, failed to check all titles.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Give time for mouse to stop moving
            Console.WriteLine("Waiting for mouse to stop moving...");
            Thread.Sleep(3000);

            Console.WriteLine("Checking all titles...");
            input.FocusMKV();
            List<Title> titles = input.CheckedTitles.Order().Select(x => scraper.Titles[x]).ToList();
            foreach (Title title in titles)
            {
                input.ToggleTitleSelection(title);
            }
            Console.WriteLine("Finished.");
            PlaySound(HappySound);
        }
    }
}
