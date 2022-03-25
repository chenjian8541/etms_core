using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.User.Request
{
    public class UserLoginSendSmsCodeSafeRequest : IValidate
    {
        public string VerificationCode { get; set; }
        public string Code { get; set; }

        public string Phone { get; set; }

        public string Validate()
        {
            Code = Code.Trim();
            Phone = Phone.Trim();
            if (string.IsNullOrEmpty(Code))
            {
                return "机构编码不能为空";
            }
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
