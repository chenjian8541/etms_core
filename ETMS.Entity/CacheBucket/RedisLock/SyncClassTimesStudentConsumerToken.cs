using ETMS.Entity.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket.RedisLock
{
    public class SyncClassTimesStudentConsumerToken: IRedisToken
    {
        public SyncClassTimesStudentConsumerToken(int tenantId, long classTimesId)
        {
            this.TenantId = tenantId;
            this.ClassTimesId = classTimesId;
        }

        public int TenantId { get; set; }

        public long ClassTimesId { get; set; }

        public TimeSpan TimeOut { get; set; } = TimeSpan.FromSeconds(20);

        public string GetKey()
        {
            return $"SyncClassTimesStudentConsumerToken_{TenantId}_{ClassTimesId}";
        }
    }
}
