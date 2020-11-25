using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.HisData.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IIncomeLogBLL : IBaseBLL
    {
        Task<ResponseBase> IncomeLogGetPaging(IncomeLogGetPagingPagingRequest request);

        Task<ResponseBase> IncomeLogAdd(IncomeLogAddRequest request);

        Task<ResponseBase> IncomeLogRevoke(IncomeLogRevokeRequest request);
    }
}
