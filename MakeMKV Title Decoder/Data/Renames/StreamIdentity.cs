using MkvToolNix.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Utils;

namespace MakeMKV_Title_Decoder.Data.Renames
{
	public class StreamIdentity
	{
		/// <summary>
		/// Relative to the root folder of the disc
		/// </summary>
		[JsonInclude]
        public readonly string SourceFile;

		/// <summary>
		/// The loaded length of the stream, usually loaded from metadata located on the disc.
		/// </summary>
		[JsonInclude]
        public readonly TimeSpan Duration;

		/// <summary>
		/// The size of the file in bytes
		/// </summary>
		[JsonInclude]
        public readonly DataSize FileSize;

		[JsonInclude]
        public readonly ContainerType? ContainerType;

		internal StreamIdentity(string relativePath, TimeSpan duration, DataSize fileSize, MkvMergeID info)
		{
            this.SourceFile = relativePath;
            this.Duration = duration;
            this.FileSize = fileSize;
            this.ContainerType = info.Container?.Properties?.ContainerType;
        }

        [JsonConstructor]
        private StreamIdentity(string sourceFile, TimeSpan duration, DataSize fileSize, ContainerType? containerType) { 
            this.SourceFile = sourceFile;
            this.Duration = duration;
            this.FileSize = fileSize;
            this.ContainerType= containerType;
        }

        /// <summary>
        /// Returns the reason it didnt match, or null is matched successfully
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public string? Match(StreamIdentity other)
        {
            if (this.SourceFile != other.SourceFile) return "File path does not match.";
            if (this.Duration != other.Duration) return "Length of clips do not match."; 
            if (this.FileSize != other.FileSize) return "File sizes do not match.";
            if (this.ContainerType != other.ContainerType) return "Media container types do not match.";
            return null;
        }
    }
}
