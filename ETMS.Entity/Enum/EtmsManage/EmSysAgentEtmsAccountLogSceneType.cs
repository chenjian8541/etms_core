using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Enum.EtmsManage
{
    public struct EmSysAgentEtmsAccountLogSceneType
    {
        /// <summary>
        /// 授权点数管理(手动增加或扣减)
        /// 给机构充值
        /// </summary>
        public const int Change = 0;

        /// <summary>
        /// 撤销给机构的充值
        /// </summary>
        public const int TenantEtmsLogRepeal = 1;

        /// <summary>
        /// 删除机构（返还）
        /// </summary>
        public const int TenantDel = 2;
    }
}
