using PgcDemuxLib.Data.VMG;
using PgcDemuxLib.Data.VTS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;

namespace PgcDemuxLib
{
    public class Dvd
    {
        internal const int SECTOR_SIZE = 2048;

        [JsonInclude]
        public readonly string Folder;

        [JsonInclude]
        public readonly VmgIfo VMG;

        [JsonInclude]
        public readonly ReadOnlyArray<VtsIfo> TitleSets;

        private Dvd(string folder)
        {
            folder = Path.Combine(folder, "VIDEO_TS");
            if (!Directory.Exists(folder))
            {
                throw new DirectoryNotFoundException($"Failed to find directory: {folder}");
            }
            this.Folder = folder;

            this.VMG = new VmgIfo(folder, "VIDEO_TS.IFO");

            this.TitleSets = new ReadOnlyArray<VtsIfo>(VMG.NumberOfTitleSets);
            for (int i = 0; i < this.VMG.NumberOfTitleSets; i++)
            {
                this.TitleSets[i] = new VtsIfo(folder, $"VTS_{(i + 1):00}_0.IFO");
            }
        }

        public static Dvd? ParseFolder(string folderPath)
        {
            try
            {
                return new Dvd(folderPath);
            } catch(Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Failed to parse DVD:");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                Console.ResetColor();
                return null;
            }
        }

        public bool DemuxAllPgc(string outputFolder)
        {
            foreach(var vts in this.TitleSets)
            {
                if (vts?.MenuProgramChainTable != null) {
                    foreach ((var menu, int index) in vts.MenuProgramChainTable.All.SelectMany(languageUnit => languageUnit.All).WithIndex())
                    {
                        if (menu.NumberOfCells > 0)
                        {
                            if(!vts.DemuxMenuPgc(outputFolder, index))
                            {
                                return false;
                            }
                        }
                    }
                }

                if (vts?.TitleProgramChainTable != null)
                {
                    foreach((var title, int index) in vts.TitleProgramChainTable.All.WithIndex())
                    {
                        if (title.NumberOfCells > 0)
                        {
                            for (int angle = 0; angle < title.NumberOfAngles; angle++)
                            {
                                if (!vts.DemuxTitlePgc(outputFolder, index, angle))
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
            }

            return true;
        }

        public bool DemuxAllCells(string outputFolder)
        {
            if (this.VMG.MenuCellAddressTable != null)
            {
                foreach (var cell in this.VMG.MenuCellAddressTable.All)
                {
                    if (!this.VMG.DemuxMenuCell(outputFolder, cell.VobID, cell.CellID))
                    {
                        return false;
                    }
                }
            }

            foreach (var vts in this.TitleSets)
            {
                if (vts.MenuCellAddressTable != null)
                {
                    foreach (var cell in vts.MenuCellAddressTable.All)
                    {
                        if (!vts.DemuxMenuCell(outputFolder, cell.VobID, cell.CellID))
                        {
                            return false;
                        }
                    }
                }

                if (vts.TitleSetCellAddressTable != null)
                {
                    foreach (var cell in vts.TitleSetCellAddressTable.All)
                    {
                        if (!vts.DemuxTitleCell(outputFolder, cell.VobID, cell.CellID))
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public bool SaveToFile(string filePath)
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true, TypeInfoResolver = new DefaultJsonTypeInfoResolver() };
                using var stream = File.Create(filePath);
                JsonSerializer.Serialize(stream, this, options);
                stream.Flush();
                stream.Close();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
