using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FfmpegInterface.FFProbeCore {
    public class AudioStream : MediaStream {
        public int Channels { get; set; }
        public string ChannelLayout { get; set; } = null!;
        public int SampleRateHz { get; set; }
        public string Profile { get; set; } = null!;
    }
}
