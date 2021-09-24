using ETMS.Entity.Common;
using ETMS.Entity.Dto.PaymentService.Output;
using ETMS.Entity.Dto.PaymentService.Request;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IPaymentMerchantBLL
    {
        ResponseBase MerchantCheckName(MerchantCheckNameRequest request);

        Task<ResponseBase> MerchantAdd(MerchantAddRequest request);

        Task<ResponseBase> MerchantEdit(MerchantEditRequest request);

        Task<ResponseBase> MerchantQueryPC(MerchantQueryPCRequest request);

        Task<ResponseBase> MerchantQueryH5(MerchantQueryH5Request request);

        Task<MerchantAuditCallbackOutput> MerchantAuditCallback(MerchantAuditCallbackRequest request);
    }
}
