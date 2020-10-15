using ETMS.Entity.EtmsManage.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.EtmsManage.Dto.Agent.Request
{
    public class AgentPagingRequest : AgentPagingBase
    {
        public string Key { get; set; }

        public override string ToString()
        {
            var condition = new StringBuilder(DataFilterWhereGet("Id"));
            if (!string.IsNullOrEmpty(Key))
            {
                condition.Append($" AND (Name LIKE '{Key}%' OR Phone LIKE '{Key}%' )");
            }
            return condition.ToString();
        }
    }
}
