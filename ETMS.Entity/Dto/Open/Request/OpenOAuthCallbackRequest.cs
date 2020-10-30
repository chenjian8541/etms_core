using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Open.Request
{
    public class OpenOAuthCallbackRequest
    {
        public string AuthCode { get; set; }

        public int TenantId { get; set; }

        public string AppId { get; set; }

        public int ExpiresIn { get; set; }
    }
}
