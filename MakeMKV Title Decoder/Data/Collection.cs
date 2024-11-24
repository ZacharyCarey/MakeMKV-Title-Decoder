using MakeMKV_Title_Decoder.Data.Renames;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.Data
{
    public class Collection
    {
        [JsonInclude]
        public string Name = "New Collection";

        /// <summary>
        /// The attachment output/extracted file path
        /// </summary>
        [JsonInclude]
        public List<string> Attachments = new();

        public override string ToString()
        {
            return Name;
        }
    }
}
