using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Enum.EtmsManage
{
    public struct EmSysActivityRouteItemStatus
    {
        /// <summary>
        /// 进行中
        /// </summary>
        public const int Going = 0;

        /// <summary>
        /// 已成团
        /// </summary>
        public const int FinishItem = 1;

        /// <summary>
        /// 已满团(已完成)
        /// </summary>
        public const int FinishFull = 2;

        /// <summary>
        /// 已过期
        /// </summary>
        public const int Expired = 3;
    }
}
