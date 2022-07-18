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

        public static int GetActivityRouteItemStatus(int t, DateTime endTime)
        {
            if (t == Going && DateTime.Now >= endTime)
            {
                return Expired;
            }
            return t;
        }

        public static string GetActivityRouteItemStatusDesc(int activityType, int t, DateTime endTime)
        {
            if (t == Going && DateTime.Now >= endTime)
            {
                return "已过期";
            }
            if (activityType == EmActivityType.Haggling)
            {
                switch (t)
                {
                    case Going:
                    case FinishItem:
                        return "进行中";
                    case FinishFull:
                        return "砍价成功";
                    case Expired:
                        return "已过期";
                }
                return string.Empty;
            }
            switch (t)
            {
                case Going:
                    return "进行中";
                case FinishItem:
                    return "已成团";
                case FinishFull:
                    return "已满团";
                case Expired:
                    return "已过期";
            }
            return string.Empty;
        }
    }
}
