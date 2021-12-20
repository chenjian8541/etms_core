using ETMS.Entity.Enum;
using ETMS.Entity.EtmsManage.Common;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.EtmsManage.Dto.DataLog.Request
{
    public class DangerousIpPagingRequest : AgentPagingBase
    {
        public string Ip { get; set; }

        public override string ToString()
        {
            var condition = new StringBuilder($"IsDeleted = {EmIsDeleted.Normal}");
            if (!string.IsNullOrEmpty(Ip))
            {
                condition.Append($" AND (RemoteIpAddress LIKE '%{Ip}%' OR LocalIpAddress LIKE '%{Ip}%')");
            }
            return condition.ToString();
        }
    }
}
