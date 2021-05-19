using ETMS.Entity.EtmsManage.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.EtmsManage.Dto.TenantManage.Request
{
    public class TenantSetUserRequest : AgentRequestBase
    {
        public List<int> Ids { get; set; }

        public long UserId { get; set; }

        public override string Validate()
        {
            if (Ids == null || Ids.Count == 0)
            {
                return "请选择机构";
            }
            if (UserId <= 0)
            {
                return "请选择业务员";
            }
            return base.Validate();
        }
    }
}