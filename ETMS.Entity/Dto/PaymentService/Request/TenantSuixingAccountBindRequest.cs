using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.PaymentService.Request
{
    public class TenantSuixingAccountBindRequest : RequestBase
    {
        public string Mno { get; set; }

        public override string Validate()
        {
            if (string.IsNullOrEmpty(Mno))
            {
                return "商户编号不能为空";
            }
            return string.Empty;
        }
    }
}
