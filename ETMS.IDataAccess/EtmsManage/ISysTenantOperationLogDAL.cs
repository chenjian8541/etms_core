using ETMS.Entity.Database.Manage;
using ETMS.Entity.EtmsManage.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.EtmsManage
{
    public interface ISysTenantOperationLogDAL
    {
        Task AddSysTenantOperationLog(SysTenantOperationLog entity);

        Task AddSysTenantOperationLog(List<SysTenantOperationLog> entitys);

        Task<Tuple<IEnumerable<SysTenantOperationLog>, int>> GetPaging(AgentPagingBase request);
    }
}
