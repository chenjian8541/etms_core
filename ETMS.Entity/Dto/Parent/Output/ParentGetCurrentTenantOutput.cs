using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Parent.Output
{
    public class ParentGetCurrentTenantOutput
    {
        public int CurrentTenantId { get; set; }

        public string CurrentTenantName { get; set; }

        public string CurrentTenantCode { get; set; }

        public bool IsHasMultipleTenant { get; set; }
    }
}
