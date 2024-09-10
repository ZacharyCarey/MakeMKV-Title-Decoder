using libbluray.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace libbluray.bdnav.Mpls
{
    public class PlaylistExtension : ExtensionData
    {

        const string module = "libbluray.bdnav.Mpls.PlaylistExtension";

        // extension data (profile 5, version 2.4)
        public ushort ext_sub_count;

        /// <summary>
        /// sub path entries extension
        /// </summary>
        public SubPath[] ext_sub_path = Array.Empty<SubPath>();

        // extension data (Picture-In-Picture metadata)
        public ushort ext_pip_data_count;

        /// <summary>
        /// pip metadata extension
        /// </summary>
        public MPLS_PIP_METADATA[] ext_pip_data = Array.Empty<MPLS_PIP_METADATA>();

        // extension data (Static Metadata)
        public byte ext_static_metadata_count;
        public MPLS_STATIC_METADATA[] ext_static_metadata = Array.Empty<MPLS_STATIC_METADATA>();

        protected override bool ParseExtensionData(BitStream bits, ushort id1, ushort id2)
        {
            if (id1 == 1)
            {
                if (id2 == 1)
                {
                    // Pip metadata extension
                    return _parse_pip_metadata_extension(bits);
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
                    return _parse_subpath_extension(bits);
                }
            }

            if (id2 == 3)
            {
                if (id2 == 5)
                {
                    // Static metadata extension
                    return _parse_static_metadata_extension(bits);
                }
            }

            Utils.BD_DEBUG(LogLevel.Critical, module, $"_parse_mpls_extension(): unhandled extension {id1}.{id2}");
            return false;
        }

        private bool _parse_pip_metadata_extension(BitStream bits)
        {
            MPLS_PIP_METADATA[]? data = null;
            int ii;

            uint start_address_byte = (uint)(bits.Position / 8);
            uint len = bits.Read<uint>();
            ushort entries = bits.Read<ushort>();

            if (len < 1 || entries < 1)
            {
                return false;
            }

            data = new MPLS_PIP_METADATA[entries];
            for (ii = 0; ii < entries; ii++)
            {
                data[ii] = new MPLS_PIP_METADATA();
                if (!_parse_pip_metadata_block(bits, start_address_byte, data[ii]))
                {
                    Utils.BD_DEBUG(LogLevel.Critical, module, "error parsing pip metadata extension");
                    return false;
                }
            }

            ext_pip_data_count = entries;
            ext_pip_data = data;

            return true;
        }

        private bool _parse_subpath_extension(BitStream bits)
        {
            SubPath[]? sub_path = null;
            int ii;

            uint len = bits.Read<uint>();
            ushort sub_count = bits.Read<ushort>();

            if (len < 1 || sub_count < 1)
            {
                return false;
            }

            sub_path = new SubPath[sub_count];
            for (ii = 0; ii < sub_count; ii++)
            {
                sub_path[ii] = new SubPath();
                if (!sub_path[ii].Parse(bits))
                {
                    Utils.BD_DEBUG(LogLevel.Critical, module, "error parsing extension subpath");
                    return false;
                }
            }
            ext_sub_path = sub_path;
            ext_sub_count = sub_count;

            return true;
        }

        private bool _parse_static_metadata_extension(BitStream bits)
        {
            MPLS_STATIC_METADATA[]? static_metadata = null;

            long len = bits.Read<uint>();
            if (len < 32) // at least one static metadata entry
            {
                return false;
            }
            if (bits.AvailableBytes() < len)
            {
                Utils.BD_DEBUG(LogLevel.Critical, module, "_parse_static_metadata_extension: unexpected end of file");
                return false;
            }

            byte sm_count = bits.Read<byte>();
            if (sm_count < 1)
            {
                return false;
            }
            bits.Skip(24);

            static_metadata = new MPLS_STATIC_METADATA[sm_count];
            for (int i = 0; i < sm_count; i++)
            {
                static_metadata[i] = new MPLS_STATIC_METADATA();
                if (!_parse_static_metadata(bits, static_metadata[i]))
                {
                    Utils.BD_DEBUG(LogLevel.Critical, module, "error parsing static metadata extension");
                    return false;
                }
            }
            ext_static_metadata = static_metadata;
            ext_static_metadata_count = sm_count;

            return true;
        }

        private static bool _parse_static_metadata(BitStream bits, MPLS_STATIC_METADATA data)
        {
            if (bits.AvailableBytes() < 28)
            {
                Utils.BD_DEBUG(LogLevel.Critical, module, "_parse_static_metadata: unexpected end of file");
                return false;
            }

            data.dynamic_range_type = bits.Read<byte>(4);
            bits.Skip(4);
            bits.Skip(24);
            for (int i = 0; i < 3; i++)
            {
                data.display_primaries_x[i] = bits.Read<ushort>();
                data.display_primaries_y[i] = bits.Read<ushort>();
            }
            data.white_point_x = bits.Read<ushort>();
            data.white_point_y = bits.Read<ushort>();
            data.max_display_mastering_luminance = bits.Read<ushort>();
            data.min_display_mastering_luminance = bits.Read<ushort>();
            data.max_CLL = bits.Read<ushort>();
            data.max_FALL = bits.Read<ushort>();

            return true;
        }

        private static bool _parse_pip_metadata_block(BitStream bits, uint start_address_byte, MPLS_PIP_METADATA data)
        {
            uint data_address;
            bool result;
            long pos;

            data.clip_ref = bits.Read<ushort>();
            data.secondary_video_ref = bits.Read<byte>();
            bits.Skip(8);
            data.timeline_type = bits.Read<byte>(4);
            data.luma_key_flag = bits.ReadBool();
            data.trick_play_flag = bits.ReadBool();
            bits.Skip(10);
            if (data.luma_key_flag)
            {
                bits.Skip(8);
                data.upper_limit_luma_key = bits.Read<byte>();
            }
            else
            {
                bits.Skip(16);
            }
            bits.Skip(16);

            data_address = bits.Read<uint>();

            pos = bits.Position;
            bits.SeekByte(start_address_byte + data_address);
            if (bits.EOF)
            {
                return false;
            }
            result = _parse_pip_data(bits, data);
            bits.Seek(pos);
            if (bits.EOF)
            {
                return false;
            }

            return result;
        }

        private static bool _parse_pip_data(BitStream bits, MPLS_PIP_METADATA block)
        {
            MPLS_PIP_DATA[]? data = null;
            uint ii;

            ushort entries = bits.Read<ushort>();
            if (entries < 1)
            {
                return true;
            }

            data = new MPLS_PIP_DATA[entries];
            for (ii = 0; ii < entries; ii++)
            {
                data[ii].time = bits.Read<uint>();
                data[ii].xpos = bits.Read<ushort>(12);
                data[ii].ypos = bits.Read<ushort>(12);
                data[ii].scale_factor = bits.Read<byte>(4);
                bits.Skip(4);
            }

            block.data_count = entries;
            block.data = data;

            return true;
        }
    }
}
