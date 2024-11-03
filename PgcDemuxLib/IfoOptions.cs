using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PgcDemuxLib {
    public class CustomVobOptions {
        /// <summary>
        /// one file per vob_id
        /// </summary>
        public bool SplitVOB = false;
        public bool WriteNavPacks = true;
        public bool WriteVideoPacks = true;
        public bool WriteAudioPacks = true;
        public bool WriteSubPacks = true;
        public bool OnlyFirstIFrame = false;
        public bool PatchLbaNumber = true;
    }

    public struct IfoOptions {

        public DemuxingMode Mode = DemuxingMode.PGC;

        /// <summary>
        /// Selects the PGC number (from 1 to nPGCs). Default 1
        /// </summary>
        public int PGC = 1;

        /// <summary>
        /// Selects the Angle number (from 1 to n). Default 1
        /// </summary>
        public int Angle = 1;

        /// <summary>
        /// Default 1
        /// </summary>
        public int VID = 1;

        /// <summary>
        /// Selects a cell vobid (from 1 to n). Default 1
        /// </summary>
        public int CID = 1;

        /// <summary>
        /// Extracts/No extracts video file. Default NO
        /// </summary>
        public bool ExtractVideo = false;

        /// <summary>
        /// Extracts/No extracts audio streams. Default NO
        /// </summary>
        public bool ExtractAudio = false;

        /// <summary>
        /// Extracts/No extracts subs streams. Default NO
        /// </summary>
        public bool ExtractSubtitles = false;

        /// <summary>
        /// Generates a single PGC VOB. Default NO
        /// </summary>
        public bool ExportVOB = false;

        /// <summary>
        /// Generates a custom VOB file. Default NULL
        /// </summary>
        public CustomVobOptions CustomVOB = null;

        /// <summary>
        /// Generates a Celltimes.txt file. Default NO
        /// </summary>
        public bool GenerateCellTimes = false;

        /// <summary>
        /// Includes Last end time in Celltimes.txt. Default NO
        /// </summary>
        public bool IncludeEndTime = false;

        /// <summary>
        /// Generates a log file. Default NO
        /// </summary>
        public bool GenerateLog = false;

        /// <summary>
        /// Domain. Default Title (except if filename is VIDEO_TS.IFO)
        /// </summary>
        public DemuxingDomain DomainType = DemuxingDomain.Titles;

        public string? VobName = null;

        public IfoOptions() {

        }
    }
}
