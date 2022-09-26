using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Parent.Request
{
    public class ParentEnterH5Request : ParentRequestBase
    {
        public string TenantNo { get; set; }

        public override string Validate()
        {
            if (string.IsNullOrEmpty(TenantNo))
            {
                return "请求数据格式错误";
            }
            return base.Validate();
        }
    }
}
