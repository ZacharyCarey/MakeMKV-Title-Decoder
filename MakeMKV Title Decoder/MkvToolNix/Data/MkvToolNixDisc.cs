using libbluray.file;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Xml;

namespace MakeMKV_Title_Decoder.MkvToolNix.Data {
    public class MkvToolNixDisc {

        public readonly string RootPath;
        public readonly MkvMergeID[] Streams;
        public readonly MkvMergeID[] Playlists;

        public string? Title = null;
        public long? NumberOfSets { get; private set; } = null;
        public long? SetNumber { get; private set; } = null;

        private MkvToolNixDisc(string rootPath, MkvMergeID[] streams, MkvMergeID[] playlists) {
            this.RootPath = rootPath;
            this.Streams = streams;
            this.Playlists = playlists;
        }

        private static void ParseIdentify(List<MkvMergeID> result, string filePath, int index, int length, uint offset, uint total, IProgress<SimpleProgress>? progress) {
            string fileName = Path.GetFileName(filePath);
            MkvMergeID? identification = MkvToolNixInterface.Identify(filePath);
            if (identification == null)
            {
                // TODO move to logger??
                // TODO how to handle error?
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Failed to identify stream file: {fileName}");
                Console.ResetColor();
            } else
            {
                Console.WriteLine($"Identified stream file: {fileName}");
                result.Add(identification);
            }
            if (progress != null)
            {
                SimpleProgress currentProg = new SimpleProgress((uint)index, (uint)length, offset + (uint)index, total);
                progress.Report(currentProg);
            }
        }

        private XmlNode? FindNode(XmlNodeList nodes, int index, params string[] names) {
            foreach (XmlNode node in nodes)
            {
                if (node.Name == names[index])
                {
                    index++;
                    if (index == names.Length)
                    {
                        return node;
                    } else
                    {
                        return FindNode(node.ChildNodes, index, names);
                    }
                }
            }

            return null;
        }

        private void ParseDiscInfo() {
            string filePath = Path.Combine(this.RootPath, "BDMV", "META", "DL", "bdmt_eng.xml");
            if (File.Exists(filePath))
            {
                try
                {
                    var file = file_win32.OpenFile(filePath, FileMode.Open);
                    if (file != null)
                    {
                        XmlDocument doc = new();
                        doc.Load(file.GetStream());

                        XmlNode? titleNode = FindNode(doc.ChildNodes, 0, "disclib", "di:discinfo", "di:title");
                        if (titleNode != null)
                        {
                            XmlNode? nameNode = FindNode(titleNode.ChildNodes, 0, "di:name");
                            if (nameNode != null)
                            {
                                this.Title = nameNode.InnerText;
                            }

                            XmlNode? numSetsNode = FindNode(titleNode.ChildNodes, 0, "di:numSets");
                            if (numSetsNode != null)
                            {
                                long value;
                                if (long.TryParse(numSetsNode.InnerText, out value))
                                {
                                    this.NumberOfSets = value;
                                }
                            }

                            XmlNode? setNumNode = FindNode(titleNode.ChildNodes, 0, "di:setNumber");
                            if (setNumNode != null)
                            {
                                long value;
                                if (long.TryParse(setNumNode.InnerText, out value))
                                {
                                    this.SetNumber = value;
                                }
                            }

                            // Successfully loaded what was available
                            return;
                        }
                    }
                } catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Error reading disc info: {ex.Message}");
                    Console.ResetColor();
                }
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Could not find disc info file.");
            Console.ResetColor();
        }

        public static MkvToolNixDisc? Open(string path, IProgress<SimpleProgress>? progress = null) {
            List<MkvMergeID> streams = new();
            List<MkvMergeID> playlists = new();
            string[] streamFilePaths = Directory.GetFiles(Path.Combine(path, "BDMV", "STREAM"));
            string[] playlistFilePaths = Directory.GetFiles(Path.Combine(path, "BDMV", "PLAYLIST"));
            uint total = (uint)streamFilePaths.Length + (uint)playlistFilePaths.Length;
            uint offset = 0;

            for(int i = 0; i < streamFilePaths.Length; i++)
            {
                ParseIdentify(streams, streamFilePaths[i], i, streamFilePaths.Length, offset, total, progress);
            }
            offset += (uint)streamFilePaths.Length;
            // TODO add optional "info text" to SimpleProgress that allows us to show the current filename in TaskProgress form
            for(int i = 0; i < playlistFilePaths.Length; i++)
            {
                ParseIdentify(playlists, playlistFilePaths[i], i, playlistFilePaths.Length, offset, total, progress);
            }
            offset += (uint)playlistFilePaths.Length;

            var result = new MkvToolNixDisc(path, streams.ToArray(), playlists.ToArray());
            result.ParseDiscInfo();

            return result;
        }

        public static MkvToolNixDisc? OpenAsync(string path) {
            var progressForm = new TaskProgressViewer<Task<MkvToolNixDisc?>, SimpleProgress>(
                (IProgress<SimpleProgress> progress) =>
                {
                    return Task.Run(() => MkvToolNixDisc.Open(path, progress));
                }
            );
            progressForm.ShowDialog();
            return progressForm.Task?.Result;
        }
    }
}
