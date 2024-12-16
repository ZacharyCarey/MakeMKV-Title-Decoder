using libbluray.bdnav.Mpls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Utils;
using FFMpeg_Wrapper.ffprobe;
using FFMpeg_Wrapper;
using CLI_Wrapper;
using libbluray;

namespace MakeMKV_Title_Decoder.Data.BluRay
{
    public class BlurayDisc : LoadedDisc
    {
        private List<DiscPlaylist> playlists;

        public override bool ForceVlcTrackIndex => false;
        public override bool ForceTranscoding => false;

        private BlurayDisc(string root, List<LoadedStream> streams, string? title, long? numSets, long? setNum, List<DiscPlaylist> playlists) : base(root, title, numSets, setNum, streams)
        {
            this.playlists = playlists;
        }

        public static BlurayDisc? Open(string root, IProgress<SimpleProgress>? progress = null)
        {
            string? ffprobeEXE = FileUtils.GetFFProbeExe();
            if (ffprobeEXE == null) return null;
            FFProbe ffprobe = new(ffprobeEXE);

            string streamDir = Path.Combine("BDMV", "STREAM");
            string playlistDir = Path.Combine("BDMV", "PLAYLIST");
            string[] streamFilePaths = Directory.GetFiles(Path.Combine(root, streamDir));
            string[] playlistFilePaths = Directory.GetFiles(Path.Combine(root, playlistDir));
            SimpleProgress currentProgress = new();
            currentProgress.TotalMax = (uint)streamFilePaths.Length + (uint)playlistFilePaths.Length;

            List<LoadedStream> streams = new();
            for (int i = 0; i < streamFilePaths.Length; i++)
            {
                string fileName = Path.GetFileName(streamFilePaths[i]);
                MediaAnalysis? id = ParseIdentify(ffprobe, root, streamDir, fileName);
                if (id == null) return null;

                streams.Add(new BlurayStream(root, Path.Combine(streamDir, fileName), id));
                currentProgress.Total++;
                progress?.Report(currentProgress);
            }
            currentProgress.Total = (uint)streamFilePaths.Length;

            // TODO add optional "info text" to SimpleProgress that allows us to show the current filename in TaskProgress form
            List<DiscPlaylist> playlists = new();
            for (int i = 0; i < playlistFilePaths.Length; i++)
            {
                string fileName = Path.GetFileName(playlistFilePaths[i]);
                List<string>? playlistFiles = GetPlaylist(Path.Combine(root, playlistDir, fileName));
                if (playlistFiles != null)
                {
                    DiscPlaylist playlist = new();
                    playlist.Name = fileName;
                    playlist.SourceFiles = playlistFiles.Select(x =>
                    {
                        try
                        {
                            return Path.Combine(streamDir, x);
                        }
                        catch (Exception) {
                            return null;
                        }
                    }).Where(x => x != null).Cast<string>().ToList();
                    playlists.Add(playlist);
                }
                currentProgress.Total++;
                progress?.Report(currentProgress);
            }
            currentProgress.Total = (uint)streamFilePaths.Length + (uint)playlistFilePaths.Length;

            string? title;
            long? numSets;
            long? setNum;
            ParseDiscInfo(root, out title, out numSets, out setNum);

            if (title == null)
            {
                title = new DirectoryInfo(root).Name;
            }

            return new BlurayDisc(root, streams, title, numSets, setNum, playlists);
        }

        private static void ParseDiscInfo(string root, out string? title, out long? numberOfSets, out long? setNumber)
        {
            title = null;
            numberOfSets = null;
            setNumber = null;

            string filePath = Path.Combine(root, "BDMV", "META", "DL", "bdmt_eng.xml");
            if (!System.IO.File.Exists(filePath))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Could not find disc info file.");
                Console.ResetColor();
                return;
            }
            try
            {
                var file = System.IO.File.OpenRead(filePath);
                if (file != null)
                {
                    XmlDocument doc = new();
                    doc.Load(file);

                    XmlNode? titleNode = FindNode(doc.ChildNodes, 0, "disclib", "di:discinfo", "di:title");
                    if (titleNode != null)
                    {
                        XmlNode? nameNode = FindNode(titleNode.ChildNodes, 0, "di:name");
                        if (nameNode != null)
                        {
                            title = nameNode.InnerText;
                        }

                        XmlNode? numSetsNode = FindNode(titleNode.ChildNodes, 0, "di:numSets");
                        if (numSetsNode != null)
                        {
                            long value;
                            if (long.TryParse(numSetsNode.InnerText, out value))
                            {
                                numberOfSets = value;
                            }
                        }

                        XmlNode? setNumNode = FindNode(titleNode.ChildNodes, 0, "di:setNumber");
                        if (setNumNode != null)
                        {
                            long value;
                            if (long.TryParse(setNumNode.InnerText, out value))
                            {
                                setNumber = value;
                            }
                        }

                        // Successfully loaded what was available
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error reading disc info: {ex.Message}");
                Console.ResetColor();
            }
        }

        private static XmlNode? FindNode(XmlNodeList nodes, int index, params string[] names)
        {
            foreach (XmlNode node in nodes)
            {
                if (node.Name == names[index])
                {
                    index++;
                    if (index == names.Length)
                    {
                        return node;
                    }
                    else
                    {
                        return FindNode(node.ChildNodes, index, names);
                    }
                }
            }

            return null;
        }
    
        public override List<DiscPlaylist> GetPlaylists()
        {
            return this.playlists.Select(x => x.DeepCopy()).ToList();
        }

        private static List<string>? GetPlaylist(string mplsFile) {
            List<string> playlist = new();
            // MplsFile? mpls = MplsFile.Parse(mplsFile);
            //if (mpls == null) return null;
            TSPlaylistFile mpls = new(mplsFile);
            if (!mpls.Scan()) return null;
            foreach (var streamID in mpls.Streams)
            {
                playlist.Add($"{streamID:00000}.m2ts");
            }
            return playlist;
        }
    }
}
