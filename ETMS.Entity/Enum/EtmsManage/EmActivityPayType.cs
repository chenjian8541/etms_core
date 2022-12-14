using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Enum.EtmsManage
{
    public struct EmActivityPayType
    {
        /// <summary>
        /// 团购：团购价
        /// 砍价：可以直接支付
        /// </summary>
        public const int Type0 = 0;

        /// <summary>
        /// 团购：定金
        /// 砍价：必须砍到底价才能支付
        /// </summary>
        public const int Type1 = 1;

        public static string GetActivityPayTypeDesc(int activityType, int t)
        {
            if (activityType == EmActivityType.GroupPurchase)
            {
                return t == Type0 ? "团价" : "定金";
            }
            if (activityType == EmActivityType.Haggling)
            {
                return "砍后价";
            }
            return string.Empty;
        }
    }
}
