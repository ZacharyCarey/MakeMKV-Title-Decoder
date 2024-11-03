using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PgcDemuxLib.Data.VMG
{
    /// <summary>
    /// https://dvd.sourceforge.net/dvdinfo/ifo_vmg.html#tt
    /// </summary>
    public class TT_SRPT
    {
        /// <summary>
        /// Byte index in file
        /// </summary>
        [JsonInclude]
        internal readonly int Address;

        [JsonInclude]
        TitleInfo[] TitleInfo;

        public TitleInfo this[int index]
        {
            get => TitleInfo[index];
        }

        internal TT_SRPT(byte[] file, int addr)
        {
            Address = addr;

            int numTitles = file.GetNbytes(addr, 2);
            int lastByte = file.GetNbytes(addr + 4, 4);

            // Some odd releases dont fill out this field
            if (lastByte == 0)
            {
                lastByte = numTitles * 12 + 7;
            }
            ReadOnlySpan<byte> data = file.AsSpan(addr + 8, lastByte - 7);
            int globalAddr = addr + 8;

            int numEntries = data.Length / 12;
            if (numTitles > data.Length / 12)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"WARNING: TT_SRPT has {numTitles} titles, but there are only enough bytes for {numEntries} entries. Data may be truncated.");
                Console.ResetColor();
                numTitles = numEntries;
            }

            TitleInfo = new TitleInfo[numTitles];
            for (int i = 0; i < TitleInfo.Length; i++)
            {
                TitleInfo[i] = new TitleInfo(data.Slice(i * 12, 12), globalAddr + i * 12);
            }
        }
    }

    public readonly struct TitleInfo
    {
        [JsonInclude]
        internal readonly int Address;

        [JsonInclude]
        public readonly TitleType Type;

        [JsonInclude]
        public readonly int NumberOfAngles;

        /// <summary>
        /// On a DVD, PTT = Chapters
        /// </summary>
        [JsonInclude]
        public readonly int NumberOfChapters;

        [JsonInclude]
        public readonly int ParentalManagementMask;

        /// <summary>
        /// Also known as "VTSN"
        /// </summary>
        [JsonInclude]
        public readonly int VideoTitleSetNumber;

        /// <summary>
        /// Title number within VTS, also known as VTS_TTN
        /// </summary>
        [JsonInclude]
        public readonly int TitleNumber;

        internal readonly int VtsAddress;

        internal TitleInfo(ReadOnlySpan<byte> data, int globalAddr)
        {
            this.Address = globalAddr;
            Type = new TitleType(data[0]);
            NumberOfAngles = data[1];
            NumberOfChapters = data.GetNbytes(2, 2);
            ParentalManagementMask = data.GetNbytes(4, 2);
            VideoTitleSetNumber = data[6];
            TitleNumber = data[7];
            VtsAddress = data.GetNbytes(8, 4);
        }
    }

    public readonly struct TitleType
    {
        [JsonInclude]
        public readonly bool IsSequential;

        [JsonInclude]
        public readonly bool CommandsValidInCell;

        [JsonInclude]
        public readonly bool CommandsValidInPrePost;

        [JsonInclude]
        public readonly bool CommandsValidInButton;

        /// <summary>
        /// Can only be <see cref="UserOperationFlag.TimePlayOrSearch"/> or <see cref="UserOperationFlag.PttPlayOrSearch"/>
        /// </summary>
        [JsonInclude, JsonConverter(typeof(JsonStringEnumConverter))]
        public readonly UserOperationFlag UserOperations;

        internal TitleType(byte data)
        {
            IsSequential = (data >> 7 & 1) == 1;

            bool CommandsValidInCell = (data >> 5 & 1) == 1;
            bool CommandsValidInPrePost = (data >> 4 & 1) == 1;
            bool CommandsValidInButton = (data >> 3 & 1) == 1;
            bool cmdValid = (data >> 2 & 1) == 1;

            if (!cmdValid)
            {
                CommandsValidInCell = false;
                CommandsValidInPrePost = false;
                CommandsValidInButton = false;
            }

            // UserOperation flags in the file, 0 = Allowed, 1 = inhibited
            UserOperations = UserOperationFlag.None;
            if ((data >> 1 & 1) == 0) UserOperations |= UserOperationFlag.PttPlayOrSearch;
            if ((data & 1) == 0) UserOperations |= UserOperationFlag.TimePlayOrSearch;
        }
    }
}
