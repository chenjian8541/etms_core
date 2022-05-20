using ETMS.Entity.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket.RedisLock
{
    public class ParentStudentReservationSubmitToken : IRedisToken
    {
        public ParentStudentReservationSubmitToken(int tenantId, long classTimesId)
        {
            this.TenantId = tenantId;
            this.ClassTimesId = classTimesId;
        }

        public int TenantId { get; set; }

        public long ClassTimesId { get; set; }

        public TimeSpan TimeOut { get; set; } = TimeSpan.FromSeconds(60);

        public string GetKey()
        {
            return $"ParentStudentReservationSubmitToken_{TenantId}_{ClassTimesId}";
        }
    }
}
