using ETMS.Entity.Database.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.EtmsManage.Statistics
{
    public interface ISysTenantStatisticsWeekDAL
    {
        Task<SysTenantStatisticsWeek> GetSysTenantStatisticsWeek(int tenantId);

        Task SaveSysTenantStatisticsWeek(SysTenantStatisticsWeek entity);
    }
}
