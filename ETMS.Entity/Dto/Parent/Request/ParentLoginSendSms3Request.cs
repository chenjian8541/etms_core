using ETMS.Entity.CacheBucket;
using ETMS.Entity.Common;
using ETMS.Entity.Dto.Parent.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Parent.Request
{
    public class ParentLoginSendSms3Request : IValidate
    {
        public int TenantId { get; set; }

        public string Code { get; set; }

        public string Phone { get; set; }

        public string Validate()
        {
            if (TenantId <= 0)
            {
                return "请提供机构信息";
            }
            if (string.IsNullOrEmpty(Phone))
            {
                return "手机号码不能为空";
            }
            return string.Empty;
        }
    }
}