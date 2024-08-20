using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.Data.MkvToolNix
{
    public class MkvInfo
    {

        public EBML_Header EBML_Head;
        public MkvSegment Segment;

        private MkvInfo(EBML_Header header, MkvSegment segment)
        {
            EBML_Head = header;
            Segment = segment;
        }

        public static MkvInfo? Parse(IEnumerable<string> input)
        {
            IEnumerator<string> itr = input.GetEnumerator();
            Dictionary<string, object?> data = new();

            try
            {
                if (itr.MoveNext())
                {
                    Parse(data, itr, 0);
                }
                EBML_Header header = EBML_Header.Parse((Dictionary<string, object?>)data[MkvInfoKeys.EBML_Head]);
                MkvSegment segment = MkvSegment.Parse((Dictionary<string, object?>)data[MkvInfoKeys.Segment]);
                return new MkvInfo(header, segment);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        private static bool Parse(object data, IEnumerator<string> itr, int tab)
        {
            KeyValuePair<string, object?>? last = null;

            bool hasItem = true;
            while (hasItem)
            {
                int tabIndex = itr.Current.IndexOf('+');
                if (tabIndex == tab + 1)
                {
                    // The previous item was a list!
                    if (last == null)
                    {
                        throw new FormatException("Header name was not found!");
                    }

                    if (data is List<KeyValuePair<string, object?>> list)
                    {
                        list.Add(last.Value);

                    }
                    else if (data is Dictionary<string, object?> dict)
                    {

                        dict.Add(last.Value.Key, last.Value.Value);
                    }

                    // Read the data for the sub-list
                    hasItem = Parse(last.Value.Value, itr, tab + 1);
                    continue;
                }
                else if (tabIndex < tab)
                {
                    // Exited this list
                    if (last != null)
                    {
                        if (data is List<KeyValuePair<string, object?>> list)
                        {
                            list.Add(last.Value);
                        }
                        else if (data is Dictionary<string, object?> dict)
                        {

                            dict.Add(last.Value.Key, last.Value.Value);
                        }
                    }
                    return true;
                }
                else if (tabIndex == tab)
                {
                    // Just save the data
                    if (last != null)
                    {
                        if (data is List<KeyValuePair<string, object?>> list)
                        {
                            list.Add(last.Value);
                        }
                        else if (data is Dictionary<string, object?> dict)
                        {

                            dict.Add(last.Value.Key, last.Value.Value);
                        }
                    }
                }
                else
                {
                    throw new FormatException("Unknown error occurred.");
                }

                // Parse the key/value
                last = null;
                string str = itr.Current.Substring(tabIndex + 2);
                int index = str.IndexOf(':');
                if (index >= 0)
                {
                    string key = str.Substring(0, index);
                    string value = str.Substring(index + 2);
                    switch (key)
                    {
                        case MkvInfoKeys.EBML_Version:
                        case MkvInfoKeys.EBML_ReadVersion:
                        case MkvInfoKeys.Max_EBML_IdLength:
                        case MkvInfoKeys.Max_EBML_SizeLength:
                        case MkvInfoKeys.DocumentTypeVersion:
                        case MkvInfoKeys.DocumentTypeReadVersion:
                        case MkvInfoKeys.TimestampScale:
                        case MkvInfoKeys.TrackNumber:
                        case MkvInfoKeys.PixelWidth:
                        case MkvInfoKeys.PixelHeight:
                        case MkvInfoKeys.DisplayWidth:
                        case MkvInfoKeys.DisplayHeight:
                        case MkvInfoKeys.SamplingFrequency:
                        case MkvInfoKeys.Channels:
                        case MkvInfoKeys.BitDepth:
                            last = new(key, int.Parse(value.Split()[0]));
                            break;
                        case MkvInfoKeys.Flag_DefaultTrack:
                        case MkvInfoKeys.Flag_Lacing:
                        case MkvInfoKeys.Flag_Commentary:
                        case MkvInfoKeys.Flag_EditionHidden:
                        case MkvInfoKeys.Flag_EditionDefault:
                        case MkvInfoKeys.Flag_ChapterHidden:
                        case MkvInfoKeys.Flag_ChapterEnabled:
                            last = new(key, int.Parse(value) != 0);
                            break;
                        case MkvInfoKeys.DocumentType:
                        case MkvInfoKeys.MultiplexingApplication:
                        case MkvInfoKeys.WritingApplication:
                        case MkvInfoKeys.Title:
                        case MkvInfoKeys.TrackUID:
                        case MkvInfoKeys.TrackType:
                        case MkvInfoKeys.CodecID:
                        case MkvInfoKeys.CodecPrivateData:
                        case MkvInfoKeys.LanguageIETF:
                        case MkvInfoKeys.Name:
                        case MkvInfoKeys.EditionUID:
                        case MkvInfoKeys.ChapterUID:
                        case MkvInfoKeys.ChapterString:
                        case MkvInfoKeys.ChapterLanguage:
                        case MkvInfoKeys.ChapterLanguageIETF:
                            last = new(key, value);
                            break;
                        case MkvInfoKeys.Segment:
                        case MkvInfoKeys.EBML_Void:
                            if (!value.StartsWith("size "))
                            {
                                throw new FormatException("Expected size");
                            }
                            last = new(key, value.Substring("size ".Length));
                            break;
                        case MkvInfoKeys.Duration:
                        case MkvInfoKeys.DefaultDuration:
                        case MkvInfoKeys.ChapterTimeStart:
                        case MkvInfoKeys.ChapterTimeEnd:
                            value = value.Split()[0];
                            int dot = str.IndexOf('.');
                            string decimals = str.Substring(dot + 1);
                            if (decimals.Length > 7) // Limitation of timespan parsing
                            {
                                str = str.Substring(0, dot) + '.' + decimals.Substring(0, 7);
                            }
                            last = new(key, TimeSpan.Parse(str));
                            break;
                        case MkvInfoKeys.Date:
                            if (!value.EndsWith(" UTC"))
                            {
                                throw new FormatException("Expected UTC formatted date.");
                            }
                            last = new(key, DateTime.Parse(value.Substring(0, value.Length - " UTC".Length)));
                            break;
                        case MkvInfoKeys.SegmentUID:
                            var values = value.Split().Select(hex => Convert.ToInt32(hex, 16)).ToList();
                            last = new(key, values);
                            break;
                    }
                }
                if (last == null)
                {
                    switch (str)
                    {
                        case MkvInfoKeys.EBML_Head:
                        case MkvInfoKeys.SeekHead:
                        case MkvInfoKeys.SegmentInfo:
                        case MkvInfoKeys.VideoTrack:
                        case MkvInfoKeys.AudioTrack:
                        case MkvInfoKeys.ContentEncoding:
                        case MkvInfoKeys.ContentCompression:
                        case MkvInfoKeys.Cluster:
                        case MkvInfoKeys.Track:
                            last = new(str, new Dictionary<string, object?>());
                            break;
                        case string chapter when chapter.StartsWith(MkvInfoKeys.Chapter + " "):
                            //last = new(MkvInfoKeys.Chapter, chapter.Substring((MkvInfoKeys.Chapter + " ").Length));
                            last = new(str, new Dictionary<string, object?>());
                            break;
                        case MkvInfoKeys.Tracks:
                        case MkvInfoKeys.ContentEncodings:
                        case MkvInfoKeys.EditionEntry:
                        case MkvInfoKeys.Chapters:
                            last = new(str, new List<KeyValuePair<string, object?>>());
                            break;
                    }
                }
                if (last == null)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Unknown MkvInfo key: '{str}'");
                    Console.ResetColor();
                    last = new(str, new Dictionary<string, object?>());
                }

                // Get next line
                hasItem = itr.MoveNext();
            }

            if (last != null)
            {
                if (data is List<KeyValuePair<string, object?>> list)
                {
                    list.Add(last.Value);
                }
                else if (data is Dictionary<string, object?> dict)
                {

                    dict.Add(last.Value.Key, last.Value.Value);
                }
            }

            return false;
        }
    }
}
