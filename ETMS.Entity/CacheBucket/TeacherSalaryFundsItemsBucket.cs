using ETMS.Entity.Config;
using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.CacheBucket
{
    public class TeacherSalaryFundsItemsBucket : ICacheDataContract
    {
        public List<EtTeacherSalaryFundsItems> TeacherSalaryFundsItemsList { get; set; }

        public TimeSpan TimeOut { get; } = TimeSpan.FromDays(BucketTimeOutConfig.ComTimeOutDay);

        public string GetKeyFormat(params object[] parms)
        {
            return $"TeacherSalaryFundsItemsBucket_{parms[0]}";
        }
    }
}
