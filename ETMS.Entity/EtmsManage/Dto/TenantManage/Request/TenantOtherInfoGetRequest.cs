using ETMS.Entity.EtmsManage.Common;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.EtmsManage.Dto.TenantManage.Request
{
    public class TenantOtherInfoGetRequest : AgentRequestBase
    {
        public int TenantId { get; set; }

        public override string Validate()
        {
            if (TenantId <= 0)
            {
                return "机构Id不能为空";
            }
            return base.Validate();
        }
    }
}
