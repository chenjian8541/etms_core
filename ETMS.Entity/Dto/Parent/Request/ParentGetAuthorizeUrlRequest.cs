using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Parent.Request
{
    public class ParentGetAuthorizeUrlRequest : IValidate
    {
        public string SourceUrl { get; set; }

        public string Validate()
        {
            if (string.IsNullOrEmpty(SourceUrl))
            {
                return "授权跳转地址不能为空";
            }
            return string.Empty;
        }
    }
}
