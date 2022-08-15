using ETMS.Entity.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket.RedisLock
{
    public class AutoSyncTenantClassDetailToken : IRedisToken
    {
        public AutoSyncTenantClassDetailToken(int tenantId, long classId)
        {
            this.TenantId = tenantId;
            this.ClassId = classId;
        }

        public int TenantId { get; set; }

        public long ClassId { get; set; }

        public TimeSpan TimeOut { get; set; } = TimeSpan.FromSeconds(30);

        public string GetKey()
        {
            return $"AutoSyncTenantClassDetailToken_{TenantId}_{ClassId}";
        }
    }
}
