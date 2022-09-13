using ETMS.Entity.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket.RedisLock
{
    public class StudentCheckChoiceClassToken : IRedisToken
    {
        public StudentCheckChoiceClassToken(int tenantId, long studentCheckOnLogId)
        {
            this.TenantId = tenantId;
            this.StudentCheckOnLogId = studentCheckOnLogId;
        }

        public int TenantId { get; set; }

        public long StudentCheckOnLogId { get; set; }

        public TimeSpan TimeOut { get; set; } = TimeSpan.FromSeconds(20);

        public string GetKey()
        {
            return $"StudentCheckChoiceClassToken_{TenantId}_{StudentCheckOnLogId}";
        }
    }
}
