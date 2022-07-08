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
        private readonly IActivityRoute2DAL _activityRoute2DAL;

        public ActivityRouteDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider,
            IActivityRoute2DAL activityRoute2DAL) : base(dbWrapper, cacheProvider)
        {
            this._activityRoute2DAL = activityRoute2DAL;
        }

        public override void InitTenantId(int tenantId)
        {
            base.InitTenantId(tenantId);
            this._activityRoute2DAL.InitTenantId(tenantId);
        }

        public override void ResetTenantId(int tenantId)
        {
            base.ResetTenantId(tenantId);
            this._activityRoute2DAL.ResetTenantId(tenantId);
        }

        protected override async Task<ActivityRouteBucket> GetDb(params object[] keys)
        {
            var id = keys[1].ToLong();
            var activityRoute = await _dbWrapper.Find<EtActivityRoute>(
                p => p.Id == id && p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal
                && p.RouteStatus == EmActivityRouteStatus.Normal);
            if (activityRoute == null)
            {
                return null;
            }

            var activityRouteItems = await _dbWrapper.FindListMini<EtActivityRouteItem>(
                p => p.ActivityRouteId == id && p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal
                && p.RouteStatus == EmActivityRouteStatus.Normal && p.PayStatus != EmActivityRoutePayStatus.Refunded);

            List<EtActivityHaggleLog> activityHaggleLogs = null;
            if (activityRoute.ActivityType == EmActivityType.Haggling)
            {
                activityHaggleLogs = await _dbWrapper.FindListMini<EtActivityHaggleLog>(
                    p => p.ActivityRouteId == id && p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal);
            }
            return new ActivityRouteBucket()
            {
                ActivityRoute = activityRoute,
                ActivityRouteItems = activityRouteItems,
                ActivityHaggleLogs = activityHaggleLogs
            };
        }

        #region 无效数据处理

        public async Task<EtActivityRouteItem> GetActivityRouteItemTemp(long id)
        {
            return await this._dbWrapper.Find<EtActivityRouteItem>(p => p.Id == id);
        }

        public async Task<EtActivityRoute> GetActivityRouteTemp(long routeId)
        {
            return await this._dbWrapper.Find<EtActivityRoute>(p => p.Id == routeId);
        }

        public async Task UpdateActivityRouteAboutPayFinishTemp(long routeId, DateTime payTime)
        {
            await _dbWrapper.Execute($"UPDATE EtActivityRoute SET RouteStatus = {EmActivityRouteStatus.Normal},CountFinish = CountFinish + 1,PayStatus = {EmActivityRoutePayStatus.Paid},PayFinishTime = '{payTime.EtmsToString()}' WHERE Id = {routeId}");
        }

        public async Task UpdateActivityRouteItemAboutPayFinishTemp(long itemId, DateTime payTime)
        {
            await _dbWrapper.Execute($"UPDATE EtActivityRouteItem SET RouteStatus = {EmActivityRouteStatus.Normal},PayStatus = {EmActivityRoutePayStatus.Paid},PayFinishTime = '{payTime.EtmsToString()}' WHERE Id = {itemId}");
        }

        public async Task UpdateActivityRouteAboutRefundTemp(long routeId)
        {
            await _dbWrapper.Execute($"UPDATE EtActivityRoute SET CountFinish = CountFinish - 1,PayStatus = {EmActivityRoutePayStatus.Refunded} WHERE Id = {routeId}");
            RemoveCache(_tenantId, routeId);
            this._activityRoute2DAL.RemoveCache(routeId);
        }

        public async Task UpdateActivityRouteItemAboutRefundTemp(long itemId, long routeId)
        {
            await _dbWrapper.Execute($"UPDATE EtActivityRouteItem SET PayStatus = {EmActivityRoutePayStatus.Refunded} WHERE Id = {itemId}");
            RemoveCache(_tenantId, routeId);
            this._activityRoute2DAL.RemoveCache(routeId);
        }

        #endregion

        public async Task<EtActivityRoute> GetActivityRoute(long id)
        {
            return await this._activityRoute2DAL.GetActivityRoute(id);
        }

        public async Task<EtActivityRouteItem> GetActivityRouteItem(long id)
        {
            return await _dbWrapper.Find<EtActivityRouteItem>(p => p.Id == id && p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal
            && p.RouteStatus == EmActivityRouteStatus.Normal);
        }

        public async Task<ActivityRouteBucket> GetActivityRouteBucket(long activityRouteId)
        {
            return await GetCache(_tenantId, activityRouteId);
        }

        public async Task<IEnumerable<EtActivityRoute>> ActivityRouteTop10(long activityId)
        {
            var sql = $"SELECT TOP 10 * FROM EtActivityRoute WHERE TenantId = {_tenantId} AND ActivityId = {activityId} AND RouteStatus = {EmActivityRouteStatus.Normal} AND IsDeleted = {EmIsDeleted.Normal} ORDER BY CountFinish DESC";
            return await _dbWrapper.ExecuteObject<EtActivityRoute>(sql);
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
            this._activityRoute2DAL.RemoveCache(activityRouteId);
        }

        public async Task SetActivityRouteItemStatus(long activityRouteId, int newStatus)
        {
            var sql = $"UPDATE EtActivityRouteItem SET [Status] = {newStatus} WHERE ActivityRouteId = {activityRouteId} AND TenantId = {_tenantId}";
            await _dbWrapper.Execute(sql);
        }

        public async Task SetFinishCountAndStatus(long activityRouteId, int countFinish, int newStatus)
        {
            await _dbWrapper.Execute(
                $"UPDATE EtActivityRoute SET CountFinish = {countFinish},[Status] = {newStatus}WHERE Id = {activityRouteId} AND TenantId = {_tenantId}");
            RemoveCache(_tenantId, activityRouteId);
            this._activityRoute2DAL.RemoveCache(activityRouteId);
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
            var sql = string.Empty;
            if (activityType == EmActivityType.GroupPurchase)
            {
                sql = $"SELECT COUNT(0) FROM EtActivityRoute WHERE TenantId = {_tenantId} AND ActivityId = {activityId} AND RouteStatus = {EmActivityRouteStatus.Normal} AND IsDeleted = {EmIsDeleted.Normal} AND [Status] == {EmSysActivityRouteItemStatus.FinishItem}";
            }
            else
            {
                sql = $"SELECT COUNT(0) FROM EtActivityRoute WHERE TenantId = {_tenantId} AND ActivityId = {activityId} AND RouteStatus = {EmActivityRouteStatus.Normal} AND IsDeleted = {EmIsDeleted.Normal} AND CountFinish >= CountLimit";
            }
            var obj = await _dbWrapper.ExecuteScalar(sql);
            return obj.ToInt();
        }

        public async Task<int> GetFinishFullCount(long activityId)
        {
            var sql = $"SELECT COUNT(0) FROM EtActivityRoute WHERE TenantId = {_tenantId} AND ActivityId = {activityId} AND RouteStatus = {EmActivityRouteStatus.Normal} AND IsDeleted = {EmIsDeleted.Normal} AND [Status] == {EmSysActivityRouteItemStatus.FinishFull}";
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
                sql = $"SELECT COUNT(0) FROM EtActivityRouteItem WHERE TenantId = {_tenantId} AND ActivityRouteId = {activityRouteId} AND RouteStatus = {EmActivityRouteStatus.Normal} AND PayStatus <> {EmActivityRoutePayStatus.Refunded} AND IsDeleted = {EmIsDeleted.Normal}";
            }
            else
            {
                sql = $"SELECT COUNT(0) FROM EtActivityHaggleLog WHERE TenantId = {_tenantId} AND ActivityRouteId = {activityRouteId} AND IsDeleted = {EmIsDeleted.Normal}";
            }
            var obj = await _dbWrapper.ExecuteScalar(sql);
            return obj.ToInt();
        }

        public async Task SyncActivityBascInfo(EtActivityMain bascInfo)
        {
            var sql = $"UPDATE EtActivityRoute SET ActivityTenantName='{bascInfo.TenantName}',ActivityName = '{bascInfo.Name}',ActivityCoverImage = '{bascInfo.CoverImage}',ActivityTitle = '{bascInfo.Title}' WHERE TenantId = {_tenantId} AND ActivityId = {bascInfo.Id} AND IsDeleted = {EmIsDeleted.Normal}";
            await _dbWrapper.Execute(sql);
            sql = $"UPDATE EtActivityRouteItem SET ActivityTenantName='{bascInfo.TenantName}',ActivityName = '{bascInfo.Name}',ActivityCoverImage = '{bascInfo.CoverImage}',ActivityTitle = '{bascInfo.Title}' WHERE TenantId = {_tenantId} AND ActivityId = {bascInfo.Id} AND IsDeleted = {EmIsDeleted.Normal}";
            await _dbWrapper.Execute(sql);
        }

        public async Task<bool> ExistActivity(long activityId)
        {
            var sql = $"SELECT TOP 1 0 FROM EtActivityRoute WHERE TenantId = {_tenantId} AND ActivityId = {activityId} AND IsDeleted = {EmIsDeleted.Normal} AND RouteStatus = {EmActivityRouteStatus.Normal}";
            var obj = await _dbWrapper.ExecuteScalar(sql);
            return obj != null;
        }

        public async Task UpdateActivityRouteTag(long id, string newTag)
        {
            await _dbWrapper.Execute($"UPDATE EtActivityRoute SET Tag = '{newTag}' WHERE Id = {id}");
        }

        public async Task UpdateActivityRouteItemTag(long id, string newTag)
        {
            await _dbWrapper.Execute($"UPDATE EtActivityRouteItem SET Tag = '{newTag}' WHERE Id = {id}");
        }

        public async Task<EtActivityRouteItem> GetEtActivityRouteItemByUserId(long activityId, long miniPgmUserId)
        {
            return await _dbWrapper.Find<EtActivityRouteItem>(
                p => p.TenantId == _tenantId && p.ActivityId == activityId && p.MiniPgmUserId == miniPgmUserId
                && p.IsDeleted == EmIsDeleted.Normal && p.RouteStatus == EmActivityRouteStatus.Normal);
        }

        public async Task UpdateActivityRouteRouteStatus(long routeId, byte newRouteStatus)
        {
            var sql = $"UPDATE EtActivityRoute SET RouteStatus = {newRouteStatus} WHERE TenantId = {_tenantId} AND Id = {routeId}";
            await _dbWrapper.Execute(sql);
            RemoveCache(_tenantId, routeId);
            this._activityRoute2DAL.RemoveCache(routeId);
        }

        public async Task UpdateActivityRouteItemRouteStatus(long routeId, long routeItemId, byte newRouteStatus)
        {
            var sql = $"UPDATE EtActivityRouteItem SET RouteStatus = {newRouteStatus} WHERE TenantId = {_tenantId} AND Id = {routeItemId}";
            await _dbWrapper.Execute(sql);
            RemoveCache(_tenantId, routeId);
            this._activityRoute2DAL.RemoveCache(routeId);
        }

        public async Task<EtActivityHaggleLog> GetActivityHaggleLog(long activityId, long routeId, long miniPgmUserId)
        {
            var sql = $"SELECT TOP 1 * FROM EtActivityHaggleLog WHERE TenantId = {_tenantId} AND ActivityId = {activityId} AND ActivityRouteId = {routeId} AND MiniPgmUserId = {miniPgmUserId} AND IsDeleted = {EmIsDeleted.Normal} ORDER BY Id DESC";
            var log = await _dbWrapper.ExecuteObject<EtActivityHaggleLog>(sql);
            return log.FirstOrDefault();
        }

        public async Task UpdateActivityRouteShareQRCodeInfo(long id, string shareQRCode)
        {
            var sql = $"UPDATE EtActivityRoute SET ShareQRCode = '{shareQRCode}' WHERE Id = {id} AND TenantId = {_tenantId}";
            await _dbWrapper.Execute(sql);
            RemoveCache(_tenantId, id);
            this._activityRoute2DAL.RemoveCache(id);
        }

        public async Task UpdateActivityRouteItemShareQRCodeInfo(long id, string shareQRCode)
        {
            var sql = $"UPDATE EtActivityRouteItem SET ShareQRCode = '{shareQRCode}' WHERE Id = {id} AND TenantId = {_tenantId}";
            await _dbWrapper.Execute(sql);
        }

        public async Task UpdateActivityRoutePayOrder(long id, string payOrderNo, string payUuid)
        {
            var sql = $"UPDATE EtActivityRoute SET PayOrderNo = '{payOrderNo}',PayUuid = '{payUuid}' WHERE Id = {id} AND TenantId = {_tenantId}";
            await _dbWrapper.Execute(sql);
        }

        public async Task UpdateActivityRouteItemPayOrder(long id, string payOrderNo, string payUuid)
        {
            var sql = $"UPDATE EtActivityRouteItem SET PayOrderNo = '{payOrderNo}',PayUuid = '{payUuid}' WHERE Id = {id} AND TenantId = {_tenantId}";
            await _dbWrapper.Execute(sql);
        }
    }
}
