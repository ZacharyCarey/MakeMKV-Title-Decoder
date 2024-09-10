using libbluray.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace libbluray.bdnav.Indx {

    public enum indx_access_type : UInt32 {
        /// <summary>
        /// jump into this title is permitted.  title number may be shown on UI.
        /// </summary>
        permitted = 0,

        /// <summary>
        /// jump into this title is prohibited. title number may be shown on UI.
        /// </summary>
        prohibited = 1,

        /// <summary>
        /// jump into this title is prohibited. title number shall not be shown on UI.
        /// </summary>
        hidden = 3,

        /// <summary>
        /// if set, jump to this title is not allowed
        /// </summary>
        PROHIBITED_MASK = 0x01,

        /// <summary>
        /// if set, title number shall not be displayed on UI
        /// </summary>
        HIDDEN_MASK = 0x02,

        Unknown = 0xFFFFFFFF
    }

    public class IndexTitle {
        const string module = "libbluray.bdnav.Indx.IndexTitle";

        public indx_object_type object_type;
        public indx_access_type access_type;
        public IndexBdjObject bdj = new();
        public IndexHdmvObject hdmv = new();

        public bool Parse(BitStream bits) {
            this.object_type = bits.ReadEnum(2, indx_object_type.Unknown, module);
            this.access_type = bits.ReadEnum(2, indx_access_type.Unknown, module);
            bits.Skip(28);

            switch (this.object_type)
            {
                case indx_object_type.hdmv:
                    if (!this.hdmv.Parse(bits))
                    {
                        return false;
                    }
                    break;
                case indx_object_type.bdj:
                    if (!this.bdj.Parse(bits))
                    {
                        return false;
                    }
                    break;
                default:
                    Utils.BD_DEBUG(LogLevel.Critical, module, $"index.bdmv: unknown object type {this.object_type}");
                    return false;
            }

            return true;
        }
    }
}
