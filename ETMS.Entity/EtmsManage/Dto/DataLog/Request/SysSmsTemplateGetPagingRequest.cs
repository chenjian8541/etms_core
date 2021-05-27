using ETMS.Entity.EtmsManage.Common;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.EtmsManage.Dto.DataLog.Request
{
    public class SysSmsTemplateGetPagingRequest : AgentPagingBase
    {
        public int? AgentId { get; set; }

        public int? TenantId { get; set; }

        public byte? HandleStatus { get; set; }

        public override string ToString()
        {
            var condition = new StringBuilder(DataFilterWhereGet("AgentId"));
            if (AgentId != null)
            {
                condition.Append($" AND AgentId = {AgentId.Value}");
            }
            if (TenantId != null)
            {
                condition.Append($" AND TenantId = {TenantId.Value}");
            }
            if (HandleStatus != null)
            {
                condition.Append($" AND HandleStatus = {HandleStatus.Value}");
            }
            return condition.ToString();
        }
    }
}
