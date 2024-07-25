using System;
using System.IO;
using System.Media;
using System.Reflection;

namespace MakeMKV_Title_Decoder {
    public partial class Form1 : Form {
        string OutputFolder = null;
        SegmentIdentifier identifier = null;
        List<Title> AllTitles = null;

        const string HappySound = "Success.wav";
        const string SadSound = "Error.wav";

        public Form1() {
            InitializeComponent();
        }

        private DvdType getDvdType() {
            if (this.MovieRadioBtn.Checked)
            {
                return DvdType.Movie;
            }
            else if (this.TvRadioBtn.Checked)
            {
                return DvdType.TV;
            }
            else if (this.BonusFeaturesRadioBtn.Checked)
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

                MakeMKVInput input = new();
                MakeMKVScraper scraper = new MakeMKVScraper(input);

                // Save output folder for later
                this.OutputFolder = input.ReadOutputFolder();
                Console.WriteLine($"Found output path: {this.OutputFolder ?? "null"}");

                // scrape all titles
                input.FocusMKV();
                scraper.Scrape();
                AllTitles = scraper.Titles;

                Console.WriteLine($"Found {scraper.Titles.Count} titles.");

                // identify which to keep
                identifier = new SegmentIdentifier(scraper.Titles, getDvdType());
               
                foreach(Title title in AllTitles.Reverse<Title>()) // Reverse since our cursor will be at the 
                {
                    // Determined by identifier
                    bool deselect = identifier.DeselectTitlesIndicies.Contains(title.Index);
                    bool isMainFeature = (title.Index == identifier.MainFeature.Index) || (identifier.MainTitleTracks.Contains(title.Index));
                    bool isBonusFeature = identifier.BonusFeatures.Contains(title.Index);

                    // Filter additional titles
                    if ((deselect == false) && (isMainFeature == false) && (isBonusFeature == false) && this.IgnoreIncompleteCheckBox.Checked)
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
                        input.ToggleAttachment(title);
                    }

                    if (deselect)
                    {
                        input.ToggleTitleSelection(title);
                    }
                }

                Refocus();

                // Print data
                Console.WriteLine($"The main feature is {identifier.MainFeature.SimplifiedFileName}");
                if (!identifier.IsMovie)
                {
                    for (int i = 0; i < identifier.MainTitleTracks.Count; i++)
                    {
                        int episode = i + 1;
                        Title title = AllTitles[identifier.MainTitleTracks[i]];
                        Console.WriteLine($"\tEpisode {episode:D2} is file {title.SimplifiedFileName}");
                    }
                }
                Console.WriteLine("Possible bonus features:");
                IEnumerable<Title> bonusFeatures = identifier.BonusFeatures.Select(i => AllTitles[i]);
                foreach (Title bonus in bonusFeatures.OrderBy(x => x.SimplifiedFileName))
                {
                    Console.WriteLine($"\t{bonus.SimplifiedFileName}");
                }

                PlaySound(HappySound);
                /*int n = scraper.Titles[0].SourceFileDuplicateNumber;
                PlaylistCreater file = new();
                file.Add("Test1.txt");
                file.Add("Folder1/Test2.txt");
                file.Add("C:\\Users\\Zach\\source\\repos\\MakeMKV Title Decoder\\MakeMKV Title Decoder\\obj\\test2.txt");
                file.Add("C:\\Users\\Zach\\source\\repos\\MakeMKV Title Decoder\\MakeMKV Title Decoder\\bin\\Debug\\net8.0-windows\\Folder2\\test.txt");
                file.Save("test.m3u8");*/
            }
            catch (Exception err)
            {
                PlaySound(SadSound);
                MessageBox.Show(err.Message, "An error occured.", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                if (identifier.IsMovie)
                {
                    try
                    {
                        string oldPath = Path.Combine(folder, identifier.MainFeature.FileName);
                        string newPath = Path.Combine(folder, "MainFeature.mkv");
                        File.Move(oldPath, newPath);
                        Console.WriteLine($"Renamed from {oldPath} to {newPath}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Failed to rename file.");
                        MessageBox.Show(ex.Message, "Failed to rename file.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    try
                    {
                        for (int i = 0; i < identifier.MainTitleTracks.Count; i++)
                        {
                            Title title = AllTitles[identifier.MainTitleTracks[i]];
                            string oldPath = Path.Combine(folder, title.FileName);
                            string newPath = Path.Combine(folder, $"Episode_{(i + 1):D2}.mkv");
                            File.Move(oldPath, newPath);
                            Console.WriteLine($"Renamed from {oldPath} to {newPath}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Failed to rename file.");
                        MessageBox.Show(ex.Message, "Failed to rename file.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e) {

        }
    }
}
