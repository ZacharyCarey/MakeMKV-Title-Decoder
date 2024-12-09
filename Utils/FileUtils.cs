using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Utils {
    public static class FileUtils {

        public static IEnumerable<string> SearchProgramFiles(string folderName, string? exeName) {
            Environment.SpecialFolder[] searchLocations = {
                Environment.SpecialFolder.CommonProgramFiles,
                Environment.SpecialFolder.CommonProgramFilesX86,
                Environment.SpecialFolder.ProgramFiles,
                Environment.SpecialFolder.ProgramFilesX86
            };

            HashSet<string> searchedPaths = new();

            foreach (var specialFolderType in searchLocations)
            {
                string? folder = null;
                try
                {
                    folder = Environment.GetFolderPath(specialFolderType);
                    if (searchedPaths.Contains(folder))
                    {
                        continue;
                    }
                    searchedPaths.Add(folder);

                    if (!Directory.Exists(folder))
                    {
                        continue;
                    }

                    folder = Path.Combine(folder, folderName);
                    if (!Directory.Exists(folder))
                    {
                        continue;
                    }

                    if (exeName != null)
                    {
                        folder = Path.Combine(folder, exeName);
                        if (!File.Exists(folder))
                        {
                            continue;
                        }
                    }

                    //yield return folder;
                } catch (Exception)
                {
                    //folder = null;
                    continue;
                }

                yield return folder;
            }
        }

        public static string? SearchLocalExeFiles(string relativeExePath) {
            try
            {
                string cd = Assembly.GetEntryAssembly().Location;
                string parentFolder = Path.GetDirectoryName(cd);
                string path = Path.Combine(parentFolder, relativeExePath);
                if (File.Exists(path))
                {
                    return path;
                } else
                {
                    return null;
                }
            } catch (Exception)
            {
                return null;
            }
        }

    }
}
