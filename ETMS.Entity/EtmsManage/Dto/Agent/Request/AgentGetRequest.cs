using ETMS.Entity.EtmsManage.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.EtmsManage.Dto.Agent.Request
{
    public class AgentGetRequest: AgentRequestBase
    {
        public int Id { get; set; }

        public override string Validate()
        {
            if (Id <= 0)
            {
                return "代理商Id不能为空";
            }
            return base.Validate();
        }
    }
}
