using ETMS.Entity.Common;
using ETMS.Entity.EtmsManage.Dto.TenantManage.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness.EtmsManage
{
    public interface ISysTenantManage
    {
        Task<ResponseBase> TenantAdd(TenantAddRequest request);
    }
}
