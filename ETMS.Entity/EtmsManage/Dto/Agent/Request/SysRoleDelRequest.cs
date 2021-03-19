using ETMS.Entity.EtmsManage.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.EtmsManage.Dto.Agent.Request
{
    public class SysRoleDelRequest : AgentRequestBase
    {
        public int CId { get; set; }

        public override string Validate()
        {
            if (CId <= 0)
            {
                return "请求数据格式错误";
            }
            return string.Empty;
        }
    }
}