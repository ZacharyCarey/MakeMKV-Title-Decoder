using PgcDemuxLib.Data.VMG;
using PgcDemuxLib.Data.VTS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using Utils;
using Iso639;
using PgcDemuxLib.Data;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Globalization;

namespace PgcDemuxLib
{
    public struct CellID {
        public int VTS;
        public bool IsMenu;
        public int CID;
        public int VID;
    }

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
            this.Folder = folder;

            folder = Path.Combine(folder, "VIDEO_TS");
            if (!Directory.Exists(folder))
            {
                throw new DirectoryNotFoundException($"Failed to find directory: {folder}");
            }

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

        private static string GetSourceFileName(CellID cell) {
            return $"VTS-{cell.VTS:00}{(cell.IsMenu ? "_Menu" : "")}_VID-{cell.VID:X4}_CID-{cell.CID:X2}.VOB";
        }

        public static bool TryParseSourceFileName(string name, out CellID result) {
            result = new();

            if (!name.StartsWith("VTS-")) return false;
            int index = "VTS-".Length;

            if (!int.TryParse(name.Substring(index, 2), out result.VTS)) return false;
            if (result.VTS < 0 || result.VTS > 99) return false;
            index += 2;

            if (name.Substring(index, "_Menu".Length) == "_Menu")
            {
                result.IsMenu = true;
                index += "_Menu".Length;
            }

            if (name.Substring(index, "_VID-".Length) != "_VID-") return false;
            index += "_VID-".Length;

            if (!int.TryParse(name.Substring(index, 4), NumberStyles.HexNumber, null, out result.VID)) return false;
            index += 4;

            if (name.Substring(index, "_CID-".Length) != "_CID-") return false;
            index += "_CID-".Length;

            if (!int.TryParse(name.Substring(index, 2), NumberStyles.HexNumber, null, out result.CID)) return false;
            index += 2;

            if (name[index] != '.') return false;

            return true;
        }

        public IEnumerable<string> GetSourceFiles() {
            if (this.VMG.MenuCellAddressTable != null)
            {
                foreach(ADT adt in this.VMG.MenuCellAddressTable.All.Distinct(new CellEqualityComparer()))
                {
                    CellID cell = new() {
                        VTS = 0,
                        IsMenu = true,
                        VID = adt.VobID,
                        CID = adt.CellID
                    };
                    yield return GetSourceFileName(cell);
                }
            }
            foreach (var vts in this.TitleSets)
            {
                if (vts.MenuCellAddressTable != null)
                {
                    foreach(ADT adt in vts.MenuCellAddressTable.All.Distinct(new CellEqualityComparer()))
                    {
                        CellID cell = new() {
                            VTS = vts.TitleSet,
                            IsMenu = true,
                            VID = adt.VobID,
                            CID = adt.CellID
                        };
                        yield return GetSourceFileName(cell);
                    }
                }
                if (vts.TitleSetCellAddressTable != null)
                {
                    foreach(ADT adt in vts.TitleSetCellAddressTable.All.Distinct(new CellEqualityComparer()))
                    {
                        CellID cell = new() {
                            VTS = vts.TitleSet,
                            IsMenu = false,
                            VID = adt.VobID,
                            CID = adt.CellID
                        };
                        yield return GetSourceFileName(cell);
                    }
                }
            }
        }

        public DemuxResult DemuxSourceFile(string outputFolder, string sourceFile, IProgress<SimpleProgress>? progress = null, SimpleProgress? maxProgress = null) {
            CellID cell;
            if (!TryParseSourceFileName(sourceFile, out cell)) return new DemuxResult(); // failure

            IfoBase ifo;
            if (cell.VTS == 0)
            {
                ifo = this.VMG;
            } else
            {
                if (cell.VTS < 0 || (cell.VTS - 1) >= this.TitleSets.Count) return new DemuxResult(); // failure
                ifo = this.TitleSets[cell.VTS - 1];
            }

            // Try to find the cell
            DemuxResult result;
            if (cell.IsMenu)
            {
                result = ifo.DemuxMenuCell(outputFolder, sourceFile, cell.VID, cell.CID, progress, maxProgress);
            } else
            {
                result = ifo.DemuxTitleCell(outputFolder, sourceFile, cell.VID, cell.CID, progress, maxProgress);
            }
            return result;
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

        private class CellEqualityComparer : IEqualityComparer<ADT> {
            public bool Equals(ADT? x, ADT? y) {
                if (x != null && y != null)
                {
                    return (x.VobID == y.VobID) && (x.CellID == y.CellID);
                } else
                {
                    if ((x == null) && (y == null)) return true; 
                    else return false;
                }
            }

            public int GetHashCode([DisallowNull] ADT obj) {
                return HashCode.Combine(obj.VobID, obj.CellID);
            }
        }
    }
}
