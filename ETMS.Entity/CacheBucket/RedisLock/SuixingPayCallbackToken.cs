using ETMS.Entity.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket.RedisLock
{
    public class SuixingPayCallbackToken : IRedisToken
    {
        public SuixingPayCallbackToken(int tenantId, long activityRouteId)
        {
            this.TenantId = tenantId;
            this.ActivityRouteItemId = activityRouteId;
        }

        public int TenantId { get; set; }

        public long ActivityRouteItemId { get; set; }

        public TimeSpan TimeOut { get; set; } = TimeSpan.FromSeconds(30);

        public string GetKey()
        {
            return $"SuixingPayCallbackToken_{TenantId}_{ActivityRouteItemId}";
        }
    }
}
