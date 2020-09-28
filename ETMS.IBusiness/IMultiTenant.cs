using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.IBusiness
{
    public interface IMultiTenant
    {
        void ResetTenantId(int tenantId);
    }
}
