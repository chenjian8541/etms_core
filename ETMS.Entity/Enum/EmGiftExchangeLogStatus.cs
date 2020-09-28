using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    /// <summary>
    /// 礼品兑换记录状态
    /// </summary>
    public struct EmGiftExchangeLogStatus
    {
        /// <summary>
        /// 未处理
        /// </summary>
        public const int Unprocessed = 0;

        /// <summary>
        /// 已处理
        /// </summary>
        public const int Processed = 1;

        ///// <summary>
        ///// 已通过
        ///// </summary>
        //public const int Processed = 2;


        public static string GetStatusDesc(byte status)
        {
            switch (status)
            {
                case EmGiftExchangeLogStatus.Unprocessed:
                    return "未处理";
                case EmGiftExchangeLogStatus.Processed:
                    return "已处理";
                default:
                    return string.Empty;
            }
        }
    }
}
