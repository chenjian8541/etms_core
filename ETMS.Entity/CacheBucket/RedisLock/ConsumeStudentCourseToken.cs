using ETMS.Entity.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket.RedisLock
{
    public class ConsumeStudentCourseToken : IRedisToken
    {
        public ConsumeStudentCourseToken(int tenantId, long studentCourseDetailId)
        {
            this.TenantId = tenantId;
            this.StudentCourseDetailId = studentCourseDetailId;
        }

        public int TenantId { get; set; }

        public long StudentCourseDetailId { get; set; }

        public TimeSpan TimeOut { get; set; } = TimeSpan.FromSeconds(20);

        public string GetKey()
        {
            return $"ConsumeStudentCourseToken_{TenantId}_{StudentCourseDetailId}";
        }
    }
}
