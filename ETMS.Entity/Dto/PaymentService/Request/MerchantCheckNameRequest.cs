using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.PaymentService.Request
{
    public class MerchantCheckNameRequest : IValidate
    {
        public string MerchantName { get; set; }

        public string Validate()
        {
            if (string.IsNullOrEmpty(MerchantName))
            {
                return "请填写商户名称";
            }
            return string.Empty;
        }
    }
}
