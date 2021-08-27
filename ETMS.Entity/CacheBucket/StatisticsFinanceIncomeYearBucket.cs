using ETMS.Entity.Config;
using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket
{
    public class StatisticsFinanceIncomeYearBucket : ICacheDataContract
    {
        public decimal TotalSumIn { get; set; }

        public decimal TotalSumOut { get; set; }

        public TimeSpan TimeOut { get; } = TimeSpan.FromDays(BucketTimeOutConfig.ComTimeOutDay);

        /// <summary>
        /// 机构+年
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        public string GetKeyFormat(params object[] parms)
        {
            return $"StatisticsFinanceIncomeYearBucket_{parms[0]}_{parms[1]}";
        }
    }
}
