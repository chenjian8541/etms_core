using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Parent.Request
{
    public class ParentGetAuthorizeUrl2Request : ParentRequestBase
    {
        public string SourceUrl { get; set; }

        public string State { get; set; }

        public override string Validate()
        {
            if (string.IsNullOrEmpty(SourceUrl))
            {
                return "授权跳转地址不能为空";
            }
            return string.Empty;
        }
    }
}
