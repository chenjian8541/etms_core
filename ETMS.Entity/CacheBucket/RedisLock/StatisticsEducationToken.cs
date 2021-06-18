using ETMS.Entity.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket.RedisLock
{
    public class StatisticsEducationToken : IRedisToken
    {
        public StatisticsEducationToken(int tenantId)
        {
            this.TenantId = tenantId;
        }

        public int TenantId { get; set; }

        public TimeSpan TimeOut { get; set; } = TimeSpan.FromSeconds(60);

        public string GetKey()
        {
            return $"StatisticsEducationToken_{TenantId}";
        }
    }
}
