using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MkvToolNix.Data
{
    public class EBML_Header
    {
        public int Version;
        public int ReadVersion;
        public int IdLength;
        public int SizeLength;
        public string DocumentType;
        public int DocumentTypeVersion;
        public int DocumentTypeReadVersion;

        private EBML_Header()
        {

        }

        public static EBML_Header Parse(Dictionary<string, object?> data)
        {
            EBML_Header result = new();

            result.Version = (int)data[MkvInfoKeys.EBML_Version];
            result.ReadVersion = (int)data[MkvInfoKeys.EBML_ReadVersion];
            result.IdLength = (int)data[MkvInfoKeys.Max_EBML_IdLength];
            result.SizeLength = (int)data[MkvInfoKeys.Max_EBML_SizeLength];
            result.DocumentType = (string)data[MkvInfoKeys.DocumentType];
            result.DocumentTypeVersion = (int)data[MkvInfoKeys.DocumentTypeVersion];
            result.DocumentTypeReadVersion = (int)data[MkvInfoKeys.DocumentTypeReadVersion];

            return result;
        }
    }
}
