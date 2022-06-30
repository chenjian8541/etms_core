using ETMS.Entity.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket.RedisLock
{
    public class ActivityAutoRefundRouteItemToken : IRedisToken
    {
        public ActivityAutoRefundRouteItemToken(int tenantId, long activityRouteItemId)
        {
            this.TenantId = tenantId;
            this.ActivityRouteItemId = activityRouteItemId;
        }

        public int TenantId { get; set; }

        public long ActivityRouteItemId { get; set; }

        public TimeSpan TimeOut { get; set; } = TimeSpan.FromSeconds(60);

        public string GetKey()
        {
            return $"ActivityAutoRefundRouteItemToken_{TenantId}_{ActivityRouteItemId}";
        }
    }
}
