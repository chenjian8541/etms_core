using ETMS.Entity.Common;
using ETMS.Entity.Dto.Product.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface ICostBLL : IBaseBLL
    {
        Task<ResponseBase> CostAdd(CostAddRequest request);

        Task<ResponseBase> CostEdit(CostEditRequest request);

        Task<ResponseBase> CostGet(CostGetRequest request);

        Task<ResponseBase> CostDel(CostDelRequest request);

        Task<ResponseBase> CostStatusChange(CostStatusChangeRequest request);

        Task<ResponseBase> CostGetPaging(CostGetPagingRequest request);
    }
}
