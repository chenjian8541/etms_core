using ETMS.Entity.Config;
using System;
using System.Collections.Generic;
using System.Text;
namespace ETMS.Entity.CacheBucket.RedisLock
{
    public class SyncStudentStudentCourseIdsToken : IRedisToken
    {
        public SyncStudentStudentCourseIdsToken(int tenantId, long studentId)
        {
            this.TenantId = tenantId;
            this.StudentId = studentId;
        }

        public int TenantId { get; set; }

        public long StudentId { get; set; }

        public TimeSpan TimeOut { get; set; } = TimeSpan.FromSeconds(20);

        public string GetKey()
        {
            return $"SyncStudentStudentCourseIdsToken_{TenantId}_{StudentId}";
        }
    }
}