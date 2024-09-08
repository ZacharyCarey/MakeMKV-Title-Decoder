using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder {
    internal class Mpu3PlaylistCreater {

        List<string> files = new();

        public void Save(string path) {
            using (FileStream stream = File.OpenWrite(path))
            {
                byte[] magicNumber = { 0xEF, 0xBB, 0xBF, 0x23 };
                stream.Write(magicNumber, 0, magicNumber.Length);

                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.WriteLine();

                    string basePath = Directory.GetParent(path).FullName;
                    foreach(string file in files)
                    {
                        writer.WriteLine(Path.GetRelativePath(basePath, file));
                    }
                }
            }
        }

        public void Add(string path) {
            files.Add(path);
        }
    }
}
