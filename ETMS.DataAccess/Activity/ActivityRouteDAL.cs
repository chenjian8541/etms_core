using ETMS.DataAccess.Core;
using ETMS.Entity.CacheBucket;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.ICache;
using ETMS.IDataAccess;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ETMS.Entity.Temp;
using ETMS.Entity.View.OnlyOneFiled;
using ETMS.Entity.View;
using ETMS.IDataAccess.Activity;
using ETMS.Entity.Enum.EtmsManage;

namespace ETMS.DataAccess.Activity
{
    public class ActivityRouteDAL : DataAccessBase<ActivityRouteBucket>, IActivityRouteDAL
    {
        public ActivityRouteDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        protected override async Task<ActivityRouteBucket> GetDb(params object[] keys)
        {
            var id = keys[1].ToLong();
            var activityRoute = await _dbWrapper.Find<EtActivityRoute>(
                p => p.Id == id && p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal);
            if (activityRoute == null)
            {
                return null;
            }

            var activityRouteItems = await _dbWrapper.FindListMiddle<EtActivityRouteItem>(
                p => p.ActivityRouteId == id && p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal
                && p.RouteStatus == EmActivityRouteStatus.Normal);

            List<EtActivityHaggleLog> activityHaggleLogs = null;
            if (activityRoute.ActivityType == EmActivityType.Haggling)
            {
                activityHaggleLogs = await _dbWrapper.FindListShort<EtActivityHaggleLog>(
                    p => p.ActivityRouteId == id && p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal);
            }
            return new ActivityRouteBucket()
            {
                ActivityRoute = activityRoute,
                ActivityRouteItems = activityRouteItems,
                ActivityHaggleLogs = activityHaggleLogs
            };
        }

        public async Task<EtActivityRoute> GetActivityRouteDb(long activityRouteId)
        {
            return await _dbWrapper.Find<EtActivityRoute>(p => p.Id == activityRouteId && p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal);
        }

        public async Task<ActivityRouteBucket> GetActivityRouteBucket(long activityRouteId)
        {
            return await GetCache(_tenantId, activityRouteId);
        }

        public async Task<Tuple<IEnumerable<EtActivityRoute>, int>> GetPagingRoute(IPagingRequest request)
        {
            return await _dbWrapper.ExecutePage<EtActivityRoute>("EtActivityRoute", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }

        public async Task<Tuple<IEnumerable<EtActivityRouteItem>, int>> GetPagingRouteItem(IPagingRequest request)
        {
            return await _dbWrapper.ExecutePage<EtActivityRouteItem>("EtActivityRouteItem", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }

        public async Task AddActivityRoute(EtActivityRoute entity)
        {
            await _dbWrapper.Insert(entity);
        }

        /// <summary>
        /// 临时增加一个，然后异步去同步
        /// </summary>
        /// <param name="activityRouteId"></param>
        /// <returns></returns>
        public async Task TempAddActivityRouteCount(long activityRouteId)
        {
            await _dbWrapper.Execute(
                $"UPDATE EtActivityRoute SET CountFinish = CountFinish + 1 WHERE Id = {activityRouteId} AND TenantId = {_tenantId}");
            RemoveCache(_tenantId, activityRouteId);
        }

        public async Task SetFinishCount(long activityRouteId, int countFinish)
        {
            await _dbWrapper.Execute(
                $"UPDATE EtActivityRoute SET CountFinish = {countFinish} WHERE Id = {activityRouteId} AND TenantId = {_tenantId}");
            RemoveCache(_tenantId, activityRouteId);
        }

        public async Task AddActivityRouteItem(EtActivityRouteItem entity)
        {
            await _dbWrapper.Insert(entity);
            RemoveCache(_tenantId, entity.ActivityRouteId);
        }

        public async Task AddActivityHaggleLog(EtActivityHaggleLog entity)
        {
            await _dbWrapper.Insert(entity);
            await UpdateCache(_tenantId, entity.ActivityRouteId);
        }

        public async Task<int> GetJoinCount(long activityId, int activityType)
        {
            var sql = string.Empty;
            if (activityType == EmActivityType.GroupPurchase)
            {
                sql = $"SELECT COUNT(0) FROM EtActivityRouteItem WHERE TenantId = {_tenantId} AND ActivityId = {activityId} AND RouteStatus = {EmActivityRouteStatus.Normal} AND IsDeleted = {EmIsDeleted.Normal}";
            }
            else
            {
                sql = $"SELECT COUNT(0) FROM EtActivityRoute WHERE TenantId = {_tenantId} AND ActivityId = {activityId} AND RouteStatus = {EmActivityRouteStatus.Normal} AND IsDeleted = {EmIsDeleted.Normal}";
            }
            var obj = await _dbWrapper.ExecuteScalar(sql);
            return obj.ToInt();
        }

        public async Task<int> GetRouteCount(long activityId, int activityType)
        {
            var sql = $"SELECT COUNT(0) FROM EtActivityRoute WHERE TenantId = {_tenantId} AND ActivityId = {activityId} AND RouteStatus = {EmActivityRouteStatus.Normal} AND IsDeleted = {EmIsDeleted.Normal}";
            var obj = await _dbWrapper.ExecuteScalar(sql);
            return obj.ToInt();
        }

        public async Task<int> GetFinishCount(long activityId, int activityType)
        {
            var sql = $"SELECT COUNT(0) FROM EtActivityRoute WHERE TenantId = {_tenantId} AND ActivityId = {activityId} AND RouteStatus = {EmActivityRouteStatus.Normal} AND IsDeleted = {EmIsDeleted.Normal} AND CountFinish >= CountLimit";
            var obj = await _dbWrapper.ExecuteScalar(sql);
            return obj.ToInt();
        }

        public async Task<int> GetActivityRouteFinishCount(long activityRouteId, int activityType)
        {
            if (activityType != EmActivityType.GroupPurchase && activityType != EmActivityType.Haggling)
            {
                return 0;
            }

            var sql = string.Empty;
            if (activityType == EmActivityType.GroupPurchase)
            {
                sql = $"SELECT COUNT(0) FROM EtActivityRouteItem WHERE TenantId = {_tenantId} AND ActivityRouteId = {activityRouteId} AND RouteStatus = {EmActivityRouteStatus.Normal} AND IsDeleted = {EmIsDeleted.Normal}";
            }
            else
            {
                sql = $"SELECT COUNT(0) FROM EtActivityHaggleLog WHERE TenantId = {_tenantId} AND ActivityRouteId = {activityRouteId} AND IsDeleted = {EmIsDeleted.Normal}";
            }
            var obj = await _dbWrapper.ExecuteScalar(sql);
            return obj.ToInt();
        }
    }
}
