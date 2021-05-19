using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.EtmsManage.Dto.Agent.Output
{
    public class CheckAgentLoginOutput
    {
        public bool IsRoleLimitData { get; set; }

        public bool IsUserLimitData { get; set; }
    }
}
