using ETMS.Entity.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket.RedisLock
{
    public class StudentCourseDetailAnalyzeToken : IRedisToken
    {
        public StudentCourseDetailAnalyzeToken(int tenantId, long studentId, long CourseId)
        {
            this.TenantId = tenantId;
            this.StudentId = studentId;
            this.CourseId = CourseId;
        }
        public int TenantId { get; set; }

        public long StudentId { get; set; }

        public long CourseId { get; set; }

        public TimeSpan TimeOut { get; set; } = TimeSpan.FromSeconds(10);

        public string GetKey()
        {
            return $"StudentCourseDetailAnalyzeToken_{TenantId}_{StudentId}_{CourseId}";
        }
    }
}
