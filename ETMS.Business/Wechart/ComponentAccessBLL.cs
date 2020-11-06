using ETMS.Entity.Database.Manage;
using ETMS.Entity.Enum;
using ETMS.IBusiness.EtmsManage;
using ETMS.IBusiness.Wechart;
using ETMS.IDataAccess.Wechart;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business.Wechart
{
    public class ComponentAccessBLL : IComponentAccessBLL
    {
        private readonly ISysAppsettingsBLL _sysAppsettingsBLL;

        private readonly ISysTenantWechartAuthDAL _sysTenantWechartAuthDAL;

        private readonly ISysWechartAuthorizerTokenDAL _sysWechartAuthorizerTokenDAL;

        private readonly ISysWechartVerifyTicketDAL _sysWechartVerifyTicketDAL;

        public ComponentAccessBLL(ISysAppsettingsBLL sysAppsettingsBLL, ISysTenantWechartAuthDAL sysTenantWechartAuthDAL,
           ISysWechartAuthorizerTokenDAL sysWechartAuthorizerTokenDAL, ISysWechartVerifyTicketDAL sysWechartVerifyTicketDAL)
        {
            this._sysAppsettingsBLL = sysAppsettingsBLL;
            this._sysTenantWechartAuthDAL = sysTenantWechartAuthDAL;
            this._sysWechartAuthorizerTokenDAL = sysWechartAuthorizerTokenDAL;
            this._sysWechartVerifyTicketDAL = sysWechartVerifyTicketDAL;
        }

        public async Task<SysTenantWechartAuth> GetTenantWechartAuthSelf(int tenantId)
        {
            return await _sysTenantWechartAuthDAL.GetSysTenantWechartAuth(tenantId);
        }

        public async Task<SysTenantWechartAuth> GetTenantWechartAuth(int tenantId)
        {
            if (tenantId == 0)
            {
                return await _sysAppsettingsBLL.GetWechartAuthDefault();
            }
            var myWechartAuth = await _sysTenantWechartAuthDAL.GetSysTenantWechartAuth(tenantId);
            if (myWechartAuth == null || myWechartAuth.AuthorizeState == EmSysTenantWechartAuthAuthorizeState.Unauthorized)
            {
                LOG.Log.Warn($"[GetTenantWechartAuth]机构{tenantId}未获取到可用的授权信息，将使用默认授权", this.GetType());
                return await _sysAppsettingsBLL.GetWechartAuthDefault();
            }
            return myWechartAuth;
        }

        public async Task<bool> SaveSysTenantWechartAuth(SysTenantWechartAuth entity)
        {
            LOG.Log.Debug($"[SaveSysTenantWechartAuth]保存机构授权信息:{JsonConvert.SerializeObject(entity)}", this.GetType());
            await _sysTenantWechartAuthDAL.SaveSysTenantWechartAuth(entity);
            return true;
        }

        public async Task<string> GetSysWechartVerifyTicket(string componentAppId)
        {
            var logTicket = await _sysWechartVerifyTicketDAL.GetSysWechartVerifyTicket(componentAppId);
            if (logTicket == null)
            {
                return string.Empty;
            }
            return logTicket.ComponentVerifyTicket;
        }

        public async Task<bool> SaveSysWechartVerifyTicket(string componentAppId, string componentVerifyTicket)
        {
            LOG.Log.Debug($"[SaveSysWechartVerifyTicket]保存component_verify_ticket,componentAppId:{componentAppId},componentVerifyTicket:{componentVerifyTicket}", this.GetType());
            await _sysWechartVerifyTicketDAL.SaveSysWechartVerifyTicket(componentAppId, componentVerifyTicket);
            return true;
        }

        public async Task<SysWechartAuthorizerToken> GetSysWechartAuthorizerToken(string authorizerAppid)
        {
            return await _sysWechartAuthorizerTokenDAL.GetSysWechartAuthorizerToken(authorizerAppid);
        }

        public async Task<bool> SaveSysWechartAuthorizerToken(SysWechartAuthorizerToken entity)
        {
            LOG.Log.Debug($"[SaveSysWechartAuthorizerToken]保存AuthorizerToken:{JsonConvert.SerializeObject(entity)}", this.GetType());
            await _sysWechartAuthorizerTokenDAL.SaveSysWechartAuthorizerToken(entity);
            return true;
        }

        public async Task<bool> OnUnauthorizeTenantWechart(string authorizerAppid)
        {
            LOG.Log.Debug($"[OnUnauthorizeTenantWechart]取消授权authorizerAppid:{authorizerAppid}", this.GetType());
            await _sysTenantWechartAuthDAL.OnUnauthorizeTenantWechart(authorizerAppid);
            return true;
        }
    }
}
