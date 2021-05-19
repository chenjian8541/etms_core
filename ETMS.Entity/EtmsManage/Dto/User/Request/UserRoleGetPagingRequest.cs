using ETMS.Entity.EtmsManage.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.EtmsManage.Dto.User.Request
{
    public class UserRoleGetPagingRequest : AgentPagingBase
    {
        public int? AgentId { get; set; }

        public string Name { get; set; }

        public override string ToString()
        {
            var condition = new StringBuilder(DataFilterWhereGet());
            if (AgentId != null)
            {
                condition.Append($" AND AgentId = {AgentId.Value}");
            }
            if (!string.IsNullOrEmpty(Name))
            {
                condition.Append($" AND Name LIKE '%{Name}%'");
            }
            return condition.ToString();
        }
    }
}
