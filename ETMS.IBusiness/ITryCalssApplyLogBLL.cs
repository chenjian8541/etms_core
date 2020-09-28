using ETMS.Entity.Common;
using ETMS.Entity.Dto.Educational.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface ITryCalssApplyLogBLL: IBaseBLL
    {
        Task<ResponseBase> TryCalssApplyLogPaging(TryCalssApplyLogPagingRequest request);

        Task<ResponseBase> TryCalssApplyLogHandle(TryCalssApplyLogHandleRequest request);
    }
}
