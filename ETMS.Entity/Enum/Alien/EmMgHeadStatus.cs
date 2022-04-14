using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Enum.Alien
{
    public struct EmMgHeadStatus
    {
        /// <summary>
        /// 正常
        /// </summary>
        public const byte Normal = 0;

        /// <summary>
        /// 已锁定
        /// </summary>
        public const byte IsLock = 1;

        public static string GetHeadStatusDesc(byte t)
        {
            return t == Normal ? "正常" : "已锁定";
        }
    }
}
