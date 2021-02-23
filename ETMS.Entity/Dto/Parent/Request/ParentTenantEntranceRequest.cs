using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Parent.Request
{
    public class ParentTenantEntranceRequest: ParentRequestBase
    {
        public int TenantId { get; set; }

        public override string Validate()
        {
            if (TenantId <= 0)
            {
                return "请选择机构信息";
            }
            return base.Validate();
        }
    }
}
