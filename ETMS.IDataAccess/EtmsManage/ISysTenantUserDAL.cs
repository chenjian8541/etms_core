using ETMS.Entity.Database.Manage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.EtmsManage
{
    public interface ISysTenantUserDAL
    {
        Task ResetTenantAllUser(int tenantId);

        Task AddTenantUser(int tenantId, long userId, string phone, bool isRefreshCache);

        Task RemoveTenantUser(int tenantId, string phone);

        Task UpdateTenantUserOpTime(int tenantId, string phone, DateTime opTime);

        Task<List<SysTenantUser>> GetTenantUser(string phone);
    }
}
