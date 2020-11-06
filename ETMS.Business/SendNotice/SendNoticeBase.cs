using ETMS.Business.WxCore;
using ETMS.Entity.ExternalService.Dto.Request;
using ETMS.IBusiness.Wechart;
using ETMS.IDataAccess;
using ETMS.IDataAccess.EtmsManage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business
{
    public abstract class SendNoticeBase
    {
        protected readonly IStudentWechatDAL _studentWechatDAL;

        protected readonly IComponentAccessBLL _componentAccessBLL;

        private readonly ISysTenantDAL _sysTenantDAL;

        public SendNoticeBase(IStudentWechatDAL studentWechatDAL, IComponentAccessBLL componentAccessBLL, ISysTenantDAL sysTenantDAL)
        {
            this._studentWechatDAL = studentWechatDAL;
            this._componentAccessBLL = componentAccessBLL;
            this._sysTenantDAL = sysTenantDAL;
        }

        protected async Task<string> GetStudentOpenId(bool isSendWeChat, string phone)
        {
            if (!isSendWeChat)
            {
                return string.Empty;
            }
            var wx = await _studentWechatDAL.GetStudentWechatByPhone(phone);
            return wx?.WechatOpenid;
        }

        protected async Task<NoticeRequestBase> GetNoticeRequestBase(int tenantId)
        {
            var tenant = await _sysTenantDAL.GetTenant(tenantId);
            var tenantWechartAuth = await _componentAccessBLL.GetTenantWechartAuth(tenantId);
            var wxAccessToken = AuthorizationInfoService.GetWXAccessToken(tenantWechartAuth.AuthorizerAppid);
            return new NoticeRequestBase(tenantId,
                tenantWechartAuth.Id, tenant.Name,
                tenant.SmsSignature,
               WeChatLimit.IsSendTemplateMessage(tenantId, tenantWechartAuth.ServiceTypeInfo, tenantWechartAuth.VerifyTypeInfo),
               wxAccessToken);
        }
    }
}
