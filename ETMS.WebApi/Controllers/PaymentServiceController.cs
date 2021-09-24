using ETMS.Entity.Common;
using ETMS.Entity.Dto.PaymentService.Output;
using ETMS.Entity.Dto.PaymentService.Request;
using ETMS.IBusiness;
using ETMS.LOG;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ETMS.WebApi.Controllers
{
    [Route("api/pay/[action]")]
    [ApiController]
    [Authorize]
    public class PaymentServiceController : ControllerBase
    {
        private readonly IPaymentMerchantBLL _paymentMerchantBLL;

        public PaymentServiceController(IPaymentMerchantBLL paymentMerchantBLL)
        {
            this._paymentMerchantBLL = paymentMerchantBLL;
        }

        [AllowAnonymous]
        public ResponseBase MerchantCheckName(MerchantCheckNameRequest request)
        {
            try
            {
                return _paymentMerchantBLL.MerchantCheckName(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [AllowAnonymous]
        public async Task<ResponseBase> MerchantAdd(MerchantAddRequest request)
        {
            try
            {
                return await _paymentMerchantBLL.MerchantAdd(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [AllowAnonymous]
        public async Task<ResponseBase> MerchantEdit(MerchantEditRequest request)
        {
            try
            {
                return await _paymentMerchantBLL.MerchantEdit(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> MerchantQueryPC(MerchantQueryPCRequest request)
        {
            try
            {
                return await _paymentMerchantBLL.MerchantQueryPC(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [AllowAnonymous]
        public async Task<ResponseBase> MerchantQueryH5(MerchantQueryH5Request request)
        {
            try
            {
                return await _paymentMerchantBLL.MerchantQueryH5(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [AllowAnonymous]
        public async Task<MerchantAuditCallbackOutput> MerchantAuditCallback(MerchantAuditCallbackRequest request)
        {
            try
            {
                return await _paymentMerchantBLL.MerchantAuditCallback(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return new MerchantAuditCallbackOutput()
                {
                    return_code = "02",
                    trace_no = request.trace_no,
                    return_msg = "处理时发生异常"
                };
            }
        }
    }
}
