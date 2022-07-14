using ETMS.Entity.Common;
using ETMS.Entity.Config;
using ETMS.Entity.Dto.Open2.Request;
using ETMS.Entity.Dto.OpenParent.Request;
using ETMS.Entity.Dto.OpenParent2.Request;
using ETMS.IBusiness;
using ETMS.IBusiness.Open;
using ETMS.IBusiness.Wechart;
using ETMS.LOG;
using ETMS.Utility;
using ETMS.WebApi.Controllers.Open;
using ETMS.WebApi.FilterAttribute;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Senparc.CO2NET.AspNet.HttpUtility;
using Senparc.Weixin.MP.MessageHandlers;
using Senparc.Weixin.Open.Containers;
using Senparc.Weixin.Open.Entities.Request;
using System;
using System.Threading.Tasks;

namespace ETMS.WebApi.Controllers
{
    [Route("api/openParent2/[action]")]
    [ApiController]
    [EtmsSignatureWxminiAuthorize]
    public class OpenParent2Controller : ControllerBase
    {
        private readonly IOpenParent2BLL _openParentBLL;

        public OpenParent2Controller(IOpenParent2BLL openParentBLL)
        {
            this._openParentBLL = openParentBLL;
        }

        [AllowAnonymous]
        public async Task<ResponseBase> WxMiniLogin(WxMiniLoginRequest request)
        {
            try
            {
                return await _openParentBLL.WxMiniLogin(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> WxMiniEditUserInfo(WxMiniEditUserInfoRequest request)
        {
            try
            {
                return await _openParentBLL.WxMiniEditUserInfo(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> DecodedPhoneNumber(DecodedPhoneNumberRequest request)
        {
            try
            {
                return await _openParentBLL.DecodedPhoneNumber(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> WxMiniActivityRouteItemGetPaging(WxMiniActivityRouteItemGetPagingRequest request)
        {
            try
            {
                return await _openParentBLL.WxMiniActivityRouteItemGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> WxMiniActivityHomeGet(WxMiniActivityHomeGetRequest request)
        {
            try
            {
                return await _openParentBLL.WxMiniActivityHomeGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> WxMiniActivityHomeGet2(WxMiniActivityHomeGet2Request request)
        {
            try
            {
                return await _openParentBLL.WxMiniActivityHomeGet2(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> WxMiniActivityRouteItemMoreGetPaging(WxMiniActivityRouteItemMoreGetPagingRequest request)
        {
            try
            {
                return await _openParentBLL.WxMiniActivityRouteItemMoreGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> WxMiniActivityGroupPurchaseDiscount(WxMiniActivityGroupPurchaseDiscountRequest request)
        {
            try
            {
                return await _openParentBLL.WxMiniActivityGroupPurchaseDiscount(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> WxMiniActivityGetSimple(WxMiniActivityGetSimpleRequest request)
        {
            try
            {
                return await _openParentBLL.WxMiniActivityGetSimple(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> WxMiniActivityDynamicBulletGetPaging(WxMiniActivityDynamicBulletGetPagingRequest request)
        {
            try
            {
                return await _openParentBLL.WxMiniActivityDynamicBulletGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> WxMiniGroupPurchaseStartCheck(WxMiniGroupPurchaseStartCheckRequest request)
        {
            try
            {
                return await _openParentBLL.WxMiniGroupPurchaseStartCheck(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> WxMiniGroupPurchaseStartGo(WxMiniGroupPurchaseStartGoRequest request)
        {
            try
            {
                return await _openParentBLL.WxMiniGroupPurchaseStartGo(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> WxMiniGroupPurchaseJoinCheck(WxMiniGroupPurchaseJoinCheckRequest request)
        {
            try
            {
                return await _openParentBLL.WxMiniGroupPurchaseJoinCheck(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> WxMiniGroupPurchaseJoin(WxMiniGroupPurchaseJoinRequest request)
        {
            try
            {
                return await _openParentBLL.WxMiniGroupPurchaseJoin(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> WxMiniHagglingStartCheck(WxMiniHagglingStartCheckRequest request)
        {
            try
            {
                return await _openParentBLL.WxMiniHagglingStartCheck(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> WxMiniHagglingStartGo(WxMiniHagglingStartGoRequest request)
        {
            try
            {
                return await _openParentBLL.WxMiniHagglingStartGo(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> WxMiniHagglingAssistGo(WxMiniHagglingAssistGoRequest request)
        {
            try
            {
                return await _openParentBLL.WxMiniHagglingAssistGo(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public ResponseBase WxMiniActivityCall(WxMiniActivityCallRequest request)
        {
            try
            {
                return _openParentBLL.WxMiniActivityCall(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public ResponseBase WxMiniActivityShare(WxMiniActivityShareRequest request)
        {
            try
            {
                return _openParentBLL.WxMiniActivityShare(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public ResponseBase WxMiniPaySuccess(WxMiniPaySuccessRequest request)
        {
            try
            {
                return _openParentBLL.WxMiniPaySuccess(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> WxMiniHagglingLogGetPaging(WxMiniHagglingLogGetPagingRequest request)
        {
            try
            {
                return await _openParentBLL.WxMiniHagglingLogGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }
    }
}
