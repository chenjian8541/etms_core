using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum.EtmsManage
{
    public struct EmSysUpgradeMsgStatus
    {
        public const byte Normal = 0;

        public const byte Invalid = 1;

        public static string GetSysUpgradeMsgStatusDesc(byte b)
        {
            if (b == Normal)
            {
                return "正常";
            }
            return "无效";
        }
    }
}
