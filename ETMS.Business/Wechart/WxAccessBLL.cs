using ETMS.Business.Common;
using ETMS.Entity.Common;
using ETMS.Entity.Config;
using ETMS.Entity.Dto.Wx.Output;
using ETMS.Entity.Dto.Wx.Request;
using ETMS.IBusiness.Wechart;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business.Wechart
{
    public class WxAccessBLL : IWxAccessBLL
    {
        private readonly IComponentAccessBLL _componentAccessBLL;

        private readonly IAppConfigurtaionServices _appConfigurtaionServices;


        public WxAccessBLL(IComponentAccessBLL componentAccessBLL, IAppConfigurtaionServices appConfigurtaionServices)
        {
            this._componentAccessBLL = componentAccessBLL;
            this._appConfigurtaionServices = appConfigurtaionServices;
        }

        public void InitTenantId(int tenantId)
        {
        }

        public async Task<ResponseBase> WxConfigBascGet(WxConfigBascGetRequest request)
        {
            var myTenantWechartAuth = await _componentAccessBLL.GetTenantWechartAuth(request.LoginTenantId);
            var tenantNo = TenantLib.GetTenantEncrypt(request.LoginTenantId);
            var weChatEntranceConfig = _appConfigurtaionServices.AppSettings.WxConfig.WeChatEntranceConfig;
            return ResponseBase.Success(new WxConfigBascGetOutput()
            {
                HeadImg = myTenantWechartAuth.HeadImg,
                NickName = myTenantWechartAuth.NickName,
                PrincipalName = myTenantWechartAuth.PrincipalName,
                QrcodeUrl = myTenantWechartAuth.QrcodeUrl,
                ServiceTypeInfo = myTenantWechartAuth.ServiceTypeInfo,
                VerifyTypeInfo = myTenantWechartAuth.VerifyTypeInfo,
                ParentLoginUrl = string.Format(weChatEntranceConfig.ParentLogin, tenantNo),
                TeacherLoginUrl = string.Format(weChatEntranceConfig.TeacherLogin, tenantNo)
            });
        }
    }
}
