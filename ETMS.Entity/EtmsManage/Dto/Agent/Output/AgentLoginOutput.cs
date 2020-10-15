using ETMS.Entity.Dto.User.Output;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.EtmsManage.Dto.Agent.Output
{
    public class AgentLoginOutput
    {
        /// <summary>
        /// 授权Token
        /// </summary>
        public string Token { get; set; }

        public DateTime ExpiresTime { get; set; }

        public PermissionOutput Permission { get; set; }
    }
}
