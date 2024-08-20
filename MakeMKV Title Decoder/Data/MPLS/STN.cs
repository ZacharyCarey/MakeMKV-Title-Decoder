using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Stream = MakeMKV_Title_Decoder.Data.MPLS.MplsStream;

namespace MakeMKV_Title_Decoder.Data.MPLS {
    public struct STN {

        public uint num_video;
        public uint num_audio;
        public uint num_pg;
        public uint num_ig;
        public uint num_secondary_audio;
        public uint num_secondary_video;
        public uint num_pip_pg;
        public MplsStream[] audio_streams;
        public MplsStream[] video_streams;
        public MplsStream[] pg_streams;

        public void dump() {
            Console.WriteLine($"      stn dump");
            Console.WriteLine($"        num_video / num_audio / num_pg / num_ig:    {num_video} / {num_audio} / {num_pg} / {num_ig}");
            Console.WriteLine($"        num_sec_video / num_sec_audio / num_pip_pg: {num_secondary_video} / {num_secondary_audio} / {num_pip_pg}");
        
            foreach(var stream in video_streams)
            {
                stream.dump("video");
            }

            foreach(var stream in audio_streams)
            {
                stream.dump("audio");
            }

            foreach(var stream in pg_streams)
            {
                stream.dump("pg");
            }
        }
    }
}
