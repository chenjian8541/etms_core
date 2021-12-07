using ETMS.Entity.Database.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.EtmsManage
{
    public interface ITenantFubeiAccountDAL
    {
        Task<SysTenantFubeiAccount> GetTenantFubeiAccount(long tenantId);

        Task AddTenantFubeiAccount(SysTenantFubeiAccount entity);

        Task EditTenantFubeiAccount(SysTenantFubeiAccount entity);
    }
}
