using FfmpegInterface.FFProbeCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace FfmpegInterface.FFMpegCore {
    public class SelectedStreams {

        private List<string> Args;
        int outputStreamIndex = 0;

        public SelectedStreams() {
            Args = new();
        }

        private SelectedStreams(string streams) {
            this.Args = new() { streams };
        }

        internal IEnumerable<string> Arguments { 
            get {
                foreach(string arg in Args)
                {
                    yield return arg;
                }
            } 
        }

        public static SelectedStreams All() {
            return new SelectedStreams("-map 0"); 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="languageCode">ISO 639-2</param>
        /// <returns></returns>
        public SelectedStreams FromAnalysis(MediaStream stream, LanguageCode? language = null) {
            // -map 0:0 -map 0:1 -map 0:2
            return this.FromIndex(stream.Index, language);
        }

        public SelectedStreams FromIndex(int streamIndex, LanguageCode? language = null) {
            this.Args.Add($"-map 0:{streamIndex}");
            if (language != null)
            {
                // TODO this is hard-coded to only work for audio stream. Not very good as
                // a general library but it works for now
                this.Args.Add($"-metadata:s:a:{outputStreamIndex} language={language.Value.Code}");
            }
            outputStreamIndex++;
            return this;
        }

        public SelectedStreams ByType(params StreamType[] types) {
            foreach (var type in types)
            {
                this.Args.Add($"-map 0:{GetStreamSpecifier(type)}?");
            }
            return this;
        }

        private static string GetStreamSpecifier(StreamType type) {
            switch (type)
            {
                case StreamType.Video: return "v";
                case StreamType.Audio: return "a";
                case StreamType.Subtitle: return "s";
                case StreamType.Data: return "d";
                case StreamType.Attachment: return "t";
                default:
                    throw new ArgumentException("Invalid enum value.");
            }
        }
    }

    public enum StreamType {
        Video,
        Audio,
        Subtitle,
        Data,
        Attachment
    }
}
