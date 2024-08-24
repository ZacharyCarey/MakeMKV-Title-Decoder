using libbluray.bdnav;
using libbluray.disc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MakeMKV_Title_Decoder {
    public partial class ViewInfo : Form {

        BD_DISC disc;

        public ViewInfo(BD_DISC disc) {
            this.disc = disc;
            InitializeComponent();
        }

        private void ViewInfo_Load(object sender, EventArgs e) {
            MPLS_PL? playlist = mpls.get(disc, "00001.mpls");
            this.SourceFileLabel.Text = "00001";
            if (playlist != null)
            {
                MPLS_STREAM stream = playlist.play_item[0].stn.video[0];
                DisplaySizeLabel.Text = GetResolutionText(stream.video_format);
                FPSLabel.Text = $"{GetFPSText(stream.video_rate)} FPS";
            }


            var dir = disc.open_dir(Path.Combine("BDMV", "CLIPINF"));
            string filename;
            while(dir.read(out filename))
            {
                TitleList.Items.Add(Path.GetFileNameWithoutExtension(filename));
            }

            var clip = CLPI_CL.get(disc, "00001.clpi");
            Console.WriteLine(clip != null);
        }

        private static string GetResolutionText(VideoFormat format) {
            switch(format)
            {
                case VideoFormat._480I:
                case VideoFormat._480P:
                    return "720x480";
                case VideoFormat._576I:
                case VideoFormat._576P:
                    return "720x576";
                case VideoFormat._720P:
                    return "1280x720";
                case VideoFormat._1080I:
                case VideoFormat._1080P:
                    return "1920x1080";
                case VideoFormat._2160P:
                    return "3840x2160";
                default:
                    return "Unknown";
            }
        }

        private static string GetFPSText(VideoRate rate) {
            switch(rate)
            {
                case VideoRate._24000_1001: return "23.976";
                case VideoRate._24: return "24";
                case VideoRate._25: return "25";
                case VideoRate._30000_1001: return "29.97";
                case VideoRate._50: return "50";
                case VideoRate._60000_1001: return "59.94";
                default:
                    return "Unknown";
            }
        }
    }
}
