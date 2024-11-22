using MakeMKV_Title_Decoder.Forms.TmdbBrowser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.Data.Renames
{
    public class ShowOutputName
    {
        internal const string TmdbPrefix = "tmdbid=";

        [JsonInclude]
        public ShowType Type;

        [JsonInclude]
        public long TmdbID;

        [JsonInclude]
        public string Name;

        public ShowOutputName(ShowType type, long tmdbId, string name)
        {
            this.Type = type;
            this.TmdbID = tmdbId;
            this.Name = name;
        }

        public string GetFolderPath(long season)
        {
            List<string> folders = new();

            folders.Add($"{this.Name} [{TmdbPrefix}{TmdbID.ToString() ?? ""}]");

            if (Type == ShowType.Movie)
            {
                
            }
            else if (Type == ShowType.TV)
            {
                folders.Add($"Season {season}");
            }
            else
            {
                throw new Exception();
            }

            return Path.Combine(folders.ToArray());
        }

        public void SetValues(ShowOutputName other)
        {
            this.Type = other.Type;
            this.Name = other.Name;
            this.TmdbID = other.TmdbID;
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
