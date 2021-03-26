using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.User.Request
{
    public class UserLoginBySmsRequest : IValidate
    {
        public string Code { get; set; }

        public string Phone { get; set; }

        public string SmsCode { get; set; }

        public string IpAddress { get; set; }

        public int ClientType { get; set; }

        public string Validate()
        {
            Code = Code.Trim();
            Phone = Phone.Trim();
            SmsCode = SmsCode.Trim();
            if (string.IsNullOrEmpty(Code))
            {
                return "机构编码不能为空";
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
