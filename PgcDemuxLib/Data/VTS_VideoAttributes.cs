using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PgcDemuxLib.Data
{
    /// <summary>
    /// https://dvd.sourceforge.net/dvdinfo/ifo.html#vidatt
    /// </summary>
    public class VTS_VideoAttributes
    {
        const int Address = 0x200;

        [JsonInclude, JsonConverter(typeof(JsonStringEnumConverter))]
        public readonly CodingMode Encoding;

        [JsonInclude, JsonConverter(typeof(JsonStringEnumConverter))]
        public readonly VideoFormat Format;

        [JsonInclude, JsonConverter(typeof(JsonStringEnumConverter))]
        public readonly AspectRatio AspectRatio;

        [JsonInclude]
        public readonly bool AutoScanAllowed;

        [JsonInclude]
        public readonly bool AutoLetterboxAllowed;

        [JsonInclude, JsonConverter(typeof(JsonStringEnumConverter))]
        public readonly VideoResolution Resolution;

        [JsonInclude]
        public readonly bool Letterboxed;

        /// <summary>
        /// PAL only
        /// </summary>
        [JsonInclude, JsonConverter(typeof(JsonStringEnumConverter))]
        public readonly CameraType CameraType;

        internal VTS_VideoAttributes(byte[] file)
        {
            int temp = file[Address];

            Encoding = Util.ParseEnum<CodingMode>(temp >> 6 & 0b11);
            Format = Util.ParseEnum<VideoFormat>(temp >> 4 & 0b11);
            AspectRatio = Util.ParseEnum<AspectRatio>(temp >> 2 & 0b11);
            AutoScanAllowed = (temp >> 1 & 0b1) == 0;
            AutoLetterboxAllowed = (temp & 0b1) == 0;

            temp = file[Address + 1];

            Resolution = Util.ParseEnum<VideoResolution>(temp >> 3 & 0b111);
            Letterboxed = (temp >> 2 & 0b1) != 0;
            CameraType = Util.ParseEnum<CameraType>(temp & 0b1);
        }
    }

    public enum CodingMode
    {
        Mpeg1 = 0,
        Mpeg2 = 1
    }

    public enum VideoFormat
    {
        NTSC = 0,
        PAL = 1
    }

    public enum AspectRatio
    {
        _4_3 = 0,
        _16_9 = 3
    }

    public enum VideoResolution
    {
        NTSC_720x480_PAL_720x576 = 0,
        NTSC_704x480_PAL_704x576 = 1,
        NTSC_352x480_PAL_352x576 = 2,
        NTSC_352x240_PAL_352x288 = 3
    }

    public enum CameraType
    {
        Camera = 0,
        Film = 1
    }
}
