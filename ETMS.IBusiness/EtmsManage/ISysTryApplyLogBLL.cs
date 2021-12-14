using ETMS.Entity.Common;
using ETMS.Entity.EtmsManage.Dto.DataLog.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness.EtmsManage
{
    public interface ISysTryApplyLogBLL
    {
        Task<ResponseBase> TryApplyLogGetPaging(TryApplyLogGetPagingRequest request);

        Task<ResponseBase> TryApplyLogHandle(TryApplyLogHandleRequest request);
    }
}
