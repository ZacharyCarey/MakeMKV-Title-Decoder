using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libbluray.bdnav {
    public class META_THUMBNAIL {
        /// <summary>
        /// Path to thumbnail image (relative to disc root)
        /// </summary>
        public string path;

        /// <summary>
        /// thumbnail width (32 bits, -1 is unknown)
        /// </summary>
        public Int64 xres = -1;

        /// <summary>
        /// thumbnail height (32 bits, -1 is unknown)
        /// </summary>
        public Int64 yres = -1;
    }

    public class META_TITLE {
        /// <summary>
        /// title number, from disc index
        /// </summary>
        public UInt32 title_number;

        public string title_name;
    }

    /// <summary>
    /// Meta disc library, metadata entry
    /// </summary>
    public class META_DL {
        /// <summary>
        /// language used in this entry
        /// </summary>
        public string language_code = "";

        /// <summary>
        /// source file, relative to disc root
        /// </summary>
        public string filename;

        /// <summary>
        /// disc name
        /// </summary>
        public string di_name;

        /// <summary>
        /// alternative name
        /// </summary>
        public string di_alternative;

        /// <summary>
        /// number of discs in original volume or collection
        /// </summary>
        public Int32 di_num_sets;

        /// <summary>
        /// sequence order of the disc from an original collection
        /// </summary>
        public Int32 di_set_number;

        /// <summary>
        /// number of title entries
        /// </summary>
        public UInt32 toc_count;

        /// <summary>
        /// title data
        /// </summary>
        public List<META_TITLE> toc_entries = new();

        /// <summary>
        /// number of thumbnails
        /// </summary>
        public byte thumb_count;

        /// <summary>
        /// thumbnail data
        /// </summary>
        public List<META_THUMBNAIL> thumbnails = new();
    }
}
