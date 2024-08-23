using libbluray.disc;
using libbluray.file;
using libbluray.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libbluray.bdnav {
    public static class mpls {

        const string SIG1 = "MPLS";
        const string module = "DBG_NAV";

        private static bool _parse_uo(BITSTREAM bits, BD_UO_MASK uo) {
            byte[] buf = new byte[8];
            bits.read_bytes(buf);
            return BD_UO_MASK.parse(buf, uo);
        }

        private static bool _parse_appinfo(BITSTREAM bits, MPLS_AI ai) {
            Int64 len;

            if (!bits.is_align(0x07))
            {
                Utils.BD_DEBUG(LogLevel.Critical, module, "_parse_appinfo: alignment error");
            }
            len = bits.read(32);

            if (bits.avail() < len * 8)
            {
                Utils.BD_DEBUG(LogLevel.Critical, module, "_parse_appinfo: unexpected end of file");
            }

            bits.skip(8);
            ai.playback_type = (byte)bits.read(8);
            if (ai.playback_type == 2 || ai.playback_type == 3) // TODO enum???
            {
                ai.playback_count = (UInt16)bits.read(16);
            } else
            {
                // reserved
                bits.skip(16);
            }
            _parse_uo(bits, ai.uo_mask);
            ai.random_access_flag = bits.read_bool();
            ai.audio_mix_flag = bits.read_bool();
            ai.lossless_bypass_flag = bits.read_bool();
            ai.mvc_base_view_r_flag = bits.read_bool();
            ai.sdr_conversion_notification_flag = bits.read_bool();

            return true;
        }

        private static bool _parse_header(BITSTREAM bits, MPLS_PL pl) {
            pl.type_indicator = SIG1;
            if (!bdmv.parse_header(bits, pl.type_indicator, out pl.type_indicator2))
            {
                return false;
            }

            if (bits.avail() < 5 * 32 + 160)
            {
                Utils.BD_DEBUG(LogLevel.Critical, module, "_parse_header: unexpected end of file");
                return false;
            }

            pl.list_pos = bits.read(32);
            pl.mark_pos = bits.read(32);
            pl.ext_pos = bits.read(32);

            // Skip 160 reserved bits
            bits.skip(160);

            _parse_appinfo(bits, pl.app_info);
            return true;
        }

        private static bool _parse_stream(BITSTREAM bits, MPLS_STREAM s) {
            Int32 len;
            Int64 pos;

            if (!bits.is_align(0x07))
            {
                Utils.BD_DEBUG(LogLevel.Critical, module, "_parse_stream: Stream alignment error");
            }
            len = (Int32)bits.read(8);
            pos = bits.pos() >> 3;

            s.stream_type = (byte)bits.read(8);
            switch(s.stream_type) 
            {
                case 1:
                    s.pid = (UInt16)bits.read(16);
                    break;
                case 2:
                    s.subpath_id = (byte)bits.read(8);
                    s.subclip_id = (byte)bits.read(8);
                    s.pid = (UInt16)bits.read(16);
                    break;
                case 3:
                case 4:
                    s.subpath_id = (byte)bits.read(8);
                    s.pid = (UInt16)bits.read(16);
                    break;
                default:
                    Utils.BD_DEBUG(LogLevel.Critical, module, $"unrecognized stream type {s.stream_type:X2}");
                    break;
            }

            if (bits.seek_byte(pos + len) < 0)
            {
                return false;
            }

            len = (Int32)bits.read(8);
            pos = bits.pos() >> 3;

            s.lang = "";
            s.coding_type = (StreamType)bits.read(8);
            switch(s.coding_type)
            {
                case StreamType.VIDEO_MPEG1:
                case StreamType.VIDEO_MPEG2:
                case StreamType.VIDEO_VC1:
                case StreamType.VIDEO_H264:
                case StreamType.VIDEO_HEVC:
                    s.simple_type = SimpleStreamType.Video;
                    s.video_format = (VideoFormat)bits.read(4);
                    s.video_rate = (VideoRate)bits.read(4);
                    if (s.coding_type == StreamType.VIDEO_HEVC)
                    {
                        s.dynamic_range_type = (byte)bits.read(4);
                        s.color_space = (ColorSpace)bits.read(4);
                        s.cr_flag = bits.read_bool();
                        s.hdr_plus_flag = bits.read_bool();
                    }
                    break;
                case StreamType.AUDIO_MPEG1:
                case StreamType.AUDIO_MPEG2:
                case StreamType.AUDIO_LPCM:
                case StreamType.AUDIO_AC3:
                case StreamType.AUDIO_DTS:
                case StreamType.AUDIO_TRUHD:
                case StreamType.AUDIO_AC3PLUS:
                case StreamType.AUDIO_DTSHD:
                case StreamType.AUDIO_DTSHD_MASTER:
                case (StreamType)0xa1: // TODO unknown type
                case (StreamType)0xa2: // TODO unknown type
                    s.simple_type = SimpleStreamType.Audio;
                    s.audio_format = (AudioFormat)bits.read(4);
                    s.audio_rate = (AudioRate)bits.read(4);
                    bits.read_string(out s.lang, 3);
                    break;
                case StreamType.SUB_PG:
                case StreamType.SUB_IG:
                    s.simple_type = SimpleStreamType.Text;
                    bits.read_string(out s.lang, 3);
                    break;
                case StreamType.SUB_TEXT:
                    s.simple_type = SimpleStreamType.Text;
                    s.char_code = (TextCharCode)bits.read(8);
                    bits.read_string(out s.lang, 3);
                    break;
                default:
                    s.simple_type = SimpleStreamType.Unknown;
                    Utils.BD_DEBUG(LogLevel.Critical, module, $"unrecognized coding type {s.coding_type:X2}");
                    break;
            }

            if (bits.seek_byte(pos + len) < 0)
            {
                return false;
            }

            return true;
        }

        private static bool _parse_stn(BITSTREAM bits, MPLS_STN stn) {
            Int32 len;
            Int64 pos;
            MPLS_STREAM[] ss = null;
            Int32 ii, jj;

            if (!bits.is_align(0x07))
            {
                Utils.BD_DEBUG(LogLevel.Critical, module, "_parse_stream: Stream alignment error");
            }

            // Skip STN len
            len = (Int32)bits.read(16);
            pos = bits.pos() >> 3;

            // Skip 2 reserved bytes
            bits.skip(16);

            stn.num_video = (byte)bits.read(8);
            stn.num_audio = (byte)bits.read(8);
            stn.num_pg = (byte)bits.read(8);
            stn.num_ig = (byte)bits.read(8);
            stn.num_secondary_audio = (byte)bits.read(8);
            stn.num_secondary_video = (byte)bits.read(8);
            stn.num_pip_pg = (byte)bits.read(8);
            stn.num_dv = (byte)bits.read(8);

            // 4 reserve bytes
            bits.skip(4 * 8);

            // Primary video streams
            ss = null;
            if (stn.num_video > 0)
            {
                ss = new MPLS_STREAM[stn.num_video];
                for(ii = 0; ii < stn.num_video; ii++)
                {
                    ss[ii] = new MPLS_STREAM();
                    if (!_parse_stream(bits, ss[ii]))
                    {
                        Utils.BD_DEBUG(LogLevel.Critical, module, "error parsing video entry");
                        return false;
                    }
                }
            }
            stn.video = ss;

            // Primary audio streams
            ss = null;
            if (stn.num_audio > 0)
            {
                ss = new MPLS_STREAM[stn.num_audio];
                for (ii = 0; ii < stn.num_audio; ii++)
                {
                    ss[ii] = new MPLS_STREAM();
                    if (!_parse_stream(bits, ss[ii]))
                    {
                        Utils.BD_DEBUG(LogLevel.Critical, module, "error parsing audio entry");
                        return false;
                    }
                }
            }
            stn.audio = ss;

            // Presentation Graphic streams
            ss = null;
            if (stn.num_pg > 0 || stn.num_pip_pg > 0)
            {
                ss = new MPLS_STREAM[stn.num_pg + stn.num_pip_pg];
                for(ii = 0; ii < (stn.num_pg + stn.num_pip_pg); ii++)
                {
                    ss[ii] = new MPLS_STREAM();
                    if (!_parse_stream(bits, ss[ii]))
                    {
                        Utils.BD_DEBUG(LogLevel.Critical, module, "error parsing audio entry");
                        return false;
                    }
                }
            }
            stn.pg = ss;

            // Interactive graphic streams
            ss = null;
            if (stn.num_ig > 0)
            {
                ss = new MPLS_STREAM[stn.num_ig];
                for(ii = 0; ii < stn.num_ig; ii++)
                {
                    ss[ii] = new MPLS_STREAM();
                    if (!_parse_stream(bits, ss[ii]))
                    {
                        Utils.BD_DEBUG(LogLevel.Critical, module, "error parsing ig entry");
                        return false;
                    }
                }
            }
            stn.ig = ss;

            // Secondary audio streams
            if (stn.num_secondary_audio > 0)
            {
                ss = new MPLS_STREAM[stn.num_secondary_audio];
                stn.secondary_audio = ss;
                for(ii = 0; ii < stn.num_secondary_audio; ii++)
                {
                    ss[ii] = new MPLS_STREAM();
                    if (!_parse_stream(bits, ss[ii]))
                    {
                        Utils.BD_DEBUG(LogLevel.Critical, module, "error parsing secondary audio entry");
                        return false;
                    }

                    // read secondary audio extra attributes
                    ss[ii].sa_num_primary_audio_ref = (byte)bits.read(8);
                    bits.skip(8);
                    if (ss[ii].sa_num_primary_audio_ref > 0)
                    {
                        ss[ii].sa_primary_audio_ref = new byte[ss[ii].sa_num_primary_audio_ref];
                        for (jj = 0; jj < ss[ii].sa_num_primary_audio_ref; jj++)
                        {
                            ss[ii].sa_primary_audio_ref[jj] = (byte)bits.read(8);
                        }
                        if (ss[ii].sa_num_primary_audio_ref % 2 != 0)
                        {
                            bits.skip(8);
                        }
                    }
                }
            }

            // Secondary video streams
            if (stn.num_secondary_video > 0)
            {
                ss = new MPLS_STREAM[stn.num_secondary_video];
                stn.secondary_video = ss;
                for(ii = 0; ii < stn.num_secondary_video; ii++)
                {
                    ss[ii] = new MPLS_STREAM();
                    if(!_parse_stream(bits, ss[ii]))
                    {
                        Utils.BD_DEBUG(LogLevel.Critical, module, "error parsing secondary video entry");
                        return false;
                    }

                    // read secondary video extra attributes
                    ss[ii].sv_num_secondary_audio_ref = (byte)bits.read(8);
                    bits.skip(8);
                    if (ss[ii].sv_num_secondary_audio_ref > 0)
                    {
                        ss[ii].sv_secondary_audio_ref = new byte[ss[ii].sv_num_secondary_audio_ref];
                        for(jj = 0; jj < ss[ii].sv_num_secondary_audio_ref; jj++)
                        {
                            ss[ii].sv_secondary_audio_ref[jj] = (byte)bits.read(8);
                        }
                        if (ss[ii].sv_num_secondary_audio_ref % 2 != 0)
                        {
                            bits.skip(8);
                        }
                    }
                    ss[ii].sv_num_pip_pg_ref = (byte)bits.read(8);
                    bits.skip(8);
                    if (ss[ii].sv_num_pip_pg_ref > 0)
                    {
                        ss[ii].sv_pip_pg_ref = new byte[ss[ii].sv_num_pip_pg_ref];
                        for(jj = 0; jj < ss[ii].sv_num_pip_pg_ref; jj++)
                        {
                            ss[ii].sv_pip_pg_ref[jj] = (byte)bits.read(8);
                        }
                        if (ss[ii].sv_num_pip_pg_ref % 2 != 0)
                        {
                            bits.skip(8);
                        }
                    }
                }
            }

            // Dolby vision enhancement layer streams
            ss = null;
            if (stn.num_dv > 0)
            {
                ss = new MPLS_STREAM[stn.num_dv];
                for(ii = 0; ii < stn.num_dv; ii++)
                {
                    ss[ii] = new MPLS_STREAM();
                    if (!_parse_stream(bits, ss[ii]))
                    {
                        Utils.BD_DEBUG(LogLevel.Critical, module, "error parsing dv entry");
                        return false;
                    }
                }
            }
            stn.dv = ss;

            if (bits.seek_byte(pos + len) < 0)
            {
                return false;
            }

            return true;
        }

        private static bool _parse_playitem(BITSTREAM bits, MPLS_PI pi) {
            Int32 len, ii;
            Int64 pos;
            string clip_id, coded_id;
            byte stc_id;

            if (!bits.is_align(0x07))
            {
                Utils.BD_DEBUG(LogLevel.Critical, module, "_parse_playitem: stream alignment error");
            }

            // playitem length
            len = (Int32)bits.read(16);
            pos = bits.pos() >> 3;

            if (len < 18)
            {
                Utils.BD_DEBUG(LogLevel.Critical, module, $"_parse_playitem: invalid length {len}");
                return false;
            }
            if (bits.avail() / 8 < len)
            {
                Utils.BD_DEBUG(LogLevel.Critical, module, "_parse_playitem: unexpected EOF");
                return false;
            }

            // Primary clip identifier
            bits.read_string(out clip_id, 5);

            bits.read_string(out coded_id, 4);
            if (coded_id != "M2TS" && coded_id != "FMTS")
            {
                Utils.BD_DEBUG(LogLevel.Critical, module, $"Incorrect CodecIdentifier ({coded_id})");
            }

            // Skip reserved 11 bits
            bits.skip(11);

            pi.is_multi_angle = bits.read_bool();

            pi.connection_condition = (byte)bits.read(4);
            if (pi.connection_condition != 0x01
                && pi.connection_condition != 0x05
                && pi.connection_condition != 0x06
            ) {
                Utils.BD_DEBUG(LogLevel.Critical, module, $"Unexpected connection condition {pi.connection_condition:X2}");
            }

            stc_id = (byte)bits.read(8);
            pi.in_time = bits.read(32);
            pi.out_time = bits.read(32);

            _parse_uo(bits, pi.uo_mask);
            pi.random_access_flag = bits.read_bool();
            bits.skip(7);
            pi.still_mode = (byte)bits.read(8);
            if (pi.still_mode == 0x01)
            {
                pi.still_time = (UInt16)bits.read(16);
            } else
            {
                bits.skip(16);
            }

            pi.angle_count = 1;
            if (pi.is_multi_angle)
            {
                pi.angle_count = (byte)bits.read(8);
                if (pi.angle_count < 1)
                {
                    pi.angle_count = 1;
                }
                bits.skip(6);
                pi.is_different_audio = bits.read_bool();
                pi.is_seamless_angle = bits.read_bool();
            }
            pi.clip = new MPLS_CLIP[pi.angle_count];
            pi.clip[0].clip_id = clip_id;
            pi.clip[0].codec_id = coded_id;
            pi.clip[0].stc_id = stc_id;
            for(ii = 1; ii < pi.angle_count; ii++)
            {
                bits.read_string(out pi.clip[ii].clip_id, 5);

                bits.read_string(out pi.clip[ii].codec_id, 4);
                if ((pi.clip[ii].codec_id != "M2TS") && (pi.clip[ii].codec_id != "FMTS"))
                {
                    Utils.BD_DEBUG(LogLevel.Critical, module, $"Incorrect CodecIdentifier ({pi.clip[ii].codec_id})");
                }
                pi.clip[ii].stc_id = (byte)bits.read(8);
            }
            if (!_parse_stn(bits, pi.stn))
            {
                return false;
            }

            // Seek past any unused items
            if (bits.seek_byte(pos + len) < 0)
            {
                return false;
            }

            return true;
        }

        private static bool _parse_subplayitem(BITSTREAM bits, MPLS_SUB_PI spi) {
            Int32 len, ii;
            Int64 pos;
            string clip_id, codec_id;
            byte stc_id;

            if (!bits.is_align(0x07))
            {
                Utils.BD_DEBUG(LogLevel.Critical, module, "_parse_subplayitem: alignment error");
            }

            // playitem length
            len = (Int32)bits.read(16);
            pos = bits.pos() >> 3;

            if (len < 24)
            {
                Utils.BD_DEBUG(LogLevel.Critical, module, $"_parse_subplayitem: invalid length {len}");
                return false;
            }

            if (bits.avail() / 8 < len)
            {
                Utils.BD_DEBUG(LogLevel.Critical, module, "_parse_subplayitem: unexpected EOF");
                return false;
            }

            // primary clip identifier
            bits.read_string(out clip_id, 5);

            bits.read_string(out codec_id, 4);
            if ((codec_id != "M2TS") && (codec_id != "FMTS"))
            {
                Utils.BD_DEBUG(LogLevel.Critical, module, $"Incorrect CodecIdentifier ({codec_id})");
            }

            bits.skip(27);

            spi.connection_condition = (byte)bits.read(4);

            if (spi.connection_condition != 0X01
                && spi.connection_condition != 0X05
                && spi.connection_condition != 0X06)
            {
                Utils.BD_DEBUG(LogLevel.Critical, module, $"Unexpected connection condition {spi.connection_condition:X2}");
            }
            spi.is_multi_clip = bits.read_bool();
            stc_id = (byte)bits.read(8);
            spi.in_time = bits.read(32);
            spi.out_time = bits.read(32);
            spi.sync_play_item_id = (UInt16)bits.read(16);
            spi.sync_pts = bits.read(32);
            spi.clip_count = 1;
            if (spi.is_multi_clip)
            {
                spi.clip_count = (byte)bits.read(8);
                if (spi.clip_count < 1)
                {
                    spi.clip_count = 1;
                }
            }
            spi.clip = new MPLS_CLIP[spi.clip_count];
            spi.clip[0] = new MPLS_CLIP();
            spi.clip[0].clip_id = clip_id;
            spi.clip[0].codec_id = codec_id;
            spi.clip[0].stc_id = stc_id;
            for(ii = 1; ii < spi.clip_count; ii++)
            {
                // Primary clip identifer
                bits.read_string(out spi.clip[ii].clip_id, 5);

                bits.read_string(out spi.clip[ii].codec_id, 4);
                if ((spi.clip[ii].codec_id != "M2TS") && (spi.clip[ii].codec_id != "FMTS"))
                {
                    Utils.BD_DEBUG(LogLevel.Critical, module, $"Incorrect CodecIdentifier ({spi.clip[ii].codec_id})");
                }
                spi.clip[ii].stc_id = (byte)bits.read(8);
            }

            // Seek to end of subpath
            if (bits.seek_byte(pos + len) < 0)
            {
                return false;
            }

            return true;
        }

        private static bool _parse_subpath(BITSTREAM bits, MPLS_SUB sp) {
            Int32 len, ii;
            Int64 pos;
            MPLS_SUB_PI[]? spi = null;

            if (!bits.is_align(0x07))
            {
                Utils.BD_DEBUG(LogLevel.Critical, module, "_parse_subpath: alignment error");
            }

            // PlayItem Length
            len = (Int32)bits.read(32);
            pos = bits.pos() >> 3;

            bits.skip(8);
            sp.type = (byte)bits.read(8);
            bits.skip(15);
            sp.is_repeat = bits.read_bool();
            bits.skip(8);
            sp.sub_playitem_count = (byte)bits.read(8);

            if (sp.sub_playitem_count > 0)
            {
                spi = new MPLS_SUB_PI[sp.sub_playitem_count];
                sp.sub_play_item = spi;
                for(ii = 0; ii < sp.sub_playitem_count; ii++)
                {
                    spi[ii] = new MPLS_SUB_PI();
                    if (!_parse_subplayitem(bits, spi[ii]))
                    {
                        Utils.BD_DEBUG(LogLevel.Critical, module, "error parsing sub play item");
                        return false;
                    }
                }
            }

            // Seek to end of subpath
            if (bits.seek_byte(pos + len) < 0)
            {
                return false;
            }

            return true;
        }

        private static bool _parse_playlistmark(BITSTREAM bits, MPLS_PL pl) {
            Int64 len;
            Int32 ii;
            MPLS_PLM[]? plm = null;

            if (bits.seek_byte(pl.mark_pos) < 0)
            {
                return false;
            }

            // Length field
            len = bits.read(32);

            if (bits.avail() / 8 < len)
            {
                Utils.BD_DEBUG(LogLevel.Critical, module, "_parse_playlistmark: unexpected EOF");
                return false;
            }

            // Then get the number of marks
            pl.mark_count = (UInt16)bits.read(16);

            if (bits.avail() / (8 * 14) < pl.mark_count)
            {
                Utils.BD_DEBUG(LogLevel.Critical, module, "_parse_playlistmark: unextected EOF");
                return false;
            }

            plm = new MPLS_PLM[pl.mark_count];
            for(ii = 0; ii < pl.mark_count; ii++)
            {
                plm[ii] = new();
                bits.skip(8); // reserved
                plm[ii].mark_type = (byte)bits.read(8);
                plm[ii].play_item_ref = (UInt16)bits.read(16);
                plm[ii].time = bits.read(32);
                plm[ii].entry_es_pid = (UInt16)bits.read(16);
                plm[ii].duration = bits.read(32);
            }
            pl.play_mark = plm;
            return true;
        }

        private static bool _parse_playlist(BITSTREAM bits, MPLS_PL pl) {
            Int64 len;
            Int32 ii;
            MPLS_PI[]? pi = null;
            MPLS_SUB[]? sub_path = null;

            if (bits.seek_byte(pl.list_pos) < 0)
            {
                return false;
            }

            // playlist length
            len = bits.read(32);

            if (bits.avail() < len * 8)
            {
                Utils.BD_DEBUG(LogLevel.Critical, module, "_parse_playlist: unexpected end of file");
                return false;
            }

            // Skip reserved bytes
            bits.skip(16);

            pl.list_count = (UInt16)bits.read(16);
            pl.sub_count = (UInt16)bits.read(16);

            if (pl.list_count > 0)
            {
                pi = new MPLS_PI[pl.list_count];
                pl.play_item = pi;
                for(ii = 0; ii < pl.list_count; ii++)
                {
                    pi[ii] = new MPLS_PI();
                    if (!_parse_playitem(bits, pi[ii]))
                    {
                        Utils.BD_DEBUG(LogLevel.Critical, module, "error parsing play list item");
                        return false;
                    }
                }
            }

            if (pl.sub_count > 0)
            {
                sub_path = new MPLS_SUB[pl.sub_count];
                pl.sub_path = sub_path;
                for (ii = 0; ii < pl.sub_count; ii++)
                {
                    sub_path[ii] = new MPLS_SUB();
                    if (!_parse_subpath(bits, sub_path[ii]))
                    {
                        Utils.BD_DEBUG(LogLevel.Critical, module, "error parsing subpath");
                        return false;
                    }
                }
            }

            return true;
        }

        private static bool _parse_pip_data(BITSTREAM bits, MPLS_PIP_METADATA block) {
            MPLS_PIP_DATA[]? data = null;
            UInt32 ii;

            UInt16 entries = (UInt16)bits.read(16);
            if (entries < 1)
            {
                return true;
            }

            data = new MPLS_PIP_DATA[entries];
            for (ii = 0; ii < entries; ii++)
            {
                data[ii].time = bits.read(32);
                data[ii].xpos = (UInt16)bits.read(12);
                data[ii].ypos = (UInt16)bits.read(12);
                data[ii].scale_factor = (byte)bits.read(4);
                bits.skip(4);
            }

            block.data_count = entries;
            block.data = data;

            return true;
        }

        private static bool _parse_pip_metadata_block(BITSTREAM bits, UInt32 start_address, MPLS_PIP_METADATA data) {
            UInt32 data_address;
            bool result;
            Int64 pos;

            data.clip_ref = (UInt16)bits.read(16);
            data.secondary_video_ref = (byte)bits.read(8);
            bits.skip(8);
            data.timeline_type = (byte)bits.read(4);
            data.luma_key_flag = bits.read_bool();
            data.trick_play_flag = bits.read_bool();
            bits.skip(10);
            if (data.luma_key_flag)
            {
                bits.skip(8);
                data.upper_limit_luma_key = (byte)bits.read(8);
            } else
            {
                bits.skip(16);
            }
            bits.skip(16);

            data_address = bits.read(32);

            pos = bits.pos() / 8;
            if (bits.seek_byte(start_address + data_address) < 0)
            {
                return false;
            }
            result = _parse_pip_data(bits, data);
            if (bits.seek_byte(pos) < 0)
            {
                return false;
            }

            return result;
        }

        private static bool _parse_pip_metadata_extension(BITSTREAM bits, MPLS_PL pl) {
            MPLS_PIP_METADATA[]? data = null;
            Int32 ii;

            UInt32 start_address = (UInt32)bits.pos() / 8;
            UInt32 len = bits.read(32);
            UInt16 entries = (UInt16)bits.read(16);

            if (len < 1 || entries < 1)
            {
                return false;
            }

            data = new MPLS_PIP_METADATA[entries];
            for(ii = 0; ii < entries; ii++)
            {
                data[ii] = new MPLS_PIP_METADATA();
                if (!_parse_pip_metadata_block(bits, start_address, data[ii]))
                {
                    Utils.BD_DEBUG(LogLevel.Critical, module, "error parsing pip metadata extension");
                    return false;
                }
            }

            pl.ext_pip_data_count = entries;
            pl.ext_pip_data = data;

            return true;
        }

        private static bool _parse_subpath_extension(BITSTREAM bits, MPLS_PL pl) {
            MPLS_SUB[]? sub_path = null;
            Int32 ii;

            UInt32 len = bits.read(32);
            UInt16 sub_count = (UInt16)bits.read(16);

            if (len < 1 || sub_count < 1)
            {
                return false;
            }

            sub_path = new MPLS_SUB[sub_count];
            for(ii = 0; ii < sub_count; ii++)
            {
                sub_path[ii] = new MPLS_SUB();
                if (!_parse_subpath(bits, sub_path[ii])) {
                    Utils.BD_DEBUG(LogLevel.Critical, module, "error parsing extension subpath");
                    return false;
                }
            }
            pl.ext_sub_path = sub_path;
            pl.ext_sub_count = sub_count;

            return true;
        }

        private static bool _parse_static_metadata(BITSTREAM bits, MPLS_STATIC_METADATA data) {
            Int32 ii;
            if (bits.avail() < 28 * 8)
            {
                Utils.BD_DEBUG(LogLevel.Critical, module, "_parse_static_metadata: unexpected end of file");
                return false;
            }

            data.dynamic_range_type = (byte)bits.read(4);
            bits.skip(4);
            bits.skip(24);
            for(ii = 0; ii < 3; ii++)
            {
                data.display_primaries_x[ii] = (UInt16)bits.read(16);
                data.display_primaries_y[ii] = (UInt16)bits.read(16);
            }
            data.white_point_x = (UInt16)bits.read(16);
            data.white_point_y = (UInt16)bits.read(16);
            data.max_display_mastering_luminance = (UInt16)bits.read(16);
            data.min_display_mastering_luminance = (UInt16)bits.read(16);
            data.max_CLL = (UInt16)bits.read(16);
            data.max_FALL = (UInt16)bits.read(16);

            return true;
        }

        private static bool _parse_static_metadata_extension(BITSTREAM bits, MPLS_PL pl) {
            MPLS_STATIC_METADATA[]? static_metadata = null;
            UInt32 len;
            Int32 ii;

            len = bits.read(32);
            if (len < 32) // at least one static metadata entry
            {
                return false;
            }
            if (bits.avail() < len * 8)
            {
                Utils.BD_DEBUG(LogLevel.Critical, module, "_parse_static_metadata_extension: unexpected end of file");
                return false;
            }

            byte sm_count = (byte)bits.read(8);
            if (sm_count < 1)
            {
                return false;
            }
            bits.skip(24);

            static_metadata = new MPLS_STATIC_METADATA[sm_count];
            for(ii = 0; ii < sm_count; ii++)
            {
                static_metadata[ii] = new MPLS_STATIC_METADATA();
                if (!_parse_static_metadata(bits, static_metadata[ii]))
                {
                    Utils.BD_DEBUG(LogLevel.Critical, module, "error parsing static metadata extension");
                    return false;
                }
            }
            pl.ext_static_metadata = static_metadata;
            pl.ext_static_metadata_count = sm_count;

            return true;
        }

        private static bool _parse_mpls_extension(BITSTREAM bits, Int32 id1, Int32 id2, object handle) {
            MPLS_PL? pl = (MPLS_PL)handle;

            if (id1 == 1)
            {
                if (id2 == 1)
                {
                    // Pip metadata extension
                    return _parse_pip_metadata_extension(bits, pl);
                } 
            }

            if (id1 == 2)
            {
                if (id2 == 1)
                {
                    return false;
                }
                if (id2 == 2)
                {
                    // subpath entries extension
                    return _parse_subpath_extension(bits, pl);
                }
            }

            if (id2 == 3)
            {
                if (id2 == 5)
                {
                    // Static metadata extension
                    return _parse_static_metadata_extension(bits, pl);
                }
            }

            Utils.BD_DEBUG(LogLevel.Critical, module, $"_parse_mpls_extension(): unhandled extension {id1}.{id2}");
            return false;
        }

        private static MPLS_PL? _mpls_parse(BD_FILE_H fp) {
            BITSTREAM bits = new();
            MPLS_PL? pl = null;

            if (bits.init(fp) < 0)
            {
                Utils.BD_DEBUG(LogLevel.Warning, module, "?????.mpls: read error");
                return null;
            }

            pl = new MPLS_PL();
            if (!_parse_header(bits, pl))
            {
                return null;
            }
            if (!_parse_playlist(bits, pl))
            {
                return null;
            }
            if (!_parse_playlistmark(bits, pl))
            {
                return null;
            }
            if (pl.ext_pos > 0)
            {
                bdmv.parse_extension_data(bits, (int)pl.ext_pos, _parse_mpls_extension, pl);
            }

            return pl;
        }

        public static MPLS_PL? parse(string path) {
            MPLS_PL? pl = null;
            BD_FILE_H? fp = null;

            fp = file_win32.OpenFile(path, FileMode.Open);
            if (fp == null)
            {
                Utils.BD_DEBUG(LogLevel.Critical, module, $"Failed to open {path}");
                return null;
            }

            pl = _mpls_parse(fp);
            fp.close();
            return pl;
        }

        private static MPLS_PL? _mpls_get(BD_DISC disc, string dir, string file) {
            MPLS_PL? pl = null;
            BD_FILE_H? fp = null;

            fp = disc.open_file(dir, file);
            if (fp == null)
            {
                return null;
            }

            pl = _mpls_parse(fp);
            fp.close();
            return pl;
        }

        public static MPLS_PL? get(BD_DISC disc, string file) {
            MPLS_PL? pl = null;

            pl = _mpls_get(disc, Path.Combine("BDMV", "PLAYLIST"), file);
            if (pl != null)
            {
                return pl;
            }

            // if failed, try backup file
            pl = _mpls_get(disc, Path.Combine("BDMV", "BACKUP", "PLAYLIST"), file);
            return pl;
        }

    }
}
