using ETMS.Entity.Common;
using ETMS.Entity.Dto.HisData.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IOrderBLL : IBaseBLL
    {
        Task<ResponseBase> OrderGetPaging(OrderGetPagingRequest request);

        Task<ResponseBase> OrderGetDetail(OrderGetDetailRequest request);

        Task<ResponseBase> OrderPayment(OrderPaymentRequest request);

        Task<ResponseBase> OrderGetSimpleDetail(OrderGetSimpleDetailRequest request);

        Task<ResponseBase> OrderEditRemark(OrderEditRemarkRequest request);

        Task<ResponseBase> OrderEditCommission(OrderEditCommissionRequest request);

        Task<ResponseBase> OrderOperationLogGetPaging(OrderOperationLogGetPagingRequest request);

        Task<ResponseBase> OrderRepeal(OrderRepealRequest request);
    }
}
