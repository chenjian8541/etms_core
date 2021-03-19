using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Product.Request
{
    public class GoodsStatusChangeRequest : RequestBase
    {
        public long CId { get; set; }

        public byte NewStatus { get; set; }

        public override string Validate()
        {
            if (CId <= 0)
            {
                return "请求数据格式错误";
            }
            return string.Empty;
        }
    }
}