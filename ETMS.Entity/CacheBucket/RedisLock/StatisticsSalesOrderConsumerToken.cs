using ETMS.Entity.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket.RedisLock
{
    public class StatisticsSalesOrderConsumerToken : IRedisToken
    {
        public StatisticsSalesOrderConsumerToken(int tenantId)
        {
            this.TenantId = tenantId;
        }

        public int TenantId { get; set; }

        public TimeSpan TimeOut { get; set; } = TimeSpan.FromSeconds(10);

        public string GetKey()
        {
            return $"StatisticsSalesOrderConsumerToken_{TenantId}";
        }
    }
}