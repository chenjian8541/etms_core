using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Open2.Request
{
    public class SendSmsCodeAboutRegisterRequest : IValidate
    {
        public string Phone { get; set; }

        public string VerificationCode { get; set; }

        public string Validate()
        {
            if (string.IsNullOrEmpty(Phone))
            {
                return "手机号码不能为空";
            }
            if (string.IsNullOrEmpty(VerificationCode))
            {
                return "校验码不能为空";
            }
            return string.Empty;
        }
    }
}
