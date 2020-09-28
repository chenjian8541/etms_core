using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    /// <summary>
    /// 收支明细状态
    /// </summary>
    public struct EmIncomeLogStatus
    {
        /// <summary>
        /// 正常
        /// </summary>
        public const byte Normal = 0;

        /// <summary>
        /// 作废,无效
        /// </summary>
        public const byte Repeal = 1;

        public static string GetIncomeLogStatusDesc(byte b)
        {
            if (b == Normal)
            {
                return "正常";
            }
            return "已作废";
        }
    }
}
