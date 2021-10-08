using ETMS.Entity.Common;
using ETMS.Entity.Dto.PaymentService.Request;
using ETMS.Entity.Dto.Product.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IPaymentBLL : IBaseBLL
    {
        Task<ResponseBase> TenantLcsPayLogPaging(TenantLcsPayLogPagingRequest request);

        Task<ResponseBase> BarCodePay(BarCodePayRequest request);

        Task<ResponseBase> LcsPayQuery(LcsPayQueryRequest request);

        Task<ResponseBase> LcsPayRefund(LcsPayRefundRequest request);
    }
}
