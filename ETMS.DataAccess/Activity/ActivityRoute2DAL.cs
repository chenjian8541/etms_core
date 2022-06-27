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
    public class ActivityRoute2DAL : DataAccessBase<ActivityRouteSimpleBucket>, IActivityRoute2DAL
    {
        public ActivityRoute2DAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        protected override async Task<ActivityRouteSimpleBucket> GetDb(params object[] keys)
        {
            var id = keys[1].ToLong();
            var activityRoute = await _dbWrapper.Find<EtActivityRoute>(
                p => p.Id == id && p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal
                && p.RouteStatus == EmActivityRouteStatus.Normal);
            if (activityRoute == null)
            {
                return null;
            }
            return new ActivityRouteSimpleBucket()
            {
                ActivityRoute = activityRoute
            };
        }

        public void RemoveCache(long activityRouteId)
        {
            RemoveCache(_tenantId, activityRouteId);
        }

        public async Task<EtActivityRoute> GetActivityRoute(long activityRouteId)
        {
            var bucket = await GetCache(_tenantId, activityRouteId);
            return bucket?.ActivityRoute;
        }
    }
}
