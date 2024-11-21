using libbluray.bdnav.Clpi;
using MkvToolNix.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.Data.BluRay
{
    public class BlurayStream : LoadedStream
    {

        internal BlurayStream(string root, string filePath, MkvMergeID data) : base(root, filePath, data)
        {

        }

        protected override TimeSpan GetDuration(string root, string filePath)
        {
            // Loaded from CLPI file, if possible
            ClpiFile? clpiFile = ClpiFile.Parse(Path.Combine(root, "BDMV", "CLIPINF", $"{Path.GetFileNameWithoutExtension(filePath)}.clpi"));
            if (clpiFile == null)
            {
                throw new Exception("Failed to calculate duration.");
            }

            return clpiFile.sequence.atc_seq.SelectMany(x => x.stc_seq.Select(y => y.Length)).Aggregate((a, b) => a + b);            
        }
    }
}
