using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.User.Request
{
    public class UserGetAuthorizeUrlRequest : IValidate
    {
        public string TenantNo { get; set; }

        public string SourceUrl { get; set; }

        public string State { get; set; }

        public string Validate()
        {
            if (string.IsNullOrEmpty(TenantNo))
            {
                return "请提供机构信息";
            }
            if (string.IsNullOrEmpty(SourceUrl))
            {
                return "授权跳转地址不能为空";
            }
            return string.Empty;
        }
    }
}