using ETMS.Entity.Database.Manage;
using ETMS.Entity.EtmsManage.Common;
using ETMS.Entity.EtmsManage.View;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.EtmsManage
{
    public interface ISysTenantLogDAL
    {
        Task AddSysTenantEtmsAccountLog(SysTenantEtmsAccountLog entity, long userId);

        Task AddSysTenantSmsLog(SysTenantSmsLog entity, long userId);

        Task<SysTenantEtmsAccountLog> GetTenantEtmsAccountLog(long id);

        Task EditTenantEtmsAccountLog(SysTenantEtmsAccountLog entity);

        Task<Tuple<IEnumerable<SysTenantEtmsAccountLogView>, int>> GetTenantEtmsAccountLogPaging(AgentPagingBase request);

        Task<Tuple<IEnumerable<SysTenantSmsLogVew>, int>> GetTenantSmsLogPaging(AgentPagingBase request);

        Task<List<SysTenantEtmsAccountLog>> GetTenantEtmsAccountLogNormal(int tenantId, int agentId, int versionId);

        Task AddSysTenantExDateLog(SysTenantExDateLog entity);

        Task<Tuple<IEnumerable<SysTenantExDateLog>, int>> GetSysTenantExDateLogPaging(AgentPagingBase request);
    }
}
