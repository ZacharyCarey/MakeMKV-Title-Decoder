using FFMpeg_Wrapper.ffmpeg;
using FFMpeg_Wrapper;
using FFMpeg_Wrapper.ffprobe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Diagnostics;
using FFMpeg_Wrapper.Codecs;
using Utils;

namespace MakeMKV_Title_Decoder.Data {
    public class RemuxCommand {

        [JsonInclude, JsonRequired]
        public string OriginalFile;

        [JsonInclude]
        public List<RemuxStream> Streams = new();

        public FFMpegArgs GetCommand(FFMpeg ffmpeg, MediaAnalysis inputFile, OutputFile outputFile) {
            var args = ffmpeg.Transcode(outputFile);
            args.AddInput(new FileInput(inputFile));

            foreach(var stream in Streams)
            {
                stream.GetArgs(args, inputFile);
            }

            return args;
        }
    }

    public class RemuxStream {
        [JsonInclude]
        public int? InputIndex = null;

        [JsonInclude, JsonRequired]
        public string? Operation;

        [JsonInclude]
        public Dictionary<string, string>? Attributes = null;

        internal void GetArgs(FFMpegArgs args, MediaAnalysis inputFile) {
            StreamOptions stream;
            if (Operation == "Copy")
            {
                Debug.Assert(InputIndex != null);
                stream = new StreamOptions(0, InputIndex.Value);
                stream.SetCodec(Codecs.Copy);
            } else if (Operation == "Empty Audio")
            {
                int inputIndex = args.AddInput(new AudioNullSrcInput(inputFile.Duration));
                stream = new StreamOptions(inputIndex, 0);
                stream.SetCodec(Codecs.AC3);
            } else
            {
                throw new Exception();
            }

            if (this.Attributes != null) {
                foreach (var pair in this.Attributes)
                {
                    switch(pair.Key)
                    {
                        case "Sample Rate":
                            stream.SetAudioFrequency(int.Parse(pair.Value));
                            break;
                        case "Channels":
                            stream.SetAudioChannels(int.Parse(pair.Value));
                            break;
                        case "Default":
                            stream.SetFlag(StreamFlag.Default, bool.Parse(pair.Value));
                            break;
                        case "Commentary":
                            stream.SetFlag(StreamFlag.Commentary, bool.Parse(pair.Value));
                            break;
                        case "Language":
                            stream.SetLanguage(LanguageJsonConverter.TryParseLanguage(pair.Value));
                            break;
                        case "Name":
                            stream.SetName(pair.Value);
                            break;
                        default: 
                            Debug.Fail($"Unknown attribute key in remux: '{pair.Key}'"); 
                            break;
                    }
                }
            }

            args.AddStream(stream);
        }
    }
}
