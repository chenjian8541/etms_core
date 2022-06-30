using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.OpenParent2.Request
{
    public class WxMiniHagglingAssistGoRequest : OpenParent2RequestBase
    {
        public int TenantId { get; set; }

        public long ActivityRouteItemId { get; set; }

        public override string Validate()
        {
            if (TenantId <= 0)
            {
                return "请求数据格式错误";
            }
            if (ActivityRouteItemId <= 0)
            {
                return "请求数据格式错误";
            }
            return base.Validate();
        }
    }
}
