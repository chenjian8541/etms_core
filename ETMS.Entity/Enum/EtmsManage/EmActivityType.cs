using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Enum.EtmsManage
{
    public struct EmActivityType
    {
        /// <summary>
        /// 团购
        /// </summary>
        public const int GroupPurchase = 0;

        /// <summary>
        /// 砍价
        /// </summary>
        public const int Haggling = 1;

        /// <summary>
        /// 秒杀
        /// </summary>
        public const int Seckill = 2;

        /// <summary>
        /// 分销
        /// </summary>
        public const int Distribution = 3;
    }
}
