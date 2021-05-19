using System;
using System.Collections.Generic;
using System.Text;
using ETMS.Entity.EtmsManage.Common;

namespace ETMS.Entity.EtmsManage.Dto.User.Request
{
    public class UserRoleDelRequest : AgentRequestBase
    {
        public int Id { get; set; }

        public override string Validate()
        {
            if (Id <= 0)
            {
                return "数据校验不合法";
            }
            return base.Validate();
        }
    }
}
