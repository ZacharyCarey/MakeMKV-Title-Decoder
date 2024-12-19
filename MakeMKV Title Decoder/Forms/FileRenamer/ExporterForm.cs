using FFMpeg_Wrapper.Filters.Video;
using MakeMKV_Title_Decoder.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace MakeMKV_Title_Decoder.Forms.FileRenamer {
    public partial class ExporterForm : Form {
        /// <summary>
        /// The amount of minutes it takes to transfer 1 GB
        /// </summary>
        const double CopyTimePerGB = 0.46870601;

        // Key = input/output resolutions
        Dictionary<string, (double Time, double Size)> TranscodingEstimated = new() {
            { "2160/2160", (1.48681924330641, 0.177088705132092) },
            { "2160/1080", (0.564927839087237, 0.100961742870243) },
            { "2160/720", (0.295599467022854, 0.217953770199662) },
            { "2160/480", (0.217953770199662, 0.131950416996034) },
            { "1080/1080", (0.566453599087411, 0.255660869094682) },
            { "480/1080", (0.554675286250584, 1.75738909866316) },
            { "480/480", (0.292052505927378, 0.565509889482981) },
        };

        Dictionary<CheckBox, (DataSize FileSize, TimeSpan ProcessTime)> Estimates = new();

        public IEnumerable<ScaleResolution?> SelectedMainFeatureResolutions => MainFeatureGUI.Where(x => x.IsSelected).Select(x => x.Scale);
        public ScaleResolution? SelectedExtrasResolutions => ExtrasGUI.Where(x => x.IsSelected).Select(x => x.Scale).FirstOrDefault();

        GuiCheckBox[] MainFeatureGUI;
        GuiRadioButton[] ExtrasGUI;

        DataSize[] EstimatedMainFeatureSizes = new DataSize[5];
        TimeSpan[] EstimatedMainFeatureTime = new TimeSpan[5];

        DataSize[] EstimatedExtrasSizes = new DataSize[5];
        TimeSpan[] EstimatedExtrasTime = new TimeSpan[5];

        LoadedDisc Disc;

        public ExporterForm(LoadedDisc disc, IEnumerable<Exportable> exportables) {
            this.Disc = disc;
            this.DialogResult = DialogResult.Cancel;
            exportables = exportables.Where(x => x.IsTranscodable);

            InitializeComponent();

            bool allowOriginalQuality = !disc.ForceTranscoding;
            this.ExportOriginalCheckBox.Enabled = allowOriginalQuality;
            this.ExportOriginalRadioButton.Enabled = allowOriginalQuality;
            this.DVDWarningLabel.Visible = !allowOriginalQuality;

            this.MainFeatureGUI = [
                new(ExportOriginalCheckBox, ExportOriginalFileSizeLabel, ExportOriginalTimeLabel, 0, null),
                new(Export4kCheckBox, Export4kFileSizeLabel, Export4kEstimatedTimeLabel, 2160, ScaleResolution.UHD_3840x2160),
                new(Export1080pCheckBox, Export1080pFileSizeLabel, Export1080pEstimatedTimeLabel, 1080, ScaleResolution.HD_1920x1080),
                new(Export720pCheckBox, Export720pFileSizeLabel, Export720pEstimatedTimeLabel, 720, ScaleResolution.HD_1280x720),
                new(Export480pCheckBox, Export480pFileSizeLabel, Export480pEstimatedTimeLabel, 480, ScaleResolution.SD_720x480)
            ];

            this.ExtrasGUI = [
                new(ExportOriginalRadioButton, ExtrasOriginalSizeLabel, ExtrasOriginalTimeLabel, 0, null),
                new(Export4kRadioButton, Extras4kSizeLabel, Extras4kTimeLabel, 2160, ScaleResolution.UHD_3840x2160),
                new(Export1080pRadioButton, Extras1080pSizeLabel, Extras1080pTimeLabel, 1080, ScaleResolution.HD_1920x1080),
                new(Export720pRadioButton, Extras720pSizeLabel, Extras720pTimeLabel, 720, ScaleResolution.HD_1280x720),
                new(Export480pRadioButton, Extras480pSizeLabel, Extras480pTimeLabel, 480, ScaleResolution.SD_720x480)
            ];

            List<Playlist> mainFeatures = new();
            List<Playlist> extras = new();
            foreach (Exportable exportable in exportables)
            {
                if (exportable is Playlist playlist)
                {
                    if (playlist.OutputFile.Type == FeatureType.MainFeature)
                    {
                        mainFeatures.Add(playlist);
                    } else
                    {
                        extras.Add(playlist);
                    }
                }
            }

            int mainFeatureHeight = GetLargestResolution(mainFeatures);
            this.MainFeatureLabel.Text = $"Main Feature: {mainFeatureHeight}p";
            for (int i = 0; i < MainFeatureGUI.Length; i++)
            {
                this.CalculateEstimated(mainFeatures, MainFeatureGUI[i].Resolution, out EstimatedMainFeatureSizes[i], out EstimatedMainFeatureTime[i]);
                UpdateUI(MainFeatureGUI[i].EstimatedSizeLabel, MainFeatureGUI[i].Resolution, EstimatedMainFeatureSizes[i]);
                UpdateUI(MainFeatureGUI[i].EstimatedTimeLabel, EstimatedMainFeatureTime[i]);
                MainFeatureGUI[i].CheckBox.CheckedChanged += ResolutionCheckBox_Checked;

                // Only allow selection if same or smaller resolution
                if (i != 0)
                {
                    MainFeatureGUI[i].CheckBox.Enabled = (MainFeatureGUI[i].Resolution <= mainFeatureHeight);
                }
            }

            int extrasHeight = GetLargestResolution(extras);
            this.ExtrasLabel.Text = $"Extras: {extrasHeight}p";
            for (int i = 0; i < ExtrasGUI.Length; i++)
            {
                this.CalculateEstimated(extras, ExtrasGUI[i].Resolution, out EstimatedExtrasSizes[i], out EstimatedExtrasTime[i]);
                UpdateUI(ExtrasGUI[i].EstimatedSizeLabel, ExtrasGUI[i].Resolution, EstimatedExtrasSizes[i]);
                UpdateUI(ExtrasGUI[i].EstimatedTimeLabel, EstimatedExtrasTime[i]);
                ExtrasGUI[i].Button.CheckedChanged += ResolutionCheckBox_Checked;
            }

            UpdateTotalUI();
        }

        private void ExporterForm_Load(object sender, EventArgs e) {

        }

        private void ExportBtn_Click(object sender, EventArgs e) {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void ResolutionCheckBox_Checked(object? sender, EventArgs e) {
            UpdateTotalUI();
        }

        private void UpdateTotalUI() {
            DataSize totalSize = new();
            TimeSpan totalTime = new();

            for (int i = 0; i < MainFeatureGUI.Length; i++)
            {
                if (MainFeatureGUI[i].IsSelected)
                {
                    totalSize += EstimatedMainFeatureSizes[i];
                    totalTime += EstimatedMainFeatureTime[i];
                }
            }

            for (int i = 0; i < ExtrasGUI.Length; i++)
            {
                if (ExtrasGUI[i].IsSelected)
                {
                    totalSize += EstimatedExtrasSizes[i];
                    totalTime += EstimatedExtrasTime[i];
                }
            }

            this.TotalFileSizeLabel.Text = totalSize.ToString();
            this.TotalTimeLabel.Text = $"{totalTime:hh\\:mm\\:ss}";
        }

        private static void UpdateUI(Label label, int targetResolution, DataSize size) {
            string prefix;
            if (targetResolution == 0)
            {
                prefix = "";
            } else
            {
                prefix = $"Estimated ";
            }

            string sizeString;
            if (size.Near(new DataSize(), new DataSize(100, Unit.None)))
            {
                // Size is effectively zero, just assume we dont have a good size estimate
                sizeString = "";
            } else
            {
                sizeString = size.ToString();
            }

            label.Text = $"{prefix}{sizeString}";
        }

        private static void UpdateUI(Label label, TimeSpan time) {
            if (time.TotalSeconds < 1)
            {
                // Just assume we dont have a good time estimate
                label.Text = "";
            } else
            {
                label.Text = $"{time:hh\\:mm\\:ss}";
            }
        }

        private void CalculateEstimated(IEnumerable<Playlist> playlists, int targetResolution, out DataSize estimatedSize, out TimeSpan estimatedTime) {
            estimatedSize = new();
            estimatedTime = new();
            int verticalResolution = 0;

            // First get the total of all files in the playlist
            foreach (var file in playlists.SelectMany(x => x.SourceFiles))
            {
                LoadedStream stream = this.Disc[file.SourceUID];
                estimatedSize += stream.Identity.FileSize;
                estimatedTime += stream.Duration;

                int height = stream.Tracks.Select(x => x.Identity.Height ?? 0).Max();
                if (height > verticalResolution) verticalResolution = height;
            }

            // Attempt to calculate estimates
            if (targetResolution == 0)
            {
                // Original file copy
                estimatedTime = TimeSpan.FromMinutes(estimatedSize.AsGB() * CopyTimePerGB);
            } else
            {
                (double Time, double Size) estimatePercentage;
                if (verticalResolution != 0 && this.TranscodingEstimated.TryGetValue($"{verticalResolution}/{targetResolution}", out estimatePercentage))
                {
                    estimatedSize *= estimatePercentage.Size;
                    estimatedTime *= estimatePercentage.Time;
                } else if (verticalResolution != 0)
                {
                    Log.Warn($"Unknown estimate for input resolution '{verticalResolution}' and target resolution '{targetResolution}'.");
                    estimatedSize = new();
                    estimatedTime = new();
                } else
                {
                    estimatedSize = new();
                    estimatedTime = new();
                }
            }
        }

        private int GetLargestResolution(IEnumerable<Playlist> playlists) {
            int largest = 0;
            foreach(var file in playlists.SelectMany(x => x.SourceFiles))
            {
                LoadedStream stream = this.Disc[file.SourceUID];
                int height = stream.Tracks.Select(x => x.Identity.Height ?? 0).Max();
                if (height > largest) largest = height;
            }
            return largest;
        }

        private abstract class GuiElement {
            internal abstract bool IsSelected { get; }
            public Label EstimatedSizeLabel;
            public Label EstimatedTimeLabel;
            public int Resolution; // Original = 0
            public ScaleResolution? Scale;

            protected GuiElement(Label sizeLabel, Label timeLabel, int resolution, ScaleResolution? scale) {
                this.EstimatedSizeLabel = sizeLabel;
                this.EstimatedTimeLabel = timeLabel;
                this.Resolution = resolution;
                this.Scale = scale;
            }
        }
        private class GuiCheckBox : GuiElement {
            public CheckBox CheckBox;

            internal override bool IsSelected => CheckBox.Checked;

            public GuiCheckBox(CheckBox checkBox, Label sizeLabel, Label timeLabel, int resolution, ScaleResolution? scale) 
                : base(sizeLabel, timeLabel, resolution, scale)    
            {
                this.CheckBox = checkBox;
            }
        }
        private class GuiRadioButton : GuiElement {
            public RadioButton Button;

            internal override bool IsSelected => Button.Checked;

            public GuiRadioButton(RadioButton button, Label sizeLabel, Label timeLabel, int resolution, ScaleResolution? scale)
                : base(sizeLabel, timeLabel, resolution, scale) {
                this.Button = button;
            }
        }

    }
}
