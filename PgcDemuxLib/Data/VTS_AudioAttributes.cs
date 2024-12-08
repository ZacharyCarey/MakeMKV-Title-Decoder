using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PgcDemuxLib.Data
{
    public struct VTS_AudioAttributes
    {

        [JsonInclude, JsonConverter(typeof(JsonStringEnumConverter))]
        public readonly AudioEncoding CodingMode;

        [JsonInclude]
        public readonly bool MultichannelExtensionPresent;

        /// <summary>
        /// Iso 639-1
        /// </summary>
        [JsonInclude]
        public readonly string? LanguageCode;

        [JsonInclude, JsonConverter(typeof(JsonStringEnumConverter))]
        public readonly AudioMode Mode;

        /// <summary>
        /// Only valid when <see cref="CodingMode"/> equals <see cref="AudioEncoding.Mpeg1"/> or <see cref="AudioEncoding.Mpeg2ext"/>
        /// </summary>
        [JsonInclude]
        public readonly bool HasDRC;

        /// <summary>
        /// Only valid when <see cref="CodingMode"/> equals <see cref="AudioEncoding.LPCM"/>
        /// </summary>
        [JsonInclude, JsonConverter(typeof(JsonStringEnumConverter))]
        public readonly LpcmQuantization Quantization;

        /// <summary>
        /// In bits per secons
        /// </summary>
        [JsonInclude]
        public const int SampleRate = 48000;

        [JsonInclude]
        public readonly int Channels;

        [JsonInclude, JsonConverter(typeof(JsonStringEnumConverter))]
        public readonly CodeExtension CodeExtension;

        [JsonInclude]
        public readonly bool DolbySurroundSupport;

        internal VTS_AudioAttributes(byte[] file, int addr)
        {
            CodingMode = Util.ParseEnum<AudioEncoding>((file[addr] >> 5) & 0b111);
            MultichannelExtensionPresent = ((file[addr] >> 4) & 0b1) == 1;

            bool hasLanguageCode = ((file[addr] >> 2) & 0b11) == 1;
            if (hasLanguageCode)
            {
                LanguageCode = file.GetString(addr + 2, 2);
            } else
            {
                LanguageCode = null;
            }

            Mode = Util.ParseEnum<AudioMode>(file[addr] & 0b11);

            int temp = (file[addr + 1] >> 6) & 0b11;
            if (CodingMode == AudioEncoding.Mpeg1 || CodingMode == AudioEncoding.Mpeg2ext)
            {
                HasDRC = (temp == 1);
            } else
            {
                HasDRC = false;
            }

            if (CodingMode == AudioEncoding.LPCM)
            {
                Quantization = Util.ParseEnum<LpcmQuantization>(temp);
            } else
            {
                Quantization = LpcmQuantization.None;
            }

            int sampleRate = (file[addr + 1] >> 4) & 0b11;
            if (sampleRate != 0)
            {
                throw new Exception($"Unknown sample rate '{sampleRate}'");
            }

            this.Channels = (file[addr + 1] & 0b111) + 1;

            if (!Util.TryParseEnum(file[addr + 5], out CodeExtension))
            {
                CodeExtension = CodeExtension.Unspecified;
            }

            if (Mode == AudioMode.Surround)
            {
                DolbySurroundSupport = ((file[addr + 7] >> 3) & 0b1) == 1;
            } else
            {
                DolbySurroundSupport = false;
            }
        }

    }

    public enum AudioEncoding
    {
        AC3 = 0,
        Mpeg1 = 2,
        Mpeg2ext = 3,
        LPCM = 4,
        DTS = 6
    }

    public enum AudioMode
    {
        Unspecified = 0,
        Karaoke = 1,
        Surround = 2
    }

    public enum LpcmQuantization
    {
        _16bps = 0,
        _20bps = 1,
        _24bps = 2,
        None = 0xFF
    }

    public enum CodeExtension
    {
        Unspecified = 0,
        Normal = 1,
        VisuallyImpaired = 2,
        DirectorsComments = 3,
        AlternateDirectorsComments = 4
    }
}
