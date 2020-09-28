using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    /// <summary>
    /// 收支类型
    /// </summary>
    public struct EmIncomeLogType
    {
        /// <summary>
        /// 收入
        /// </summary>
        public const byte AccountIn = 0;

        /// <summary>
        /// 支出
        /// </summary>
        public const byte AccountOut = 1;


        public static string GetIncomeLogType(byte b)
        {
            switch (b)
            {
                case AccountIn:
                    return "收入";
                case AccountOut:
                    return "支出";
            }
            return string.Empty;
        }
    }
}
