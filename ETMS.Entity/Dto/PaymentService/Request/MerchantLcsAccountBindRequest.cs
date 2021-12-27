using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.PaymentService.Request
{
    public class MerchantLcsAccountBindRequest : RequestBase
    {
        public string MerchantNo { get; set; }

        public string TerminalId { get; set; }

        public override string Validate()
        {
            if (string.IsNullOrEmpty(MerchantNo))
            {
                return "请输入商户号";
            }
            if (string.IsNullOrEmpty(TerminalId))
            {
                return "请输入终端号";
            }
            return base.Validate();
        }
    }
}
