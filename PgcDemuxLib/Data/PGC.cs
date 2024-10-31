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
        internal readonly int Address;

        [JsonInclude]
        public readonly int ID;

        public int NumberOfCells => CellInfo.Length;

        [JsonInclude]
        public readonly int NumberOfAngles;


        /// <summary>
        /// Playback time, BCD, hh:mm:ss:ff with bits 7&6 of frame (last) byte indicating frame rate.
        /// 11 = 30fps, 10 = illegal, 01 = 25fps, 00 = illegal
        /// </summary>
        [JsonInclude]
        public readonly TimeSpan Duration;

        [JsonInclude]
        public readonly CellInfo[] CellInfo;

        internal PGC(byte[] file, int addr, int ID)
        {
            Address = addr;
            this.ID = ID;

            int numCells = file[addr + 3];
            Duration = Util.ParseDuration(file.GetNbytes(addr + 4, 4));

            int cellPlaybackInfoTableAddr = file.GetNbytes(addr + 0xE8, 2);
            if (cellPlaybackInfoTableAddr != 0) cellPlaybackInfoTableAddr += addr;

            int cellPositionInfoTableAddr = file.GetNbytes(addr + 0xEA, 2);
            if (cellPositionInfoTableAddr != 0) cellPositionInfoTableAddr += addr;

            if ((cellPlaybackInfoTableAddr == 0 || cellPositionInfoTableAddr == 0) && numCells != 0)
            // There is something wrong...
            {
                throw new Exception($"ERROR: There is something wrong in number of cells in PGC.");
            }

            if (numCells > 0) {
                this.CellInfo = new CellInfo[numCells];
                for(int i = 0; i < CellInfo.Length; i++)
                {
                    int cellPositionInfoAddr = cellPositionInfoTableAddr + 4 * i;
                    int cellPlaybackInfoAddr = cellPlaybackInfoTableAddr + 24 * i;
                    this.CellInfo[i] = new CellInfo(file, cellPositionInfoAddr, cellPlaybackInfoAddr);
                }

                // Find all angles
                NumberOfAngles = 1;
                for (int cell = 0; cell < numCells; cell++)
                {
                    var info = CellInfo[cell];
                    if (info.CellType == CellType.FirstOfAngleBlock && info.BlockType == BlockType.AngleBlock)
                        NumberOfAngles = 1;
                    else if (info.CellType == CellType.MiddleOfAngleBlock && info.BlockType == BlockType.AngleBlock)
                        NumberOfAngles++;
                    else if (info.CellType == CellType.LastOfAngleBlock && info.BlockType == BlockType.AngleBlock)
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

    /// <summary>
    /// A combination of cell position info and cell playback info
    /// https://dvd.sourceforge.net/dvdinfo/pgc.html#pos
    /// https://dvd.sourceforge.net/dvdinfo/pgc.html#play
    /// </summary>
    public class CellInfo
    {
        // Cell position info
        public readonly int VobID;
        public readonly int CellID;

        // Cell playback info
        /// <summary>
        /// cell playback time, BCD, hh:mm:ss:ff with bits 7&6 of frame (last) byte indicating frame rate
        /// 11 = 30 fps, 10 = illegal, 01 = 25 fps, 00 = illegal
        /// </summary>
        public readonly TimeSpan RawDuration;
        public readonly CellType CellType;
        public readonly BlockType BlockType;
        public readonly bool SeamlessPlayback;
        public readonly bool Interleaved;
        public readonly bool StcDiscontinuity;
        public readonly bool SeamlessAngle;
        public readonly int FirstVobuStartSector;
        public readonly int FirstVobuEndSector;
        public readonly int LastVobuStartSector;
        public readonly int LastVobuEndSector;

        internal CellInfo(byte[] file, int cellPositionAddr, int cellPlaybackAddr)
        {
            // Cell position info
            VobID = file.GetNbytes(cellPositionAddr, 2);
            CellID = file[cellPositionAddr + 3];

            // Cell playback info
            int temp = file[cellPlaybackAddr];
            CellType = Util.ParseEnum<CellType>(temp >> 6 & 0b11);
            BlockType = Util.ParseEnum<BlockType>(temp >> 4 & 0b11);
            SeamlessPlayback = (temp >> 3 & 1) != 0;
            Interleaved = (temp >> 2 & 1) != 0;
            StcDiscontinuity = (temp >> 1 & 1) != 0;
            SeamlessAngle = (temp & 1) != 0;

            RawDuration = Util.ParseDuration(file.GetNbytes(cellPlaybackAddr + 4, 4));

            FirstVobuStartSector = file.GetNbytes(cellPlaybackAddr + 8, 4);
            FirstVobuEndSector = file.GetNbytes(cellPlaybackAddr + 0xC, 4);
            LastVobuStartSector = file.GetNbytes(cellPlaybackAddr + 0x10, 4);
            LastVobuEndSector = file.GetNbytes(cellPlaybackAddr + 0x14, 4);
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
