using PgcDemuxLib.Data.VMG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PgcDemuxLib.Data.VTS
{
    public class VTS_PTT_SRPT
    {
        /// <summary>
        /// Byte index in file
        /// </summary>
        [JsonInclude]
        internal readonly int Address;

        [JsonInclude]
        internal readonly int NumberOfTitles;

        [JsonInclude]
        readonly VTS_PTT[] TitleChapters;

        public VTS_PTT this[int TitleNumber]
        {
            get => TitleChapters[TitleNumber];
        }

        internal VTS_PTT_SRPT(byte[] file, int addr)
        {
            // I had to look at ibdvdread to figure out this one
            Address = addr;

            NumberOfTitles = file.GetNbytes(addr, 2);
            int lastByte = file.GetNbytes(addr + 4, 4);

            //NOTE: One release didn't fill this field, so this is just an extra check
            if (lastByte == 0)
            {
                lastByte = NumberOfTitles * 4 + 7;
            }

            // TODO use this technique everywhere to add checks for going outside the bounds of data structures
            ReadOnlySpan<byte> data = file.AsSpan(addr + 8, lastByte - 7);

            if (NumberOfTitles == 0)
            {
                throw new Exception("Zero entries in PTT search table.");
            }

            if (NumberOfTitles > data.Length / 4)
            {
                throw new Exception("PTT search table too small.");
            }

            List<int> offsets = new();
            for (int i = 0; i < NumberOfTitles; i++)
            {
                int offset = data.GetNbytes(i * 4, 4);
                if (offset + 4 > lastByte + 1)
                {
                    // Dont mess with butes beyond the end of the allocation
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"VTS_PTT_SRPT had {NumberOfTitles} titles, but was clipped to {i} to prevent rading outside the struct.");
                    Console.ResetColor();
                    this.NumberOfTitles = i;
                    break;
                }
                offsets.Add(offset);
            }

            TitleChapters = new VTS_PTT[NumberOfTitles];
            for (int i = 0; i < TitleChapters.Length; i++)
            {
                int pttLen;
                if (i < TitleChapters.Length - 1)
                {
                    pttLen = (offsets[i + 1] - offsets[i]);
                } else
                {
                    // The last ptt is calculated differently
                    pttLen = (lastByte + 1 - offsets[i]);
                }

                // A few DVDs were mastered strangley and have titles with 0 PTTs
                if (pttLen < 0) pttLen = 0;

                TitleChapters[i] = new VTS_PTT(file.AsSpan(addr + offsets[i], pttLen), addr + offsets[i]);
            }
        }

    }

    public class VTS_PTT
    {
        [JsonInclude]
        internal readonly int Address;

        [JsonInclude]
        readonly Chapter[] Chapters;

        internal VTS_PTT(ReadOnlySpan<byte> data, int globalAddr)
        {
            this.Address = globalAddr;

            // DVDs created by the VDR-to-DVD device LG RC590M violate the following requirement
            if (data.Length % 4 != 0)
            {
                throw new Exception("Invalid PTT size");
            }

            this.Chapters = new Chapter[data.Length / 4];
            for (int i = 0; i < this.Chapters.Length; i++)
            {
                this.Chapters[i] = new Chapter(data.Slice(i * 4, 4));
            }
        }

    }

    public struct Chapter
    {
        /// <summary>
        /// Also known as the "PGCN"
        /// </summary>
        [JsonInclude]
        public int ProgramChainNumber;

        /// <summary>
        /// Also known as the "PGN"
        /// </summary>
        [JsonInclude]
        public int ProgramNumber;

        internal Chapter(ReadOnlySpan<byte> data)
        {
            ProgramChainNumber = data.GetNbytes(0, 2);
            ProgramNumber = data.GetNbytes(2, 2);
        }
    }
}
