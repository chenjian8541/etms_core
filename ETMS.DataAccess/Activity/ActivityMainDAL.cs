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

namespace ETMS.DataAccess.Activity
{
    public class ActivityMainDAL : DataAccessBase<ActivityMainBucket>, IActivityMainDAL
    {
        public ActivityMainDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        protected override async Task<ActivityMainBucket> GetDb(params object[] keys)
        {
            var id = keys[1].ToLong();
            var log = await _dbWrapper.Find<EtActivityMain>(p => p.Id == id && p.TenantId == _tenantId
            && p.IsDeleted == EmIsDeleted.Normal);
            if (log == null)
            {
                return null;
            }
            return new ActivityMainBucket()
            {
                ActivityMain = log
            };
        }

        public async Task AddActivityMain(EtActivityMain entity)
        {
            await this._dbWrapper.Insert(entity);
        }

        public async Task UpdateActivityMainShareQRCode(long id, string shareQRCode)
        {
            await _dbWrapper.Execute($"UPDATE EtActivityMain SET ShareQRCode = '{shareQRCode}' WHERE Id = {id}");
            await UpdateCache(_tenantId, id);
        }

        public async Task UpdateActivityMainStatus(long id, int newActivityStatus)
        {
            await _dbWrapper.Execute($"UPDATE EtActivityMain SET ActivityStatus = {newActivityStatus} WHERE Id = {id}");
            await UpdateCache(_tenantId, id);
        }

        public async Task UpdateActivityMainIsShowInParent(long id, bool isShowInParent)
        {
            var newShowInParent = isShowInParent ? 1 : 0;
            await _dbWrapper.Execute($"UPDATE EtActivityMain SET IsShowInParent = {newShowInParent} WHERE Id = {id}");
            await UpdateCache(_tenantId, id);
        }

        public async Task EditActivityMain(EtActivityMain entity)
        {
            await this._dbWrapper.Update(entity);
            await UpdateCache(_tenantId, entity.Id);
        }

        public async Task<EtActivityMain> GetActivityMain(long id)
        {
            var bucket = await GetCache(_tenantId, id);
            return bucket?.ActivityMain;
        }

        public async Task<Tuple<IEnumerable<EtActivityMain>, int>> GetPaging(IPagingRequest request)
        {
            return await _dbWrapper.ExecutePage<EtActivityMain>("EtActivityMain", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }

        public async Task AddBehaviorCount(long activityId, int addPVCount, int addUVCount, int addTranspondCount, int addVisitCount)
        {
            var str = new StringBuilder("UPDATE EtActivityMain SET");
            if (addPVCount > 0)
            {
                str.Append($" PVCount = PVCount + {addPVCount},");
            }
            if (addUVCount > 0)
            {
                str.Append($" UVCount = UVCount + {addUVCount},");
            }
            if (addTranspondCount > 0)
            {
                str.Append($" TranspondCount = TranspondCount + {addTranspondCount},");
            }
            if (addVisitCount > 0)
            {
                str.Append($" VisitCount = VisitCount + {addVisitCount},");
            }
            await _dbWrapper.Execute($"{str.ToString().TrimEnd(',')} WHERE Id = {activityId}");
            RemoveCache(_tenantId, activityId);
        }

        public async Task SetEffectCount(long activityId, int joinCount, int routeCount, int finishCount, int finishFullCount)
        {
            var sql = $"UPDATE EtActivityMain SET JoinCount = {joinCount},RouteCount = {routeCount}, FinishCount = {finishCount},FinishFullCount = {finishFullCount} WHERE Id = {activityId}";
            await _dbWrapper.Execute(sql);
        }

        public async Task DelActivityMain(long activityId)
        {
            var sql = new StringBuilder();
            sql.Append($"UPDATE EtActivityMain SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId} AND Id = {activityId};");
            sql.Append($"UPDATE EtActivityRoute SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId} AND ActivityId = {activityId};");
            sql.Append($"UPDATE EtActivityRouteItem SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId} AND ActivityId = {activityId};");
            await _dbWrapper.Execute(sql.ToString());
            RemoveCache(_tenantId, activityId);
        }
    }
}
