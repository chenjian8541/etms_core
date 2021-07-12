using ETMS.Business.WxCore;
using ETMS.Entity.Database.Source;
using ETMS.Entity.ExternalService.Dto.Request;
using ETMS.IBusiness.EventConsumer;
using ETMS.IBusiness.Wechart;
using ETMS.IDataAccess;
using ETMS.IDataAccess.EtmsManage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business.SendNotice
{
    public abstract class SendNoticeBase
    {
        protected readonly IComponentAccessBLL _componentAccessBLL;

        protected readonly ISysTenantDAL _sysTenantDAL;

        protected readonly ITenantLibBLL _tenantLibBLL;

        public SendNoticeBase(IComponentAccessBLL componentAccessBLL, ISysTenantDAL sysTenantDAL, ITenantLibBLL tenantLibBLL)
        {
            this._componentAccessBLL = componentAccessBLL;
            this._sysTenantDAL = sysTenantDAL;
            this._tenantLibBLL = tenantLibBLL;
        }

        protected async Task<NoticeRequestBase> GetNoticeRequestBase(int tenantId, bool isWxNotice = true)
        {
            var tenant = await _sysTenantDAL.GetTenant(tenantId);
            if (!isWxNotice)
            {
                return new NoticeRequestBase(tenantId, 0, tenant.Name, tenant.SmsSignature, true, string.Empty, string.Empty);
            }
            var tenantWechartAuth = await _componentAccessBLL.GetTenantWechartAuth(tenantId);
            var wxAccessToken = AuthorizationInfoService.GetWXAccessToken(tenantWechartAuth.AuthorizerAppid);
            return new NoticeRequestBase(tenantId,
                tenantWechartAuth.Id, tenant.Name,
                tenant.SmsSignature,
               WeChatLimit.IsSendTemplateMessage(tenantId, tenantWechartAuth.ServiceTypeInfo, tenantWechartAuth.VerifyTypeInfo),
               wxAccessToken, tenantWechartAuth.AuthorizerAppid);
        }
    }
}
