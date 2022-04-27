using ETMS.Entity.Config;
using ETMS.Entity.Database.Source;
using ETMS.Entity.View;
using System;
using System.Collections.Generic;
using System.Text;


namespace ETMS.Entity.CacheBucket
{
    public class TeacherSchooltimeConfigBucket : ICacheDataContract
    {
        public List<EtTeacherSchooltimeConfig> TeacherSchooltimeConfigs { get; set; }

        public List<EtTeacherSchooltimeConfigDetail> EtTeacherSchooltimeConfigDetails { get; set; }

        public TeacherSchooltimeConfigExcludeView TeacherSchooltimeConfigExclude { get; set; }

        public TimeSpan TimeOut { get; } = TimeSpan.FromDays(BucketTimeOutConfig.ComTimeOutDay);

        /// <summary>
        /// TenantId+TeacherId
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        public string GetKeyFormat(params object[] parms)
        {
            return $"TeacherSchooltimeConfigBucket_{parms[0]}_{parms[1]}";
        }
    }
}
