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
        Task<SysActivityRouteItem> GetSysActivityRouteItem(long tenantId, long routeItemId);

        Task AddSysActivityRouteItem(SysActivityRouteItem entity);

        Task EdiSysActivityRouteItem(SysActivityRouteItem entity);

        Task SyncActivityBascInfo(EtActivityMain bascInfo);

        Task DelSysActivityRouteItemByTenantId(long tenantId);

        Task DelSysActivityRouteItemByActivityId(long tenantId,long activityId);

        Task DelSysActivityRouteItemByRouteItemId(long tenantId, long routeItemId);

        Task<Tuple<IEnumerable<SysActivityRouteItem>, int>> GetPagingRouteItem(IPagingRequest request);

        Task UpdateActivityRouteItemInfo(long tenantId, long activityId, long activityRouteId, int newStatus,int finishCount);

    }
}
