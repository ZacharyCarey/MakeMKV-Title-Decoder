using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public interface TaskProgress
    {
        public UInt32 Current { get; }
        public UInt32 CurrentMax { get; }

        public UInt32 Total { get; }
        public UInt32 TotalMax { get; }
    }

    public struct SimpleProgress : TaskProgress
    {

        public UInt32 Current { get; set; }
        public UInt32 CurrentMax { get; set; }

        public UInt32 Total { get; set; }
        public UInt32 TotalMax { get; set; }

        public SimpleProgress()
        {
            this.Current = 0;
            this.CurrentMax = 0;

            this.Total = 0;
            this.TotalMax = 0;
        }

        public SimpleProgress(UInt32 total, UInt32 totalMax)
        {
            this.Current = 0;
            this.CurrentMax = 0;

            this.Total = total;
            this.TotalMax = totalMax;
        }

        public SimpleProgress(UInt32 current, UInt32 currentMax, UInt32 total, UInt32 totalMax)
        {
            this.Current = current;
            this.CurrentMax = currentMax;

            this.Total = total;
            this.TotalMax = totalMax;
        }

    }
}
