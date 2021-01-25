using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.User.Request
{
    public class UserBindingWeChatRequest : RequestBase
    {
        public string Code { get; set; }

        public override string Validate()
        {
            if (string.IsNullOrEmpty(Code))
            {
                return "授权code不能为空";
            }
            return string.Empty;
        }
    }
}
