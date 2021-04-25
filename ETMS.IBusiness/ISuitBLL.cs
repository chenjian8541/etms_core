using ETMS.Entity.Common;
using ETMS.Entity.Dto.Product.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface ISuitBLL : IBaseBLL
    {
        Task<ResponseBase> SuitAdd(SuitAddRequest request);

        Task<ResponseBase> SuitEdit(SuitEditRequest request);

        Task<ResponseBase> SuitGet(SuitGetRequest request);

        Task<ResponseBase> SuitDel(SuitDelRequest request);

        Task<ResponseBase> SuitChangeStatus(SuitChangeStatusRequest request);

        Task<ResponseBase> SuitGetPaging(SuitGetPagingRequest request);
    }
}
