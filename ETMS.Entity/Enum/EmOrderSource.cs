using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Enum
{
    public struct EmOrderSource
    {
        /// <summary>
        /// 机构创建
        /// </summary>
        public const int TenantCreate = 0;

        /// <summary>
        /// 在线商城
        /// </summary>
        public const int MallGoodsOrder = 1;

        /// <summary>
        /// 导入订单
        /// </summary>
        public const int OrderImport = 2;

        public static string GetOrderSourceDesc(int t)
        {
            switch (t)
            {
                case TenantCreate:
                    return "机构创建";
                case MallGoodsOrder:
                    return "在线商城";
                case OrderImport:
                    return "导入订单";
            }
            return string.Empty;
        }
    }
}
