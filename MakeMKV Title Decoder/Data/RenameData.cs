using MakeMKV_Title_Decoder.Data.Renames;
using MakeMKV_Title_Decoder.Forms.FileRenamer;
using MakeMKV_Title_Decoder.Forms.TmdbBrowser;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MakeMKV_Title_Decoder.Data
{

    public class RenameData {
        public const string Version = "1.0";

        [JsonInclude]
        public DiscIdentity DiscID;

        [JsonInclude]
        public List<ClipRename> Clips = new();

        // Playlist data created by the user for how files should be multiplexed / outputted
        [JsonInclude]
        public List<Playlist> Playlists = new();

        public RenameData(DiscIdentity discID) {
            this.DiscID = discID;
        }
    }

    public class DiscIdentity
    {
        [JsonInclude]
        public readonly string? Title;

        [JsonInclude]
        public readonly long? NumberOfSets;

        [JsonInclude]
        public readonly long? SetNumber;

        [JsonConstructor]
        public DiscIdentity(string? title, long? numberOfSets, long? setNumber)
        {
            this.Title = title;
            this.NumberOfSets = numberOfSets;
            this.SetNumber = setNumber; 
        }

        /// <summary>
        /// Returns the reason it didnt match, or null is matched successfully
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public string? Match(DiscIdentity other)
        {
            if (this.Title != other.Title) return "Disc title does not match.";
            if (this.NumberOfSets != other.NumberOfSets) return "Number of disc sets does not match.";
            if (this.SetNumber != other.SetNumber) return "Disc set number does not match.";
            return null;
        }
    }

    public class OutputName {
        const string TmdbPrefix = "tmdbid=";

        [JsonInclude]
        public TmdbID? ShowID;

        [JsonInclude]
        public string? ShowName;

        // For multiple versions of the same episode, which version is this one?
        [JsonInclude]
        public string? MultiVersion;

        [JsonInclude]
        public FeatureType? Type;

        [JsonInclude]
        public string? ExtraName;

        public string GetFolderPath() {
            List<string> folders = new();

            folders.Add($"{this.ShowName} [{TmdbPrefix}{ShowID?.ID.ToString() ?? ""}]");

            if (ShowID?.Type == ShowType.Movie)
            {
                // TODO this is bad
            } else if (ShowID?.Type == ShowType.TV)
            {
                folders.Add($"Season {ShowID.Season}");
            } else
            {
                throw new Exception();
            }

            return Path.Combine(folders.ToArray());
        }

        public string? GetBonusFolder() {
            switch (ShowID?.Type)
            {
                case ShowType.Movie:
                case ShowType.TV:
                    return GetFolderName(this.Type ?? FeatureType.MainFeature);
                default:
                    throw new Exception();
            }
        }

        public string GetFileName() {
            string versionString = "";
            if (MultiVersion != null)
            {
                versionString = $" - {MultiVersion}";
            }

            if (ShowID?.Type == ShowType.Movie)
            {
                string? bonusFolder = GetFolderName(this.Type ?? FeatureType.MainFeature);
                if (bonusFolder != null)
                {
                    return $"{ExtraName ?? ""}{versionString}.mkv";
                } else
                {
                    return $"{ShowName ?? ""} [{TmdbPrefix}{ShowID?.ID.ToString() ?? ""}]{versionString}.mkv";
                }
            } else if (ShowID?.Type == ShowType.TV)
            {
                string? bonusFolder = GetFolderName(this.Type ?? FeatureType.MainFeature);
                if (bonusFolder != null)
                {
                    return $"{ExtraName ?? ""}{versionString}.mkv";
                } else
                {
                    return $"{ShowName ?? ""} S{ShowID.Season}E{ShowID.Episode}{versionString}.mkv";
                }
            } else
            {
                throw new Exception();
            }
        }

        private static string? GetFolderName(FeatureType type) {
            switch (type)
            {
                case FeatureType.Extras: return "extras";
                case FeatureType.Specials: return "specials";
                case FeatureType.Shorts: return "shorts";
                case FeatureType.Scenes: return "scenes";
                case FeatureType.Featurettes: return "featurettes";
                case FeatureType.BehindTheScenes: return "behind the scenes";
                case FeatureType.DeletedScenes: return "deleted scenes";
                case FeatureType.Interviews: return "interviews";
                case FeatureType.Trailers: return "trailers";
                default:
                    return null;
            }
        }
    }
}
