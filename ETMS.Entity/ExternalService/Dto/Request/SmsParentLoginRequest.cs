using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.ExternalService.Dto.Request
{
    public class SmsParentLoginRequest: SmsBase
    {
        public SmsParentLoginRequest(int tenantId) : base(tenantId)
        { }

        public string Phone { get; set; }

        public string ValidCode { get; set; }
    }
}
