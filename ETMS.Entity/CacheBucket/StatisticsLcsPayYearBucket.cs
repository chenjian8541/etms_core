using ETMS.Entity.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.CacheBucket
{
    public class StatisticsLcsPayYearBucket : ICacheDataContract
    {
        public decimal TotalMoney { get; set; }

        public decimal TotalMoneyRefund { get; set; }

        public decimal TotalMoneyValue { get; set; }

        public TimeSpan TimeOut { get; } = TimeSpan.FromDays(BucketTimeOutConfig.ComTimeOutDay);

        /// <summary>
        /// 机构+年
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        public string GetKeyFormat(params object[] parms)
        {
            return $"StatisticsLcsPayYearBucket_{parms[0]}_{parms[1]}";
        }
    }
}
