using MkvToolNix;
using MkvToolNix.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Utils;

namespace MkvToolNix.Data
{
    public class MkvMergeID : MkvToolNixData<MkvMergeID>
    {

        /// <summary>
        /// An array describing the attachments found, if any
        /// </summary>
        [JsonPropertyName("attachments"), JsonRequired, JsonInclude]
        public List<MkvAttachment> Attachments = new();

        [JsonPropertyName("chapters"), JsonRequired, JsonInclude]
        public List<MkvChapter> Chapters = new();

        /// <summary>
        /// Information about the identified container
        /// </summary>
        [JsonPropertyName("container"), JsonInclude]
        public MkvContainer? Container = null;

        /// <summary>
        /// The identified file's name
        /// </summary>
        [JsonIgnore]
        public string FileName = "";

        [JsonIgnore]
        public DataSize FileSize = new();

        [JsonIgnore]
        public string FileDirectory;

        [JsonPropertyName("global_tags"), JsonRequired, JsonInclude]
        public List<MkvGlobalTag> GlobalTags = new();

        /// <summary>
        /// The output format's version
        /// </summary>
        [JsonPropertyName("identification_format_version"), JsonInclude]
        public long? IdentificationFormatVersion = new();

        [JsonPropertyName("track_tags"), JsonRequired, JsonInclude]
        public List<MkvTrackTag> TrackTags = new();

        [JsonPropertyName("tracks"), JsonRequired, JsonInclude]
        public List<MkvTrack> Tracks = new();
        //private Dictionary<MkvTrackType, long> trackIndexCount = new();

        [JsonPropertyName("errors"), JsonRequired, JsonInclude]
        public List<string> Errors = new();

        [JsonPropertyName("warnings"), JsonRequired, JsonInclude]
        public List<string> Warnings = new();

        public string GetRelativePath()
        {
            try
            {
                return Path.Combine(FileDirectory, FileName);
            }
            catch (Exception)
            {
                // Full path didnt work, just return the file name as a backup
                return FileName;
            }
        }

        public string GetFullPath(string root)
        {
            try
            {
                return Path.Combine(root, FileDirectory, FileName);
            }
            catch (Exception)
            {
                // Full path didnt work, just return the file name as a backup
                return FileName;
            }
        }

        public static MkvMergeID? TryParse(IEnumerable<string> std, IProgress<SimpleProgress>? progress = null, object? tag = null)
        {
            try
            {
                Stream stream = new SequentialStream(std);
                return JsonSerializer.Deserialize<MkvMergeID>(stream);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        internal void SetFile(string root, string dir, string name)
        {
            this.FileDirectory = dir;
            this.FileName = name;
            this.FileSize = DataSize.FromFile(Path.Combine(root, dir, name)) ?? new DataSize();
        }

        public override string ToString()
        {
            return FileName ?? "N/A";
        }
    }

    public class MkvAttachment
    {
        [JsonPropertyName("content_type"), JsonInclude]
        public string? ContentType = null;

        [JsonPropertyName("description"), JsonInclude]
        public string? Description = null;

        [JsonPropertyName("file_name"), JsonInclude]
        public string? FileName = null;

        [JsonPropertyName("id"), JsonInclude]
        public long? Id = null;

        [JsonPropertyName("size"), JsonInclude]
        public long? Size = null;

        [JsonPropertyName("properties"), JsonInclude]
        public MkvAttachmentProperties? Properties = new();

        [JsonPropertyName("type"), JsonInclude]
        public string? Type = null;
    }

    public class MkvAttachmentProperties
    {
        [JsonPropertyName("uid"), JsonInclude]
        public long? UID = null;
    }

    public class MkvChapter
    {
        [JsonPropertyName("num_entries"), JsonRequired, JsonInclude]
        public long NumEntries = 0;
    }

    public class MkvContainer
    {

        /// <summary>
        /// Additional properties for the container varying by container format.
        /// </summary>
        [JsonPropertyName("properties"), JsonInclude]
        public MkvContainerProperties? Properties = new();

        /// <summary>
        /// States whether or not mkvmerge knows about the format
        /// </summary>
        [JsonPropertyName("recognized"), JsonRequired, JsonInclude]
        public bool Recognized = false;

        /// <summary>
        /// States whether or not mkvmerge can read the format
        /// </summary>
        [JsonPropertyName("supported"), JsonRequired, JsonInclude]
        public bool Supported = false;

        /// <summary>
        /// A human-readable description/name for the container format
        /// </summary>
        [JsonPropertyName("type"), JsonInclude]
        public string? Type = null;
    }

    public enum ContainerType
    {
        is_unknown = 0,
        aac,
        ac3,
        asf,
        avc_es,
        avi,
        cdxa,
        chapters,
        coreaudio,
        dirac,
        dts,
        dv,
        flac,
        flv,
        hevc_es,
        hdsub,
        ivf,
        matroska,
        microdvd,
        mp3,
        mpeg_es,
        mpeg_ps,
        MpegTransportStream,
        ogm,
        pgssup,
        qtmp4,
        real,
        srt,
        ssa,
        truehd,
        tta,
        usf,
        vc1,
        vobbtn,
        vobsub,
        wav,
        wavpack4,
        webvtt,
        hdmv_textst,
        obu,
        avi_dv_1,
        max = avi_dv_1
    }

    public class MkvContainerProperties
    {

        /// <summary>
        /// A unique number identifying the container type that's supposed to stay constant over all future releases of MKVToolNix
        /// </summary>
        [JsonPropertyName("container_type"), JsonInclude]
        public ContainerType? ContainerType = null;

        /// <summary>
        /// The muxing date in ISO 8601 format (in local time zone)
        /// ^[0-9]{4}-[0-9]{2}-[0-9]{2}T[0-9]{2}:[0-9]{2}:[0-9]{2}([+-][0-9]{2}:[0-9]{2}|Z)$
        /// </summary>
        [JsonPropertyName("date_local"), JsonInclude]
        public string? DateLocal = null;

        /// <summary>
        /// The muxing date in ISO 8601 format (in UTC)
        /// ^[0-9]{4}-[0-9]{2}-[0-9]{2}T[0-9]{2}:[0-9]{2}:[0-9]{2}([+-][0-9]{2}:[0-9]{2}|Z)$
        /// </summary>
        [JsonPropertyName("date_utc"), JsonInclude]
        public string? DateUTC = null;

        [JsonPropertyName("duration"), JsonInclude]
        private long? duration = null;

        /// <summary>
        /// The file's/segment's duration in nanoseconds
        /// </summary>
        public TimeSpan? Duration => (duration != null) ? new TimeSpan(duration.Value / 100) : null;

        /// <summary>
        /// States whether or not the container has timestamps for the packets (e.g. Matroska, MP4) or not (e.g. SRT, MP3)
        /// </summary>
        [JsonPropertyName("is_providing_timestamps"), JsonInclude]
        public bool? IsProvidingTimestamps = null;

        /// <summary>
        /// A Unicode string containing the name and possibly version of the low-level library or application that created the file
        /// </summary>
        [JsonPropertyName("muxing_application"), JsonInclude]
        public string? MuxingApplication = null;

        /// <summary>
        /// A hexadecimal string of the next segment's UID (only for Matroska files)
        /// </summary>
        [JsonPropertyName("next_segment_uid"), JsonInclude]
        public string? NextSegmentUID = null;

        /// <summary>
        /// An array of names of additional files processed as well
        /// </summary>
        [JsonPropertyName("other_file"), JsonInclude]
        public List<string> OtherFiles = new();

        /// <summary>
        /// States whether or not the identified file is a playlist (e.g. MPLS) referring to several other files
        /// </summary>
        [JsonPropertyName("playlist"), JsonInclude]
        public bool? Playlist = null;

        /// <summary>
        /// The number of chapters in a playlist if it is a one
        /// </summary>
        [JsonPropertyName("playlist_chapters"), JsonInclude]
        public long? PlaylistChapters = null;


        [JsonPropertyName("playlist_duration"), JsonInclude]
        private long? playlistDuration = null;

        /// <summary>
        /// The total duration in nanoseconds of all files referenced by the playlist if it is a one
        /// </summary>
        public TimeSpan? PlaylistDuration => (playlistDuration != null) ? new TimeSpan(playlistDuration.Value / 100) : null;

        /// <summary>
        /// An array of file names the playlist contains
        /// </summary>
        [JsonPropertyName("playlist_file"), JsonInclude]
        public List<string> PlaylistFiles = new();

        [JsonPropertyName("playlist_size"), JsonInclude]
        private long? playlistSize = null;

        /// <summary>
        /// The total size in bytes of all files referenced by the playlist if it is a one
        /// </summary>
        public DataSize? PlaylistSize => (playlistSize != null) ? new DataSize(playlistSize.Value, Unit.None) : null;

        /// <summary>
        /// A hexadecimal string of the previous segment's UID (only for Matroska files)
        /// </summary>
        [JsonPropertyName("previous_segment_uid"), JsonInclude]
        public string? PreviousSegmentUID = null;

        /// <summary>
        /// A container describing multiple programs multiplexed into the source file, e.g. multiple programs in one DVB transport stream
        /// </summary>
        [JsonPropertyName("programs"), JsonInclude]
        public List<MkvProgram> Programs = new();

        /// <summary>
        /// A hexadecimal string of the segment's UID (only for Matroska files)
        /// </summary>
        [JsonPropertyName("segment_uid"), JsonInclude]
        public string? SegmentUID = null;

        /// <summary>
        /// Base unit for segment ticks and track ticks, in nanoseconds. A timestamp_scale value of 1.000.000 means scaled timestamps in the segment are expressed in milliseconds.
        /// </summary>
        [JsonPropertyName("timestamp_scale"), JsonInclude]
        public long? TimestampScale = null;

        [JsonPropertyName("title"), JsonInclude]
        public string? Title = null;

        /// <summary>
        /// A Unicode string containing the name and possibly version of the high-level application that created the file
        /// </summary>
        [JsonPropertyName("writing_application"), JsonInclude]
        public string? WritingApplication = null;
    }

    public class MkvProgram
    {

        /// <summary>
        /// A unique number identifying a set of tracks that belong together; used e.g. in DVB for multiplexing multiple stations within a single transport stream
        /// </summary>
        [JsonPropertyName("program_number"), JsonInclude]
        public long? ProgramNumber = null;

        /// <summary>
        /// The name of a service provided by this program, e.g. a TV channel name such as 'arte HD'
        /// </summary>
        [JsonPropertyName("service_name"), JsonInclude]
        public string? ServiceName = null;

        /// <summary>
        /// The name of the provider of the service provided by this program, e.g. a TV station name such as 'ARD'
        /// </summary>
        [JsonPropertyName("service_provider"), JsonInclude]
        public string? ServiceProvider = null;
    }

    public class MkvGlobalTag
    {
        [JsonPropertyName("num_entries"), JsonInclude, JsonRequired]
        public long NumEntries = 0;
    }

    public class MkvTrackTag
    {
        [JsonPropertyName("num_entries"), JsonInclude, JsonRequired]
        public long NumEntries = 0;

        [JsonPropertyName("track_id"), JsonInclude, JsonRequired]
        public long TrackID = 0;
    }

    public enum MkvTrackType
    {
        Unknown,
        Video,
        Audio,
        Subtitles
    }

    public class MkvTrack
    {
        [JsonPropertyName("codec"), JsonInclude, JsonRequired]
        public string Codec = "";

        [JsonPropertyName("id"), JsonInclude, JsonRequired]
        public long ID = -1;

        //public long Index = -1;

        [JsonPropertyName("type"), JsonInclude, JsonRequired, JsonConverter(typeof(JsonStringEnumConverter))]
        public MkvTrackType Type = MkvTrackType.Unknown;

        [JsonPropertyName("properties"), JsonInclude]
        public MkvTrackProperties? Properties = null;
    }

    public enum AacIsSbr
    {
        True,
        False,
        Unknown
    }

    public class MkvTrackProperties
    {
        [JsonPropertyName("aac_is_abr"), JsonInclude, JsonConverter(typeof(JsonStringEnumConverter))]
        public AacIsSbr? AacIsSbr = null;

        [JsonPropertyName("alpha_mode"), JsonInclude]
        public long? AlphaMode = null;

        [JsonPropertyName("audio_bits_per_sample"), JsonInclude]
        public long? AudioBitsPerSample = null;

        [JsonPropertyName("audio_channels"), JsonInclude]
        public long? AudioChannels = null;

        /// <summary>
        /// Audio emphasis applied on audio samples. The player MUST apply the inverse emphasis to get the proper audio samples.
        /// </summary>
        [JsonPropertyName("audio_emphasis"), JsonInclude]
        public long? AudioEmphasis = null;

        [JsonPropertyName("audio_sampling_frequency"), JsonInclude]
        public long? AudioSamplingFrequency = null;

        /// <summary>
        /// ^-?[0-9]+,-?[0-9]+$
        /// </summary>
        [JsonPropertyName("cb_subsample"), JsonInclude]
        public string? CbSubsample = null;

        /// <summary>
        /// ^-?[0-9]+,-?[0-9]+$
        /// </summary>
        [JsonPropertyName("chroma_siting"), JsonInclude]
        public string? ChromaSiting = null;

        /// <summary>
        /// ^-?[0-9]+,-?[0-9]+$
        /// </summary>
        [JsonPropertyName("chroma_subsample"), JsonInclude]
        public string? ChromaSubsample = null;

        /// <summary>
        /// ^-?[0-9]+(\\.[0-9]+)?,-?[0-9]+(\\.[0-9]+)?,-?[0-9]+(\\.[0-9]+)?,-?[0-9]+(\\.[0-9]+)?,-?[0-9]+(\\.[0-9]+)?,-?[0-9]+(\\.[0-9]+)?$
        /// </summary>
        [JsonPropertyName("chromaticity_coordinates"), JsonInclude]
        public string? ChromaticityCoordinates = null;

        [JsonPropertyName("codec_delay"), JsonInclude]
        public long? CodecDelay = null;

        [JsonPropertyName("codec_id"), JsonInclude]
        public string? CodecID = null;

        [JsonPropertyName("codec_name"), JsonInclude]
        public string? CodecName = null;

        [JsonPropertyName("codec_private_data"), JsonInclude]
        public string? CodecPrivateData = null;

        [JsonPropertyName("codec_private_length"), JsonInclude]
        public long? CodecPrivateLength = null;

        [JsonPropertyName("content_encoding_algorithms"), JsonInclude]
        public string? ContentEncodingAlgorithms = null;

        [JsonPropertyName("color_bits_per_channel"), JsonInclude]
        public long? ColorBitsPerChannel = null;

        [JsonPropertyName("color_matrix_coefficients"), JsonInclude]
        public long? ColorMatrixCoefficients = null;

        [JsonPropertyName("color_primaries"), JsonInclude]
        public long? ColorPrimaries = null;

        [JsonPropertyName("color_range"), JsonInclude]
        public long? ColorRange = null;

        [JsonPropertyName("color_transfer_characteristics"), JsonInclude]
        public long? ColorTransferCharacteristics = null;

        [JsonPropertyName("default_duration"), JsonInclude]
        public long? DefaultDuration = null;

        [JsonPropertyName("default_track"), JsonInclude]
        public bool? DefaultTrack = null;

        /// <summary>
        /// ^[0-9]+x[0-9]+$
        /// </summary>
        [JsonPropertyName("display_dimensions"), JsonInclude]
        public string? DisplayDimensions = null;

        [JsonPropertyName("display_unit"), JsonInclude]
        public long? DisplayUnit = null;

        [JsonPropertyName("enabled_track"), JsonInclude]
        public bool? EnabledTrack = null;

        /// <summary>
        /// The encoding/character set of a track containing text (e.g. subtitles) if it can be determined with confidence. For such tracks the encoding cannot be changed by the user.
        /// </summary>
        [JsonPropertyName("encoding"), JsonInclude]
        public string? Encoding = null;

        [JsonPropertyName("forced_track"), JsonInclude]
        public bool? ForcedTrack = null;

        /// <summary>
        /// Can be set if that track is suitable for users with hearing impairments.
        /// </summary>
        [JsonPropertyName("flag_hearing_impaired"), JsonInclude]
        public bool? FlagHearingImpaired = null;

        /// <summary>
        /// Can be set if that track is suitable for users with visual impairments.
        /// </summary>
        [JsonPropertyName("flag_visual_impaired"), JsonInclude]
        public bool? FlagVisualImpaired = null;

        /// <summary>
        /// Can be set if that track contains textual descriptions of video content suitable for playback via a text-to-speech system for a visually-impaired user.
        /// </summary>
        [JsonPropertyName("flag_text_descriptions"), JsonInclude]
        public bool? FlagTextDescriptions = null;

        /// <summary>
        /// Can be set if that track is in the content's original language (not a translation).
        /// </summary>
        [JsonPropertyName("flag_original"), JsonInclude]
        public bool? FlagOriginal = null;

        /// <summary>
        /// Can be set if that track contains commentary.
        /// </summary>
        [JsonPropertyName("flag_commentary"), JsonInclude]
        public bool? FlagCommentary = null;

        /// <summary>
        /// The track's language as an ISO 639-2 language code
        /// </summary>
        [JsonPropertyName("language"), JsonInclude]
        public string? Language = null;

        /// <summary>
        /// The track's language as an IETF BCP 47/RFC 5646 language tag
        /// </summary>
        [JsonPropertyName("language_ietf"), JsonInclude]
        public string? LanguageIETF = null;

        [JsonPropertyName("max_content_light"), JsonInclude]
        public long? MaxContentLight = null;

        [JsonPropertyName("max_frame_light"), JsonInclude]
        public long? MaxFrameLight = null;

        [JsonPropertyName("max_luminance"), JsonInclude]
        public double? MaxLuminance = null;

        [JsonPropertyName("min_luminance"), JsonInclude]
        public double? MinLuminance = null;

        /// <summary>
        /// The minimum timestamp in nanoseconds of all the frames of this track found within the first couple of seconds of the file
        /// </summary>
        [JsonPropertyName("minimum_timestamp"), JsonInclude]
        public long? MinimumTimestamp = null;

        /// <summary>
        /// An array of track IDs indicating which tracks were originally multiplexed within the same track in the source file
        /// </summary>
        [JsonPropertyName("multiplexed_tracks"), JsonInclude]
        public List<long> MultiplexedTracks = new();

        [JsonPropertyName("number"), JsonInclude]
        public long? Number = null;

        [JsonPropertyName("num_index_entries"), JsonInclude]
        public long? NumIndexEntries = null;

        [JsonPropertyName("packetizer"), JsonInclude]
        public string? Packetizer = null;

        /// <summary>
        /// ^[0-9]+x[0-9]+$
        /// </summary>
        [JsonPropertyName("pixel_dimensions"), JsonInclude]
        public string? PixelDimensions = null;

        /// <summary>
        /// A unique number identifying a set of tracks that belong together; used e.g. in DVB for multiplexing multiple stations within a single transport stream
        /// </summary>
        [JsonPropertyName("program_number"), JsonInclude]
        public long? ProgramNumber = null;

        [JsonPropertyName("projection_pose_pitch"), JsonInclude]
        public double? ProjectionPosePitch = null;

        [JsonPropertyName("projection_pose_roll"), JsonInclude]
        public double? ProjectionPoseRoll = null;

        [JsonPropertyName("projection_pose_yaw"), JsonInclude]
        public double? ProjectionPoseYaw = null;

        /// <summary>
        /// ^([0-9A-F]{2})*$
        /// </summary>
        [JsonPropertyName("projection_private"), JsonInclude]
        public string? ProjectionPrivate = null;

        [JsonPropertyName("projection_type"), JsonInclude]
        public long? ProjectionType = null;

        [JsonPropertyName("stereo_mode"), JsonInclude]
        public long? StereoMode = null;

        /// <summary>
        /// A format-specific ID identifying a track, possibly in combination with a 'sub_stream_id' (e.g. the program ID in an MPEG transport stream)
        /// </summary>
        [JsonPropertyName("stream_id"), JsonInclude]
        public long? StreamID = null;

        /// <summary>
        /// A format-specific ID identifying a track together with a 'stream_id'
        /// </summary>
        [JsonPropertyName("sub_stream_id"), JsonInclude]
        public long? SubStreamID = null;

        [JsonPropertyName("teletext_page"), JsonInclude]
        public long? TeletextPage = null;

        [JsonPropertyName("text_subtitles"), JsonInclude]
        public bool? TextSubtitles = false;

        [JsonPropertyName("track_name"), JsonInclude]
        public string? TrackName = null;

        [JsonPropertyName("uid"), JsonInclude]
        public long? UID = null;

        /// <summary>
        /// ^-?[0-9]+(\\.[0-9]+)?,-?[0-9]+(\\.[0-9]+)?$
        /// </summary>
        [JsonPropertyName("white_color_coordinates"), JsonInclude]
        public string? WhiteColorCoordinates = null;
    }
}
