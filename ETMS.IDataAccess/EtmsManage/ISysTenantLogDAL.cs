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
        Task AddSysTenantEtmsAccountLog(SysTenantEtmsAccountLog entity);

        Task AddSysTenantSmsLog(SysTenantSmsLog entity);

        Task<Tuple<IEnumerable<SysTenantEtmsAccountLogView>, int>> GetTenantEtmsAccountLogPaging(AgentPagingBase request);

        Task<Tuple<IEnumerable<SysTenantSmsLogVew>, int>> GetTenantSmsLogPaging(AgentPagingBase request);

        Task<List<SysTenantEtmsAccountLog>> GetTenantEtmsAccountLog(int tenantId, int agentId, int versionId);
    }
}
