using ETMS.Entity.Common;
using ETMS.Entity.Dto.Parent.Request;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ETMS.Utility;
using ETMS.Entity.Config;
using ETMS.ExternalService.Contract;
using ETMS.Business.Common;
using ETMS.Entity.Dto.Parent.Output;
using ETMS.IDataAccess.EtmsManage;
using ETMS.Entity.CacheBucket;
using ETMS.Entity.ExternalService.Dto.Request;
using WxApi;
using WxApi.UserManager;
using ETMS.LOG;
using WxApi.ReceiveEntity;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Enum;
using ETMS.IBusiness.Wechart;
using Senparc.Weixin.Open.OAuthAPIs;
using Senparc.Weixin.Open.Containers;
using Microsoft.AspNetCore.Http;
using ETMS.Entity.Dto.SysCom.Output;
using ETMS.Entity.Dto.Open2.Output;
using Senparc.Weixin.MP.Helpers;

namespace ETMS.Business.WxCore
{
    public abstract class WeChatAccessBLL
    {
        protected readonly IComponentAccessBLL _componentAccessBLL;

        protected readonly IAppConfigurtaionServices _appConfigurtaionServices;

        public WeChatAccessBLL(IComponentAccessBLL componentAccessBLL, IAppConfigurtaionServices appConfigurtaionServices)
        {
            this._componentAccessBLL = componentAccessBLL;
            this._appConfigurtaionServices = appConfigurtaionServices;
        }

        protected virtual async Task<ResponseBase> GetAuthorizeUrl(int tenantId, string sourceUrl)
        {
            var tenantWechartAuth = await _componentAccessBLL.GetTenantWechartAuth(tenantId);
            if (tenantWechartAuth == null)
            {
                Log.Error($"[GetAuthorizeUrl]未找到机构授权信息,tenantId:{tenantId}", this.GetType());
                return ResponseBase.CommonError("机构绑定的微信公众号无权限");
            }
            var componentAppid = _appConfigurtaionServices.AppSettings.SenparcConfig.SenparcWeixinSetting.ComponentConfig.ComponentAppid;
            var url = OAuthApi.GetAuthorizeUrl(tenantWechartAuth.AuthorizerAppid, componentAppid, sourceUrl, tenantId.ToString(),
                new[] { Senparc.Weixin.Open.OAuthScope.snsapi_userinfo, Senparc.Weixin.Open.OAuthScope.snsapi_base });
            Log.Info($"[GetAuthorizeUrl]{url}", this.GetType());
            return ResponseBase.Success(url);
        }

        /// <summary>
        /// snsapi_base 静默授权
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="sourceUrl"></param>
        /// <returns></returns>
        protected virtual async Task<ResponseBase> GetAuthorizeUrl2(int tenantId, string sourceUrl)
        {
            var tenantWechartAuth = await _componentAccessBLL.GetTenantWechartAuth(tenantId);
            if (tenantWechartAuth == null)
            {
                Log.Error($"[GetAuthorizeUrl2]未找到机构授权信息,tenantId:{tenantId}", this.GetType());
                return ResponseBase.CommonError("机构绑定的微信公众号无权限");
            }
            var componentAppid = _appConfigurtaionServices.AppSettings.SenparcConfig.SenparcWeixinSetting.ComponentConfig.ComponentAppid;
            var url = OAuthApi.GetAuthorizeUrl(tenantWechartAuth.AuthorizerAppid, componentAppid, sourceUrl, tenantId.ToString(),
                new[] { Senparc.Weixin.Open.OAuthScope.snsapi_base });
            Log.Info($"[GetAuthorizeUrl2]{url}", this.GetType());
            return ResponseBase.Success(url);
        }

        protected virtual OAuthAccessTokenResult GetAuthAccessToken(string authorizerAppid, string code)
        {
            var componentAppid = _appConfigurtaionServices.AppSettings.SenparcConfig.SenparcWeixinSetting.ComponentConfig.ComponentAppid;
            var componentAccessToken = ComponentContainer.GetComponentAccessToken(componentAppid);
            return OAuthApi.GetAccessToken(authorizerAppid, componentAppid, componentAccessToken, code);
        }

        protected virtual OAuthUserInfo GetUserInfo(string access_token, string openid)
        {
            return OAuthApi.GetUserInfo(access_token, openid);
        }

        protected virtual async Task<ResponseBase> GetJsSdkUiPackage(int tenantId, string url)
        {
            var output = new GetJsSdkUiPackageOutput()
            {
                IsSuccess = false
            };
            try
            {
                var tenantWechartAuth = await _componentAccessBLL.GetTenantWechartAuth(tenantId);
                if (tenantWechartAuth == null)
                {
                    Log.Fatal($"[GetJsSdkUiPackage]未找到机构授权信息,tenantId:{tenantId}", this.GetType());
                    return ResponseBase.Success(output);
                }
                var sysWechartAuthorizerToken = await _componentAccessBLL.GetSysWechartAuthorizerToken(tenantWechartAuth.AuthorizerAppid);
                if (sysWechartAuthorizerToken == null)
                {
                    Log.Fatal($"[GetJsSdkUiPackage]未获取到token信息,tenantId:{tenantId}", this.GetType());
                    return ResponseBase.Success(output);
                }
                var ticketResult = Senparc.Weixin.Open.ComponentAPIs.ComponentApi.GetJsApiTicket(sysWechartAuthorizerToken.AuthorizerAccessToken);
                var noncestr = JSSDKHelper.GetNoncestr();
                var timestamp = JSSDKHelper.GetTimestamp();
                var signature = JSSDKHelper.GetSignature(ticketResult.ticket, noncestr, timestamp, url);
                output.IsSuccess = true;
                output.MyData = new GetJsSdkUiPackageData()
                {
                    AppId = tenantWechartAuth.AuthorizerAppid,
                    NonceStr = noncestr,
                    Timestamp = timestamp,
                    Signature = signature
                };
                return ResponseBase.Success(output);
            }
            catch (Exception ex)
            {
                Log.Fatal($"[GetJsSdkUiPackage]{ex.Message},tenantId:{tenantId}", ex, this.GetType());
                return ResponseBase.Success(output);
            }
        }
    }
}
