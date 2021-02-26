using ETMS.Entity.Config;
using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket
{
    public class StatisticsStudentAccountRechargeBucket : ICacheDataContract
    {
        public EtStatisticsStudentAccountRecharge StatisticsStudentAccountRecharge { get; set; }

        public TimeSpan TimeOut { get; } = TimeSpan.FromDays(BucketTimeOutConfig.ComTimeOutDay);

        /// <summary>
        /// 机构
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        public string GetKeyFormat(params object[] parms)
        {
            return $"StatisticsStudentAccountRechargeBucket_{parms[0]}";
        }
    }
}
