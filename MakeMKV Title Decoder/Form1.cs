namespace MakeMKV_Title_Decoder {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void ScrapeBtn_Click(object sender, EventArgs e) {
            try
            {
                MakeMKVInput input = new();
                MakeMKVScraper scraper = new MakeMKVScraper(input);
                scraper.Scrape();

                Console.WriteLine($"Found {scraper.Titles.Count} titles.");

                SegmentIdentifier identifier = new SegmentIdentifier(scraper.Titles);
                Console.WriteLine($"MainFeature = {identifier.MainFeature.FileName}");

                foreach(int titleIndex in identifier.DeselectTitlesIndicies)
                {
                    input.CheckUncheckTitle(titleIndex);
                }

                input.OpenDropdown(identifier.MainFeature.Index);

                Refocus();

                // Print data

                int n = scraper.Titles[0].SourceFileDuplicateNumber;
                PlaylistCreater file = new();
                file.Add("Test1.txt");
                file.Add("Folder1/Test2.txt");
                file.Add("C:\\Users\\Zach\\source\\repos\\MakeMKV Title Decoder\\MakeMKV Title Decoder\\obj\\test2.txt");
                file.Add("C:\\Users\\Zach\\source\\repos\\MakeMKV Title Decoder\\MakeMKV Title Decoder\\bin\\Debug\\net8.0-windows\\Folder2\\test.txt");
                file.Save("test.m3u8");
            }catch(Exception err)
            {
                MessageBox.Show(err.Message, "An error occured.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Refocus() {
            // Regain focus of window
            this.WindowState = FormWindowState.Minimized;
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }
    }
}
