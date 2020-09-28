using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    /// <summary>
    /// 费用 状态
    /// </summary>
    public struct EmCostStatus
    {
        /// <summary>
        /// 启用
        /// </summary>
        public const byte Enabled = 0;

        /// <summary>
        /// 禁用
        /// </summary>
        public const byte Disabled = 1;

        public static string GetCostStatusDesc(byte status)
        {
            switch (status)
            {
                case EmCostStatus.Enabled:
                    return "启用";
                case EmCostStatus.Disabled:
                    return "禁用";
            }
            return string.Empty;
        }
    }
}
