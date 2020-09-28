using ETMS.Entity.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket
{
    public class ParentStudentBucket : ICacheDataContract
    {
        public string Phone { get; set; }

        public IEnumerable<ParentStudentInfo> ParentStudents { get; set; }

        public TimeSpan TimeOut { get; set; } = TimeSpan.FromDays(BucketTimeOutConfig.ParentStudentTimeOutDay);

        public string GetKeyFormat(params object[] parms)
        {
            return $"ParentStudentBucket_{parms[0]}_{parms[1]}";
        }
    }

    public class ParentStudentInfo
    {
        public long Id { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// 性别  <see cref="ETMS.Entity.Enum.EmGender"/>
        /// </summary>
        public byte? Gender { get; set; }

        public string Avatar { get; set; }
    }
}
