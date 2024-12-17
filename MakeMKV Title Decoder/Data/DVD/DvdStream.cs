using FFMpeg_Wrapper.ffprobe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace MakeMKV_Title_Decoder.Data.DVD
{
    public class DvdStream : LoadedStream
    {

        internal DvdStream(string root, string filePath, MediaAnalysis data, TimeSpan duration) : base(root, filePath, data)
        {
            this.Duration = duration;
        }

        public override int GetVlcID(LoadedTrack track) {
            return this.Tracks.WithIndex().First(x => x.Value == track).Index;
        }
    }
}
