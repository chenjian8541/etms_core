using ETMS.Entity.Database.Manage;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.EtmsManage
{
    public interface ISysTenantDAL
    {
        Task<SysTenant> GetTenant(string tenantCode);

        Task<SysTenant> GetTenant(int id);

        Task<List<SysTenant>> GetTenants();

        Task<int> AddTenant(SysTenant sysTenant);
    }
}
