using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Parent.Request
{
    public class WxMessageDetailSetReadRequest : ParentRequestBase
    {
        public long WxMessageDetailId { get; set; }

        public override string Validate()
        {
            if (WxMessageDetailId <= 0)
            {
                return "请求数据不合法";
            }
            return base.Validate();
        }
    }
}
