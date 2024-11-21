using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.Forms.TmdbBrowser {
    public enum ShowType {
        Movie,
        TV
    }

    public class TmdbID {
        [JsonInclude]
        public ShowType Type;

        [JsonInclude]
        public long ID;

        [JsonInclude]
        public long? Season;

        [JsonInclude]
        public long? Episode;

        public TmdbID(ShowType type, long ID, long? season = null, long? episode = null) {
            this.Type = type;
            this.ID = ID;
            this.Season = season;
            this.Episode = episode;
        }

        public TmdbID(TmdbID copy) {
            this.Type = copy.Type;
            this.ID = copy.ID;
            this.Season = copy.Season;
            this.Episode = copy.Episode;
        }

        public string GetTmdbWebsite() {
            string addr = "https://www.themoviedb.org/";
            if (this.Type == ShowType.Movie)
            {
                addr += "movie/";
            } else if (this.Type == ShowType.TV)
            {
                addr += "tv/";
            } else
            {
                return addr;
            }

            addr += $"{ID}/";

            if (this.Season != null)
            {
                addr += $"season/{Season}/";
                
                if (this.Episode != null)
                {
                    addr += $"episode/{Episode}/";
                }
            }

            return addr;
        }

        public override string ToString() {
            List<string> strings = new();
            strings.Add($"{this.Type.ToString()}={this.ID}");

            if (Season != null)
            {
                strings.Add($"Season={Season}");
            }

            if(Episode != null)
            {
                strings.Add($"Episode={Episode}");
            }

            return $"[{string.Join(", ", strings)}]";
        }
    }
}
