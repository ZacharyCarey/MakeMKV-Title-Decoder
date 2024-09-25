using JsonSerializable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.Forms.TmdbBrowser {
    public enum ShowType {
        Movie,
        TV
    }

    public class TmdbID : IJsonSerializable {
        public ShowType Type;
        public long ID;
        public long? Season;
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

        public void LoadFromJson(JsonData Data) {
            var obj = (JsonObject)Data;

            JsonString? str = (JsonString?)obj["Type"] ?? null;
            if (str != null)
            {
                if (!Enum.TryParse(str.Value, out this.Type))
                {
                    this.Type = ShowType.Movie;
                }
            } else
            {
                this.Type = ShowType.Movie;
            }

            this.ID = obj.Load<JsonInteger>("ID")?.Value ?? 0;
            this.Season = obj.Load<JsonInteger>("Season")?.Value ?? null;
            this.Episode = obj.Load<JsonInteger>("Episode")?.Value ?? null;
        }

        public JsonData SaveToJson() {
            JsonObject obj = new();

            obj["Type"] = new JsonString(this.Type.ToString());
            obj["ID"] = new JsonInteger(this.ID);
            if (this.Season != null) obj["Season"] = new JsonInteger(this.Season.Value);
            if (this.Episode != null) obj["Episode"] = new JsonInteger(this.Episode.Value);

            return obj;
        }
    }
}
