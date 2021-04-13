using ETMS.Entity.EtmsManage.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.EtmsManage.Dto.TenantManage.Request
{
    public class AIFaceBiduAccountGetPagingRequest : AgentPagingBase
    {
        public string Remark { get; set; }

        public override string ToString()
        {
            var condition = new StringBuilder(DataFilterWhereGet("AgentId"));
            if (!string.IsNullOrEmpty(Remark))
            {
                condition.Append($" AND [Remark] LIKE '%{Remark}%'");
            }
            return condition.ToString();
        }
    }
}
