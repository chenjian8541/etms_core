using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.BasicData.Request
{
    public class GetTenantInfoH5ByNoRequest : IValidate
    {
        public string TenantNo { get; set; }

        public string Validate()
        {
            if (string.IsNullOrEmpty(TenantNo))
            {
                return "请提供机构信息";
            }
            return string.Empty;
        }
    }
}
