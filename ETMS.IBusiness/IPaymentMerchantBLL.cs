using ETMS.Entity.Common;
using ETMS.Entity.Dto.PaymentService.Output;
using ETMS.Entity.Dto.PaymentService.Request;
using System;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IPaymentMerchantBLL
    {
        ResponseBase MerchantCheckName(MerchantCheckNameRequest request);

        Task<ResponseBase> MerchantSave(MerchantAddRequest request);

        [Obsolete("使用MerchantSave方法")]
        Task<ResponseBase> MerchantAdd(MerchantAddRequest request);

        [Obsolete("使用MerchantSave方法")]
        Task<ResponseBase> MerchantEdit(MerchantAddRequest request);

        Task<ResponseBase> MerchantQueryPC(MerchantQueryPCRequest request);

        Task<ResponseBase> MerchantQueryH5(MerchantQueryH5Request request);

        Task<MerchantAuditCallbackOutput> MerchantAuditCallback(MerchantAuditCallbackRequest request);
    }
}
