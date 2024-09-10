using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.libs.MakeMKV
{

    public enum ApItemAttributeId
    {
        Unknown = 0,
        Type = 1,
        Name = 2,
        LangCode = 3,
        LangName = 4,
        CodecId = 5,
        CodecShort = 6,
        CodecLong = 7,
        ChapterCount = 8,
        Duration = 9,
        DiskSize = 10,
        DiskSizeBytes = 11,
        StreamTypeExtension = 12,
        Bitrate = 13,
        AudioChannelsCount = 14,
        AngleInfo = 15,
        SourceFileName = 16,
        AudioSampleRate = 17,
        AudioSampleSize = 18,
        VideoSize = 19,
        VideoAspectRatio = 20,
        VideoFrameRate = 21,
        StreamFlags = 22,
        DateTime = 23,
        OriginalTitleId = 24,
        SegmentsCount = 25,
        SegmentsMap = 26,
        OutputFileName = 27,
        MetadataLanguageCode = 28,
        MetadataLanguageName = 29,
        TreeInfo = 30,
        PanelTitle = 31,
        VolumeName = 32,
        OrderWeight = 33,
        OutputFormat = 34,
        OutputFormatDescription = 35,
        SeamlessInfo = 36,
        PanelText = 37,
        MkvFlags = 38,
        MkvFlagsText = 39,
        AudioChannelLayoutName = 40,
        OutputCodecShort = 41,
        OutputConversionType = 42,
        OutputAudioSampleRate = 43,
        OutputAudioSampleSize = 44,
        OutputAudioChannelsCount = 45,
        OutputAudioChannelLayoutName = 46,
        OutputAudioChannelLayout = 47,
        OutputAudioMixDescription = 48,
        Comment = 49,
        OffsetSequenceId = 50,
        MaxValue
    }

    public enum ApSettingId
    {
        Unknown = 0,
        dvd_MinimumTitleLength,
        dvd_TestMTL,
        dvd_SPRemoveMethod,
        app_DataDir,
        app_Key,
        app_KeyHash,
        io_ErrorRetryCount,
        io_IgnoreReadErrors,
        io_RBufSizeMB,
        io_TIPS_Server,
        app_ExpertMode,
        io_DarwinK2Workaround,
        fs_ForceIsoForUDF102,
        app_DestinationType,
        app_DestinationDir,
        app_ShowDebug,
        app_DebugKey,
        app_PreferredLanguage,
        app_BackupDecrypted,
        app_InterfaceLanguage,
        app_UpdateEnable,
        app_UpdateLastCheck,
        io_SingleDrive,
        app_ShowAVSyncMessages,
        bdplus_DumpAlways,
        deprecated_s_EnableUPNP,
        deprecated_s_BindIp,
        deprecated_s_BindPort,
        screen_geometry,
        screen_state,
        app_DefaultProfileName,
        app_DefaultSelectionString,
        app_Java,
        app_ccextractor,
        app_SiteInfoString,
        path_OpenFile,
        path_DestDir,
        path_BackupDirMRU,
        path_DestDirMRU,
        app_DefaultOutputFileName,
        sdf_Stop,
        app_Proxy,
        MaxValue
    }

    public static class APDefs
    {

        public const uint MaxCdromDevices = 16;
        public const uint Progress_MaxValue = 65536;
        public const uint Progress_MaxLayoutItems = 10;
        public const uint UIMSG_BOX_MASK = 3854;
        public const uint UIMSG_BOXOK = 260;
        public const uint UIMSG_BOXERROR = 516;
        public const uint UIMSG_BOXWARNING = 1028;
        public const uint UIMSG_BOXYESNO = 776;
        public const uint UIMSG_BOXYESNO_ERR = 1288;
        public const uint UIMSG_YES = 0;
        public const uint UIMSG_NO = 1;
        public const uint UIMSG_DEBUG = 32;
        public const uint UIMSG_HIDDEN = 64;
        public const uint UIMSG_EVENT = 128;
        public const uint UIMSG_HAVE_URL = 131072;
        public const uint UIMSG_VITEM_BASE = 5200;
        public const uint MMBD_DISC_FLAG_BUSENC = 2;
        public const uint MMBD_MMBD_DISC_FLAG_AACS = 4;
        public const uint MMBD_MMBD_DISC_FLAG_BDPLUS = 8;
        public const uint vastr_Name = 0;
        public const uint vastr_Version = 1;
        public const uint vastr_Platform = 2;
        public const uint vastr_Build = 3;
        public const uint vastr_KeyType = 4;
        public const uint vastr_KeyFeatures = 5;
        public const uint vastr_KeyExpiration = 6;
        public const uint vastr_EvalState = 7;
        public const uint vastr_ProgExpiration = 8;
        public const uint vastr_LatestVersion = 9;
        public const uint vastr_RestartRequired = 10;
        public const uint vastr_ExpertMode = 11;
        public const uint vastr_ProfileCount = 12;
        public const uint vastr_ProgExpired = 13;
        public const uint vastr_OutputFolderName = 14;
        public const uint vastr_OutputBaseName = 15;
        public const uint vastr_CurrentProfile = 16;
        public const uint vastr_OpenFileFilter = 17;
        public const uint vastr_WebSiteURL = 18;
        public const uint vastr_OpenDVDFileFilter = 19;
        public const uint vastr_DefaultSelectionString = 20;
        public const uint vastr_DefaultOutputFileName = 21;
        public const uint vastr_ExternalAppItem = 22;
        public const uint vastr_InterfaceLanguage = 23;
        public const uint vastr_ProfileString = 24;





        public const uint DskFsFlagDvdFilesPresent = 1;
        public const uint DskFsFlagHdvdFilesPresent = 2;
        public const uint DskFsFlagBlurayFilesPresent = 4;
        public const uint DskFsFlagAacsFilesPresent = 8;
        public const uint DskFsFlagBdsvmFilesPresent = 16;


        public const uint DriveStateNoDrive = 256;
        public const uint DriveStateUnmounting = 257;
        public const uint DriveStateEmptyClosed = 0;
        public const uint DriveStateEmptyOpen = 1;
        public const uint DriveStateInserted = 2;
        public const uint DriveStateLoading = 3;


        public const uint Notify_UpdateLayoutFlag_NoTime = 1;
        public const uint ProgressCurrentIndex_SourceName = 65280;
        public const uint BackupFlagDecryptVideo = 1;
        public const uint OpenFlagManualMode = 1;
        public const uint UpdateDrivesFlagNoScan = 1;
        public const uint UpdateDrivesFlagNoSingleDrive = 2;


        public const uint AVStreamFlag_DirectorsComments = 1;
        public const uint AVStreamFlag_AlternateDirectorsComments = 2;
        public const uint AVStreamFlag_ForVisuallyImpaired = 4;
        public const uint AVStreamFlag_CoreAudio = 256;
        public const uint AVStreamFlag_SecondaryAudio = 512;
        public const uint AVStreamFlag_HasCoreAudio = 1024;
        public const uint AVStreamFlag_DerivedStream = 2048;
        public const uint AVStreamFlag_ForcedSubtitles = 4096;
        public const uint AVStreamFlag_ProfileSecondaryStream = 16384;
        public const uint AVStreamFlag_OffsetSequenceIdPresent = 32768;


        public const uint APP_LOC_MAX = 7000;


        public const ulong APP_DUMP_DONE_PARTIAL = 5004;
        public const ulong APP_DUMP_DONE = 5005;
        public const ulong APP_INIT_FAILED = 5009;
        public const ulong APP_ASK_FOLDER_CREATE = 5013;
        public const ulong APP_FOLDER_INVALID = 5016;
        public const ulong PROGRESS_APP_SAVE_MKV_FREE_SPACE = 5033;
        public const ulong PROT_DEMO_KEY_EXPIRED = 5021;
        public const ulong APP_EVAL_TIME_NEVER = 5067;
        public const ulong APP_BACKUP_FAILED = 5069;
        public const ulong APP_BACKUP_COMPLETED = 5070;
        public const ulong APP_BACKUP_COMPLETED_HASHFAIL = 5079;
        public const ulong PROFILE_NAME_DEFAULT = 5086;
        public const ulong VITEM_NAME = 5202;
        public const ulong VITEM_TIMESTAMP = 5223;
        public const ulong APP_IFACE_TITLE = 6000;
        public const ulong APP_CAPTION_MSG = 6001;
        public const ulong APP_ABOUTBOX_TITLE = 6002;
        public const ulong APP_IFACE_OPENFILE_TITLE = 6003;
        public const ulong APP_SETTINGDLG_TITLE = 6135;
        public const ulong APP_BACKUPDLG_TITLE = 6136;
        public const ulong APP_IFACE_OPENFILE_FILTER_TEMPLATE1 = 6007;
        public const ulong APP_IFACE_OPENFILE_FILTER_TEMPLATE2 = 6008;
        public const ulong APP_IFACE_OPENFOLDER_TITLE = 6005;
        public const ulong APP_IFACE_OPENFOLDER_INFO_TITLE = 6006;
        public const ulong APP_IFACE_PROGRESS_TITLE = 6038;
        public const ulong APP_IFACE_PROGRESS_ELAPSED_ONLY = 6039;
        public const ulong APP_IFACE_PROGRESS_ELAPSED_ETA = 6040;
        public const ulong APP_IFACE_ACT_OPENFILES_NAME = 6010;
        public const ulong APP_IFACE_ACT_OPENFILES_SKEY = 6011;
        public const ulong APP_IFACE_ACT_OPENFILES_STIP = 6012;
        public const ulong APP_IFACE_ACT_OPENFILES_DVD_NAME = 6024;
        public const ulong APP_IFACE_ACT_OPENFILES_DVD_STIP = 6026;
        public const ulong APP_IFACE_ACT_CLOSEDISK_NAME = 6013;
        public const ulong APP_IFACE_ACT_CLOSEDISK_STIP = 6014;
        public const ulong APP_IFACE_ACT_SETFOLDER_NAME = 6015;
        public const ulong APP_IFACE_ACT_SETFOLDER_STIP = 6016;
        public const ulong APP_IFACE_ACT_SAVEALLMKV_NAME = 6017;
        public const ulong APP_IFACE_ACT_SAVEALLMKV_STIP = 6018;
        public const ulong APP_IFACE_ACT_CANCEL_NAME = 6036;
        public const ulong APP_IFACE_ACT_CANCEL_STIP = 6037;
        public const ulong APP_IFACE_ACT_STREAMING_NAME = 6131;
        public const ulong APP_IFACE_ACT_STREAMING_STIP = 6132;
        public const ulong APP_IFACE_ACT_BACKUP_NAME = 6133;
        public const ulong APP_IFACE_ACT_BACKUP_STIP = 6134;
        public const ulong APP_IFACE_ACT_QUIT_NAME = 6019;
        public const ulong APP_IFACE_ACT_QUIT_SKEY = 6020;
        public const ulong APP_IFACE_ACT_QUIT_STIP = 6021;
        public const ulong APP_IFACE_ACT_ABOUT_NAME = 6022;
        public const ulong APP_IFACE_ACT_ABOUT_STIP = 6023;
        public const ulong APP_IFACE_ACT_SETTINGS_NAME = 6042;
        public const ulong APP_IFACE_ACT_SETTINGS_STIP = 6043;
        public const ulong APP_IFACE_ACT_HELPPAGE_NAME = 6045;
        public const ulong APP_IFACE_ACT_HELPPAGE_STIP = 6046;
        public const ulong APP_IFACE_ACT_REGISTER_NAME = 6047;
        public const ulong APP_IFACE_ACT_REGISTER_STIP = 6048;
        public const ulong APP_IFACE_ACT_PURCHASE_NAME = 6145;
        public const ulong APP_IFACE_ACT_PURCHASE_STIP = 6146;
        public const ulong APP_IFACE_ACT_CLEARLOG_NAME = 6110;
        public const ulong APP_IFACE_ACT_CLEARLOG_STIP = 6111;
        public const ulong APP_IFACE_ACT_EJECT_NAME = 6052;
        public const ulong APP_IFACE_ACT_EJECT_STIP = 6053;
        public const ulong APP_IFACE_ACT_REVERT_NAME = 6105;
        public const ulong APP_IFACE_ACT_REVERT_STIP = 6106;
        public const ulong APP_IFACE_ACT_NEWINSTANCE_NAME = 6107;
        public const ulong APP_IFACE_ACT_NEWINSTANCE_STIP = 6108;
        public const ulong APP_IFACE_ACT_OPENDISC_DVD = 6062;
        public const ulong APP_IFACE_ACT_OPENDISC_HDDVD = 6063;
        public const ulong APP_IFACE_ACT_OPENDISC_BRAY = 6064;
        public const ulong APP_IFACE_ACT_OPENDISC_LOADING = 6065;
        public const ulong APP_IFACE_ACT_OPENDISC_UNKNOWN = 6099;
        public const ulong APP_IFACE_ACT_OPENDISC_NODISC = 6109;
        public const ulong APP_IFACE_ACT_TTREE_TOGGLE = 6066;
        public const ulong APP_IFACE_ACT_TTREE_SELECT_ALL = 6067;
        public const ulong APP_IFACE_ACT_TTREE_UNSELECT_ALL = 6068;
        public const ulong APP_IFACE_MENU_FILE = 6030;
        public const ulong APP_IFACE_MENU_VIEW = 6031;
        public const ulong APP_IFACE_MENU_HELP = 6032;
        public const ulong APP_IFACE_MENU_TOOLBAR = 6034;
        public const ulong APP_IFACE_MENU_SETTINGS = 6044;
        public const ulong APP_IFACE_MENU_DRIVES = 6035;
        public const ulong APP_IFACE_CANCEL_CONFIRM = 6041;
        public const ulong APP_IFACE_FATAL_COMM = 6050;
        public const ulong APP_IFACE_FATAL_MEM = 6051;
        public const ulong APP_IFACE_GUI_VERSION = 6054;
        public const ulong APP_IFACE_LATEST_VERSION = 6158;
        public const ulong APP_IFACE_LICENSE_TYPE = 6055;
        public const ulong APP_IFACE_EVAL_STATE = 6056;
        public const ulong APP_IFACE_EVAL_EXPIRATION = 6057;
        public const ulong APP_IFACE_PROG_EXPIRATION = 6142;
        public const ulong APP_IFACE_WEBSITE_URL = 6159;
        public const ulong APP_IFACE_VIDEO_FOLDER_NAME_WIN = 6058;
        public const ulong APP_IFACE_VIDEO_FOLDER_NAME_MAC = 6059;
        public const ulong APP_IFACE_VIDEO_FOLDER_NAME_LINUX = 6060;
        public const ulong APP_IFACE_DEFAULT_FOLDER_NAME = 6061;
        public const ulong APP_IFACE_MAIN_FRAME_INFO = 6069;
        public const ulong APP_IFACE_MAIN_FRAME_MAKE_MKV = 6070;
        public const ulong APP_IFACE_MAIN_FRAME_PROFILE = 6180;
        public const ulong APP_IFACE_MAIN_FRAME_PROPERTIES = 6181;
        public const ulong APP_IFACE_EMPTY_FRAME_INFO = 6075;
        public const ulong APP_IFACE_EMPTY_FRAME_SOURCE = 6071;
        public const ulong APP_IFACE_EMPTY_FRAME_TYPE = 6072;
        public const ulong APP_IFACE_EMPTY_FRAME_LABEL = 6073;
        public const ulong APP_IFACE_EMPTY_FRAME_PROTECTION = 6074;
        public const ulong APP_IFACE_EMPTY_FRAME_DVD_MANUAL = 6084;
        public const ulong APP_IFACE_REGISTER_TEXT = 6076;
        public const ulong APP_IFACE_REGISTER_CODE_INCORRECT = 6077;
        public const ulong APP_IFACE_REGISTER_CODE_NOT_SAVED = 6078;
        public const ulong APP_IFACE_REGISTER_CODE_SAVED = 6079;
        public const ulong APP_IFACE_SETTINGS_IO_OPTIONS = 6080;
        public const ulong APP_IFACE_SETTINGS_IO_AUTO = 6081;
        public const ulong APP_IFACE_SETTINGS_IO_READ_RETRY = 6082;
        public const ulong APP_IFACE_SETTINGS_IO_READ_BUFFER = 6083;
        public const ulong APP_IFACE_SETTINGS_IO_NO_DIRECT_ACCESS = 6150;
        public const ulong APP_IFACE_SETTINGS_IO_DARWIN_K2_WORKAROUND = 6151;
        public const ulong APP_IFACE_SETTINGS_IO_SINGLE_DRIVE = 6168;
        public const ulong APP_IFACE_SETTINGS_DVD_AUTO = 6085;
        public const ulong APP_IFACE_SETTINGS_DVD_MIN_LENGTH = 6086;
        public const ulong APP_IFACE_SETTINGS_DVD_SP_REMOVE = 6087;
        public const ulong APP_IFACE_SETTINGS_AACS_KEY_DIR = 6088;
        public const ulong APP_IFACE_SETTINGS_BDP_MISC = 6129;
        public const ulong APP_IFACE_SETTINGS_BDP_DUMP_ALWAYS = 6130;
        public const ulong APP_IFACE_SETTINGS_DEST_TYPE_NONE = 6089;
        public const ulong APP_IFACE_SETTINGS_DEST_TYPE_AUTO = 6090;
        public const ulong APP_IFACE_SETTINGS_DEST_TYPE_SEMIAUTO = 6091;
        public const ulong APP_IFACE_SETTINGS_DEST_TYPE_CUSTOM = 6092;
        public const ulong APP_IFACE_SETTINGS_DESTDIR = 6093;
        public const ulong APP_IFACE_SETTINGS_GENERAL_MISC = 6094;
        public const ulong APP_IFACE_SETTINGS_LOG_DEBUG_MSG = 6095;
        public const ulong APP_IFACE_SETTINGS_DATA_DIR = 6167;
        public const ulong APP_IFACE_SETTINGS_EXPERT_MODE = 6169;
        public const ulong APP_IFACE_SETTINGS_SHOW_AVSYNC = 6170;
        public const ulong APP_IFACE_SETTINGS_GENERAL_ONLINE_UPDATES = 6188;
        public const ulong APP_IFACE_SETTINGS_ENABLE_INTERNET_ACCESS = 6187;
        public const ulong APP_IFACE_SETTINGS_PROXY_SERVER = 6189;
        public const ulong APP_IFACE_SETTINGS_TAB_GENERAL = 6096;
        public const ulong APP_IFACE_SETTINGS_MSG_FAILED = 6097;
        public const ulong APP_IFACE_SETTINGS_MSG_RESTART = 6098;
        public const ulong APP_IFACE_SETTINGS_TAB_LANGUAGE = 6152;
        public const ulong APP_IFACE_SETTINGS_LANG_INTERFACE = 6153;
        public const ulong APP_IFACE_SETTINGS_LANG_PREFERRED = 6154;
        public const ulong APP_IFACE_SETTINGS_LANGUAGE_AUTO = 6156;
        public const ulong APP_IFACE_SETTINGS_LANGUAGE_NONE = 6157;
        public const ulong APP_IFACE_SETTINGS_TAB_IO = 6164;
        public const ulong APP_IFACE_SETTINGS_TAB_STREAMING = 6165;
        public const ulong APP_IFACE_SETTINGS_TAB_PROT = 6166;
        public const ulong APP_IFACE_SETTINGS_TAB_ADVANCED = 6172;
        public const ulong APP_IFACE_SETTINGS_ADV_DEFAULT_PROFILE = 6173;
        public const ulong APP_IFACE_SETTINGS_ADV_DEFAULT_SELECTION = 6174;
        public const ulong APP_IFACE_SETTINGS_ADV_EXTERN_EXEC_PATH = 6175;
        public const ulong APP_IFACE_SETTINGS_PROT_JAVA_PATH = 6177;
        public const ulong APP_IFACE_SETTINGS_ADV_OUTPUT_FILE_NAME_TEMPLATE = 6178;
        public const ulong APP_IFACE_SETTINGS_TAB_INTEGRATION = 6190;
        public const ulong APP_IFACE_SETTINGS_INT_TEXT = 6191;
        public const ulong APP_IFACE_SETTINGS_INT_HDR_PATH = 6192;
        public const ulong APP_IFACE_BACKUPDLG_TEXT_CAPTION = 6137;
        public const ulong APP_IFACE_BACKUPDLG_TEXT = 6138;
        public const ulong APP_IFACE_BACKUPDLG_FOLDER = 6139;
        public const ulong APP_IFACE_BACKUPDLG_OPTIONS = 6147;
        public const ulong APP_IFACE_BACKUPDLG_DECRYPT = 6148;
        public const ulong APP_IFACE_DRIVEINFO_LOADING = 6100;
        public const ulong APP_IFACE_DRIVEINFO_UNMOUNTING = 6112;
        public const ulong APP_IFACE_DRIVEINFO_WAIT = 6101;
        public const ulong APP_IFACE_DRIVEINFO_NODISC = 6102;
        public const ulong APP_IFACE_DRIVEINFO_DATADISC = 6103;
        public const ulong APP_IFACE_DRIVEINFO_NONE = 6104;
        public const ulong APP_IFACE_FLAGS_DIRECTORS_COMMENTS = 6125;
        public const ulong APP_IFACE_FLAGS_ALT_DIRECTORS_COMMENTS = 6126;
        public const ulong APP_IFACE_FLAGS_SECONDARY_AUDIO = 6127;
        public const ulong APP_IFACE_FLAGS_FOR_VISUALLY_IMPAIRED = 6128;
        public const ulong APP_IFACE_FLAGS_CORE_AUDIO = 6143;
        public const ulong APP_IFACE_FLAGS_FORCED_SUBTITLES = 6144;
        public const ulong APP_IFACE_FLAGS_PROFILE_SECONDARY_STREAM = 6171;
        public const ulong APP_IFACE_ITEMINFO_SOURCE = 6119;
        public const ulong APP_IFACE_ITEMINFO_TITLE = 6120;
        public const ulong APP_IFACE_ITEMINFO_TRACK = 6121;
        public const ulong APP_IFACE_ITEMINFO_ATTACHMENT = 6122;
        public const ulong APP_IFACE_ITEMINFO_CHAPTER = 6123;
        public const ulong APP_IFACE_ITEMINFO_CHAPTERS = 6124;
        public const ulong APP_TTREE_TITLE = 6200;
        public const ulong APP_TTREE_VIDEO = 6201;
        public const ulong APP_TTREE_AUDIO = 6202;
        public const ulong APP_TTREE_SUBPICTURE = 6203;
        public const ulong APP_TTREE_ATTACHMENT = 6214;
        public const ulong APP_TTREE_CHAPTERS = 6215;
        public const ulong APP_TTREE_CHAPTER = 6216;
        public const ulong APP_TTREE_FORCED_SUBTITLES = 6211;
        public const ulong APP_TTREE_HDR_TYPE = 6204;
        public const ulong APP_TTREE_HDR_DESC = 6205;
        public const ulong DVD_TYPE_DISK = 6206;
        public const ulong BRAY_TYPE_DISK = 6209;
        public const ulong HDDVD_TYPE_DISK = 6212;
        public const ulong MKV_TYPE_FILE = 6213;
        public const ulong APP_TTREE_CHDESC = 6207;
        public const ulong APP_TTREE_ANGLE_DESC = 6210;
        public const ulong APP_DVD_MANUAL_TITLE = 6220;
        public const ulong APP_DVD_MANUAL_TEXT = 6225;
        public const ulong APP_DVD_TITLES_COUNT = 6221;
        public const ulong APP_DVD_COUNT_CELLS = 6222;
        public const ulong APP_DVD_COUNT_PGC = 6223;
        public const ulong APP_DVD_BROKEN_TITLE_ENTRY = 6224;
        public const ulong APP_SINGLE_DRIVE_TITLE = 6226;
        public const ulong APP_SINGLE_DRIVE_TEXT = 6227;
        public const ulong APP_SINGLE_DRIVE_ALL = 6228;
        public const ulong APP_SINGLE_DRIVE_CAPTION = 6229;
        public const ulong APP_SI_DRIVEINFO = 6300;
        public const ulong APP_SI_PROFILE = 6301;
        public const ulong APP_SI_MANUFACTURER = 6302;
        public const ulong APP_SI_PRODUCT = 6303;
        public const ulong APP_SI_REVISION = 6304;
        public const ulong APP_SI_SERIAL = 6305;
        public const ulong APP_SI_FIRMWARE = 6306;
        public const ulong APP_SI_FIRDATE = 6307;
        public const ulong APP_SI_BECFLAGS = 6308;
        public const ulong APP_SI_HIGHEST_AACS = 6309;
        public const ulong APP_SI_DISCINFO = 6320;
        public const ulong APP_SI_NODISC = 6321;
        public const ulong APP_SI_DISCLOAD = 6322;
        public const ulong APP_SI_CAPACITY = 6323;
        public const ulong APP_SI_DISCTYPE = 6324;
        public const ulong APP_SI_DISCSIZE = 6325;
        public const ulong APP_SI_DISCRATE = 6326;
        public const ulong APP_SI_DISCLAYERS = 6327;
        public const ulong APP_SI_DISCCBL = 6329;
        public const ulong APP_SI_DISCCBL25 = 6330;
        public const ulong APP_SI_DISCCBL27 = 6331;
        public const ulong APP_SI_DEVICE = 6332;

    }
}
