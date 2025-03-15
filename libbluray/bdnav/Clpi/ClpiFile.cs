using libbluray.disc;
using libbluray.file;
using libbluray.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libbluray.bdnav.Clpi {
    public class ClpiFile {
        const string module = "libbluray.bdnav.Clpi.ClpiFile";

        public string type_indicator = ""; // Length 4
        public string type_indicator2 = ""; // Length 4
        public UInt32 sequence_info_start_addr;
        public UInt32 program_info_start_addr;
        public UInt32 cpi_start_addr;
        public UInt32 clip_mark_start_addr;
        public UInt32 ext_data_start_addr;
        public ClipInfo clip = new();
        public SequenceInfo sequence = new();
        public ClipProgramInfo program = new();
        public ClipCPI cpi = new();
        // skip clip mark & extension data

        // extensions for 3D
        public ClipExtension Extension = new();

        private const string SIG1 = "HDMV";

        private bool ParseHeader(BitStream bits) {
            this.type_indicator = SIG1;
            if (!bdmv.parse_header(bits, this.type_indicator, out this.type_indicator2))
            {
                return false;
            }

            if (bits.AvailableBytes() < 5 * 4)
            {
                Utils.BD_DEBUG(LogLevel.Critical, module, "_parse_header: unexpected end of file");
                return false;
            }

            this.sequence_info_start_addr = bits.Read<UInt32>();
            this.program_info_start_addr = bits.Read<UInt32>();
            this.cpi_start_addr = bits.Read<UInt32>();
            this.clip_mark_start_addr = bits.Read<UInt32>();
            this.ext_data_start_addr = bits.Read<UInt32>(); // may be 0

            return true;
        }

        private bool Parse(BitStream bits) {

            if (!ParseHeader(bits))
            {
                return false;
            }
            if (this.ext_data_start_addr > 0)
            {
                Extension.Parse(bits, this.ext_data_start_addr);
            }

            bits.SeekByte(40);
            if (!this.clip.Parse(bits))
            {
                return false;
            }

            bits.SeekByte(sequence_info_start_addr);
            if (!sequence.Parse(bits))
            {
                return false;
            }

            bits.SeekByte(this.program_info_start_addr);
            if (!this.program.Parse(bits))
            {
                return false;
            }

            bits.SeekByte(this.cpi_start_addr);
            // TODO
            /*if (!this.cpi.Parse(bits))
            {
                return false;
            }*/

            return true;
        }

        public static ClpiFile? Parse(BD_FILE_H file) {
            BitStream bits = new(file);
            ClpiFile clpi = new();
            if (!clpi.Parse(bits))
            {
                return null;
            }
            return clpi;
        }

        public static ClpiFile? Parse(string path) {
            BD_FILE_H? file = file_win32.OpenFile(path, FileMode.Open);
            if (file == null)
            {
                Utils.BD_DEBUG(LogLevel.Critical, module, $"Failed to open {path}");
                return null;
            }

            try
            {
                ClpiFile? result = Parse(file);
                return result;
            } catch (Exception e)
            {
                return null;
            } finally
            {
                file.close();
            }
        }

        public static ClpiFile? Get(BD_DISC disc, string filename) {
            ClpiFile? result = null;
            BD_FILE_H? file = disc.open_file(Path.Combine("BDMV", "CLIPINF"), filename);
            if (file != null)
            {
                result = Parse(file);
            }

            // if failed, try backup file
            if (result == null)
            {
                file = disc.open_file(Path.Combine("BDMV", "BACKUP", "CLIPINF"), filename);
                if (file != null)
                {
                    result = Parse(file);
                }
            }

            return result;
        }

        public UInt32 find_stc_spn(byte stc_id) {
            int ii;
            ClipAtcSequence atc;

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
            EpMapEntry entry;
            ClipCPI cpi = this.cpi;
            UInt32 ii = 0, jj = 0;
            UInt32 coarse_pts = 0, pts = 0; // 45khz timestamps
            UInt32 spn = 0, coarse_spn = 0, stc_spn = 0;
            UInt32 start = 0, end = 0;
            UInt32 reff = 0;

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
            for (ii = 0; ii < entry.num_ep_coarse; ii++)
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
            if (ii < entry.num_ep_coarse - 1)
            {
                end = entry.coarse[ii + 1].ref_ep_fine_id;
            } else
            {
                end = entry.num_ep_fine;
            }
            for (jj = start; jj < end; jj++)
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
            EpMapEntry entry = null;
            ClipCPI cpi = this.cpi;
            UInt32 ii = 0, jj = 0;
            UInt32 coarse_spn = 0, spn = 0;
            UInt32 start = 0, end = 0;
            UInt32 reff = 0;

            // Assumed that there is only one pid of interest
            entry = cpi.entry[0];

            for (ii = 0; ii < entry.num_ep_coarse; ii++)
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
            for (jj = start; jj < end; jj++)
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
            if (ii == entry.num_ep_coarse)
            {
                time = 0;
                return this.clip.num_source_packets;
            }
            coarse_spn = (entry.coarse[ii].spn_ep & ~0x1FFFFu);
            if (angle_change != 0)
            {
                // Keep looking till there's an angle change point
                for (; jj < end; jj++)
                {
                    if (entry.fine[jj].is_angle_change_point)
                    {
                        time = (UInt32)(((UInt64)(entry.coarse[ii].pts_ep & ~0x01) << 18)
                            + ((UInt64)entry.fine[jj].pts_ep << 8));
                        return coarse_spn + (UInt32)entry.fine[jj].spn_ep;
                    }
                }
                for (ii++; ii < entry.num_ep_coarse; ii++)
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

        /*public ClpiFile copy() {
            ClpiFile dest = new();
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
            dest.sequence.atc_seq = new ClipAtcSequence[this.sequence.atc_seq.Length];
            for (ii = 0; ii < this.sequence.num_atc_seq; ii++)
            {
                dest.sequence.atc_seq[ii] = new();
                dest.sequence.atc_seq[ii].spn_atc_start = this.sequence.atc_seq[ii].spn_atc_start;
                dest.sequence.atc_seq[ii].offset_stc_id = this.sequence.atc_seq[ii].offset_stc_id;
                dest.sequence.atc_seq[ii].num_stc_seq = this.sequence.atc_seq[ii].num_stc_seq;
                dest.sequence.atc_seq[ii].stc_seq = new ClipStcSequence[this.sequence.atc_seq[ii].stc_seq.Length];
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
            dest.program.progs = new ClipProgram[this.program.progs.Length];
            for (ii = 0; ii < this.program.num_prog; ii++)
            {
                dest.program.progs[ii] = new();
                dest.program.progs[ii].spn_program_sequence_start = this.program.progs[ii].spn_program_sequence_start;
                dest.program.progs[ii].program_map_pid = this.program.progs[ii].program_map_pid;
                dest.program.progs[ii].num_streams = this.program.progs[ii].num_streams;
                dest.program.progs[ii].num_groups = this.program.progs[ii].num_groups;
                dest.program.progs[ii].streams = new ClipProgramStream[this.program.progs[ii].streams.Length];
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
            dest.cpi.entry = new EpMapEntry[this.cpi.entry.Length];
            for (ii = 0; ii < dest.cpi.num_stream_pid; ii++)
            {
                dest.cpi.entry[ii] = new();
                dest.cpi.entry[ii].pid = this.cpi.entry[ii].pid;
                dest.cpi.entry[ii].ep_stream_type = this.cpi.entry[ii].ep_stream_type;
                dest.cpi.entry[ii].num_ep_coarse = this.cpi.entry[ii].num_ep_coarse;
                dest.cpi.entry[ii].num_ep_fine = this.cpi.entry[ii].num_ep_fine;
                dest.cpi.entry[ii].ep_map_stream_start_addr = this.cpi.entry[ii].ep_map_stream_start_addr;
                dest.cpi.entry[ii].coarse = new EpCoarse[this.cpi.entry[ii].coarse.Length];
                for (jj = 0; jj < this.cpi.entry[ii].num_ep_coarse; jj++)
                {
                    dest.cpi.entry[ii].coarse[jj] = new();
                    dest.cpi.entry[ii].coarse[jj].ref_ep_fine_id = this.cpi.entry[ii].coarse[jj].ref_ep_fine_id;
                    dest.cpi.entry[ii].coarse[jj].pts_ep = this.cpi.entry[ii].coarse[jj].pts_ep;
                    dest.cpi.entry[ii].coarse[jj].spn_ep = this.cpi.entry[ii].coarse[jj].spn_ep;
                }
                dest.cpi.entry[ii].fine = new EpFine[this.cpi.entry[ii].fine.Length];
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
        }*/
    }
}
