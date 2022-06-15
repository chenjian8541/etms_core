using ETMS.Entity.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket.RedisLock
{
    public class SyncActivityRouteFinishCountToken : IRedisToken
    {
        public SyncActivityRouteFinishCountToken(int tenantId, long activityRouteId)
        {
            this.TenantId = tenantId;
            this.ActivityRouteId = activityRouteId;
        }

        public int TenantId { get; set; }

        public long ActivityRouteId { get; set; }

        public TimeSpan TimeOut { get; set; } = TimeSpan.FromSeconds(10);

        public string GetKey()
        {
            return $"SyncActivityRouteFinishCountToken_{TenantId}_{ActivityRouteId}";
        }
    }
}
