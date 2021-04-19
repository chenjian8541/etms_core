using ETMS.Entity.Common;
using ETMS.Entity.Config;
using ETMS.Entity.Dto.Open.Request;
using ETMS.Entity.Dto.Wx.Request;
using ETMS.IBusiness.SysOp;
using ETMS.IBusiness.Wechart;
using ETMS.LOG;
using ETMS.WebApi.Controllers.Open;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Senparc.Weixin.Exceptions;
using Senparc.Weixin.Open.ComponentAPIs;
using Senparc.Weixin.Open.Containers;
using System;
using System.Threading.Tasks;

namespace ETMS.WebApi.Controllers
{
    [Route("api/openOAuth/[action]")]
    [ApiController]
    [Authorize]
    public class OpenOAuthController : ControllerBase
    {
        private readonly IAppConfigurtaionServices _appConfigurtaionServices;

        private readonly IComponentAccessBLL _componentAccessBLL;

        private readonly IWxAccessBLL _wxAccessBLL;

        private readonly ISysSafeSmsCodeCheckBLL _sysSafeSmsCodeCheckBLL;

        public OpenOAuthController(IAppConfigurtaionServices appConfigurtaionServices, IComponentAccessBLL componentAccessBLL,
            IWxAccessBLL wxAccessBLL, ISysSafeSmsCodeCheckBLL sysSafeSmsCodeCheckBLL)
        {
            this._appConfigurtaionServices = appConfigurtaionServices;
            this._componentAccessBLL = componentAccessBLL;
            this._wxAccessBLL = wxAccessBLL;
            this._sysSafeSmsCodeCheckBLL = sysSafeSmsCodeCheckBLL;
        }

        /// <summary>
        /// 获取授权url
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ResponseBase ComponentOAuthGet(ComponentOAuthGetRequest request)
        {
            try
            {
                var chekSmsResult = _sysSafeSmsCodeCheckBLL.SysSafeSmsCodeCheck(request.LoginTenantId, request.SmsCode);
                if (!chekSmsResult.IsResponseSuccess())
                {
                    return chekSmsResult;
                }
                var appSettings = this._appConfigurtaionServices.AppSettings;
                var preAuthCode = ComponentContainer.TryGetPreAuthCode(appSettings.SenparcConfig.SenparcWeixinSetting.ComponentConfig.ComponentAppid,
                    appSettings.SenparcConfig.SenparcWeixinSetting.ComponentConfig.ComponentSecret, true);
                var callbackUrl = $"{appSettings.SenparcConfig.SenparcWeixinSetting.ComponentConfig.ComponentOAuthCallbackUrl}?tid={request.LoginTenantId}";
                var url = ComponentApi.GetComponentLoginPageUrl(appSettings.SenparcConfig.SenparcWeixinSetting.ComponentConfig.ComponentAppid,
                    preAuthCode, callbackUrl) + "&auth_Type=1";
                return ResponseBase.Success(url);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex, this.GetType());
                return new ResponseBase().GetResponseCodeError();
            }
        }

        /// <summary>
        /// 授权后
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<ResponseBase> OpenOAuthCallback(OpenOAuthCallbackRequest request)
        {
            try
            {
                Log.Debug($"[OpenOAuthCallback]{JsonConvert.SerializeObject(request)}", this.GetType());
                var appSettings = this._appConfigurtaionServices.AppSettings;
                var componentAppId = appSettings.SenparcConfig.SenparcWeixinSetting.ComponentConfig.ComponentAppid;
                var queryAuthResult = ComponentContainer.GetQueryAuthResult(componentAppId, request.AuthCode);
                var authorizerInfoResult = AuthorizerContainer.GetAuthorizerInfoResult(componentAppId, queryAuthResult.authorization_info.authorizer_appid, true);
                if (authorizerInfoResult != null && authorizerInfoResult.authorizer_info.MiniProgramInfo == null)
                {
                    return await AuthorizerInfoService.AddAuthorizerInfo(appSettings, queryAuthResult.authorization_info, authorizerInfoResult.authorizer_info, request.TenantId, _componentAccessBLL);
                }
                return ResponseBase.Success();
            }
            catch (ErrorJsonResultException errMsg)
            {
                Log.Error($"[OpenOAuthCallback]{JsonConvert.SerializeObject(request)}", errMsg, this.GetType());
                return ResponseBase.CommonError("公众号授权失败,只支持已认证的服务号");
            }
            catch (Exception ex)
            {
                Log.Error($"[OpenOAuthCallback]{JsonConvert.SerializeObject(request)}", ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        /// <summary>
        /// 获取机构微信公众号配置
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ResponseBase> WxConfigBascGet(WxConfigBascGetRequest request)
        {
            try
            {
                _wxAccessBLL.InitTenantId(request.LoginTenantId);
                return await _wxAccessBLL.WxConfigBascGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }
    }
}
