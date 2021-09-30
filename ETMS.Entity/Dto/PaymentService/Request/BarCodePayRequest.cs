using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.PaymentService.Request
{
    public class BarCodePayRequest : RequestBase
    {
        public long StudentId { get; set; }

        public decimal PayMoney { get; set; }

        public string AuthNo { get; set; }

        public int OrderType { get; set; }

        public string OrderDesc { get; set; }

        public override string Validate()
        {
            if (StudentId <= 0)
            {
                return "请求数据格式错误";
            }
            if (PayMoney <= 0)
            {
                return "支付金额必须大于0";
            }
            if (string.IsNullOrEmpty(AuthNo))
            {
                return "请求数据格式错误";
            }
            return base.Validate();
        }
    }
}
