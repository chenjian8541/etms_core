using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Open2.Request
{
    public class GetJsSdkUiPackageRequest : Open2Base
    {
        public string Url { get; set; }

        public override string Validate()
        {
            if (string.IsNullOrEmpty(Url))
            {
                return "请求数据格式错误";
            }
            return base.Validate();
        }
    }
}
