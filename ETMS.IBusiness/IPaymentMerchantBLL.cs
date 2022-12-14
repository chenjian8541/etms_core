using ETMS.Entity.Common;
using ETMS.Entity.Dto.PaymentService.Output;
using ETMS.Entity.Dto.PaymentService.Request;
using System;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IPaymentMerchantBLL
    {
        Task<ResponseBase> TenantPaymentSetGet(RequestBase request);

        Task<ResponseBase> TenantFubeiAccountBind(TenantFubeiAccountBindRequest request);

        Task<ResponseBase> TenantFubeiAccountGet(RequestBase request);

        ResponseBase MerchantCheckName(MerchantCheckNameRequest request);

        Task<ResponseBase> MerchantSave(MerchantAddRequest request);

        Task<ResponseBase> MerchantLcsAccountBind(MerchantLcsAccountBindRequest request);

        [Obsolete("使用MerchantSave方法")]
        Task<ResponseBase> MerchantAdd(MerchantAddRequest request);

        [Obsolete("使用MerchantSave方法")]
        Task<ResponseBase> MerchantEdit(MerchantAddRequest request);

        Task<ResponseBase> MerchantQueryPC(MerchantQueryPCRequest request);

        Task<ResponseBase> MerchantQueryH5(MerchantQueryH5Request request);

        Task<MerchantAuditCallbackOutput> MerchantAuditCallback(MerchantAuditCallbackRequest request);

        Task<ResponseBase> TenantSuixingAccountGet(RequestBase request);

        Task<ResponseBase> TenantSuixingAccountBind(TenantSuixingAccountBindRequest request);

        Task<ResponseBase> TenantSuixingAccountGet2(RequestBase request);

        Task<ResponseBase> TenantSuixingAccountBind2(TenantSuixingAccountBind2Request request);
    }
}
