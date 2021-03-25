using ETMS.Entity.Config;
using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket
{
    public class StudentAccountRechargeBinderBucket : ICacheDataContract
    {
        public EtStudentAccountRechargeBinder StudentAccountRechargeBinder { get; set; }

        public TimeSpan TimeOut { get; } = TimeSpan.FromDays(BucketTimeOutConfig.ComTimeOutDay);

        /// <summary>
        /// 机构+学员ID
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        public string GetKeyFormat(params object[] parms)
        {
            return $"StudentAccountRechargeBinderBucket_{parms[0]}_{parms[1]}";
        }
    }
}