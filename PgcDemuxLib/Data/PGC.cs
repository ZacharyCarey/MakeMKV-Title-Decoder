using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PgcDemuxLib.Data
{
    /// <summary>
    /// https://dvd.sourceforge.net/dvdinfo/pgc.html
    /// </summary>
    public class PGC
    {
        /// <summary>
        /// Byte index in file
        /// </summary>
        [JsonInclude]
        internal readonly int Address;

        [JsonInclude]
        public readonly int ID;

        [JsonInclude]
        public int NumberOfPrograms;

        [JsonIgnore]
        public int NumberOfCells => CellInfo.Length;

        [JsonInclude]
        public readonly int NumberOfAngles;

        /// <summary>
        /// -1 = infinite
        /// </summary>
        [JsonInclude]
        public readonly int StillTime;

        /// <summary>
        /// Playback time, BCD, hh:mm:ss:ff with bits 7&6 of frame (last) byte indicating frame rate.
        /// 11 = 30fps, 10 = illegal, 01 = 25fps, 00 = illegal
        /// </summary>
        [JsonInclude]
        public readonly TimeSpan Duration;

        [JsonInclude]
        public readonly CellInfo[] CellInfo;

        [JsonInclude]
        public readonly bool IsEntryPGC;

        [JsonInclude, JsonConverter(typeof(JsonStringEnumConverter))]
        public readonly MenuType MenuType;

        [JsonInclude]
        public readonly int TitleNumber;

        /// <summary>
        /// PGCN = Program Chain Number
        /// </summary>
        [JsonInclude]
        public readonly int NextPGCN;

        /// <summary>
        /// PGCN = Program Chain Number
        /// </summary>
        [JsonInclude]
        public readonly int PreviousPGCN;

        /// <summary>
        /// PGCN = Program Chain Number
        /// </summary>
        [JsonInclude]
        public readonly int GroupPGCN;

        internal PGC(ReadOnlySpan<byte> data, int globalAddr, int ID, int isEntryPGC, int menuMask, int titleNumber)
        {
            Address = globalAddr;
            this.ID = ID;
            ReadOnlySpan<byte> header = data.Slice(0, 236);

            IsEntryPGC = (isEntryPGC == 1);
            MenuType = Util.ParseEnum<MenuType>(menuMask);
            this.TitleNumber = titleNumber;

            this.NumberOfPrograms = header[2];
            int numCells = header[3];
            Duration = Util.ParseDuration(header.GetNbytes(4, 4));

            this.NextPGCN = header.GetNbytes(0x9C, 2);
            this.PreviousPGCN = header.GetNbytes(0x9E, 2);
            this.GroupPGCN = header.GetNbytes(0xA0, 2);

            this.StillTime = header[0xA2];
            if (this.StillTime == 255)
            {
                this.StillTime = -1;
            }

            int cellPlaybackInfoTableAddr = header.GetNbytes(0xE8, 2);
            int cellPositionInfoTableAddr = header.GetNbytes(0xEA, 2);
            if ((cellPlaybackInfoTableAddr == 0 || cellPositionInfoTableAddr == 0) && numCells != 0)
            // There is something wrong...
            {
                throw new Exception($"ERROR: There is something wrong in number of cells in PGC.");
            }

            if (numCells > 0) {
                ReadOnlySpan<byte> cellPlaybackInfoTable = data.Slice(cellPlaybackInfoTableAddr);
                ReadOnlySpan<byte> cellPositionInfoTable = data.Slice(cellPositionInfoTableAddr);

                this.CellInfo = new CellInfo[numCells];
                for(int i = 0; i < CellInfo.Length; i++)
                {
                    this.CellInfo[i] = new CellInfo(cellPositionInfoTable.Slice(i * 4, 4), cellPlaybackInfoTable.Slice(i * 24, 24));
                }

                // Find all angles
                NumberOfAngles = 1;
                for (int cell = 0; cell < numCells; cell++)
                {
                    var info = CellInfo[cell];
                    if (info.IsFirstAngle)
                        NumberOfAngles = 1;
                    else if (info.IsMiddleAngle)
                        NumberOfAngles++;
                    else if (info.IsLastAngle)
                    {
                        NumberOfAngles++;
                        break;
                    }
                }
            } else
            {
                this.CellInfo = Array.Empty<CellInfo>();
                this.NumberOfAngles = 0;
            }


        }
    }

    public enum MenuType
    {
        None = 0,
        Title = 2,
        Root = 3,
        Subpicture = 4,
        Audio = 5,
        Angle = 6,
        Chapter = 7
    }

    /// <summary>
    /// A combination of cell position info and cell playback info
    /// https://dvd.sourceforge.net/dvdinfo/pgc.html#pos
    /// https://dvd.sourceforge.net/dvdinfo/pgc.html#play
    /// </summary>
    public class CellInfo
    {
        // Cell position info
        [JsonInclude]
        public readonly int VobID;

        [JsonInclude]
        public readonly int CellID;

        // Cell playback info
        /// <summary>
        /// cell playback time, BCD, hh:mm:ss:ff with bits 7&6 of frame (last) byte indicating frame rate
        /// 11 = 30 fps, 10 = illegal, 01 = 25 fps, 00 = illegal
        /// </summary>
        [JsonInclude]
        public readonly TimeSpan Duration;

        [JsonInclude, JsonConverter(typeof(JsonStringEnumConverter))]
        public readonly CellType CellType;

        [JsonInclude, JsonConverter(typeof(JsonStringEnumConverter))]
        public readonly BlockType BlockType;

        [JsonInclude]
        public readonly bool SeamlessPlayback;

        [JsonInclude]
        public readonly bool Interleaved;

        [JsonInclude]
        public readonly bool StcDiscontinuity;

        [JsonInclude]
        public readonly bool SeamlessAngle;

        [JsonInclude]
        public readonly int FirstVobuStartSector;

        [JsonInclude]
        public readonly int FirstVobuEndSector;

        [JsonInclude]
        public readonly int LastVobuStartSector;

        [JsonInclude]
        public readonly int LastVobuEndSector;

        public bool IsNormal => (CellType == CellType.Normal && BlockType == BlockType.Normal);
        public bool IsFirstAngle => (CellType == CellType.FirstOfAngleBlock && BlockType == BlockType.AngleBlock);
        public bool IsMiddleAngle => (CellType == CellType.MiddleOfAngleBlock && BlockType == BlockType.AngleBlock);
        public bool IsLastAngle => (CellType == CellType.LastOfAngleBlock && BlockType == BlockType.AngleBlock);

        internal CellInfo(ReadOnlySpan<byte> cellPositionAddr, ReadOnlySpan<byte> cellPlaybackAddr)
        {
            // Cell position info
            VobID = cellPositionAddr.GetNbytes(0, 2);
            CellID = cellPositionAddr[3];

            // Cell playback info
            int cellCategory = cellPlaybackAddr[0];
            CellType = Util.ParseEnum<CellType>(cellCategory >> 6 & 0b11);
            BlockType = Util.ParseEnum<BlockType>(cellCategory >> 4 & 0b11);
            SeamlessPlayback = (cellCategory >> 3 & 1) != 0;
            Interleaved = (cellCategory >> 2 & 1) != 0;
            StcDiscontinuity = (cellCategory >> 1 & 1) != 0;
            SeamlessAngle = (cellCategory & 1) != 0;

            Duration = Util.ParseDuration(cellPlaybackAddr.GetNbytes(4, 4));

            FirstVobuStartSector = cellPlaybackAddr.GetNbytes(8, 4);
            FirstVobuEndSector = cellPlaybackAddr.GetNbytes(0xC, 4);
            LastVobuStartSector = cellPlaybackAddr.GetNbytes(0x10, 4);
            LastVobuEndSector = cellPlaybackAddr.GetNbytes(0x14, 4);
        }
    }

    public enum CellType
    {
        Normal = 0,
        FirstOfAngleBlock = 1,
        MiddleOfAngleBlock = 2,
        LastOfAngleBlock = 3
    }

    public enum BlockType
    {
        Normal = 0,
        AngleBlock = 1
    }
}
