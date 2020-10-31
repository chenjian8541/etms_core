using ETMS.Entity.Database.Manage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness.Wechart
{
    public interface IComponentAccessBLL
    {
        Task<SysTenantWechartAuth> GetTenantWechartAuthSelf(int tenantId);

        Task<SysTenantWechartAuth> GetTenantWechartAuth(int tenantId);

        Task<bool> SaveSysTenantWechartAuth(SysTenantWechartAuth entity);

        Task<string> GetSysWechartVerifyTicket(string componentAppId);

        Task<bool> SaveSysWechartVerifyTicket(string componentAppId, string componentVerifyTicket);

        Task<SysWechartAuthorizerToken> GetSysWechartAuthorizerToken(string authorizerAppid);

        Task<bool> SaveSysWechartAuthorizerToken(SysWechartAuthorizerToken entity);

        Task<bool> OnUnauthorizeTenantWechart(string authorizerAppid);
    }
}
