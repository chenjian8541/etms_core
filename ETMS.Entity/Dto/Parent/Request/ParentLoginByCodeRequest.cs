using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Parent.Request
{
    public class ParentLoginByCodeRequest : IValidate
    {
        public string TenantNo { get; set; }

        public string Code { get; set; }

        public string Validate()
        {
            if (string.IsNullOrEmpty(TenantNo))
            {
                return "请提供机构信息";
            }
            if (string.IsNullOrEmpty(Code))
            {
                return "授权code不能为空";
            }
            return string.Empty;
        }
    }
}
