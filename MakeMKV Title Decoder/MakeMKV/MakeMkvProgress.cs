using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.MakeMKV {
    public struct MakeMkvProgress {
        public int Current;
        public int Total;

        public static MakeMkvProgress Max => new MakeMkvProgress(65536, 65536);

        public MakeMkvProgress() {
            this.Current = 0;
            this.Total = 0;
        }

        public MakeMkvProgress(int current, int total) {
            this.Current = current;
            this.Total = total;
        }

        public static bool operator ==(MakeMkvProgress left, MakeMkvProgress right) {
            return (left.Current == right.Current) && (left.Total == right.Total);
        }

        public static bool operator !=(MakeMkvProgress left, MakeMkvProgress right) {
            return !(left == right);
        }

        public override bool Equals(object? obj) {
            if (obj == null)
            {
                return false;
            } else if (obj is MakeMkvProgress other)
            {
                return this == other;
            } else
            {
                return false;
            }
        }

        public override int GetHashCode() {
            return (this.Current + this.Total).GetHashCode();
        }

        public override string ToString() {
            int current = this.Current * 100 / 65536;
            int total = this.Total * 100 / 65536;
            return $"Progress=[Current: {current:00}%, Total: {total:00}%]";
        }
    }
}
