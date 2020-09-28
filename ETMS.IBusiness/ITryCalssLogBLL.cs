using ETMS.Entity.Common;
using ETMS.Entity.Dto.Educational.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface ITryCalssLogBLL : IBaseBLL
    {
        Task<ResponseBase> TryCalssLogGetPaging(TryCalssLogGetPagingRequest request);
    }
}
