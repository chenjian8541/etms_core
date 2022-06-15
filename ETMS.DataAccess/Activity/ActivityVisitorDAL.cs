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
    public class ActivityVisitorDAL : DataAccessBase<ActivityVisitorBucket>, IActivityVisitorDAL
    {
        public ActivityVisitorDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        protected override async Task<ActivityVisitorBucket> GetDb(params object[] keys)
        {
            var activityId = keys[1].ToLong();
            var miniPgmUserId = keys[2].ToLong();
            var log = await _dbWrapper.Find<EtActivityVisitor>(p => p.TenantId == _tenantId
            && p.ActivityId == activityId && p.MiniPgmUserId == miniPgmUserId && p.IsDeleted == EmIsDeleted.Normal);
            if (log == null)
            {
                return null;
            }
            return new ActivityVisitorBucket()
            {
                ActivityVisitor = log
            };
        }

        public async Task AddActivityVisitor(EtActivityVisitor entity)
        {
            await this._dbWrapper.Insert(entity);
            await UpdateCache(_tenantId, entity.ActivityId, entity.MiniPgmUserId);
        }

        public async Task<EtActivityVisitor> GetActivityVisitor(long activityId, long miniPgmUserId)
        {
            var bucket = await GetCache(_tenantId, activityId, miniPgmUserId);
            return bucket?.ActivityVisitor;
        }
    }
}
