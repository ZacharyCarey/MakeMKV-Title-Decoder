using FFMpeg_Wrapper.Codecs;
using FFMpeg_Wrapper.ffmpeg;
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
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace MakeMKV_Title_Decoder.Forms.FileRenamer {
    public partial class ExporterForm : Form {
        public IEnumerable<TranscodeArgs?> SelectedMainFeatureResolutions => MainFeatureGUI.Where(x => x.IsSelected).Select(x => x.GetTranscodeArgs());
        public IEnumerable<TranscodeArgs?> SelectedExtrasResolutions => ExtrasGUI.Where(x => x.IsSelected).Select(x => x.GetTranscodeArgs());
        public ScaleResolution? MainFeatureResolution { get; set; } = null;
        public ScaleResolution? ExtrasResolution { get; set; } = null;

        GuiCheckBox[] MainFeatureGUI;
        GuiRadioButton[] ExtrasGUI;

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
                new(ExportOriginalCheckBox, -1, null),
                new(TvEncodingCheckBox, 2160, new TvTranscodeArgs(ScaleResolution.UHD_3840x2160)),
                new(TvEncoding1080pCheckBox, 1080, new TvTranscodeArgs(ScaleResolution.HD_1920x1080)),
                new(TvEncoding720pCheckBox, 720, new TvTranscodeArgs(ScaleResolution.HD_1280x720)),
                new(TvEncoding480pCheckBox, 480, new TvTranscodeArgs(ScaleResolution.SD_720x480)),
                new(MobileEncodingCheckBox, 1080, new MobileTranscodeArgs(ScaleResolution.HD_1920x1080)),
                new(MobileEncoding720pCheckBox, 720, new MobileTranscodeArgs(ScaleResolution.HD_1280x720)),
                new(MobileEncoding480pCheckBox, 480, new MobileTranscodeArgs(ScaleResolution.SD_720x480))
            ];

            this.ExtrasGUI = [
                new(ExportOriginalRadioButton, -1, null),
                new(Export4kRadioButton, 2160, new TvTranscodeArgs(ScaleResolution.UHD_3840x2160, null)),
                new(Export1080pRadioButton, 1080, new TvTranscodeArgs(ScaleResolution.HD_1920x1080, null)),
                new(Export720pRadioButton, 720, new TvTranscodeArgs(ScaleResolution.HD_1280x720, null)),
                new(Export480pRadioButton, 480, new TvTranscodeArgs(ScaleResolution.SD_720x480, null))
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
                //MainFeatureGUI[i].CheckBox.CheckedChanged += ResolutionCheckBox_Checked;

                // Only allow selection if same or smaller resolution
                if (i != 0)
                {
                    int target_res = MainFeatureGUI[i].Resolution;
                    MainFeatureGUI[i].CheckBox.Enabled = (target_res < 0) || (target_res <= mainFeatureHeight);
                }
            }

            // Used externally
            foreach (ScaleResolution resolution in Enum.GetValues<ScaleResolution>().OrderBy(x => (int)x))
            {
                if ((int)resolution <= mainFeatureHeight)
                {
                    this.MainFeatureResolution = resolution;
                }
            }

            int extrasHeight = GetLargestResolution(extras);
            this.ExtrasLabel.Text = $"Extras: {extrasHeight}p";
            for (int i = 0; i < ExtrasGUI.Length; i++)
            {
                ExtrasGUI[i].Button.CheckedChanged += ResolutionCheckBox_Checked;

                // Only allow selection if same or smaller resolution
                if (i != 0)
                {
                    int target_res = ExtrasGUI[i].Resolution;
                    ExtrasGUI[i].Button.Enabled = (target_res < 0) || (target_res <= extrasHeight);
                }
            }

            // Always allow the lowest resolution
            this.ExtrasGUI.Last().Button.Enabled = true;

            // Used externally
            foreach (ScaleResolution resolution in Enum.GetValues<ScaleResolution>().OrderBy(x => (int)x))
            {
                if ((int)resolution <= extrasHeight)
                {
                    this.ExtrasResolution = resolution;
                }
            }
        }

        private void ExporterForm_Load(object sender, EventArgs e) {

        }

        private void ExportBtn_Click(object sender, EventArgs e) {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void ResolutionCheckBox_Checked(object? sender, EventArgs e) {
            
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
            public int Resolution; // Original = 0
            TranscodeArgs? TranscodeArgs;

            public TranscodeArgs? GetTranscodeArgs() => TranscodeArgs;

            protected GuiElement(int resolution, TranscodeArgs? transcodeArg) {
                this.Resolution = resolution;
                this.TranscodeArgs = transcodeArg;
            }
        }
        private class GuiCheckBox : GuiElement {
            public ButtonBase CheckBox;

            internal override bool IsSelected => getCheckedFunc(CheckBox);
            private readonly Func<ButtonBase, bool> getCheckedFunc;

            public GuiCheckBox(ButtonBase checkBox, int resolution, TranscodeArgs? transcodeArgs) 
                : base(resolution, transcodeArgs)    
            {
                this.CheckBox = checkBox;
                if (checkBox is CheckBox)
                {
                    getCheckedFunc = GetCheckBoxChecked;
                } else if (checkBox is RadioButton)
                {
                    getCheckedFunc = GetRadioButtonChecked;
                } else
                {
                    throw new Exception("Invalid button type.");
                }
            }

            private static bool GetCheckBoxChecked(ButtonBase btn) {
                return ((CheckBox)btn).Checked;
            }

            private static bool GetRadioButtonChecked(ButtonBase btn) {
                return ((RadioButton)btn).Checked;
            }
        } 

        private class GuiRadioButton : GuiElement {
            public RadioButton Button;

            internal override bool IsSelected => Button.Checked;

            public GuiRadioButton(RadioButton button, int resolution, TranscodeArgs? args)
                : base(resolution, args) {
                this.Button = button;
            }
        }

    }

    public class TvTranscodeArgs : TranscodeArgs {
        public string? VersionTag { get; private set; }
        public ScaleResolution? ScaleDownToResolution { get; private set; }

        public TvTranscodeArgs(string? tag = "TV") {
            this.ScaleDownToResolution = null;
            this.VersionTag = tag;
        }

        public TvTranscodeArgs(ScaleResolution resolution, string? tag = "TV") {
            this.ScaleDownToResolution = resolution;
            this.VersionTag = tag;
        }

        public void GetVideoOptions(VideoStreamOptions options) {
            options.SetCodec(
                Codecs.LibSvtAV1
                .SetPreset(5)
                .SetCRF(20)
            );
        }

        public void GetAudioOptions(AudioStreamOptions options) {
            options.SetCodec(Codecs.EAC3);
        }
    }

    public class MobileTranscodeArgs : TranscodeArgs {
        public string? VersionTag { get; private set; }
        public ScaleResolution? ScaleDownToResolution { get; private set; } = ScaleResolution.HD_1920x1080;

        public MobileTranscodeArgs(string? tag = "Mobile") {
            this.VersionTag = tag;
        }

        public MobileTranscodeArgs(ScaleResolution resolution, string? tag = "Mobile") {
            this.VersionTag = tag;
            this.ScaleDownToResolution = resolution;
        }

        public void GetVideoOptions(VideoStreamOptions options) {
            options.SetCodec(
                Codecs.LibSvtAV1
                .SetPreset(8)
                .SetCRF(35)
            );
        }

        public void GetAudioOptions(AudioStreamOptions options) {
            options.SetCodec(Codecs.AAC)
                .SetAudioBitrate(384000)
                .SetAudioChannels(2);
        }
    }

    public class CompatibilityTranscodeArgs : TranscodeArgs {
        public static CompatibilityTranscodeArgs Bitrate25Mbps => new(21992000, 21992000, 43984000, 384000);
        public static CompatibilityTranscodeArgs Bitrate4Mbps => new(3808000, 3808000, 7616000, 192000);

        public string VersionTag => "Compatibility";
        public ScaleResolution? ScaleDownToResolution => ScaleResolution.HD_1920x1080;

        private long videoBitrate;
        private long maxRate;
        private long bufferSize;
        private long audioBitrate;

        public CompatibilityTranscodeArgs(long VideoBitrate, long MaxRate, long BufferSize, long AudioBitrate) {
            this.videoBitrate = VideoBitrate;
            this.maxRate = MaxRate;
            this.bufferSize = BufferSize;
            this.audioBitrate = AudioBitrate;
        }

        public void GetVideoOptions(VideoStreamOptions options) {
            options.SetCodec(Codecs.NvidiaH264)
            .SetVideoBitrate(videoBitrate)
            .SetGroupOfPictureSize(144)
            .SetMaxRate(maxRate)
            .SetBufferSize(bufferSize)
            .SetMinimumKeyInterval(144)
            .SetProfile("high")
            .SetPixelFormat("yuv420p");
        }

        public void GetAudioOptions(AudioStreamOptions options) {
            options.SetCodec(Codecs.AAC)
            .SetAudioBitrate(audioBitrate)
            .SetAudioChannels(2);
        }
    }
}
