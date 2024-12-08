using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FfmpegInterface.FFProbeCore
{
    public interface IMediaAnalysis
    {
        TimeSpan Duration { get; }
        MediaFormat Format { get; }
        List<ChapterData> Chapters { get; }
        AudioStream? PrimaryAudioStream { get; }
        VideoStream? PrimaryVideoStream { get; }
        SubtitleStream? PrimarySubtitleStream { get; }
        List<VideoStream> VideoStreams { get; }
        List<AudioStream> AudioStreams { get; }
        List<SubtitleStream> SubtitleStreams { get; }
        IReadOnlyList<string> ErrorData { get; }
    }
}
