using PgcDemuxLib.Data.VTS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PgcDemuxLib.Data.VMG
{
    public class VmgIfo : IfoBase
    {
        [JsonInclude]
        public override string ParentFolder { get; protected set; }

        [JsonInclude]
        public readonly string FileName;

        [JsonInclude]
        public override int TitleSet { get; protected set; } = 0;

        [JsonInclude]
        public readonly VersionNumber Version;

        [JsonInclude]
        public readonly int NumberOfVolumes;

        [JsonInclude]
        public readonly int VolumeNumber;

        /// <summary>
        /// This is most likely used for double sides discs, or possibly
        /// multiple discs
        /// </summary>
        [JsonInclude]
        public readonly int SideID;

        [JsonInclude]
        public readonly int NumberOfTitleSets;

        /// <summary>
        /// Usually the broadcast company that released the dvd.
        /// i.e. Warner Home Video
        /// </summary>
        [JsonInclude]
        public readonly string ProviderID;

        [JsonInclude]
        public readonly PGC FirstPlayPGC;

        [JsonInclude]
        public readonly TT_SRPT TitleInfoTable;

        [JsonInclude]
        public override VTSM_PGCI_UT? MenuProgramChainTable { get; protected set; }

        [JsonInclude]
        public readonly VMG_PTL_MAIT? ParentalManagementMasks;

        [JsonInclude]
        public override VTS_C_ADT? MenuCellAddressTable { get; protected set; }

        [JsonInclude]
        public readonly VTS_C_ADT? MenuVobuAddressMap;

        [JsonInclude]
        public readonly VTS_VideoAttributes MenuVideoAttributes;

        [JsonInclude]
        public readonly ReadOnlyArray<VTS_AudioAttributes> MenuAudioAttributes;

        [JsonInclude]
        public readonly VTS_SubpictureAttributes? MenuSubpictureAttributes;

        [JsonIgnore]
        public override VTS_PGCI? TitleProgramChainTable { get; protected set; } = null;

        [JsonIgnore]
        public override VTS_C_ADT? TitleSetCellAddressTable { get; protected set; } = null;

        internal VmgIfo(string folder, string fileName)
        {
            this.ParentFolder = folder;
            FileName = fileName;

            if (FileName.ToUpper() != "VIDEO_TS.IFO")
            {
                throw new Exception("Invalid file name.");
            }

            byte[] file = File.ReadAllBytes(Path.Combine(folder, fileName));

            string header = file.GetString(0, 12);
            if (header != "DVDVIDEO-VMG")
            {
                throw new Exception("Invalid file format.");
            }

            Version = new VersionNumber(file, 0x20);
            NumberOfVolumes = file.GetNbytes(0x26, 2);
            VolumeNumber = file.GetNbytes(0x28, 2);
            SideID = file[0x2A];
            NumberOfTitleSets = file.GetNbytes(0x3E, 2);
            ProviderID = file.GetString(0x40, 32, true);

            int addr = file.GetNbytes(0x84, 4);
            Util.AssertValidAddress(addr, "FP_PGC");
            FirstPlayPGC = new PGC(file.AsSpan(addr), addr, -1, 0, 0, -1);

            addr = Dvd.SECTOR_SIZE * file.GetNbytes(0xC4, 4);
            Util.AssertValidAddress(addr, "TT_SRPT");
            TitleInfoTable = new TT_SRPT(file, addr);

            addr = Dvd.SECTOR_SIZE * file.GetNbytes(0xC8, 4);
            Util.AssertValidAddress(addr, "VMGM_PGCI_UT");
            MenuProgramChainTable = new VTSM_PGCI_UT(file, addr);

            addr = Dvd.SECTOR_SIZE * file.GetNbytes(0xCC, 4);
            ParentalManagementMasks = (addr == 0) ? null : new VMG_PTL_MAIT(file, addr);

            addr = Dvd.SECTOR_SIZE * file.GetNbytes(0xD8, 4);
            MenuCellAddressTable = (addr == 0) ? null : new VTS_C_ADT(file, addr);

            addr = Dvd.SECTOR_SIZE * file.GetNbytes(0xDC, 4);
            MenuVobuAddressMap = (addr == 0) ? null : new VTS_C_ADT(file, addr);

            this.MenuVideoAttributes = new VTS_VideoAttributes(file, 0x100);

            int numAudioStreams = file.GetNbytes(0x102, 2);
            this.MenuAudioAttributes = new ReadOnlyArray<VTS_AudioAttributes>(numAudioStreams);
            for (int i = 0; i < numAudioStreams; i++)
            {
                this.MenuAudioAttributes[i] = new VTS_AudioAttributes(file, 0x104 + 8 * i);
            }

            int numSubpictureAttributes = file.GetNbytes(0x154, 2);
            if (numSubpictureAttributes > 1)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("More than 1 VMGM_VOBS was listed. Only the first one is being read.");
                Console.ResetColor();
                numSubpictureAttributes = 1;
            }
            if (numSubpictureAttributes >= 1) {
                this.MenuSubpictureAttributes = new VTS_SubpictureAttributes(file, 0x156);
            } else
            {
                this.MenuSubpictureAttributes = null;
            }

            OrganizeCells();
        }

        public bool DemuxMenu(string outputFolder, int pgcIndex)
        {
            //string fileName = $"VTS-{this.TitleSet}_PGC-{pgcIndex}_Angle-{angleIndex}.vob";
            //return Demux(Path.Combine(outputFolder, fileName), this.TitleProgramChainTable[pgcIndex], this.SortedTitleCells, angleIndex, options);
            IfoOptions options = new IfoOptions();
            options.Angle = 1;
            options.DomainType = DemuxingDomain.Menus;
            options.ExportVOB = true;
            options.Mode = DemuxingMode.PGC;
            options.PGC = pgcIndex + 1;
            options.CombinedVobName = $"VMG_Menu-{options.PGC:000}.VOB";

            PgcDemux demux = new PgcDemux(this, options, outputFolder);
            bool result = demux.Demux(outputFolder); ;
            demux.Close();
            return result;
        }

        internal override string GetVobPath(int id)
        {
            if (id != 0) throw new ArgumentException();
            return Path.Combine(ParentFolder, $"VIDEO_TS.VOB");
        }
    }
}
