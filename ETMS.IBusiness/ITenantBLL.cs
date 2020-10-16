using ETMS.Entity.Common;
using ETMS.Entity.Dto.BasicData.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface ITenantBLL : IBaseBLL
    {
        Task<ResponseBase> TenantGet(TenantGetRequest request);
    }
}
