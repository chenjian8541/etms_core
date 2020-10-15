using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum.EtmsManage
{
    public struct EmSysTenantStatus
    {
        /// <summary>
        /// 正常
        /// </summary>
        public const byte Normal = 0;

        /// <summary>
        /// 已锁定
        /// </summary>
        public const byte IsLock = 1;

        /// <summary>
        /// 已过期
        /// </summary>
        public const byte Expired = 2;

        public static byte GetSysTenantStatus(byte t, DateTime exDate)
        {
            if (exDate < DateTime.Now.Date)
            {
                return Expired;
            }
            return t;
        }

        public static string GetSysTenantStatusDesc(byte t, DateTime exDate)
        {
            if (exDate < DateTime.Now.Date)
            {
                return "已过期";
            }
            switch (t)
            {
                case Normal:
                    return "正常";
                case IsLock:
                    return "已锁定";
                case Expired:
                    return "已过期";
            }
            return string.Empty;
        }
    }
}
