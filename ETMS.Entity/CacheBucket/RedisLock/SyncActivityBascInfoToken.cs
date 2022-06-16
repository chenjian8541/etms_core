using ETMS.Entity.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket.RedisLock
{
    public class SyncActivityBascInfoToken : IRedisToken
    {
        public SyncActivityBascInfoToken(int tenantId, long activityId)
        {
            this.TenantId = tenantId;
            this.ActivityId = activityId;
        }

        public int TenantId { get; set; }

        public long ActivityId { get; set; }

        public TimeSpan TimeOut { get; set; } = TimeSpan.FromSeconds(20);

        public string GetKey()
        {
            return $"SyncActivityBascInfoToken_{TenantId}_{ActivityId}";
        }
    }
}