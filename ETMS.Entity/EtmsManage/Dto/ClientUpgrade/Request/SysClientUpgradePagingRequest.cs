using ETMS.Entity.EtmsManage.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.EtmsManage.Dto.ClientUpgrade.Request
{
    public class SysClientUpgradePagingRequest : AgentPagingBase
    {
        public string VersionNo { get; set; }

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
