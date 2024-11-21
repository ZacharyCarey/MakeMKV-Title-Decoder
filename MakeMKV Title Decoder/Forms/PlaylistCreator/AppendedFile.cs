using MakeMKV_Title_Decoder.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.Forms.PlaylistCreator
{
    public class AppendedFile {
        public LoadedStream Source;
        public Color Color;
        public List<AppendedFile> AppendedFiles = new();

        public AppendedFile(LoadedStream source, Color color) {
            this.Source = source;
            this.Color = color;
        }
    }
}
