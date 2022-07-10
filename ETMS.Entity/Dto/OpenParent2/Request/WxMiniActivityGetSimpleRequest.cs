using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.OpenParent2.Request
{
    public class WxMiniActivityGetSimpleRequest: OpenParent2RequestBase
    {
        public int TenantId { get; set; }

        public long ActivityMainId { get; set; }

        public long? ActivityRouteId { get; set; }

        public override string Validate()
        {
            if (TenantId <= 0)
            {
                return "请求数据格式错误";
            }
            if (ActivityMainId <= 0)
            {
                return "请求数据格式错误";
            }
            return base.Validate();
        }
    }
}
