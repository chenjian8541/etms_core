using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Open.Request
{
    public class ComponentOAuthGetRequest : RequestBase
    {
        public string SmsCode { get; set; }

        public override string Validate()
        {
            if (string.IsNullOrEmpty(SmsCode))
            {
                return "请先验证管理员身份";
            }
            return base.Validate();
        }
    }
}
