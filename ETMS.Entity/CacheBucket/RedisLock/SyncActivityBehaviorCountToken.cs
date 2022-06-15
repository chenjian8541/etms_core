using ETMS.Entity.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket.RedisLock
{
    public class SyncActivityBehaviorCountToken : IRedisToken
    {
        public SyncActivityBehaviorCountToken(int tenantId, long activityId)
        {
            this.TenantId = tenantId;
            this.ActivityId = activityId;
        }

        public int TenantId { get; set; }

        public long ActivityId { get; set; }

        public TimeSpan TimeOut { get; set; } = TimeSpan.FromSeconds(10);

        public string GetKey()
        {
            return $"SyncActivityBehaviorCountToken_{TenantId}_{ActivityId}";
        }
    }
}
