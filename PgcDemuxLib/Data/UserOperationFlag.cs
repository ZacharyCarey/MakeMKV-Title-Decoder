using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PgcDemuxLib.Data
{
    [Flags]
    public enum UserOperationFlag
    {
        None = 0,
        TimePlayOrSearch =              0b0000000000000000000000001,
        PttPlayOrSearch =               0b0000000000000000000000010,
        TitlePlay =                     0b0000000000000000000000100,
        Stop =                          0b0000000000000000000001000,
        GoUp =                          0b0000000000000000000010000,
        TimeOrPttSearch =               0b0000000000000000000100000,
        TopPgOrPrevPgSearch =           0b0000000000000000001000000,
        NextPgSearch =                  0b0000000000000000010000000,
        ForwardScan =                   0b0000000000000000100000000,
        BackwardScan =                  0b0000000000000001000000000,
        MenuCallTitle =                 0b0000000000000010000000000,
        MenuCallRoot =                  0b0000000000000100000000000,
        MenuCallSubpicture =            0b0000000000001000000000000,
        MenuCallAudio =                 0b0000000000010000000000000,
        MenuCallAngle =                 0b0000000000100000000000000,
        MenuCallPtt =                   0b0000000001000000000000000,
        Resume =                        0b0000000010000000000000000,
        ButtonSelectOrActivate =        0b0000000100000000000000000,
        StillOff =                      0b0000001000000000000000000,
        PausOn =                        0b0000010000000000000000000,
        AudioStreamChange =             0b0000100000000000000000000,
        SubpictureStreamChange =        0b0001000000000000000000000,
        AngleChange =                   0b0010000000000000000000000,
        KaraokeAudioMixChange =         0b0100000000000000000000000,
        VideoPresentationModeChange =   0b1000000000000000000000000
    }
}
