using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.PaymentService.Request
{
    public class LcsPayRefundRequest : RequestBase
    {
        public long LcsAccountId { get; set; }

        public override string Validate()
        {
            if (LcsAccountId <= 0)
            {
                return "请求数据格式错误";
            }
            return base.Validate();
        }
    }
}
