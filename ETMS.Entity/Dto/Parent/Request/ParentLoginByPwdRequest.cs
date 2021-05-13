using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Parent.Request
{
    public class ParentLoginByPwdRequest : IValidate
    {
        public string TenantNo { get; set; }

        public string Code { get; set; }

        public string Phone { get; set; }

        public string Pwd { get; set; }

        public string StudentWechartId { get; set; }

        public string IpAddress { get; set; }

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
            if (string.IsNullOrEmpty(Pwd))
            {
                return "密码不能为空";
            }
            if (string.IsNullOrEmpty(StudentWechartId))
            {
                return "请先授权用户信息";
            }
            return string.Empty;
        }
    }
}

