using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Enum.EtmsManage
{
    public struct EmTenantSuixingAccountStatus
    {
        /// <summary>
        /// 启用
        /// </summary>
        public const int Enable = 0;

        /// <summary>
        /// 停用
        /// </summary>
        public const int Stop = 1;

        /// <summary>
        /// 注销账户
        /// </summary>
        public const int AccountInvalid = 2;
    }
}
