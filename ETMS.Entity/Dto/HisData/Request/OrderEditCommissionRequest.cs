using ETMS.Entity.Common;
using ETMS.Entity.Dto.Common.Request;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.HisData.Request
{
    public class OrderEditCommissionRequest : RequestBase
    {
        public long CId { get; set; }

        public List<MultiSelectValueRequest> NewCommissionUsers { get; set; }

        public override string Validate()
        {
            if (CId <= 0)
            {
                return "请求数据不合法";
            }
            return string.Empty;
        }
    }
}
