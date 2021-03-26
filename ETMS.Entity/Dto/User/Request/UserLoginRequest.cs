using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.User.Request
{
    /// <summary>
    /// 用户登录
    /// </summary>
    public class UserLoginRequest : IValidate
    {
        public string Code { get; set; }

        public string Phone { get; set; }

        public string Pwd { get; set; }

        public string IpAddress { get; set; }

        public string Validate()
        {
            Code = Code.Trim();
            Phone = Phone.Trim();
            Pwd = Pwd.Trim();
            if (string.IsNullOrEmpty(Code))
            {
                return "机构编码不能为空";
            }
            if (string.IsNullOrEmpty(Phone))
            {
                return "手机号码不能为空";
            }
            if (string.IsNullOrEmpty(Pwd))
            {
                return "密码不能为空";
            }
            return string.Empty;
        }
    }
}
