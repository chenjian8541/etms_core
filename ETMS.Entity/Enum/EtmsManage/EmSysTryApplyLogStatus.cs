using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Enum.EtmsManage
{
    public struct EmSysTryApplyLogStatus
    {
        /// <summary>
        /// 未处理
        /// </summary>
        public const byte Untreated = 0;

        /// <summary>
        /// 已处理
        /// </summary>
        public const byte Processed = 1;
    }
}
