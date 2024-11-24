using FfmpegInterface;
using MakeMKV_Title_Decoder.libs.MakeMKV.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.Data.Renames
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "Type")]
    [JsonDerivedType(typeof(SingleFrameExtracted), "Single Frame")]
    [JsonDerivedType(typeof(AllFramesExtracted), "All Frames")]
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
        public readonly TimeSpan FrameTime;

        [JsonConstructor]
        private SingleFrameExtracted(string sourceFile, TimeSpan frameTime)
        {
            this.SourceFile = sourceFile;
            FrameTime = frameTime;
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

            ffmpeg instance = new ffmpeg();
            instance.ExtractFrame(inputFile, FrameTime, outputFile);

            return File.Exists(outputFile);
        }

        /// <summary>
        /// Source file is relative to disc root
        /// </summary>
        /// <param name="Disc"></param>
        /// <param name="SourceFile"></param>
        /// <returns></returns>
        public static SingleFrameExtracted? Extract(LoadedDisc Disc, LoadedStream SourceFile, TimeSpan? frameTime = null)
        {
            if (frameTime == null)
            {
                frameTime = SourceFile.Identity.Duration / 2;
            }

            foreach(var at in Disc.RenameData.Attachments)
            {
                if (at is SingleFrameExtracted frame)
                {
                    if (frame.SourceFile == SourceFile.Identity.SourceFile && (frameTime.Value - frame.FrameTime).Duration().Milliseconds < 5)
                    {
                        // Found this attachment already
                        return null;
                    }
                }
            }

            var attachment = new SingleFrameExtracted(SourceFile.Identity.SourceFile, SourceFile.Identity.Duration / 2);
            if (attachment.Extract(Disc))
            {
                return attachment;
            } else
            {
                return null;
            }
        }
    }

    public class AllFramesExtracted : Attachment
    {
        [JsonIgnore]
        public override AttachmentType AttachmentType => AttachmentType.Image;

        [JsonIgnore]
        public override string FilePath => Path.Combine(AttachmentsFolder, $"{Path.GetFileNameWithoutExtension(SourceFile)}_{FrameIndex}.png");

        [JsonInclude]
        public readonly string SourceFile;

        [JsonInclude]
        public readonly int FrameIndex;

        [JsonConstructor]
        private AllFramesExtracted(string sourceFile, int frameIndex)
        {
            SourceFile = sourceFile;
            FrameIndex = frameIndex;
        }

        public override bool Extract(LoadedDisc disc)
        {
            string? attachmentsFolder = Attachment.GenerateAttachmentsFolder(disc);
            if (attachmentsFolder == null) return false;

            string inputFile = Path.Combine(disc.Root, SourceFile);
            string outputFile = Path.Combine(disc.Root, attachmentsFolder, $"{Path.GetFileNameWithoutExtension(SourceFile)}_{FrameIndex}.png");

            if (File.Exists(outputFile))
            {
                return true;
            }

            ffmpeg instance = new ffmpeg();
            instance.ExtractAllFrames(inputFile, Path.Combine(disc.Root, attachmentsFolder));

            return File.Exists(outputFile);
        }

        public static IEnumerable<AllFramesExtracted> Extract(LoadedDisc Disc, LoadedStream SourceFile)
        {
            foreach (var at in Disc.RenameData.Attachments)
            {
                if (at is AllFramesExtracted frame)
                {
                    if (frame.SourceFile == SourceFile.Identity.SourceFile)
                    {
                        // Found this attachment already
                        yield break;
                    }
                }
            }

            // Used as a dummy to run the extraction command
            var dummy = new AllFramesExtracted(SourceFile.Identity.SourceFile, 1);
            if (!dummy.Extract(Disc)) yield break;

            string? attachmentsFolder = Attachment.GenerateAttachmentsFolder(Disc);
            Debug.Assert(attachmentsFolder != null);

            string baseName = $"{Path.GetFileNameWithoutExtension(SourceFile.Identity.SourceFile)}_";
            string searchPattern = $"{baseName}*.png";
            foreach (var filePath in Directory.GetFiles(attachmentsFolder, searchPattern))
            {
                string name = Path.GetFileNameWithoutExtension(filePath);
                int frameCount;
                if (int.TryParse(name[baseName.Length..], out frameCount))
                {
                    yield return new AllFramesExtracted(SourceFile.Identity.SourceFile, frameCount);
                } else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Failed to parse frame number!");
                    Console.ResetColor();
                }
            }
        }
    }
}
