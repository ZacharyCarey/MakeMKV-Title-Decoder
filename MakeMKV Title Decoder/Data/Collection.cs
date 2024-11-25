﻿using MakeMKV_Title_Decoder.Data.Renames;
using MakeMKV_Title_Decoder.Forms.FileRenamer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Utils;

namespace MakeMKV_Title_Decoder.Data
{
    public class Collection : Exportable
    {
        [JsonInclude]
        public string Name { get; set; } = "New Collection";

        /// <summary>
        /// The attachment output/extracted file path
        /// </summary>
        [JsonInclude]
        public List<string> Attachments = new();

        [JsonInclude]
        public OutputName OutputFile { get; private set; } = new();

        public void Export(LoadedDisc disc, string outputFolder, string outputFile, IProgress<SimpleProgress>? progress, SimpleProgress? totalProgress = null)
        {
            string folderName = Path.GetFileNameWithoutExtension(outputFile);
            string folderPath = Path.Combine(outputFolder, folderName);

            try
            {
                Directory.CreateDirectory(folderPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to create folder: " + folderPath);
                return;
            }

            foreach(var attachmentPath in this.Attachments)
            {
                // Find attachment details.
                Attachment? attachment = disc.RenameData.Attachments.Where(x => x.FilePath == attachmentPath).FirstOrDefault();
                if (attachment != null)
                {
                    try
                    {
                        string sourcePath = Path.Combine(disc.Root, attachmentPath);
                        string outputPath;
                        if (attachment.Name == null)
                        {
                            outputPath = Path.GetFileName(attachmentPath);
                        }else
                        {
                            outputPath = attachment.Name + Path.GetExtension(attachmentPath);
                        }
                        outputPath = Path.Combine(folderPath, outputPath);
                        File.Copy(sourcePath, outputPath, true);
                    }catch(Exception ex)
                    {
                        MessageBox.Show("Failed to copy file: " + ex.Message);
                    }
                } else
                {
                    MessageBox.Show("Critical error: Failed to find attachment rename data.");
                }
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
