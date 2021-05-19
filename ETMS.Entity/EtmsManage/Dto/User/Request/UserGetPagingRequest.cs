using ETMS.Entity.EtmsManage.Common;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.EtmsManage.Dto.User.Request
{
    public class UserGetPagingRequest : AgentPagingBase
    {
        public int? AgentId { get; set; }

        public string Phone { get; set; }

        public override string ToString()
        {
            var condition = new StringBuilder(DataFilterWhereGet());
            if (AgentId != null)
            {
                condition.Append($" AND AgentId = {AgentId.Value}");
            }
            if (!string.IsNullOrEmpty(Phone))
            {
                condition.Append($" AND Phone LIKE '%{Phone}%'");
            }
            return condition.ToString();
        }
    }
}
