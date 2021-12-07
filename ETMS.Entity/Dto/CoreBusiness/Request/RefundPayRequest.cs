using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.CoreBusiness.Request
{
    public class RefundPayRequest
    {
        public DateTime Now { get; set; }

        public EtTenantLcsPayLog Paylog { get; set; }
    }
}
