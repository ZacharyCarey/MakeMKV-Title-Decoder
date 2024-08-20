using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.Data.MkvToolNix
{
    internal static class MkvInfoKeys
    {

        public const string EBML_Version = "EBML version";
        public const string EBML_ReadVersion = "EBML read version";
        public const string Max_EBML_IdLength = "Maximum EBML ID length";
        public const string Max_EBML_SizeLength = "Maximum EBML size length";
        public const string DocumentTypeVersion = "Document type version";
        public const string DocumentTypeReadVersion = "Document type read version";
        public const string TimestampScale = "Timestamp scale";
        public const string TrackNumber = "Track number";
        public const string PixelWidth = "Pixel width";
        public const string PixelHeight = "Pixel height";
        public const string DisplayWidth = "Display width";
        public const string DisplayHeight = "Display height";
        public const string SamplingFrequency = "Sampling frequency";
        public const string Channels = "Channels";
        public const string BitDepth = "Bit depth";

        public const string Flag_DefaultTrack = "\"Default track\" flag";
        public const string Flag_Lacing = "\"Lacing\" flag";
        public const string Flag_Commentary = "\"Commentary\" flag";
        public const string Flag_EditionHidden = "Edition flag hidden";
        public const string Flag_EditionDefault = "Edition flag default";
        public const string Flag_ChapterHidden = "Chapter flag hidden";
        public const string Flag_ChapterEnabled = "Chapter flag enabled";

        public const string DocumentType = "Document type";
        public const string MultiplexingApplication = "Multiplexing application";
        public const string WritingApplication = "Writing application";
        public const string Title = "Title";
        public const string TrackUID = "Track UID";
        public const string TrackType = "Track type";
        public const string CodecID = "Codec ID";
        public const string CodecPrivateData = "Codec's private data";
        public const string LanguageIETF = "Language (IETF BCP 47)";
        public const string Name = "Name";
        public const string EditionUID = "Edition UID";
        public const string ChapterUID = "Chapter UID";
        public const string ChapterString = "Chapter string";
        public const string ChapterLanguage = "Chapter language";
        public const string ChapterLanguageIETF = "Chapter language (IETF BCP 47)";

        public const string Segment = "Segment";
        public const string EBML_Void = "EBML void";

        public const string Duration = "Duration";
        public const string DefaultDuration = "Default duration";
        public const string ChapterTimeStart = "Chapter time start";
        public const string ChapterTimeEnd = "Chapter time end";

        public const string Date = "Date";

        public const string SegmentUID = "Segment UID";

        public const string EBML_Head = "EBML head";
        public const string SeekHead = "Seek head (subentries will be skipped)";
        public const string SegmentInfo = "Segment information";
        public const string Tracks = "Tracks";
        public const string Track = "Track";
        public const string VideoTrack = "Video track";
        public const string AudioTrack = "Audio track";
        public const string ContentEncodings = "Content encodings";
        public const string ContentEncoding = "Content encoding";
        public const string ContentCompression = "Content compression";
        public const string Chapters = "Chapters";
        public const string EditionEntry = "Edition entry";
        public const string Cluster = "Cluster";
        public const string Chapter = "Chapter";
    }
}
