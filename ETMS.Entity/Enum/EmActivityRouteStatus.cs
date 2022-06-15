using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Enum
{
    public struct EmActivityRouteStatus
    {
        /// <summary>
        /// 无效（未支付）
        /// </summary>
        public const byte Invalid = 0;

        /// <summary>
        /// 正常（已支付）
        /// </summary>
        public const byte Normal = 1;
    }
}
