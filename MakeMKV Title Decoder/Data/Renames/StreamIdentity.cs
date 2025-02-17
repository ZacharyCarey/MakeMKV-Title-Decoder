﻿using FFMpeg_Wrapper.ffprobe;
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
		/// The size of the file in bytes
		/// </summary>
		[JsonInclude]
        public readonly DataSize FileSize;

		[JsonInclude]
        public readonly string? ContainerType;

		internal StreamIdentity(string relativePath, DataSize fileSize, MediaAnalysis info)
		{
            this.SourceFile = relativePath;
            this.FileSize = fileSize;
            this.ContainerType = info.Format.FormatName;
        }

        [JsonConstructor]
        private StreamIdentity(string sourceFile, DataSize fileSize, string? containerType) { 
            this.SourceFile = sourceFile;
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
            //if (this.FileSize != other.FileSize) return "File sizes do not match.";
            if (this.ContainerType != other.ContainerType) return "Media container types do not match.";
            return null;
        }
    }
}
