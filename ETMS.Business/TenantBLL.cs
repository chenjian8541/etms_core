using ETMS.Entity.Common;
using ETMS.Entity.Dto.BasicData.Request;
using ETMS.IBusiness;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business
{
    public class TenantBLL : ITenantBLL
    {
        public void InitTenantId(int tenantId)
        {
        }

        public async Task<ResponseBase> TenantGet(TenantGetRequest request)
        {
            return null;
        }
    }
}
