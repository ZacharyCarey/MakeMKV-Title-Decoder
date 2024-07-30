using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.MakeMKV {
    internal struct DiscDrive {
        public UInt32 Index;
        public UInt32 Visible;
        public UInt32 Enabled;
        public UInt32 Flags; // see AP_DskFsFlagXXX in apdefs.h
        public string DriveName;
        public string DiscName;
        public string DriveLetter;

        public bool IsValid => !string.IsNullOrWhiteSpace(this.DriveName);
        public bool HasDisc => !string.IsNullOrWhiteSpace(this.DiscName);

        public override string ToString() {
            return DriveName;
        }
    }
}
