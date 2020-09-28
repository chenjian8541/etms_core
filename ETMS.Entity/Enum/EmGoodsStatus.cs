using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    /// <summary>
    /// 物品状态
    /// </summary>
    public struct EmGoodsStatus
    {
        /// <summary>
        /// 启用
        /// </summary>
        public const byte Enabled = 0;

        /// <summary>
        /// 禁用
        /// </summary>
        public const byte Disabled = 1;

        public static string GetGoodsStatusDesc(byte status)
        {
            switch (status)
            {
                case EmGoodsStatus.Enabled:
                    return "启用";
                case EmGoodsStatus.Disabled:
                    return "禁用";
            }
            return string.Empty;
        }
    }
}
