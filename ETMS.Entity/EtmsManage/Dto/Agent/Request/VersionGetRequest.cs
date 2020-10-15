using ETMS.Entity.EtmsManage.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.EtmsManage.Dto.Agent.Request
{
    public class VersionGetRequest : AgentRequestBase
    {
        public int CId { get; set; }

        public override string Validate()
        {
            if (CId <= 0)
            {
                return "代理商Id不能为空";
            }
            return base.Validate();
        }
    }
}
