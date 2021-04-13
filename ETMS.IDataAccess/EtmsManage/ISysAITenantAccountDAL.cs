using ETMS.Entity.Database.Manage;
using ETMS.Entity.EtmsManage.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.EtmsManage
{
    public interface ISysAITenantAccountDAL
    {
        Task<SysAITenantAccount> GetSysAITenantAccount(int id);

        Task<int> AddSysAITenantAccount(SysAITenantAccount entity);

        Task<bool> EditSysAITenantAccount(SysAITenantAccount entity);

        Task<List<SysAITenantAccount>> GetSysAITenantAccountSystem();

        Task<List<SysAITenantAccount>> GetSysAITenantAccount();

        Task<bool> ExistAITenantAccount(string secretId, int id = 0);

        Task<bool> IsCanNotDel(int id);

        Task<Tuple<IEnumerable<SysAITenantAccount>, int>> GetPaging(AgentPagingBase request);
    }
}
