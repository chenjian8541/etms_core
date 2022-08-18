using ETMS.Entity.Config;
using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket
{
    public class ReservationCourseSetBucket : ICacheDataContract
    {
        public List<EtReservationCourseSet> ReservationCourseSets { get; set; }

        public TimeSpan TimeOut { get; } = TimeSpan.FromDays(BucketTimeOutConfig.ComTimeOutDay);

        public string GetKeyFormat(params object[] parms)
        {
            return $"ReservationCourseSetBucket_{parms[0]}";
        }
    }
}
