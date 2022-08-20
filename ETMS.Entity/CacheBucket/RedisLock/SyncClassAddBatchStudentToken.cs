using ETMS.Entity.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket.RedisLock
{
    public class SyncClassAddBatchStudentToken : IRedisToken
    {
        public SyncClassAddBatchStudentToken(int tenantId, long classId)
        {
            this.TenantId = tenantId;
            this.ClassId = classId;
        }

        public int TenantId { get; set; }

        public long ClassId { get; set; }

        public TimeSpan TimeOut { get; set; } = TimeSpan.FromSeconds(40);

        public string GetKey()
        {
            return $"SyncClassAddBatchStudentToken_{TenantId}_{ClassId}";
        }
    }
}
