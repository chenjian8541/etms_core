using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.PaymentService.Request
{
    public class LcsPayQueryRequest : RequestBase
    {
        public long LcsAccountId { get; set; }

        public string OrderNo { get; set; }

        public string pay_type { get; set; }

        public string out_trade_no { get; set; }

        public override string Validate()
        {
            if (LcsAccountId <= 0)
            {
                return "请求数据格式错误";
            }
            if (string.IsNullOrEmpty(OrderNo))
            {
                return "请求数据格式错误";
            }
            //if (string.IsNullOrEmpty(out_trade_no))
            //{
            //    return "请求数据格式错误";
            //}
            return base.Validate();
        }
    }
}
