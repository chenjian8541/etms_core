using ETMS.Entity.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.CacheBucket
{
    public class TeacherSalaryMonthStatisticsBucket : ICacheDataContract
    {
        public decimal TotalPayItemSum { get; set; }

        public TimeSpan TimeOut { get; } = TimeSpan.FromDays(BucketTimeOutConfig.ComTimeOutDay);

        /// <summary>
        /// 机构+员工+日期(年月)
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        public string GetKeyFormat(params object[] parms)
        {
            var ot = Convert.ToDateTime(parms[2]);
            var otKey = ot.ToString("yyyyMM");
            return $"TeacherSalaryMonthStatisticsBucket_{parms[0]}_{parms[1]}_{otKey}";
        }
    }
}
