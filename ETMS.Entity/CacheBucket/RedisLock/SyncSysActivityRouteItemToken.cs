using ETMS.Entity.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket.RedisLock
{
    public class SyncSysActivityRouteItemToken: IRedisToken
    {
        public SyncSysActivityRouteItemToken(int tenantId, long activityRouteItemId)
        {
            this.TenantId = tenantId;
            this.ActivityRouteItemId = activityRouteItemId;
        }

        public int TenantId { get; set; }

        public long ActivityRouteItemId { get; set; }

        public TimeSpan TimeOut { get; set; } = TimeSpan.FromSeconds(30);

        public string GetKey()
        {
            return $"SyncSysActivityRouteItemToken_{TenantId}_{ActivityRouteItemId}";
        }
    }
}
