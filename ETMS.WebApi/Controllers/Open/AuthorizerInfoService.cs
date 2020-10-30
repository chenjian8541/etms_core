using ETMS.Entity.Common;
using ETMS.Entity.Config;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Enum;
using ETMS.IBusiness.Wechart;
using Newtonsoft.Json;
using Senparc.Weixin.Open.ComponentAPIs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ETMS.WebApi.Controllers.Open
{
    public class AuthorizerInfoService
    {
        public static async Task<ResponseBase> AddAuthorizerInfo(AppSettings appSettings, AuthorizationInfo authorizationinfo, AuthorizerInfo authorizerinfo, int tenantId, IComponentAccessBLL componentAccessBLL)
        {
            var strParms = $"tenantId:{tenantId},authorizationinfo:{JsonConvert.SerializeObject(authorizationinfo)},authorizerinfo:{JsonConvert.SerializeObject(authorizerinfo)}";
            LOG.Log.Debug($"[AddAuthorizerInfo]添加授权信息,{strParms}", typeof(AuthorizerInfoService));
            if ((int)authorizerinfo.service_type_info.id != 2 && (int)authorizerinfo.verify_type_info.id == -1)
            {
                LOG.Log.Warn($"[AddAuthorizerInfo]未认证的订阅号无法授权,{strParms}", typeof(AuthorizerInfoService));
                return ResponseBase.CommonError("未认证的订阅号无法授权");
            }
            string code;
            string value;
            var codeList = new List<int>();
            var valueList = new List<string>();
            foreach (var item in authorizationinfo.func_info)
            {
                codeList.Add(item.funcscope_category != null ? (int)item.funcscope_category.id : 0);
                valueList.Add(item.funcscope_category != null ? item.funcscope_category.id.ToString() : "空（异常）");
            }
            code = string.Join(",", codeList.ToArray());
            value = string.Join(",", valueList.ToArray());
            var oldTenantWechartAuth = await componentAccessBLL.GetTenantWechartAuth(tenantId);
            if (oldTenantWechartAuth == null)
            {
                oldTenantWechartAuth = new SysTenantWechartAuth();
                oldTenantWechartAuth.CreateOt = DateTime.Now;
            }
            else
            {
                LOG.Log.Debug($"[AddAuthorizerInfo]记录旧的授权信息,{JsonConvert.SerializeObject(oldTenantWechartAuth)}", typeof(AuthorizerInfoService));
            }
            oldTenantWechartAuth.UserName = authorizerinfo.user_name;
            oldTenantWechartAuth.TenantId = tenantId;
            oldTenantWechartAuth.Alias = authorizerinfo.alias;
            oldTenantWechartAuth.HeadImg = authorizerinfo.head_img;
            oldTenantWechartAuth.NickName = authorizerinfo.nick_name;
            oldTenantWechartAuth.UserName = authorizerinfo.user_name;
            oldTenantWechartAuth.QrcodeUrl = authorizerinfo.qrcode_url;
            oldTenantWechartAuth.ServiceTypeInfo = authorizerinfo.service_type_info.id.ToString();
            oldTenantWechartAuth.VerifyTypeInfo = authorizerinfo.verify_type_info.id.ToString();
            oldTenantWechartAuth.AuthorizerAppid = authorizationinfo.authorizer_appid;
            oldTenantWechartAuth.PermissionsKey = code;
            oldTenantWechartAuth.PermissionsValue = value;
            oldTenantWechartAuth.AuthorizeState = EmSysTenantWechartAuthAuthorizeState.Authorized;
            oldTenantWechartAuth.ModifyOt = DateTime.Now;
            await componentAccessBLL.SaveSysTenantWechartAuth(oldTenantWechartAuth);
            return ResponseBase.Success();
        }
    }
}
