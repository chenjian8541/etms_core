using ETMS.Entity.Common;
using ETMS.Entity.Enum;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Activity.Request
{
    public class ActivityRouteItemGetRequest : RequestBase
    {
        public long ActivityRouteId { get; set; }

        public override string Validate()
        {
            if (ActivityRouteId <= 0)
            {
                return "请求数据格式错误";
            }
            return string.Empty;
        }
    }
}
