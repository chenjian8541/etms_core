using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.PaymentService.Request
{
    public class MerchantQueryH5Request : IValidate
    {
        public string TenantNo { get; set; }

        public string UserNo { get; set; }

        public string Validate()
        {
            if (string.IsNullOrEmpty(TenantNo) || string.IsNullOrEmpty(UserNo))
            {
                return "请求数据格式错误";
            }
            return string.Empty;
        }
    }
}
