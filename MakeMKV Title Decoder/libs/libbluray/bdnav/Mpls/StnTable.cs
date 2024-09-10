using libbluray.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace libbluray.bdnav.Mpls {
    public class StnTable {
        const string module = "libbluray.bdnav.Mpls.StnTable";

        public StreamEntry[] PrimaryVideoStreams = Array.Empty<StreamEntry>();
        public StreamEntry[] PrimaryAudioStreams = Array.Empty<StreamEntry>();
        public StreamEntry[] PrimaryPresentationGraphicStreams = Array.Empty<StreamEntry>();
        public StreamEntry[] PrimaryInteractiveGraphicsStreams = Array.Empty<StreamEntry>();
        public StreamEntry[] SecondaryAudioStreams = Array.Empty<StreamEntry>();
        public StreamEntry[] SecondaryVideoStreams = Array.Empty<StreamEntry>();
        public StreamEntry[] SecondaryPresentationGraphicStreams = Array.Empty<StreamEntry>();

        /// <summary>
        /// Dolby vision enhancement layer
        /// </summary>
        public StreamEntry[] DolbyVisionEnhancementStreams = Array.Empty<StreamEntry>();

        private static bool ParseStreams(BitStream bits, Int32 count, out StreamEntry[] result, string error_msg) {
            if (count < 0)
            {
                result = Array.Empty<StreamEntry>();
                return true;
            }

            StreamEntry[] streams = new StreamEntry[count];
            for (int i = 0; i < count; i++)
            {
                streams[i] = new StreamEntry();
                if (!streams[i].Parse(bits))
                {
                    Utils.BD_DEBUG(LogLevel.Critical, module, error_msg);
                    result = Array.Empty<StreamEntry>();
                    return false;
                }
            }

            result = streams;
            return true;
        }

        public bool Parse(BitStream bits) {
            if (!bits.IsAligned())
            {
                Utils.BD_DEBUG(LogLevel.Warning, module, "_parse_stream: Stream alignment error");
            }

            Int32 len = bits.Read<UInt16>();
            Int64 pos = bits.Position;

            // Skip 2 reserved bytes
            bits.Skip(16);

            Int32 numPrimaryVideo = bits.Read<byte>();
            Int32 numPrimaryAudio = bits.Read<byte>();
            Int32 numPrimaryPG = bits.Read<byte>();
            Int32 numPrimaryIG = bits.Read<byte>();
            Int32 numSecondaryAudio = bits.Read<byte>();
            Int32 numSecondaryVideo = bits.Read<byte>();
            Int32 numSecondaryPG = bits.Read<byte>();
            Int32 numDV = bits.Read<byte>();

            // 4 reserve bytes
            bits.Skip(32);

            // Primary video streams
            if (!ParseStreams(bits, numPrimaryVideo, out this.PrimaryVideoStreams, "error parsing primary video entry"))
            {
                return false;
            }

            // Primary audio streams
            if (!ParseStreams(bits, numPrimaryVideo, out this.PrimaryAudioStreams, "error parsing primary audio entry"))
            {
                return false;
            }

            // Presentation Graphic streams
            if (!ParseStreams(bits, numPrimaryPG, out this.PrimaryPresentationGraphicStreams, "error parsing primary presentation graphic entry"))
            {
                return false;
            }

            if (!ParseStreams(bits, numSecondaryPG, out this.SecondaryPresentationGraphicStreams, "error parsing secondary presentation graphic entry"))
            {
                return false;
            }

            // Interactive graphic streams
            if (!ParseStreams(bits, numPrimaryIG, out this.PrimaryInteractiveGraphicsStreams, "error parsing primary interactive graphic entry"))
            {
                return false;
            }

            // Secondary audio streams
            if (!ParseStreams(bits, numSecondaryAudio, out this.SecondaryAudioStreams, "error parsing secondary audio entry"))
            {
                return false;
            }
            // TODO additional data???
            /*if (stn.num_secondary_audio > 0)
            {
                ss = new StreamEntry[stn.num_secondary_audio];
                stn.secondary_audio = ss;
                for (ii = 0; ii < stn.num_secondary_audio; ii++)
                {
                    ss[ii] = new StreamEntry();
                    if (!ss[ii].Parse(bits))
                    {
                        Utils.BD_DEBUG(LogLevel.Critical, module, "error parsing secondary audio entry");
                        return false;
                    }

                    // read secondary audio extra attributes
                    ss[ii].sa_num_primary_audio_ref = (byte)bits.read_bits(8);
                    bits.skip_bits(8);
                    if (ss[ii].sa_num_primary_audio_ref > 0)
                    {
                        ss[ii].sa_primary_audio_ref = new byte[ss[ii].sa_num_primary_audio_ref];
                        for (jj = 0; jj < ss[ii].sa_num_primary_audio_ref; jj++)
                        {
                            ss[ii].sa_primary_audio_ref[jj] = (byte)bits.read_bits(8);
                        }
                        if (ss[ii].sa_num_primary_audio_ref % 2 != 0)
                        {
                            bits.skip_bits(8);
                        }
                    }
                }
            }*/

            // Secondary video streams
            if (!ParseStreams(bits, numSecondaryVideo, out this.SecondaryVideoStreams, "error parsing secondary video entry"))
            {
                return false;
            }
            // TODO additional data????
            /*if (stn.num_secondary_video > 0)
            {
                ss = new StreamEntry[stn.num_secondary_video];
                stn.secondary_video = ss;
                for (ii = 0; ii < stn.num_secondary_video; ii++)
                {
                    ss[ii] = new StreamEntry();
                    if (!ss[ii].Parse(bits))
                    {
                        Utils.BD_DEBUG(LogLevel.Critical, module, "error parsing secondary video entry");
                        return false;
                    }

                    // read secondary video extra attributes
                    ss[ii].sv_num_secondary_audio_ref = (byte)bits.read_bits(8);
                    bits.skip_bits(8);
                    if (ss[ii].sv_num_secondary_audio_ref > 0)
                    {
                        ss[ii].sv_secondary_audio_ref = new byte[ss[ii].sv_num_secondary_audio_ref];
                        for (jj = 0; jj < ss[ii].sv_num_secondary_audio_ref; jj++)
                        {
                            ss[ii].sv_secondary_audio_ref[jj] = (byte)bits.read_bits(8);
                        }
                        if (ss[ii].sv_num_secondary_audio_ref % 2 != 0)
                        {
                            bits.skip_bits(8);
                        }
                    }
                    ss[ii].sv_num_pip_pg_ref = (byte)bits.read_bits(8);
                    bits.skip_bits(8);
                    if (ss[ii].sv_num_pip_pg_ref > 0)
                    {
                        ss[ii].sv_pip_pg_ref = new byte[ss[ii].sv_num_pip_pg_ref];
                        for (jj = 0; jj < ss[ii].sv_num_pip_pg_ref; jj++)
                        {
                            ss[ii].sv_pip_pg_ref[jj] = (byte)bits.read_bits(8);
                        }
                        if (ss[ii].sv_num_pip_pg_ref % 2 != 0)
                        {
                            bits.skip_bits(8);
                        }
                    }
                }
            }*/

            // Dolby vision enhancement layer streams
            if (!ParseStreams(bits, numDV, out this.DolbyVisionEnhancementStreams, "error parsing dolby vision entry"))
            {
                return false;
            }

            bits.Seek(pos + len * 8);
            if (bits.EOF)
            {
                return false;
            }

            return true;
        }
    }
}
