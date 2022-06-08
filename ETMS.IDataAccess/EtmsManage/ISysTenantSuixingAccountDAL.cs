using ETMS.Entity.Database.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.EtmsManage
{
    public interface ISysTenantSuixingAccountDAL
    {
        Task<SysTenantSuixingAccount> GetTenantSuixingAccount(long tenantId);

        Task<SysTenantSuixingAccount> GetTenantSuixingAccount(string mno);

        Task AddTenantSuixingAccount(SysTenantSuixingAccount entity);

        Task EditTenantSuixingAccount(SysTenantSuixingAccount entity);
    }
}
