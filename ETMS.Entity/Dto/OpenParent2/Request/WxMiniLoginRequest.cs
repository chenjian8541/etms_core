using ETMS.Entity.Common;
using ETMS.Entity.Enum;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.OpenParent2.Request
{
    public class WxMiniLoginRequest : IValidate
    {
        public string Code { get; set; }

        public string Validate()
        {
            if (string.IsNullOrEmpty(Code))
            {
                return "请求数据格式错误";
            }
            return string.Empty;
        }
    }
}
