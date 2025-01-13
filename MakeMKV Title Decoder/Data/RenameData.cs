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

        [JsonInclude]
        public List<Attachment> Attachments = new();

        // Playlist data created by the user for how files should be multiplexed / outputted
        [JsonInclude]
        public List<Playlist> Playlists = new();

        [JsonInclude]
        public List<Collection> Collections = new(); 

        [JsonInclude]
        public List<ShowOutputName> ShowOutputNames = new();

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

        public string GetSafeDiscName() {
            string optional = "";
            if (NumberOfSets != null || SetNumber != null)
            {
                optional = $" {SetNumber?.ToString() ?? "_"} of {NumberOfSets?.ToString() ?? "_"}";
            }

            return Utils.GetFileSafeName($"{Title ?? "Unknown"}{optional}");
        }
    }

    public class OutputName {
        /// <summary>
        /// The index of the <see cref="ShowOutputName"/> in the "ShowOutputNames" list
        /// in the rename data.
        /// </summary>
        [JsonInclude]
        public long ShowIndex = -1;

        /// <summary>
        /// Only applicable for TV shows
        /// </summary>
        [JsonInclude]
        public long Season = -1;

        /// <summary>
        /// Only applicable for TV shows
        /// </summary>
        [JsonInclude]
        public long Episode = -1;

        [JsonInclude]
        public FeatureType? Type;

        [JsonInclude]
        public string? ExtraName;

        [JsonInclude]
        public long? MultipleEpisodesRange = null;

        public string? GetBonusFolder(ShowType showType) {
            switch (showType)
            {
                case ShowType.Movie:
                case ShowType.TV:
                    return GetFolderName(this.Type ?? FeatureType.MainFeature);
                default:
                    throw new Exception();
            }
        }

        public string GetFileName(ShowOutputName ShowID) {
            if (ShowID.Type == ShowType.Movie)
            {
                string? bonusFolder = GetFolderName(this.Type ?? FeatureType.MainFeature);
                if (bonusFolder != null)
                {
                    return $"{ExtraName ?? ""}.mkv";
                } else
                {
                    return $"{ShowID.Name ?? ""} [{ShowOutputName.TmdbPrefix}{ShowID.TmdbID.ToString() ?? ""}].mkv";
                }
            } else if (ShowID.Type == ShowType.TV)
            {
                string? bonusFolder = GetFolderName(this.Type ?? FeatureType.MainFeature);
                if (bonusFolder != null)
                {
                    return $"{ExtraName ?? ""}.mkv";
                } else
                {
                    if (this.MultipleEpisodesRange == null)
                    {
                        return $"{ShowID.Name ?? ""} S{Season}E{Episode}.mkv";
                    } else
                    {
                        return $"{ShowID.Name ?? ""} S{Season}E{Episode}-E{MultipleEpisodesRange}.mkv";
                    }
                }
            } else
            {
                throw new Exception();
            }
        }

        public TmdbID? GetTmdbID(RenameData renameData)
        {
            if (this.ShowIndex < 0) return null;
            var showName = renameData.ShowOutputNames[(int)this.ShowIndex];
            TmdbID id = new TmdbID(showName.Type, showName.TmdbID);
            if (this.Season >= 0) id.Season = this.Season;
            if (this.Episode >= 0) id.Episode = this.Episode;

            return id;
        }

        public ShowOutputName? GetShowName(RenameData rename)
        {
            if (this.ShowIndex < 0) return null;
            return rename.ShowOutputNames[(int)this.ShowIndex];
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
                case FeatureType.Gallery: return "gallery";
                default:
                    return null;
            }
        }
    }
}
