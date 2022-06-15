using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Enum
{
    public struct EmActivityStatus
    {
        /// <summary>
        /// 未发布
        /// </summary>
        public const int Unpublished = 0;

        /// <summary>
        /// 进行中
        /// </summary>
        public const int Processing = 1;

        /// <summary>
        /// 已下架
        /// </summary>
        public const int TakeDown = 2;

        /// <summary>
        /// 已结束
        /// </summary>
        public const int Over = 3;
    }
}
