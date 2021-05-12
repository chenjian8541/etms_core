using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    public struct EmUserSmsLogType
    {
        /// <summary>
        /// 上课通知
        /// </summary>
        public const int NoticeOfClassToday = 1;

        public static string GetTypeDesc(int type)
        {
            switch (type)
            {
                case NoticeOfClassToday:
                    return "上课通知";
            }
            return string.Empty;
        }
    }
}
