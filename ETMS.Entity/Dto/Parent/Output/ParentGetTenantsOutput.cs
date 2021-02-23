using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Parent.Output
{
    public class ParentGetTenantsOutput
    {
        public int TenantId { get; set; }

        public string TenantName { get; set; }

        public string TenantCode { get; set; }

        public bool IsCurrentLogin { get; set; }
    }
}
