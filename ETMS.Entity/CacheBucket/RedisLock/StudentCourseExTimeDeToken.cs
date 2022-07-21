using ETMS.Entity.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket.RedisLock
{
    public class StudentCourseExTimeDeToken : IRedisToken
    {
        public StudentCourseExTimeDeToken(int tenantId,long studentId,long courseId)
        {
            this.TenantId = tenantId;
            this.StudentId = studentId;
            this.CourseId = courseId;
        }

        public int TenantId { get; set; }

        public long StudentId { get; set; }

        public long CourseId { get; set; }

        public TimeSpan TimeOut { get; set; } = TimeSpan.FromSeconds(60);

        public string GetKey()
        {
            return $"StudentCourseExTimeDeToken_{TenantId}_{StudentId}_{CourseId}";
        }
    }
}
