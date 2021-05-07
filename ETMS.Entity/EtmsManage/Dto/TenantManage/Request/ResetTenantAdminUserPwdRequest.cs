using ETMS.Entity.EtmsManage.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.EtmsManage.Dto.TenantManage.Request
{
    public class ResetTenantAdminUserPwdRequest : AgentRequestBase
    {
        public int TenantId { get; set; }

        public string NewPwd { get; set; }

        public override string Validate()
        {
            if (TenantId <= 0)
            {
                return "请求数据不合法";
            }
            if (string.IsNullOrEmpty(NewPwd))
            {
                return "请输入新密码";
            }
            return base.Validate();
        }
    }
}
