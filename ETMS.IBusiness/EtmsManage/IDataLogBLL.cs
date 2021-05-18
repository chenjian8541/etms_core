using ETMS.Entity.Common;
using ETMS.Entity.EtmsManage.Dto.DataLog.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness.EtmsManage
{
    public interface IDataLogBLL
    {
        Task<ResponseBase> SysSmsLogPaging(SysSmsLogPagingRequest request);

        Task<ResponseBase> SysTenantOperationLogPaging(SysTenantOperationLogPagingRequest request);

        Task<ResponseBase> SysTenantExDateLogPaging(SysTenantExDateLogPagingRequest request);
    }
}
