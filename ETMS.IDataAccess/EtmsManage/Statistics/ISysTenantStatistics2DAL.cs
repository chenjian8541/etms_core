using ETMS.Entity.Database.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.EtmsManage.Statistics
{
    public interface ISysTenantStatistics2DAL
    {
        Task<SysTenantStatistics2> GetSysTenantStatistics(int tenantId);

        Task SaveSysTenantStatistics2(SysTenantStatistics2 entity);
    }
}
