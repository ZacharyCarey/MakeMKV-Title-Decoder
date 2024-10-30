using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PgcDemuxLib
{
    /// <summary>
    /// https://dvd.sourceforge.net/dvdinfo/ifo.html#vidatt
    /// </summary>
    public class VTS_VideoAttributes
    {
        const int Address = 0x200;

        public readonly CodingMode Encoding;
        public readonly VideoFormat Format;
        public readonly AspectRatio AspectRatio;
        public readonly bool AutoScanAllowed;
        public readonly bool AutoLetterboxAllowed;
        public readonly VideoResolution Resolution;
        public readonly bool Letterboxed;

        /// <summary>
        /// PAL only
        /// </summary>
        public readonly CameraType CameraType;

        internal VTS_VideoAttributes(byte[] file)
        {
            int temp = file[Address];

            this.Encoding = Util.ParseEnum<CodingMode>((temp >> 6) & 0b11);
            this.Format = Util.ParseEnum<VideoFormat>((temp >> 4) & 0b11);
            this.AspectRatio = Util.ParseEnum<AspectRatio>((temp >> 2) & 0b11);
            this.AutoScanAllowed = ((temp >> 1) & 0b1) == 0;
            this.AutoLetterboxAllowed = (temp & 0b1) == 0;

            temp = file[Address + 1];

            this.Resolution = Util.ParseEnum<VideoResolution>((temp >> 3) & 0b111);
            this.Letterboxed = ((temp >> 2) & 0b1) != 0;
            this.CameraType = Util.ParseEnum<CameraType>(temp & 0b1);
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
