using ETMS.Entity.Database.Manage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.Wechart
{
    public interface ISysTenantWechartAuthDAL
    {
        Task<SysTenantWechartAuth> GetSysTenantWechartAuth(int tenantId);

        Task<bool> SaveSysTenantWechartAuth(SysTenantWechartAuth entity);

        Task<bool> OnUnauthorizeTenantWechart(string authorizerAppid);
    }
}
