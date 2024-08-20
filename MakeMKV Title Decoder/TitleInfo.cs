using MakeMKV_Title_Decoder.MakeMKV.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MakeMKV_Title_Decoder
{
    public partial class TitleInfo : UserControl {
        public TitleInfo() {
            InitializeComponent();
        }

        private void TitleInfo_Load(object sender, EventArgs e) {

        }

        public void LoadTitle(Title? title) {
            this.NameLabel.Text = title?.Name ?? "N/A";
            this.SourceFileNameLabel.Text = title?.SourceFileName ?? "";
            this.DurationLabel.Text = title?.Duration?.ToString() ?? "";
            this.SizeLabel.Text = title?.Size?.ToString() ?? "";
            if (title?.Segments == null)
            {
                this.SegmentsLabel.Text = $"";
            } else
            {
                this.SegmentsLabel.Text = $"[{string.Join(", ", title.Segments)}]";
            }
            this.CommentLabel.Text = title?.Comment ?? "";
            this.FileNameLabel.Text = title?.OutputFileName ?? "N/A";
        }
    }
}
