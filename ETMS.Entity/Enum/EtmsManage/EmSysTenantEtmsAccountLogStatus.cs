using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Enum.EtmsManage
{
    public struct EmSysTenantEtmsAccountLogStatus
    {
        /// <summary>
        /// 正常
        /// </summary>
        public const byte Normal = 0;

        /// <summary>
        /// 已撤销
        /// </summary>
        public const byte Revoked = 1;

        public static string GetEtmsAccountLogStatusDesc(byte b)
        {
            return b == Normal ? "正常" : "已撤销";
        }
    }
}
