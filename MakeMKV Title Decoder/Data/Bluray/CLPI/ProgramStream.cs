using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.Data.Bluray.CLPI
{
    public class ProgramStream
    {

        public uint pid = 0;
        public uint coding_type = 0;
        public uint format = 0;
        public uint rate = 0;
        public uint aspect = 0;
        public uint oc_flag = 0;
        public uint char_code = 0;

        /// <summary>
        /// bcp47
        /// </summary>
        public string language = "";

        public ProgramStream() { }

        public void dump()
        {
            Console.WriteLine($"Program stream dump:");
            Console.WriteLine($"  pid:         {pid}");
            Console.WriteLine($"  coding_type: {coding_type}");
            Console.WriteLine($"  format:      {format}");
            Console.WriteLine($"  rate:        {rate}");
            Console.WriteLine($"  aspect:      {aspect}");
            Console.WriteLine($"  oc_flag:     {oc_flag}");
            Console.WriteLine($"  char_code:   {char_code}");
            Console.WriteLine($"  language:    {language}");
        }

    }
}
