using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.HisData.Request
{
    public class OrderRepealRequest : RequestBase
    {
        public long OrderId { get; set; }

        public string Remark { get; set; }

        public override string Validate()
        {
            if (OrderId <= 0)
            {
                return "请求数据不合法";
            }
            if (string.IsNullOrEmpty(Remark))
            {
                return "请填写作废原因";
            }
            return string.Empty;
        }
    }
}
