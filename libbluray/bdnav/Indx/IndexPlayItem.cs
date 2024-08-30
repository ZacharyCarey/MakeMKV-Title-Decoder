using libbluray.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libbluray.bdnav.Indx {

    public enum indx_object_type : UInt32 {
        hdmv = 1,
        bdj = 2,

        Unknown = 0xFFFFFFFF
    }

    public class IndexPlayItem {
        const string module = "libbluray.bdnav.Indx.IndexPlayItem";

        public indx_object_type object_type;
        public IndexBdjObject bdj = new();
        public IndexHdmvObject hdmv = new();

        public bool Parse(BitStream bits) {
            this.object_type = bits.ReadEnum(2, indx_object_type.Unknown);
            bits.Skip(30);

            switch (this.object_type)
            {
                case indx_object_type.hdmv:
                    return this.hdmv.Parse(bits);
                case indx_object_type.bdj:
                    return this.bdj.Parse(bits);
            }

            Utils.BD_DEBUG(LogLevel.Critical, module, $"index.bdmv: unknown object type {this.object_type}");
            return false;
        }
    }
}
