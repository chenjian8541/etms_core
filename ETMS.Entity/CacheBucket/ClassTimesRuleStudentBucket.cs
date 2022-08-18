using ETMS.Entity.Config;
using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket
{
    public class ClassTimesRuleStudentBucket : ICacheDataContract
    {
        public List<EtClassTimesRuleStudent> ClassTimesRuleStudents { get; set; }

        public TimeSpan TimeOut { get; } = TimeSpan.FromDays(BucketTimeOutConfig.ComTimeOutDay);

        /// <summary>
        /// 机构id,班级id,排个规则id
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        public string GetKeyFormat(params object[] parms)
        {
            return $"ClassTimesRuleStudentBucket_{parms[0]}_{parms[1]}_{parms[2]}";
        }
    }
}
