using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace MakeMKV_Title_Decoder.libs.MakeMKV
{
    public struct MakeMkvProgress : TaskProgress
    {
        public uint Current { get; private set; }
        public uint Total { get; private set; }

        public uint CurrentMax => 65536;
        public uint TotalMax => 65536;

        public MakeMkvProgress()
        {
            Current = 0;
            Total = 0;
        }

        public MakeMkvProgress(int current, int total)
        {
            Current = (uint)current;
            Total = (uint)total;
        }

        public static bool operator ==(MakeMkvProgress left, MakeMkvProgress right)
        {
            return left.Current == right.Current && left.Total == right.Total;
        }

        public static bool operator !=(MakeMkvProgress left, MakeMkvProgress right)
        {
            return !(left == right);
        }

        public override bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }
            else if (obj is MakeMkvProgress other)
            {
                return this == other;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return (Current + Total).GetHashCode();
        }

        public override string ToString()
        {
            uint current = Current * 100 / CurrentMax;
            uint total = Total * 100 / TotalMax;
            return $"Progress=[Current: {current:00}%, Total: {total:00}%]";
        }
    }
}
