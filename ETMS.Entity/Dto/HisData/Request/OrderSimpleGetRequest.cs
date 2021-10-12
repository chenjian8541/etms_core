using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.HisData.Request
{
    public class OrderSimpleGetRequest : RequestBase
    {
        public string OrderNo { get; set; }

        public override string Validate()
        {
            if (string.IsNullOrEmpty(OrderNo))
            {
                return "请求数据格式错误";
            }
            return string.Empty;
        }
    }
}

