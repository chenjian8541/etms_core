using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.OpenParent.Request
{
    public class ParentOpenLoginBySmsRequest : IValidate
    {
        public string TenantNo { get; set; }

        public string Phone { get; set; }

        public string SmsCode { get; set; }

        public string Validate()
        {
            if (string.IsNullOrEmpty(TenantNo))
            {
                return "请提供机构信息";
            }
            if (string.IsNullOrEmpty(Phone))
            {
                return "手机号码不能为空";
            }
            if (string.IsNullOrEmpty(SmsCode))
            {
                return "验证码不能为空";
            }
            return string.Empty;
        }
    }
}
