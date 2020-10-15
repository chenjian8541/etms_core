using ETMS.Entity.EtmsManage.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.EtmsManage.Dto.Agent.Request
{
    public class AgentSetPwdRequest : AgentRequestBase
    {
        public int Id { get; set; }

        public string NewPwd { get; set; }

        public override string Validate()
        {
            if (Id <= 0)
            {
                return "代理商Id不能为空";
            }
            if (string.IsNullOrEmpty(NewPwd))
            {
                return "密码不能为空";
            }
            return base.Validate();
        }
    }
}
