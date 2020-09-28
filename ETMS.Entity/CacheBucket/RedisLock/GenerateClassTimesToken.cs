using ETMS.Entity.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket.RedisLock
{
    public class GenerateClassTimesToken : IRedisToken
    {
        public GenerateClassTimesToken(int tenantId, long classTimesRuleId)
        {
            this.TenantId = tenantId;
            this.ClassTimesRuleId = classTimesRuleId;
        }

        public int TenantId { get; set; }

        public long ClassTimesRuleId { get; set; }

        public TimeSpan TimeOut { get; set; } = TimeSpan.FromSeconds(20);

        public string GetKey()
        {
            return $"GenerateClassTimesToken_{TenantId}_{ClassTimesRuleId}";
        }
    }
}
