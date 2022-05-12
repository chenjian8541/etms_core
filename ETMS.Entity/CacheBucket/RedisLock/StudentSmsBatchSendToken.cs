using ETMS.Entity.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket.RedisLock
{
    public class StudentSmsBatchSendToken : IRedisToken
    {
        public StudentSmsBatchSendToken(int tenantId)
        {
            this.TenantId = tenantId;
        }

        public int TenantId { get; set; }

        public TimeSpan TimeOut { get; set; } = TimeSpan.FromMinutes(5);

        public string GetKey()
        {
            return $"StudentSmsBatchSendToken_{TenantId}";
        }
    }
}
