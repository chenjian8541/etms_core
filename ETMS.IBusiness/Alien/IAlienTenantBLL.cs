using ETMS.Entity.Alien.Dto.Tenant.Request;
using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness.Alien
{
    public interface IAlienTenantBLL : IAlienBaseBLL
    {
        Task<ResponseBase> TenantOperationLogPaging(TenantOperationLogPagingRequest request);
    }
}
