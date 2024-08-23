using libbluray.disc;
using libbluray.file;
using libbluray.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace libbluray.bdnav {
    public class CLPI_CL {
        public string type_indicator = ""; // Length 4
        public string type_indicator2 = ""; // Length 4
        public UInt32 sequence_info_start_addr;
        public UInt32 program_info_start_addr;
        public UInt32 cpi_start_addr;
        public UInt32 clip_mark_start_addr;
        public UInt32 ext_data_start_addr;
        public CLPI_CLIP_INFO clip = new();
        public CLPI_SEQ_INFO sequence = new();
        public CLPI_PROG_INFO program = new();
        public CLPI_CPI cpi = new();
        // skip clip mark & extension data

        // extensions for 3D

        public CLPI_EXTENT_START extent_start = new(); /* extent start points (.ssif interleaving) */
        public CLPI_PROG_INFO program_ss = new();
        public CLPI_CPI cpi_ss = new();

        private const string SIG1 = "HDMV";
        private const string module = "DBG_NAV";
        private const string module2 = "DBG_HDMV";

        private static bool _parse_stream_attr(BITSTREAM bits, CLPI_PROG_STREAM ss) {
            Int64 pos;
            Int32 len;

            if (!bits.is_align(0x07))
            {
                Utils.BD_DEBUG(LogLevel.Critical, module, "_parse_stream_attr(): Stream alignment error");
            }

            len = (int)bits.read(8);
            pos = bits.pos() >> 3;

            ss.lang = "";
            ss.isrc = new byte[12];
            ss.coding_type = (StreamType)bits.read(8);
            switch(ss.coding_type)
            {
                case StreamType.VIDEO_MPEG1:
                case StreamType.VIDEO_MPEG2:
                case StreamType.VIDEO_VC1:
                case StreamType.VIDEO_H264:
                case (StreamType)0x20: // TODO unknown type
                case StreamType.VIDEO_HEVC:
                    ss.Type = SimpleStreamType.Video;
                    ss.video_format = (VideoFormat)bits.read(4);
                    ss.video_rate = (VideoRate)bits.read(4);
                    ss.aspect = (AspectRatio)bits.read(4);
                    bits.skip(2);
                    ss.oc_flag = bits.read_bool();
                    if (ss.coding_type == StreamType.VIDEO_HEVC)
                    {
                        ss.cr_flag = bits.read_bool();
                        ss.dynamic_range_type = (byte)bits.read(4);
                        ss.color_space = (ColorSpace)bits.read(4);
                        ss.hdr_plus_flag = bits.read_bool();
                        bits.skip(7);
                    } else
                    {
                        bits.skip(17);
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
                case (StreamType)0xA1: // TODO unknown type
                case (StreamType)0xA2: // TODO unknown type
                    ss.Type = SimpleStreamType.Audio;
                    ss.audio_format = (AudioFormat)bits.read(4);
                    ss.audio_rate = (AudioRate)bits.read(4);
                    bits.read_string(out ss.lang, 3);
                    break;

                case StreamType.SUB_PG:
                case StreamType.SUB_IG:
                case (StreamType)0xA0: // TODO unknown type
                    ss.Type = SimpleStreamType.Text;
                    bits.read_string(out ss.lang, 3);
                    bits.skip(8);
                    break;

                case StreamType.SUB_TEXT:
                    ss.Type = SimpleStreamType.Text;
                    ss.char_code = (TextCharCode)bits.read(8);
                    bits.read_string(out ss.lang, 3);
                    break;

                default:
                    Utils.BD_DEBUG(LogLevel.Critical, module, $"_parse_stream_attr(): unrecognized coding type {ss.coding_type:X2}");
                    break;
            }

            bits.read_bytes(ss.isrc);

            // Skip over any padding
            if (bits.seek_byte(pos + len) < 0)
            {
                return false;
            }
            return true;
        }

        private static bool _parse_header(BITSTREAM bits, CLPI_CL cl) {
            cl.type_indicator = SIG1;
            if (!bdmv.parse_header(bits, cl.type_indicator, out cl.type_indicator2))
            {
                return false;
            }

            if (bits.avail() < 5 * 32)
            {
                Utils.BD_DEBUG(LogLevel.Critical, module, "_parse_header: unexpected end of file");
                return false;
            }

            cl.sequence_info_start_addr = bits.read(32);
            cl.program_info_start_addr = bits.read(32);
            cl.cpi_start_addr = bits.read(32);
            cl.clip_mark_start_addr = bits.read(32);
            cl.ext_data_start_addr = bits.read(32);

            return true;
        }

        private static bool _parse_clipinfo(BITSTREAM bits, CLPI_CL cl) {
            Int64 pos;
            int len;
            int ii;

            if (bits.seek_byte(40) < 0)
            {
                return false;
            }

            bits.skip(32); // clipinfo len
            bits.skip(16); // reserved
            cl.clip.clip_stream_type = (byte)bits.read(8);
            cl.clip.application_type = (byte)bits.read(8);
            bits.skip(31); // skip reserved 31 bits
            cl.clip.is_atc_delta = bits.read_bool();
            cl.clip.ts_recording_rate = bits.read(32);
            cl.clip.num_source_packets = bits.read(32);

            // reserved 128 bytes
            bits.skip(128 * 8);

            // ts type info block
            len = (Int16)bits.read(16);
            pos = bits.pos() >> 3;
            if (len != 0)
            {
                cl.clip.ts_type_info.validity = (byte)bits.read(8);
                bits.read_string(out cl.clip.ts_type_info.format_id, 4);
                // Seek past the stuff we dont know anything about
                if (bits.seek_byte(pos + len) < 0)
                {
                    return false;
                }
            }
            if (cl.clip.is_atc_delta)
            {
                bits.skip(8); // reserved
                cl.clip.atc_delta_count = (byte)bits.read(8);
                cl.clip.atc_delta = new CLPI_ATC_DELTA[cl.clip.atc_delta_count];
                for (ii = 0; ii < cl.clip.atc_delta_count; ii++)
                {
                    cl.clip.atc_delta[ii].delta = bits.read(32);
                    bits.read_string(out cl.clip.atc_delta[ii].file_id, 5);
                    bits.read_string(out cl.clip.atc_delta[ii].file_code, 4);
                    bits.skip(8);
                }
            }

            // font info
            if (cl.clip.application_type == 6 /*Sub TS for a sub-path of Text subtitle*/)
            {
                CLPI_FONT_INFO fi = cl.clip.font_info;
                bits.skip(8);
                fi.font_count = (byte)bits.read(8);
                if (fi.font_count > 0)
                {
                    fi.font = new CLPI_FONT[fi.font_count];
                    for (ii = 0; ii < fi.font_count; ii++)
                    {
                        bits.read_string(out fi.font[ii].file_id, 5);
                        bits.skip(8);
                    }
                }
            }

            return true;
        }

        private static bool _parse_sequence(BITSTREAM bits, CLPI_CL cl) {
            int ii;
            int jj;

            if (bits.seek_byte(cl.sequence_info_start_addr) < 0)
            {
                return false;
            }

            bits.skip(5 * 8); // length, reserved
            // Get the number of sequences
            cl.sequence.num_atc_seq = (byte)bits.read(8);

            CLPI_ATC_SEQ[] atc_seq = new CLPI_ATC_SEQ[cl.sequence.num_atc_seq];
            cl.sequence.atc_seq = atc_seq;
            for (ii = 0; ii < cl.sequence.num_atc_seq; ii++)
            {
                atc_seq[ii].spn_atc_start = bits.read(32);
                atc_seq[ii].num_stc_seq = (byte)bits.read(8);
                atc_seq[ii].offset_stc_id = (byte)bits.read(8);

                CLPI_STC_SEQ[] stc_seq = new CLPI_STC_SEQ[atc_seq[ii].num_stc_seq];
                atc_seq[ii].stc_seq = stc_seq;
                for (jj = 0; jj < atc_seq[ii].num_stc_seq; jj++)
                {
                    stc_seq[jj].pcr_pid = (UInt16)bits.read(16);
                    stc_seq[jj].spn_stc_start = bits.read(32);
                    stc_seq[jj].presentation_start_time = bits.read(32);
                    stc_seq[jj].presentation_end_time = bits.read(32);
                }
            }

            return true;
        }

        private static bool _parse_program(BITSTREAM bits, CLPI_PROG_INFO program) {
            int ii;
            int jj;

            bits.skip(5 * 8); // length, reserved
            // Get the number of sequences
            program.num_prog = (byte)bits.read(8);

            CLPI_PROG[] progs = new CLPI_PROG[program.num_prog];
            program.progs = progs;
            for (ii = 0; ii < program.num_prog; ii++)
            {
                progs[ii].spn_program_sequence_start = bits.read(32);
                progs[ii].program_map_pid = (UInt16)bits.read(16);
                progs[ii].num_streams = (byte)bits.read(8);
                progs[ii].num_groups = (byte)bits.read(8);

                CLPI_PROG_STREAM[] ps = new CLPI_PROG_STREAM[progs[ii].num_streams];
                progs[ii].streams = ps;
                for (jj = 0; jj < progs[ii].num_streams; jj++)
                {
                    ps[jj].pid = (UInt16)bits.read(16);
                    if (!_parse_stream_attr(bits, ps[jj]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static bool _parse_program_info(BITSTREAM bits, CLPI_CL cl) {
            if (bits.seek_byte(cl.program_info_start_addr) < 0)
            {
                return false;
            }

            return _parse_program(bits, cl.program);
        }

        public static bool _parse_ep_map_stream(BITSTREAM bits, CLPI_EP_MAP_ENTRY ee) {
            UInt32 fine_start;
            Int32 ii;
            CLPI_EP_COARSE[] coarse = null;
            CLPI_EP_FINE[] fine = null;

            if (bits.seek_byte(ee.ep_map_stream_start_addr) < 0)
            {
                return false;
            }
            fine_start = bits.read(32);

            if (bits.avail() / (8 * 8) < ee.num_ep_coarse)
            {
                Utils.BD_DEBUG(LogLevel.Critical, module2, "clpi_parse: unexpected EOF (EP coarse)");
                return false;
            }

            coarse = new CLPI_EP_COARSE[ee.num_ep_coarse];
            ee.coarse = coarse;
            for(ii = 0; ii < ee.num_ep_coarse; ii++)
            {
                coarse[ii].ref_ep_fine_id = (int)bits.read(18);
                coarse[ii].pts_ep = (int)bits.read(14);
                coarse[ii].spn_ep = bits.read(32);
            }

            if (bits.seek_byte(ee.ep_map_stream_start_addr + fine_start) < 0)
            {
                return false;
            }

            if (bits.avail() / (8 * 4) < ee.num_ep_fine)
            {
                Utils.BD_DEBUG(LogLevel.Critical, module2, "clpi_parse: unexpected EOF (EP fine)");
                return false;
            }

            fine = new CLPI_EP_FINE[ee.num_ep_fine];
            ee.fine = fine;
            for (ii = 0; ii < ee.num_ep_fine; ii++)
            {
                fine[ii].is_angle_change_point = bits.read_bool();
                fine[ii].i_end_position_offset = (byte)bits.read(3);
                fine[ii].pts_ep = (int)bits.read(11);
                fine[ii].spn_ep = (int)bits.read(17);
            }
            return true;
        }

        private static bool _parse_cpi(BITSTREAM bits, CLPI_CPI cpi) {
            int ii;
            UInt32 ep_map_pos;
            UInt32 len;

            len = bits.read(32);
            if (len == 0)
            {
                return true;
            }

            bits.skip(12);
            cpi.type = (byte)bits.read(4);
            ep_map_pos = (UInt32)(bits.pos() >> 3);

            // ep map start here
            bits.skip(8);
            cpi.num_stream_pid = (byte)bits.read(8);

            CLPI_EP_MAP_ENTRY[] entry = new CLPI_EP_MAP_ENTRY[cpi.num_stream_pid];
            cpi.entry = entry;
            for (ii = 0; ii < cpi.num_stream_pid; ii++)
            {
                entry[ii].pid = (UInt16)bits.read(16);
                bits.skip(10);
                entry[ii].ep_stream_type = (byte)bits.read(4);
                entry[ii].num_ep_coarse = (Int16)bits.read(16);
                entry[ii].num_ep_fine = (Int16)bits.read(18);
                entry[ii].ep_map_stream_start_addr = bits.read(32) + ep_map_pos;
            }
            for (ii = 0; ii < cpi.num_stream_pid; ii++)
            {
                if (!_parse_ep_map_stream(bits, cpi.entry[ii]))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool _parse_cpi_info(BITSTREAM bits, CLPI_CL cl) {
            if (bits.seek_byte(cl.cpi_start_addr) < 0)
            {
                return false;
            }

            return _parse_cpi(bits, cl.cpi);
        }

        public UInt32 find_stc_spn(byte stc_id) {
            int ii;
            CLPI_ATC_SEQ atc;

            for (ii = 0; ii < this.sequence.num_atc_seq; ii++)
            {
                atc = this.sequence.atc_seq[ii];
                if (stc_id < atc.offset_stc_id + atc.num_stc_seq)
                {
                    return atc.stc_seq[stc_id - atc.offset_stc_id].spn_stc_start;
                }
            }
            return 0;
        }

        /// <summary>
        /// Looks up the start packet number for the timestamp
        /// Returns the spn for the entry that is closest to but
        /// before the given timestamp
        /// </summary>
        /// <param name="timestamp"></param>
        /// <param name="before"></param>
        /// <param name="stc_id"></param>
        /// <returns></returns>
        public UInt32 lookup_spn(UInt32 timestamp, bool before, byte stc_id) {
            CLPI_EP_MAP_ENTRY entry;
            CLPI_CPI cpi = this.cpi;
            Int32 ii = 0, jj = 0;
            UInt32 coarse_pts = 0, pts = 0; // 45khz timestamps
            UInt32 spn = 0, coarse_spn = 0, stc_spn = 0;
            Int32 start = 0, end = 0;
            Int32 reff = 0;

            if (cpi.num_stream_pid < 1 || cpi.entry == null)
            {
                if (before)
                {
                    return 0;
                }
            }

            // Assumes that there is only one pid of interest
            entry = cpi.entry[0];

            // Use sequence info to find spn_stc_start before doing
            // PTS search. The spn_stc_start defined the point in
            // the EP map to start searching
            stc_spn = this.find_stc_spn(stc_id);
            for(ii = 0; ii < entry.num_ep_coarse; ii++)
            {
                reff = entry.coarse[ii].ref_ep_fine_id;
                if (entry.coarse[ii].spn_ep >= stc_spn)
                {
                    // The desired starting point is either after this point
                    // or in the middle of the previous coarse entry
                    break;
                }
            }
            if (ii >= entry.num_ep_coarse)
            {
                return this.clip.num_source_packets;
            }
            pts = (UInt32)(((UInt64)(entry.coarse[ii].pts_ep & ~0x01) << 18)
                + ((UInt64)entry.fine[reff].pts_ep << 8));
            if (pts > timestamp && (ii != 0))
            {
                // The starting point and desired PTS is in the previous coarse entry
                ii--;
                coarse_pts = (UInt32)(entry.coarse[ii].pts_ep & ~0x01) << 18;
                coarse_spn = entry.coarse[ii].spn_ep;
                start = entry.coarse[ii].ref_ep_fine_id;
                end = entry.coarse[ii + 1].ref_ep_fine_id;

                // Find a fine entry that has both spn > stc_spn and ptc > timestamp
                for (jj = start; jj < end; jj++)
                {
                    pts = coarse_pts + ((UInt32)entry.fine[jj].pts_ep << 8);
                    spn = (coarse_spn & ~0x1FFFFu) + (uint)entry.fine[jj].spn_ep;
                    if (stc_spn >= spn && pts > timestamp)
                    {
                        break;
                    }
                }
                goto done;
            }

            // If we've gotten this far, the desires timestamp is somewhere
            // after the coarse entry we found the stc_spn in
            start = ii;
            for (ii = start; ii < entry.num_ep_coarse; ii++)
            {
                reff = entry.coarse[ii].ref_ep_fine_id;
                pts = (UInt32)(((UInt64)(entry.coarse[ii].pts_ep & ~0x01) << 18)
                    + ((UInt64)entry.fine[reff].pts_ep << 8));
                if (pts > timestamp)
                {
                    break;
                }
            }

            // If the timestamp is before the first entry, then return
            // the beginning of the clip
            if (ii == 0)
            {
                return 0;
            }
            ii--;
            coarse_pts = (UInt32)(entry.coarse[ii].pts_ep & ~0x01) << 18;
            start = entry.coarse[ii].ref_ep_fine_id;
            if (ii  < entry.num_ep_coarse - 1)
            {
                end = entry.coarse[ii + 1].ref_ep_fine_id;
            } else
            {
                end = entry.num_ep_fine;
            }
            for(jj = start; jj < end; jj++)
            {
                pts = coarse_pts + ((UInt32)entry.fine[jj].pts_ep << 8);
                if (pts > timestamp)
                {
                    break;
                }
            }

        done:
            if (before)
            {
                jj--;
            }
            if (jj == end)
            {
                ii++;
                if (ii >= entry.num_ep_coarse)
                {
                    // End of file
                    return this.clip.num_source_packets;
                }
                jj = entry.coarse[ii].ref_ep_fine_id;
            }
            spn = (entry.coarse[ii].spn_ep & ~0x1FFFFu) + (UInt32)entry.fine[jj].spn_ep;
            return spn;
        }

        /// <summary>
        /// Looks up the start packet number that is closest to the requested packet
        /// Returns the spn for the entry that is closest to but
        /// before the given packet
        /// </summary>
        public UInt32 access_point(UInt32 pkt, Int32 next, Int32 angle_change, out UInt32 time) {
            CLPI_EP_MAP_ENTRY entry = null;
            CLPI_CPI cpi = this.cpi;
            Int32 ii = 0, jj = 0;
            UInt32 coarse_spn = 0, spn = 0;
            Int32 start = 0, end = 0;
            Int32 reff = 0;

            // Assumed that there is only one pid of interest
            entry = cpi.entry[0];

            for(ii = 0; ii < entry.num_ep_coarse; ii++)
            {
                reff = entry.coarse[ii].ref_ep_fine_id;
                spn = (entry.coarse[ii].spn_ep & ~0x1FFFFu) + (uint)entry.fine[reff].spn_ep;
                if (spn > pkt)
                {
                    break;
                }
            }

            // If the timestamp is before the first entry, then return
            // the beginning of the clip
            if (ii == 0)
            {
                time = 0;
                return 0;
            }
            ii--;
            coarse_spn = (entry.coarse[ii].spn_ep & ~0x1FFFFu);
            start = entry.coarse[ii].ref_ep_fine_id;
            if (ii < entry.num_ep_coarse - 1)
            {
                end = entry.coarse[ii + 1].ref_ep_fine_id;
            } else
            {
                end = entry.num_ep_fine;
            }
            for(jj = start; jj < end; jj++)
            {
                spn = coarse_spn + (uint)entry.fine[jj].spn_ep;
                if (spn >= pkt)
                {
                    break;
                }
            }
            if ((jj == end) && (next != 0))
            {
                ii++;
                jj = 0;
            } else if ((spn != pkt) && (next == 0))
            {
                jj--;
            }
            if(ii == entry.num_ep_coarse)
            {
                time = 0;
                return this.clip.num_source_packets;
            }
            coarse_spn = (entry.coarse[ii].spn_ep & ~0x1FFFFu);
            if (angle_change != 0)
            {
                // Keep looking till there's an angle change point
                for(; jj < end; jj++)
                {
                    if (entry.fine[jj].is_angle_change_point)
                    {
                        time = (UInt32)(((UInt64)(entry.coarse[ii].pts_ep & ~0x01) << 18)
                            + ((UInt64)entry.fine[jj].pts_ep << 8));
                        return coarse_spn + (UInt32)entry.fine[jj].spn_ep;
                    }
                }
                for(ii++; ii < entry.num_ep_coarse; ii++)
                {
                    start = entry.coarse[ii].ref_ep_fine_id;
                    if (ii < entry.num_ep_coarse - 1)
                    {
                        end = entry.coarse[ii + 1].ref_ep_fine_id;
                    } else
                    {
                        end = entry.num_ep_fine;
                    }
                    for (jj = start; jj < end; jj++)
                    {
                        if (entry.fine[jj].is_angle_change_point)
                        {
                            time = (UInt32)(((UInt64)(entry.coarse[ii].pts_ep & ~0x01) << 18)
                                + ((UInt64)entry.fine[jj].pts_ep << 8));
                            return coarse_spn + (UInt32)entry.fine[jj].spn_ep;
                        }
                    }
                }
                time = 0;
                return this.clip.num_source_packets;
            }
            time = (UInt32)(((UInt64)(entry.coarse[ii].pts_ep & ~0x01) << 18)
                + ((UInt64)entry.fine[jj].pts_ep << 8));
            return coarse_spn + (UInt32)entry.fine[jj].spn_ep;
        }

        private static bool _parse_extent_start_points(BITSTREAM bits, CLPI_EXTENT_START es) {
            UInt32 ii;

            bits.skip(32); // length
            es.num_point = bits.read(32);

            es.point = new UInt32[es.num_point];
            for (ii = 0; ii < es.num_point; ii++)
            {
                es.point[ii] = bits.read(32);
            }

            return true;
        }

        private static bool _parse_clpi_extension(BITSTREAM bits, Int32 id1, Int32 id2, object handle) {
            CLPI_CL cl = (CLPI_CL)handle;

            if (id1 == 1)
            {
                if (id2 == 2)
                {
                    // LPCM down mix coefficient
                    //_parse_lpcm_down_mix_coeff(bits, &cl->lpcm_down_mix_coeff);
                    return false;
                }
            }

            if (id1 == 2)
            {
                if (id2 == 4)
                {
                    // Extent start point
                    return _parse_extent_start_points(bits, cl.extent_start);
                }
                if (id2 == 5)
                {
                    // ProgramInfo SS
                    return _parse_program(bits, cl.program_ss);
                }
                if (id2 == 6)
                {
                    // CPI SS
                    return _parse_cpi(bits, cl.cpi_ss);
                }
            }

            Utils.BD_DEBUG(LogLevel.Critical, module2, $"_parse_clpi_extension(): unhandled extension {id1}.{id2}");
            return false;
        }

        private static CLPI_CL? _clpi_parse(BD_FILE_H fp) {
            BITSTREAM bits = new();
            CLPI_CL cl = null;

            if (bits.init(fp) < 0)
            {
                Utils.BD_DEBUG(module, "?????.clpi: read error");
                return null;
            }

            cl = new CLPI_CL();
            if (cl.ext_data_start_addr > 0)
            {
                bdmv.parse_extension_data(bits, (Int32)cl.ext_data_start_addr, _parse_clpi_extension, cl);
            }
            if (!_parse_clipinfo(bits, cl))
            {
                return null;
            }
            if (!_parse_sequence(bits, cl))
            {
                return null;
            }
            if (!_parse_program_info(bits, cl))
            {
                return null;
            }
            if (!_parse_cpi_info(bits, cl))
            {
                return null;
            }

            return cl;
        }

        public static CLPI_CL? clpi_parse(string path) {
            BD_FILE_H? fp = null;
            CLPI_CL cl = null;

            fp = file_win32.OpenFile(path, FileMode.Open);
            if (fp == null)
            {
                Utils.BD_DEBUG(LogLevel.Critical, module, $"Failed to open {path}");
                return null;
            }

            cl = _clpi_parse(fp);
            fp.close();
            return cl;
        }

        private static CLPI_CL? _clpi_get(BD_DISC disc, string dir, string file) {
            BD_FILE_H? fp = null;
            CLPI_CL? cl = null;

            fp = disc.open_file(dir, file);
            if (fp == null)
            {
                return null;
            }

            cl = _clpi_parse(fp);
            fp.close();
            return cl;
        }

        public static CLPI_CL? get(BD_DISC? disc, string file) {
            CLPI_CL? cl = null;

            /*cl = disc.cache_get(disc, file);
            if (cl != null)
            {
                return cl;
            }*/

            cl = _clpi_get(disc, Path.Combine("BDMV", "CLIPINF"), file);
            if (cl == null)
            {
                // if failed, try backup file
                cl = _clpi_get(disc, Path.Combine("BDMV", "BACKUP", "CLIPINF"), file);
            }

            if (cl != null)
            {
                //disc_cache_put(disc, file, cl);
            }

            return cl;
        }

        public CLPI_CL copy() {
            CLPI_CL dest = new();
            int ii, jj;

            dest.clip.clip_stream_type = this.clip.clip_stream_type;
            dest.clip.application_type = this.clip.application_type;
            dest.clip.is_atc_delta = this.clip.is_atc_delta;
            dest.clip.atc_delta_count = this.clip.atc_delta_count;
            dest.clip.ts_recording_rate = this.clip.ts_recording_rate;
            dest.clip.num_source_packets = this.clip.num_source_packets;
            dest.clip.ts_type_info.validity = this.clip.ts_type_info.validity;
            dest.clip.ts_type_info.format_id = this.clip.ts_type_info.format_id;
            dest.clip.atc_delta = new CLPI_ATC_DELTA[this.clip.atc_delta.Length];
            for (ii = 0; ii < this.clip.atc_delta_count; ii++)
            {
                dest.clip.atc_delta[ii] = new();
                dest.clip.atc_delta[ii].delta = this.clip.atc_delta[ii].delta;
                dest.clip.atc_delta[ii].file_id = this.clip.atc_delta[ii].file_id;
                dest.clip.atc_delta[ii].file_code = this.clip.atc_delta[ii].file_code;
            }

            dest.sequence.num_atc_seq = this.sequence.num_atc_seq;
            dest.sequence.atc_seq = new CLPI_ATC_SEQ[this.sequence.atc_seq.Length];
            for (ii = 0; ii < this.sequence.num_atc_seq; ii++)
            {
                dest.sequence.atc_seq[ii] = new();
                dest.sequence.atc_seq[ii].spn_atc_start = this.sequence.atc_seq[ii].spn_atc_start;
                dest.sequence.atc_seq[ii].offset_stc_id = this.sequence.atc_seq[ii].offset_stc_id;
                dest.sequence.atc_seq[ii].num_stc_seq = this.sequence.atc_seq[ii].num_stc_seq;
                dest.sequence.atc_seq[ii].stc_seq = new CLPI_STC_SEQ[this.sequence.atc_seq[ii].stc_seq.Length];
                for (jj = 0; jj < this.sequence.atc_seq[ii].num_stc_seq; jj++)
                {
                    dest.sequence.atc_seq[ii].stc_seq[jj] = new();
                    dest.sequence.atc_seq[ii].stc_seq[jj].spn_stc_start = this.sequence.atc_seq[ii].stc_seq[jj].spn_stc_start;
                    dest.sequence.atc_seq[ii].stc_seq[jj].pcr_pid = this.sequence.atc_seq[ii].stc_seq[jj].pcr_pid;
                    dest.sequence.atc_seq[ii].stc_seq[jj].presentation_start_time = this.sequence.atc_seq[ii].stc_seq[jj].presentation_start_time;
                    dest.sequence.atc_seq[ii].stc_seq[jj].presentation_end_time = this.sequence.atc_seq[ii].stc_seq[jj].presentation_end_time;
                }
            }

            dest.program.num_prog = this.program.num_prog;
            dest.program.progs = new CLPI_PROG[this.program.progs.Length];
            for (ii = 0; ii < this.program.num_prog; ii++)
            {
                dest.program.progs[ii] = new();
                dest.program.progs[ii].spn_program_sequence_start = this.program.progs[ii].spn_program_sequence_start;
                dest.program.progs[ii].program_map_pid = this.program.progs[ii].program_map_pid;
                dest.program.progs[ii].num_streams = this.program.progs[ii].num_streams;
                dest.program.progs[ii].num_groups = this.program.progs[ii].num_groups;
                dest.program.progs[ii].streams = new CLPI_PROG_STREAM[this.program.progs[ii].streams.Length];
                for (jj = 0; jj < this.program.progs[ii].num_streams; jj++)
                {
                    dest.program.progs[ii].streams[jj] = new();
                    dest.program.progs[ii].streams[jj].coding_type = this.program.progs[ii].streams[jj].coding_type;
                    dest.program.progs[ii].streams[jj].pid = this.program.progs[ii].streams[jj].pid;
                    dest.program.progs[ii].streams[jj].video_format = this.program.progs[ii].streams[jj].video_format;
                    dest.program.progs[ii].streams[jj].audio_format = this.program.progs[ii].streams[jj].audio_format;
                    dest.program.progs[ii].streams[jj].video_rate = this.program.progs[ii].streams[jj].video_rate;
                    dest.program.progs[ii].streams[jj].audio_rate = this.program.progs[ii].streams[jj].audio_rate;
                    dest.program.progs[ii].streams[jj].aspect = this.program.progs[ii].streams[jj].aspect;
                    dest.program.progs[ii].streams[jj].oc_flag = this.program.progs[ii].streams[jj].oc_flag;
                    dest.program.progs[ii].streams[jj].cr_flag = this.program.progs[ii].streams[jj].cr_flag;
                    dest.program.progs[ii].streams[jj].dynamic_range_type = this.program.progs[ii].streams[jj].dynamic_range_type;
                    dest.program.progs[ii].streams[jj].color_space = this.program.progs[ii].streams[jj].color_space;
                    dest.program.progs[ii].streams[jj].hdr_plus_flag = this.program.progs[ii].streams[jj].hdr_plus_flag;
                    dest.program.progs[ii].streams[jj].char_code = this.program.progs[ii].streams[jj].char_code;
                    dest.program.progs[ii].streams[jj].lang = this.program.progs[ii].streams[jj].lang;
                    Array.Copy(this.program.progs[ii].streams[jj].isrc, dest.program.progs[ii].streams[jj].isrc, this.program.progs[ii].streams[jj].isrc.Length);
                }
            }

            dest.cpi.num_stream_pid = this.cpi.num_stream_pid;
            dest.cpi.entry = new CLPI_EP_MAP_ENTRY[this.cpi.entry.Length];
            for (ii = 0; ii < dest.cpi.num_stream_pid; ii++)
            {
                dest.cpi.entry[ii] = new();
                dest.cpi.entry[ii].pid = this.cpi.entry[ii].pid;
                dest.cpi.entry[ii].ep_stream_type = this.cpi.entry[ii].ep_stream_type;
                dest.cpi.entry[ii].num_ep_coarse = this.cpi.entry[ii].num_ep_coarse;
                dest.cpi.entry[ii].num_ep_fine = this.cpi.entry[ii].num_ep_fine;
                dest.cpi.entry[ii].ep_map_stream_start_addr = this.cpi.entry[ii].ep_map_stream_start_addr;
                dest.cpi.entry[ii].coarse = new CLPI_EP_COARSE[this.cpi.entry[ii].coarse.Length];
                for (jj = 0; jj < this.cpi.entry[ii].num_ep_coarse; jj++)
                {
                    dest.cpi.entry[ii].coarse[jj] = new();
                    dest.cpi.entry[ii].coarse[jj].ref_ep_fine_id = this.cpi.entry[ii].coarse[jj].ref_ep_fine_id;
                    dest.cpi.entry[ii].coarse[jj].pts_ep = this.cpi.entry[ii].coarse[jj].pts_ep;
                    dest.cpi.entry[ii].coarse[jj].spn_ep = this.cpi.entry[ii].coarse[jj].spn_ep;
                }
                dest.cpi.entry[ii].fine = new CLPI_EP_FINE[this.cpi.entry[ii].fine.Length];
                for (jj = 0; jj < this.cpi.entry[ii].num_ep_fine; jj++)
                {
                    dest.cpi.entry[ii].fine[jj] = new();
                    dest.cpi.entry[ii].fine[jj].is_angle_change_point = this.cpi.entry[ii].fine[jj].is_angle_change_point;
                    dest.cpi.entry[ii].fine[jj].i_end_position_offset = this.cpi.entry[ii].fine[jj].i_end_position_offset;
                    dest.cpi.entry[ii].fine[jj].pts_ep = this.cpi.entry[ii].fine[jj].pts_ep;
                    dest.cpi.entry[ii].fine[jj].spn_ep = this.cpi.entry[ii].fine[jj].spn_ep;
                }
            }

            dest.clip.font_info.font_count = this.clip.font_info.font_count;
            if (dest.clip.font_info.font_count > 0)
            {
                dest.clip.font_info.font = new CLPI_FONT[this.clip.font_info.font.Length];
                Array.Copy(this.clip.font_info.font, dest.clip.font_info.font, this.clip.font_info.font.Length);
            }

            return dest;
        }

        
        //BD_PRIVATE void clpi_free(struct clpi_cl **cl);
    }
}
