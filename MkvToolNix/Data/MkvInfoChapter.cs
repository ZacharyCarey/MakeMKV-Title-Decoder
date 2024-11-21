using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MkvToolNix.Data
{
    public class MkvInfoChapter
    {

        /*
|+ Chapters
| + Edition entry
|  + Edition flag hidden: 0
|  + Edition flag default: 1
|  + Edition UID: 8963729032494963272
|  + Chapter atom
|   + Chapter UID: 13668184677037791113
|   + Chapter time start: 00:00:00.000000000
|   + Chapter flag hidden: 0
|   + Chapter flag enabled: 1
|   + Chapter time end: 00:09:03.793250000
|   + Chapter display
|    + Chapter string: Chapter 01
|    + Chapter language: eng
|    + Chapter language (IETF BCP 47): en
         */

        public static MkvInfoChapter Parse(List<KeyValuePair<string, object?>> data)
        {
            return new MkvInfoChapter();
        }

    }
}
