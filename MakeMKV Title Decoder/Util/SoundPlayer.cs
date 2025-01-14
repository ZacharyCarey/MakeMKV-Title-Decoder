using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MakeMKV_Title_Decoder.Util {
    internal static class SoundPlayer {

        public const string HappySound = "Sounds.Success.wav";
        public const string SadSound = "Sounds.Error.wav";


        public static void Play(string name) {
            var assembly = Assembly.GetExecutingAssembly();

            /*foreach(string str in assembly.GetManifestResourceNames())
            {
                Console.WriteLine(str);
            }*/
            string file = "MakeMKV_Title_Decoder." + name;
            using (Stream stream = assembly.GetManifestResourceStream(file))
            {
                System.Media.SoundPlayer sound = new(stream);
                sound.Play();
            }
        }

    }
}
