using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.libs.MakeMKV
{
    public struct DiscDrive
    {
        public uint Index;
        public uint Visible;
        public uint Enabled;
        public uint Flags; // see AP_DskFsFlagXXX in apdefs.h
        public string DriveName;
        public string DiscName;
        public string DriveLetter;

        public bool IsValid => !string.IsNullOrWhiteSpace(DriveName);
        public bool HasDisc => !string.IsNullOrWhiteSpace(DiscName);

        public override string ToString()
        {
            return DriveName;
        }
    }
}
