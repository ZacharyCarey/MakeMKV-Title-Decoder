using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.Data.MPLS {

    public struct ChapterName {
        /// <summary>
        /// bcp47
        /// </summary>
        public string language;
        public string name;
    }

    public struct Chapter {

        public UInt64 timestamp;
        public List<ChapterName> names = new();

        public Chapter() {

        }

    }
}
