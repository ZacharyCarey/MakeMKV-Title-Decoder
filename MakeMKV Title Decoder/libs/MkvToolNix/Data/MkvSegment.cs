using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.libs.MkvToolNix.Data
{
    public class MkvSegment
    {

        //public List<MkvSeekEntry> SeekHead;
        public MkvSegmentInfo SegmentInfo;
        public List<MkvInfoTrack> Tracks;
        public List<MkvInfoChapter> Chapters;

        private MkvSegment()
        {

        }

        public static MkvSegment Parse(Dictionary<string, object?> data)
        {
            MkvSegment result = new();

            result.SegmentInfo = MkvSegmentInfo.Parse((Dictionary<string, object?>)data[MkvInfoKeys.SegmentInfo]);
            foreach (var pair in (List<KeyValuePair<string, object?>>)data[MkvInfoKeys.Tracks])
            {
                result.Tracks.Add(MkvInfoTrack.Parse(pair));
            }

            if (data.ContainsKey(MkvInfoKeys.Chapters))
            {
                var chapters = (List<KeyValuePair<string, object?>>)data[MkvInfoKeys.Chapters];
                foreach (var pair in chapters)
                {
                    if (pair.Key != MkvInfoKeys.EditionEntry)
                    {
                        throw new FormatException("Unknown chapter entry.");
                    }

                    result.Chapters.Add(MkvInfoChapter.Parse((List<KeyValuePair<string, object?>>)pair.Value));
                }
            }

            return result;
        }
    }
}
