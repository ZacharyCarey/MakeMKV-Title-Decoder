using JsonSerializable;
using MakeMKV_Title_Decoder.MkvToolNix.MkvToolNix;
using MakeMKV_Title_Decoder.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.MkvToolNix.Data {
    public class MkvMergeID : MkvToolNixData, IJsonSerializable {

        /// <summary>
        /// An array describing the attachments found, if any
        /// </summary>
        public List<MkvAttachment> Attachments = new();

        public List<MkvChapter> Chapters = new();

        /// <summary>
        /// Information about the identified container
        /// </summary>
        public MkvContainer? Container = null;

        /// <summary>
        /// The identified file's name
        /// </summary>
        public string FileName = "";

        public DataSize FileSize = new();
        public string FileDirectory;

        public List<MkvGlobalTag> GlobalTags = new();

        /// <summary>
        /// The output format's version
        /// </summary>
        public long? IdentificationFormatVersion = new();

        public List<MkvTrackTag> TrackTags = new();

        public List<MkvTrack> Tracks = new();
        private Dictionary<MkvTrackType, long> trackIndexCount = new();

        public List<string> Errors = new();
        public List<string> Warnings = new();

        public MkvMergeID(string root, string directory, string fileName) {
            this.FileDirectory = directory;
            this.FileName = fileName;
            try
            {
                string path = Path.Combine(root, directory, fileName);
                this.FileSize = new DataSize(new FileInfo(path).Length, Unit.None);
            } catch (Exception)
            {
                this.FileSize = new();
            }
        }

        public string GetRelativePath() {
            try
            {
                return Path.Combine(this.FileDirectory, this.FileName);
            } catch(Exception)
            {
                // Full path didnt work, just return the file name as a backup
                return this.FileName;
            }
        }

        public string GetFullPath(MkvToolNixDisc disc) {
            try
            {
                return Path.Combine(disc.RootPath, this.FileDirectory, this.FileName);
            } catch(Exception)
            {
                // Full path didnt work, just return the file name as a backup
                return this.FileName;
            }
        }

        public bool Parse(IEnumerable<string> std, IProgress<SimpleProgress>? progress = null) {
            try
            {
                Stream stream = new SequentialStream(std);
                /*try
                {
                    File.Delete("JsonData.json");
                } catch (Exception) { }
                using (FileStream file = File.OpenWrite("JsonData.json"))
                {
                    byte[] buffer = new byte[1024];
                    while (true)
                    {
                        int readBytes = stream.Read(buffer, 0, 1024);
                        if (readBytes > 0)
                        {
                            file.Write(buffer, 0, readBytes);
                        }
                        if (readBytes < 1024)
                        {
                            break;
                        }
                    }
                    file.Flush();
                }*/

                Json.Read(stream, this);
                return true;
            }catch(Exception ex)
            {
                return false;
            }
        }

        public void LoadFromJson(JsonData Data) {
            var obj = (JsonObject)Data;
            this.Attachments.LoadFromJson(obj["attachments"]);
            this.Chapters.LoadFromJson(obj["chapters"]);
            this.Container = MkvToolNixUtils.ParseOptionalObject<MkvContainer>(obj["container"]);
            this.Errors.LoadFromJson(obj["errors"]);
            /*this.FileName = MkvToolNixUtils.ParseOptional<JsonString>(obj["file_name"]) ?? (string?)null;
            if (FileName != null)
            {
                try
                {
                    this.FileName = Path.GetFileName(this.FileName);
                } catch (Exception) { }
            }*/

            this.GlobalTags.LoadFromJson(obj["global_tags"]);
            this.IdentificationFormatVersion = MkvToolNixUtils.ParseOptional<JsonInteger>(obj["identification_format_version"]);
            this.TrackTags.LoadFromJson(obj["track_tags"]);
            this.Tracks.LoadFromJson(obj["tracks"]);
            this.Warnings.LoadFromJson(obj["warnings"]);

            foreach(var track in this.Tracks)
            {
                if (track != null)
                {
                    if (!this.trackIndexCount.TryGetValue(track.Type, out track.Index))
                    {
                        track.Index = 0;
                    }
                    this.trackIndexCount[track.Type] = (track.Index + 1);
                }
            }
        }

        public JsonData SaveToJson() {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return this.FileName ?? "N/A";
        }
    }

    public class MkvAttachment : IJsonSerializable {

        public string? ContentType = null;
        public string? Description = null;
        public string? FileName = null;
        public long? Id = null;
        public long? Size = null;
        public MkvAttachmentProperties? Properties = new();
        public string? Type = null;

        public void LoadFromJson(JsonData Data) {
            var obj = (JsonObject)Data;

            this.ContentType = MkvToolNixUtils.ParseOptional<JsonString>(obj["content_type"]) ?? (string?)null;
            this.Description = MkvToolNixUtils.ParseOptional<JsonString>(obj["description"]) ?? (string?)null;
            this.FileName = MkvToolNixUtils.ParseOptional<JsonString>(obj["file_name"]) ?? (string?)null;
            this.Id = MkvToolNixUtils.ParseOptional<JsonInteger>(obj["id"]) ?? (long?)null;
            this.Size = MkvToolNixUtils.ParseOptional<JsonInteger>(obj["size"]) ?? (long?)null;
            this.Properties = MkvToolNixUtils.ParseOptionalObject<MkvAttachmentProperties>(obj["properties"]);
            this.Type = MkvToolNixUtils.ParseOptional<JsonString>(obj["type"]) ?? (string?)null;
        }

        public JsonData SaveToJson() {
            throw new NotImplementedException();
        }
    }

    public class MkvAttachmentProperties : IJsonSerializable {
        public long? UID = null;

        public void LoadFromJson(JsonData Data) {
            var obj = (JsonObject)Data;
            this.UID = MkvToolNixUtils.ParseOptional<JsonInteger>(obj["uid"]) ?? (long?)null;
        }

        public JsonData SaveToJson() {
            throw new NotImplementedException();
        }
    }

    public class MkvChapter : IJsonSerializable {
        /// <summary>
        /// Required
        /// </summary>
        public long NumEntries = 0;

        public void LoadFromJson(JsonData Data) {
            var obj = (JsonObject)Data;
            this.NumEntries = (JsonInteger)obj["num_entries"];
        }

        public JsonData SaveToJson() {
            throw new NotImplementedException();
        }
    }

    public class MkvContainer : IJsonSerializable {

        /// <summary>
        /// Additional properties for the container varying by container format.
        /// </summary>
        public MkvContainerProperties? Properties = new();

        /// <summary>
        /// Required.
        /// States whether or not mkvmerge knows about the format
        /// </summary>
        public bool Recognized = false;

        /// <summary>
        /// Required.
        /// States whether or not mkvmerge can read the format
        /// </summary>
        public bool Supported = false;

        /// <summary>
        /// A human-readable description/name for the container format
        /// </summary>
        public string? Type = null;

        public void LoadFromJson(JsonData Data) {
            var obj = (JsonObject)Data;
            this.Properties = MkvToolNixUtils.ParseOptionalObject<MkvContainerProperties>(obj["properties"]);
            this.Recognized = (JsonBool)obj["recognized"];
            this.Supported = (JsonBool)obj["supported"];
            this.Type = MkvToolNixUtils.ParseOptional<JsonString>(obj["type"]) ?? (string?)null;
        }

        public JsonData SaveToJson() {
            throw new NotImplementedException();
        }
    }

    public enum ContainerType {
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

    public class MkvContainerProperties : IJsonSerializable {

        /// <summary>
        /// A unique number identifying the container type that's supposed to stay constant over all future releases of MKVToolNix
        /// </summary>
        public ContainerType? ContainerType = null;

        /// <summary>
        /// The muxing date in ISO 8601 format (in local time zone)
        /// ^[0-9]{4}-[0-9]{2}-[0-9]{2}T[0-9]{2}:[0-9]{2}:[0-9]{2}([+-][0-9]{2}:[0-9]{2}|Z)$
        /// </summary>
        public string? DateLocal = null;

        /// <summary>
        /// The muxing date in ISO 8601 format (in UTC)
        /// ^[0-9]{4}-[0-9]{2}-[0-9]{2}T[0-9]{2}:[0-9]{2}:[0-9]{2}([+-][0-9]{2}:[0-9]{2}|Z)$
        /// </summary>
        public string? DateUTC = null;

        /// <summary>
        /// The file's/segment's duration in nanoseconds
        /// </summary>
        public TimeSpan? Duration = null;

        /// <summary>
        /// States whether or not the container has timestamps for the packets (e.g. Matroska, MP4) or not (e.g. SRT, MP3)
        /// </summary>
        public bool? IsProvidingTimestamps = null;

        /// <summary>
        /// A Unicode string containing the name and possibly version of the low-level library or application that created the file
        /// </summary>
        public string? MuxingApplication = null;

        /// <summary>
        /// A hexadecimal string of the next segment's UID (only for Matroska files)
        /// </summary>
        public string? NextSegmentUID = null;

        /// <summary>
        /// An array of names of additional files processed as well
        /// </summary>
        public List<string> OtherFiles = new();

        /// <summary>
        /// States whether or not the identified file is a playlist (e.g. MPLS) referring to several other files
        /// </summary>
        public bool? Playlist = null;

        /// <summary>
        /// The number of chapters in a playlist if it is a one
        /// </summary>
        public long? PlaylistChapters = null;

        /// <summary>
        /// The total duration in nanoseconds of all files referenced by the playlist if it is a one
        /// </summary>
        public TimeSpan? PlaylistDuration = null;

        /// <summary>
        /// An array of file names the playlist contains
        /// </summary>
        public List<string> PlaylistFiles = new();

        /// <summary>
        /// The total size in bytes of all files referenced by the playlist if it is a one
        /// </summary>
        public DataSize? PlaylistSize = null;

        /// <summary>
        /// A hexadecimal string of the previous segment's UID (only for Matroska files)
        /// </summary>
        public string? PreviousSegmentUID = null;

        /// <summary>
        /// A container describing multiple programs multiplexed into the source file, e.g. multiple programs in one DVB transport stream
        /// </summary>
        public List<MkvProgram> Programs = new();

        /// <summary>
        /// A hexadecimal string of the segment's UID (only for Matroska files)
        /// </summary>
        public string? SegmentUID = null;

        // TODO is this used for the duration???
        /// <summary>
        /// Base unit for segment ticks and track ticks, in nanoseconds. A timestamp_scale value of 1.000.000 means scaled timestamps in the segment are expressed in milliseconds.
        /// </summary>
        public long? TimestampScale = null;

        public string? Title = null;

        /// <summary>
        /// A Unicode string containing the name and possibly version of the high-level application that created the file
        /// </summary>
        public string? WritingApplication = null;

        public void LoadFromJson(JsonData Data) {
            var obj = (JsonObject)Data;

            this.ContainerType = MkvToolNixUtils.ParseEnum(obj["container_type"], MakeMKV_Title_Decoder.MkvToolNix.Data.ContainerType.is_unknown);
            this.DateLocal = MkvToolNixUtils.ParseOptional<JsonString>(obj["date_local"]) ?? (string?)null;
            this.DateUTC = MkvToolNixUtils.ParseOptional<JsonString>(obj["date_utc"]) ?? (string?)null;

            var duration = MkvToolNixUtils.ParseOptional<JsonInteger>(obj["duration"]) ?? (long?)null;
            if (duration != null)
            {
                this.Duration = new TimeSpan(duration.Value / 100);
            } else
            {
                this.Duration = null;
            }

            this.IsProvidingTimestamps = MkvToolNixUtils.ParseOptional<JsonBool>(obj["is_providing_timestamps"]) ?? (bool?)null;
            this.MuxingApplication = MkvToolNixUtils.ParseOptional<JsonString>(obj["muxing_application"]) ?? (string?)null;
            this.NextSegmentUID = MkvToolNixUtils.ParseOptional<JsonString>(obj["next_segment_uid"]) ?? (string?)null;
            this.OtherFiles.LoadFromJson(obj["other_file"]);
            this.Playlist = MkvToolNixUtils.ParseOptional<JsonBool>(obj["playlist"]) ?? (bool?)null;
            this.PlaylistChapters = MkvToolNixUtils.ParseOptional<JsonInteger>(obj["playlist_chapters"]) ?? (long?)null;

            var playlistDuration = MkvToolNixUtils.ParseOptional<JsonInteger>(obj["playlist_duration"]) ?? (long?)null;
            if (playlistDuration != null)
            {
                this.PlaylistDuration = new TimeSpan(playlistDuration.Value / 100);
            } else
            {
                this.PlaylistDuration = null;
            }

            this.PlaylistFiles.LoadFromJson(obj["playlist_file"]);

            var size = MkvToolNixUtils.ParseOptional<JsonInteger>(obj["playlist_size"]) ?? (long?)null;
            if (size != null)
            {
                this.PlaylistSize = new DataSize(size.Value, Unit.None);
            } else
            {
                this.PlaylistSize = null;
            }

            this.PreviousSegmentUID = MkvToolNixUtils.ParseOptional<JsonString>(obj["previous_segment_uid"]) ?? (string?)null;
            this.Programs.LoadFromJson(obj["programs"]);
            this.SegmentUID = MkvToolNixUtils.ParseOptional<JsonString>(obj["segment_uid"]) ?? (string?)null;
            this.TimestampScale = MkvToolNixUtils.ParseOptional<JsonInteger>(obj["timestamp_scale"]) ?? (long?)null;
            this.Title = MkvToolNixUtils.ParseOptional<JsonString>(obj["title"]) ?? (string?)null;
            this.WritingApplication = MkvToolNixUtils.ParseOptional<JsonString>(obj["writing_application"]) ?? (string?)null;
        }

        public JsonData SaveToJson() {
            throw new NotImplementedException();
        }
    }

    public class MkvProgram : IJsonSerializable {

        /// <summary>
        /// A unique number identifying a set of tracks that belong together; used e.g. in DVB for multiplexing multiple stations within a single transport stream
        /// </summary>
        public long? ProgramNumber = null;

        /// <summary>
        /// The name of a service provided by this program, e.g. a TV channel name such as 'arte HD'
        /// </summary>
        public string? ServiceName = null;

        /// <summary>
        /// The name of the provider of the service provided by this program, e.g. a TV station name such as 'ARD'
        /// </summary>
        public string? ServiceProvider = null;

        public void LoadFromJson(JsonData Data) {
            var obj = (JsonObject)Data;

            this.ProgramNumber = MkvToolNixUtils.ParseOptional<JsonInteger>(obj["program_number"]) ?? (long?)null;
            this.ServiceName = MkvToolNixUtils.ParseOptional<JsonString>(obj["service_name"]) ?? (string?)null;
            this.ServiceProvider = MkvToolNixUtils.ParseOptional<JsonString>(obj["service_provider"]) ?? (string?)null;
        }

        public JsonData SaveToJson() {
            throw new NotImplementedException();
        }
    }

    public class MkvGlobalTag : IJsonSerializable {
        /// <summary>
        /// Required.
        /// </summary>
        public long NumEntries = 0;

        public void LoadFromJson(JsonData Data) {
            var obj = (JsonObject)Data;
            this.NumEntries = (JsonInteger)obj["num_entries"];
        }

        public JsonData SaveToJson() {
            throw new NotImplementedException();
        }
    }

    public class MkvTrackTag : IJsonSerializable {
        /// <summary>
        /// Required.
        /// </summary>
        public long NumEntries = 0;

        /// <summary>
        /// Required.
        /// </summary>
        public long TrackID = 0;

        public void LoadFromJson(JsonData Data) {
            var obj = (JsonObject)Data;
            this.NumEntries = (JsonInteger)obj["num_entries"];
            this.TrackID = (JsonInteger)obj["track_id"];
        }

        public JsonData SaveToJson() {
            throw new NotImplementedException();
        }
    }

    public enum MkvTrackType {
        Unknown,
        Video,
        Audio,
        Subtitles
    }

    public class MkvTrack : IJsonSerializable {
        /// <summary>
        /// Required.
        /// </summary>
        public string Codec = "";

        /// <summary>
        /// Required.
        /// </summary>
        public long ID = -1;

        public long Index = -1;

        /// <summary>
        /// Required.
        /// </summary>
        public MkvTrackType Type = MkvTrackType.Unknown;

        public MkvTrackProperties? Properties = null;

        public void LoadFromJson(JsonData Data) {
            var obj = (JsonObject)Data;
            this.Codec = (JsonString)obj["codec"];
            this.ID = (JsonInteger)obj["id"];
            this.Type = MkvToolNixUtils.ParseEnumName(obj["type"], MkvTrackType.Unknown);
            this.Properties = MkvToolNixUtils.ParseOptionalObject<MkvTrackProperties>(obj["properties"]);
        }

        public JsonData SaveToJson() {
            throw new NotImplementedException();
        }
    }

    public enum AacIsSbr {
        True,
        False,
        Unknown
    }

    public class MkvTrackProperties : IJsonSerializable {

        public AacIsSbr? AacIsSbr = null;
        public long? AlphaMode = null;
        public long? AudioBitsPerSample = null;
        public long? AudioChannels = null;

        /// <summary>
        /// Audio emphasis applied on audio samples. The player MUST apply the inverse emphasis to get the proper audio samples.
        /// </summary>
        public long? AudioEmphasis = null;

        public long? AudioSamplingFrequency = null;

        /// <summary>
        /// ^-?[0-9]+,-?[0-9]+$
        /// </summary>
        public string? CbSubsample = null;

        /// <summary>
        /// ^-?[0-9]+,-?[0-9]+$
        /// </summary>
        public string? ChromaSiting = null;

        /// <summary>
        /// ^-?[0-9]+,-?[0-9]+$
        /// </summary>
        public string? ChromaSubsample = null;

        /// <summary>
        /// ^-?[0-9]+(\\.[0-9]+)?,-?[0-9]+(\\.[0-9]+)?,-?[0-9]+(\\.[0-9]+)?,-?[0-9]+(\\.[0-9]+)?,-?[0-9]+(\\.[0-9]+)?,-?[0-9]+(\\.[0-9]+)?$
        /// </summary>
        public string? ChromaticityCoordinates = null;

        public long? CodecDelay = null;
        public string? CodecID = null;
        public string? CodecName = null;
        public string? CodecPrivateData = null;
        public long? CodecPrivateLength = null;
        public string? ContentEncodingAlgorithms = null;
        public long? ColorBitsPerChannel = null;
        public long? ColorMatrixCoefficients = null;
        public long? ColorPrimaries = null;
        public long? ColorRange = null;
        public long? ColorTransferCharacteristics = null;
        public long? DefaultDuration = null;
        public bool? DefaultTrack = null;

        /// <summary>
        /// ^[0-9]+x[0-9]+$
        /// </summary>
        public string? DisplayDimensions = null;

        public long? DisplayUnit = null;
        public bool? EnabledTrack = null;

        /// <summary>
        /// The encoding/character set of a track containing text (e.g. subtitles) if it can be determined with confidence. For such tracks the encoding cannot be changed by the user.
        /// </summary>
        public string? Encoding = null;

        public bool? ForcedTrack = null;

        /// <summary>
        /// Can be set if that track is suitable for users with hearing impairments.
        /// </summary>
        public bool? FlagHearingImpaired = null;

        /// <summary>
        /// Can be set if that track is suitable for users with visual impairments.
        /// </summary>
        public bool? FlagVisualImpaired = null;

        /// <summary>
        /// Can be set if that track contains textual descriptions of video content suitable for playback via a text-to-speech system for a visually-impaired user.
        /// </summary>
        public bool? FlagTextDescriptions = null;

        /// <summary>
        /// Can be set if that track is in the content's original language (not a translation).
        /// </summary>
        public bool? FlagOriginal = null;

        /// <summary>
        /// Can be set if that track contains commentary.
        /// </summary>
        public bool? FlagCommentary = null;

        /// <summary>
        /// The track's language as an ISO 639-2 language code
        /// </summary>
        public string? Language = null;

        /// <summary>
        /// The track's language as an IETF BCP 47/RFC 5646 language tag
        /// </summary>
        public string? LanguageIETF = null;

        public long? MaxContentLight = null;
        public long? MaxFrameLight = null;
        public double? MaxLuminance = null;
        public double? MinLuminance = null;

        /// <summary>
        /// The minimum timestamp in nanoseconds of all the frames of this track found within the first couple of seconds of the file
        /// </summary>
        public long? MinimumTimestamp = null;

        /// <summary>
        /// An array of track IDs indicating which tracks were originally multiplexed within the same track in the source file
        /// </summary>
        public List<long> MultiplexedTracks = new();

        public long? Number = null;
        public long? NumIndexEntries = null;
        public string? Packetizer = null;

        /// <summary>
        /// ^[0-9]+x[0-9]+$
        /// </summary>
        public string? PixelDimensions = null;

        /// <summary>
        /// A unique number identifying a set of tracks that belong together; used e.g. in DVB for multiplexing multiple stations within a single transport stream
        /// </summary>
        public long? ProgramNumber = null;

        public double? ProjectionPosePitch = null;
        public double? ProjectionPoseRoll = null;
        public double? ProjectionPoseYaw = null;

        /// <summary>
        /// ^([0-9A-F]{2})*$
        /// </summary>
        public string? ProjectionPrivate = null;

        public long? ProjectionType = null;
        public long? StereoMode = null;

        /// <summary>
        /// A format-specific ID identifying a track, possibly in combination with a 'sub_stream_id' (e.g. the program ID in an MPEG transport stream)
        /// </summary>
        public long? StreamID = null;

        /// <summary>
        /// A format-specific ID identifying a track together with a 'stream_id'
        /// </summary>
        public long? SubStreamID = null;

        public long? TeletextPage = null;
        public bool? TextSubtitles = false;
        public string? TrackName = null;
        public long? UID = null;

        /// <summary>
        /// ^-?[0-9]+(\\.[0-9]+)?,-?[0-9]+(\\.[0-9]+)?$
        /// </summary>
        public string? WhiteColorCoordinates = null;

        public void LoadFromJson(JsonData Data) {
            var obj = (JsonObject)Data;

            this.AacIsSbr = MkvToolNixUtils.ParseEnumName(obj["aac_is_sbr"], MakeMKV_Title_Decoder.MkvToolNix.Data.AacIsSbr.Unknown);
            this.AlphaMode = MkvToolNixUtils.ParseOptional<JsonInteger>(obj["alpha_mode"]) ?? (long?)null;
            this.AudioBitsPerSample = MkvToolNixUtils.ParseOptional<JsonInteger>(obj["audio_bits_per_sample"]) ?? (long?)null;
            this.AudioChannels = MkvToolNixUtils.ParseOptional<JsonInteger>(obj["audio_channels"]) ?? (long?)null;
            this.AudioEmphasis = MkvToolNixUtils.ParseOptional<JsonInteger>(obj["audio_emphasis"]) ?? (long?)null;
            this.AudioSamplingFrequency = MkvToolNixUtils.ParseOptional<JsonInteger>(obj["audio_sampling_frequency"]) ?? (long?)null;
            this.CbSubsample = MkvToolNixUtils.ParseOptional<JsonString>(obj["cb_subsample"]) ?? (string?)null;
            this.ChromaSiting = MkvToolNixUtils.ParseOptional<JsonString>(obj["chroma_siting"]) ?? (string?)null;
            this.ChromaSubsample = MkvToolNixUtils.ParseOptional<JsonString>(obj["chroma_subsample"]) ?? (string?)null;
            this.ChromaticityCoordinates = MkvToolNixUtils.ParseOptional<JsonString>(obj["chromaticity_coordinates"]) ?? (string?)null;
            this.CodecDelay = MkvToolNixUtils.ParseOptional<JsonInteger>(obj["codec_delay"]) ?? (long?)null;
            this.CodecID = MkvToolNixUtils.ParseOptional<JsonString>(obj["codec_id"]) ?? (string?)null;
            this.CodecName = MkvToolNixUtils.ParseOptional<JsonString>(obj["codec_name"]) ?? (string?)null;
            this.CodecPrivateData = MkvToolNixUtils.ParseOptional<JsonString>(obj["codec_private_data"]) ?? (string?)null;
            this.CodecPrivateLength = MkvToolNixUtils.ParseOptional<JsonInteger>(obj["codec_private_length"]) ?? (long?)null;
            this.ContentEncodingAlgorithms = MkvToolNixUtils.ParseOptional<JsonString>(obj["content_encoding_algorithms"]) ?? (string?)null;
            this.ColorBitsPerChannel = MkvToolNixUtils.ParseOptional<JsonInteger>(obj["color_bits_per_channel"]) ?? (long?)null;
            this.ColorMatrixCoefficients = MkvToolNixUtils.ParseOptional<JsonInteger>(obj["color_matrix_coefficients"]) ?? (long?)null;
            this.ColorPrimaries = MkvToolNixUtils.ParseOptional<JsonInteger>(obj["color_primaries"]) ?? (long?)null;
            this.ColorRange = MkvToolNixUtils.ParseOptional<JsonInteger>(obj["color_range"]) ?? (long?)null;
            this.ColorTransferCharacteristics = MkvToolNixUtils.ParseOptional<JsonInteger>(obj["color_transfer_characteristics"]) ?? (long?)null;
            this.DefaultDuration = MkvToolNixUtils.ParseOptional<JsonInteger>(obj["default_duration"]) ?? (long?)null;
            this.DefaultTrack = MkvToolNixUtils.ParseOptional<JsonBool>(obj["default_track"]) ?? (bool?)null;
            this.DisplayDimensions = MkvToolNixUtils.ParseOptional<JsonString>(obj["display_dimensions"]) ?? (string?)null;
            this.DisplayUnit = MkvToolNixUtils.ParseOptional<JsonInteger>(obj["display_unit"]) ?? (long?)null;
            this.EnabledTrack = MkvToolNixUtils.ParseOptional<JsonBool>(obj["enabled_track"]) ?? (bool?)null;
            this.Encoding = MkvToolNixUtils.ParseOptional<JsonString>(obj["encoding"]) ?? (string?)null;
            this.ForcedTrack = MkvToolNixUtils.ParseOptional<JsonBool>(obj["forced_track"]) ?? (bool?)null;
            this.FlagHearingImpaired = MkvToolNixUtils.ParseOptional<JsonBool>(obj["flag_hearing_impaired"]) ?? (bool?)null;
            this.FlagVisualImpaired = MkvToolNixUtils.ParseOptional<JsonBool>(obj["flag_visual_impaired"]) ?? (bool?)null;
            this.FlagTextDescriptions = MkvToolNixUtils.ParseOptional<JsonBool>(obj["flag_text_descriptions"]) ?? (bool?)null;
            this.FlagOriginal = MkvToolNixUtils.ParseOptional<JsonBool>(obj["flag_original"]) ?? (bool?)null;
            this.FlagCommentary = MkvToolNixUtils.ParseOptional<JsonBool>(obj["flag_commentary"]) ?? (bool?)null;
            this.Language = MkvToolNixUtils.ParseOptional<JsonString>(obj["language"]) ?? (string?)null;
            this.LanguageIETF = MkvToolNixUtils.ParseOptional<JsonString>(obj["language_ietf"]) ?? (string?)null;
            this.MaxContentLight = MkvToolNixUtils.ParseOptional<JsonInteger>(obj["max_content_light"]) ?? (long?)null;
            this.MaxFrameLight = MkvToolNixUtils.ParseOptional<JsonInteger>(obj["max_frame_light"]) ?? (long?)null;
            this.MaxLuminance = MkvToolNixUtils.ParseOptional<JsonDecimal>(obj["max_luminance"]) ?? (double?)null;
            this.MinLuminance = MkvToolNixUtils.ParseOptional<JsonDecimal>(obj["min_luminance"]) ?? (double?)null;
            this.MinimumTimestamp = MkvToolNixUtils.ParseOptional<JsonInteger>(obj["minimum_timestamp"]) ?? (long?)null;
            this.MultiplexedTracks.LoadFromJson(obj["multiplexed_tracks"]);
            this.Number = MkvToolNixUtils.ParseOptional<JsonInteger>(obj["number"]) ?? (long?)null;
            this.NumIndexEntries = MkvToolNixUtils.ParseOptional<JsonInteger>(obj["num_index_entries"]) ?? (long?)null;
            this.Packetizer = MkvToolNixUtils.ParseOptional<JsonString>(obj["packetizer"]) ?? (string?)null;
            this.PixelDimensions = MkvToolNixUtils.ParseOptional<JsonString>(obj["pixel_dimensions"]) ?? (string?)null;
            this.ProgramNumber = MkvToolNixUtils.ParseOptional<JsonInteger>(obj["program_number"]) ?? (long?)null;
            this.ProjectionPosePitch = MkvToolNixUtils.ParseOptional<JsonDecimal>(obj["projection_pose_pitch"]) ?? (double?)null;
            this.ProjectionPoseRoll = MkvToolNixUtils.ParseOptional<JsonDecimal>(obj["projection_pose_roll"]) ?? (double?)null;
            this.ProjectionPoseYaw = MkvToolNixUtils.ParseOptional<JsonDecimal>(obj["projection_pose_yaw"]) ?? (double?)null;
            this.ProjectionPrivate = MkvToolNixUtils.ParseOptional<JsonString>(obj["projection_private"]) ?? (string?)null;
            this.ProjectionType = MkvToolNixUtils.ParseOptional<JsonInteger>(obj["projection_type"]) ?? (long?)null;
            this.StereoMode = MkvToolNixUtils.ParseOptional<JsonInteger>(obj["stereo_mode"]) ?? (long?)null;
            this.StreamID = MkvToolNixUtils.ParseOptional<JsonInteger>(obj["stream_id"]) ?? (long?)null;
            this.SubStreamID = MkvToolNixUtils.ParseOptional<JsonInteger>(obj["sub_stream_id"]) ?? (long?)null;
            this.TeletextPage = MkvToolNixUtils.ParseOptional<JsonInteger>(obj["teletext_page"]) ?? (long?)null;
            this.TextSubtitles = MkvToolNixUtils.ParseOptional<JsonBool>(obj["text_subtitles"]) ?? (bool?)null;
            this.TrackName = MkvToolNixUtils.ParseOptional<JsonString>(obj["track_name"]) ?? (string?)null;
            this.UID = MkvToolNixUtils.ParseOptional<JsonInteger>(obj["uid"]) ?? (long?)null;
            this.WhiteColorCoordinates = MkvToolNixUtils.ParseOptional<JsonString>(obj["white_color_coordinates"]) ?? (string?)null;
        }

        public JsonData SaveToJson() {
            throw new NotImplementedException();
        }
    }
}
