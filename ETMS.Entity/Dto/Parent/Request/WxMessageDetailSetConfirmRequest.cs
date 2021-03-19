using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Parent.Request
{
    public class WxMessageDetailSetConfirmRequest : ParentRequestBase
    {
        public long WxMessageDetailId { get; set; }

        public override string Validate()
        {
            if (WxMessageDetailId <= 0)
            {
                return "请求数据格式错误";
            }
            return base.Validate();
        }
    }
}
