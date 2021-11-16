using ETMS.Entity.Common;
using ETMS.Entity.Dto.PaymentService.Output;
using ETMS.Entity.Dto.PaymentService.Request;
using ETMS.IBusiness;
using ETMS.LOG;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.WebApi.Controllers
{
    [Route("api/pay/[action]")]
    [ApiController]
    [Authorize]
    public class PaymentServiceController : ControllerBase
    {
        private readonly IPaymentMerchantBLL _paymentMerchantBLL;

        private readonly IPaymentBLL _paymentBLL;

        private readonly IPaymentStatisticsBLL _paymentStatisticsBLL;

        public PaymentServiceController(IPaymentMerchantBLL paymentMerchantBLL, IPaymentBLL paymentBLL, IPaymentStatisticsBLL paymentStatisticsBLL)
        {
            this._paymentMerchantBLL = paymentMerchantBLL;
            this._paymentBLL = paymentBLL;
            this._paymentStatisticsBLL = paymentStatisticsBLL;
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
        public async Task<ResponseBase> MerchantSave(MerchantAddRequest request)
        {
            try
            {
                return await _paymentMerchantBLL.MerchantSave(request);
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
        public async Task<ResponseBase> MerchantEdit(MerchantAddRequest request)
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

        [AllowAnonymous]
        public async Task<LcsPayJspayCallbackOutput> LcsPayJspayCallback()
        {
            try
            {
                using (var stream = Request.Body)
                {
                    var buffer = new byte[Request.ContentLength.Value];
                    await stream.ReadAsync(buffer, 0, buffer.Length);
                    var postData = Encoding.UTF8.GetString(buffer);
                    LOG.Log.Info("[LcsPayJspayCallback]利楚扫呗支付回调", postData, this.GetType());
                    var request = Newtonsoft.Json.JsonConvert.DeserializeObject<LcsPayJspayCallbackRequest>(postData);
                    return await _paymentBLL.LcsPayJspayCallback(request);
                }
            }
            catch (Exception ex)
            {
                Log.Error("利楚扫呗支付回调发生异常", ex, this.GetType());
                return new LcsPayJspayCallbackOutput()
                {
                    return_code = "02",
                    return_msg = "失败"
                };
            }
        }

        public async Task<ResponseBase> TenantLcsPayLogPaging(TenantLcsPayLogPagingRequest request)
        {
            try
            {
                _paymentBLL.InitTenantId(request.LoginTenantId);
                return await _paymentBLL.TenantLcsPayLogPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> BarCodePay(BarCodePayRequest request)
        {
            try
            {
                _paymentBLL.InitTenantId(request.LoginTenantId);
                return await _paymentBLL.BarCodePay(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> LcsPayQuery(LcsPayQueryRequest request)
        {
            try
            {
                _paymentBLL.InitTenantId(request.LoginTenantId);
                return await _paymentBLL.LcsPayQuery(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> LcsPayRefund(LcsPayRefundRequest request)
        {
            try
            {
                _paymentBLL.InitTenantId(request.LoginTenantId);
                return await _paymentBLL.LcsPayRefund(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StatisticsLcsPayDayGetLine(StatisticsLcsPayDayGetLineRequest request)
        {
            try
            {
                _paymentStatisticsBLL.InitTenantId(request.LoginTenantId);
                return await _paymentStatisticsBLL.StatisticsLcsPayDayGetLine(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StatisticsLcsPayDayGetPaging(StatisticsLcsPayDayGetPagingRequest request)
        {
            try
            {
                _paymentStatisticsBLL.InitTenantId(request.LoginTenantId);
                return await _paymentStatisticsBLL.StatisticsLcsPayDayGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StatisticsLcsPayMonthGetLine(StatisticsLcsPayMonthGetLineRequest request)
        {
            try
            {
                _paymentStatisticsBLL.InitTenantId(request.LoginTenantId);
                return await _paymentStatisticsBLL.StatisticsLcsPayMonthGetLine(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StatisticsLcsPayMonthGetPaging(StatisticsLcsPayMonthGetPagingRequest request)
        {
            try
            {
                _paymentStatisticsBLL.InitTenantId(request.LoginTenantId);
                return await _paymentStatisticsBLL.StatisticsLcsPayMonthGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StatisticsLcsPayYear(StatisticsLcsPayYearRequest request)
        {
            try
            {
                _paymentStatisticsBLL.InitTenantId(request.LoginTenantId);
                return await _paymentStatisticsBLL.StatisticsLcsPayYear(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }
    }
}
