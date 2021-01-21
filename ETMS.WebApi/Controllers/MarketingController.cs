using ETMS.Entity.Common;
using ETMS.Entity.Dto.Marketing.Request;
using ETMS.IBusiness;
using ETMS.LOG;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ETMS.WebApi.Controllers
{
    [Route("api/marketing/[action]")]
    [ApiController]
    [Authorize]
    public class MarketingController : ControllerBase
    {
        private readonly IGiftBLL _giftBLL;

        private readonly ICouponsBLL _couponsBLL;

        public MarketingController(IGiftBLL giftBLL, ICouponsBLL couponsBLL)
        {
            this._giftBLL = giftBLL;
            this._couponsBLL = couponsBLL;
        }

        [ActionName("giftAdd")]
        [HttpPost]
        public async Task<ResponseBase> GiftAdd(GiftAddRequest request)
        {
            try
            {
                _giftBLL.InitTenantId(request.LoginTenantId);
                return await _giftBLL.GiftAdd(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [ActionName("giftEdit")]
        [HttpPost]
        public async Task<ResponseBase> GiftEdit(GiftEditRequest request)
        {
            try
            {
                _giftBLL.InitTenantId(request.LoginTenantId);
                return await _giftBLL.GiftEdit(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [ActionName("giftGet")]
        [HttpPost]
        public async Task<ResponseBase> GiftGet(GiftGetRequest request)
        {
            try
            {
                _giftBLL.InitTenantId(request.LoginTenantId);
                return await _giftBLL.GiftGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [ActionName("giftDel")]
        [HttpPost]
        public async Task<ResponseBase> GiftDel(GiftDelRequest request)
        {
            try
            {
                _giftBLL.InitTenantId(request.LoginTenantId);
                return await _giftBLL.GiftDel(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [ActionName("giftGetPaging")]
        [HttpPost]
        public async Task<ResponseBase> GiftGetPaging(GiftGetPagingRequest request)
        {
            try
            {
                _giftBLL.InitTenantId(request.LoginTenantId);
                return await _giftBLL.GiftGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [ActionName("getExchangeLogPaging")]
        [HttpPost]
        public async Task<ResponseBase> GetExchangeLogPaging(GetExchangeLogPagingRequest request)
        {
            try
            {
                _giftBLL.InitTenantId(request.LoginTenantId);
                return await _giftBLL.GetExchangeLogPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [ActionName("getExchangeLogDetailPaging")]
        [HttpPost]
        public async Task<ResponseBase> GetExchangeLogDetailPaging(GetExchangeLogDetailPagingRequest request)
        {
            try
            {
                _giftBLL.InitTenantId(request.LoginTenantId);
                return await _giftBLL.GetExchangeLogDetailPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [ActionName("getExchangeLogDetail")]
        [HttpPost]
        public async Task<ResponseBase> GetExchangeLogDetail(GetExchangeLogDetailRequest request)
        {
            try
            {
                _giftBLL.InitTenantId(request.LoginTenantId);
                return await _giftBLL.GetExchangeLogDetail(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ExchangeLogHandle(ExchangeLogHandleRequest request)
        {
            try
            {
                _giftBLL.InitTenantId(request.LoginTenantId);
                return await _giftBLL.ExchangeLogHandle(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [HttpPost]
        public async Task<ResponseBase> CouponsAdd(CouponsAddRequest request)
        {
            try
            {
                _couponsBLL.InitTenantId(request.LoginTenantId);
                return await _couponsBLL.CouponsAdd(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [HttpPost]
        public async Task<ResponseBase> CouponsStatusChange(CouponsStatusChangeRequest request)
        {
            try
            {
                _couponsBLL.InitTenantId(request.LoginTenantId);
                return await _couponsBLL.CouponsStatusChange(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [HttpPost]
        public async Task<ResponseBase> CouponsDel(CouponsDelRequest request)
        {
            try
            {
                _couponsBLL.InitTenantId(request.LoginTenantId);
                return await _couponsBLL.CouponsDel(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [HttpPost]
        public async Task<ResponseBase> CouponsGetPaging(CouponsGetPagingRequest request)
        {
            try
            {
                _couponsBLL.InitTenantId(request.LoginTenantId);
                return await _couponsBLL.CouponsGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [HttpPost]
        public async Task<ResponseBase> CouponsStudentGetPaging(CouponsStudentGetPagingRequest request)
        {
            try
            {
                _couponsBLL.InitTenantId(request.LoginTenantId);
                return await _couponsBLL.CouponsStudentGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> CouponsStudentUsePaging(CouponsStudentUsrPagingRequest request)
        {
            try
            {
                _couponsBLL.InitTenantId(request.LoginTenantId);
                return await _couponsBLL.CouponsStudentUsePaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [HttpPost]
        public async Task<ResponseBase> CouponsStudentWriteOff(CouponsStudentWriteOffRequest request)
        {
            try
            {
                _couponsBLL.InitTenantId(request.LoginTenantId);
                return await _couponsBLL.CouponsStudentWriteOff(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> CouponsStudentGetCanUse(CouponsStudentGetCanUseRequest request)
        {
            try
            {
                _couponsBLL.InitTenantId(request.LoginTenantId);
                return await _couponsBLL.CouponsStudentGetCanUse(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> GiftExchangeReception(GiftExchangeReceptionRequest request)
        {
            try
            {
                _giftBLL.InitTenantId(request.LoginTenantId);
                return await _giftBLL.GiftExchangeReception(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> CouponsStudentSend(CouponsStudentSendRequest request)
        {
            try
            {
                _couponsBLL.InitTenantId(request.LoginTenantId);
                return await _couponsBLL.CouponsStudentSend(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentCouponsNormalGet(StudentCouponsNormalGet2Request request)
        {
            try
            {
                _couponsBLL.InitTenantId(request.LoginTenantId);
                return await _couponsBLL.StudentCouponsNormalGet2(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentCouponsUsedGet(StudentCouponsUsedGet2Request request)
        {
            try
            {
                _couponsBLL.InitTenantId(request.LoginTenantId);
                return await _couponsBLL.StudentCouponsUsedGet2(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentCouponsExpiredGet(StudentCouponsExpiredGet2Request request)
        {
            try
            {
                _couponsBLL.InitTenantId(request.LoginTenantId);
                return await _couponsBLL.StudentCouponsExpiredGet2(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }
    }
}
