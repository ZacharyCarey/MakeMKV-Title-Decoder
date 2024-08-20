using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.Data.Bluray.BlurayIndex
{
    public struct BlurayIndex
    {

        public FirstPlayback first_playback;
        public TopMenu top_menu;
        public BlurayTitle[] titles;

        public BlurayIndex() { }

        public void dump()
        {
            Console.WriteLine($"Index dump:");

            first_playback.dump();
            top_menu.dump();

            Console.WriteLine($"  num_titles:                 {titles.Length}");

            uint index = 0;
            foreach (var title in titles)
            {
                title.dump(index++);
            }
        }

    }
}
