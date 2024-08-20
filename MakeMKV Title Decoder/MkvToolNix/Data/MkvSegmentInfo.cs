using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.MkvToolNix.MkvToolNix
{
    public class MkvSegmentInfo
    {
        public int TimestampScale;
        public string MultiplexingApplication = "";
        public string WritingApplication = "";
        public TimeSpan Duration;
        public DateTime Date;
        public string Title = "";
        public List<int> SegmentUID = new();

        private MkvSegmentInfo()
        {

        }

        public static MkvSegmentInfo Parse(Dictionary<string, object?> data)
        {
            MkvSegmentInfo result = new();

            result.TimestampScale = (int)data[MkvInfoKeys.TimestampScale];
            result.MultiplexingApplication = (string)data[MkvInfoKeys.MultiplexingApplication];
            result.WritingApplication = (string)data[MkvInfoKeys.WritingApplication];
            result.Duration = (TimeSpan)data[MkvInfoKeys.Duration];
            result.Date = (DateTime)data[MkvInfoKeys.Date];
            result.Title = (string)data[MkvInfoKeys.Title];
            result.SegmentUID = (List<int>)data[MkvInfoKeys.SegmentUID];

            return result;
        }
    }
}
