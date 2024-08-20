using MakeMKV_Title_Decoder.Util;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.Data.Bluray.MPLS
{
    public class MplsParser : ByteParser
    {

        public Header Header;
        public Playlist Playlist;
        public List<Chapter> Chapters = new();

        public MplsParser(string path) : base(path)
        {

        }

        public bool Parse()
        {
            try
            {
                parse_header();
                Seek(0);
                parse_playlist();
                parse_chapters();
                read_chapter_names(FilePath);
#if DEBUG
                dump();
#endif
                return true;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"MPLS Exception: {ex.Message}");
                Console.ResetColor();
                return false;
            }
        }

        private void parse_header()
        {
            Header.type_indicator1 = ReadString(4);
            Header.type_indicator2 = ReadString(4);
            Header.playlist_pos = ReadUInt32();
            Header.chapter_pos = ReadUInt32();
            Header.ext_pos = ReadUInt32();

            if (Header.type_indicator1 != "MPLS" || !(Header.type_indicator2 == "0100" || Header.type_indicator2 == "0200" || Header.type_indicator2 == "0300"))
            {
                throw new FormatException($"Wrong type indicator 1({Header.type_indicator1}) or 2 ({Header.type_indicator2})");
            }
        }
        private void parse_playlist()
        {
            Playlist.duration = 0;

            Seek((int)Header.playlist_pos);
            Skip(4 + 2); // playlist length, reserved bytes

            Playlist.list_count = ReadUInt16();
            Playlist.sub_count = ReadUInt16();

            Playlist.items = new PlayItem[Playlist.list_count];
            Playlist.sub_paths = new SubPath[Playlist.sub_count];

            for (uint i = 0; i < Playlist.list_count; i++)
            {
                Playlist.items[i] = parse_play_item();
            }

            for (uint i = 0; i < Playlist.sub_count; i++)
            {
                Playlist.sub_paths[i] = parse_sub_path();
            }
        }

        private PlayItem parse_play_item()
        {
            var item = new PlayItem();

            uint length = ReadUInt16();
            BitIndex position = Index;

            item.clip_id = ReadString(5);
            item.codec_id = ReadString(4);

            Skip(0, 11); // Reserved

            item.is_multi_angle = ReadBool();
            item.connection_condition = Read<uint>(4);
            item.stc_id = ReadUInt8();
            item.in_time = mpls_time_to_timestamp(ReadUInt32());
            item.out_time = mpls_time_to_timestamp(ReadUInt32());
            item.relative_in_time = Playlist.duration;
            Playlist.duration += item.out_time - item.in_time;

            Skip(12); // UO_mask_table, random_access_flag, reserved, still_mode

            if (item.is_multi_angle)
            {
                uint num_angles = ReadUInt8();
                Skip(1); // reserved, is_different_audio, is_seamless_angle_change

                if (num_angles > 0)
                {
                    Skip(((int)num_angles - 1) * 10); // clip_id, clip_codec_id, stc_id
                }
            }

            Skip(2 + 2); // STN Length, reserved

            item.stn = parse_stn();

            position.Byte += (int)length;
            Seek(position);

            return item;
        }

        private STN parse_stn()
        {
            var stn = new STN();

            stn.num_video = ReadUInt8();
            stn.num_audio = ReadUInt8();
            stn.num_pg = ReadUInt8();
            stn.num_ig = ReadUInt8();
            stn.num_secondary_audio = ReadUInt8();
            stn.num_secondary_video = ReadUInt8();
            stn.num_pip_pg = ReadUInt8();

            Skip(5); // reserved

            stn.video_streams = new MplsStream[stn.num_video];
            stn.audio_streams = new MplsStream[stn.num_audio];
            stn.pg_streams = new MplsStream[stn.num_pg];

            for (uint i = 0; i < stn.num_video; i++)
            {
                stn.video_streams[i] = parse_stream();
            }

            for (uint i = 0; i < stn.num_audio; i++)
            {
                stn.audio_streams[i] = parse_stream();
            }

            for (uint i = 0; i < stn.num_pg; i++)
            {
                stn.pg_streams[i] = parse_stream();
            }

            return stn;
        }

        private MplsStream parse_stream()
        {
            var str = new MplsStream();

            int length = ReadUInt8();
            BitIndex position = Index;

            str.stream_type = (StreamType)ReadUInt8();

            switch (str.stream_type)
            {
                case StreamType.UsedByPlayItem:
                    str.pid = ReadUInt16();
                    break;
                case StreamType.UsedBySubPathType23456:
                    str.sub_path_id = ReadUInt8();
                    str.sub_clip_id = ReadUInt8();
                    str.pid = ReadUInt16();
                    break;
                case StreamType.UsedBySubPathType7:
                    str.sub_path_id = ReadUInt8();
                    str.pid = ReadUInt16();
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Unknown stream type: {(int)str.stream_type}");
                    Console.ResetColor();
                    break;
            }

            position.Byte += length;
            Seek(position);

            length = ReadUInt8();
            position = Index;

            str.coding_type = (StreamCodingType)ReadUInt8();
            switch (str.coding_type)
            {
                case StreamCodingType.mpeg2_video_primary_secondary:
                case StreamCodingType.mpeg4_avc_video_primary_secondary:
                case StreamCodingType.vc1_video_primary_secondary:
                    str.format = Read<uint>(4);
                    str.rate = Read<uint>(4);
                    break;
                case StreamCodingType.lpcm_audio_primary:
                case StreamCodingType.ac3_audio_primary:
                case StreamCodingType.dts_audio_primary:
                case StreamCodingType.truehd_audio_primary:
                case StreamCodingType.eac3_audio_primary:
                case StreamCodingType.dts_hd_audio_primary:
                case StreamCodingType.dts_hd_xll_audio_primary:
                case StreamCodingType.eac3_audio_secondary:
                case StreamCodingType.dts_hd_audio_secondary:
                    str.format = Read<uint>(4);
                    str.rate = Read<uint>(4);
                    str.language = ReadString(3);
                    break;
                case StreamCodingType.presentation_graphics_subtitles:
                case StreamCodingType.interactive_graphics_menu:
                    str.language = ReadString(3);
                    break;
                case StreamCodingType.text_subtitles:
                    str.char_code = ReadUInt8();
                    str.language = ReadString(3);
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Unrecognized coding type {(int)str.coding_type:x2}");
                    Console.ResetColor();
                    break;
            }

            position.Byte += length;
            Seek(position);

            return str;
        }

        private SubPath parse_sub_path()
        {
            var path = new SubPath();

            Skip(4 + 1); // Length, reserved
            path.type = (SubPathType)ReadUInt8();
            Skip(0, 15); // reserved
            path.is_repeat_sub_path = ReadBool();
            Skip(1); // reserved
            var num_sub_play_items = ReadUInt8();

            path.items = new SubPlayItem[num_sub_play_items];
            for (uint i = 0; i < num_sub_play_items; i++)
            {
                path.items[i] = parse_sub_play_item();
            }

            return path;
        }

        private SubPlayItem parse_sub_play_item()
        {
            var item = new SubPlayItem();

            Skip(2); // Length
            item.clip_file_name = ReadString(5);
            item.codec_id = ReadString(4);
            Skip(0, 27); // reserved
            item.connection_condition = Read<uint>(4);
            item.is_multi_clip_entries = ReadBool();
            item.ref_to_stc_id = ReadUInt8();
            item.in_time = mpls_time_to_timestamp(ReadUInt32());
            item.out_time = mpls_time_to_timestamp(ReadUInt32());
            item.sync_playitem_id = ReadUInt16();
            item.sync_start_pts_of_playitem = mpls_time_to_timestamp(ReadUInt32());

            if (!item.is_multi_clip_entries)
            {
                return item;
            }

            var num_clips = ReadUInt8();
            Skip(1); // reserved

            item.clips = new SubPlayItemClip[num_clips - 1];
            for (uint i = 0; i < num_clips; i++)
            {
                item.clips[i] = parse_sub_play_item_clip();
            }

            return item;
        }

        private SubPlayItemClip parse_sub_play_item_clip()
        {
            var clip = new SubPlayItemClip();

            clip.clip_file_name = ReadString(5);
            clip.codec_id = ReadString(4);
            clip.ref_to_stc_id = ReadUInt8();

            return clip;
        }

        private void parse_chapters()
        {
            Seek((int)Header.chapter_pos);
            Skip(4); // Length
            uint num_chapters = ReadUInt16();

            // 0 unknown
            // 1 type
            // 2, 3 stream_file_index
            // 4, 5, 6, 7 chapter_time
            // 8 - 13 unknown

            for (uint i = 0; i < num_chapters; i++)
            {
                Seek((int)(Header.chapter_pos + 4 + 2 + i * 14));
                Skip(1); // reserved
                if (ReadUInt8() != 1) // chapter type: entry mark
                {
                    continue;
                }

                uint play_item_index = ReadUInt16();
                if (play_item_index >= Playlist.items.Length)
                {
                    continue;
                }

                Chapter chapter = new();

                ref var play_item = ref Playlist.items[play_item_index];
                chapter.timestamp = mpls_time_to_timestamp(ReadUInt32()) - play_item.in_time + play_item.relative_in_time;

                Chapters.Add(chapter);
            }

            Chapters.Sort((x, y) => x.timestamp.CompareTo(y.timestamp));
        }

        private void read_chapter_names(string base_file_name)
        {
            var matches = Regex.Matches(Path.GetFileName(base_file_name), "(.{5})\\.mpls$");
            if (matches.Count == 0)
            {
                return;
            }

            var all_names = locate_and_parse_for_title(base_file_name, matches[0].Groups[1].Value);

            for (uint chapter_index = 0, num_chapters = (uint)Chapters.Count; chapter_index < num_chapters; chapter_index++)
            {
                foreach (var pair in all_names)
                {
                    if (chapter_index < pair.Value.Count)
                    {
                        ChapterName name = new();
                        name.language = pair.Key;
                        name.name = pair.Value[(int)chapter_index];
                        Chapters[(int)chapter_index].names.Add(name);
                    }
                }
            }
        }

        private Dictionary<string, List<string>> locate_and_parse_for_title(string location, string title_number)
        {
            /*var base_dir = find_base_dir(location);
            if (string.IsNullOrEmpty(base_dir))
            {
                return new();
            }

            var track_chapter_names_dir = Path.Combine(base_dir, "META", "TN");
            if (!Directory.Exists(track_chapter_names_dir))
            {
                return new();
            }*/

            // TODO
            /*
              QRegularExpression tnmt_re{Q(fmt::format("tnmt_([a-z]{{3}})_{}\\.xml", title_number))};

              std::vector<chapter_names_t> chapter_names;

              for (boost::filesystem::directory_iterator dir_itr{track_chapter_names_dir}, end_itr; dir_itr != end_itr; ++dir_itr) {
                auto matches = tnmt_re.match(Q(dir_itr->path().filename()));
                if (!matches.hasMatch())
                  continue;

                auto const language = to_utf8(matches.captured(1));

                mxdebug_if(debug, fmt::format("found TNMT file for language {}\n", language));

                auto names = parse_tnmt_xml(*dir_itr);
                if (!names.empty())
                  chapter_names.emplace_back(language, names);
              }

              std::sort(chapter_names.begin(), chapter_names.end());

              return chapter_names;
             */

            return new();
        }

        private static ulong mpls_time_to_timestamp(ulong value)
        {
            return value * 1000000 / 45;
        }

        public void dump()
        {
            Console.WriteLine($"MPLS class dump");
            Console.WriteLine($"  ok:           {true}");
            Console.WriteLine($"  num_chapters: {Chapters.Count}");

            foreach (var entry in Chapters)
            {
                List<string> names = new();
                foreach (var name in entry.names)
                {
                    names.Add($"{name.language}:{name.name}");
                }

                var names_str = names.Count == 0 ? "" : $" {string.Join(" ", names)}";

                Console.WriteLine($"    {entry.timestamp}{names_str}");
            }

            Header.dump();
            Playlist.dump();
        }
    }
}
