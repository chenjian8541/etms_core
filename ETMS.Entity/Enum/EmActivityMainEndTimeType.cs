using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Enum
{
    public struct EmActivityMainEndTimeType
    {
        /// <summary>
        /// 固定时间
        /// </summary>
        public const int DateTime = 0;

        /// <summary>
        /// 按小时
        /// </summary>
        public const int Hour = 1;

        /// <summary>
        /// 按天
        /// </summary>
        public const int Day = 2;
    }
}
