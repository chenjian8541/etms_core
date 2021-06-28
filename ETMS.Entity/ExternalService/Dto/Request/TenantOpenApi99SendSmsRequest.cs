using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.ExternalService.Dto.Request
{
    public class TenantOpenApi99SendSmsRequest : SmsBase
    {
        public TenantOpenApi99SendSmsRequest(int tenantId) : base(tenantId)
        {
        }

        public string SmsContent { get; set; }

        public List<string> Phones { get; set; }
    }
}
