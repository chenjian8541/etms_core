using ETMS.Entity.CacheBucket;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.Activity
{
    public interface IActivityRouteDAL : IBaseDAL
    {
        Task<EtActivityRoute> GetActivityRouteDb(long activityRouteId);

        Task<ActivityRouteBucket> GetActivityRouteBucket(long activityRouteId);

        Task<Tuple<IEnumerable<EtActivityRoute>, int>> GetPagingRoute(IPagingRequest request);

        Task<Tuple<IEnumerable<EtActivityRouteItem>, int>> GetPagingRouteItem(IPagingRequest request);

        Task AddActivityRoute(EtActivityRoute entity);

        /// <summary>
        /// 临时增加一个，然后异步去同步
        /// </summary>
        /// <param name="activityRouteId"></param>
        /// <returns></returns>
        Task TempAddActivityRouteCount(long activityRouteId);

        Task SetFinishCount(long activityRouteId, int countFinish);

        Task AddActivityRouteItem(EtActivityRouteItem entity);

        Task AddActivityHaggleLog(EtActivityHaggleLog entity);

        Task<int> GetJoinCount(long activityId,int activityType);

        Task<int> GetRouteCount(long activityId, int activityType);

        Task<int> GetFinishCount(long activityId, int activityType);

        Task<int> GetActivityRouteFinishCount(long activityRouteId, int activityType);

        Task SyncActivityBascInfo(EtActivityMain bascInfo);

        Task<bool> ExistActivity(long activityId);
    }
}
