using ETMS.Entity.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket.RedisLock
{
    public class SyncClassTimesRuleStudentInfoToken : IRedisToken
    {
        public SyncClassTimesRuleStudentInfoToken(int tenantId, long classId, long ruleId)
        {
            this.TenantId = tenantId;
            this.ClassId = classId;
            this.RuleId = ruleId;
        }

        public long ClassId { get; set; }

        public long RuleId { get; set; }

        public int TenantId { get; set; }

        public TimeSpan TimeOut { get; set; } = TimeSpan.FromSeconds(20);

        public string GetKey()
        {
            return $"SyncClassTimesRuleStudentInfoToken_{TenantId}_{ClassId}_{RuleId}";
        }
    }
}
