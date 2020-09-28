using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    /// <summary>
    /// 试听记录状态
    /// </summary>
    public struct EmTryCalssLogStatus
    {
        /// <summary>
        /// 已预约
        /// </summary>
        public const byte IsBooked = 0;

        /// <summary>
        /// 已体验
        /// </summary>
        public const byte IsTry = 1;

        /// <summary>
        /// 已取消
        /// </summary>
        public const byte IsCancel = 2;

        /// <summary>
        /// 已过期
        /// </summary>
        public const byte IsExpired = 3;

        public static string GetTryCalssLogStatus(byte b)
        {
            switch (b)
            {
                case IsBooked:
                    return "已预约";
                case IsTry:
                    return "已体验";
                case IsCancel:
                    return "已取消";
                case IsExpired:
                    return "已过期";
            }
            return string.Empty;
        }
    }
}
