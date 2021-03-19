using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    public struct EmClassTimesReservationLogStatus
    {
        /// <summary>
        /// 已约课
        /// </summary>
        public const byte Normal = 0;

        /// <summary>
        /// 已取消
        /// </summary>
        public const byte Cancel = 1;

        /// <summary>
        /// 已上课(学员已到课)
        /// </summary>
        public const byte BeClassArrived = 2;

        /// <summary>
        /// 已失效
        /// </summary>
        public const byte Invalidation = 3;

        public static byte GetClassTimesReservationLogStatus(byte status, DateTime now, DateTime classOt)
        {
            if (status == Normal)
            {
                if (classOt < now)
                {
                    return Invalidation;
                }
            }
            return status;
        }

        public static string GetClassTimesReservationLogStatusDesc(byte status, DateTime now, DateTime classOt)
        {
            if (status == Normal)
            {
                if (classOt < now)
                {
                    status = Invalidation;
                }
            }
            switch (status)
            {
                case Normal:
                    return "已约课";
                case Cancel:
                    return "已取消";
                case BeClassArrived:
                    return "已上课";
                case Invalidation:
                    return "已失效";
            }
            return string.Empty;
        }
    }
}
