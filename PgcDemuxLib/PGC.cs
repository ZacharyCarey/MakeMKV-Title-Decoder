using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PgcDemuxLib
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

        public readonly int NumberOfCells; // TODO remove
        

        /// <summary>
        /// Playback time, BCD, hh:mm:ss:ff with bits 7&6 of frame (last) byte indicating frame rate.
        /// 11 = 30fps, 10 = illegal, 01 = 25fps, 00 = illegal
        /// </summary>
        public readonly int RawDuration;

        public readonly VTS_CPIT? CellPlaybackInformationTable;
        public readonly VTS_CPsIT? CellPositionInformationTable;

        private PGC(IfoParseContext context, byte[] file, int addr)
        {
            var ifo = context.ifo;
            this.Address = addr;

            this.NumberOfCells = file[addr + 3];
            this.RawDuration = file.GetNbytes(addr + 4 , 4);

            int offset = file.GetNbytes(addr + 0xE8, 2);
            if (offset == 0) this.CellPlaybackInformationTable = null;
            else this.CellPlaybackInformationTable = VTS_CPIT.Parse(file, addr + offset, context, this);

            offset = file.GetNbytes(addr + 0xEA, 2);
            if (offset == 0) this.CellPositionInformationTable = null;
            else this.CellPositionInformationTable = VTS_CPsIT.Parse(file, addr + offset, context, this);
        }

        internal static PGC Parse(byte[] file, int addr, IfoParseContext context)
        {
            PGC result;
            if (!context.VTS_PGC.TryGetValue(addr, out result))
            {
                result = new PGC(context, file, addr);
                context.VTS_PGC[addr] = result;
            }
            return result;
        }
    }

    /// <summary>
    /// https://dvd.sourceforge.net/dvdinfo/pgc.html#pos
    /// </summary>
    public class VTS_CPsIT
    {
        /// <summary>
        /// Byte index in file
        /// </summary>
        internal readonly int Address;

        private readonly CellPositionInfo[] Cells;

        public CellPositionInfo this[int index]
        {
            get => this.Cells[index];
        }

        internal VTS_CPsIT(byte[] file, int addr, PGC pgc)
        {
            this.Address = addr;
            this.Cells = new CellPositionInfo[pgc.NumberOfCells];

            for (int i = 0; i < this.Cells.Length; i++)
            {
                this.Cells[i] = new CellPositionInfo(file, addr + i * 4);
            }
        }

        internal static VTS_CPsIT Parse(byte[] file, int addr, IfoParseContext context, PGC pgc)
        {
            VTS_CPsIT result;
            if (!context.VTS_CPsIT.TryGetValue(addr, out result))
            {
                result = new VTS_CPsIT(file, addr, pgc);
                context.VTS_CPsIT[addr] = result;
            }
            return result;
        }
    }

    public class CellPositionInfo
    {
        public readonly int VobID;
        public readonly int CellID;

        internal CellPositionInfo(byte[] file, int addr)
        {
            this.VobID = file.GetNbytes(addr, 2);
            this.CellID = file[addr + 3];
        }
    }

    /// <summary>
    /// https://dvd.sourceforge.net/dvdinfo/pgc.html#play
    /// </summary>
    public class VTS_CPIT
    {
        /// <summary>
        /// Byte index in file
        /// </summary>
        internal readonly int Address;

        public readonly int NumberOfAngles;

        private readonly CellPlaybackInfo[] Cells;

        public CellPlaybackInfo this[int index]
        {
            get => this.Cells[index];
        }

        internal VTS_CPIT(byte[] file, int addr, PGC pgc)
        {
            this.Address = addr;
            this.Cells = new CellPlaybackInfo[pgc.NumberOfCells];

            this.NumberOfAngles = 1;

            for (int i = 0; i < pgc.NumberOfCells; i++)
            {
                this.Cells[i] = new CellPlaybackInfo(file, addr + 24 * i);
            }

            int nCell;
            bool bEndAngle; 
            for (nCell = 0, bEndAngle = false; nCell < pgc.NumberOfCells && bEndAngle == false; nCell++)
            {
                var cell = this.Cells[nCell];
                if (cell.CellType == CellType.FirstOfAngleBlock && cell.BlockType == BlockType.AngleBlock)
                    this.NumberOfAngles = 1;
                else if (cell.CellType == CellType.MiddleOfAngleBlock && cell.BlockType == BlockType.AngleBlock)
                    this.NumberOfAngles++;
                else if (cell.CellType == CellType.LastOfAngleBlock && cell.BlockType == BlockType.AngleBlock)
                {
                    this.NumberOfAngles++;
                    bEndAngle = true;
                }
            }
        }

        internal static VTS_CPIT Parse(byte[] file, int addr, IfoParseContext context, PGC pgc)
        {
            VTS_CPIT result;
            if (!context.VTS_CPIT.TryGetValue(addr, out result))
            {
                result = new VTS_CPIT(file, addr, pgc);
                context.VTS_CPIT[addr] = result;
            }
            return result;
        }
    }

    public class CellPlaybackInfo
    {
        /// <summary>
        /// cell playback time, BCD, hh:mm:ss:ff with bits 7&6 of frame (last) byte indicating frame rate
        /// 11 = 30 fps, 10 = illegal, 01 = 25 fps, 00 = illegal
        /// </summary>
        public readonly int RawDuration;
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

        internal CellPlaybackInfo(byte[] file, int addr)
        {
            int temp = file[addr];
            this.CellType = Util.ParseEnum<CellType>((temp >> 6) & 0b11);
            this.BlockType = Util.ParseEnum<BlockType>((temp >> 4) & 0b11);
            this.SeamlessPlayback = ((temp >> 3) & 1) != 0;
            this.Interleaved = ((temp >> 2) & 1) != 0;
            this.StcDiscontinuity = ((temp >> 1) & 1) != 0;
            this.SeamlessAngle = (temp & 1) != 0;

            this.RawDuration = file.GetNbytes(addr + 4, 4);

            this.FirstVobuStartSector = file.GetNbytes(addr + 8, 4);
            this.FirstVobuEndSector = file.GetNbytes(addr + 0xC, 4);
            this.LastVobuStartSector = file.GetNbytes(addr + 0x10, 4);
            this.LastVobuEndSector = file.GetNbytes(addr + 0x14, 4);
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
