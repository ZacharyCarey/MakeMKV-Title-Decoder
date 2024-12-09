using FFMpeg_Wrapper;
using MakeMKV_Title_Decoder.libs.MakeMKV.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Utils;

namespace MakeMKV_Title_Decoder.Data.Renames
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "Type")]
    [JsonDerivedType(typeof(SingleFrameExtracted), "Extracted Frame")]
    public abstract class Attachment
    {
        [JsonIgnore]
        protected const string AttachmentsFolder = "attachments";

        [JsonIgnore]
        public abstract AttachmentType AttachmentType { get; }

        /// <summary>
        /// Relative path to the extracted file.
        /// </summary>
        [JsonIgnore]
        public abstract string FilePath { get; }

        /// <summary>
        /// The rename given by the user
        /// </summary>
        [JsonInclude]
        public string? Name;

        protected Attachment()
        {

        }

        public abstract bool Extract(LoadedDisc Disc);

        protected static string? GenerateAttachmentsFolder(LoadedDisc disc)
        {
            string relativePath = "attachments";
            string attachmentsFolder = Path.Combine(disc.Root, relativePath);
            if (!Directory.Exists(attachmentsFolder))
            {
                try
                {
                    Directory.CreateDirectory(attachmentsFolder);
                    return relativePath;
                }
                catch (Exception)
                {
                    MessageBox.Show("Failed to create attachments folder.");
                    return null;
                }
            } else
            {
                return relativePath;
            }
        }

        public override string ToString()
        {
            return Name ?? "[N/A]";
        }
    }

    public enum AttachmentType
    {
        Image
    }

    public class SingleFrameExtracted : Attachment
    {
        [JsonIgnore]
        public override AttachmentType AttachmentType => AttachmentType.Image;

        [JsonIgnore]
        public override string FilePath => Path.Combine(AttachmentsFolder, $"{Path.GetFileNameWithoutExtension(SourceFile)}.png");

        [JsonInclude]
        public readonly string SourceFile;

        [JsonInclude]
        public uint FrameIndex;

        [JsonConstructor]
        private SingleFrameExtracted(string sourceFile, uint frameIndex)
        {
            this.SourceFile = sourceFile;
            FrameIndex = frameIndex;
        }

        public override bool Extract(LoadedDisc disc)
        {
            string? attachmentsFolder = Attachment.GenerateAttachmentsFolder(disc);
            if (attachmentsFolder == null) return false;

            string inputFile = Path.Combine(disc.Root, SourceFile);
            string outputFile = Path.Combine(disc.Root, attachmentsFolder, $"{Path.GetFileNameWithoutExtension(SourceFile)}.png");

            if (File.Exists(outputFile))
            {
                return true;
            }

            FFMpeg ffmpeg = new(FileUtils.GetFFMpegExe());
            bool result = ffmpeg.Snapshot(inputFile, this.FrameIndex, outputFile).Run();
            return result && File.Exists(outputFile);
        }

        /// <summary>
        /// Source file is relative to disc root
        /// </summary>
        /// <param name="Disc"></param>
        /// <param name="SourceFile"></param>
        /// <returns></returns>
        public static SingleFrameExtracted? Extract(LoadedDisc Disc, LoadedStream SourceFile, uint frameIndex)
        {
            foreach(var at in Disc.RenameData.Attachments)
            {
                if (at is SingleFrameExtracted frame)
                {
                    if (frame.SourceFile == SourceFile.Identity.SourceFile && frame.FrameIndex == frameIndex)
                    {
                        // Found this attachment already
                        return null;
                    }
                }
            }

            var attachment = new SingleFrameExtracted(SourceFile.Identity.SourceFile, frameIndex);
            if (attachment.Extract(Disc))
            {
                return attachment;
            } else
            {
                return null;
            }
        }
    }
}
