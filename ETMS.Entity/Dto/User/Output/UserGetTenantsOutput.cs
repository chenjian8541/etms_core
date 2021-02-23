using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.User.Output
{
    public class UserGetTenantsOutput
    {
        public int TenantId { get; set; }

        public string TenantName { get; set; }

        public string TenantCode { get; set; }

        public bool IsCurrentLogin { get; set; }
    }
}
