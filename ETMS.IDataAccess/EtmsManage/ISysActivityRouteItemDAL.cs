using ETMS.Entity.Common;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.EtmsManage
{
    public interface ISysActivityRouteItemDAL
    {
        Task AddSysActivityRouteItem(SysActivityRouteItem entity);

        Task SyncActivityBascInfo(EtActivityMain bascInfo);

        Task DelSysActivityRouteItemByTenantId(long tenantId);

        Task DelSysActivityRouteItemByActivityId(long tenantId,long activityId);

        Task<Tuple<IEnumerable<SysActivityRouteItem>, int>> GetPagingRouteItem(IPagingRequest request);
    }
}
