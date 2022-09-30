using ETMS.Entity.Database.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.EtmsManage
{
    public interface ISysTenantSuixingAccount2DAL
    {
        Task<SysTenantSuixingAccount2> GetTenantSuixingAccount(long tenantId);

        Task<SysTenantSuixingAccount2> GetTenantSuixingAccount(string mno);

        Task AddTenantSuixingAccount(SysTenantSuixingAccount2 entity);

        Task EditTenantSuixingAccount(SysTenantSuixingAccount2 entity);
    }
}
