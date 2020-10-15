using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum.EtmsManage
{
    public struct EmSysAgentIsLock
    {
        /// <summary>
        /// 正常
        /// </summary>
        public const byte Normal = 0;

        /// <summary>
        /// 锁定
        /// </summary>
        public const byte IsLock = 1;

        public static string GetSysAgentIsLockDesc(byte b)
        {
            if (b == Normal)
            {
                return "正常";
            }
            return "锁定";
        }
    }
}
