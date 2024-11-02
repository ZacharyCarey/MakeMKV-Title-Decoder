using PgcDemuxLib.Data.VMG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PgcDemuxLib.Data.VTS
{
    public class VtsIfo
    {
        [JsonInclude]
        public readonly string FileName;

        [JsonInclude]
        public readonly int TitleSet;

        [JsonInclude]
        public readonly VersionNumber Version;

        [JsonInclude]
        public readonly VTS_PTT_SRPT TitleAndChapterInfoTable;

        [JsonInclude]
        public readonly VTS_PGCI TitleProgramChainTable;

        [JsonInclude]
        public readonly VTSM_PGCI_UT? MenuProgramChainTable;

        [JsonInclude]
        public readonly VTS_TMAPTI TimeMap;

        [JsonInclude]
        public readonly VTS_C_ADT? MenuCellAddressTable;

        [JsonInclude]
        public readonly VTS_C_ADT MenuVobuAddressMap;

        [JsonInclude]
        public readonly VTS_C_ADT? TitleSetCellAddressTable;

        [JsonInclude]
        public readonly VTS_C_ADT TitleSetVobuAddressMap;

        [JsonInclude]
        public readonly VTS_VideoAttributes MenuVideoAttributes;

        [JsonInclude]
        public readonly ReadOnlyArray<VTS_AudioAttributes> MenuAudioAttributes;

        [JsonInclude]
        public readonly VTS_SubpictureAttributes? MenuSubpictureAttributes;

        [JsonInclude]
        public readonly VTS_VideoAttributes TitleSetVideoAttributes;

        [JsonInclude]
        public readonly ReadOnlyArray<VTS_AudioAttributes> TitleSetAudioAttributes;

        [JsonInclude]
        public readonly ReadOnlyArray<VTS_SubpictureAttributes> TitleSetSubpictureAttributes;

        internal VtsIfo(string folder, string fileName)
        {
            FileName = fileName;

            if (fileName[..4].ToUpper() != "VTS_") throw new Exception("Invalid file name.");
            if (fileName[^6..].ToUpper() != "_0.IFO") throw new Exception("Invalid file name.");
            this.TitleSet = int.Parse(fileName[4..^6]);

            byte[] file = File.ReadAllBytes(Path.Combine(folder, fileName));

            string header = file.GetString(0, 12);
            if (header != "DVDVIDEO-VTS")
            {
                throw new Exception("Invalid file format.");
            }

            Version = new VersionNumber(file, 0x20);

            int addr = PgcDemux.SECTOR_SIZE * file.GetNbytes(0xC8, 4);
            Util.AssertValidAddress(addr, "VTS_PTT_SRPT");
            TitleAndChapterInfoTable = new VTS_PTT_SRPT(file, addr);

            addr = PgcDemux.SECTOR_SIZE * file.GetNbytes(0xCC, 4);
            Util.AssertValidAddress(addr, "VTS_PGCI");
            TitleProgramChainTable = new VTS_PGCI(file, addr);

            addr = PgcDemux.SECTOR_SIZE * file.GetNbytes(0xD0, 4);
            MenuProgramChainTable = (addr == 0) ? null : new VTSM_PGCI_UT(file, addr);

            addr = PgcDemux.SECTOR_SIZE * file.GetNbytes(0xD4, 4);
            Util.AssertValidAddress(addr, "VTS_TMAPTI");
            TimeMap = new VTS_TMAPTI(file, addr);

            addr = PgcDemux.SECTOR_SIZE * file.GetNbytes(0xD8, 4);
            MenuCellAddressTable = (addr == 0) ? null : new VTS_C_ADT(file, addr);

            addr = PgcDemux.SECTOR_SIZE * file.GetNbytes(0xDC, 4);
            Util.AssertValidAddress(addr, "VTSM_VOBU_ADMAP");
            MenuVobuAddressMap = new VTS_C_ADT(file, addr);

            addr = PgcDemux.SECTOR_SIZE * file.GetNbytes(0xE0, 4);
            TitleSetCellAddressTable = (addr == 0) ? null : new VTS_C_ADT(file, addr);

            addr = PgcDemux.SECTOR_SIZE * file.GetNbytes(0xE4, 4);
            Util.AssertValidAddress(addr, "VTS_VOBU_ADMAP");
            TitleSetVobuAddressMap = new VTS_C_ADT(file, addr);

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
            if (numSubpictureAttributes >= 1)
            {
                this.MenuSubpictureAttributes = new VTS_SubpictureAttributes(file, 0x156);
            }
            else
            {
                this.MenuSubpictureAttributes = null;
            }

            TitleSetVideoAttributes = new VTS_VideoAttributes(file, 0x200);

            numAudioStreams = file.GetNbytes(0x202, 2);
            this.TitleSetAudioAttributes = new ReadOnlyArray<VTS_AudioAttributes>(numAudioStreams);
            for (int i = 0; i < numAudioStreams; i++)
            {
                this.TitleSetAudioAttributes[i] = new VTS_AudioAttributes(file, 0x204 + 8 * i);
            }

            numSubpictureAttributes = file.GetNbytes(0x254, 2);
            this.TitleSetSubpictureAttributes = new ReadOnlyArray<VTS_SubpictureAttributes>(numSubpictureAttributes);
            for (int i = 0; i < numSubpictureAttributes; i++)
            {
                this.TitleSetSubpictureAttributes[i] = new VTS_SubpictureAttributes(file, 0x256 + i * 6);
            }
        }
    }
}
