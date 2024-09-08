using MakeMKV_Title_Decoder.MkvToolNix.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.Forms.PlaylistCreator {
    public class AppendedFile {
        public MkvMergeID Source;
        public Color Color;

        public AppendedFile(MkvMergeID source, Color color) {
            this.Source = source;
            this.Color = color;
        }
    }
}
