using ETMS.Entity.Config;
using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket
{
    /// <summary>
    /// 学员  课程剩余信息
    /// </summary>
    public class StudentCourseBucket : ICacheDataContract
    {
        public List<EtStudentCourse> StudentCourses { get; set; }

        public TimeSpan TimeOut { get; } = TimeSpan.FromDays(BucketTimeOutConfig.ComTimeOutDay);

        /// <summary>
        /// 机构_学员
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        public string GetKeyFormat(params object[] parms)
        {
            return $"StudentCourseBucket_{parms[0]}_{parms[1]}";
        }
    }
}
