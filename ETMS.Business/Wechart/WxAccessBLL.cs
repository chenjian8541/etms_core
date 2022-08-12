using ETMS.Business.Common;
using ETMS.Entity.Common;
using ETMS.Entity.Config;
using ETMS.Entity.Dto.Wx.Output;
using ETMS.Entity.Dto.Wx.Request;
using ETMS.IBusiness.Wechart;
using ETMS.IDataAccess;
using ETMS.IDataAccess.Wechart;
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

        private readonly ISysTenantWechartAuthDAL _sysTenantWechartAuthDAL;

        private readonly IUserOperationLogDAL _userOperationLogDAL;

        public WxAccessBLL(IComponentAccessBLL componentAccessBLL, IAppConfigurtaionServices appConfigurtaionServices, IUserOperationLogDAL userOperationLogDAL)
        {
            this._componentAccessBLL = componentAccessBLL;
            this._appConfigurtaionServices = appConfigurtaionServices;
            this._userOperationLogDAL = userOperationLogDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _userOperationLogDAL);
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

        public async Task<ResponseBase> WxUnbound(WxUnboundRequest request)
        {
            await _sysTenantWechartAuthDAL.DelSysTenantWechartAuth(request.LoginTenantId);
            await _userOperationLogDAL.AddUserLog(request, "解绑微信公众号", Entity.Enum.EmUserOperationType.SystemConfigModify);

            return ResponseBase.Success();
        }
    }
}
