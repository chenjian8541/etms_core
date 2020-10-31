using ETMS.Entity.Config;
using ETMS.IOC;
using Senparc.Weixin.Open.ComponentAPIs;
using Senparc.Weixin.Open.Containers;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Business.WxCore
{
    public class AuthorizationInfoService
    {
        /// <summary>
        /// component_AppId
        /// </summary>
        private static string ComponentAppId;

        /// <summary>
        /// component_Secret
        /// </summary>
        private static string ComponentSecret { get; set; }

        public static AuthorizationInfo GetAuthorizerAccessToken(string authorizerId, bool getNewTicket = false)
        {
            string componentAccessToken = null;
            if (getNewTicket)
            {
                componentAccessToken = ComponentContainer.GetComponentAccessToken(ComponentAppId, null, getNewTicket);
            }
            else
            {
                componentAccessToken = ComponentContainer.GetComponentAccessToken(ComponentAppId);
            }
            var authorizationInfo = AuthorizerContainer.GetAuthorizationInfo(ComponentAppId, authorizerId, getNewTicket);
            if (authorizationInfo == null)
            {
                throw new Exception("获取刷新令牌失败");
            }
            return authorizationInfo;
        }

        public AuthorizationInfo GetAuthorizerToken(string authorizerId)
        {
            return GetAuthorizerAccessToken(authorizerId);
        }

        static AuthorizationInfoService()
        {
            var appConfigurtaionServices = CustomServiceLocator.GetInstance<IAppConfigurtaionServices>();
            ComponentAppId = appConfigurtaionServices.AppSettings.SenparcConfig.SenparcWeixinSetting.ComponentConfig.ComponentAppid;
            ComponentSecret = appConfigurtaionServices.AppSettings.SenparcConfig.SenparcWeixinSetting.ComponentConfig.ComponentSecret;
        }
    }
}
