using ETMS.Entity.EtmsManage.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.EtmsManage.Dto.ClientUpgrade.Request
{
    public class SysClientUpgradePagingRequest : AgentPagingBase
    {
        public string VersionNo { get; set; }

        /// <summary>
        /// 是否需要限制用户数据
        /// </summary>
        /// <returns></returns>
        public override bool IsNeedLimitUserData()
        {
            return true;
        }

        public override string ToString()
        {
            var condition = new StringBuilder(DataFilterWhereGet("AgentId"));
            if (!string.IsNullOrEmpty(VersionNo))
            {
                condition.Append($" AND [VersionNo] LIKE '%{VersionNo}%'");
            }
            return condition.ToString();
        }
    }
}
